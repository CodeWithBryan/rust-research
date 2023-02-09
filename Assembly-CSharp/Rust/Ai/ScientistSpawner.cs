using System;
using ConVar;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000AFE RID: 2814
	public class ScientistSpawner : SpawnGroup
	{
		// Token: 0x0600439C RID: 17308 RVA: 0x00188330 File Offset: 0x00186530
		protected override void Spawn(int numToSpawn)
		{
			if (!AI.npc_enable)
			{
				return;
			}
			if (base.currentPopulation == this.maxPopulation)
			{
				this._lastSpawnCallHadMaxAliveMembers = true;
				this._lastSpawnCallHadAliveMembers = true;
				return;
			}
			if (this._lastSpawnCallHadMaxAliveMembers)
			{
				this._nextForcedRespawn = UnityEngine.Time.time + 2200f;
			}
			if (UnityEngine.Time.time < this._nextForcedRespawn)
			{
				if (base.currentPopulation == 0 && this._lastSpawnCallHadAliveMembers)
				{
					this._lastSpawnCallHadMaxAliveMembers = false;
					this._lastSpawnCallHadAliveMembers = false;
					return;
				}
				if (base.currentPopulation > 0)
				{
					this._lastSpawnCallHadMaxAliveMembers = false;
					this._lastSpawnCallHadAliveMembers = (base.currentPopulation > 0);
					return;
				}
			}
			this._lastSpawnCallHadMaxAliveMembers = false;
			this._lastSpawnCallHadAliveMembers = (base.currentPopulation > 0);
			base.Spawn(numToSpawn);
		}

		// Token: 0x0600439D RID: 17309 RVA: 0x000059DD File Offset: 0x00003BDD
		protected override void PostSpawnProcess(BaseEntity entity, BaseSpawnPoint spawnPoint)
		{
		}

		// Token: 0x0600439E RID: 17310 RVA: 0x001883E4 File Offset: 0x001865E4
		protected override void OnDrawGizmos()
		{
			base.OnDrawGizmos();
			if (this.LookAtInterestPointsStationary != null && this.LookAtInterestPointsStationary.Length != 0)
			{
				Gizmos.color = Color.magenta - new Color(0f, 0f, 0f, 0.5f);
				foreach (Transform transform in this.LookAtInterestPointsStationary)
				{
					if (transform != null)
					{
						Gizmos.DrawSphere(transform.position, 0.1f);
						Gizmos.DrawLine(base.transform.position, transform.position);
					}
				}
			}
		}

		// Token: 0x04003C1F RID: 15391
		[Header("Scientist Spawner")]
		public bool Mobile = true;

		// Token: 0x04003C20 RID: 15392
		public bool NeverMove;

		// Token: 0x04003C21 RID: 15393
		public bool SpawnHostile;

		// Token: 0x04003C22 RID: 15394
		public bool OnlyAggroMarkedTargets = true;

		// Token: 0x04003C23 RID: 15395
		public bool IsPeacekeeper = true;

		// Token: 0x04003C24 RID: 15396
		public bool IsBandit;

		// Token: 0x04003C25 RID: 15397
		public bool IsMilitaryTunnelLab;

		// Token: 0x04003C26 RID: 15398
		public WaypointSet Waypoints;

		// Token: 0x04003C27 RID: 15399
		public Transform[] LookAtInterestPointsStationary;

		// Token: 0x04003C28 RID: 15400
		public Vector2 RadioEffectRepeatRange = new Vector2(10f, 15f);

		// Token: 0x04003C29 RID: 15401
		public Model Model;

		// Token: 0x04003C2A RID: 15402
		[SerializeField]
		private AiLocationManager _mgr;

		// Token: 0x04003C2B RID: 15403
		private float _nextForcedRespawn = float.PositiveInfinity;

		// Token: 0x04003C2C RID: 15404
		private bool _lastSpawnCallHadAliveMembers;

		// Token: 0x04003C2D RID: 15405
		private bool _lastSpawnCallHadMaxAliveMembers;
	}
}
