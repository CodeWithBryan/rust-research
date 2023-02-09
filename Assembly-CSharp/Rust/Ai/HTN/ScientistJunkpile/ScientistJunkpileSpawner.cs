using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistJunkpile
{
	// Token: 0x02000B02 RID: 2818
	public class ScientistJunkpileSpawner : MonoBehaviour, IServerComponent, ISpawnGroup
	{
		// Token: 0x170005EB RID: 1515
		// (get) Token: 0x060043B0 RID: 17328 RVA: 0x00188AF7 File Offset: 0x00186CF7
		public int currentPopulation
		{
			get
			{
				return this.Spawned.Count;
			}
		}

		// Token: 0x060043B1 RID: 17329 RVA: 0x00188B04 File Offset: 0x00186D04
		private void Awake()
		{
			this.SpawnPoints = base.GetComponentsInChildren<BaseSpawnPoint>();
			if (SingletonComponent<SpawnHandler>.Instance)
			{
				SingletonComponent<SpawnHandler>.Instance.SpawnGroups.Add(this);
			}
		}

		// Token: 0x060043B2 RID: 17330 RVA: 0x00188B2E File Offset: 0x00186D2E
		public void Fill()
		{
			this.DoRespawn();
		}

		// Token: 0x060043B3 RID: 17331 RVA: 0x00188B38 File Offset: 0x00186D38
		public void Clear()
		{
			if (this.Spawned == null)
			{
				return;
			}
			foreach (BaseCombatEntity baseCombatEntity in this.Spawned)
			{
				if (!(baseCombatEntity == null) && !(baseCombatEntity.gameObject == null) && !(baseCombatEntity.transform == null))
				{
					BaseEntity baseEntity = baseCombatEntity.gameObject.ToBaseEntity();
					if (baseEntity)
					{
						baseEntity.Kill(BaseNetworkable.DestroyMode.None);
					}
				}
			}
			this.Spawned.Clear();
		}

		// Token: 0x060043B4 RID: 17332 RVA: 0x00188BD8 File Offset: 0x00186DD8
		public void SpawnInitial()
		{
			this.nextRespawnTime = UnityEngine.Time.time + UnityEngine.Random.Range(3f, 4f);
			this.pendingRespawn = true;
		}

		// Token: 0x060043B5 RID: 17333 RVA: 0x00188BFC File Offset: 0x00186DFC
		public void SpawnRepeating()
		{
			this.CheckIfRespawnNeeded();
		}

		// Token: 0x060043B6 RID: 17334 RVA: 0x00188C04 File Offset: 0x00186E04
		public void CheckIfRespawnNeeded()
		{
			if (!this.pendingRespawn)
			{
				if (this.Spawned == null || this.Spawned.Count == 0 || this.IsAllSpawnedDead())
				{
					this.ScheduleRespawn();
					return;
				}
			}
			else if ((this.Spawned == null || this.Spawned.Count == 0 || this.IsAllSpawnedDead()) && UnityEngine.Time.time >= this.nextRespawnTime)
			{
				this.DoRespawn();
			}
		}

		// Token: 0x060043B7 RID: 17335 RVA: 0x00188C70 File Offset: 0x00186E70
		private bool IsAllSpawnedDead()
		{
			for (int i = 0; i < this.Spawned.Count; i++)
			{
				BaseCombatEntity baseCombatEntity = this.Spawned[i];
				if (!(baseCombatEntity == null) && !(baseCombatEntity.transform == null) && !baseCombatEntity.IsDestroyed && !baseCombatEntity.IsDead())
				{
					return false;
				}
				this.Spawned.RemoveAt(i);
				i--;
			}
			return true;
		}

		// Token: 0x060043B8 RID: 17336 RVA: 0x00188CDC File Offset: 0x00186EDC
		public void ScheduleRespawn()
		{
			this.nextRespawnTime = UnityEngine.Time.time + UnityEngine.Random.Range(this.MinRespawnTimeMinutes, this.MaxRespawnTimeMinutes) * 60f;
			this.pendingRespawn = true;
		}

		// Token: 0x060043B9 RID: 17337 RVA: 0x00188D08 File Offset: 0x00186F08
		public void DoRespawn()
		{
			if (!Application.isLoading && !Application.isLoadingSave)
			{
				this.SpawnScientist();
			}
			this.pendingRespawn = false;
		}

		// Token: 0x060043BA RID: 17338 RVA: 0x00188D28 File Offset: 0x00186F28
		public void SpawnScientist()
		{
			if (!AI.npc_enable)
			{
				return;
			}
			if (this.Spawned == null || this.Spawned.Count >= this.MaxPopulation)
			{
				return;
			}
			float num = this.SpawnBaseChance;
			ScientistJunkpileSpawner.JunkpileType spawnType = this.SpawnType;
			if (spawnType != ScientistJunkpileSpawner.JunkpileType.A)
			{
				if (spawnType == ScientistJunkpileSpawner.JunkpileType.G)
				{
					num = AI.npc_junkpile_g_spawn_chance;
				}
			}
			else
			{
				num = AI.npc_junkpile_a_spawn_chance;
			}
			if (UnityEngine.Random.value > num)
			{
				return;
			}
			int num2 = this.MaxPopulation - this.Spawned.Count;
			for (int i = 0; i < num2; i++)
			{
				Vector3 pos;
				Quaternion rot;
				if (!(this.GetSpawnPoint(out pos, out rot) == null))
				{
					BaseEntity baseEntity = GameManager.server.CreateEntity(this.ScientistPrefab.resourcePath, pos, rot, false);
					if (!(baseEntity != null))
					{
						return;
					}
					baseEntity.enableSaving = false;
					baseEntity.gameObject.AwakeFromInstantiate();
					baseEntity.Spawn();
					this.Spawned.Add((BaseCombatEntity)baseEntity);
				}
			}
		}

		// Token: 0x060043BB RID: 17339 RVA: 0x00188E10 File Offset: 0x00187010
		private BaseSpawnPoint GetSpawnPoint(out Vector3 pos, out Quaternion rot)
		{
			BaseSpawnPoint baseSpawnPoint = null;
			pos = Vector3.zero;
			rot = Quaternion.identity;
			int num = UnityEngine.Random.Range(0, this.SpawnPoints.Length);
			for (int i = 0; i < this.SpawnPoints.Length; i++)
			{
				baseSpawnPoint = this.SpawnPoints[(num + i) % this.SpawnPoints.Length];
				if (baseSpawnPoint && baseSpawnPoint.gameObject.activeSelf)
				{
					break;
				}
			}
			if (baseSpawnPoint)
			{
				baseSpawnPoint.GetLocation(out pos, out rot);
			}
			return baseSpawnPoint;
		}

		// Token: 0x04003C40 RID: 15424
		public GameObjectRef ScientistPrefab;

		// Token: 0x04003C41 RID: 15425
		[NonSerialized]
		public List<BaseCombatEntity> Spawned = new List<BaseCombatEntity>();

		// Token: 0x04003C42 RID: 15426
		[NonSerialized]
		public BaseSpawnPoint[] SpawnPoints;

		// Token: 0x04003C43 RID: 15427
		public int MaxPopulation = 1;

		// Token: 0x04003C44 RID: 15428
		public bool InitialSpawn;

		// Token: 0x04003C45 RID: 15429
		public float MinRespawnTimeMinutes = 120f;

		// Token: 0x04003C46 RID: 15430
		public float MaxRespawnTimeMinutes = 120f;

		// Token: 0x04003C47 RID: 15431
		public float MovementRadius = -1f;

		// Token: 0x04003C48 RID: 15432
		public bool ReducedLongRangeAccuracy;

		// Token: 0x04003C49 RID: 15433
		public ScientistJunkpileSpawner.JunkpileType SpawnType;

		// Token: 0x04003C4A RID: 15434
		[Range(0f, 1f)]
		public float SpawnBaseChance = 1f;

		// Token: 0x04003C4B RID: 15435
		private float nextRespawnTime;

		// Token: 0x04003C4C RID: 15436
		private bool pendingRespawn;

		// Token: 0x02000F3A RID: 3898
		public enum JunkpileType
		{
			// Token: 0x04004DC6 RID: 19910
			A,
			// Token: 0x04004DC7 RID: 19911
			B,
			// Token: 0x04004DC8 RID: 19912
			C,
			// Token: 0x04004DC9 RID: 19913
			D,
			// Token: 0x04004DCA RID: 19914
			E,
			// Token: 0x04004DCB RID: 19915
			F,
			// Token: 0x04004DCC RID: 19916
			G
		}
	}
}
