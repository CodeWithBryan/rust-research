using System;
using ConVar;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000B00 RID: 2816
	public class AiLocationSpawner : SpawnGroup
	{
		// Token: 0x060043A7 RID: 17319 RVA: 0x001886FC File Offset: 0x001868FC
		public override void SpawnInitial()
		{
			if (this.IsMainSpawner)
			{
				if (this.Location == AiLocationSpawner.SquadSpawnerLocation.MilitaryTunnels)
				{
					this.maxPopulation = AI.npc_max_population_military_tunnels;
					this.numToSpawnPerTickMax = AI.npc_spawn_per_tick_max_military_tunnels;
					this.numToSpawnPerTickMin = AI.npc_spawn_per_tick_min_military_tunnels;
					this.respawnDelayMax = AI.npc_respawn_delay_max_military_tunnels;
					this.respawnDelayMin = AI.npc_respawn_delay_min_military_tunnels;
				}
				else
				{
					this.defaultMaxPopulation = this.maxPopulation;
					this.defaultNumToSpawnPerTickMax = this.numToSpawnPerTickMax;
					this.defaultNumToSpawnPerTickMin = this.numToSpawnPerTickMin;
				}
			}
			else
			{
				this.defaultMaxPopulation = this.maxPopulation;
				this.defaultNumToSpawnPerTickMax = this.numToSpawnPerTickMax;
				this.defaultNumToSpawnPerTickMin = this.numToSpawnPerTickMin;
			}
			base.SpawnInitial();
		}

		// Token: 0x060043A8 RID: 17320 RVA: 0x001887A4 File Offset: 0x001869A4
		protected override void Spawn(int numToSpawn)
		{
			if (!AI.npc_enable)
			{
				this.maxPopulation = 0;
				this.numToSpawnPerTickMax = 0;
				this.numToSpawnPerTickMin = 0;
				return;
			}
			if (numToSpawn == 0)
			{
				if (this.IsMainSpawner)
				{
					if (this.Location == AiLocationSpawner.SquadSpawnerLocation.MilitaryTunnels)
					{
						this.maxPopulation = AI.npc_max_population_military_tunnels;
						this.numToSpawnPerTickMax = AI.npc_spawn_per_tick_max_military_tunnels;
						this.numToSpawnPerTickMin = AI.npc_spawn_per_tick_min_military_tunnels;
						numToSpawn = UnityEngine.Random.Range(this.numToSpawnPerTickMin, this.numToSpawnPerTickMax + 1);
					}
					else
					{
						this.maxPopulation = this.defaultMaxPopulation;
						this.numToSpawnPerTickMax = this.defaultNumToSpawnPerTickMax;
						this.numToSpawnPerTickMin = this.defaultNumToSpawnPerTickMin;
						numToSpawn = UnityEngine.Random.Range(this.numToSpawnPerTickMin, this.numToSpawnPerTickMax + 1);
					}
				}
				else
				{
					this.maxPopulation = this.defaultMaxPopulation;
					this.numToSpawnPerTickMax = this.defaultNumToSpawnPerTickMax;
					this.numToSpawnPerTickMin = this.defaultNumToSpawnPerTickMin;
					numToSpawn = UnityEngine.Random.Range(this.numToSpawnPerTickMin, this.numToSpawnPerTickMax + 1);
				}
			}
			float num = this.chance;
			AiLocationSpawner.SquadSpawnerLocation location = this.Location;
			if (location != AiLocationSpawner.SquadSpawnerLocation.JunkpileA)
			{
				if (location == AiLocationSpawner.SquadSpawnerLocation.JunkpileG)
				{
					num = AI.npc_junkpile_g_spawn_chance;
				}
			}
			else
			{
				num = AI.npc_junkpile_a_spawn_chance;
			}
			if (numToSpawn == 0 || UnityEngine.Random.value > num)
			{
				return;
			}
			numToSpawn = Mathf.Min(numToSpawn, this.maxPopulation - base.currentPopulation);
			for (int i = 0; i < numToSpawn; i++)
			{
				GameObjectRef prefab = base.GetPrefab();
				Vector3 pos;
				Quaternion rot;
				BaseSpawnPoint spawnPoint = this.GetSpawnPoint(prefab, out pos, out rot);
				if (spawnPoint)
				{
					BaseEntity baseEntity = GameManager.server.CreateEntity(prefab.resourcePath, pos, rot, true);
					if (baseEntity)
					{
						baseEntity.Spawn();
						SpawnPointInstance spawnPointInstance = baseEntity.gameObject.AddComponent<SpawnPointInstance>();
						spawnPointInstance.parentSpawnPointUser = this;
						spawnPointInstance.parentSpawnPoint = spawnPoint;
						spawnPointInstance.Notify();
					}
				}
			}
		}

		// Token: 0x060043A9 RID: 17321 RVA: 0x0018894B File Offset: 0x00186B4B
		protected override BaseSpawnPoint GetSpawnPoint(GameObjectRef prefabRef, out Vector3 pos, out Quaternion rot)
		{
			return base.GetSpawnPoint(prefabRef, out pos, out rot);
		}

		// Token: 0x04003C36 RID: 15414
		public AiLocationSpawner.SquadSpawnerLocation Location;

		// Token: 0x04003C37 RID: 15415
		public AiLocationManager Manager;

		// Token: 0x04003C38 RID: 15416
		public JunkPile Junkpile;

		// Token: 0x04003C39 RID: 15417
		public bool IsMainSpawner = true;

		// Token: 0x04003C3A RID: 15418
		public float chance = 1f;

		// Token: 0x04003C3B RID: 15419
		private int defaultMaxPopulation;

		// Token: 0x04003C3C RID: 15420
		private int defaultNumToSpawnPerTickMax;

		// Token: 0x04003C3D RID: 15421
		private int defaultNumToSpawnPerTickMin;

		// Token: 0x02000F37 RID: 3895
		public enum SquadSpawnerLocation
		{
			// Token: 0x04004DB6 RID: 19894
			MilitaryTunnels,
			// Token: 0x04004DB7 RID: 19895
			JunkpileA,
			// Token: 0x04004DB8 RID: 19896
			JunkpileG,
			// Token: 0x04004DB9 RID: 19897
			CH47,
			// Token: 0x04004DBA RID: 19898
			None,
			// Token: 0x04004DBB RID: 19899
			Compound,
			// Token: 0x04004DBC RID: 19900
			BanditTown,
			// Token: 0x04004DBD RID: 19901
			CargoShip
		}
	}
}
