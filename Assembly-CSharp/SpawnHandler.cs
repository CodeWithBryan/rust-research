using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConVar;
using UnityEngine;

// Token: 0x0200054F RID: 1359
public class SpawnHandler : SingletonComponent<SpawnHandler>
{
	// Token: 0x06002953 RID: 10579 RVA: 0x000FACD0 File Offset: 0x000F8ED0
	protected void OnEnable()
	{
		this.AllSpawnPopulations = this.SpawnPopulations.Concat(this.ConvarSpawnPopulations).ToArray<SpawnPopulation>();
		base.StartCoroutine(this.SpawnTick());
		base.StartCoroutine(this.SpawnGroupTick());
		base.StartCoroutine(this.SpawnIndividualTick());
	}

	// Token: 0x06002954 RID: 10580 RVA: 0x000FAD20 File Offset: 0x000F8F20
	public static BasePlayer.SpawnPoint GetSpawnPoint()
	{
		if (SingletonComponent<SpawnHandler>.Instance == null || SingletonComponent<SpawnHandler>.Instance.CharDistribution == null)
		{
			return null;
		}
		BasePlayer.SpawnPoint spawnPoint = new BasePlayer.SpawnPoint();
		if (!((WaterSystem.OceanLevel < 0.5f) ? SpawnHandler.GetSpawnPointStandard(spawnPoint) : FloodedSpawnHandler.GetSpawnPoint(spawnPoint, WaterSystem.OceanLevel + 1f)))
		{
			return null;
		}
		return spawnPoint;
	}

	// Token: 0x06002955 RID: 10581 RVA: 0x000FAD78 File Offset: 0x000F8F78
	private static bool GetSpawnPointStandard(BasePlayer.SpawnPoint spawnPoint)
	{
		for (int i = 0; i < 60; i++)
		{
			if (SingletonComponent<SpawnHandler>.Instance.CharDistribution.Sample(out spawnPoint.pos, out spawnPoint.rot, false, 0f))
			{
				bool flag = true;
				if (TerrainMeta.Path != null)
				{
					using (List<MonumentInfo>.Enumerator enumerator = TerrainMeta.Path.Monuments.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.Distance(spawnPoint.pos) < 50f)
							{
								flag = false;
								break;
							}
						}
					}
				}
				if (flag)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002956 RID: 10582 RVA: 0x000FAE28 File Offset: 0x000F9028
	public void UpdateDistributions()
	{
		SpawnHandler.<>c__DisplayClass23_0 CS$<>8__locals1 = new SpawnHandler.<>c__DisplayClass23_0();
		if (global::World.Size == 0U)
		{
			return;
		}
		this.SpawnDistributions = new SpawnDistribution[this.AllSpawnPopulations.Length];
		this.population2distribution = new Dictionary<SpawnPopulation, SpawnDistribution>();
		Vector3 size = TerrainMeta.Size;
		Vector3 position = TerrainMeta.Position;
		CS$<>8__locals1.pop_res = Mathf.NextPowerOfTwo((int)(global::World.Size * 0.25f));
		SpawnFilter filter;
		byte[] map;
		float cutoff;
		for (int i = 0; i < this.AllSpawnPopulations.Length; i++)
		{
			SpawnPopulation spawnPopulation = this.AllSpawnPopulations[i];
			if (spawnPopulation == null)
			{
				Debug.LogError("Spawn handler contains null spawn population.");
			}
			else
			{
				byte[] map = new byte[CS$<>8__locals1.pop_res * CS$<>8__locals1.pop_res];
				SpawnFilter filter = spawnPopulation.Filter;
				float cutoff = spawnPopulation.FilterCutoff;
				Parallel.For(0, CS$<>8__locals1.pop_res, delegate(int z)
				{
					for (int j = 0; j < CS$<>8__locals1.pop_res; j++)
					{
						float normX = ((float)j + 0.5f) / (float)CS$<>8__locals1.pop_res;
						float normZ = ((float)z + 0.5f) / (float)CS$<>8__locals1.pop_res;
						float factor = filter.GetFactor(normX, normZ, true);
						map[z * CS$<>8__locals1.pop_res + j] = (byte)((factor >= cutoff) ? (255f * factor) : 0f);
					}
				});
				SpawnDistribution value = this.SpawnDistributions[i] = new SpawnDistribution(this, map, position, size);
				this.population2distribution.Add(spawnPopulation, value);
			}
		}
		CS$<>8__locals1.char_res = Mathf.NextPowerOfTwo((int)(global::World.Size * 0.5f));
		map = new byte[CS$<>8__locals1.char_res * CS$<>8__locals1.char_res];
		filter = this.CharacterSpawn;
		cutoff = this.CharacterSpawnCutoff;
		Parallel.For(0, CS$<>8__locals1.char_res, delegate(int z)
		{
			for (int j = 0; j < CS$<>8__locals1.char_res; j++)
			{
				float normX = ((float)j + 0.5f) / (float)CS$<>8__locals1.char_res;
				float normZ = ((float)z + 0.5f) / (float)CS$<>8__locals1.char_res;
				float factor = filter.GetFactor(normX, normZ, true);
				map[z * CS$<>8__locals1.char_res + j] = (byte)((factor >= cutoff) ? (255f * factor) : 0f);
			}
		});
		this.CharDistribution = new SpawnDistribution(this, map, position, size);
	}

	// Token: 0x06002957 RID: 10583 RVA: 0x000FAFFC File Offset: 0x000F91FC
	public void FillPopulations()
	{
		if (this.SpawnDistributions == null)
		{
			return;
		}
		for (int i = 0; i < this.AllSpawnPopulations.Length; i++)
		{
			if (!(this.AllSpawnPopulations[i] == null))
			{
				this.SpawnInitial(this.AllSpawnPopulations[i], this.SpawnDistributions[i]);
			}
		}
	}

	// Token: 0x06002958 RID: 10584 RVA: 0x000FB04C File Offset: 0x000F924C
	public void FillGroups()
	{
		for (int i = 0; i < this.SpawnGroups.Count; i++)
		{
			this.SpawnGroups[i].Fill();
		}
	}

	// Token: 0x06002959 RID: 10585 RVA: 0x000FB080 File Offset: 0x000F9280
	public void FillIndividuals()
	{
		for (int i = 0; i < this.SpawnIndividuals.Count; i++)
		{
			SpawnIndividual spawnIndividual = this.SpawnIndividuals[i];
			this.Spawn(Prefab.Load<Spawnable>(spawnIndividual.PrefabID, null, null), spawnIndividual.Position, spawnIndividual.Rotation);
		}
	}

	// Token: 0x0600295A RID: 10586 RVA: 0x000FB0D0 File Offset: 0x000F92D0
	public void InitialSpawn()
	{
		if (ConVar.Spawn.respawn_populations && this.SpawnDistributions != null)
		{
			for (int i = 0; i < this.AllSpawnPopulations.Length; i++)
			{
				if (!(this.AllSpawnPopulations[i] == null))
				{
					this.SpawnInitial(this.AllSpawnPopulations[i], this.SpawnDistributions[i]);
				}
			}
		}
		if (ConVar.Spawn.respawn_groups)
		{
			for (int j = 0; j < this.SpawnGroups.Count; j++)
			{
				this.SpawnGroups[j].SpawnInitial();
			}
		}
	}

	// Token: 0x0600295B RID: 10587 RVA: 0x000FB153 File Offset: 0x000F9353
	public void StartSpawnTick()
	{
		this.spawnTick = true;
	}

	// Token: 0x0600295C RID: 10588 RVA: 0x000FB15C File Offset: 0x000F935C
	private IEnumerator SpawnTick()
	{
		for (;;)
		{
			yield return CoroutineEx.waitForEndOfFrame;
			if (this.spawnTick && ConVar.Spawn.respawn_populations)
			{
				yield return CoroutineEx.waitForSeconds(ConVar.Spawn.tick_populations);
				int num;
				for (int i = 0; i < this.AllSpawnPopulations.Length; i = num + 1)
				{
					SpawnPopulation spawnPopulation = this.AllSpawnPopulations[i];
					if (!(spawnPopulation == null))
					{
						SpawnDistribution spawnDistribution = this.SpawnDistributions[i];
						if (spawnDistribution != null)
						{
							try
							{
								if (this.SpawnDistributions != null)
								{
									this.SpawnRepeating(spawnPopulation, spawnDistribution);
								}
							}
							catch (Exception message)
							{
								Debug.LogError(message);
							}
							yield return CoroutineEx.waitForEndOfFrame;
						}
					}
					num = i;
				}
			}
		}
		yield break;
	}

	// Token: 0x0600295D RID: 10589 RVA: 0x000FB16B File Offset: 0x000F936B
	private IEnumerator SpawnGroupTick()
	{
		for (;;)
		{
			yield return CoroutineEx.waitForEndOfFrame;
			if (this.spawnTick && ConVar.Spawn.respawn_groups)
			{
				yield return CoroutineEx.waitForSeconds(1f);
				int num;
				for (int i = 0; i < this.SpawnGroups.Count; i = num + 1)
				{
					ISpawnGroup spawnGroup = this.SpawnGroups[i];
					if (spawnGroup != null)
					{
						try
						{
							spawnGroup.SpawnRepeating();
						}
						catch (Exception message)
						{
							Debug.LogError(message);
						}
						yield return CoroutineEx.waitForEndOfFrame;
					}
					num = i;
				}
			}
		}
		yield break;
	}

	// Token: 0x0600295E RID: 10590 RVA: 0x000FB17A File Offset: 0x000F937A
	private IEnumerator SpawnIndividualTick()
	{
		for (;;)
		{
			yield return CoroutineEx.waitForEndOfFrame;
			if (this.spawnTick && ConVar.Spawn.respawn_individuals)
			{
				yield return CoroutineEx.waitForSeconds(ConVar.Spawn.tick_individuals);
				int num;
				for (int i = 0; i < this.SpawnIndividuals.Count; i = num + 1)
				{
					SpawnIndividual spawnIndividual = this.SpawnIndividuals[i];
					try
					{
						this.Spawn(Prefab.Load<Spawnable>(spawnIndividual.PrefabID, null, null), spawnIndividual.Position, spawnIndividual.Rotation);
					}
					catch (Exception message)
					{
						Debug.LogError(message);
					}
					yield return CoroutineEx.waitForEndOfFrame;
					num = i;
				}
			}
		}
		yield break;
	}

	// Token: 0x0600295F RID: 10591 RVA: 0x000FB18C File Offset: 0x000F938C
	public void SpawnInitial(SpawnPopulation population, SpawnDistribution distribution)
	{
		int targetCount = this.GetTargetCount(population, distribution);
		int currentCount = this.GetCurrentCount(population, distribution);
		int num = targetCount - currentCount;
		this.Fill(population, distribution, targetCount, num, num * population.SpawnAttemptsInitial);
	}

	// Token: 0x06002960 RID: 10592 RVA: 0x000FB1C4 File Offset: 0x000F93C4
	public void SpawnRepeating(SpawnPopulation population, SpawnDistribution distribution)
	{
		int targetCount = this.GetTargetCount(population, distribution);
		int currentCount = this.GetCurrentCount(population, distribution);
		int num = targetCount - currentCount;
		num = Mathf.RoundToInt((float)num * population.GetCurrentSpawnRate());
		num = UnityEngine.Random.Range(Mathf.Min(num, this.MinSpawnsPerTick), Mathf.Min(num, this.MaxSpawnsPerTick));
		this.Fill(population, distribution, targetCount, num, num * population.SpawnAttemptsRepeating);
	}

	// Token: 0x06002961 RID: 10593 RVA: 0x000FB228 File Offset: 0x000F9428
	private void Fill(SpawnPopulation population, SpawnDistribution distribution, int targetCount, int numToFill, int numToTry)
	{
		if (targetCount == 0)
		{
			return;
		}
		if (!population.Initialize())
		{
			Debug.LogError("[Spawn] No prefabs to spawn in " + population.ResourceFolder, population);
			return;
		}
		if (Global.developer > 1)
		{
			Debug.Log(string.Concat(new object[]
			{
				"[Spawn] Population ",
				population.ResourceFolder,
				" needs to spawn ",
				numToFill
			}));
		}
		float num = Mathf.Max((float)population.ClusterSizeMax, distribution.GetGridCellArea() * population.GetMaximumSpawnDensity());
		population.UpdateWeights(distribution, targetCount);
		while (numToFill >= population.ClusterSizeMin && numToTry > 0)
		{
			ByteQuadtree.Element node = distribution.SampleNode();
			int num2 = UnityEngine.Random.Range(population.ClusterSizeMin, population.ClusterSizeMax + 1);
			num2 = Mathx.Min(numToTry, numToFill, num2);
			for (int i = 0; i < num2; i++)
			{
				Vector3 vector;
				Quaternion rot;
				bool flag = distribution.Sample(out vector, out rot, node, population.AlignToNormal, (float)population.ClusterDithering) && population.Filter.GetFactor(vector, true) > 0f;
				if (flag && population.FilterRadius > 0f)
				{
					flag = (population.Filter.GetFactor(vector + Vector3.forward * population.FilterRadius, true) > 0f && population.Filter.GetFactor(vector - Vector3.forward * population.FilterRadius, true) > 0f && population.Filter.GetFactor(vector + Vector3.right * population.FilterRadius, true) > 0f && population.Filter.GetFactor(vector - Vector3.right * population.FilterRadius, true) > 0f);
				}
				Prefab<Spawnable> prefab;
				if (flag && population.TryTakeRandomPrefab(out prefab))
				{
					if (population.GetSpawnPosOverride(prefab, ref vector, ref rot) && (float)distribution.GetCount(vector) < num)
					{
						this.Spawn(population, prefab, vector, rot);
						numToFill--;
					}
					else
					{
						population.ReturnPrefab(prefab);
					}
				}
				numToTry--;
			}
		}
		population.OnPostFill(this);
	}

	// Token: 0x06002962 RID: 10594 RVA: 0x000FB44C File Offset: 0x000F964C
	public GameObject Spawn(SpawnPopulation population, Prefab<Spawnable> prefab, Vector3 pos, Quaternion rot)
	{
		if (prefab == null)
		{
			return null;
		}
		if (prefab.Component == null)
		{
			Debug.LogError("[Spawn] Missing component 'Spawnable' on " + prefab.Name);
			return null;
		}
		Vector3 one = Vector3.one;
		DecorComponent[] components = PrefabAttribute.server.FindAll<DecorComponent>(prefab.ID);
		prefab.Object.transform.ApplyDecorComponents(components, ref pos, ref rot, ref one);
		if (!prefab.ApplyTerrainAnchors(ref pos, rot, one, TerrainAnchorMode.MinimizeMovement, population.Filter))
		{
			return null;
		}
		if (!prefab.ApplyTerrainChecks(pos, rot, one, population.Filter))
		{
			return null;
		}
		if (!prefab.ApplyTerrainFilters(pos, rot, one, null))
		{
			return null;
		}
		if (!prefab.ApplyWaterChecks(pos, rot, one))
		{
			return null;
		}
		if (!prefab.ApplyBoundsChecks(pos, rot, one, this.BoundsCheckMask))
		{
			return null;
		}
		if (Global.developer > 1)
		{
			Debug.Log("[Spawn] Spawning " + prefab.Name);
		}
		BaseEntity baseEntity = prefab.SpawnEntity(pos, rot, false);
		if (baseEntity == null)
		{
			Debug.LogWarning("[Spawn] Couldn't create prefab as entity - " + prefab.Name);
			return null;
		}
		Spawnable component = baseEntity.GetComponent<Spawnable>();
		if (component.Population != population)
		{
			component.Population = population;
		}
		baseEntity.gameObject.AwakeFromInstantiate();
		baseEntity.Spawn();
		return baseEntity.gameObject;
	}

	// Token: 0x06002963 RID: 10595 RVA: 0x000FB588 File Offset: 0x000F9788
	private GameObject Spawn(Prefab<Spawnable> prefab, Vector3 pos, Quaternion rot)
	{
		if (!this.CheckBounds(prefab.Object, pos, rot, Vector3.one))
		{
			return null;
		}
		BaseEntity baseEntity = prefab.SpawnEntity(pos, rot, true);
		if (baseEntity == null)
		{
			Debug.LogWarning("[Spawn] Couldn't create prefab as entity - " + prefab.Name);
			return null;
		}
		baseEntity.Spawn();
		return baseEntity.gameObject;
	}

	// Token: 0x06002964 RID: 10596 RVA: 0x000FB5E2 File Offset: 0x000F97E2
	public bool CheckBounds(GameObject gameObject, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		return SpawnHandler.CheckBounds(gameObject, pos, rot, scale, this.BoundsCheckMask);
	}

	// Token: 0x06002965 RID: 10597 RVA: 0x000FB5F4 File Offset: 0x000F97F4
	public static bool CheckBounds(GameObject gameObject, Vector3 pos, Quaternion rot, Vector3 scale, LayerMask mask)
	{
		if (gameObject == null)
		{
			return true;
		}
		if (mask != 0)
		{
			BaseEntity component = gameObject.GetComponent<BaseEntity>();
			if (component != null && UnityEngine.Physics.CheckBox(pos + rot * Vector3.Scale(component.bounds.center, scale), Vector3.Scale(component.bounds.extents, scale), rot, mask))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002966 RID: 10598 RVA: 0x000FB668 File Offset: 0x000F9868
	public void EnforceLimits(bool forceAll = false)
	{
		if (this.SpawnDistributions == null)
		{
			return;
		}
		for (int i = 0; i < this.AllSpawnPopulations.Length; i++)
		{
			if (!(this.AllSpawnPopulations[i] == null))
			{
				SpawnPopulation spawnPopulation = this.AllSpawnPopulations[i];
				SpawnDistribution distribution = this.SpawnDistributions[i];
				if (forceAll || spawnPopulation.EnforcePopulationLimits)
				{
					this.EnforceLimits(spawnPopulation, distribution);
				}
			}
		}
	}

	// Token: 0x06002967 RID: 10599 RVA: 0x000FB6C8 File Offset: 0x000F98C8
	private void EnforceLimits(SpawnPopulation population, SpawnDistribution distribution)
	{
		int targetCount = this.GetTargetCount(population, distribution);
		Spawnable[] array = this.FindAll(population);
		if (array.Length <= targetCount)
		{
			return;
		}
		Debug.Log(string.Concat(new object[]
		{
			population,
			" has ",
			array.Length,
			" objects, but max allowed is ",
			targetCount
		}));
		int num = array.Length - targetCount;
		Debug.Log(" - deleting " + num + " objects");
		foreach (Spawnable spawnable in array.Take(num))
		{
			BaseEntity baseEntity = spawnable.gameObject.ToBaseEntity();
			if (baseEntity.IsValid())
			{
				baseEntity.Kill(BaseNetworkable.DestroyMode.None);
			}
			else
			{
				GameManager.Destroy(spawnable.gameObject, 0f);
			}
		}
	}

	// Token: 0x06002968 RID: 10600 RVA: 0x000FB7B4 File Offset: 0x000F99B4
	public Spawnable[] FindAll(SpawnPopulation population)
	{
		return (from x in UnityEngine.Object.FindObjectsOfType<Spawnable>()
		where x.gameObject.activeInHierarchy && x.Population == population
		select x).ToArray<Spawnable>();
	}

	// Token: 0x06002969 RID: 10601 RVA: 0x000FB7EC File Offset: 0x000F99EC
	public int GetTargetCount(SpawnPopulation population, SpawnDistribution distribution)
	{
		float num = TerrainMeta.Size.x * TerrainMeta.Size.z;
		float num2 = population.GetCurrentSpawnDensity();
		if (!population.ScaleWithLargeMaps)
		{
			num = Mathf.Min(num, 16000000f);
		}
		if (population.ScaleWithSpawnFilter)
		{
			num2 *= distribution.Density;
		}
		return Mathf.RoundToInt(num * num2);
	}

	// Token: 0x0600296A RID: 10602 RVA: 0x000FB843 File Offset: 0x000F9A43
	public int GetCurrentCount(SpawnPopulation population, SpawnDistribution distribution)
	{
		return distribution.Count;
	}

	// Token: 0x0600296B RID: 10603 RVA: 0x000FB84B File Offset: 0x000F9A4B
	public void AddRespawn(SpawnIndividual individual)
	{
		this.SpawnIndividuals.Add(individual);
	}

	// Token: 0x0600296C RID: 10604 RVA: 0x000FB85C File Offset: 0x000F9A5C
	public void AddInstance(Spawnable spawnable)
	{
		if (spawnable.Population != null)
		{
			SpawnDistribution spawnDistribution;
			if (!this.population2distribution.TryGetValue(spawnable.Population, out spawnDistribution))
			{
				Debug.LogWarning("[SpawnHandler] trying to add instance to invalid population: " + spawnable.Population);
				return;
			}
			spawnDistribution.AddInstance(spawnable);
		}
	}

	// Token: 0x0600296D RID: 10605 RVA: 0x000FB8AC File Offset: 0x000F9AAC
	public void RemoveInstance(Spawnable spawnable)
	{
		if (spawnable.Population != null)
		{
			SpawnDistribution spawnDistribution;
			if (!this.population2distribution.TryGetValue(spawnable.Population, out spawnDistribution))
			{
				Debug.LogWarning("[SpawnHandler] trying to remove instance from invalid population: " + spawnable.Population);
				return;
			}
			spawnDistribution.RemoveInstance(spawnable);
		}
	}

	// Token: 0x0600296E RID: 10606 RVA: 0x000FB8FC File Offset: 0x000F9AFC
	public static float PlayerFraction()
	{
		float num = (float)Mathf.Max(Server.maxplayers, 1);
		return Mathf.Clamp01((float)BasePlayer.activePlayerList.Count / num);
	}

	// Token: 0x0600296F RID: 10607 RVA: 0x000FB928 File Offset: 0x000F9B28
	public static float PlayerLerp(float min, float max)
	{
		return Mathf.Lerp(min, max, SpawnHandler.PlayerFraction());
	}

	// Token: 0x06002970 RID: 10608 RVA: 0x000FB938 File Offset: 0x000F9B38
	public static float PlayerExcess()
	{
		float num = Mathf.Max(ConVar.Spawn.player_base, 1f);
		float num2 = (float)BasePlayer.activePlayerList.Count;
		if (num2 <= num)
		{
			return 0f;
		}
		return (num2 - num) / num;
	}

	// Token: 0x06002971 RID: 10609 RVA: 0x000FB970 File Offset: 0x000F9B70
	public static float PlayerScale(float scalar)
	{
		return Mathf.Max(1f, SpawnHandler.PlayerExcess() * scalar);
	}

	// Token: 0x06002972 RID: 10610 RVA: 0x000FB983 File Offset: 0x000F9B83
	public void DumpReport(string filename)
	{
		File.AppendAllText(filename, "\r\n\r\nSpawnHandler Report:\r\n\r\n" + this.GetReport(true));
	}

	// Token: 0x06002973 RID: 10611 RVA: 0x000FB99C File Offset: 0x000F9B9C
	public string GetReport(bool detailed = true)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (this.AllSpawnPopulations == null)
		{
			stringBuilder.AppendLine("Spawn population array is null.");
		}
		if (this.SpawnDistributions == null)
		{
			stringBuilder.AppendLine("Spawn distribution array is null.");
		}
		if (this.AllSpawnPopulations != null && this.SpawnDistributions != null)
		{
			for (int i = 0; i < this.AllSpawnPopulations.Length; i++)
			{
				if (!(this.AllSpawnPopulations[i] == null))
				{
					SpawnPopulation spawnPopulation = this.AllSpawnPopulations[i];
					SpawnDistribution spawnDistribution = this.SpawnDistributions[i];
					if (spawnPopulation != null)
					{
						if (!string.IsNullOrEmpty(spawnPopulation.ResourceFolder))
						{
							stringBuilder.AppendLine(spawnPopulation.name + " (autospawn/" + spawnPopulation.ResourceFolder + ")");
						}
						else
						{
							stringBuilder.AppendLine(spawnPopulation.name);
						}
						if (detailed)
						{
							stringBuilder.AppendLine("\tPrefabs:");
							if (spawnPopulation.Prefabs != null)
							{
								foreach (Prefab<Spawnable> prefab in spawnPopulation.Prefabs)
								{
									stringBuilder.AppendLine(string.Concat(new object[]
									{
										"\t\t",
										prefab.Name,
										" - ",
										prefab.Object
									}));
								}
							}
							else
							{
								stringBuilder.AppendLine("\t\tN/A");
							}
						}
						if (spawnDistribution != null)
						{
							int currentCount = this.GetCurrentCount(spawnPopulation, spawnDistribution);
							int targetCount = this.GetTargetCount(spawnPopulation, spawnDistribution);
							stringBuilder.AppendLine(string.Concat(new object[]
							{
								"\tPopulation: ",
								currentCount,
								"/",
								targetCount
							}));
						}
						else
						{
							stringBuilder.AppendLine("\tDistribution #" + i + " is not set.");
						}
					}
					else
					{
						stringBuilder.AppendLine("Population #" + i + " is not set.");
					}
					stringBuilder.AppendLine();
				}
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x04002187 RID: 8583
	public float TickInterval = 60f;

	// Token: 0x04002188 RID: 8584
	public int MinSpawnsPerTick = 100;

	// Token: 0x04002189 RID: 8585
	public int MaxSpawnsPerTick = 100;

	// Token: 0x0400218A RID: 8586
	public LayerMask PlacementMask;

	// Token: 0x0400218B RID: 8587
	public LayerMask PlacementCheckMask;

	// Token: 0x0400218C RID: 8588
	public float PlacementCheckHeight = 25f;

	// Token: 0x0400218D RID: 8589
	public LayerMask RadiusCheckMask;

	// Token: 0x0400218E RID: 8590
	public float RadiusCheckDistance = 5f;

	// Token: 0x0400218F RID: 8591
	public LayerMask BoundsCheckMask;

	// Token: 0x04002190 RID: 8592
	public SpawnFilter CharacterSpawn;

	// Token: 0x04002191 RID: 8593
	public float CharacterSpawnCutoff;

	// Token: 0x04002192 RID: 8594
	public SpawnPopulation[] SpawnPopulations;

	// Token: 0x04002193 RID: 8595
	internal SpawnDistribution[] SpawnDistributions;

	// Token: 0x04002194 RID: 8596
	internal SpawnDistribution CharDistribution;

	// Token: 0x04002195 RID: 8597
	internal List<ISpawnGroup> SpawnGroups = new List<ISpawnGroup>();

	// Token: 0x04002196 RID: 8598
	internal List<SpawnIndividual> SpawnIndividuals = new List<SpawnIndividual>();

	// Token: 0x04002197 RID: 8599
	[ReadOnly]
	public SpawnPopulation[] ConvarSpawnPopulations;

	// Token: 0x04002198 RID: 8600
	private Dictionary<SpawnPopulation, SpawnDistribution> population2distribution;

	// Token: 0x04002199 RID: 8601
	private bool spawnTick;

	// Token: 0x0400219A RID: 8602
	private SpawnPopulation[] AllSpawnPopulations;
}
