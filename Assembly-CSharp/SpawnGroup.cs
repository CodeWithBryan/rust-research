using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using UnityEngine;

// Token: 0x0200054C RID: 1356
public class SpawnGroup : BaseMonoBehaviour, IServerComponent, ISpawnPointUser, ISpawnGroup
{
	// Token: 0x1700033C RID: 828
	// (get) Token: 0x06002936 RID: 10550 RVA: 0x000FA5AD File Offset: 0x000F87AD
	public int currentPopulation
	{
		get
		{
			return this.spawnInstances.Count;
		}
	}

	// Token: 0x06002937 RID: 10551 RVA: 0x000FA5BA File Offset: 0x000F87BA
	public virtual bool WantsInitialSpawn()
	{
		return this.wantsInitialSpawn;
	}

	// Token: 0x06002938 RID: 10552 RVA: 0x000FA5C2 File Offset: 0x000F87C2
	public virtual bool WantsTimedSpawn()
	{
		return this.respawnDelayMax != float.PositiveInfinity;
	}

	// Token: 0x06002939 RID: 10553 RVA: 0x000FA5D4 File Offset: 0x000F87D4
	public float GetSpawnDelta()
	{
		return (this.respawnDelayMax + this.respawnDelayMin) * 0.5f / SpawnHandler.PlayerScale(ConVar.Spawn.player_scale);
	}

	// Token: 0x0600293A RID: 10554 RVA: 0x000FA5F4 File Offset: 0x000F87F4
	public float GetSpawnVariance()
	{
		return (this.respawnDelayMax - this.respawnDelayMin) * 0.5f / SpawnHandler.PlayerScale(ConVar.Spawn.player_scale);
	}

	// Token: 0x0600293B RID: 10555 RVA: 0x000FA614 File Offset: 0x000F8814
	protected void Awake()
	{
		if (TerrainMeta.TopologyMap == null)
		{
			return;
		}
		int topology = TerrainMeta.TopologyMap.GetTopology(base.transform.position);
		int num = 469762048;
		int num2 = MonumentInfo.TierToMask(this.Tier);
		if (num2 != num && (num2 & topology) == 0)
		{
			return;
		}
		this.spawnPoints = base.GetComponentsInChildren<BaseSpawnPoint>();
		if (this.WantsTimedSpawn())
		{
			this.spawnClock.Add(this.GetSpawnDelta(), this.GetSpawnVariance(), new Action(this.Spawn));
		}
		if (!this.temporary && SingletonComponent<SpawnHandler>.Instance)
		{
			SingletonComponent<SpawnHandler>.Instance.SpawnGroups.Add(this);
		}
		if (this.forceInitialSpawn)
		{
			base.Invoke(new Action(this.SpawnInitial), 1f);
		}
	}

	// Token: 0x0600293C RID: 10556 RVA: 0x000FA6DC File Offset: 0x000F88DC
	public void Fill()
	{
		if (this.isSpawnerActive)
		{
			this.Spawn(this.maxPopulation);
		}
	}

	// Token: 0x0600293D RID: 10557 RVA: 0x000FA6F4 File Offset: 0x000F88F4
	public void Clear()
	{
		for (int i = this.spawnInstances.Count - 1; i >= 0; i--)
		{
			SpawnPointInstance spawnPointInstance = this.spawnInstances[i];
			BaseEntity baseEntity = spawnPointInstance.gameObject.ToBaseEntity();
			if (this.setFreeIfMovedBeyond != null && !this.setFreeIfMovedBeyond.bounds.Contains(baseEntity.transform.position))
			{
				spawnPointInstance.Retire();
			}
			else if (baseEntity)
			{
				baseEntity.Kill(BaseNetworkable.DestroyMode.None);
			}
		}
		this.spawnInstances.Clear();
	}

	// Token: 0x0600293E RID: 10558 RVA: 0x000FA784 File Offset: 0x000F8984
	public bool HasSpawned(uint prefabID)
	{
		foreach (SpawnPointInstance spawnPointInstance in this.spawnInstances)
		{
			BaseEntity baseEntity = spawnPointInstance.gameObject.ToBaseEntity();
			if (baseEntity && baseEntity.prefabID == prefabID)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600293F RID: 10559 RVA: 0x000FA7F4 File Offset: 0x000F89F4
	public virtual void SpawnInitial()
	{
		if (!this.wantsInitialSpawn)
		{
			return;
		}
		if (this.isSpawnerActive)
		{
			if (this.fillOnSpawn)
			{
				this.Spawn(this.maxPopulation);
				return;
			}
			this.Spawn();
		}
	}

	// Token: 0x06002940 RID: 10560 RVA: 0x000FA824 File Offset: 0x000F8A24
	public void SpawnRepeating()
	{
		for (int i = 0; i < this.spawnClock.events.Count; i++)
		{
			LocalClock.TimedEvent timedEvent = this.spawnClock.events[i];
			if (UnityEngine.Time.time > timedEvent.time)
			{
				timedEvent.delta = this.GetSpawnDelta();
				timedEvent.variance = this.GetSpawnVariance();
				this.spawnClock.events[i] = timedEvent;
			}
		}
		this.spawnClock.Tick();
	}

	// Token: 0x06002941 RID: 10561 RVA: 0x000FA8A2 File Offset: 0x000F8AA2
	public void ObjectSpawned(SpawnPointInstance instance)
	{
		this.spawnInstances.Add(instance);
	}

	// Token: 0x06002942 RID: 10562 RVA: 0x000FA8B0 File Offset: 0x000F8AB0
	public void ObjectRetired(SpawnPointInstance instance)
	{
		this.spawnInstances.Remove(instance);
	}

	// Token: 0x06002943 RID: 10563 RVA: 0x000FA8BF File Offset: 0x000F8ABF
	public void DelayedSpawn()
	{
		base.Invoke(new Action(this.Spawn), 1f);
	}

	// Token: 0x06002944 RID: 10564 RVA: 0x000FA8D8 File Offset: 0x000F8AD8
	public void Spawn()
	{
		if (this.isSpawnerActive)
		{
			this.Spawn(UnityEngine.Random.Range(this.numToSpawnPerTickMin, this.numToSpawnPerTickMax + 1));
		}
	}

	// Token: 0x06002945 RID: 10565 RVA: 0x000FA8FC File Offset: 0x000F8AFC
	protected virtual void Spawn(int numToSpawn)
	{
		numToSpawn = Mathf.Min(numToSpawn, this.maxPopulation - this.currentPopulation);
		for (int i = 0; i < numToSpawn; i++)
		{
			GameObjectRef prefab = this.GetPrefab();
			if (prefab != null && !string.IsNullOrEmpty(prefab.guid))
			{
				Vector3 pos;
				Quaternion rot;
				BaseSpawnPoint spawnPoint = this.GetSpawnPoint(prefab, out pos, out rot);
				if (spawnPoint)
				{
					BaseEntity baseEntity = GameManager.server.CreateEntity(prefab.resourcePath, pos, rot, false);
					if (baseEntity)
					{
						if (baseEntity.enableSaving && !(spawnPoint is SpaceCheckingSpawnPoint))
						{
							baseEntity.enableSaving = false;
						}
						baseEntity.gameObject.AwakeFromInstantiate();
						baseEntity.Spawn();
						this.PostSpawnProcess(baseEntity, spawnPoint);
						SpawnPointInstance spawnPointInstance = baseEntity.gameObject.AddComponent<SpawnPointInstance>();
						spawnPointInstance.parentSpawnPointUser = this;
						spawnPointInstance.parentSpawnPoint = spawnPoint;
						spawnPointInstance.Notify();
					}
				}
			}
		}
	}

	// Token: 0x06002946 RID: 10566 RVA: 0x000059DD File Offset: 0x00003BDD
	protected virtual void PostSpawnProcess(BaseEntity entity, BaseSpawnPoint spawnPoint)
	{
	}

	// Token: 0x06002947 RID: 10567 RVA: 0x000FA9D8 File Offset: 0x000F8BD8
	protected GameObjectRef GetPrefab()
	{
		float num = (float)this.prefabs.Sum(delegate(SpawnGroup.SpawnEntry x)
		{
			if (!this.preventDuplicates || !this.HasSpawned(x.prefab.resourceID))
			{
				return x.weight;
			}
			return 0;
		});
		if (num == 0f)
		{
			return null;
		}
		float num2 = UnityEngine.Random.Range(0f, num);
		foreach (SpawnGroup.SpawnEntry spawnEntry in this.prefabs)
		{
			int num3 = (this.preventDuplicates && this.HasSpawned(spawnEntry.prefab.resourceID)) ? 0 : spawnEntry.weight;
			if ((num2 -= (float)num3) <= 0f)
			{
				return spawnEntry.prefab;
			}
		}
		return this.prefabs[this.prefabs.Count - 1].prefab;
	}

	// Token: 0x06002948 RID: 10568 RVA: 0x000FAAB4 File Offset: 0x000F8CB4
	protected virtual BaseSpawnPoint GetSpawnPoint(GameObjectRef prefabRef, out Vector3 pos, out Quaternion rot)
	{
		BaseSpawnPoint baseSpawnPoint = null;
		pos = Vector3.zero;
		rot = Quaternion.identity;
		int num = UnityEngine.Random.Range(0, this.spawnPoints.Length);
		for (int i = 0; i < this.spawnPoints.Length; i++)
		{
			BaseSpawnPoint baseSpawnPoint2 = this.spawnPoints[(num + i) % this.spawnPoints.Length];
			if (!(baseSpawnPoint2 == null) && baseSpawnPoint2.IsAvailableTo(prefabRef) && !baseSpawnPoint2.HasPlayersIntersecting())
			{
				baseSpawnPoint = baseSpawnPoint2;
				break;
			}
		}
		if (baseSpawnPoint)
		{
			baseSpawnPoint.GetLocation(out pos, out rot);
		}
		return baseSpawnPoint;
	}

	// Token: 0x06002949 RID: 10569 RVA: 0x000FAB3E File Offset: 0x000F8D3E
	protected virtual void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 1f, 0f, 1f);
		Gizmos.DrawSphere(base.transform.position, 0.25f);
	}

	// Token: 0x04002174 RID: 8564
	[InspectorFlags]
	public MonumentTier Tier = (MonumentTier)(-1);

	// Token: 0x04002175 RID: 8565
	public List<SpawnGroup.SpawnEntry> prefabs;

	// Token: 0x04002176 RID: 8566
	public int maxPopulation = 5;

	// Token: 0x04002177 RID: 8567
	public int numToSpawnPerTickMin = 1;

	// Token: 0x04002178 RID: 8568
	public int numToSpawnPerTickMax = 2;

	// Token: 0x04002179 RID: 8569
	public float respawnDelayMin = 10f;

	// Token: 0x0400217A RID: 8570
	public float respawnDelayMax = 20f;

	// Token: 0x0400217B RID: 8571
	public bool wantsInitialSpawn = true;

	// Token: 0x0400217C RID: 8572
	public bool temporary;

	// Token: 0x0400217D RID: 8573
	public bool forceInitialSpawn;

	// Token: 0x0400217E RID: 8574
	public bool preventDuplicates;

	// Token: 0x0400217F RID: 8575
	public bool isSpawnerActive = true;

	// Token: 0x04002180 RID: 8576
	public BoxCollider setFreeIfMovedBeyond;

	// Token: 0x04002181 RID: 8577
	protected bool fillOnSpawn;

	// Token: 0x04002182 RID: 8578
	protected BaseSpawnPoint[] spawnPoints;

	// Token: 0x04002183 RID: 8579
	private List<SpawnPointInstance> spawnInstances = new List<SpawnPointInstance>();

	// Token: 0x04002184 RID: 8580
	private LocalClock spawnClock = new LocalClock();

	// Token: 0x02000CF8 RID: 3320
	[Serializable]
	public class SpawnEntry
	{
		// Token: 0x0400447D RID: 17533
		public GameObjectRef prefab;

		// Token: 0x0400447E RID: 17534
		public int weight = 1;

		// Token: 0x0400447F RID: 17535
		public bool mobile;
	}
}
