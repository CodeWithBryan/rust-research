using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x0200092F RID: 2351
public class PrefabPoolWarmup
{
	// Token: 0x06003801 RID: 14337 RVA: 0x0014BC10 File Offset: 0x00149E10
	public static void Run()
	{
		if (Rust.Application.isLoadingPrefabs)
		{
			return;
		}
		Rust.Application.isLoadingPrefabs = true;
		string[] assetList = PrefabPoolWarmup.GetAssetList();
		for (int i = 0; i < assetList.Length; i++)
		{
			PrefabPoolWarmup.PrefabWarmup(assetList[i]);
		}
		Rust.Application.isLoadingPrefabs = false;
	}

	// Token: 0x06003802 RID: 14338 RVA: 0x0014BC4D File Offset: 0x00149E4D
	public static IEnumerator Run(float deltaTime, Action<string> statusFunction = null, string format = null)
	{
		if (UnityEngine.Application.isEditor)
		{
			yield break;
		}
		if (Rust.Application.isLoadingPrefabs)
		{
			yield break;
		}
		if (!Pool.prewarm)
		{
			yield break;
		}
		Rust.Application.isLoadingPrefabs = true;
		string[] prewarmAssets = PrefabPoolWarmup.GetAssetList();
		Stopwatch sw = Stopwatch.StartNew();
		int num;
		for (int i = 0; i < prewarmAssets.Length; i = num + 1)
		{
			if (sw.Elapsed.TotalSeconds > (double)deltaTime || i == 0 || i == prewarmAssets.Length - 1)
			{
				if (statusFunction != null)
				{
					statusFunction(string.Format((format != null) ? format : "{0}/{1}", i + 1, prewarmAssets.Length));
				}
				yield return CoroutineEx.waitForEndOfFrame;
				sw.Reset();
				sw.Start();
			}
			PrefabPoolWarmup.PrefabWarmup(prewarmAssets[i]);
			num = i;
		}
		Rust.Application.isLoadingPrefabs = false;
		yield break;
	}

	// Token: 0x06003803 RID: 14339 RVA: 0x0014BC6C File Offset: 0x00149E6C
	public static string[] GetAssetList()
	{
		return (from x in GameManifest.Current.prefabProperties
		where x.pool
		select x.name).ToArray<string>();
	}

	// Token: 0x06003804 RID: 14340 RVA: 0x0014BCD0 File Offset: 0x00149ED0
	private static void PrefabWarmup(string path)
	{
		if (!string.IsNullOrEmpty(path))
		{
			GameObject gameObject = GameManager.server.FindPrefab(path);
			if (gameObject != null && gameObject.SupportsPooling())
			{
				int serverCount = gameObject.GetComponent<Poolable>().ServerCount;
				List<GameObject> list = new List<GameObject>();
				for (int i = 0; i < serverCount; i++)
				{
					list.Add(GameManager.server.CreatePrefab(path, true));
				}
				for (int j = 0; j < serverCount; j++)
				{
					GameManager.server.Retire(list[j]);
				}
			}
		}
	}
}
