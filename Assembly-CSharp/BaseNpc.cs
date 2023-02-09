using System;
using System.Collections;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

// Token: 0x0200003E RID: 62
public class BaseNpc : BaseCombatEntity
{
	// Token: 0x06000426 RID: 1062 RVA: 0x000328B0 File Offset: 0x00030AB0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseNpc.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000427 RID: 1063 RVA: 0x000328F0 File Offset: 0x00030AF0
	public void UpdateDestination(Vector3 position)
	{
		if (this.IsStopped)
		{
			this.IsStopped = false;
		}
		if ((this.Destination - position).sqrMagnitude > 0.010000001f)
		{
			this.Destination = position;
		}
		this.ChaseTransform = null;
	}

	// Token: 0x06000428 RID: 1064 RVA: 0x00032935 File Offset: 0x00030B35
	public void UpdateDestination(Transform tx)
	{
		this.IsStopped = false;
		this.ChaseTransform = tx;
	}

	// Token: 0x06000429 RID: 1065 RVA: 0x00032945 File Offset: 0x00030B45
	public void StopMoving()
	{
		this.IsStopped = true;
		this.ChaseTransform = null;
		this.SetFact(BaseNpc.Facts.PathToTargetStatus, 0, true, true);
	}

	// Token: 0x0600042A RID: 1066 RVA: 0x00032960 File Offset: 0x00030B60
	public override void ApplyInheritedVelocity(Vector3 velocity)
	{
		this.ServerPosition = BaseNpc.GetNewNavPosWithVelocity(this, velocity);
	}

	// Token: 0x0600042B RID: 1067 RVA: 0x00032970 File Offset: 0x00030B70
	public static Vector3 GetNewNavPosWithVelocity(global::BaseEntity ent, Vector3 velocity)
	{
		global::BaseEntity parentEntity = ent.GetParentEntity();
		if (parentEntity != null)
		{
			velocity = parentEntity.transform.InverseTransformDirection(velocity);
		}
		Vector3 targetPosition = ent.ServerPosition + velocity * UnityEngine.Time.fixedDeltaTime;
		NavMeshHit navMeshHit;
		NavMesh.Raycast(ent.ServerPosition, targetPosition, out navMeshHit, -1);
		if (!navMeshHit.position.IsNaNOrInfinity())
		{
			return navMeshHit.position;
		}
		return ent.ServerPosition;
	}

	// Token: 0x1700004C RID: 76
	// (get) Token: 0x0600042C RID: 1068 RVA: 0x000329E0 File Offset: 0x00030BE0
	// (set) Token: 0x0600042D RID: 1069 RVA: 0x000329E8 File Offset: 0x00030BE8
	public int AgentTypeIndex
	{
		get
		{
			return this.agentTypeIndex;
		}
		set
		{
			this.agentTypeIndex = value;
		}
	}

	// Token: 0x1700004D RID: 77
	// (get) Token: 0x0600042E RID: 1070 RVA: 0x000329F1 File Offset: 0x00030BF1
	// (set) Token: 0x0600042F RID: 1071 RVA: 0x000329F9 File Offset: 0x00030BF9
	public bool IsStuck { get; set; }

	// Token: 0x1700004E RID: 78
	// (get) Token: 0x06000430 RID: 1072 RVA: 0x00032A02 File Offset: 0x00030C02
	// (set) Token: 0x06000431 RID: 1073 RVA: 0x00032A0A File Offset: 0x00030C0A
	public bool AgencyUpdateRequired { get; set; }

	// Token: 0x1700004F RID: 79
	// (get) Token: 0x06000432 RID: 1074 RVA: 0x00032A13 File Offset: 0x00030C13
	// (set) Token: 0x06000433 RID: 1075 RVA: 0x00032A1B File Offset: 0x00030C1B
	public bool IsOnOffmeshLinkAndReachedNewCoord { get; set; }

	// Token: 0x06000434 RID: 1076 RVA: 0x00032A24 File Offset: 0x00030C24
	public override string DebugText()
	{
		return base.DebugText() + string.Format("\nBehaviour: {0}", this.CurrentBehaviour) + string.Format("\nAttackTarget: {0}", this.AttackTarget) + string.Format("\nFoodTarget: {0}", this.FoodTarget) + string.Format("\nSleep: {0:0.00}", this.Sleep);
	}

	// Token: 0x06000435 RID: 1077 RVA: 0x00032A98 File Offset: 0x00030C98
	public void TickAi()
	{
		if (!AI.think)
		{
			return;
		}
		if (TerrainMeta.WaterMap != null)
		{
			this.waterDepth = TerrainMeta.WaterMap.GetDepth(this.ServerPosition);
			this.wasSwimming = this.swimming;
			this.swimming = (this.waterDepth > this.Stats.WaterLevelNeck * 0.25f);
		}
		else
		{
			this.wasSwimming = false;
			this.swimming = false;
			this.waterDepth = 0f;
		}
		using (TimeWarning.New("TickNavigation", 0))
		{
			this.TickNavigation();
		}
		if (!AiManager.ai_dormant || this.GetNavAgent.enabled || this.CurrentBehaviour == BaseNpc.Behaviour.Sleep || this.NewAI)
		{
			using (TimeWarning.New("TickMetabolism", 0))
			{
				this.TickSleep();
				this.TickMetabolism();
				this.TickSpeed();
			}
		}
	}

	// Token: 0x06000436 RID: 1078 RVA: 0x00032BA0 File Offset: 0x00030DA0
	private void TickSpeed()
	{
		if (!this.LegacyNavigation)
		{
			return;
		}
		float num = this.Stats.Speed;
		if (this.NewAI)
		{
			num = (this.swimming ? this.ToSpeed(BaseNpc.SpeedEnum.Walk) : this.TargetSpeed);
			num *= 0.5f + base.healthFraction * 0.5f;
			this.NavAgent.speed = Mathf.Lerp(this.NavAgent.speed, num, 0.5f);
			this.NavAgent.angularSpeed = this.Stats.TurnSpeed;
			this.NavAgent.acceleration = this.Stats.Acceleration;
			return;
		}
		num *= 0.5f + base.healthFraction * 0.5f;
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Idle)
		{
			num *= 0.2f;
		}
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Eat)
		{
			num *= 0.3f;
		}
		float num2 = Mathf.Min(this.NavAgent.speed / this.Stats.Speed, 1f);
		num2 = BaseNpc.speedFractionResponse.Evaluate(num2);
		float num3 = 1f - 0.9f * Vector3.Angle(base.transform.forward, (this.NavAgent.nextPosition - this.ServerPosition).normalized) / 180f * num2 * num2;
		num *= num3;
		this.NavAgent.speed = Mathf.Lerp(this.NavAgent.speed, num, 0.5f);
		this.NavAgent.angularSpeed = this.Stats.TurnSpeed * (1.1f - num2);
		this.NavAgent.acceleration = this.Stats.Acceleration;
	}

	// Token: 0x06000437 RID: 1079 RVA: 0x00032D48 File Offset: 0x00030F48
	protected virtual void TickMetabolism()
	{
		float num = 0.00016666666f;
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Sleep)
		{
			num *= 0.01f;
		}
		if (this.NavAgent.desiredVelocity.sqrMagnitude > 0.1f)
		{
			num *= 2f;
		}
		this.Energy.Add(num * 0.1f * -1f);
		if (this.Stamina.TimeSinceUsed > 5f)
		{
			float num2 = 0.06666667f;
			this.Stamina.Add(0.1f * num2);
		}
		float secondsSinceAttacked = base.SecondsSinceAttacked;
	}

	// Token: 0x06000438 RID: 1080 RVA: 0x00032DDD File Offset: 0x00030FDD
	public virtual bool WantsToEat(global::BaseEntity best)
	{
		return best.HasTrait(global::BaseEntity.TraitFlag.Food) && !best.HasTrait(global::BaseEntity.TraitFlag.Alive);
	}

	// Token: 0x06000439 RID: 1081 RVA: 0x00032DF8 File Offset: 0x00030FF8
	public virtual float FearLevel(global::BaseEntity ent)
	{
		float num = 0f;
		BaseNpc baseNpc = ent as BaseNpc;
		if (baseNpc != null && baseNpc.Stats.Size > this.Stats.Size)
		{
			if (baseNpc.WantsToAttack(this) > 0.25f)
			{
				num += 0.2f;
			}
			if (baseNpc.AttackTarget == this)
			{
				num += 0.3f;
			}
			if (baseNpc.CurrentBehaviour == BaseNpc.Behaviour.Attack)
			{
				num *= 1.5f;
			}
			if (baseNpc.CurrentBehaviour == BaseNpc.Behaviour.Sleep)
			{
				num *= 0.1f;
			}
		}
		if (ent as global::BasePlayer != null)
		{
			num += 1f;
		}
		return num;
	}

	// Token: 0x0600043A RID: 1082 RVA: 0x00026FFC File Offset: 0x000251FC
	public virtual float HateLevel(global::BaseEntity ent)
	{
		return 0f;
	}

	// Token: 0x0600043B RID: 1083 RVA: 0x00032E98 File Offset: 0x00031098
	protected virtual void TickSleep()
	{
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Sleep)
		{
			this.IsSleeping = true;
			this.Sleep += 0.00033333336f;
		}
		else
		{
			this.IsSleeping = false;
			this.Sleep -= 2.7777778E-05f;
		}
		this.Sleep = Mathf.Clamp01(this.Sleep);
	}

	// Token: 0x0600043C RID: 1084 RVA: 0x00032EF4 File Offset: 0x000310F4
	public void TickNavigationWater()
	{
		if (!this.LegacyNavigation)
		{
			return;
		}
		if (!AI.move)
		{
			return;
		}
		if (!this.IsNavRunning())
		{
			return;
		}
		if (this.IsDormant || !this.syncPosition)
		{
			this.StopMoving();
			return;
		}
		Vector3 position = base.transform.position;
		this.stepDirection = Vector3.zero;
		if (this.ChaseTransform)
		{
			this.TickChase();
		}
		if (this.NavAgent.isOnOffMeshLink)
		{
			this.HandleNavMeshLinkTraversal(0.1f, ref position);
		}
		else if (this.NavAgent.hasPath)
		{
			this.TickFollowPath(ref position);
		}
		if (!this.ValidateNextPosition(ref position))
		{
			return;
		}
		position.y = 0f - this.Stats.WaterLevelNeck;
		this.UpdatePositionAndRotation(position);
		this.TickIdle();
		this.TickStuck();
	}

	// Token: 0x0600043D RID: 1085 RVA: 0x00032FC4 File Offset: 0x000311C4
	public void TickNavigation()
	{
		if (!this.LegacyNavigation)
		{
			return;
		}
		if (!AI.move)
		{
			return;
		}
		if (!this.IsNavRunning())
		{
			return;
		}
		if (this.IsDormant || !this.syncPosition)
		{
			this.StopMoving();
			return;
		}
		Vector3 position = base.transform.position;
		this.stepDirection = Vector3.zero;
		if (this.ChaseTransform)
		{
			this.TickChase();
		}
		if (this.NavAgent.isOnOffMeshLink)
		{
			this.HandleNavMeshLinkTraversal(0.1f, ref position);
		}
		else if (this.NavAgent.hasPath)
		{
			this.TickFollowPath(ref position);
		}
		if (!this.ValidateNextPosition(ref position))
		{
			return;
		}
		this.UpdatePositionAndRotation(position);
		this.TickIdle();
		this.TickStuck();
	}

	// Token: 0x0600043E RID: 1086 RVA: 0x0003307C File Offset: 0x0003127C
	private void TickChase()
	{
		Vector3 vector = this.ChaseTransform.position;
		Vector3 vector2 = base.transform.position - vector;
		if ((double)vector2.magnitude < 5.0)
		{
			vector += vector2.normalized * this.AttackOffset.z;
		}
		if ((this.NavAgent.destination - vector).sqrMagnitude > 0.010000001f)
		{
			this.NavAgent.SetDestination(vector);
		}
	}

	// Token: 0x0600043F RID: 1087 RVA: 0x00033105 File Offset: 0x00031305
	private void HandleNavMeshLinkTraversal(float delta, ref Vector3 moveToPosition)
	{
		if (!this._traversingNavMeshLink && !this.HandleNavMeshLinkTraversalStart(delta))
		{
			return;
		}
		this.HandleNavMeshLinkTraversalTick(delta, ref moveToPosition);
		if (!this.IsNavMeshLinkTraversalComplete(delta, ref moveToPosition))
		{
			this._currentNavMeshLinkTraversalTimeDelta += delta;
		}
	}

	// Token: 0x06000440 RID: 1088 RVA: 0x0003313C File Offset: 0x0003133C
	private bool HandleNavMeshLinkTraversalStart(float delta)
	{
		OffMeshLinkData currentOffMeshLinkData = this.NavAgent.currentOffMeshLinkData;
		if (!currentOffMeshLinkData.valid || !currentOffMeshLinkData.activated || currentOffMeshLinkData.offMeshLink == null)
		{
			return false;
		}
		Vector3 normalized = (currentOffMeshLinkData.endPos - currentOffMeshLinkData.startPos).normalized;
		normalized.y = 0f;
		Vector3 desiredVelocity = this.NavAgent.desiredVelocity;
		desiredVelocity.y = 0f;
		if (Vector3.Dot(desiredVelocity, normalized) < 0.1f)
		{
			this.CompleteNavMeshLink();
			return false;
		}
		this._currentNavMeshLink = currentOffMeshLinkData;
		this._currentNavMeshLinkName = this._currentNavMeshLink.linkType.ToString();
		if (currentOffMeshLinkData.offMeshLink.biDirectional)
		{
			if ((currentOffMeshLinkData.endPos - this.ServerPosition).sqrMagnitude < 0.05f)
			{
				this._currentNavMeshLinkEndPos = currentOffMeshLinkData.startPos;
				this._currentNavMeshLinkOrientation = Quaternion.LookRotation(currentOffMeshLinkData.startPos + Vector3.up * (currentOffMeshLinkData.endPos.y - currentOffMeshLinkData.startPos.y) - currentOffMeshLinkData.endPos);
			}
			else
			{
				this._currentNavMeshLinkEndPos = currentOffMeshLinkData.endPos;
				this._currentNavMeshLinkOrientation = Quaternion.LookRotation(currentOffMeshLinkData.endPos + Vector3.up * (currentOffMeshLinkData.startPos.y - currentOffMeshLinkData.endPos.y) - currentOffMeshLinkData.startPos);
			}
		}
		else
		{
			this._currentNavMeshLinkEndPos = currentOffMeshLinkData.endPos;
			this._currentNavMeshLinkOrientation = Quaternion.LookRotation(currentOffMeshLinkData.endPos + Vector3.up * (currentOffMeshLinkData.startPos.y - currentOffMeshLinkData.endPos.y) - currentOffMeshLinkData.startPos);
		}
		this._traversingNavMeshLink = true;
		this.NavAgent.ActivateCurrentOffMeshLink(false);
		this.NavAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
		float num = Mathf.Max(this.NavAgent.speed, 2.8f);
		float magnitude = (this._currentNavMeshLink.startPos - this._currentNavMeshLink.endPos).magnitude;
		this._currentNavMeshLinkTraversalTime = magnitude / num;
		this._currentNavMeshLinkTraversalTimeDelta = 0f;
		if (!(this._currentNavMeshLinkName == "OpenDoorLink") && !(this._currentNavMeshLinkName == "JumpRockLink"))
		{
			this._currentNavMeshLinkName == "JumpFoundationLink";
		}
		return true;
	}

	// Token: 0x06000441 RID: 1089 RVA: 0x000333D0 File Offset: 0x000315D0
	private void HandleNavMeshLinkTraversalTick(float delta, ref Vector3 moveToPosition)
	{
		if (this._currentNavMeshLinkName == "OpenDoorLink")
		{
			moveToPosition = Vector3.Lerp(this._currentNavMeshLink.startPos, this._currentNavMeshLink.endPos, this._currentNavMeshLinkTraversalTimeDelta);
			return;
		}
		if (this._currentNavMeshLinkName == "JumpRockLink")
		{
			moveToPosition = Vector3.Lerp(this._currentNavMeshLink.startPos, this._currentNavMeshLink.endPos, this._currentNavMeshLinkTraversalTimeDelta);
			return;
		}
		if (this._currentNavMeshLinkName == "JumpFoundationLink")
		{
			moveToPosition = Vector3.Lerp(this._currentNavMeshLink.startPos, this._currentNavMeshLink.endPos, this._currentNavMeshLinkTraversalTimeDelta);
			return;
		}
		moveToPosition = Vector3.Lerp(this._currentNavMeshLink.startPos, this._currentNavMeshLink.endPos, this._currentNavMeshLinkTraversalTimeDelta);
	}

	// Token: 0x06000442 RID: 1090 RVA: 0x000334B4 File Offset: 0x000316B4
	private bool IsNavMeshLinkTraversalComplete(float delta, ref Vector3 moveToPosition)
	{
		if (this._currentNavMeshLinkTraversalTimeDelta >= this._currentNavMeshLinkTraversalTime)
		{
			moveToPosition = this._currentNavMeshLink.endPos;
			this._traversingNavMeshLink = false;
			this._currentNavMeshLink = default(OffMeshLinkData);
			this._currentNavMeshLinkTraversalTime = 0f;
			this._currentNavMeshLinkTraversalTimeDelta = 0f;
			this._currentNavMeshLinkName = string.Empty;
			this._currentNavMeshLinkOrientation = Quaternion.identity;
			this.CompleteNavMeshLink();
			return true;
		}
		return false;
	}

	// Token: 0x06000443 RID: 1091 RVA: 0x00033528 File Offset: 0x00031728
	private void CompleteNavMeshLink()
	{
		this.NavAgent.ActivateCurrentOffMeshLink(true);
		this.NavAgent.CompleteOffMeshLink();
		this.NavAgent.isStopped = false;
		this.NavAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
	}

	// Token: 0x06000444 RID: 1092 RVA: 0x0003355C File Offset: 0x0003175C
	private void TickFollowPath(ref Vector3 moveToPosition)
	{
		moveToPosition = this.NavAgent.nextPosition;
		this.stepDirection = this.NavAgent.desiredVelocity.normalized;
	}

	// Token: 0x06000445 RID: 1093 RVA: 0x00033594 File Offset: 0x00031794
	private bool ValidateNextPosition(ref Vector3 moveToPosition)
	{
		if (!ValidBounds.Test(moveToPosition) && base.transform != null && !base.IsDestroyed)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Invalid NavAgent Position: ",
				this,
				" ",
				moveToPosition,
				" (destroying)"
			}));
			base.Kill(global::BaseNetworkable.DestroyMode.None);
			return false;
		}
		return true;
	}

	// Token: 0x06000446 RID: 1094 RVA: 0x00033608 File Offset: 0x00031808
	private void UpdatePositionAndRotation(Vector3 moveToPosition)
	{
		this.ServerPosition = moveToPosition;
		this.UpdateAiRotation();
	}

	// Token: 0x06000447 RID: 1095 RVA: 0x00033617 File Offset: 0x00031817
	private void TickIdle()
	{
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Idle)
		{
			this.idleDuration += 0.1f;
			return;
		}
		this.idleDuration = 0f;
	}

	// Token: 0x06000448 RID: 1096 RVA: 0x00033640 File Offset: 0x00031840
	public void TickStuck()
	{
		if (this.IsNavRunning() && !this.NavAgent.isStopped && (this.lastStuckPos - this.ServerPosition).sqrMagnitude < 0.0625f && this.AttackReady())
		{
			this.stuckDuration += 0.1f;
			if (this.stuckDuration >= 5f && Mathf.Approximately(this.lastStuckTime, 0f))
			{
				this.lastStuckTime = UnityEngine.Time.time;
				this.OnBecomeStuck();
				return;
			}
		}
		else
		{
			this.stuckDuration = 0f;
			this.lastStuckPos = this.ServerPosition;
			if (UnityEngine.Time.time - this.lastStuckTime > 5f)
			{
				this.lastStuckTime = 0f;
				this.OnBecomeUnStuck();
			}
		}
	}

	// Token: 0x06000449 RID: 1097 RVA: 0x00033708 File Offset: 0x00031908
	public void OnBecomeStuck()
	{
		this.IsStuck = true;
	}

	// Token: 0x0600044A RID: 1098 RVA: 0x00033711 File Offset: 0x00031911
	public void OnBecomeUnStuck()
	{
		this.IsStuck = false;
	}

	// Token: 0x0600044B RID: 1099 RVA: 0x0003371C File Offset: 0x0003191C
	public void UpdateAiRotation()
	{
		if (!this.IsNavRunning())
		{
			return;
		}
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Sleep)
		{
			return;
		}
		if (this._traversingNavMeshLink)
		{
			Vector3 vector;
			if (this.ChaseTransform != null)
			{
				vector = this.ChaseTransform.localPosition - this.ServerPosition;
			}
			else if (this.AttackTarget != null)
			{
				vector = this.AttackTarget.ServerPosition - this.ServerPosition;
			}
			else
			{
				vector = this.NavAgent.destination - this.ServerPosition;
			}
			if (vector.sqrMagnitude > 1f)
			{
				vector = this._currentNavMeshLinkEndPos - this.ServerPosition;
			}
			if (vector.sqrMagnitude > 0.001f)
			{
				this.ServerRotation = this._currentNavMeshLinkOrientation;
				return;
			}
		}
		else if ((this.NavAgent.destination - this.ServerPosition).sqrMagnitude > 1f)
		{
			Vector3 forward = this.stepDirection;
			if (forward.sqrMagnitude > 0.001f)
			{
				this.ServerRotation = Quaternion.LookRotation(forward);
				return;
			}
		}
		if (this.ChaseTransform && this.CurrentBehaviour == BaseNpc.Behaviour.Attack)
		{
			Vector3 vector2 = this.ChaseTransform.localPosition - this.ServerPosition;
			float sqrMagnitude = vector2.sqrMagnitude;
			if (sqrMagnitude < 9f && sqrMagnitude > 0.001f)
			{
				this.ServerRotation = Quaternion.LookRotation(vector2.normalized);
				return;
			}
		}
		else if (this.AttackTarget && this.CurrentBehaviour == BaseNpc.Behaviour.Attack)
		{
			Vector3 vector3 = this.AttackTarget.ServerPosition - this.ServerPosition;
			float sqrMagnitude2 = vector3.sqrMagnitude;
			if (sqrMagnitude2 < 9f && sqrMagnitude2 > 0.001f)
			{
				this.ServerRotation = Quaternion.LookRotation(vector3.normalized);
				return;
			}
		}
	}

	// Token: 0x17000050 RID: 80
	// (get) Token: 0x0600044C RID: 1100 RVA: 0x000338E5 File Offset: 0x00031AE5
	public float GetAttackRate
	{
		get
		{
			return this.AttackRate;
		}
	}

	// Token: 0x0600044D RID: 1101 RVA: 0x000338ED File Offset: 0x00031AED
	public bool AttackReady()
	{
		return UnityEngine.Time.realtimeSinceStartup >= this.nextAttackTime;
	}

	// Token: 0x0600044E RID: 1102 RVA: 0x00033900 File Offset: 0x00031B00
	public virtual void StartAttack()
	{
		if (!this.AttackTarget)
		{
			return;
		}
		if (!this.AttackReady())
		{
			return;
		}
		if ((this.AttackTarget.ServerPosition - this.ServerPosition).magnitude > this.AttackRange)
		{
			return;
		}
		this.nextAttackTime = UnityEngine.Time.realtimeSinceStartup + this.AttackRate;
		BaseCombatEntity combatTarget = this.CombatTarget;
		if (!combatTarget)
		{
			return;
		}
		combatTarget.Hurt(this.AttackDamage, this.AttackDamageType, this, true);
		this.Stamina.Use(this.AttackCost);
		this.BusyTimer.Activate(0.5f, null);
		base.SignalBroadcast(global::BaseEntity.Signal.Attack, null);
		base.ClientRPC<Vector3>(null, "Attack", this.AttackTarget.ServerPosition);
	}

	// Token: 0x0600044F RID: 1103 RVA: 0x000339C4 File Offset: 0x00031BC4
	public void Attack(BaseCombatEntity target)
	{
		if (target == null)
		{
			return;
		}
		Vector3 vector = target.ServerPosition - this.ServerPosition;
		if (vector.magnitude > 0.001f)
		{
			this.ServerRotation = Quaternion.LookRotation(vector.normalized);
		}
		this.nextAttackTime = UnityEngine.Time.realtimeSinceStartup + this.AttackRate;
		target.Hurt(this.AttackDamage, this.AttackDamageType, this, true);
		this.Stamina.Use(this.AttackCost);
		base.SignalBroadcast(global::BaseEntity.Signal.Attack, null);
		base.ClientRPC<Vector3>(null, "Attack", target.ServerPosition);
	}

	// Token: 0x06000450 RID: 1104 RVA: 0x00033A60 File Offset: 0x00031C60
	public virtual void Eat()
	{
		if (!this.FoodTarget)
		{
			return;
		}
		this.BusyTimer.Activate(0.5f, null);
		this.FoodTarget.Eat(this, 0.5f);
		this.StartEating(UnityEngine.Random.value * 5f + 0.5f);
		base.ClientRPC<Vector3>(null, "Eat", this.FoodTarget.transform.position);
	}

	// Token: 0x06000451 RID: 1105 RVA: 0x00033AD1 File Offset: 0x00031CD1
	public virtual void AddCalories(float amount)
	{
		this.Energy.Add(amount / 1000f);
	}

	// Token: 0x06000452 RID: 1106 RVA: 0x00033AE5 File Offset: 0x00031CE5
	public virtual void Startled()
	{
		base.ClientRPC<Vector3>(null, "Startled", base.transform.position);
	}

	// Token: 0x06000453 RID: 1107 RVA: 0x00033AFE File Offset: 0x00031CFE
	private bool IsAfraid()
	{
		this.SetFact(BaseNpc.Facts.IsAfraid, 0, true, true);
		return false;
	}

	// Token: 0x06000454 RID: 1108 RVA: 0x00033B0C File Offset: 0x00031D0C
	protected bool IsAfraidOf(BaseNpc.AiStatistics.FamilyEnum family)
	{
		foreach (BaseNpc.AiStatistics.FamilyEnum familyEnum in this.Stats.IsAfraidOf)
		{
			if (family == familyEnum)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000455 RID: 1109 RVA: 0x00033B40 File Offset: 0x00031D40
	private bool CheckHealthThresholdToFlee()
	{
		if (base.healthFraction > this.Stats.HealthThresholdForFleeing)
		{
			if (this.Stats.HealthThresholdForFleeing < 1f)
			{
				this.SetFact(BaseNpc.Facts.IsUnderHealthThreshold, 0, true, true);
				return false;
			}
			if (this.GetFact(BaseNpc.Facts.HasEnemy) == 1)
			{
				this.SetFact(BaseNpc.Facts.IsUnderHealthThreshold, 0, true, true);
				return false;
			}
		}
		bool flag = UnityEngine.Random.value < this.Stats.HealthThresholdFleeChance;
		this.SetFact(BaseNpc.Facts.IsUnderHealthThreshold, flag ? 1 : 0, true, true);
		return flag;
	}

	// Token: 0x06000456 RID: 1110 RVA: 0x00033BBC File Offset: 0x00031DBC
	private void TickBehaviourState()
	{
		if (this.GetFact(BaseNpc.Facts.WantsToFlee) == 1 && this.IsNavRunning() && this.NavAgent.pathStatus == NavMeshPathStatus.PathComplete && UnityEngine.Time.realtimeSinceStartup - (this.maxFleeTime - this.Stats.MaxFleeTime) > 0.5f)
		{
			this.TickFlee();
		}
		if (this.GetFact(BaseNpc.Facts.CanTargetEnemies) == 0)
		{
			this.TickBlockEnemyTargeting();
		}
		if (this.GetFact(BaseNpc.Facts.CanTargetFood) == 0)
		{
			this.TickBlockFoodTargeting();
		}
		if (this.GetFact(BaseNpc.Facts.IsAggro) == 1)
		{
			this.TickAggro();
		}
		if (this.GetFact(BaseNpc.Facts.IsEating) == 1)
		{
			this.TickEating();
		}
		if (this.GetFact(BaseNpc.Facts.CanNotMove) == 1)
		{
			this.TickWakeUpBlockMove();
		}
	}

	// Token: 0x06000457 RID: 1111 RVA: 0x00033C60 File Offset: 0x00031E60
	private void WantsToFlee()
	{
		if (this.GetFact(BaseNpc.Facts.WantsToFlee) == 1 || !this.IsNavRunning())
		{
			return;
		}
		this.SetFact(BaseNpc.Facts.WantsToFlee, 1, true, true);
		this.maxFleeTime = UnityEngine.Time.realtimeSinceStartup + this.Stats.MaxFleeTime;
	}

	// Token: 0x06000458 RID: 1112 RVA: 0x000059DD File Offset: 0x00003BDD
	private void TickFlee()
	{
	}

	// Token: 0x06000459 RID: 1113 RVA: 0x00033C98 File Offset: 0x00031E98
	public bool BlockEnemyTargeting(float timeout)
	{
		if (this.GetFact(BaseNpc.Facts.CanTargetEnemies) == 0)
		{
			return false;
		}
		this.SetFact(BaseNpc.Facts.CanTargetEnemies, 0, true, true);
		this.blockEnemyTargetingTimeout = UnityEngine.Time.realtimeSinceStartup + timeout;
		this.blockTargetingThisEnemy = this.AttackTarget;
		return true;
	}

	// Token: 0x0600045A RID: 1114 RVA: 0x00033CC9 File Offset: 0x00031EC9
	private void TickBlockEnemyTargeting()
	{
		if (this.GetFact(BaseNpc.Facts.CanTargetEnemies) == 1)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup > this.blockEnemyTargetingTimeout)
		{
			this.SetFact(BaseNpc.Facts.CanTargetEnemies, 1, true, true);
		}
	}

	// Token: 0x0600045B RID: 1115 RVA: 0x00033CED File Offset: 0x00031EED
	public bool BlockFoodTargeting(float timeout)
	{
		if (this.GetFact(BaseNpc.Facts.CanTargetFood) == 0)
		{
			return false;
		}
		this.SetFact(BaseNpc.Facts.CanTargetFood, 0, true, true);
		this.blockFoodTargetingTimeout = UnityEngine.Time.realtimeSinceStartup + timeout;
		return true;
	}

	// Token: 0x0600045C RID: 1116 RVA: 0x00033D14 File Offset: 0x00031F14
	private void TickBlockFoodTargeting()
	{
		if (this.GetFact(BaseNpc.Facts.CanTargetFood) == 1)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup > this.blockFoodTargetingTimeout)
		{
			this.SetFact(BaseNpc.Facts.CanTargetFood, 1, true, true);
		}
	}

	// Token: 0x0600045D RID: 1117 RVA: 0x00033D3C File Offset: 0x00031F3C
	public bool TryAggro(BaseNpc.EnemyRangeEnum range)
	{
		if (Mathf.Approximately(this.Stats.Hostility, 0f) && Mathf.Approximately(this.Stats.Defensiveness, 0f))
		{
			return false;
		}
		if (this.GetFact(BaseNpc.Facts.IsAggro) == 0 && (range == BaseNpc.EnemyRangeEnum.AggroRange || range == BaseNpc.EnemyRangeEnum.AttackRange))
		{
			float num = (range == BaseNpc.EnemyRangeEnum.AttackRange) ? 1f : this.Stats.Defensiveness;
			num = Mathf.Max(num, this.Stats.Hostility);
			if (UnityEngine.Time.realtimeSinceStartup > this.lastAggroChanceCalcTime + 5f)
			{
				this.lastAggroChanceResult = UnityEngine.Random.value;
				this.lastAggroChanceCalcTime = UnityEngine.Time.realtimeSinceStartup;
			}
			if (this.lastAggroChanceResult < num)
			{
				return this.StartAggro(this.Stats.DeaggroChaseTime);
			}
		}
		return false;
	}

	// Token: 0x0600045E RID: 1118 RVA: 0x00033DF7 File Offset: 0x00031FF7
	public bool StartAggro(float timeout)
	{
		if (this.GetFact(BaseNpc.Facts.IsAggro) == 1)
		{
			return false;
		}
		this.SetFact(BaseNpc.Facts.IsAggro, 1, true, true);
		this.aggroTimeout = UnityEngine.Time.realtimeSinceStartup + timeout;
		return true;
	}

	// Token: 0x0600045F RID: 1119 RVA: 0x000059DD File Offset: 0x00003BDD
	private void TickAggro()
	{
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x00033E1F File Offset: 0x0003201F
	public bool StartEating(float timeout)
	{
		if (this.GetFact(BaseNpc.Facts.IsEating) == 1)
		{
			return false;
		}
		this.SetFact(BaseNpc.Facts.IsEating, 1, true, true);
		this.eatTimeout = UnityEngine.Time.realtimeSinceStartup + timeout;
		return true;
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x00033E47 File Offset: 0x00032047
	private void TickEating()
	{
		if (this.GetFact(BaseNpc.Facts.IsEating) == 0)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup > this.eatTimeout)
		{
			this.SetFact(BaseNpc.Facts.IsEating, 0, true, true);
		}
	}

	// Token: 0x06000462 RID: 1122 RVA: 0x00033E6C File Offset: 0x0003206C
	public bool WakeUpBlockMove(float timeout)
	{
		if (this.GetFact(BaseNpc.Facts.CanNotMove) == 1)
		{
			return false;
		}
		this.SetFact(BaseNpc.Facts.CanNotMove, 1, true, true);
		this.wakeUpBlockMoveTimeout = UnityEngine.Time.realtimeSinceStartup + timeout;
		return true;
	}

	// Token: 0x06000463 RID: 1123 RVA: 0x00033E94 File Offset: 0x00032094
	private void TickWakeUpBlockMove()
	{
		if (this.GetFact(BaseNpc.Facts.CanNotMove) == 0)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup > this.wakeUpBlockMoveTimeout)
		{
			this.SetFact(BaseNpc.Facts.CanNotMove, 0, true, true);
		}
	}

	// Token: 0x06000464 RID: 1124 RVA: 0x00033EBC File Offset: 0x000320BC
	private void OnFactChanged(BaseNpc.Facts fact, byte oldValue, byte newValue)
	{
		if (fact <= BaseNpc.Facts.IsAggro)
		{
			switch (fact)
			{
			case BaseNpc.Facts.CanTargetEnemies:
				if (newValue == 1)
				{
					this.blockTargetingThisEnemy = null;
				}
				break;
			case BaseNpc.Facts.Health:
			case BaseNpc.Facts.IsTired:
				break;
			case BaseNpc.Facts.Speed:
				if (newValue == 0)
				{
					this.StopMoving();
					this.CurrentBehaviour = BaseNpc.Behaviour.Idle;
					return;
				}
				if (newValue != 1)
				{
					this.IsStopped = false;
					return;
				}
				this.IsStopped = false;
				this.CurrentBehaviour = BaseNpc.Behaviour.Wander;
				return;
			case BaseNpc.Facts.IsSleeping:
				if (newValue > 0)
				{
					this.CurrentBehaviour = BaseNpc.Behaviour.Sleep;
					this.SetFact(BaseNpc.Facts.CanTargetEnemies, 0, false, true);
					this.SetFact(BaseNpc.Facts.CanTargetFood, 0, true, true);
					return;
				}
				this.CurrentBehaviour = BaseNpc.Behaviour.Idle;
				this.SetFact(BaseNpc.Facts.CanTargetEnemies, 1, true, true);
				this.SetFact(BaseNpc.Facts.CanTargetFood, 1, true, true);
				this.WakeUpBlockMove(this.Stats.WakeupBlockMoveTime);
				this.TickSenses();
				return;
			default:
				if (fact != BaseNpc.Facts.IsAggro)
				{
					return;
				}
				if (newValue > 0)
				{
					this.CurrentBehaviour = BaseNpc.Behaviour.Attack;
					return;
				}
				this.BlockEnemyTargeting(this.Stats.DeaggroCooldown);
				return;
			}
		}
		else if (fact != BaseNpc.Facts.FoodRange)
		{
			if (fact != BaseNpc.Facts.IsEating)
			{
				return;
			}
			if (newValue == 0)
			{
				this.FoodTarget = null;
				return;
			}
		}
		else if (newValue == 0)
		{
			this.CurrentBehaviour = BaseNpc.Behaviour.Eat;
			return;
		}
	}

	// Token: 0x06000465 RID: 1125 RVA: 0x00033FC4 File Offset: 0x000321C4
	public int TopologyPreference()
	{
		return (int)this.topologyPreference;
	}

	// Token: 0x06000466 RID: 1126 RVA: 0x00033FCC File Offset: 0x000321CC
	public bool HasAiFlag(BaseNpc.AiFlags f)
	{
		return (this.aiFlags & f) == f;
	}

	// Token: 0x06000467 RID: 1127 RVA: 0x00033FDC File Offset: 0x000321DC
	public void SetAiFlag(BaseNpc.AiFlags f, bool set)
	{
		BaseNpc.AiFlags aiFlags = this.aiFlags;
		if (set)
		{
			this.aiFlags |= f;
		}
		else
		{
			this.aiFlags &= ~f;
		}
		if (aiFlags != this.aiFlags && base.isServer)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x17000051 RID: 81
	// (get) Token: 0x06000468 RID: 1128 RVA: 0x00034028 File Offset: 0x00032228
	// (set) Token: 0x06000469 RID: 1129 RVA: 0x00034031 File Offset: 0x00032231
	public bool IsSitting
	{
		get
		{
			return this.HasAiFlag(BaseNpc.AiFlags.Sitting);
		}
		set
		{
			this.SetAiFlag(BaseNpc.AiFlags.Sitting, value);
		}
	}

	// Token: 0x17000052 RID: 82
	// (get) Token: 0x0600046A RID: 1130 RVA: 0x0003403B File Offset: 0x0003223B
	// (set) Token: 0x0600046B RID: 1131 RVA: 0x00034044 File Offset: 0x00032244
	public bool IsChasing
	{
		get
		{
			return this.HasAiFlag(BaseNpc.AiFlags.Chasing);
		}
		set
		{
			this.SetAiFlag(BaseNpc.AiFlags.Chasing, value);
		}
	}

	// Token: 0x17000053 RID: 83
	// (get) Token: 0x0600046C RID: 1132 RVA: 0x0003404E File Offset: 0x0003224E
	// (set) Token: 0x0600046D RID: 1133 RVA: 0x00034057 File Offset: 0x00032257
	public bool IsSleeping
	{
		get
		{
			return this.HasAiFlag(BaseNpc.AiFlags.Sleeping);
		}
		set
		{
			this.SetAiFlag(BaseNpc.AiFlags.Sleeping, value);
		}
	}

	// Token: 0x0600046E RID: 1134 RVA: 0x00034061 File Offset: 0x00032261
	public void InitFacts()
	{
		this.SetFact(BaseNpc.Facts.CanTargetEnemies, 1, true, true);
		this.SetFact(BaseNpc.Facts.CanTargetFood, 1, true, true);
	}

	// Token: 0x0600046F RID: 1135 RVA: 0x00034078 File Offset: 0x00032278
	public byte GetFact(BaseNpc.Facts fact)
	{
		return this.CurrentFacts[(int)fact];
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x00034084 File Offset: 0x00032284
	public void SetFact(BaseNpc.Facts fact, byte value, bool triggerCallback = true, bool onlyTriggerCallbackOnDiffValue = true)
	{
		byte b = this.CurrentFacts[(int)fact];
		this.CurrentFacts[(int)fact] = value;
		if (triggerCallback && value != b)
		{
			this.OnFactChanged(fact, b, value);
		}
	}

	// Token: 0x06000471 RID: 1137 RVA: 0x000340B4 File Offset: 0x000322B4
	public BaseNpc.EnemyRangeEnum ToEnemyRangeEnum(float range)
	{
		if (range <= this.AttackRange)
		{
			return BaseNpc.EnemyRangeEnum.AttackRange;
		}
		if (range <= this.Stats.AggressionRange)
		{
			return BaseNpc.EnemyRangeEnum.AggroRange;
		}
		if (range >= this.Stats.DeaggroRange && this.GetFact(BaseNpc.Facts.IsAggro) > 0)
		{
			return BaseNpc.EnemyRangeEnum.OutOfRange;
		}
		if (range <= this.Stats.VisionRange)
		{
			return BaseNpc.EnemyRangeEnum.AwareRange;
		}
		return BaseNpc.EnemyRangeEnum.OutOfRange;
	}

	// Token: 0x06000472 RID: 1138 RVA: 0x00034108 File Offset: 0x00032308
	public float GetActiveAggressionRangeSqr()
	{
		if (this.GetFact(BaseNpc.Facts.IsAggro) == 1)
		{
			return this.Stats.DeaggroRange * this.Stats.DeaggroRange;
		}
		return this.Stats.AggressionRange * this.Stats.AggressionRange;
	}

	// Token: 0x06000473 RID: 1139 RVA: 0x00034144 File Offset: 0x00032344
	public BaseNpc.FoodRangeEnum ToFoodRangeEnum(float range)
	{
		if (range <= 0.5f)
		{
			return BaseNpc.FoodRangeEnum.EatRange;
		}
		if (range <= this.Stats.VisionRange)
		{
			return BaseNpc.FoodRangeEnum.AwareRange;
		}
		return BaseNpc.FoodRangeEnum.OutOfRange;
	}

	// Token: 0x06000474 RID: 1140 RVA: 0x00034161 File Offset: 0x00032361
	public BaseNpc.AfraidRangeEnum ToAfraidRangeEnum(float range)
	{
		if (range <= this.Stats.AfraidRange)
		{
			return BaseNpc.AfraidRangeEnum.InAfraidRange;
		}
		return BaseNpc.AfraidRangeEnum.OutOfRange;
	}

	// Token: 0x06000475 RID: 1141 RVA: 0x00034174 File Offset: 0x00032374
	public BaseNpc.HealthEnum ToHealthEnum(float healthNormalized)
	{
		if (healthNormalized >= 0.75f)
		{
			return BaseNpc.HealthEnum.Fine;
		}
		if (healthNormalized >= 0.25f)
		{
			return BaseNpc.HealthEnum.Medium;
		}
		return BaseNpc.HealthEnum.Low;
	}

	// Token: 0x06000476 RID: 1142 RVA: 0x0003418C File Offset: 0x0003238C
	public byte ToIsTired(float energyNormalized)
	{
		bool flag = this.GetFact(BaseNpc.Facts.IsSleeping) == 1;
		if (!flag && energyNormalized < 0.1f)
		{
			return 1;
		}
		if (flag && energyNormalized < 0.5f)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06000477 RID: 1143 RVA: 0x000341BF File Offset: 0x000323BF
	public BaseNpc.SpeedEnum ToSpeedEnum(float speed)
	{
		if (speed <= 0.01f)
		{
			return BaseNpc.SpeedEnum.StandStill;
		}
		if (speed <= 0.18f)
		{
			return BaseNpc.SpeedEnum.Walk;
		}
		return BaseNpc.SpeedEnum.Run;
	}

	// Token: 0x06000478 RID: 1144 RVA: 0x000341D6 File Offset: 0x000323D6
	public float ToSpeed(BaseNpc.SpeedEnum speed)
	{
		if (speed == BaseNpc.SpeedEnum.StandStill)
		{
			return 0f;
		}
		if (speed != BaseNpc.SpeedEnum.Walk)
		{
			return this.Stats.Speed;
		}
		return 0.18f * this.Stats.Speed;
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x00034204 File Offset: 0x00032404
	public byte GetPathStatus()
	{
		if (!this.IsNavRunning())
		{
			return 2;
		}
		return (byte)this.NavAgent.pathStatus;
	}

	// Token: 0x0600047A RID: 1146 RVA: 0x0003421C File Offset: 0x0003241C
	public NavMeshPathStatus ToPathStatus(byte value)
	{
		return (NavMeshPathStatus)value;
	}

	// Token: 0x0600047B RID: 1147 RVA: 0x00034220 File Offset: 0x00032420
	private void TickSenses()
	{
		if (global::BaseEntity.Query.Server == null || this.IsDormant)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup > this.lastTickTime + this.SensesTickRate)
		{
			this.TickHearing();
			this.TickSmell();
			this.lastTickTime = UnityEngine.Time.realtimeSinceStartup;
		}
		if (!AI.animal_ignore_food)
		{
			this.TickFoodAwareness();
		}
		this.UpdateSelfFacts();
	}

	// Token: 0x0600047C RID: 1148 RVA: 0x0003427B File Offset: 0x0003247B
	private void TickHearing()
	{
		this.SetFact(BaseNpc.Facts.LoudNoiseNearby, 0, true, true);
	}

	// Token: 0x0600047D RID: 1149 RVA: 0x000059DD File Offset: 0x00003BDD
	private void TickSmell()
	{
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x00034288 File Offset: 0x00032488
	private float DecisionMomentumPlayerTarget()
	{
		float num = UnityEngine.Time.time - this.playerTargetDecisionStartTime;
		if (num > 1f)
		{
			return 0f;
		}
		return num;
	}

	// Token: 0x0600047F RID: 1151 RVA: 0x000342B4 File Offset: 0x000324B4
	private float DecisionMomentumAnimalTarget()
	{
		float num = UnityEngine.Time.time - this.animalTargetDecisionStartTime;
		if (num > 1f)
		{
			return 0f;
		}
		return num;
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x000342DD File Offset: 0x000324DD
	private void TickFoodAwareness()
	{
		if (this.GetFact(BaseNpc.Facts.CanTargetFood) == 0)
		{
			this.FoodTarget = null;
			this.SetFact(BaseNpc.Facts.FoodRange, 2, true, true);
			return;
		}
		this.SelectFood();
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x000059DD File Offset: 0x00003BDD
	private void SelectFood()
	{
	}

	// Token: 0x06000482 RID: 1154 RVA: 0x000059DD File Offset: 0x00003BDD
	private void SelectClosestFood()
	{
	}

	// Token: 0x06000483 RID: 1155 RVA: 0x000059DD File Offset: 0x00003BDD
	private void UpdateSelfFacts()
	{
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x00034304 File Offset: 0x00032504
	private byte IsMoving()
	{
		return (this.IsNavRunning() && this.NavAgent.hasPath && this.NavAgent.remainingDistance > this.NavAgent.stoppingDistance && !this.IsStuck && this.GetFact(BaseNpc.Facts.Speed) != 0) ? 1 : 0;
	}

	// Token: 0x06000485 RID: 1157 RVA: 0x00034354 File Offset: 0x00032554
	private static bool AiCaresAbout(global::BaseEntity ent)
	{
		if (ent is global::BasePlayer)
		{
			return true;
		}
		if (ent is BaseNpc)
		{
			return true;
		}
		if (!AI.animal_ignore_food)
		{
			if (ent is global::WorldItem)
			{
				return true;
			}
			if (ent is BaseCorpse)
			{
				return true;
			}
			if (ent is CollectibleEntity)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x00034390 File Offset: 0x00032590
	private static bool WithinVisionCone(BaseNpc npc, global::BaseEntity other)
	{
		if (Mathf.Approximately(npc.Stats.VisionCone, -1f))
		{
			return true;
		}
		Vector3 normalized = (other.ServerPosition - npc.ServerPosition).normalized;
		return Vector3.Dot(npc.transform.forward, normalized) >= npc.Stats.VisionCone;
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x000343F4 File Offset: 0x000325F4
	public void SetTargetPathStatus(float pendingDelay = 0.05f)
	{
		if (this.isAlreadyCheckingPathPending)
		{
			return;
		}
		if (this.NavAgent.pathPending && this.numPathPendingAttempts < 10)
		{
			this.isAlreadyCheckingPathPending = true;
			base.Invoke(new Action(this.DelayedTargetPathStatus), pendingDelay);
			return;
		}
		this.numPathPendingAttempts = 0;
		this.accumPathPendingDelay = 0f;
		this.SetFact(BaseNpc.Facts.PathToTargetStatus, this.GetPathStatus(), true, true);
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x0003445E File Offset: 0x0003265E
	private void DelayedTargetPathStatus()
	{
		this.accumPathPendingDelay += 0.1f;
		this.isAlreadyCheckingPathPending = false;
		this.SetTargetPathStatus(this.accumPathPendingDelay);
	}

	// Token: 0x06000489 RID: 1161 RVA: 0x00034488 File Offset: 0x00032688
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.NavAgent == null)
		{
			this.NavAgent = base.GetComponent<NavMeshAgent>();
		}
		if (this.NavAgent != null)
		{
			this.NavAgent.updateRotation = false;
			this.NavAgent.updatePosition = false;
			if (!this.LegacyNavigation)
			{
				base.transform.gameObject.GetComponent<BaseNavigator>().Init(this, this.NavAgent);
			}
		}
		this.IsStuck = false;
		this.AgencyUpdateRequired = false;
		this.IsOnOffmeshLinkAndReachedNewCoord = false;
		base.InvokeRandomized(new Action(this.TickAi), 0.1f, 0.1f, 0.0050000004f);
		this.Sleep = UnityEngine.Random.Range(0.5f, 1f);
		this.Stamina.Level = UnityEngine.Random.Range(0.1f, 1f);
		this.Energy.Level = UnityEngine.Random.Range(0.5f, 1f);
		this.Hydration.Level = UnityEngine.Random.Range(0.5f, 1f);
		if (this.NewAI)
		{
			this.InitFacts();
			this.fleeHealthThresholdPercentage = this.Stats.HealthThresholdForFleeing;
		}
	}

	// Token: 0x0600048A RID: 1162 RVA: 0x000345B7 File Offset: 0x000327B7
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x000345BF File Offset: 0x000327BF
	public override void Hurt(HitInfo info)
	{
		base.Hurt(info);
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x000345C8 File Offset: 0x000327C8
	public override void OnKilled(HitInfo hitInfo = null)
	{
		Assert.IsTrue(base.isServer, "OnKilled called on client!");
		BaseCorpse baseCorpse = base.DropCorpse(this.CorpsePrefab.resourcePath);
		if (baseCorpse)
		{
			baseCorpse.Spawn();
			baseCorpse.TakeChildren(this);
		}
		base.Invoke(new Action(base.KillMessage), 0.5f);
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x000059DD File Offset: 0x00003BDD
	public override void OnSensation(Sensation sensation)
	{
	}

	// Token: 0x0600048E RID: 1166 RVA: 0x00034624 File Offset: 0x00032824
	protected virtual void OnSenseGunshot(Sensation sensation)
	{
		this._lastHeardGunshotTime = UnityEngine.Time.time;
		this.LastHeardGunshotDirection = (sensation.Position - base.transform.localPosition).normalized;
		if (this.CurrentBehaviour != BaseNpc.Behaviour.Attack)
		{
			this.CurrentBehaviour = BaseNpc.Behaviour.Flee;
		}
	}

	// Token: 0x17000054 RID: 84
	// (get) Token: 0x0600048F RID: 1167 RVA: 0x00034670 File Offset: 0x00032870
	public float SecondsSinceLastHeardGunshot
	{
		get
		{
			return UnityEngine.Time.time - this._lastHeardGunshotTime;
		}
	}

	// Token: 0x17000055 RID: 85
	// (get) Token: 0x06000490 RID: 1168 RVA: 0x0003467E File Offset: 0x0003287E
	// (set) Token: 0x06000491 RID: 1169 RVA: 0x00034686 File Offset: 0x00032886
	public Vector3 LastHeardGunshotDirection { get; set; }

	// Token: 0x17000056 RID: 86
	// (get) Token: 0x06000492 RID: 1170 RVA: 0x0003468F File Offset: 0x0003288F
	// (set) Token: 0x06000493 RID: 1171 RVA: 0x00034697 File Offset: 0x00032897
	public float TargetSpeed { get; set; }

	// Token: 0x17000057 RID: 87
	// (get) Token: 0x06000494 RID: 1172 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool IsNpc
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000058 RID: 88
	// (get) Token: 0x06000495 RID: 1173 RVA: 0x000346A0 File Offset: 0x000328A0
	// (set) Token: 0x06000496 RID: 1174 RVA: 0x000346A8 File Offset: 0x000328A8
	public bool IsDormant
	{
		get
		{
			return this._isDormant;
		}
		set
		{
			this._isDormant = value;
			if (this._isDormant)
			{
				this.StopMoving();
				this.Pause();
				return;
			}
			if (this.GetNavAgent == null || AiManager.nav_disable)
			{
				this.IsDormant = true;
				return;
			}
			this.Resume();
		}
	}

	// Token: 0x17000059 RID: 89
	// (get) Token: 0x06000497 RID: 1175 RVA: 0x000346F4 File Offset: 0x000328F4
	public float SecondsSinceLastSetDestination
	{
		get
		{
			return UnityEngine.Time.time - this.lastSetDestinationTime;
		}
	}

	// Token: 0x1700005A RID: 90
	// (get) Token: 0x06000498 RID: 1176 RVA: 0x00034702 File Offset: 0x00032902
	public float LastSetDestinationTime
	{
		get
		{
			return this.lastSetDestinationTime;
		}
	}

	// Token: 0x1700005B RID: 91
	// (get) Token: 0x06000499 RID: 1177 RVA: 0x0003470A File Offset: 0x0003290A
	// (set) Token: 0x0600049A RID: 1178 RVA: 0x0003472B File Offset: 0x0003292B
	public Vector3 Destination
	{
		get
		{
			if (this.IsNavRunning())
			{
				return this.GetNavAgent.destination;
			}
			return this.Entity.ServerPosition;
		}
		set
		{
			if (this.IsNavRunning())
			{
				this.GetNavAgent.destination = value;
				this.lastSetDestinationTime = UnityEngine.Time.time;
			}
		}
	}

	// Token: 0x1700005C RID: 92
	// (get) Token: 0x0600049B RID: 1179 RVA: 0x0003474C File Offset: 0x0003294C
	// (set) Token: 0x0600049C RID: 1180 RVA: 0x00034763 File Offset: 0x00032963
	public bool IsStopped
	{
		get
		{
			return !this.IsNavRunning() || this.GetNavAgent.isStopped;
		}
		set
		{
			if (this.IsNavRunning())
			{
				if (value)
				{
					this.GetNavAgent.destination = this.ServerPosition;
				}
				this.GetNavAgent.isStopped = value;
			}
		}
	}

	// Token: 0x1700005D RID: 93
	// (get) Token: 0x0600049D RID: 1181 RVA: 0x0003478D File Offset: 0x0003298D
	// (set) Token: 0x0600049E RID: 1182 RVA: 0x000347A4 File Offset: 0x000329A4
	public bool AutoBraking
	{
		get
		{
			return this.IsNavRunning() && this.GetNavAgent.autoBraking;
		}
		set
		{
			if (this.IsNavRunning())
			{
				this.GetNavAgent.autoBraking = value;
			}
		}
	}

	// Token: 0x1700005E RID: 94
	// (get) Token: 0x0600049F RID: 1183 RVA: 0x000347BA File Offset: 0x000329BA
	public bool HasPath
	{
		get
		{
			return this.IsNavRunning() && this.GetNavAgent.hasPath;
		}
	}

	// Token: 0x060004A0 RID: 1184 RVA: 0x000347D1 File Offset: 0x000329D1
	public bool IsNavRunning()
	{
		return this.GetNavAgent != null && this.GetNavAgent.enabled && this.GetNavAgent.isOnNavMesh;
	}

	// Token: 0x060004A1 RID: 1185 RVA: 0x000347FB File Offset: 0x000329FB
	public void Pause()
	{
		if (this.GetNavAgent != null && this.GetNavAgent.enabled)
		{
			this.GetNavAgent.enabled = false;
		}
	}

	// Token: 0x060004A2 RID: 1186 RVA: 0x00034824 File Offset: 0x00032A24
	public void Resume()
	{
		if (!this.GetNavAgent.isOnNavMesh)
		{
			base.StartCoroutine(this.TryForceToNavmesh());
			return;
		}
		this.GetNavAgent.enabled = true;
	}

	// Token: 0x060004A3 RID: 1187 RVA: 0x0003484D File Offset: 0x00032A4D
	private IEnumerator TryForceToNavmesh()
	{
		yield return null;
		int numTries = 0;
		float waitForRetryTime = 1f;
		float maxDistanceMultiplier = 2f;
		if (SingletonComponent<DynamicNavMesh>.Instance != null)
		{
			while (SingletonComponent<DynamicNavMesh>.Instance.IsBuilding)
			{
				yield return CoroutineEx.waitForSecondsRealtime(waitForRetryTime);
				waitForRetryTime += 0.5f;
			}
		}
		waitForRetryTime = 1f;
		while (numTries < 4)
		{
			if (this.GetNavAgent.isOnNavMesh)
			{
				this.GetNavAgent.enabled = true;
				yield break;
			}
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(this.ServerPosition, out navMeshHit, this.GetNavAgent.height * maxDistanceMultiplier, this.GetNavAgent.areaMask))
			{
				this.ServerPosition = navMeshHit.position;
				this.GetNavAgent.Warp(this.ServerPosition);
				this.GetNavAgent.enabled = true;
				yield break;
			}
			yield return CoroutineEx.waitForSecondsRealtime(waitForRetryTime);
			maxDistanceMultiplier *= 1.5f;
			waitForRetryTime *= 1.5f;
			int num = numTries;
			numTries = num + 1;
		}
		Debug.LogWarningFormat("Failed to spawn {0} on a valid navmesh.", new object[]
		{
			base.name
		});
		base.DieInstantly();
		yield break;
	}

	// Token: 0x1700005F RID: 95
	// (get) Token: 0x060004A4 RID: 1188 RVA: 0x0003485C File Offset: 0x00032A5C
	// (set) Token: 0x060004A5 RID: 1189 RVA: 0x00034864 File Offset: 0x00032A64
	public global::BaseEntity AttackTarget { get; set; }

	// Token: 0x17000060 RID: 96
	// (get) Token: 0x060004A6 RID: 1190 RVA: 0x0003486D File Offset: 0x00032A6D
	// (set) Token: 0x060004A7 RID: 1191 RVA: 0x00034875 File Offset: 0x00032A75
	public Memory.SeenInfo AttackTargetMemory { get; set; }

	// Token: 0x17000061 RID: 97
	// (get) Token: 0x060004A8 RID: 1192 RVA: 0x0003487E File Offset: 0x00032A7E
	// (set) Token: 0x060004A9 RID: 1193 RVA: 0x00034886 File Offset: 0x00032A86
	public global::BaseEntity FoodTarget { get; set; }

	// Token: 0x17000062 RID: 98
	// (get) Token: 0x060004AA RID: 1194 RVA: 0x0003488F File Offset: 0x00032A8F
	public BaseCombatEntity CombatTarget
	{
		get
		{
			return this.AttackTarget as BaseCombatEntity;
		}
	}

	// Token: 0x17000063 RID: 99
	// (get) Token: 0x060004AB RID: 1195 RVA: 0x0003489C File Offset: 0x00032A9C
	// (set) Token: 0x060004AC RID: 1196 RVA: 0x000348A4 File Offset: 0x00032AA4
	public Vector3 SpawnPosition { get; set; }

	// Token: 0x17000064 RID: 100
	// (get) Token: 0x060004AD RID: 1197 RVA: 0x00026FFC File Offset: 0x000251FC
	public float AttackTargetVisibleFor
	{
		get
		{
			return 0f;
		}
	}

	// Token: 0x17000065 RID: 101
	// (get) Token: 0x060004AE RID: 1198 RVA: 0x00026FFC File Offset: 0x000251FC
	public float TimeAtDestination
	{
		get
		{
			return 0f;
		}
	}

	// Token: 0x17000066 RID: 102
	// (get) Token: 0x060004AF RID: 1199 RVA: 0x00002E37 File Offset: 0x00001037
	public BaseCombatEntity Entity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x17000067 RID: 103
	// (get) Token: 0x060004B0 RID: 1200 RVA: 0x000348B0 File Offset: 0x00032AB0
	public NavMeshAgent GetNavAgent
	{
		get
		{
			if (base.isClient)
			{
				return null;
			}
			if (this.NavAgent == null)
			{
				this.NavAgent = base.GetComponent<NavMeshAgent>();
				if (this.NavAgent == null)
				{
					Debug.LogErrorFormat("{0} has no nav agent!", new object[]
					{
						base.name
					});
				}
			}
			return this.NavAgent;
		}
	}

	// Token: 0x060004B1 RID: 1201 RVA: 0x0003490E File Offset: 0x00032B0E
	public float GetWantsToAttack(global::BaseEntity target)
	{
		return this.WantsToAttack(target);
	}

	// Token: 0x17000068 RID: 104
	// (get) Token: 0x060004B2 RID: 1202 RVA: 0x00034917 File Offset: 0x00032B17
	public BaseNpc.AiStatistics GetStats
	{
		get
		{
			return this.Stats;
		}
	}

	// Token: 0x17000069 RID: 105
	// (get) Token: 0x060004B3 RID: 1203 RVA: 0x0003491F File Offset: 0x00032B1F
	public float GetAttackRange
	{
		get
		{
			return this.AttackRange;
		}
	}

	// Token: 0x1700006A RID: 106
	// (get) Token: 0x060004B4 RID: 1204 RVA: 0x00034927 File Offset: 0x00032B27
	public Vector3 GetAttackOffset
	{
		get
		{
			return this.AttackOffset;
		}
	}

	// Token: 0x1700006B RID: 107
	// (get) Token: 0x060004B5 RID: 1205 RVA: 0x0003492F File Offset: 0x00032B2F
	public float GetStamina
	{
		get
		{
			return this.Stamina.Level;
		}
	}

	// Token: 0x1700006C RID: 108
	// (get) Token: 0x060004B6 RID: 1206 RVA: 0x0003493C File Offset: 0x00032B3C
	public float GetEnergy
	{
		get
		{
			return this.Energy.Level;
		}
	}

	// Token: 0x1700006D RID: 109
	// (get) Token: 0x060004B7 RID: 1207 RVA: 0x00034949 File Offset: 0x00032B49
	public float GetAttackCost
	{
		get
		{
			return this.AttackCost;
		}
	}

	// Token: 0x1700006E RID: 110
	// (get) Token: 0x060004B8 RID: 1208 RVA: 0x00034951 File Offset: 0x00032B51
	public float GetSleep
	{
		get
		{
			return this.Sleep;
		}
	}

	// Token: 0x1700006F RID: 111
	// (get) Token: 0x060004B9 RID: 1209 RVA: 0x00034959 File Offset: 0x00032B59
	public Vector3 CurrentAimAngles
	{
		get
		{
			return base.transform.forward;
		}
	}

	// Token: 0x17000070 RID: 112
	// (get) Token: 0x060004BA RID: 1210 RVA: 0x00034966 File Offset: 0x00032B66
	public float GetStuckDuration
	{
		get
		{
			return this.stuckDuration;
		}
	}

	// Token: 0x17000071 RID: 113
	// (get) Token: 0x060004BB RID: 1211 RVA: 0x0003496E File Offset: 0x00032B6E
	public float GetLastStuckTime
	{
		get
		{
			return this.lastStuckTime;
		}
	}

	// Token: 0x060004BC RID: 1212 RVA: 0x00034976 File Offset: 0x00032B76
	public bool BusyTimerActive()
	{
		return this.BusyTimer.IsActive;
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x00034983 File Offset: 0x00032B83
	public void SetBusyFor(float dur)
	{
		this.BusyTimer.Activate(dur, null);
	}

	// Token: 0x17000072 RID: 114
	// (get) Token: 0x060004BE RID: 1214 RVA: 0x00034992 File Offset: 0x00032B92
	public Vector3 AttackPosition
	{
		get
		{
			return this.ServerPosition + base.transform.TransformDirection(this.AttackOffset);
		}
	}

	// Token: 0x17000073 RID: 115
	// (get) Token: 0x060004BF RID: 1215 RVA: 0x000349B0 File Offset: 0x00032BB0
	public Vector3 CrouchedAttackPosition
	{
		get
		{
			return this.AttackPosition;
		}
	}

	// Token: 0x060004C0 RID: 1216 RVA: 0x000349B8 File Offset: 0x00032BB8
	internal float WantsToAttack(global::BaseEntity target)
	{
		if (target == null)
		{
			return 0f;
		}
		if (this.CurrentBehaviour == BaseNpc.Behaviour.Sleep)
		{
			return 0f;
		}
		if (!target.HasAnyTrait(global::BaseEntity.TraitFlag.Animal | global::BaseEntity.TraitFlag.Human))
		{
			return 0f;
		}
		if (target.GetType() == base.GetType())
		{
			return 1f - this.Stats.Tolerance;
		}
		return 1f;
	}

	// Token: 0x17000074 RID: 116
	// (get) Token: 0x060004C1 RID: 1217 RVA: 0x00026FFC File Offset: 0x000251FC
	public float currentBehaviorDuration
	{
		get
		{
			return 0f;
		}
	}

	// Token: 0x17000075 RID: 117
	// (get) Token: 0x060004C2 RID: 1218 RVA: 0x00034A1C File Offset: 0x00032C1C
	// (set) Token: 0x060004C3 RID: 1219 RVA: 0x00034A24 File Offset: 0x00032C24
	public BaseNpc.Behaviour CurrentBehaviour { get; set; }

	// Token: 0x060004C4 RID: 1220 RVA: 0x00034A2D File Offset: 0x00032C2D
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.baseNPC = Facepunch.Pool.Get<BaseNPC>();
		info.msg.baseNPC.flags = (int)this.aiFlags;
	}

	// Token: 0x060004C5 RID: 1221 RVA: 0x00034A5C File Offset: 0x00032C5C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseNPC != null)
		{
			this.aiFlags = (BaseNpc.AiFlags)info.msg.baseNPC.flags;
		}
	}

	// Token: 0x060004C6 RID: 1222 RVA: 0x00034A88 File Offset: 0x00032C88
	public override float MaxVelocity()
	{
		return this.Stats.Speed;
	}

	// Token: 0x04000335 RID: 821
	[NonSerialized]
	public Transform ChaseTransform;

	// Token: 0x04000336 RID: 822
	public int agentTypeIndex;

	// Token: 0x04000337 RID: 823
	public bool NewAI;

	// Token: 0x04000338 RID: 824
	public bool LegacyNavigation = true;

	// Token: 0x0400033A RID: 826
	private Vector3 stepDirection;

	// Token: 0x0400033D RID: 829
	private float maxFleeTime;

	// Token: 0x0400033E RID: 830
	private float fleeHealthThresholdPercentage = 1f;

	// Token: 0x0400033F RID: 831
	private float blockEnemyTargetingTimeout = float.NegativeInfinity;

	// Token: 0x04000340 RID: 832
	private float blockFoodTargetingTimeout = float.NegativeInfinity;

	// Token: 0x04000341 RID: 833
	private float aggroTimeout = float.NegativeInfinity;

	// Token: 0x04000342 RID: 834
	private float lastAggroChanceResult;

	// Token: 0x04000343 RID: 835
	private float lastAggroChanceCalcTime;

	// Token: 0x04000344 RID: 836
	private const float aggroChanceRecalcTimeout = 5f;

	// Token: 0x04000345 RID: 837
	private float eatTimeout = float.NegativeInfinity;

	// Token: 0x04000346 RID: 838
	private float wakeUpBlockMoveTimeout = float.NegativeInfinity;

	// Token: 0x04000347 RID: 839
	private global::BaseEntity blockTargetingThisEnemy;

	// Token: 0x04000348 RID: 840
	[NonSerialized]
	public float waterDepth;

	// Token: 0x04000349 RID: 841
	[NonSerialized]
	public bool swimming;

	// Token: 0x0400034A RID: 842
	[NonSerialized]
	public bool wasSwimming;

	// Token: 0x0400034B RID: 843
	private static readonly AnimationCurve speedFractionResponse = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x0400034C RID: 844
	private bool _traversingNavMeshLink;

	// Token: 0x0400034D RID: 845
	private OffMeshLinkData _currentNavMeshLink;

	// Token: 0x0400034E RID: 846
	private string _currentNavMeshLinkName;

	// Token: 0x0400034F RID: 847
	private float _currentNavMeshLinkTraversalTime;

	// Token: 0x04000350 RID: 848
	private float _currentNavMeshLinkTraversalTimeDelta;

	// Token: 0x04000351 RID: 849
	private Quaternion _currentNavMeshLinkOrientation;

	// Token: 0x04000352 RID: 850
	private Vector3 _currentNavMeshLinkEndPos;

	// Token: 0x04000353 RID: 851
	private float nextAttackTime;

	// Token: 0x04000354 RID: 852
	[SerializeField]
	[global::InspectorFlags]
	public TerrainTopology.Enum topologyPreference = (TerrainTopology.Enum)96;

	// Token: 0x04000355 RID: 853
	[global::InspectorFlags]
	public BaseNpc.AiFlags aiFlags;

	// Token: 0x04000356 RID: 854
	[NonSerialized]
	public byte[] CurrentFacts = new byte[Enum.GetValues(typeof(BaseNpc.Facts)).Length];

	// Token: 0x04000357 RID: 855
	[Header("NPC Senses")]
	public int ForgetUnseenEntityTime = 10;

	// Token: 0x04000358 RID: 856
	public float SensesTickRate = 0.5f;

	// Token: 0x04000359 RID: 857
	[NonSerialized]
	public global::BaseEntity[] SensesResults = new global::BaseEntity[64];

	// Token: 0x0400035A RID: 858
	private float lastTickTime;

	// Token: 0x0400035B RID: 859
	private float playerTargetDecisionStartTime;

	// Token: 0x0400035C RID: 860
	private float animalTargetDecisionStartTime;

	// Token: 0x0400035D RID: 861
	private bool isAlreadyCheckingPathPending;

	// Token: 0x0400035E RID: 862
	private int numPathPendingAttempts;

	// Token: 0x0400035F RID: 863
	private float accumPathPendingDelay;

	// Token: 0x04000360 RID: 864
	public const float TickRate = 0.1f;

	// Token: 0x04000361 RID: 865
	private Vector3 lastStuckPos;

	// Token: 0x04000362 RID: 866
	private float nextFlinchTime;

	// Token: 0x04000363 RID: 867
	private float _lastHeardGunshotTime = float.NegativeInfinity;

	// Token: 0x04000366 RID: 870
	[Header("BaseNpc")]
	public GameObjectRef CorpsePrefab;

	// Token: 0x04000367 RID: 871
	public BaseNpc.AiStatistics Stats;

	// Token: 0x04000368 RID: 872
	public Vector3 AttackOffset;

	// Token: 0x04000369 RID: 873
	public float AttackDamage = 20f;

	// Token: 0x0400036A RID: 874
	public DamageType AttackDamageType = DamageType.Bite;

	// Token: 0x0400036B RID: 875
	[Tooltip("Stamina to use per attack")]
	public float AttackCost = 0.1f;

	// Token: 0x0400036C RID: 876
	[Tooltip("How often can we attack")]
	public float AttackRate = 1f;

	// Token: 0x0400036D RID: 877
	[Tooltip("Maximum Distance for an attack")]
	public float AttackRange = 1f;

	// Token: 0x0400036E RID: 878
	public NavMeshAgent NavAgent;

	// Token: 0x0400036F RID: 879
	public LayerMask movementMask = 429990145;

	// Token: 0x04000370 RID: 880
	public float stuckDuration;

	// Token: 0x04000371 RID: 881
	public float lastStuckTime;

	// Token: 0x04000372 RID: 882
	public float idleDuration;

	// Token: 0x04000373 RID: 883
	private bool _isDormant;

	// Token: 0x04000374 RID: 884
	private float lastSetDestinationTime;

	// Token: 0x04000379 RID: 889
	[NonSerialized]
	public StateTimer BusyTimer;

	// Token: 0x0400037A RID: 890
	[NonSerialized]
	public float Sleep;

	// Token: 0x0400037B RID: 891
	[NonSerialized]
	public VitalLevel Stamina;

	// Token: 0x0400037C RID: 892
	[NonSerialized]
	public VitalLevel Energy;

	// Token: 0x0400037D RID: 893
	[NonSerialized]
	public VitalLevel Hydration;

	// Token: 0x02000B4D RID: 2893
	[Flags]
	public enum AiFlags
	{
		// Token: 0x04003D6C RID: 15724
		Sitting = 2,
		// Token: 0x04003D6D RID: 15725
		Chasing = 4,
		// Token: 0x04003D6E RID: 15726
		Sleeping = 8
	}

	// Token: 0x02000B4E RID: 2894
	public enum Facts
	{
		// Token: 0x04003D70 RID: 15728
		HasEnemy,
		// Token: 0x04003D71 RID: 15729
		EnemyRange,
		// Token: 0x04003D72 RID: 15730
		CanTargetEnemies,
		// Token: 0x04003D73 RID: 15731
		Health,
		// Token: 0x04003D74 RID: 15732
		Speed,
		// Token: 0x04003D75 RID: 15733
		IsTired,
		// Token: 0x04003D76 RID: 15734
		IsSleeping,
		// Token: 0x04003D77 RID: 15735
		IsAttackReady,
		// Token: 0x04003D78 RID: 15736
		IsRoamReady,
		// Token: 0x04003D79 RID: 15737
		IsAggro,
		// Token: 0x04003D7A RID: 15738
		WantsToFlee,
		// Token: 0x04003D7B RID: 15739
		IsHungry,
		// Token: 0x04003D7C RID: 15740
		FoodRange,
		// Token: 0x04003D7D RID: 15741
		AttackedLately,
		// Token: 0x04003D7E RID: 15742
		LoudNoiseNearby,
		// Token: 0x04003D7F RID: 15743
		CanTargetFood,
		// Token: 0x04003D80 RID: 15744
		IsMoving,
		// Token: 0x04003D81 RID: 15745
		IsFleeing,
		// Token: 0x04003D82 RID: 15746
		IsEating,
		// Token: 0x04003D83 RID: 15747
		IsAfraid,
		// Token: 0x04003D84 RID: 15748
		AfraidRange,
		// Token: 0x04003D85 RID: 15749
		IsUnderHealthThreshold,
		// Token: 0x04003D86 RID: 15750
		CanNotMove,
		// Token: 0x04003D87 RID: 15751
		PathToTargetStatus
	}

	// Token: 0x02000B4F RID: 2895
	public enum EnemyRangeEnum : byte
	{
		// Token: 0x04003D89 RID: 15753
		AttackRange,
		// Token: 0x04003D8A RID: 15754
		AggroRange,
		// Token: 0x04003D8B RID: 15755
		AwareRange,
		// Token: 0x04003D8C RID: 15756
		OutOfRange
	}

	// Token: 0x02000B50 RID: 2896
	public enum FoodRangeEnum : byte
	{
		// Token: 0x04003D8E RID: 15758
		EatRange,
		// Token: 0x04003D8F RID: 15759
		AwareRange,
		// Token: 0x04003D90 RID: 15760
		OutOfRange
	}

	// Token: 0x02000B51 RID: 2897
	public enum AfraidRangeEnum : byte
	{
		// Token: 0x04003D92 RID: 15762
		InAfraidRange,
		// Token: 0x04003D93 RID: 15763
		OutOfRange
	}

	// Token: 0x02000B52 RID: 2898
	public enum HealthEnum : byte
	{
		// Token: 0x04003D95 RID: 15765
		Fine,
		// Token: 0x04003D96 RID: 15766
		Medium,
		// Token: 0x04003D97 RID: 15767
		Low
	}

	// Token: 0x02000B53 RID: 2899
	public enum SpeedEnum : byte
	{
		// Token: 0x04003D99 RID: 15769
		StandStill,
		// Token: 0x04003D9A RID: 15770
		Walk,
		// Token: 0x04003D9B RID: 15771
		Run
	}

	// Token: 0x02000B54 RID: 2900
	[Serializable]
	public struct AiStatistics
	{
		// Token: 0x04003D9C RID: 15772
		[Tooltip("Ai will be less likely to fight animals that are larger than them, and more likely to flee from them.")]
		[Range(0f, 1f)]
		public float Size;

		// Token: 0x04003D9D RID: 15773
		[Tooltip("How fast we can move")]
		public float Speed;

		// Token: 0x04003D9E RID: 15774
		[Tooltip("How fast can we accelerate")]
		public float Acceleration;

		// Token: 0x04003D9F RID: 15775
		[Tooltip("How fast can we turn around")]
		public float TurnSpeed;

		// Token: 0x04003DA0 RID: 15776
		[Tooltip("Determines things like how near we'll allow other species to get")]
		[Range(0f, 1f)]
		public float Tolerance;

		// Token: 0x04003DA1 RID: 15777
		[Tooltip("How far this NPC can see")]
		public float VisionRange;

		// Token: 0x04003DA2 RID: 15778
		[Tooltip("Our vision cone for dot product - a value of -1 means we can see all around us, 0 = only infront ")]
		public float VisionCone;

		// Token: 0x04003DA3 RID: 15779
		[Tooltip("NPCs use distance visibility to basically make closer enemies easier to detect than enemies further away")]
		public AnimationCurve DistanceVisibility;

		// Token: 0x04003DA4 RID: 15780
		[Tooltip("How likely are we to be offensive without being threatened")]
		public float Hostility;

		// Token: 0x04003DA5 RID: 15781
		[Tooltip("How likely are we to defend ourselves when attacked")]
		public float Defensiveness;

		// Token: 0x04003DA6 RID: 15782
		[Tooltip("The range at which we will engage targets")]
		public float AggressionRange;

		// Token: 0x04003DA7 RID: 15783
		[Tooltip("The range at which an aggrified npc will disengage it's current target")]
		public float DeaggroRange;

		// Token: 0x04003DA8 RID: 15784
		[Tooltip("For how long will we chase a target until we give up")]
		public float DeaggroChaseTime;

		// Token: 0x04003DA9 RID: 15785
		[Tooltip("When we deaggro, how long do we wait until we can aggro again.")]
		public float DeaggroCooldown;

		// Token: 0x04003DAA RID: 15786
		[Tooltip("The threshold of our health fraction where there's a chance that we want to flee")]
		public float HealthThresholdForFleeing;

		// Token: 0x04003DAB RID: 15787
		[Tooltip("The chance that we will flee when our health threshold is triggered")]
		public float HealthThresholdFleeChance;

		// Token: 0x04003DAC RID: 15788
		[Tooltip("When we flee, what is the minimum distance we should flee?")]
		public float MinFleeRange;

		// Token: 0x04003DAD RID: 15789
		[Tooltip("When we flee, what is the maximum distance we should flee?")]
		public float MaxFleeRange;

		// Token: 0x04003DAE RID: 15790
		[Tooltip("When we flee, what is the maximum time that can pass until we stop?")]
		public float MaxFleeTime;

		// Token: 0x04003DAF RID: 15791
		[Tooltip("At what range we are afraid of a target that is in our Is Afraid Of list.")]
		public float AfraidRange;

		// Token: 0x04003DB0 RID: 15792
		[Tooltip("The family this npc belong to. Npcs in the same family will not attack each other.")]
		public BaseNpc.AiStatistics.FamilyEnum Family;

		// Token: 0x04003DB1 RID: 15793
		[Tooltip("List of the types of Npc that we are afraid of.")]
		public BaseNpc.AiStatistics.FamilyEnum[] IsAfraidOf;

		// Token: 0x04003DB2 RID: 15794
		[Tooltip("The minimum distance this npc will wander when idle.")]
		public float MinRoamRange;

		// Token: 0x04003DB3 RID: 15795
		[Tooltip("The maximum distance this npc will wander when idle.")]
		public float MaxRoamRange;

		// Token: 0x04003DB4 RID: 15796
		[Tooltip("The minimum amount of time between each time we seek a new roam destination (when idle)")]
		public float MinRoamDelay;

		// Token: 0x04003DB5 RID: 15797
		[Tooltip("The maximum amount of time between each time we seek a new roam destination (when idle)")]
		public float MaxRoamDelay;

		// Token: 0x04003DB6 RID: 15798
		[Tooltip("If an npc is mobile, they are allowed to move when idle.")]
		public bool IsMobile;

		// Token: 0x04003DB7 RID: 15799
		[Tooltip("In the range between min and max roam delay, we evaluate the random value through this curve")]
		public AnimationCurve RoamDelayDistribution;

		// Token: 0x04003DB8 RID: 15800
		[Tooltip("For how long do we remember that someone attacked us")]
		public float AttackedMemoryTime;

		// Token: 0x04003DB9 RID: 15801
		[Tooltip("How long should we block movement to make the wakeup animation not look whack?")]
		public float WakeupBlockMoveTime;

		// Token: 0x04003DBA RID: 15802
		[Tooltip("The maximum water depth this npc willingly will walk into.")]
		public float MaxWaterDepth;

		// Token: 0x04003DBB RID: 15803
		[Tooltip("The water depth at which they will start swimming.")]
		public float WaterLevelNeck;

		// Token: 0x04003DBC RID: 15804
		public float WaterLevelNeckOffset;

		// Token: 0x04003DBD RID: 15805
		[Tooltip("The range we consider using close range weapons.")]
		public float CloseRange;

		// Token: 0x04003DBE RID: 15806
		[Tooltip("The range we consider using medium range weapons.")]
		public float MediumRange;

		// Token: 0x04003DBF RID: 15807
		[Tooltip("The range we consider using long range weapons.")]
		public float LongRange;

		// Token: 0x04003DC0 RID: 15808
		[Tooltip("How long can we be out of range of our spawn point before we time out and make our way back home (when idle).")]
		public float OutOfRangeOfSpawnPointTimeout;

		// Token: 0x04003DC1 RID: 15809
		[Tooltip("If this is set to true, then a target must hold special markers (like IsHostile) for the target to be considered for aggressive action.")]
		public bool OnlyAggroMarkedTargets;

		// Token: 0x02000F5B RID: 3931
		public enum FamilyEnum
		{
			// Token: 0x04004DF3 RID: 19955
			Bear,
			// Token: 0x04004DF4 RID: 19956
			Wolf,
			// Token: 0x04004DF5 RID: 19957
			Deer,
			// Token: 0x04004DF6 RID: 19958
			Boar,
			// Token: 0x04004DF7 RID: 19959
			Chicken,
			// Token: 0x04004DF8 RID: 19960
			Horse,
			// Token: 0x04004DF9 RID: 19961
			Zombie,
			// Token: 0x04004DFA RID: 19962
			Scientist,
			// Token: 0x04004DFB RID: 19963
			Murderer,
			// Token: 0x04004DFC RID: 19964
			Player
		}
	}

	// Token: 0x02000B55 RID: 2901
	public enum Behaviour
	{
		// Token: 0x04003DC3 RID: 15811
		Idle,
		// Token: 0x04003DC4 RID: 15812
		Wander,
		// Token: 0x04003DC5 RID: 15813
		Attack,
		// Token: 0x04003DC6 RID: 15814
		Flee,
		// Token: 0x04003DC7 RID: 15815
		Eat,
		// Token: 0x04003DC8 RID: 15816
		Sleep,
		// Token: 0x04003DC9 RID: 15817
		RetreatingToCover
	}
}
