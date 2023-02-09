using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000528 RID: 1320
public class Prefab : IComparable<Prefab>
{
	// Token: 0x06002877 RID: 10359 RVA: 0x000F67D0 File Offset: 0x000F49D0
	public Prefab(string name, GameObject prefab, GameManager manager, PrefabAttribute.Library attribute)
	{
		this.ID = StringPool.Get(name);
		this.Name = name;
		this.Folder = (string.IsNullOrWhiteSpace(name) ? "" : Path.GetDirectoryName(name));
		this.Object = prefab;
		this.Manager = manager;
		this.Attribute = attribute;
		this.Parameters = (prefab ? prefab.GetComponent<PrefabParameters>() : null);
	}

	// Token: 0x06002878 RID: 10360 RVA: 0x000F683E File Offset: 0x000F4A3E
	public static implicit operator GameObject(Prefab prefab)
	{
		return prefab.Object;
	}

	// Token: 0x06002879 RID: 10361 RVA: 0x000F6848 File Offset: 0x000F4A48
	public int CompareTo(Prefab that)
	{
		if (that == null)
		{
			return 1;
		}
		PrefabPriority prefabPriority = (this.Parameters != null) ? this.Parameters.Priority : PrefabPriority.Default;
		return ((that.Parameters != null) ? that.Parameters.Priority : PrefabPriority.Default).CompareTo(prefabPriority);
	}

	// Token: 0x0600287A RID: 10362 RVA: 0x000F68A8 File Offset: 0x000F4AA8
	public bool ApplyTerrainAnchors(ref Vector3 pos, Quaternion rot, Vector3 scale, TerrainAnchorMode mode, SpawnFilter filter = null)
	{
		TerrainAnchor[] anchors = this.Attribute.FindAll<TerrainAnchor>(this.ID);
		return this.Object.transform.ApplyTerrainAnchors(anchors, ref pos, rot, scale, mode, filter);
	}

	// Token: 0x0600287B RID: 10363 RVA: 0x000F68E0 File Offset: 0x000F4AE0
	public bool ApplyTerrainAnchors(ref Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
	{
		TerrainAnchor[] anchors = this.Attribute.FindAll<TerrainAnchor>(this.ID);
		return this.Object.transform.ApplyTerrainAnchors(anchors, ref pos, rot, scale, filter);
	}

	// Token: 0x0600287C RID: 10364 RVA: 0x000F6918 File Offset: 0x000F4B18
	public bool ApplyTerrainChecks(Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
	{
		TerrainCheck[] anchors = this.Attribute.FindAll<TerrainCheck>(this.ID);
		return this.Object.transform.ApplyTerrainChecks(anchors, pos, rot, scale, filter);
	}

	// Token: 0x0600287D RID: 10365 RVA: 0x000F6950 File Offset: 0x000F4B50
	public bool ApplyTerrainFilters(Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
	{
		TerrainFilter[] filters = this.Attribute.FindAll<TerrainFilter>(this.ID);
		return this.Object.transform.ApplyTerrainFilters(filters, pos, rot, scale, filter);
	}

	// Token: 0x0600287E RID: 10366 RVA: 0x000F6988 File Offset: 0x000F4B88
	public void ApplyTerrainModifiers(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		TerrainModifier[] modifiers = this.Attribute.FindAll<TerrainModifier>(this.ID);
		this.Object.transform.ApplyTerrainModifiers(modifiers, pos, rot, scale);
	}

	// Token: 0x0600287F RID: 10367 RVA: 0x000F69BC File Offset: 0x000F4BBC
	public void ApplyTerrainPlacements(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		TerrainPlacement[] placements = this.Attribute.FindAll<TerrainPlacement>(this.ID);
		this.Object.transform.ApplyTerrainPlacements(placements, pos, rot, scale);
	}

	// Token: 0x06002880 RID: 10368 RVA: 0x000F69F0 File Offset: 0x000F4BF0
	public bool ApplyWaterChecks(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		WaterCheck[] anchors = this.Attribute.FindAll<WaterCheck>(this.ID);
		return this.Object.transform.ApplyWaterChecks(anchors, pos, rot, scale);
	}

	// Token: 0x06002881 RID: 10369 RVA: 0x000F6A24 File Offset: 0x000F4C24
	public bool ApplyBoundsChecks(Vector3 pos, Quaternion rot, Vector3 scale, LayerMask rejectOnLayer)
	{
		BoundsCheck[] bounds = this.Attribute.FindAll<BoundsCheck>(this.ID);
		BaseEntity component = this.Object.GetComponent<BaseEntity>();
		return !(component != null) || component.ApplyBoundsChecks(bounds, pos, rot, scale, rejectOnLayer);
	}

	// Token: 0x06002882 RID: 10370 RVA: 0x000F6A68 File Offset: 0x000F4C68
	public void ApplyDecorComponents(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		DecorComponent[] components = this.Attribute.FindAll<DecorComponent>(this.ID);
		this.Object.transform.ApplyDecorComponents(components, ref pos, ref rot, ref scale);
	}

	// Token: 0x06002883 RID: 10371 RVA: 0x000F6A9B File Offset: 0x000F4C9B
	public bool CheckEnvironmentVolumes(Vector3 pos, Quaternion rot, Vector3 scale, EnvironmentType type)
	{
		return this.Object.transform.CheckEnvironmentVolumes(pos, rot, scale, type);
	}

	// Token: 0x06002884 RID: 10372 RVA: 0x000F6AB2 File Offset: 0x000F4CB2
	public bool CheckEnvironmentVolumesInsideTerrain(Vector3 pos, Quaternion rot, Vector3 scale, EnvironmentType type, float padding = 0f)
	{
		return this.Object.transform.CheckEnvironmentVolumesInsideTerrain(pos, rot, scale, type, padding);
	}

	// Token: 0x06002885 RID: 10373 RVA: 0x000F6ACB File Offset: 0x000F4CCB
	public bool CheckEnvironmentVolumesOutsideTerrain(Vector3 pos, Quaternion rot, Vector3 scale, EnvironmentType type, float padding = 0f)
	{
		return this.Object.transform.CheckEnvironmentVolumesOutsideTerrain(pos, rot, scale, type, padding);
	}

	// Token: 0x06002886 RID: 10374 RVA: 0x000F6AE4 File Offset: 0x000F4CE4
	public void ApplySequenceReplacement(List<Prefab> sequence, ref Prefab replacement, Prefab[] possibleReplacements, int pathLength, int pathIndex)
	{
		PathSequence pathSequence = this.Attribute.Find<PathSequence>(this.ID);
		if (pathSequence != null)
		{
			pathSequence.ApplySequenceReplacement(sequence, ref replacement, possibleReplacements, pathLength, pathIndex);
		}
	}

	// Token: 0x06002887 RID: 10375 RVA: 0x000F6B19 File Offset: 0x000F4D19
	public GameObject Spawn(Transform transform, bool active = true)
	{
		return this.Manager.CreatePrefab(this.Name, transform, active);
	}

	// Token: 0x06002888 RID: 10376 RVA: 0x000F6B2E File Offset: 0x000F4D2E
	public GameObject Spawn(Vector3 pos, Quaternion rot, bool active = true)
	{
		return this.Manager.CreatePrefab(this.Name, pos, rot, active);
	}

	// Token: 0x06002889 RID: 10377 RVA: 0x000F6B44 File Offset: 0x000F4D44
	public GameObject Spawn(Vector3 pos, Quaternion rot, Vector3 scale, bool active = true)
	{
		return this.Manager.CreatePrefab(this.Name, pos, rot, scale, active);
	}

	// Token: 0x0600288A RID: 10378 RVA: 0x000F6B5C File Offset: 0x000F4D5C
	public BaseEntity SpawnEntity(Vector3 pos, Quaternion rot, bool active = true)
	{
		return this.Manager.CreateEntity(this.Name, pos, rot, active);
	}

	// Token: 0x0600288B RID: 10379 RVA: 0x000F6B74 File Offset: 0x000F4D74
	public static Prefab<T> Load<T>(uint id, GameManager manager = null, PrefabAttribute.Library attribute = null) where T : Component
	{
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		string text = StringPool.Get(id);
		if (string.IsNullOrWhiteSpace(text))
		{
			Debug.LogWarning(string.Format("Could not find path for prefab ID {0}", id));
			return null;
		}
		GameObject gameObject = manager.FindPrefab(text);
		T component = gameObject.GetComponent<T>();
		return new Prefab<T>(text, gameObject, component, manager, attribute);
	}

	// Token: 0x0600288C RID: 10380 RVA: 0x000F6BD4 File Offset: 0x000F4DD4
	public static Prefab Load(uint id, GameManager manager = null, PrefabAttribute.Library attribute = null)
	{
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		string text = StringPool.Get(id);
		if (string.IsNullOrWhiteSpace(text))
		{
			Debug.LogWarning(string.Format("Could not find path for prefab ID {0}", id));
			return null;
		}
		GameObject prefab = manager.FindPrefab(text);
		return new Prefab(text, prefab, manager, attribute);
	}

	// Token: 0x0600288D RID: 10381 RVA: 0x000F6C2C File Offset: 0x000F4E2C
	public static Prefab[] Load(string folder, GameManager manager = null, PrefabAttribute.Library attribute = null, bool useProbabilities = true)
	{
		if (string.IsNullOrEmpty(folder))
		{
			return null;
		}
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		string[] array = Prefab.FindPrefabNames(folder, useProbabilities);
		Prefab[] array2 = new Prefab[array.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			string text = array[i];
			GameObject prefab = manager.FindPrefab(text);
			array2[i] = new Prefab(text, prefab, manager, attribute);
		}
		return array2;
	}

	// Token: 0x0600288E RID: 10382 RVA: 0x000F6C91 File Offset: 0x000F4E91
	public static Prefab<T>[] Load<T>(string folder, GameManager manager = null, PrefabAttribute.Library attribute = null, bool useProbabilities = true) where T : Component
	{
		if (string.IsNullOrEmpty(folder))
		{
			return null;
		}
		return Prefab.Load<T>(Prefab.FindPrefabNames(folder, useProbabilities), manager, attribute);
	}

	// Token: 0x0600288F RID: 10383 RVA: 0x000F6CAC File Offset: 0x000F4EAC
	public static Prefab<T>[] Load<T>(string[] names, GameManager manager = null, PrefabAttribute.Library attribute = null) where T : Component
	{
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		Prefab<T>[] array = new Prefab<T>[names.Length];
		for (int i = 0; i < array.Length; i++)
		{
			string text = names[i];
			GameObject gameObject = manager.FindPrefab(text);
			T component = gameObject.GetComponent<T>();
			array[i] = new Prefab<T>(text, gameObject, component, manager, attribute);
		}
		return array;
	}

	// Token: 0x06002890 RID: 10384 RVA: 0x000F6D08 File Offset: 0x000F4F08
	public static Prefab LoadRandom(string folder, ref uint seed, GameManager manager = null, PrefabAttribute.Library attribute = null, bool useProbabilities = true)
	{
		if (string.IsNullOrEmpty(folder))
		{
			return null;
		}
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		string[] array = Prefab.FindPrefabNames(folder, useProbabilities);
		if (array.Length == 0)
		{
			return null;
		}
		string text = array[SeedRandom.Range(ref seed, 0, array.Length)];
		GameObject prefab = manager.FindPrefab(text);
		return new Prefab(text, prefab, manager, attribute);
	}

	// Token: 0x06002891 RID: 10385 RVA: 0x000F6D60 File Offset: 0x000F4F60
	public static Prefab<T> LoadRandom<T>(string folder, ref uint seed, GameManager manager = null, PrefabAttribute.Library attribute = null, bool useProbabilities = true) where T : Component
	{
		if (string.IsNullOrEmpty(folder))
		{
			return null;
		}
		if (manager == null)
		{
			manager = Prefab.DefaultManager;
		}
		if (attribute == null)
		{
			attribute = Prefab.DefaultAttribute;
		}
		string[] array = Prefab.FindPrefabNames(folder, useProbabilities);
		if (array.Length == 0)
		{
			return null;
		}
		string text = array[SeedRandom.Range(ref seed, 0, array.Length)];
		GameObject gameObject = manager.FindPrefab(text);
		T component = gameObject.GetComponent<T>();
		return new Prefab<T>(text, gameObject, component, manager, attribute);
	}

	// Token: 0x17000334 RID: 820
	// (get) Token: 0x06002892 RID: 10386 RVA: 0x000F6DC0 File Offset: 0x000F4FC0
	public static PrefabAttribute.Library DefaultAttribute
	{
		get
		{
			return PrefabAttribute.server;
		}
	}

	// Token: 0x17000335 RID: 821
	// (get) Token: 0x06002893 RID: 10387 RVA: 0x000F6DC7 File Offset: 0x000F4FC7
	public static GameManager DefaultManager
	{
		get
		{
			return GameManager.server;
		}
	}

	// Token: 0x06002894 RID: 10388 RVA: 0x000F6DD0 File Offset: 0x000F4FD0
	private static string[] FindPrefabNames(string strPrefab, bool useProbabilities = false)
	{
		strPrefab = strPrefab.TrimEnd(new char[]
		{
			'/'
		}).ToLower();
		GameObject[] array = FileSystem.LoadPrefabs(strPrefab + "/");
		List<string> list = new List<string>(array.Length);
		foreach (GameObject gameObject in array)
		{
			string item = strPrefab + "/" + gameObject.name.ToLower() + ".prefab";
			if (!useProbabilities)
			{
				list.Add(item);
			}
			else
			{
				PrefabParameters component = gameObject.GetComponent<PrefabParameters>();
				int num = component ? component.Count : 1;
				for (int j = 0; j < num; j++)
				{
					list.Add(item);
				}
			}
		}
		list.Sort();
		return list.ToArray();
	}

	// Token: 0x040020EF RID: 8431
	public uint ID;

	// Token: 0x040020F0 RID: 8432
	public string Name;

	// Token: 0x040020F1 RID: 8433
	public string Folder;

	// Token: 0x040020F2 RID: 8434
	public GameObject Object;

	// Token: 0x040020F3 RID: 8435
	public GameManager Manager;

	// Token: 0x040020F4 RID: 8436
	public PrefabAttribute.Library Attribute;

	// Token: 0x040020F5 RID: 8437
	public PrefabParameters Parameters;
}
