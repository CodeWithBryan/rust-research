using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using ConVar;
using UnityEngine;

// Token: 0x0200033B RID: 827
public class FileSystem_Warmup : MonoBehaviour
{
	// Token: 0x06001E12 RID: 7698 RVA: 0x000CC3D4 File Offset: 0x000CA5D4
	public static void Run()
	{
		if (Global.skipAssetWarmup_crashes)
		{
			return;
		}
		if (!FileSystem_Warmup.run)
		{
			return;
		}
		string[] assetList = FileSystem_Warmup.GetAssetList(null);
		for (int i = 0; i < assetList.Length; i++)
		{
			FileSystem_Warmup.PrefabWarmup(assetList[i]);
		}
		FileSystem_Warmup.run = false;
	}

	// Token: 0x06001E13 RID: 7699 RVA: 0x000CC41C File Offset: 0x000CA61C
	public static IEnumerator Run(string[] assetList, Action<string> statusFunction = null, string format = null, int priority = 0)
	{
		if (Global.warmupConcurrency <= 1)
		{
			return FileSystem_Warmup.RunImpl(assetList, statusFunction, format);
		}
		return FileSystem_Warmup.RunAsyncImpl(assetList, statusFunction, format, priority);
	}

	// Token: 0x06001E14 RID: 7700 RVA: 0x000CC438 File Offset: 0x000CA638
	private static IEnumerator RunAsyncImpl(string[] assetList, Action<string> statusFunction, string format, int priority)
	{
		if (Global.skipAssetWarmup_crashes)
		{
			yield break;
		}
		if (!FileSystem_Warmup.run)
		{
			yield break;
		}
		Stopwatch statusSw = Stopwatch.StartNew();
		Stopwatch sw = Stopwatch.StartNew();
		AssetPreloadResult preload = FileSystem.PreloadAssets(assetList, Global.warmupConcurrency, priority);
		int warmupIndex = 0;
		while (preload.MoveNext() || warmupIndex < preload.TotalCount)
		{
			float num = FileSystem_Warmup.CalculateFrameBudget();
			if (num > 0f)
			{
				while (warmupIndex < preload.Results.Count && sw.Elapsed.TotalSeconds < (double)num)
				{
					IReadOnlyList<ValueTuple<string, UnityEngine.Object>> results = preload.Results;
					int num2 = warmupIndex;
					warmupIndex = num2 + 1;
					FileSystem_Warmup.PrefabWarmup(results[num2].Item1);
				}
			}
			if (warmupIndex == 0 || warmupIndex == preload.TotalCount || statusSw.Elapsed.TotalSeconds > 1.0)
			{
				if (statusFunction != null)
				{
					statusFunction(string.Format(format ?? "{0}/{1}", warmupIndex, preload.TotalCount));
				}
				statusSw.Restart();
			}
			yield return CoroutineEx.waitForEndOfFrame;
			sw.Restart();
		}
		FileSystem_Warmup.run = false;
		yield break;
	}

	// Token: 0x06001E15 RID: 7701 RVA: 0x000CC45C File Offset: 0x000CA65C
	private static IEnumerator RunImpl(string[] assetList, Action<string> statusFunction = null, string format = null)
	{
		if (Global.skipAssetWarmup_crashes)
		{
			yield break;
		}
		if (!FileSystem_Warmup.run)
		{
			yield break;
		}
		Stopwatch sw = Stopwatch.StartNew();
		int num;
		for (int i = 0; i < assetList.Length; i = num + 1)
		{
			if (sw.Elapsed.TotalSeconds > (double)FileSystem_Warmup.CalculateFrameBudget() || i == 0 || i == assetList.Length - 1)
			{
				if (statusFunction != null)
				{
					statusFunction(string.Format((format != null) ? format : "{0}/{1}", i + 1, assetList.Length));
				}
				yield return CoroutineEx.waitForEndOfFrame;
				sw.Reset();
				sw.Start();
			}
			FileSystem_Warmup.PrefabWarmup(assetList[i]);
			num = i;
		}
		FileSystem_Warmup.run = false;
		yield break;
	}

	// Token: 0x06001E16 RID: 7702 RVA: 0x000300D2 File Offset: 0x0002E2D2
	private static float CalculateFrameBudget()
	{
		return 2f;
	}

	// Token: 0x06001E17 RID: 7703 RVA: 0x000CC47C File Offset: 0x000CA67C
	private static bool ShouldIgnore(string path)
	{
		for (int i = 0; i < FileSystem_Warmup.ExcludeFilter.Length; i++)
		{
			if (path.Contains(FileSystem_Warmup.ExcludeFilter[i], CompareOptions.IgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001E18 RID: 7704 RVA: 0x000CC4B0 File Offset: 0x000CA6B0
	public static string[] GetAssetList(bool? poolFilter = null)
	{
		if (poolFilter == null)
		{
			return (from x in GameManifest.Current.prefabProperties
			select x.name into x
			where !FileSystem_Warmup.ShouldIgnore(x)
			select x).ToArray<string>();
		}
		return (from x in GameManifest.Current.prefabProperties.Where(delegate(GameManifest.PrefabProperties x)
		{
			if (!FileSystem_Warmup.ShouldIgnore(x.name))
			{
				bool pool = x.pool;
				bool? poolFilter2 = poolFilter;
				return pool == poolFilter2.GetValueOrDefault() & poolFilter2 != null;
			}
			return false;
		})
		select x.name).Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray<string>();
	}

	// Token: 0x06001E19 RID: 7705 RVA: 0x000CC57D File Offset: 0x000CA77D
	private static void PrefabWarmup(string path)
	{
		GameManager.server.FindPrefab(path);
	}

	// Token: 0x040017D4 RID: 6100
	public static bool ranInBackground = false;

	// Token: 0x040017D5 RID: 6101
	public static Coroutine warmupTask;

	// Token: 0x040017D6 RID: 6102
	private static bool run = true;

	// Token: 0x040017D7 RID: 6103
	public static string[] ExcludeFilter = new string[]
	{
		"/bundled/prefabs/autospawn/monument",
		"/bundled/prefabs/autospawn/mountain",
		"/bundled/prefabs/autospawn/canyon",
		"/bundled/prefabs/autospawn/decor",
		"/bundled/prefabs/navmesh",
		"/content/ui/",
		"/prefabs/ui/",
		"/prefabs/world/",
		"/prefabs/system/",
		"/standard assets/",
		"/third party/"
	};
}
