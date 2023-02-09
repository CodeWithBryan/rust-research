using System;
using UnityEngine;

// Token: 0x020001DF RID: 479
public class ScientistBrain : BaseAIBrain
{
	// Token: 0x06001917 RID: 6423 RVA: 0x000B66A0 File Offset: 0x000B48A0
	public override void AddStates()
	{
		base.AddStates();
		base.AddState(new ScientistBrain.IdleState());
		base.AddState(new ScientistBrain.RoamState());
		base.AddState(new ScientistBrain.ChaseState());
		base.AddState(new ScientistBrain.CombatState());
		base.AddState(new ScientistBrain.TakeCoverState());
		base.AddState(new ScientistBrain.CoverState());
		base.AddState(new ScientistBrain.MountedState());
		base.AddState(new ScientistBrain.DismountedState());
		base.AddState(new BaseAIBrain.BaseFollowPathState());
		base.AddState(new BaseAIBrain.BaseNavigateHomeState());
		base.AddState(new ScientistBrain.CombatStationaryState());
		base.AddState(new BaseAIBrain.BaseMoveTorwardsState());
		base.AddState(new ScientistBrain.MoveToVector3State());
	}

	// Token: 0x06001918 RID: 6424 RVA: 0x000B6744 File Offset: 0x000B4944
	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		this.thinkRate = 0.25f;
		base.PathFinder = new HumanPathFinder();
		((HumanPathFinder)base.PathFinder).Init(this.GetBaseEntity());
		ScientistBrain.Count++;
	}

	// Token: 0x06001919 RID: 6425 RVA: 0x000B6796 File Offset: 0x000B4996
	public override void OnDestroy()
	{
		base.OnDestroy();
		ScientistBrain.Count--;
	}

	// Token: 0x0600191A RID: 6426 RVA: 0x000B67AA File Offset: 0x000B49AA
	public HumanNPC GetEntity()
	{
		return this.GetBaseEntity() as HumanNPC;
	}

	// Token: 0x0600191B RID: 6427 RVA: 0x000B67B8 File Offset: 0x000B49B8
	protected override void OnStateChanged()
	{
		base.OnStateChanged();
		if (base.CurrentState != null)
		{
			AIState stateType = base.CurrentState.StateType;
			if (stateType <= AIState.Patrol)
			{
				if (stateType - AIState.Idle > 1 && stateType != AIState.Patrol)
				{
					goto IL_46;
				}
			}
			else if (stateType != AIState.FollowPath && stateType != AIState.Cooldown)
			{
				goto IL_46;
			}
			this.GetEntity().SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, true);
			return;
			IL_46:
			this.GetEntity().SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, false);
		}
	}

	// Token: 0x040011E1 RID: 4577
	public static int Count;

	// Token: 0x02000BFF RID: 3071
	public class ChaseState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004BC1 RID: 19393 RVA: 0x00193176 File Offset: 0x00191376
		public ChaseState() : base(AIState.Chase)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004BC2 RID: 19394 RVA: 0x0019318D File Offset: 0x0019138D
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004BC3 RID: 19395 RVA: 0x0019319D File Offset: 0x0019139D
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Error;
			if (brain.PathFinder == null)
			{
				return;
			}
			this.status = StateStatus.Running;
			this.nextPositionUpdateTime = 0f;
		}

		// Token: 0x06004BC4 RID: 19396 RVA: 0x001931C9 File Offset: 0x001913C9
		private void Stop()
		{
			this.brain.Navigator.Stop();
			this.brain.Navigator.ClearFacingDirectionOverride();
		}

		// Token: 0x06004BC5 RID: 19397 RVA: 0x001931EC File Offset: 0x001913EC
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			if (this.status == StateStatus.Error)
			{
				return this.status;
			}
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				return StateStatus.Error;
			}
			float num = Vector3.Distance(baseEntity.transform.position, entity.transform.position);
			if (brain.Senses.Memory.IsLOS(baseEntity) || num <= 10f || base.TimeInState <= 5f)
			{
				brain.Navigator.SetFacingDirectionEntity(baseEntity);
			}
			else
			{
				brain.Navigator.ClearFacingDirectionOverride();
			}
			if (num <= 10f)
			{
				brain.Navigator.SetCurrentSpeed(BaseNavigator.NavigationSpeed.Normal);
			}
			else
			{
				brain.Navigator.SetCurrentSpeed(BaseNavigator.NavigationSpeed.Fast);
			}
			if (Time.time > this.nextPositionUpdateTime)
			{
				this.nextPositionUpdateTime = Time.time + UnityEngine.Random.Range(0.5f, 1f);
				Vector3 pos = entity.transform.position;
				AIInformationZone informationZone = (entity as HumanNPC).GetInformationZone(baseEntity.transform.position);
				bool flag = false;
				if (informationZone != null)
				{
					AIMovePoint bestMovePointNear = informationZone.GetBestMovePointNear(baseEntity.transform.position, entity.transform.position, 0f, brain.Navigator.BestMovementPointMaxDistance, true, entity, true);
					if (bestMovePointNear)
					{
						bestMovePointNear.SetUsedBy(entity, 5f);
						pos = brain.PathFinder.GetRandomPositionAround(bestMovePointNear.transform.position, 0f, bestMovePointNear.radius - 0.3f);
						flag = true;
					}
				}
				if (!flag)
				{
					return StateStatus.Error;
				}
				if (num < 10f)
				{
					brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Normal, 0f, 0f);
				}
				else
				{
					brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
				}
			}
			if (brain.Navigator.Moving)
			{
				return StateStatus.Running;
			}
			return StateStatus.Finished;
		}

		// Token: 0x0400405D RID: 16477
		private StateStatus status = StateStatus.Error;

		// Token: 0x0400405E RID: 16478
		private float nextPositionUpdateTime;
	}

	// Token: 0x02000C00 RID: 3072
	public class CombatState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004BC6 RID: 19398 RVA: 0x001933DC File Offset: 0x001915DC
		public CombatState() : base(AIState.Combat)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004BC7 RID: 19399 RVA: 0x001933EC File Offset: 0x001915EC
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.combatStartPosition = entity.transform.position;
			this.FaceTarget();
		}

		// Token: 0x06004BC8 RID: 19400 RVA: 0x0019340D File Offset: 0x0019160D
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			(entity as HumanNPC).SetDucked(false);
			brain.Navigator.ClearFacingDirectionOverride();
		}

		// Token: 0x06004BC9 RID: 19401 RVA: 0x00193430 File Offset: 0x00191630
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			HumanNPC humanNPC = entity as HumanNPC;
			this.FaceTarget();
			if (Time.time > this.nextActionTime)
			{
				if (UnityEngine.Random.Range(0, 3) == 1)
				{
					this.nextActionTime = Time.time + UnityEngine.Random.Range(1f, 2f);
					humanNPC.SetDucked(true);
					brain.Navigator.Stop();
				}
				else
				{
					this.nextActionTime = Time.time + UnityEngine.Random.Range(2f, 3f);
					humanNPC.SetDucked(false);
					brain.Navigator.SetDestination(brain.PathFinder.GetRandomPositionAround(this.combatStartPosition, 1f, 2f), BaseNavigator.NavigationSpeed.Normal, 0f, 0f);
				}
			}
			return StateStatus.Running;
		}

		// Token: 0x06004BCA RID: 19402 RVA: 0x001934F4 File Offset: 0x001916F4
		private void FaceTarget()
		{
			BaseEntity baseEntity = this.brain.Events.Memory.Entity.Get(this.brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				this.brain.Navigator.ClearFacingDirectionOverride();
				return;
			}
			this.brain.Navigator.SetFacingDirectionEntity(baseEntity);
		}

		// Token: 0x0400405F RID: 16479
		private float nextActionTime;

		// Token: 0x04004060 RID: 16480
		private Vector3 combatStartPosition;
	}

	// Token: 0x02000C01 RID: 3073
	public class CombatStationaryState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004BCB RID: 19403 RVA: 0x00193557 File Offset: 0x00191757
		public CombatStationaryState() : base(AIState.CombatStationary)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004BCC RID: 19404 RVA: 0x001924E6 File Offset: 0x001906E6
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.ClearFacingDirectionOverride();
		}

		// Token: 0x06004BCD RID: 19405 RVA: 0x00193568 File Offset: 0x00191768
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity != null)
			{
				brain.Navigator.SetFacingDirectionEntity(baseEntity);
			}
			else
			{
				brain.Navigator.ClearFacingDirectionOverride();
			}
			return StateStatus.Running;
		}
	}

	// Token: 0x02000C02 RID: 3074
	public class CoverState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004BCE RID: 19406 RVA: 0x001935C3 File Offset: 0x001917C3
		public CoverState() : base(AIState.Cover)
		{
		}

		// Token: 0x06004BCF RID: 19407 RVA: 0x001935CC File Offset: 0x001917CC
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			HumanNPC humanNPC = entity as HumanNPC;
			humanNPC.SetDucked(true);
			AIPoint aipoint = brain.Events.Memory.AIPoint.Get(4);
			if (aipoint != null)
			{
				aipoint.SetUsedBy(entity);
			}
			if (humanNPC.healthFraction <= brain.HealBelowHealthFraction && UnityEngine.Random.Range(0f, 1f) <= brain.HealChance)
			{
				Item item = humanNPC.FindHealingItem();
				if (item != null)
				{
					BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
					if (baseEntity == null || (!brain.Senses.Memory.IsLOS(baseEntity) && Vector3.Distance(entity.transform.position, baseEntity.transform.position) >= 5f))
					{
						humanNPC.UseHealingItem(item);
					}
				}
			}
		}

		// Token: 0x06004BD0 RID: 19408 RVA: 0x001936B0 File Offset: 0x001918B0
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			(entity as HumanNPC).SetDucked(false);
			brain.Navigator.ClearFacingDirectionOverride();
			AIPoint aipoint = brain.Events.Memory.AIPoint.Get(4);
			if (aipoint != null)
			{
				aipoint.ClearIfUsedBy(entity);
			}
		}

		// Token: 0x06004BD1 RID: 19409 RVA: 0x00193704 File Offset: 0x00191904
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			HumanNPC humanNPC = entity as HumanNPC;
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			float num = humanNPC.AmmoFractionRemaining();
			if (num == 0f || (baseEntity != null && !brain.Senses.Memory.IsLOS(baseEntity) && num < 0.25f))
			{
				humanNPC.AttemptReload();
			}
			if (baseEntity != null)
			{
				brain.Navigator.SetFacingDirectionEntity(baseEntity);
			}
			return StateStatus.Running;
		}
	}

	// Token: 0x02000C03 RID: 3075
	public class DismountedState : BaseAIBrain.BaseDismountedState
	{
		// Token: 0x06004BD2 RID: 19410 RVA: 0x0019379C File Offset: 0x0019199C
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Error;
			if (brain.PathFinder == null)
			{
				return;
			}
			AIInformationZone informationZone = (entity as HumanNPC).GetInformationZone(entity.transform.position);
			if (informationZone == null)
			{
				return;
			}
			AICoverPoint bestCoverPoint = informationZone.GetBestCoverPoint(entity.transform.position, entity.transform.position, 25f, 50f, entity, true);
			if (bestCoverPoint)
			{
				bestCoverPoint.SetUsedBy(entity, 10f);
			}
			Vector3 pos = (bestCoverPoint == null) ? entity.transform.position : bestCoverPoint.transform.position;
			if (brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Fast, 0f, 0f))
			{
				this.status = StateStatus.Running;
			}
		}

		// Token: 0x06004BD3 RID: 19411 RVA: 0x00193861 File Offset: 0x00191A61
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			if (this.status == StateStatus.Error)
			{
				return this.status;
			}
			if (brain.Navigator.Moving)
			{
				return StateStatus.Running;
			}
			return StateStatus.Finished;
		}

		// Token: 0x04004061 RID: 16481
		private StateStatus status = StateStatus.Error;
	}

	// Token: 0x02000C04 RID: 3076
	public class IdleState : BaseAIBrain.BaseIdleState
	{
	}

	// Token: 0x02000C05 RID: 3077
	public class MountedState : BaseAIBrain.BaseMountedState
	{
	}

	// Token: 0x02000C06 RID: 3078
	public class MoveToVector3State : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004BD7 RID: 19415 RVA: 0x001938AC File Offset: 0x00191AAC
		public MoveToVector3State() : base(AIState.MoveToVector3)
		{
		}

		// Token: 0x06004BD8 RID: 19416 RVA: 0x001938B6 File Offset: 0x00191AB6
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004BD9 RID: 19417 RVA: 0x001931C9 File Offset: 0x001913C9
		private void Stop()
		{
			this.brain.Navigator.Stop();
			this.brain.Navigator.ClearFacingDirectionOverride();
		}

		// Token: 0x06004BDA RID: 19418 RVA: 0x001938C8 File Offset: 0x00191AC8
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			Vector3 pos = brain.Events.Memory.Position.Get(7);
			if (!brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Fast, 0.5f, 0f))
			{
				return StateStatus.Error;
			}
			if (!brain.Navigator.Moving)
			{
				return StateStatus.Finished;
			}
			return StateStatus.Running;
		}
	}

	// Token: 0x02000C07 RID: 3079
	public class RoamState : BaseAIBrain.BaseRoamState
	{
		// Token: 0x06004BDB RID: 19419 RVA: 0x00193921 File Offset: 0x00191B21
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
			this.ClearRoamPointUsage(entity);
		}

		// Token: 0x06004BDC RID: 19420 RVA: 0x00193938 File Offset: 0x00191B38
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Error;
			this.ClearRoamPointUsage(entity);
			if (brain.PathFinder == null)
			{
				return;
			}
			this.status = StateStatus.Error;
			this.roamPoint = brain.PathFinder.GetBestRoamPoint(this.GetRoamAnchorPosition(), entity.transform.position, (entity as HumanNPC).eyes.BodyForward(), brain.Navigator.MaxRoamDistanceFromHome, brain.Navigator.BestRoamPointMaxDistance);
			if (this.roamPoint != null)
			{
				if (brain.Navigator.SetDestination(this.roamPoint.transform.position, BaseNavigator.NavigationSpeed.Slow, 0f, 0f))
				{
					this.roamPoint.SetUsedBy(entity);
					this.status = StateStatus.Running;
					return;
				}
				this.roamPoint.SetUsedBy(entity, 600f);
			}
		}

		// Token: 0x06004BDD RID: 19421 RVA: 0x00193A0E File Offset: 0x00191C0E
		private void ClearRoamPointUsage(BaseEntity entity)
		{
			if (this.roamPoint != null)
			{
				this.roamPoint.ClearIfUsedBy(entity);
				this.roamPoint = null;
			}
		}

		// Token: 0x06004BDE RID: 19422 RVA: 0x0018ED31 File Offset: 0x0018CF31
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004BDF RID: 19423 RVA: 0x00193A31 File Offset: 0x00191C31
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			if (this.status == StateStatus.Error)
			{
				return this.status;
			}
			if (brain.Navigator.Moving)
			{
				return StateStatus.Running;
			}
			return StateStatus.Finished;
		}

		// Token: 0x04004062 RID: 16482
		private StateStatus status = StateStatus.Error;

		// Token: 0x04004063 RID: 16483
		private AIMovePoint roamPoint;
	}

	// Token: 0x02000C08 RID: 3080
	public class TakeCoverState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004BE1 RID: 19425 RVA: 0x00193A62 File Offset: 0x00191C62
		public TakeCoverState() : base(AIState.TakeCover)
		{
		}

		// Token: 0x06004BE2 RID: 19426 RVA: 0x00193A73 File Offset: 0x00191C73
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Running;
			if (!this.StartMovingToCover(entity as HumanNPC))
			{
				this.status = StateStatus.Error;
			}
		}

		// Token: 0x06004BE3 RID: 19427 RVA: 0x00193A99 File Offset: 0x00191C99
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.ClearFacingDirectionOverride();
			this.ClearCoverPointUsage(entity);
		}

		// Token: 0x06004BE4 RID: 19428 RVA: 0x00193AB8 File Offset: 0x00191CB8
		private void ClearCoverPointUsage(BaseEntity entity)
		{
			AIPoint aipoint = this.brain.Events.Memory.AIPoint.Get(4);
			if (aipoint != null)
			{
				aipoint.ClearIfUsedBy(entity);
			}
		}

		// Token: 0x06004BE5 RID: 19429 RVA: 0x00193AF4 File Offset: 0x00191CF4
		private bool StartMovingToCover(HumanNPC entity)
		{
			this.coverFromEntity = this.brain.Events.Memory.Entity.Get(this.brain.Events.CurrentInputMemorySlot);
			Vector3 hideFromPosition = this.coverFromEntity ? this.coverFromEntity.transform.position : (entity.transform.position + entity.LastAttackedDir * 30f);
			AIInformationZone informationZone = entity.GetInformationZone(entity.transform.position);
			if (informationZone == null)
			{
				return false;
			}
			float minRange = (entity.SecondsSinceAttacked < 2f) ? 2f : 0f;
			float bestCoverPointMaxDistance = this.brain.Navigator.BestCoverPointMaxDistance;
			AICoverPoint bestCoverPoint = informationZone.GetBestCoverPoint(entity.transform.position, hideFromPosition, minRange, bestCoverPointMaxDistance, entity, true);
			if (bestCoverPoint == null)
			{
				return false;
			}
			Vector3 position = bestCoverPoint.transform.position;
			if (!this.brain.Navigator.SetDestination(position, BaseNavigator.NavigationSpeed.Normal, 0f, 0f))
			{
				return false;
			}
			this.FaceCoverFromEntity();
			this.brain.Events.Memory.AIPoint.Set(bestCoverPoint, 4);
			bestCoverPoint.SetUsedBy(entity);
			return true;
		}

		// Token: 0x06004BE6 RID: 19430 RVA: 0x00193C38 File Offset: 0x00191E38
		public override void DrawGizmos()
		{
			base.DrawGizmos();
		}

		// Token: 0x06004BE7 RID: 19431 RVA: 0x00193C40 File Offset: 0x00191E40
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			this.FaceCoverFromEntity();
			if (this.status == StateStatus.Error)
			{
				return this.status;
			}
			if (brain.Navigator.Moving)
			{
				return StateStatus.Running;
			}
			return StateStatus.Finished;
		}

		// Token: 0x06004BE8 RID: 19432 RVA: 0x00193C74 File Offset: 0x00191E74
		private void FaceCoverFromEntity()
		{
			this.coverFromEntity = this.brain.Events.Memory.Entity.Get(this.brain.Events.CurrentInputMemorySlot);
			if (this.coverFromEntity == null)
			{
				return;
			}
			this.brain.Navigator.SetFacingDirectionEntity(this.coverFromEntity);
		}

		// Token: 0x04004064 RID: 16484
		private StateStatus status = StateStatus.Error;

		// Token: 0x04004065 RID: 16485
		private BaseEntity coverFromEntity;
	}
}
