using System;
using UnityEngine;

// Token: 0x020001CD RID: 461
public class AnimalBrain : BaseAIBrain
{
	// Token: 0x0600186D RID: 6253 RVA: 0x000B405C File Offset: 0x000B225C
	public override void AddStates()
	{
		base.AddStates();
		base.AddState(new AnimalBrain.IdleState());
		base.AddState(new AnimalBrain.MoveTowardsState());
		base.AddState(new AnimalBrain.FleeState());
		base.AddState(new AnimalBrain.RoamState());
		base.AddState(new AnimalBrain.AttackState());
		base.AddState(new BaseAIBrain.BaseSleepState());
		base.AddState(new AnimalBrain.ChaseState());
		base.AddState(new BaseAIBrain.BaseCooldownState());
	}

	// Token: 0x0600186E RID: 6254 RVA: 0x000B40C7 File Offset: 0x000B22C7
	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		this.thinkRate = 0.25f;
		base.PathFinder = new BasePathFinder();
		AnimalBrain.Count++;
	}

	// Token: 0x0600186F RID: 6255 RVA: 0x000B40F8 File Offset: 0x000B22F8
	public override void OnDestroy()
	{
		base.OnDestroy();
		AnimalBrain.Count--;
	}

	// Token: 0x06001870 RID: 6256 RVA: 0x000B410C File Offset: 0x000B230C
	public BaseAnimalNPC GetEntity()
	{
		return this.GetBaseEntity() as BaseAnimalNPC;
	}

	// Token: 0x04001186 RID: 4486
	public static int Count;

	// Token: 0x02000BF0 RID: 3056
	public class AttackState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004B73 RID: 19315 RVA: 0x0018EAAB File Offset: 0x0018CCAB
		public AttackState() : base(AIState.Attack)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004B74 RID: 19316 RVA: 0x00191ED0 File Offset: 0x001900D0
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.attack = (entity as IAIAttack);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			BasePlayer basePlayer = baseEntity as BasePlayer;
			if (basePlayer != null && basePlayer.IsDead())
			{
				this.StopAttacking();
				return;
			}
			if (baseEntity != null && baseEntity.Health() > 0f)
			{
				BaseCombatEntity target = baseEntity as BaseCombatEntity;
				Vector3 aimDirection = AnimalBrain.AttackState.GetAimDirection(entity as BaseCombatEntity, target);
				brain.Navigator.SetFacingDirectionOverride(aimDirection);
				if (this.attack.CanAttack(baseEntity))
				{
					this.StartAttacking(baseEntity);
				}
				brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			}
		}

		// Token: 0x06004B75 RID: 19317 RVA: 0x00191F9E File Offset: 0x0019019E
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.ClearFacingDirectionOverride();
			brain.Navigator.Stop();
			this.StopAttacking();
		}

		// Token: 0x06004B76 RID: 19318 RVA: 0x00191FC4 File Offset: 0x001901C4
		private void StopAttacking()
		{
			this.attack.StopAttacking();
		}

		// Token: 0x06004B77 RID: 19319 RVA: 0x00191FD4 File Offset: 0x001901D4
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
			if (baseEntity.Health() <= 0f)
			{
				this.StopAttacking();
				return StateStatus.Finished;
			}
			BasePlayer basePlayer = baseEntity as BasePlayer;
			if (basePlayer != null && basePlayer.IsDead())
			{
				this.StopAttacking();
				return StateStatus.Finished;
			}
			BaseVehicle baseVehicle = (basePlayer != null) ? basePlayer.GetMountedVehicle() : null;
			if (baseVehicle != null && baseVehicle is BaseModularVehicle)
			{
				this.StopAttacking();
				return StateStatus.Error;
			}
			if (brain.Senses.ignoreSafeZonePlayers && basePlayer != null && basePlayer.InSafeZone())
			{
				return StateStatus.Error;
			}
			if (!brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0.25f, (baseEntity is BasePlayer && this.attack != null) ? this.attack.EngagementRange() : 0f))
			{
				return StateStatus.Error;
			}
			BaseCombatEntity target = baseEntity as BaseCombatEntity;
			Vector3 aimDirection = AnimalBrain.AttackState.GetAimDirection(entity as BaseCombatEntity, target);
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

		// Token: 0x06004B78 RID: 19320 RVA: 0x00192138 File Offset: 0x00190338
		private static Vector3 GetAimDirection(BaseCombatEntity from, BaseCombatEntity target)
		{
			if (!(from == null) && !(target == null))
			{
				return Vector3Ex.Direction2D(target.transform.position, from.transform.position);
			}
			if (!(from != null))
			{
				return Vector3.forward;
			}
			return from.transform.forward;
		}

		// Token: 0x06004B79 RID: 19321 RVA: 0x0019218D File Offset: 0x0019038D
		private void StartAttacking(BaseEntity entity)
		{
			this.attack.StartAttacking(entity);
		}

		// Token: 0x04004040 RID: 16448
		private IAIAttack attack;
	}

	// Token: 0x02000BF1 RID: 3057
	public class ChaseState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004B7A RID: 19322 RVA: 0x0018ECAD File Offset: 0x0018CEAD
		public ChaseState() : base(AIState.Chase)
		{
			base.AgrresiveState = true;
		}

		// Token: 0x06004B7B RID: 19323 RVA: 0x0019219C File Offset: 0x0019039C
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.attack = (entity as IAIAttack);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity != null)
			{
				brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			}
		}

		// Token: 0x06004B7C RID: 19324 RVA: 0x00192209 File Offset: 0x00190409
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004B7D RID: 19325 RVA: 0x0018ED31 File Offset: 0x0018CF31
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004B7E RID: 19326 RVA: 0x0019221C File Offset: 0x0019041C
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				this.Stop();
				return StateStatus.Error;
			}
			if (!brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Fast, 0.25f, (baseEntity is BasePlayer && this.attack != null) ? this.attack.EngagementRange() : 0f))
			{
				return StateStatus.Error;
			}
			if (!brain.Navigator.Moving)
			{
				return StateStatus.Finished;
			}
			return StateStatus.Running;
		}

		// Token: 0x04004041 RID: 16449
		private IAIAttack attack;
	}

	// Token: 0x02000BF2 RID: 3058
	public class FleeState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004B7F RID: 19327 RVA: 0x001922B7 File Offset: 0x001904B7
		public FleeState() : base(AIState.Flee)
		{
		}

		// Token: 0x06004B80 RID: 19328 RVA: 0x001922CC File Offset: 0x001904CC
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity != null)
			{
				this.stopFleeDistance = UnityEngine.Random.Range(80f, 100f) + Mathf.Clamp(Vector3Ex.Distance2D(brain.Navigator.transform.position, baseEntity.transform.position), 0f, 50f);
			}
			this.FleeFrom(brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot), entity);
		}

		// Token: 0x06004B81 RID: 19329 RVA: 0x00192378 File Offset: 0x00190578
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004B82 RID: 19330 RVA: 0x0018ED31 File Offset: 0x0018CF31
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004B83 RID: 19331 RVA: 0x00192388 File Offset: 0x00190588
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				return StateStatus.Finished;
			}
			if (Vector3Ex.Distance2D(brain.Navigator.transform.position, baseEntity.transform.position) >= this.stopFleeDistance)
			{
				return StateStatus.Finished;
			}
			if ((brain.Navigator.UpdateIntervalElapsed(this.nextInterval) || !brain.Navigator.Moving) && !this.FleeFrom(baseEntity, entity))
			{
				return StateStatus.Error;
			}
			return StateStatus.Running;
		}

		// Token: 0x06004B84 RID: 19332 RVA: 0x00192428 File Offset: 0x00190628
		private bool FleeFrom(BaseEntity fleeFromEntity, BaseEntity thisEntity)
		{
			if (thisEntity == null || fleeFromEntity == null)
			{
				return false;
			}
			this.nextInterval = UnityEngine.Random.Range(3f, 6f);
			Vector3 pos;
			if (!this.brain.PathFinder.GetBestFleePosition(this.brain.Navigator, this.brain.Senses, fleeFromEntity, this.brain.Events.Memory.Position.Get(4), 50f, 100f, out pos))
			{
				return false;
			}
			bool flag = this.brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Fast, 0f, 0f);
			if (!flag)
			{
				this.Stop();
			}
			return flag;
		}

		// Token: 0x04004042 RID: 16450
		private float nextInterval = 2f;

		// Token: 0x04004043 RID: 16451
		private float stopFleeDistance;
	}

	// Token: 0x02000BF3 RID: 3059
	public class IdleState : BaseAIBrain.BaseIdleState
	{
		// Token: 0x06004B85 RID: 19333 RVA: 0x001924D5 File Offset: 0x001906D5
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.FaceNewDirection(entity);
		}

		// Token: 0x06004B86 RID: 19334 RVA: 0x001924E6 File Offset: 0x001906E6
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.ClearFacingDirectionOverride();
		}

		// Token: 0x06004B87 RID: 19335 RVA: 0x001924FC File Offset: 0x001906FC
		private void FaceNewDirection(BaseEntity entity)
		{
			if (UnityEngine.Random.Range(0, 100) <= this.turnChance)
			{
				Vector3 position = entity.transform.position;
				Vector3 normalized = (BasePathFinder.GetPointOnCircle(position, 1f, UnityEngine.Random.Range(0f, 594f)) - position).normalized;
				this.brain.Navigator.SetFacingDirectionOverride(normalized);
			}
			this.nextTurnTime = Time.realtimeSinceStartup + UnityEngine.Random.Range(this.minTurnTime, this.maxTurnTime);
		}

		// Token: 0x06004B88 RID: 19336 RVA: 0x0019257C File Offset: 0x0019077C
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			if (Time.realtimeSinceStartup >= this.nextTurnTime)
			{
				this.FaceNewDirection(entity);
			}
			return StateStatus.Running;
		}

		// Token: 0x04004044 RID: 16452
		private float nextTurnTime;

		// Token: 0x04004045 RID: 16453
		private float minTurnTime = 10f;

		// Token: 0x04004046 RID: 16454
		private float maxTurnTime = 20f;

		// Token: 0x04004047 RID: 16455
		private int turnChance = 33;
	}

	// Token: 0x02000BF4 RID: 3060
	public class MoveTowardsState : BaseAIBrain.BaseMoveTorwardsState
	{
	}

	// Token: 0x02000BF5 RID: 3061
	public class RoamState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004B8B RID: 19339 RVA: 0x001925CB File Offset: 0x001907CB
		public RoamState() : base(AIState.Roam)
		{
		}

		// Token: 0x06004B8C RID: 19340 RVA: 0x001925DB File Offset: 0x001907DB
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004B8D RID: 19341 RVA: 0x001925EC File Offset: 0x001907EC
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Error;
			if (brain.PathFinder == null)
			{
				return;
			}
			Vector3 vector;
			if (brain.InGroup() && !brain.IsGroupLeader)
			{
				vector = brain.Events.Memory.Position.Get(5);
				vector = BasePathFinder.GetPointOnCircle(vector, UnityEngine.Random.Range(2f, 7f), UnityEngine.Random.Range(0f, 359f));
			}
			else
			{
				vector = brain.PathFinder.GetBestRoamPosition(brain.Navigator, brain.Events.Memory.Position.Get(4), 20f, 100f);
			}
			if (brain.Navigator.SetDestination(vector, BaseNavigator.NavigationSpeed.Slow, 0f, 0f))
			{
				if (brain.InGroup() && brain.IsGroupLeader)
				{
					brain.SetGroupRoamRootPosition(vector);
				}
				this.status = StateStatus.Running;
				return;
			}
			this.status = StateStatus.Error;
		}

		// Token: 0x06004B8E RID: 19342 RVA: 0x0018ED31 File Offset: 0x0018CF31
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004B8F RID: 19343 RVA: 0x001926D6 File Offset: 0x001908D6
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

		// Token: 0x04004048 RID: 16456
		private StateStatus status = StateStatus.Error;
	}
}
