using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001D2 RID: 466
public class FishBrain : BaseAIBrain
{
	// Token: 0x0600188A RID: 6282 RVA: 0x000B4260 File Offset: 0x000B2460
	public override void AddStates()
	{
		base.AddStates();
		base.AddState(new FishBrain.IdleState());
		base.AddState(new FishBrain.RoamState());
		base.AddState(new BaseAIBrain.BaseFleeState());
		base.AddState(new BaseAIBrain.BaseChaseState());
		base.AddState(new BaseAIBrain.BaseMoveTorwardsState());
		base.AddState(new BaseAIBrain.BaseAttackState());
		base.AddState(new BaseAIBrain.BaseCooldownState());
	}

	// Token: 0x0600188B RID: 6283 RVA: 0x000B42C0 File Offset: 0x000B24C0
	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		this.thinkRate = 0.25f;
		base.PathFinder = new UnderwaterPathFinder();
		((UnderwaterPathFinder)base.PathFinder).Init(this.GetBaseEntity());
		FishBrain.Count++;
	}

	// Token: 0x0600188C RID: 6284 RVA: 0x000B4312 File Offset: 0x000B2512
	public override void OnDestroy()
	{
		base.OnDestroy();
		FishBrain.Count--;
	}

	// Token: 0x0400118B RID: 4491
	public static int Count;

	// Token: 0x02000BF6 RID: 3062
	public class IdleState : BaseAIBrain.BaseIdleState
	{
		// Token: 0x06004B90 RID: 19344 RVA: 0x00192702 File Offset: 0x00190902
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004B91 RID: 19345 RVA: 0x00192714 File Offset: 0x00190914
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.idleRootPos = brain.Navigator.transform.position;
			this.GenerateIdlePoints(20f, 0f);
			this.currentPointIndex = 0;
			this.status = StateStatus.Error;
			if (brain.PathFinder == null)
			{
				return;
			}
			if (brain.Navigator.SetDestination(this.idleRootPos + this.idlePoints[0], BaseNavigator.NavigationSpeed.Normal, 0f, 0f))
			{
				this.status = StateStatus.Running;
				return;
			}
			this.status = StateStatus.Error;
		}

		// Token: 0x06004B92 RID: 19346 RVA: 0x0018ED31 File Offset: 0x0018CF31
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004B93 RID: 19347 RVA: 0x001927A4 File Offset: 0x001909A4
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			if (Vector3.Distance(brain.Navigator.transform.position, this.idleRootPos + this.idlePoints[this.currentPointIndex]) < 4f)
			{
				this.currentPointIndex++;
			}
			if (this.currentPointIndex >= this.idlePoints.Count)
			{
				this.currentPointIndex = 0;
			}
			if (brain.Navigator.SetDestination(this.idleRootPos + this.idlePoints[this.currentPointIndex], BaseNavigator.NavigationSpeed.Normal, 0f, 0f))
			{
				this.status = StateStatus.Running;
			}
			else
			{
				this.status = StateStatus.Error;
			}
			return this.status;
		}

		// Token: 0x06004B94 RID: 19348 RVA: 0x00192868 File Offset: 0x00190A68
		private void GenerateIdlePoints(float radius, float heightOffset)
		{
			if (this.idlePoints != null)
			{
				return;
			}
			this.idlePoints = new List<Vector3>();
			float num = 0f;
			int num2 = 32;
			float height = TerrainMeta.WaterMap.GetHeight(this.brain.Navigator.transform.position);
			float height2 = TerrainMeta.HeightMap.GetHeight(this.brain.Navigator.transform.position);
			for (int i = 0; i < num2; i++)
			{
				num += 360f / (float)num2;
				Vector3 pointOnCircle = BasePathFinder.GetPointOnCircle(Vector3.zero, radius, num);
				pointOnCircle.y += UnityEngine.Random.Range(-heightOffset, heightOffset);
				pointOnCircle.y = Mathf.Clamp(pointOnCircle.y, height2, height);
				this.idlePoints.Add(pointOnCircle);
			}
		}

		// Token: 0x04004049 RID: 16457
		private StateStatus status = StateStatus.Error;

		// Token: 0x0400404A RID: 16458
		private List<Vector3> idlePoints;

		// Token: 0x0400404B RID: 16459
		private int currentPointIndex;

		// Token: 0x0400404C RID: 16460
		private Vector3 idleRootPos;
	}

	// Token: 0x02000BF7 RID: 3063
	public class RoamState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004B96 RID: 19350 RVA: 0x0019293F File Offset: 0x00190B3F
		public RoamState() : base(AIState.Roam)
		{
		}

		// Token: 0x06004B97 RID: 19351 RVA: 0x0019294F File Offset: 0x00190B4F
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			this.Stop();
		}

		// Token: 0x06004B98 RID: 19352 RVA: 0x00192960 File Offset: 0x00190B60
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			this.status = StateStatus.Error;
			if (brain.PathFinder == null)
			{
				return;
			}
			Vector3 fallbackPos = brain.Events.Memory.Position.Get(4);
			Vector3 bestRoamPosition = brain.PathFinder.GetBestRoamPosition(brain.Navigator, fallbackPos, 5f, brain.Navigator.MaxRoamDistanceFromHome);
			if (brain.Navigator.SetDestination(bestRoamPosition, BaseNavigator.NavigationSpeed.Normal, 0f, 0f))
			{
				this.status = StateStatus.Running;
				return;
			}
			this.status = StateStatus.Error;
		}

		// Token: 0x06004B99 RID: 19353 RVA: 0x0018ED31 File Offset: 0x0018CF31
		private void Stop()
		{
			this.brain.Navigator.Stop();
		}

		// Token: 0x06004B9A RID: 19354 RVA: 0x001929E7 File Offset: 0x00190BE7
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

		// Token: 0x0400404D RID: 16461
		private StateStatus status = StateStatus.Error;
	}
}
