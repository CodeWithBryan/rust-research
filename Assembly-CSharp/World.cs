using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ConVar;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200062C RID: 1580
public static class World
{
	// Token: 0x17000379 RID: 889
	// (get) Token: 0x06002D86 RID: 11654 RVA: 0x00112B28 File Offset: 0x00110D28
	// (set) Token: 0x06002D87 RID: 11655 RVA: 0x00112B2F File Offset: 0x00110D2F
	public static uint Seed { get; set; }

	// Token: 0x1700037A RID: 890
	// (get) Token: 0x06002D88 RID: 11656 RVA: 0x00112B37 File Offset: 0x00110D37
	// (set) Token: 0x06002D89 RID: 11657 RVA: 0x00112B3E File Offset: 0x00110D3E
	public static uint Salt { get; set; }

	// Token: 0x1700037B RID: 891
	// (get) Token: 0x06002D8A RID: 11658 RVA: 0x00112B46 File Offset: 0x00110D46
	// (set) Token: 0x06002D8B RID: 11659 RVA: 0x00112B4D File Offset: 0x00110D4D
	public static uint Size { get; set; }

	// Token: 0x1700037C RID: 892
	// (get) Token: 0x06002D8C RID: 11660 RVA: 0x00112B55 File Offset: 0x00110D55
	// (set) Token: 0x06002D8D RID: 11661 RVA: 0x00112B5C File Offset: 0x00110D5C
	public static string Checksum { get; set; }

	// Token: 0x1700037D RID: 893
	// (get) Token: 0x06002D8E RID: 11662 RVA: 0x00112B64 File Offset: 0x00110D64
	// (set) Token: 0x06002D8F RID: 11663 RVA: 0x00112B6B File Offset: 0x00110D6B
	public static string Url { get; set; }

	// Token: 0x1700037E RID: 894
	// (get) Token: 0x06002D90 RID: 11664 RVA: 0x00112B73 File Offset: 0x00110D73
	// (set) Token: 0x06002D91 RID: 11665 RVA: 0x00112B7A File Offset: 0x00110D7A
	public static bool Procedural { get; set; }

	// Token: 0x1700037F RID: 895
	// (get) Token: 0x06002D92 RID: 11666 RVA: 0x00112B82 File Offset: 0x00110D82
	// (set) Token: 0x06002D93 RID: 11667 RVA: 0x00112B89 File Offset: 0x00110D89
	public static bool Cached { get; set; }

	// Token: 0x17000380 RID: 896
	// (get) Token: 0x06002D94 RID: 11668 RVA: 0x00112B91 File Offset: 0x00110D91
	// (set) Token: 0x06002D95 RID: 11669 RVA: 0x00112B98 File Offset: 0x00110D98
	public static bool Networked { get; set; }

	// Token: 0x17000381 RID: 897
	// (get) Token: 0x06002D96 RID: 11670 RVA: 0x00112BA0 File Offset: 0x00110DA0
	// (set) Token: 0x06002D97 RID: 11671 RVA: 0x00112BA7 File Offset: 0x00110DA7
	public static bool Receiving { get; set; }

	// Token: 0x17000382 RID: 898
	// (get) Token: 0x06002D98 RID: 11672 RVA: 0x00112BAF File Offset: 0x00110DAF
	// (set) Token: 0x06002D99 RID: 11673 RVA: 0x00112BB6 File Offset: 0x00110DB6
	public static bool Transfer { get; set; }

	// Token: 0x17000383 RID: 899
	// (get) Token: 0x06002D9A RID: 11674 RVA: 0x00112BBE File Offset: 0x00110DBE
	// (set) Token: 0x06002D9B RID: 11675 RVA: 0x00112BC5 File Offset: 0x00110DC5
	public static bool LoadedFromSave { get; set; }

	// Token: 0x17000384 RID: 900
	// (get) Token: 0x06002D9C RID: 11676 RVA: 0x00112BCD File Offset: 0x00110DCD
	// (set) Token: 0x06002D9D RID: 11677 RVA: 0x00112BD4 File Offset: 0x00110DD4
	public static int SpawnIndex { get; set; }

	// Token: 0x17000385 RID: 901
	// (get) Token: 0x06002D9E RID: 11678 RVA: 0x00112BDC File Offset: 0x00110DDC
	// (set) Token: 0x06002D9F RID: 11679 RVA: 0x00112BE3 File Offset: 0x00110DE3
	public static WorldSerialization Serialization { get; set; }

	// Token: 0x17000386 RID: 902
	// (get) Token: 0x06002DA0 RID: 11680 RVA: 0x00112BEB File Offset: 0x00110DEB
	public static string Name
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			if (global::World.CanLoadFromUrl())
			{
				return Path.GetFileNameWithoutExtension(WWW.UnEscapeURL(global::World.Url));
			}
			return Application.loadedLevelName;
		}
	}

	// Token: 0x06002DA1 RID: 11681 RVA: 0x00112C09 File Offset: 0x00110E09
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static string GetServerBrowserMapName()
	{
		if (!global::World.CanLoadFromUrl())
		{
			return global::World.Name;
		}
		if (global::World.Name.StartsWith("proceduralmap."))
		{
			return "Procedural Map";
		}
		return "Custom Map";
	}

	// Token: 0x06002DA2 RID: 11682 RVA: 0x00112C34 File Offset: 0x00110E34
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool CanLoadFromUrl()
	{
		return !string.IsNullOrEmpty(global::World.Url);
	}

	// Token: 0x06002DA3 RID: 11683 RVA: 0x00112C43 File Offset: 0x00110E43
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool CanLoadFromDisk()
	{
		return File.Exists(global::World.MapFolderName + "/" + global::World.MapFileName);
	}

	// Token: 0x06002DA4 RID: 11684 RVA: 0x00112C60 File Offset: 0x00110E60
	public static void CleanupOldFiles()
	{
		Regex regex1 = new Regex("proceduralmap\\.[0-9]+\\.[0-9]+\\.[0-9]+\\.map");
		Regex regex2 = new Regex("\\.[0-9]+\\.[0-9]+\\." + 233 + "\\.map");
		foreach (string path2 in from path in Directory.GetFiles(global::World.MapFolderName, "*.map")
		where regex1.IsMatch(path) && !regex2.IsMatch(path)
		select path)
		{
			try
			{
				File.Delete(path2);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError(ex.Message);
			}
		}
	}

	// Token: 0x17000387 RID: 903
	// (get) Token: 0x06002DA5 RID: 11685 RVA: 0x00112D1C File Offset: 0x00110F1C
	public static string MapFileName
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			if (global::World.CanLoadFromUrl())
			{
				return global::World.Name + ".map";
			}
			return string.Concat(new object[]
			{
				global::World.Name.Replace(" ", "").ToLower(),
				".",
				global::World.Size,
				".",
				global::World.Seed,
				".",
				233,
				".map"
			});
		}
	}

	// Token: 0x17000388 RID: 904
	// (get) Token: 0x06002DA6 RID: 11686 RVA: 0x00112DAE File Offset: 0x00110FAE
	public static string MapFolderName
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			return Server.rootFolder;
		}
	}

	// Token: 0x17000389 RID: 905
	// (get) Token: 0x06002DA7 RID: 11687 RVA: 0x00112DB8 File Offset: 0x00110FB8
	public static string SaveFileName
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			if (global::World.CanLoadFromUrl())
			{
				return string.Concat(new object[]
				{
					global::World.Name,
					".",
					233,
					".sav"
				});
			}
			return string.Concat(new object[]
			{
				global::World.Name.Replace(" ", "").ToLower(),
				".",
				global::World.Size,
				".",
				global::World.Seed,
				".",
				233,
				".sav"
			});
		}
	}

	// Token: 0x1700038A RID: 906
	// (get) Token: 0x06002DA8 RID: 11688 RVA: 0x00112DAE File Offset: 0x00110FAE
	public static string SaveFolderName
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			return Server.rootFolder;
		}
	}

	// Token: 0x06002DA9 RID: 11689 RVA: 0x00112E6B File Offset: 0x0011106B
	public static void InitSeed(int seed)
	{
		global::World.InitSeed((uint)seed);
	}

	// Token: 0x06002DAA RID: 11690 RVA: 0x00112E73 File Offset: 0x00111073
	public static void InitSeed(uint seed)
	{
		if (seed == 0U)
		{
			seed = global::World.SeedIdentifier().MurmurHashUnsigned() % 2147483647U;
		}
		if (seed == 0U)
		{
			seed = 123456U;
		}
		global::World.Seed = seed;
		Server.seed = (int)seed;
	}

	// Token: 0x06002DAB RID: 11691 RVA: 0x00112EA0 File Offset: 0x001110A0
	private static string SeedIdentifier()
	{
		return string.Concat(new object[]
		{
			SystemInfo.deviceUniqueIdentifier,
			"_",
			233,
			"_",
			Server.identity
		});
	}

	// Token: 0x06002DAC RID: 11692 RVA: 0x00112EDA File Offset: 0x001110DA
	public static void InitSalt(int salt)
	{
		global::World.InitSalt((uint)salt);
	}

	// Token: 0x06002DAD RID: 11693 RVA: 0x00112EE2 File Offset: 0x001110E2
	public static void InitSalt(uint salt)
	{
		if (salt == 0U)
		{
			salt = global::World.SaltIdentifier().MurmurHashUnsigned() % 2147483647U;
		}
		if (salt == 0U)
		{
			salt = 654321U;
		}
		global::World.Salt = salt;
		Server.salt = (int)salt;
	}

	// Token: 0x06002DAE RID: 11694 RVA: 0x00112F0F File Offset: 0x0011110F
	private static string SaltIdentifier()
	{
		return SystemInfo.deviceUniqueIdentifier + "_salt";
	}

	// Token: 0x06002DAF RID: 11695 RVA: 0x00112F20 File Offset: 0x00111120
	public static void InitSize(int size)
	{
		global::World.InitSize((uint)size);
	}

	// Token: 0x06002DB0 RID: 11696 RVA: 0x00112F28 File Offset: 0x00111128
	public static void InitSize(uint size)
	{
		if (size == 0U)
		{
			size = 4500U;
		}
		if (size < 1000U)
		{
			size = 1000U;
		}
		if (size > 6000U)
		{
			size = 6000U;
		}
		global::World.Size = size;
		Server.worldsize = (int)size;
	}

	// Token: 0x06002DB1 RID: 11697 RVA: 0x00112F60 File Offset: 0x00111160
	public static byte[] GetMap(string name)
	{
		MapData map = global::World.Serialization.GetMap(name);
		if (map == null)
		{
			return null;
		}
		return map.data;
	}

	// Token: 0x06002DB2 RID: 11698 RVA: 0x00112F84 File Offset: 0x00111184
	public static int GetCachedHeightMapResolution()
	{
		return Mathf.RoundToInt(Mathf.Sqrt((float)(global::World.GetMap("height").Length / 2)));
	}

	// Token: 0x06002DB3 RID: 11699 RVA: 0x00112F9F File Offset: 0x0011119F
	public static int GetCachedSplatMapResolution()
	{
		return Mathf.RoundToInt(Mathf.Sqrt((float)(global::World.GetMap("splat").Length / 8)));
	}

	// Token: 0x06002DB4 RID: 11700 RVA: 0x00112FBA File Offset: 0x001111BA
	public static void AddMap(string name, byte[] data)
	{
		global::World.Serialization.AddMap(name, data);
	}

	// Token: 0x06002DB5 RID: 11701 RVA: 0x00112FC8 File Offset: 0x001111C8
	public static void AddPrefab(string category, Prefab prefab, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		global::World.Serialization.AddPrefab(category, prefab.ID, position, rotation, scale);
		if (!global::World.Cached)
		{
			rotation = Quaternion.Euler(rotation.eulerAngles);
			global::World.Spawn(category, prefab, position, rotation, scale);
		}
	}

	// Token: 0x06002DB6 RID: 11702 RVA: 0x00113000 File Offset: 0x00111200
	public static PathData PathListToPathData(PathList src)
	{
		return new PathData
		{
			name = src.Name,
			spline = src.Spline,
			start = src.Start,
			end = src.End,
			width = src.Width,
			innerPadding = src.InnerPadding,
			outerPadding = src.OuterPadding,
			innerFade = src.InnerFade,
			outerFade = src.OuterFade,
			randomScale = src.RandomScale,
			meshOffset = src.MeshOffset,
			terrainOffset = src.TerrainOffset,
			splat = src.Splat,
			topology = src.Topology,
			hierarchy = src.Hierarchy,
			nodes = global::World.VectorArrayToList(src.Path.Points)
		};
	}

	// Token: 0x06002DB7 RID: 11703 RVA: 0x001130DC File Offset: 0x001112DC
	public static PathList PathDataToPathList(PathData src)
	{
		PathList pathList = new PathList(src.name, global::World.VectorListToArray(src.nodes));
		pathList.Spline = src.spline;
		pathList.Start = src.start;
		pathList.End = src.end;
		pathList.Width = src.width;
		pathList.InnerPadding = src.innerPadding;
		pathList.OuterPadding = src.outerPadding;
		pathList.InnerFade = src.innerFade;
		pathList.OuterFade = src.outerFade;
		pathList.RandomScale = src.randomScale;
		pathList.MeshOffset = src.meshOffset;
		pathList.TerrainOffset = src.terrainOffset;
		pathList.Splat = src.splat;
		pathList.Topology = src.topology;
		pathList.Hierarchy = src.hierarchy;
		pathList.Path.RecalculateTangents();
		return pathList;
	}

	// Token: 0x06002DB8 RID: 11704 RVA: 0x001131B4 File Offset: 0x001113B4
	public static Vector3[] VectorListToArray(List<VectorData> src)
	{
		Vector3[] array = new Vector3[src.Count];
		for (int i = 0; i < array.Length; i++)
		{
			VectorData vectorData = src[i];
			array[i] = new Vector3
			{
				x = vectorData.x,
				y = vectorData.y,
				z = vectorData.z
			};
		}
		return array;
	}

	// Token: 0x06002DB9 RID: 11705 RVA: 0x0011321C File Offset: 0x0011141C
	public static List<VectorData> VectorArrayToList(Vector3[] src)
	{
		List<VectorData> list = new List<VectorData>(src.Length);
		foreach (Vector3 vector in src)
		{
			list.Add(new VectorData
			{
				x = vector.x,
				y = vector.y,
				z = vector.z
			});
		}
		return list;
	}

	// Token: 0x06002DBA RID: 11706 RVA: 0x0011327F File Offset: 0x0011147F
	public static IEnumerable<PathList> GetPaths(string name)
	{
		return from p in global::World.Serialization.GetPaths(name)
		select global::World.PathDataToPathList(p);
	}

	// Token: 0x06002DBB RID: 11707 RVA: 0x001132B0 File Offset: 0x001114B0
	public static void AddPaths(IEnumerable<PathList> paths)
	{
		foreach (PathList path in paths)
		{
			global::World.AddPath(path);
		}
	}

	// Token: 0x06002DBC RID: 11708 RVA: 0x001132F8 File Offset: 0x001114F8
	public static void AddPath(PathList path)
	{
		global::World.Serialization.AddPath(global::World.PathListToPathData(path));
	}

	// Token: 0x06002DBD RID: 11709 RVA: 0x0011330A File Offset: 0x0011150A
	public static IEnumerator SpawnAsync(float deltaTime, Action<string> statusFunction = null)
	{
		int totalCount = 0;
		Dictionary<string, List<PrefabData>> assetGroups = new Dictionary<string, List<PrefabData>>(StringComparer.InvariantCultureIgnoreCase);
		foreach (PrefabData prefabData in global::World.Serialization.world.prefabs)
		{
			string text = StringPool.Get(prefabData.id);
			if (string.IsNullOrWhiteSpace(text))
			{
				UnityEngine.Debug.LogWarning(string.Format("Could not find path for prefab ID {0}, skipping spawn", prefabData.id));
			}
			else
			{
				List<PrefabData> list;
				if (!assetGroups.TryGetValue(text, out list))
				{
					list = new List<PrefabData>();
					assetGroups.Add(text, list);
				}
				list.Add(prefabData);
				int num = totalCount;
				totalCount = num + 1;
			}
		}
		int spawnedCount = 0;
		int resultIndex = 0;
		Stopwatch sw = Stopwatch.StartNew();
		AssetPreloadResult load = FileSystem.PreloadAssets(assetGroups.Keys, Global.preloadConcurrency, 10);
		while (load != null && (load.MoveNext() || assetGroups.Count > 0))
		{
			while (resultIndex < load.Results.Count && sw.Elapsed.TotalSeconds < (double)deltaTime)
			{
				string item = load.Results[resultIndex].Item1;
				List<PrefabData> list2;
				if (!assetGroups.TryGetValue(item, out list2))
				{
					int num = resultIndex;
					resultIndex = num + 1;
				}
				else if (list2.Count == 0)
				{
					assetGroups.Remove(item);
					int num = resultIndex;
					resultIndex = num + 1;
				}
				else
				{
					int index = list2.Count - 1;
					PrefabData prefab = list2[index];
					list2.RemoveAt(index);
					global::World.Spawn(prefab);
					int num = spawnedCount;
					spawnedCount = num + 1;
				}
			}
			global::World.Status(statusFunction, "Spawning World ({0}/{1})", spawnedCount, totalCount);
			yield return CoroutineEx.waitForEndOfFrame;
			sw.Restart();
		}
		yield break;
	}

	// Token: 0x06002DBE RID: 11710 RVA: 0x00113320 File Offset: 0x00111520
	public static IEnumerator Spawn(float deltaTime, Action<string> statusFunction = null)
	{
		Stopwatch sw = Stopwatch.StartNew();
		int num;
		for (int i = 0; i < global::World.Serialization.world.prefabs.Count; i = num + 1)
		{
			if (sw.Elapsed.TotalSeconds > (double)deltaTime || i == 0 || i == global::World.Serialization.world.prefabs.Count - 1)
			{
				global::World.Status(statusFunction, "Spawning World ({0}/{1})", i + 1, global::World.Serialization.world.prefabs.Count);
				yield return CoroutineEx.waitForEndOfFrame;
				sw.Reset();
				sw.Start();
			}
			global::World.Spawn(global::World.Serialization.world.prefabs[i]);
			num = i;
		}
		yield break;
	}

	// Token: 0x06002DBF RID: 11711 RVA: 0x00113338 File Offset: 0x00111538
	public static void Spawn()
	{
		for (int i = 0; i < global::World.Serialization.world.prefabs.Count; i++)
		{
			global::World.Spawn(global::World.Serialization.world.prefabs[i]);
		}
	}

	// Token: 0x06002DC0 RID: 11712 RVA: 0x00113380 File Offset: 0x00111580
	public static void Spawn(string category, string folder = null)
	{
		for (int i = global::World.SpawnIndex; i < global::World.Serialization.world.prefabs.Count; i++)
		{
			PrefabData prefabData = global::World.Serialization.world.prefabs[i];
			if (prefabData.category != category)
			{
				break;
			}
			string text = StringPool.Get(prefabData.id);
			if (!string.IsNullOrEmpty(folder) && !text.StartsWith(folder))
			{
				break;
			}
			global::World.Spawn(prefabData);
			global::World.SpawnIndex++;
		}
	}

	// Token: 0x06002DC1 RID: 11713 RVA: 0x00113404 File Offset: 0x00111604
	public static void Spawn(string category, string[] folders)
	{
		for (int i = global::World.SpawnIndex; i < global::World.Serialization.world.prefabs.Count; i++)
		{
			PrefabData prefabData = global::World.Serialization.world.prefabs[i];
			if (prefabData.category != category)
			{
				break;
			}
			string str = StringPool.Get(prefabData.id);
			if (folders != null && !str.StartsWithAny(folders))
			{
				break;
			}
			global::World.Spawn(prefabData);
			global::World.SpawnIndex++;
		}
	}

	// Token: 0x06002DC2 RID: 11714 RVA: 0x00113482 File Offset: 0x00111682
	private static void Spawn(PrefabData prefab)
	{
		global::World.Spawn(prefab.category, Prefab.Load(prefab.id, null, null), prefab.position, prefab.rotation, prefab.scale);
	}

	// Token: 0x06002DC3 RID: 11715 RVA: 0x001134C0 File Offset: 0x001116C0
	private static void Spawn(string category, Prefab prefab, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		if (prefab == null || !prefab.Object)
		{
			return;
		}
		if (!global::World.Cached)
		{
			prefab.ApplyTerrainPlacements(position, rotation, scale);
			prefab.ApplyTerrainModifiers(position, rotation, scale);
		}
		GameObject gameObject = prefab.Spawn(position, rotation, scale, true);
		if (gameObject)
		{
			gameObject.SetHierarchyGroup(category, true, false);
		}
	}

	// Token: 0x06002DC4 RID: 11716 RVA: 0x00113516 File Offset: 0x00111716
	private static void Status(Action<string> statusFunction, string status, object obj1)
	{
		if (statusFunction != null)
		{
			statusFunction(string.Format(status, obj1));
		}
	}

	// Token: 0x06002DC5 RID: 11717 RVA: 0x00113528 File Offset: 0x00111728
	private static void Status(Action<string> statusFunction, string status, object obj1, object obj2)
	{
		if (statusFunction != null)
		{
			statusFunction(string.Format(status, obj1, obj2));
		}
	}

	// Token: 0x06002DC6 RID: 11718 RVA: 0x0011353B File Offset: 0x0011173B
	private static void Status(Action<string> statusFunction, string status, object obj1, object obj2, object obj3)
	{
		if (statusFunction != null)
		{
			statusFunction(string.Format(status, obj1, obj2, obj3));
		}
	}

	// Token: 0x06002DC7 RID: 11719 RVA: 0x00113550 File Offset: 0x00111750
	private static void Status(Action<string> statusFunction, string status, params object[] objs)
	{
		if (statusFunction != null)
		{
			statusFunction(string.Format(status, objs));
		}
	}
}
