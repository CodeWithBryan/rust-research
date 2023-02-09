using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
	// Token: 0x02000AFF RID: 2815
	public class AiLocationManager : FacepunchBehaviour, IServerComponent
	{
		// Token: 0x170005E8 RID: 1512
		// (get) Token: 0x060043A0 RID: 17312 RVA: 0x001884B5 File Offset: 0x001866B5
		public AiLocationSpawner.SquadSpawnerLocation LocationType
		{
			get
			{
				if (this.MainSpawner != null)
				{
					return this.MainSpawner.Location;
				}
				return this.LocationWhenMainSpawnerIsNull;
			}
		}

		// Token: 0x060043A1 RID: 17313 RVA: 0x001884D8 File Offset: 0x001866D8
		private void Awake()
		{
			AiLocationManager.Managers.Add(this);
			if (this.SnapCoverPointsToGround)
			{
				foreach (AICoverPoint aicoverPoint in this.CoverPointGroup.GetComponentsInChildren<AICoverPoint>())
				{
					NavMeshHit navMeshHit;
					if (NavMesh.SamplePosition(aicoverPoint.transform.position, out navMeshHit, 4f, -1))
					{
						aicoverPoint.transform.position = navMeshHit.position;
					}
				}
			}
		}

		// Token: 0x060043A2 RID: 17314 RVA: 0x00188542 File Offset: 0x00186742
		private void OnDestroy()
		{
			AiLocationManager.Managers.Remove(this);
		}

		// Token: 0x060043A3 RID: 17315 RVA: 0x00188550 File Offset: 0x00186750
		public PathInterestNode GetFirstPatrolPointInRange(Vector3 from, float minRange = 10f, float maxRange = 100f)
		{
			if (this.PatrolPointGroup == null)
			{
				return null;
			}
			if (this.patrolPoints == null)
			{
				this.patrolPoints = new List<PathInterestNode>(this.PatrolPointGroup.GetComponentsInChildren<PathInterestNode>());
			}
			if (this.patrolPoints.Count == 0)
			{
				return null;
			}
			float num = minRange * minRange;
			float num2 = maxRange * maxRange;
			foreach (PathInterestNode pathInterestNode in this.patrolPoints)
			{
				float sqrMagnitude = (pathInterestNode.transform.position - from).sqrMagnitude;
				if (sqrMagnitude >= num && sqrMagnitude <= num2)
				{
					return pathInterestNode;
				}
			}
			return null;
		}

		// Token: 0x060043A4 RID: 17316 RVA: 0x00188610 File Offset: 0x00186810
		public PathInterestNode GetRandomPatrolPointInRange(Vector3 from, float minRange = 10f, float maxRange = 100f, PathInterestNode currentPatrolPoint = null)
		{
			if (this.PatrolPointGroup == null)
			{
				return null;
			}
			if (this.patrolPoints == null)
			{
				this.patrolPoints = new List<PathInterestNode>(this.PatrolPointGroup.GetComponentsInChildren<PathInterestNode>());
			}
			if (this.patrolPoints.Count == 0)
			{
				return null;
			}
			float num = minRange * minRange;
			float num2 = maxRange * maxRange;
			for (int i = 0; i < 20; i++)
			{
				PathInterestNode pathInterestNode = this.patrolPoints[UnityEngine.Random.Range(0, this.patrolPoints.Count)];
				if (UnityEngine.Time.time < pathInterestNode.NextVisitTime)
				{
					if (pathInterestNode == currentPatrolPoint)
					{
						return null;
					}
				}
				else
				{
					float sqrMagnitude = (pathInterestNode.transform.position - from).sqrMagnitude;
					if (sqrMagnitude >= num && sqrMagnitude <= num2)
					{
						pathInterestNode.NextVisitTime = UnityEngine.Time.time + AI.npc_patrol_point_cooldown;
						return pathInterestNode;
					}
				}
			}
			return null;
		}

		// Token: 0x04003C2E RID: 15406
		public static List<AiLocationManager> Managers = new List<AiLocationManager>();

		// Token: 0x04003C2F RID: 15407
		[SerializeField]
		public AiLocationSpawner MainSpawner;

		// Token: 0x04003C30 RID: 15408
		[SerializeField]
		public AiLocationSpawner.SquadSpawnerLocation LocationWhenMainSpawnerIsNull = AiLocationSpawner.SquadSpawnerLocation.None;

		// Token: 0x04003C31 RID: 15409
		public Transform CoverPointGroup;

		// Token: 0x04003C32 RID: 15410
		public Transform PatrolPointGroup;

		// Token: 0x04003C33 RID: 15411
		public CoverPointVolume DynamicCoverPointVolume;

		// Token: 0x04003C34 RID: 15412
		public bool SnapCoverPointsToGround;

		// Token: 0x04003C35 RID: 15413
		private List<PathInterestNode> patrolPoints;
	}
}
