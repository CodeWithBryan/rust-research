using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200053E RID: 1342
[CreateAssetMenu(menuName = "Rust/Spawn Population")]
public class SpawnPopulation : BaseScriptableObject
{
	// Token: 0x17000338 RID: 824
	// (get) Token: 0x060028E6 RID: 10470 RVA: 0x000F94A7 File Offset: 0x000F76A7
	public virtual float TargetDensity
	{
		get
		{
			return this._targetDensity;
		}
	}

	// Token: 0x060028E7 RID: 10471 RVA: 0x000F94B0 File Offset: 0x000F76B0
	public bool Initialize()
	{
		if (this.Prefabs == null || this.Prefabs.Length == 0)
		{
			if (!string.IsNullOrEmpty(this.ResourceFolder))
			{
				this.Prefabs = Prefab.Load<Spawnable>("assets/bundled/prefabs/autospawn/" + this.ResourceFolder, GameManager.server, PrefabAttribute.server, false);
			}
			if (this.ResourceList != null && this.ResourceList.Length != 0)
			{
				List<string> list = new List<string>();
				foreach (GameObjectRef gameObjectRef in this.ResourceList)
				{
					string resourcePath = gameObjectRef.resourcePath;
					if (string.IsNullOrEmpty(resourcePath))
					{
						Debug.LogWarning(base.name + " resource list contains invalid resource path for GUID " + gameObjectRef.guid, this);
					}
					else
					{
						list.Add(resourcePath);
					}
				}
				this.Prefabs = Prefab.Load<Spawnable>(list.ToArray(), GameManager.server, PrefabAttribute.server);
			}
			if (this.Prefabs == null || this.Prefabs.Length == 0)
			{
				return false;
			}
			this.numToSpawn = new int[this.Prefabs.Length];
		}
		return true;
	}

	// Token: 0x060028E8 RID: 10472 RVA: 0x000F95B0 File Offset: 0x000F77B0
	public void UpdateWeights(SpawnDistribution distribution, int targetCount)
	{
		int num = 0;
		for (int i = 0; i < this.Prefabs.Length; i++)
		{
			Prefab<Spawnable> prefab = this.Prefabs[i];
			int prefabWeight = this.GetPrefabWeight(prefab);
			num += prefabWeight;
		}
		int num2 = Mathf.CeilToInt((float)targetCount / (float)num);
		this.sumToSpawn = 0;
		for (int j = 0; j < this.Prefabs.Length; j++)
		{
			Prefab<Spawnable> prefab2 = this.Prefabs[j];
			int prefabWeight2 = this.GetPrefabWeight(prefab2);
			int count = distribution.GetCount(prefab2.ID);
			int num3 = Mathf.Max(prefabWeight2 * num2 - count, 0);
			this.numToSpawn[j] = num3;
			this.sumToSpawn += num3;
		}
	}

	// Token: 0x060028E9 RID: 10473 RVA: 0x000F965B File Offset: 0x000F785B
	protected virtual int GetPrefabWeight(Prefab<Spawnable> prefab)
	{
		if (!prefab.Parameters)
		{
			return 1;
		}
		return prefab.Parameters.Count;
	}

	// Token: 0x060028EA RID: 10474 RVA: 0x000F9678 File Offset: 0x000F7878
	public bool TryTakeRandomPrefab(out Prefab<Spawnable> result)
	{
		int num = UnityEngine.Random.Range(0, this.sumToSpawn);
		for (int i = 0; i < this.Prefabs.Length; i++)
		{
			if ((num -= this.numToSpawn[i]) < 0)
			{
				this.numToSpawn[i]--;
				this.sumToSpawn--;
				result = this.Prefabs[i];
				return true;
			}
		}
		result = null;
		return false;
	}

	// Token: 0x060028EB RID: 10475 RVA: 0x000F96E4 File Offset: 0x000F78E4
	public void ReturnPrefab(Prefab<Spawnable> prefab)
	{
		if (prefab == null)
		{
			return;
		}
		for (int i = 0; i < this.Prefabs.Length; i++)
		{
			if (this.Prefabs[i] == prefab)
			{
				this.numToSpawn[i]++;
				this.sumToSpawn++;
			}
		}
	}

	// Token: 0x060028EC RID: 10476 RVA: 0x000F9732 File Offset: 0x000F7932
	public float GetCurrentSpawnRate()
	{
		if (this.ScaleWithServerPopulation)
		{
			return this.SpawnRate * SpawnHandler.PlayerLerp(Spawn.min_rate, Spawn.max_rate);
		}
		return this.SpawnRate * Spawn.max_rate;
	}

	// Token: 0x060028ED RID: 10477 RVA: 0x000F975F File Offset: 0x000F795F
	public float GetCurrentSpawnDensity()
	{
		if (this.ScaleWithServerPopulation)
		{
			return this.TargetDensity * SpawnHandler.PlayerLerp(Spawn.min_density, Spawn.max_density) * 1E-06f;
		}
		return this.TargetDensity * Spawn.max_density * 1E-06f;
	}

	// Token: 0x060028EE RID: 10478 RVA: 0x000F9798 File Offset: 0x000F7998
	public float GetMaximumSpawnDensity()
	{
		if (this.ScaleWithServerPopulation)
		{
			return 2f * this.TargetDensity * SpawnHandler.PlayerLerp(Spawn.min_density, Spawn.max_density) * 1E-06f;
		}
		return 2f * this.TargetDensity * Spawn.max_density * 1E-06f;
	}

	// Token: 0x060028EF RID: 10479 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool GetSpawnPosOverride(Prefab<Spawnable> prefab, ref Vector3 newPos, ref Quaternion newRot)
	{
		return true;
	}

	// Token: 0x060028F0 RID: 10480 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnPostFill(SpawnHandler spawnHandler)
	{
	}

	// Token: 0x04002135 RID: 8501
	[Header("Spawnables")]
	public string ResourceFolder = string.Empty;

	// Token: 0x04002136 RID: 8502
	public GameObjectRef[] ResourceList;

	// Token: 0x04002137 RID: 8503
	[Header("Spawn Info")]
	[Tooltip("Usually per square km")]
	[SerializeField]
	[FormerlySerializedAs("TargetDensity")]
	private float _targetDensity = 1f;

	// Token: 0x04002138 RID: 8504
	public float SpawnRate = 1f;

	// Token: 0x04002139 RID: 8505
	public int ClusterSizeMin = 1;

	// Token: 0x0400213A RID: 8506
	public int ClusterSizeMax = 1;

	// Token: 0x0400213B RID: 8507
	public int ClusterDithering;

	// Token: 0x0400213C RID: 8508
	public int SpawnAttemptsInitial = 20;

	// Token: 0x0400213D RID: 8509
	public int SpawnAttemptsRepeating = 10;

	// Token: 0x0400213E RID: 8510
	public bool EnforcePopulationLimits = true;

	// Token: 0x0400213F RID: 8511
	public bool ScaleWithLargeMaps = true;

	// Token: 0x04002140 RID: 8512
	public bool ScaleWithSpawnFilter = true;

	// Token: 0x04002141 RID: 8513
	public bool ScaleWithServerPopulation;

	// Token: 0x04002142 RID: 8514
	public bool AlignToNormal;

	// Token: 0x04002143 RID: 8515
	public SpawnFilter Filter = new SpawnFilter();

	// Token: 0x04002144 RID: 8516
	public float FilterCutoff;

	// Token: 0x04002145 RID: 8517
	public float FilterRadius;

	// Token: 0x04002146 RID: 8518
	internal Prefab<Spawnable>[] Prefabs;

	// Token: 0x04002147 RID: 8519
	private int[] numToSpawn;

	// Token: 0x04002148 RID: 8520
	private int sumToSpawn;
}
