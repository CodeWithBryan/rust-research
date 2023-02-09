using System;
using ConVar;
using UnityEngine;

// Token: 0x020001E6 RID: 486
public class ScarecrowBrain : BaseAIBrain
{
	// Token: 0x06001967 RID: 6503 RVA: 0x000B769B File Offset: 0x000B589B
	public override void AddStates()
	{
		base.AddStates();
		base.AddState(new BaseAIBrain.BaseIdleState());
		base.AddState(new ScarecrowBrain.ChaseState());
		base.AddState(new ScarecrowBrain.AttackState());
		base.AddState(new ScarecrowBrain.RoamState());
		base.AddState(new BaseAIBrain.BaseFleeState());
	}

	// Token: 0x06001968 RID: 6504 RVA: 0x000B53CC File Offset: 0x000B35CC
	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		this.thinkRate = 0.25f;
		base.PathFinder = new HumanPathFinder();
		((HumanPathFinder)base.PathFinder).Init(this.GetBaseEntity());
	}

	// Token: 0x06001969 RID: 6505 RVA: 0x000B5407 File Offset: 0x000B3607
	public override void OnDestroy()
	{
		base.OnDestroy();
	}

	// Token: 0x02000C0D RID: 3085
	public class AttackState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004BF8 RID: 19448 RVA: 0x0018EAAB File Offset: 0x0018CCAB
		public AttackState() : base(AIState.Attack)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004BF9 RID: 19449 RVA: 0x00193F18 File Offset: 0x00192118
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			entity.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
			this.originalStoppingDistance = brain.Navigator.StoppingDistance;
			brain.Navigator.Agent.stoppingDistance = 1f;
			brain.Navigator.StoppingDistance = 1f;
			base.StateEnter(brain, entity);
			this.attack = (entity as IAIAttack);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity != null)
			{
				Vector3 aimDirection = ScarecrowBrain.AttackState.GetAimDirection(brain.Navigator.transform.position, baseEntity.transform.position);
				brain.Navigator.SetFacingDirectionOverride(aimDirection);
				if (this.attack.CanAttack(baseEntity))
				{
					this.StartAttacking(baseEntity);
				}
				brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			}
		}

		// Token: 0x06004BFA RID: 19450 RVA: 0x0019400C File Offset: 0x0019220C
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			entity.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
			brain.Navigator.Agent.stoppingDistance = this.originalStoppingDistance;
			brain.Navigator.StoppingDistance = this.originalStoppingDistance;
			brain.Navigator.ClearFacingDirectionOverride();
			brain.Navigator.Stop();
			this.StopAttacking();
		}

		// Token: 0x06004BFB RID: 19451 RVA: 0x00194072 File Offset: 0x00192272
		private void StopAttacking()
		{
			this.attack.StopAttacking();
		}

		// Token: 0x06004BFC RID: 19452 RVA: 0x00194080 File Offset: 0x00192280
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (this.attack == null)
			{
				return StateStatus.Error;
			}
			if (baseEntity == null)
			{
				brain.Navigator.ClearFacingDirectionOverride();
				this.StopAttacking();
				return StateStatus.Finished;
			}
			if (brain.Senses.ignoreSafeZonePlayers)
			{
				BasePlayer basePlayer = baseEntity as BasePlayer;
				if (basePlayer != null && basePlayer.InSafeZone())
				{
					return StateStatus.Error;
				}
			}
			Vector3Ex.Direction2D(baseEntity.transform.position, entity.transform.position);
			Vector3 position = baseEntity.transform.position;
			if (!brain.Navigator.SetDestination(position, BaseNavigator.NavigationSpeed.Fast, 0.2f, 0f))
			{
				return StateStatus.Error;
			}
			Vector3 aimDirection = ScarecrowBrain.AttackState.GetAimDirection(brain.Navigator.transform.position, baseEntity.transform.position);
			brain.Navigator.SetFacingDirectionOverride(aimDirection);
			if (this.attack.CanAttack(baseEntity))
			{
				this.StartAttacking(baseEntity);
			}
			else
			{
				this.StopAttacking();
			}
			return StateStatus.Running;
		}

		// Token: 0x06004BFD RID: 19453 RVA: 0x0018EC95 File Offset: 0x0018CE95
		private static Vector3 GetAimDirection(Vector3 from, Vector3 target)
		{
			return Vector3Ex.Direction2D(target, from);
		}

		// Token: 0x06004BFE RID: 19454 RVA: 0x00194193 File Offset: 0x00192393
		private void StartAttacking(BaseEntity entity)
		{
			this.attack.StartAttacking(entity);
		}

		// Token: 0x04004070 RID: 16496
		private IAIAttack attack;

		// Token: 0x04004071 RID: 16497
		private float originalStoppingDistance;
	}

	// Token: 0x02000C0E RID: 3086
	public class ChaseState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004BFF RID: 19455 RVA: 0x0018ECAD File Offset: 0x0018CEAD
		public ChaseState() : base(AIState.Chase)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004C00 RID: 19456 RVA: 0x001941A4 File Offset: 0x001923A4
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			entity.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
			this.throwDelayTime = UnityEngine.Time.time + UnityEngine.Random.Range(0.2f, 0.5f);
			this.useBeanCan = ((float)UnityEngine.Random.Range(0, 100) <= 20f);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity != null)
			{
				brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			}
		}

		// Token: 0x06004C01 RID: 19457 RVA: 0x00194247 File Offset: 0x00192447
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			entity.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
			this.Stop();
		}

		// Token: 0x06004C02 RID: 19458 RVA: 0x0018ED31 File Offset: 0x0018CF31
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004C03 RID: 19459 RVA: 0x00194268 File Offset: 0x00192468
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				this.Stop();
				return StateStatus.Error;
			}
			if (this.useBeanCan && UnityEngine.Time.time >= this.throwDelayTime && AI.npc_use_thrown_weapons && Halloween.scarecrows_throw_beancans && UnityEngine.Time.time >= ScarecrowNPC.NextBeanCanAllowedTime && (brain.GetBrainBaseEntity() as ScarecrowNPC).TryUseThrownWeapon(baseEntity, 10f))
			{
				brain.Navigator.Stop();
				return StateStatus.Running;
			}
			if ((brain.GetBrainBaseEntity() as BasePlayer).modelState.aiming)
			{
				return StateStatus.Running;
			}
			if (!brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0.25f, 0f))
			{
				return StateStatus.Error;
			}
			if (!brain.Navigator.Moving)
			{
				return StateStatus.Finished;
			}
			return StateStatus.Running;
		}

		// Token: 0x04004072 RID: 16498
		private float throwDelayTime;

		// Token: 0x04004073 RID: 16499
		private bool useBeanCan;
	}

	// Token: 0x02000C0F RID: 3087
	public class RoamState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004C04 RID: 19460 RVA: 0x00194353 File Offset: 0x00192553
		public RoamState() : base(AIState.Roam)
		{
		}

		// Token: 0x06004C05 RID: 19461 RVA: 0x00194363 File Offset: 0x00192563
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004C06 RID: 19462 RVA: 0x00194374 File Offset: 0x00192574
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Error;
			if (brain.PathFinder == null)
			{
				return;
			}
			ScarecrowNPC scarecrowNPC = entity as ScarecrowNPC;
			if (scarecrowNPC == null)
			{
				return;
			}
			Vector3 vector = brain.Events.Memory.Position.Get(4);
			Vector3 pos;
			if (scarecrowNPC.RoamAroundHomePoint)
			{
				pos = brain.PathFinder.GetBestRoamPositionFromAnchor(brain.Navigator, vector, vector, 1f, brain.Navigator.BestRoamPointMaxDistance);
			}
			else
			{
				pos = brain.PathFinder.GetBestRoamPosition(brain.Navigator, brain.Events.Memory.Position.Get(4), 10f, brain.Navigator.BestRoamPointMaxDistance);
			}
			if (brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Slow, 0f, 0f))
			{
				this.status = StateStatus.Running;
				return;
			}
			this.status = StateStatus.Error;
		}

		// Token: 0x06004C07 RID: 19463 RVA: 0x0018ED31 File Offset: 0x0018CF31
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004C08 RID: 19464 RVA: 0x00194451 File Offset: 0x00192651
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

		// Token: 0x04004074 RID: 16500
		private StateStatus status = StateStatus.Error;
	}
}
