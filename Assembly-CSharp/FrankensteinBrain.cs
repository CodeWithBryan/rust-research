using System;
using UnityEngine;

// Token: 0x020001E4 RID: 484
public class FrankensteinBrain : PetBrain
{
	// Token: 0x0600195E RID: 6494 RVA: 0x000B75F8 File Offset: 0x000B57F8
	public override void AddStates()
	{
		base.AddStates();
		base.AddState(new BaseAIBrain.BaseIdleState());
		base.AddState(new FrankensteinBrain.MoveTorwardsState());
		base.AddState(new BaseAIBrain.BaseChaseState());
		base.AddState(new BaseAIBrain.BaseAttackState());
		base.AddState(new FrankensteinBrain.MoveToPointState());
	}

	// Token: 0x0600195F RID: 6495 RVA: 0x000B7637 File Offset: 0x000B5837
	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		this.thinkRate = 0.25f;
		base.PathFinder = new HumanPathFinder();
		((HumanPathFinder)base.PathFinder).Init(this.GetBaseEntity());
	}

	// Token: 0x06001960 RID: 6496 RVA: 0x000B7672 File Offset: 0x000B5872
	public FrankensteinPet GetEntity()
	{
		return this.GetBaseEntity() as FrankensteinPet;
	}

	// Token: 0x06001961 RID: 6497 RVA: 0x000B767F File Offset: 0x000B587F
	public override void OnDestroy()
	{
		base.OnDestroy();
	}

	// Token: 0x040011FD RID: 4605
	[ServerVar]
	public static float MoveTowardsRate = 1f;

	// Token: 0x02000C0B RID: 3083
	public class MoveToPointState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004BEF RID: 19439 RVA: 0x00193DB7 File Offset: 0x00191FB7
		public MoveToPointState() : base(AIState.MoveToPoint)
		{
		}

		// Token: 0x06004BF0 RID: 19440 RVA: 0x00193DC4 File Offset: 0x00191FC4
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			BaseNavigator navigator = brain.Navigator;
			this.originalStopDistance = navigator.StoppingDistance;
			navigator.StoppingDistance = 0.5f;
		}

		// Token: 0x06004BF1 RID: 19441 RVA: 0x00193DF7 File Offset: 0x00191FF7
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.StoppingDistance = this.originalStopDistance;
			this.Stop();
		}

		// Token: 0x06004BF2 RID: 19442 RVA: 0x0018ED31 File Offset: 0x0018CF31
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004BF3 RID: 19443 RVA: 0x00193E18 File Offset: 0x00192018
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			Vector3 pos = brain.Events.Memory.Position.Get(6);
			if (!brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Normal, FrankensteinBrain.MoveTowardsRate, 0f))
			{
				return StateStatus.Error;
			}
			if (!brain.Navigator.Moving)
			{
				brain.LoadDefaultAIDesign();
			}
			if (!brain.Navigator.Moving)
			{
				return StateStatus.Finished;
			}
			return StateStatus.Running;
		}

		// Token: 0x0400406F RID: 16495
		private float originalStopDistance;
	}

	// Token: 0x02000C0C RID: 3084
	public class MoveTorwardsState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004BF4 RID: 19444 RVA: 0x0018F2E4 File Offset: 0x0018D4E4
		public MoveTorwardsState() : base(AIState.MoveTowards)
		{
		}

		// Token: 0x06004BF5 RID: 19445 RVA: 0x00193E85 File Offset: 0x00192085
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004BF6 RID: 19446 RVA: 0x0018ED31 File Offset: 0x0018CF31
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004BF7 RID: 19447 RVA: 0x00193E98 File Offset: 0x00192098
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (baseEntity == null)
			{
				this.Stop();
				return StateStatus.Error;
			}
			if (!brain.Navigator.SetDestination(baseEntity.transform.position, BaseNavigator.NavigationSpeed.Normal, FrankensteinBrain.MoveTowardsRate, 0f))
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
}
