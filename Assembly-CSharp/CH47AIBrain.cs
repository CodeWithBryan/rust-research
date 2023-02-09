using System;
using UnityEngine;

// Token: 0x02000457 RID: 1111
public class CH47AIBrain : BaseAIBrain
{
	// Token: 0x0600247B RID: 9339 RVA: 0x000E6A28 File Offset: 0x000E4C28
	public override void AddStates()
	{
		base.AddStates();
		base.AddState(new CH47AIBrain.IdleState());
		base.AddState(new CH47AIBrain.PatrolState());
		base.AddState(new CH47AIBrain.OrbitState());
		base.AddState(new CH47AIBrain.EgressState());
		base.AddState(new CH47AIBrain.DropCrate());
		base.AddState(new CH47AIBrain.LandState());
	}

	// Token: 0x0600247C RID: 9340 RVA: 0x000E6A7D File Offset: 0x000E4C7D
	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.FixedUpdate;
		base.PathFinder = new CH47PathFinder();
	}

	// Token: 0x0600247D RID: 9341 RVA: 0x000E6A97 File Offset: 0x000E4C97
	public void FixedUpdate()
	{
		if (base.baseEntity == null || base.baseEntity.isClient)
		{
			return;
		}
		this.Think(Time.fixedDeltaTime);
	}

	// Token: 0x02000CA8 RID: 3240
	public class DropCrate : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004D2F RID: 19759 RVA: 0x00197175 File Offset: 0x00195375
		public DropCrate() : base(AIState.DropCrate)
		{
		}

		// Token: 0x06004D30 RID: 19760 RVA: 0x0019717F File Offset: 0x0019537F
		public override bool CanInterrupt()
		{
			return base.CanInterrupt() && !this.CanDrop();
		}

		// Token: 0x06004D31 RID: 19761 RVA: 0x00197194 File Offset: 0x00195394
		public bool CanDrop()
		{
			return Time.time > this.nextDropTime && (this.brain.GetBrainBaseEntity() as CH47HelicopterAIController).CanDropCrate();
		}

		// Token: 0x06004D32 RID: 19762 RVA: 0x001971BC File Offset: 0x001953BC
		public override float GetWeight()
		{
			if (!this.CanDrop())
			{
				return 0f;
			}
			if (base.IsInState())
			{
				return 10000f;
			}
			if (this.brain.CurrentState != null && this.brain.CurrentState.StateType == AIState.Orbit && this.brain.CurrentState.TimeInState > 60f)
			{
				CH47DropZone closest = CH47DropZone.GetClosest(this.brain.mainInterestPoint);
				if (closest && Vector3Ex.Distance2D(closest.transform.position, this.brain.mainInterestPoint) < 200f)
				{
					CH47AIBrain component = this.brain.GetComponent<CH47AIBrain>();
					if (component != null)
					{
						float num = Mathf.InverseLerp(300f, 600f, component.Age);
						return 1000f * num;
					}
				}
			}
			return 0f;
		}

		// Token: 0x06004D33 RID: 19763 RVA: 0x00197298 File Offset: 0x00195498
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			ch47HelicopterAIController.SetDropDoorOpen(true);
			ch47HelicopterAIController.EnableFacingOverride(false);
			CH47DropZone closest = CH47DropZone.GetClosest(ch47HelicopterAIController.transform.position);
			if (closest == null)
			{
				this.nextDropTime = Time.time + 60f;
			}
			brain.mainInterestPoint = closest.transform.position;
			ch47HelicopterAIController.SetMoveTarget(brain.mainInterestPoint);
			base.StateEnter(brain, entity);
		}

		// Token: 0x06004D34 RID: 19764 RVA: 0x00197308 File Offset: 0x00195508
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			if (this.CanDrop() && Vector3Ex.Distance2D(brain.mainInterestPoint, ch47HelicopterAIController.transform.position) < 5f && ch47HelicopterAIController.rigidBody.velocity.magnitude < 5f)
			{
				ch47HelicopterAIController.DropCrate();
				this.nextDropTime = Time.time + 120f;
			}
			return StateStatus.Running;
		}

		// Token: 0x06004D35 RID: 19765 RVA: 0x0019737D File Offset: 0x0019557D
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			(entity as CH47HelicopterAIController).SetDropDoorOpen(false);
			this.nextDropTime = Time.time + 60f;
			base.StateLeave(brain, entity);
		}

		// Token: 0x04004368 RID: 17256
		private float nextDropTime;
	}

	// Token: 0x02000CA9 RID: 3241
	public class EgressState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004D36 RID: 19766 RVA: 0x001973A4 File Offset: 0x001955A4
		public EgressState() : base(AIState.Egress)
		{
		}

		// Token: 0x06004D37 RID: 19767 RVA: 0x00007074 File Offset: 0x00005274
		public override bool CanInterrupt()
		{
			return false;
		}

		// Token: 0x06004D38 RID: 19768 RVA: 0x001973B0 File Offset: 0x001955B0
		public override float GetWeight()
		{
			CH47HelicopterAIController ch47HelicopterAIController = this.brain.GetBrainBaseEntity() as CH47HelicopterAIController;
			if (ch47HelicopterAIController.OutOfCrates() && !ch47HelicopterAIController.ShouldLand())
			{
				return 10000f;
			}
			CH47AIBrain component = this.brain.GetComponent<CH47AIBrain>();
			if (!(component != null))
			{
				return 0f;
			}
			if (component.Age <= 1800f)
			{
				return 0f;
			}
			return 10000f;
		}

		// Token: 0x06004D39 RID: 19769 RVA: 0x00197418 File Offset: 0x00195618
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			ch47HelicopterAIController.EnableFacingOverride(false);
			Transform transform = ch47HelicopterAIController.transform;
			Rigidbody rigidBody = ch47HelicopterAIController.rigidBody;
			Vector3 rhs = (rigidBody.velocity.magnitude < 0.1f) ? transform.forward : rigidBody.velocity.normalized;
			Vector3 a = Vector3.Cross(Vector3.Cross(transform.up, rhs), Vector3.up);
			brain.mainInterestPoint = transform.position + a * 8000f;
			brain.mainInterestPoint.y = 100f;
			ch47HelicopterAIController.SetMoveTarget(brain.mainInterestPoint);
			base.StateEnter(brain, entity);
		}

		// Token: 0x06004D3A RID: 19770 RVA: 0x001974C4 File Offset: 0x001956C4
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			if (this.killing)
			{
				return StateStatus.Running;
			}
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			Vector3 position = ch47HelicopterAIController.transform.position;
			if (position.y < 85f && !this.egressAltitueAchieved)
			{
				CH47LandingZone closest = CH47LandingZone.GetClosest(position);
				if (closest != null && Vector3Ex.Distance2D(closest.transform.position, position) < 20f)
				{
					float num = 0f;
					if (TerrainMeta.HeightMap != null && TerrainMeta.WaterMap != null)
					{
						num = Mathf.Max(TerrainMeta.WaterMap.GetHeight(position), TerrainMeta.HeightMap.GetHeight(position));
					}
					num += 100f;
					Vector3 moveTarget = position;
					moveTarget.y = num;
					ch47HelicopterAIController.SetMoveTarget(moveTarget);
					return StateStatus.Running;
				}
			}
			this.egressAltitueAchieved = true;
			ch47HelicopterAIController.SetMoveTarget(brain.mainInterestPoint);
			if (base.TimeInState > 300f)
			{
				ch47HelicopterAIController.Invoke("DelayedKill", 2f);
				this.killing = true;
			}
			return StateStatus.Running;
		}

		// Token: 0x04004369 RID: 17257
		private bool killing;

		// Token: 0x0400436A RID: 17258
		private bool egressAltitueAchieved;
	}

	// Token: 0x02000CAA RID: 3242
	public class IdleState : BaseAIBrain.BaseIdleState
	{
		// Token: 0x06004D3B RID: 19771 RVA: 0x000292BC File Offset: 0x000274BC
		public override float GetWeight()
		{
			return 0.1f;
		}

		// Token: 0x06004D3C RID: 19772 RVA: 0x001975CC File Offset: 0x001957CC
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			ch47HelicopterAIController.SetMoveTarget(ch47HelicopterAIController.GetPosition() + ch47HelicopterAIController.rigidBody.velocity.normalized * 10f);
			base.StateEnter(brain, entity);
		}
	}

	// Token: 0x02000CAB RID: 3243
	public class LandState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004D3E RID: 19774 RVA: 0x00197616 File Offset: 0x00195816
		public LandState() : base(AIState.Land)
		{
		}

		// Token: 0x06004D3F RID: 19775 RVA: 0x0019762C File Offset: 0x0019582C
		public override float GetWeight()
		{
			if (!(this.brain.GetBrainBaseEntity() as CH47HelicopterAIController).ShouldLand())
			{
				return 0f;
			}
			float num = Time.time - this.lastLandtime;
			if (base.IsInState() && this.landedForSeconds < 12f)
			{
				return 1000f;
			}
			if (!base.IsInState() && num > 10f)
			{
				return 9000f;
			}
			return 0f;
		}

		// Token: 0x06004D40 RID: 19776 RVA: 0x0019769C File Offset: 0x0019589C
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			Vector3 position = ch47HelicopterAIController.transform.position;
			Vector3 forward = ch47HelicopterAIController.transform.forward;
			CH47LandingZone closest = CH47LandingZone.GetClosest(ch47HelicopterAIController.landingTarget);
			if (!closest)
			{
				return StateStatus.Error;
			}
			float magnitude = ch47HelicopterAIController.rigidBody.velocity.magnitude;
			float num = Vector3Ex.Distance2D(closest.transform.position, position);
			bool enabled = num < 40f;
			bool altitudeProtection = num > 15f && position.y < closest.transform.position.y + 10f;
			ch47HelicopterAIController.EnableFacingOverride(enabled);
			ch47HelicopterAIController.SetAltitudeProtection(altitudeProtection);
			bool flag = Mathf.Abs(closest.transform.position.y - position.y) < 3f && num <= 5f && magnitude < 1f;
			if (flag)
			{
				this.landedForSeconds += delta;
				if (this.lastLandtime == 0f)
				{
					this.lastLandtime = Time.time;
				}
			}
			float num2 = 1f - Mathf.InverseLerp(0f, 7f, num);
			this.landingHeight -= 4f * num2 * Time.deltaTime;
			if (this.landingHeight < -5f)
			{
				this.landingHeight = -5f;
			}
			ch47HelicopterAIController.SetAimDirection(closest.transform.forward);
			Vector3 moveTarget = brain.mainInterestPoint + new Vector3(0f, this.landingHeight, 0f);
			if (num < 100f && num > 15f)
			{
				Vector3 vector = Vector3Ex.Direction2D(closest.transform.position, position);
				RaycastHit raycastHit;
				if (Physics.SphereCast(position, 15f, vector, out raycastHit, num, 1218511105))
				{
					Vector3 a = Vector3.Cross(vector, Vector3.up);
					moveTarget = raycastHit.point + a * 50f;
				}
			}
			ch47HelicopterAIController.SetMoveTarget(moveTarget);
			if (flag)
			{
				if (this.landedForSeconds > 1f && Time.time > this.nextDismountTime)
				{
					foreach (BaseVehicle.MountPointInfo mountPointInfo in ch47HelicopterAIController.mountPoints)
					{
						if (mountPointInfo.mountable && mountPointInfo.mountable.AnyMounted())
						{
							this.nextDismountTime = Time.time + 0.5f;
							mountPointInfo.mountable.DismountAllPlayers();
							break;
						}
					}
				}
				if (this.landedForSeconds > 8f)
				{
					brain.GetComponent<CH47AIBrain>().ForceSetAge(float.PositiveInfinity);
				}
			}
			return StateStatus.Running;
		}

		// Token: 0x06004D41 RID: 19777 RVA: 0x0019795C File Offset: 0x00195B5C
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			brain.mainInterestPoint = (entity as CH47HelicopterAIController).landingTarget;
			this.landingHeight = 15f;
			base.StateEnter(brain, entity);
		}

		// Token: 0x06004D42 RID: 19778 RVA: 0x00197982 File Offset: 0x00195B82
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			ch47HelicopterAIController.EnableFacingOverride(false);
			ch47HelicopterAIController.SetAltitudeProtection(true);
			ch47HelicopterAIController.SetMinHoverHeight(30f);
			this.landedForSeconds = 0f;
			base.StateLeave(brain, entity);
		}

		// Token: 0x06004D43 RID: 19779 RVA: 0x00003A54 File Offset: 0x00001C54
		public override bool CanInterrupt()
		{
			return true;
		}

		// Token: 0x0400436B RID: 17259
		private float landedForSeconds;

		// Token: 0x0400436C RID: 17260
		private float lastLandtime;

		// Token: 0x0400436D RID: 17261
		private float landingHeight = 20f;

		// Token: 0x0400436E RID: 17262
		private float nextDismountTime;
	}

	// Token: 0x02000CAC RID: 3244
	public class OrbitState : BaseAIBrain.BasicAIState
	{
		// Token: 0x06004D44 RID: 19780 RVA: 0x001979B5 File Offset: 0x00195BB5
		public OrbitState() : base(AIState.Orbit)
		{
		}

		// Token: 0x06004D45 RID: 19781 RVA: 0x001979BF File Offset: 0x00195BBF
		public Vector3 GetOrbitCenter()
		{
			return this.brain.mainInterestPoint;
		}

		// Token: 0x06004D46 RID: 19782 RVA: 0x001979CC File Offset: 0x00195BCC
		public override float GetWeight()
		{
			if (base.IsInState())
			{
				float num = 1f - Mathf.InverseLerp(120f, 180f, base.TimeInState);
				return 5f * num;
			}
			if (this.brain.CurrentState != null && this.brain.CurrentState.StateType == AIState.Patrol)
			{
				CH47AIBrain.PatrolState patrolState = this.brain.CurrentState as CH47AIBrain.PatrolState;
				if (patrolState != null && patrolState.AtPatrolDestination())
				{
					return 5f;
				}
			}
			return 0f;
		}

		// Token: 0x06004D47 RID: 19783 RVA: 0x00197A4C File Offset: 0x00195C4C
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			ch47HelicopterAIController.EnableFacingOverride(true);
			ch47HelicopterAIController.InitiateAnger();
			base.StateEnter(brain, entity);
		}

		// Token: 0x06004D48 RID: 19784 RVA: 0x00197A68 File Offset: 0x00195C68
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			Vector3 orbitCenter = this.GetOrbitCenter();
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			Vector3 position = ch47HelicopterAIController.GetPosition();
			Vector3 vector = Vector3Ex.Direction2D(orbitCenter, position);
			Vector3 vector2 = Vector3.Cross(Vector3.up, vector);
			float d = (Vector3.Dot(Vector3.Cross(ch47HelicopterAIController.transform.right, Vector3.up), vector2) < 0f) ? -1f : 1f;
			float d2 = 75f;
			Vector3 normalized = (-vector + vector2 * d * 0.6f).normalized;
			Vector3 vector3 = orbitCenter + normalized * d2;
			ch47HelicopterAIController.SetMoveTarget(vector3);
			ch47HelicopterAIController.SetAimDirection(Vector3Ex.Direction2D(vector3, position));
			base.StateThink(delta, brain, entity);
			return StateStatus.Running;
		}

		// Token: 0x06004D49 RID: 19785 RVA: 0x00197B2D File Offset: 0x00195D2D
		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			CH47HelicopterAIController ch47HelicopterAIController = entity as CH47HelicopterAIController;
			ch47HelicopterAIController.EnableFacingOverride(false);
			ch47HelicopterAIController.CancelAnger();
			base.StateLeave(brain, entity);
		}
	}

	// Token: 0x02000CAD RID: 3245
	public class PatrolState : BaseAIBrain.BasePatrolState
	{
		// Token: 0x06004D4A RID: 19786 RVA: 0x00197B49 File Offset: 0x00195D49
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			brain.mainInterestPoint = brain.PathFinder.GetRandomPatrolPoint();
		}

		// Token: 0x06004D4B RID: 19787 RVA: 0x00197B64 File Offset: 0x00195D64
		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			(entity as CH47HelicopterAIController).SetMoveTarget(brain.mainInterestPoint);
			return StateStatus.Running;
		}

		// Token: 0x06004D4C RID: 19788 RVA: 0x00197B82 File Offset: 0x00195D82
		public bool AtPatrolDestination()
		{
			return Vector3Ex.Distance2D(this.GetDestination(), this.brain.transform.position) < this.patrolApproachDist;
		}

		// Token: 0x06004D4D RID: 19789 RVA: 0x001979BF File Offset: 0x00195BBF
		public Vector3 GetDestination()
		{
			return this.brain.mainInterestPoint;
		}

		// Token: 0x06004D4E RID: 19790 RVA: 0x00197BA7 File Offset: 0x00195DA7
		public override bool CanInterrupt()
		{
			return base.CanInterrupt() && this.AtPatrolDestination();
		}

		// Token: 0x06004D4F RID: 19791 RVA: 0x00197BBC File Offset: 0x00195DBC
		public override float GetWeight()
		{
			if (!base.IsInState())
			{
				float num = Mathf.InverseLerp(70f, 120f, base.TimeSinceState()) * 5f;
				return 1f + num;
			}
			if (this.AtPatrolDestination() && base.TimeInState > 2f)
			{
				return 0f;
			}
			return 3f;
		}

		// Token: 0x0400436F RID: 17263
		protected float patrolApproachDist = 75f;
	}
}
