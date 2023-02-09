using System;
using UnityEngine;

// Token: 0x020001D8 RID: 472
public class GingerbreadBrain : BaseAIBrain
{
	// Token: 0x060018C7 RID: 6343 RVA: 0x000B538D File Offset: 0x000B358D
	public override void AddStates()
	{
		base.AddStates();
		base.AddState(new BaseAIBrain.BaseIdleState());
		base.AddState(new BaseAIBrain.BaseChaseState());
		base.AddState(new GingerbreadBrain.AttackState());
		base.AddState(new GingerbreadBrain.RoamState());
		base.AddState(new BaseAIBrain.BaseFleeState());
	}

	// Token: 0x060018C8 RID: 6344 RVA: 0x000B53CC File Offset: 0x000B35CC
	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		this.thinkRate = 0.25f;
		base.PathFinder = new HumanPathFinder();
		((HumanPathFinder)base.PathFinder).Init(this.GetBaseEntity());
	}

	// Token: 0x060018C9 RID: 6345 RVA: 0x000B5407 File Offset: 0x000B3607
	public override void OnDestroy()
	{
		base.OnDestroy();
	}

	// Token: 0x02000BFB RID: 3067
	public class AttackState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004BAF RID: 19375 RVA: 0x0018EAAB File Offset: 0x0018CCAB
		public AttackState() : base(AIState.Attack)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004BB0 RID: 19376 RVA: 0x00192CDC File Offset: 0x00190EDC
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
				Vector3 aimDirection = GingerbreadBrain.AttackState.GetAimDirection(brain.Navigator.transform.position, baseEntity.transform.position);
				brain.Navigator.SetFacingDirectionOverride(aimDirection);
				if (this.attack.CanAttack(baseEntity))
				{
					this.StartAttacking(baseEntity);
				}
				brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			}
		}

		// Token: 0x06004BB1 RID: 19377 RVA: 0x00192DD0 File Offset: 0x00190FD0
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

		// Token: 0x06004BB2 RID: 19378 RVA: 0x00192E36 File Offset: 0x00191036
		private void StopAttacking()
		{
			this.attack.StopAttacking();
		}

		// Token: 0x06004BB3 RID: 19379 RVA: 0x00192E44 File Offset: 0x00191044
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
			Vector3 aimDirection = GingerbreadBrain.AttackState.GetAimDirection(brain.Navigator.transform.position, baseEntity.transform.position);
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

		// Token: 0x06004BB4 RID: 19380 RVA: 0x0018EC95 File Offset: 0x0018CE95
		private static Vector3 GetAimDirection(Vector3 from, Vector3 target)
		{
			return Vector3Ex.Direction2D(target, from);
		}

		// Token: 0x06004BB5 RID: 19381 RVA: 0x00192F57 File Offset: 0x00191157
		private void StartAttacking(BaseEntity entity)
		{
			this.attack.StartAttacking(entity);
		}

		// Token: 0x04004051 RID: 16465
		private IAIAttack attack;

		// Token: 0x04004052 RID: 16466
		private float originalStoppingDistance;
	}

	// Token: 0x02000BFC RID: 3068
	public class RoamState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004BB6 RID: 19382 RVA: 0x00192F66 File Offset: 0x00191166
		public RoamState() : base(AIState.Roam)
		{
		}

		// Token: 0x06004BB7 RID: 19383 RVA: 0x00192F76 File Offset: 0x00191176
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004BB8 RID: 19384 RVA: 0x00192F88 File Offset: 0x00191188
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

		// Token: 0x06004BB9 RID: 19385 RVA: 0x0018ED31 File Offset: 0x0018CF31
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004BBA RID: 19386 RVA: 0x00193065 File Offset: 0x00191265
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

		// Token: 0x04004053 RID: 16467
		private StateStatus status = StateStatus.Error;
	}
}
