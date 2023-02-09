using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Token: 0x02000720 RID: 1824
public class GameManifest : ScriptableObject
{
	// Token: 0x170003F1 RID: 1009
	// (get) Token: 0x060032AD RID: 12973 RVA: 0x0013938D File Offset: 0x0013758D
	public static GameManifest Current
	{
		get
		{
			if (GameManifest.loadedManifest != null)
			{
				return GameManifest.loadedManifest;
			}
			GameManifest.Load();
			return GameManifest.loadedManifest;
		}
	}

	// Token: 0x060032AE RID: 12974 RVA: 0x001393AC File Offset: 0x001375AC
	public static void Load()
	{
		if (GameManifest.loadedManifest != null)
		{
			return;
		}
		GameManifest.loadedManifest = FileSystem.Load<GameManifest>("Assets/manifest.asset", true);
		foreach (GameManifest.PrefabProperties prefabProperties in GameManifest.loadedManifest.prefabProperties)
		{
			GameManifest.guidToPath.Add(prefabProperties.guid, prefabProperties.name);
			GameManifest.pathToGuid.Add(prefabProperties.name, prefabProperties.guid);
		}
		foreach (GameManifest.GuidPath guidPath in GameManifest.loadedManifest.guidPaths)
		{
			if (!GameManifest.guidToPath.ContainsKey(guidPath.guid))
			{
				GameManifest.guidToPath.Add(guidPath.guid, guidPath.name);
				GameManifest.pathToGuid.Add(guidPath.name, guidPath.guid);
			}
		}
		DebugEx.Log(GameManifest.GetMetadataStatus(), StackTraceLogType.None);
	}

	// Token: 0x060032AF RID: 12975 RVA: 0x0013948C File Offset: 0x0013768C
	public static void LoadAssets()
	{
		if (Skinnable.All != null)
		{
			return;
		}
		Skinnable.All = FileSystem.LoadAllFromBundle<Skinnable>("skinnables.preload.bundle", "t:Skinnable");
		if (Skinnable.All == null || Skinnable.All.Length == 0)
		{
			throw new Exception("Error loading skinnables");
		}
		DebugEx.Log(GameManifest.GetAssetStatus(), StackTraceLogType.None);
	}

	// Token: 0x060032B0 RID: 12976 RVA: 0x001394DC File Offset: 0x001376DC
	internal static Dictionary<string, string[]> LoadEffectDictionary()
	{
		GameManifest.EffectCategory[] array = GameManifest.loadedManifest.effectCategories;
		Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
		foreach (GameManifest.EffectCategory effectCategory in array)
		{
			dictionary.Add(effectCategory.folder, effectCategory.prefabs.ToArray());
		}
		return dictionary;
	}

	// Token: 0x060032B1 RID: 12977 RVA: 0x00139524 File Offset: 0x00137724
	internal static string GUIDToPath(string guid)
	{
		if (string.IsNullOrEmpty(guid))
		{
			Debug.LogError("GUIDToPath: guid is empty");
			return string.Empty;
		}
		GameManifest.Load();
		string result;
		if (GameManifest.guidToPath.TryGetValue(guid, out result))
		{
			return result;
		}
		Debug.LogWarning("GUIDToPath: no path found for guid " + guid);
		return string.Empty;
	}

	// Token: 0x060032B2 RID: 12978 RVA: 0x00139574 File Offset: 0x00137774
	internal static UnityEngine.Object GUIDToObject(string guid)
	{
		UnityEngine.Object result = null;
		if (GameManifest.guidToObject.TryGetValue(guid, out result))
		{
			return result;
		}
		string text = GameManifest.GUIDToPath(guid);
		if (string.IsNullOrEmpty(text))
		{
			Debug.LogWarning("Missing file for guid " + guid);
			GameManifest.guidToObject.Add(guid, null);
			return null;
		}
		UnityEngine.Object @object = FileSystem.Load<UnityEngine.Object>(text, true);
		GameManifest.guidToObject.Add(guid, @object);
		return @object;
	}

	// Token: 0x060032B3 RID: 12979 RVA: 0x001395D8 File Offset: 0x001377D8
	internal static void Invalidate(string path)
	{
		string key;
		if (!GameManifest.pathToGuid.TryGetValue(path, out key))
		{
			return;
		}
		UnityEngine.Object @object;
		if (!GameManifest.guidToObject.TryGetValue(key, out @object))
		{
			return;
		}
		if (@object != null)
		{
			UnityEngine.Object.DestroyImmediate(@object, true);
		}
		GameManifest.guidToObject.Remove(key);
	}

	// Token: 0x060032B4 RID: 12980 RVA: 0x00139624 File Offset: 0x00137824
	private static string GetMetadataStatus()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (GameManifest.loadedManifest != null)
		{
			stringBuilder.Append("Manifest Metadata Loaded");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			stringBuilder.Append(GameManifest.loadedManifest.pooledStrings.Length.ToString());
			stringBuilder.Append(" pooled strings");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			stringBuilder.Append(GameManifest.loadedManifest.prefabProperties.Length.ToString());
			stringBuilder.Append(" prefab properties");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			stringBuilder.Append(GameManifest.loadedManifest.effectCategories.Length.ToString());
			stringBuilder.Append(" effect categories");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			stringBuilder.Append(GameManifest.loadedManifest.entities.Length.ToString());
			stringBuilder.Append(" entity names");
			stringBuilder.AppendLine();
		}
		else
		{
			stringBuilder.Append("Manifest Metadata Missing");
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060032B5 RID: 12981 RVA: 0x00139758 File Offset: 0x00137958
	private static string GetAssetStatus()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (GameManifest.loadedManifest != null)
		{
			stringBuilder.Append("Manifest Assets Loaded");
			stringBuilder.AppendLine();
			stringBuilder.Append("\t");
			stringBuilder.Append((Skinnable.All != null) ? Skinnable.All.Length.ToString() : "0");
			stringBuilder.Append(" skinnable objects");
		}
		else
		{
			stringBuilder.Append("Manifest Assets Missing");
		}
		return stringBuilder.ToString();
	}

	// Token: 0x040028F6 RID: 10486
	internal static GameManifest loadedManifest;

	// Token: 0x040028F7 RID: 10487
	internal static Dictionary<string, string> guidToPath = new Dictionary<string, string>();

	// Token: 0x040028F8 RID: 10488
	internal static Dictionary<string, string> pathToGuid = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	// Token: 0x040028F9 RID: 10489
	internal static Dictionary<string, UnityEngine.Object> guidToObject = new Dictionary<string, UnityEngine.Object>();

	// Token: 0x040028FA RID: 10490
	public GameManifest.PooledString[] pooledStrings;

	// Token: 0x040028FB RID: 10491
	public GameManifest.PrefabProperties[] prefabProperties;

	// Token: 0x040028FC RID: 10492
	public GameManifest.EffectCategory[] effectCategories;

	// Token: 0x040028FD RID: 10493
	public GameManifest.GuidPath[] guidPaths;

	// Token: 0x040028FE RID: 10494
	public string[] entities;

	// Token: 0x02000DFF RID: 3583
	[Serializable]
	public struct PooledString
	{
		// Token: 0x040048A6 RID: 18598
		[HideInInspector]
		public string str;

		// Token: 0x040048A7 RID: 18599
		public uint hash;
	}

	// Token: 0x02000E00 RID: 3584
	[Serializable]
	public class PrefabProperties
	{
		// Token: 0x040048A8 RID: 18600
		[HideInInspector]
		public string name;

		// Token: 0x040048A9 RID: 18601
		public string guid;

		// Token: 0x040048AA RID: 18602
		public uint hash;

		// Token: 0x040048AB RID: 18603
		public bool pool;
	}

	// Token: 0x02000E01 RID: 3585
	[Serializable]
	public class EffectCategory
	{
		// Token: 0x040048AC RID: 18604
		[HideInInspector]
		public string folder;

		// Token: 0x040048AD RID: 18605
		public List<string> prefabs;
	}

	// Token: 0x02000E02 RID: 3586
	[Serializable]
	public class GuidPath
	{
		// Token: 0x040048AE RID: 18606
		[HideInInspector]
		public string name;

		// Token: 0x040048AF RID: 18607
		public string guid;
	}
}
