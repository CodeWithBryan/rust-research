using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x020001D3 RID: 467
public class SimpleShark : BaseCombatEntity
{
	// Token: 0x170001DC RID: 476
	// (get) Token: 0x0600188E RID: 6286 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool IsNpc
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600188F RID: 6287 RVA: 0x000B4328 File Offset: 0x000B2528
	private void GenerateIdlePoints(Vector3 center, float radius, float heightOffset, float staggerOffset = 0f)
	{
		this.patrolPath.Clear();
		float num = 0f;
		int num2 = 32;
		int layerMask = 10551553;
		float height = TerrainMeta.WaterMap.GetHeight(center);
		float height2 = TerrainMeta.HeightMap.GetHeight(center);
		for (int i = 0; i < num2; i++)
		{
			num += 360f / (float)num2;
			float radius2 = 1f;
			Vector3 vector = BasePathFinder.GetPointOnCircle(center, radius2, num);
			Vector3 vector2 = Vector3Ex.Direction(vector, center);
			RaycastHit raycastHit;
			if (Physics.SphereCast(center, this.obstacleDetectionRadius, vector2, out raycastHit, radius + staggerOffset, layerMask))
			{
				vector = center + vector2 * (raycastHit.distance - 6f);
			}
			else
			{
				vector = center + vector2 * radius;
			}
			if (staggerOffset != 0f)
			{
				vector += vector2 * UnityEngine.Random.Range(-staggerOffset, staggerOffset);
			}
			vector.y += UnityEngine.Random.Range(-heightOffset, heightOffset);
			vector.y = Mathf.Clamp(vector.y, height2 + 3f, height - 3f);
			this.patrolPath.Add(vector);
		}
	}

	// Token: 0x06001890 RID: 6288 RVA: 0x000B4450 File Offset: 0x000B2650
	private void GenerateIdlePoints_Shrinkwrap(Vector3 center, float radius, float heightOffset, float staggerOffset = 0f)
	{
		this.patrolPath.Clear();
		float num = 0f;
		int num2 = 32;
		int layerMask = 10551553;
		float height = TerrainMeta.WaterMap.GetHeight(center);
		float height2 = TerrainMeta.HeightMap.GetHeight(center);
		for (int i = 0; i < num2; i++)
		{
			num += 360f / (float)num2;
			float radius2 = radius * 2f;
			Vector3 vector = BasePathFinder.GetPointOnCircle(center, radius2, num);
			Vector3 vector2 = Vector3Ex.Direction(center, vector);
			RaycastHit raycastHit;
			if (Physics.SphereCast(vector, this.obstacleDetectionRadius, vector2, out raycastHit, radius + staggerOffset, layerMask))
			{
				vector = raycastHit.point - vector2 * 6f;
			}
			else
			{
				vector += vector2 * radius;
			}
			if (staggerOffset != 0f)
			{
				vector += vector2 * UnityEngine.Random.Range(-staggerOffset, staggerOffset);
			}
			vector.y += UnityEngine.Random.Range(-heightOffset, heightOffset);
			vector.y = Mathf.Clamp(vector.y, height2 + 3f, height - 3f);
			this.patrolPath.Add(vector);
		}
	}

	// Token: 0x06001891 RID: 6289 RVA: 0x000B457C File Offset: 0x000B277C
	public override void ServerInit()
	{
		base.ServerInit();
		if (SimpleShark.disable)
		{
			base.Invoke(new Action(base.KillMessage), 0.01f);
			return;
		}
		base.transform.position = this.WaterClamp(base.transform.position);
		this.Init();
		base.InvokeRandomized(new Action(this.CheckSleepState), 0f, 1f, 0.5f);
	}

	// Token: 0x06001892 RID: 6290 RVA: 0x000B45F4 File Offset: 0x000B27F4
	public void CheckSleepState()
	{
		bool flag = BaseNetworkable.HasCloseConnections(base.transform.position, 100f);
		this.sleeping = !flag;
	}

	// Token: 0x06001893 RID: 6291 RVA: 0x000B4624 File Offset: 0x000B2824
	public void Init()
	{
		this.GenerateIdlePoints_Shrinkwrap(base.transform.position, 20f, 2f, 3f);
		this.states = new SimpleShark.SimpleState[2];
		this.states[0] = new SimpleShark.IdleState(this);
		this.states[1] = new SimpleShark.AttackState(this);
		base.transform.position = this.patrolPath[0];
	}

	// Token: 0x06001894 RID: 6292 RVA: 0x000B4690 File Offset: 0x000B2890
	private void Think(float delta)
	{
		if (this.states == null)
		{
			return;
		}
		if (SimpleShark.disable)
		{
			if (!base.IsInvoking(new Action(base.KillMessage)))
			{
				base.Invoke(new Action(base.KillMessage), 0.01f);
			}
			return;
		}
		if (this.sleeping)
		{
			return;
		}
		SimpleShark.SimpleState simpleState = null;
		float num = -1f;
		foreach (SimpleShark.SimpleState simpleState2 in this.states)
		{
			float num2 = simpleState2.State_Weight();
			if (num2 > num)
			{
				simpleState = simpleState2;
				num = num2;
			}
		}
		if (simpleState != this._currentState && (this._currentState == null || this._currentState.CanInterrupt()))
		{
			if (this._currentState != null)
			{
				this._currentState.State_Exit();
			}
			simpleState.State_Enter();
			this._currentState = simpleState;
		}
		this.UpdateTarget(delta);
		this._currentState.State_Think(delta);
		this.UpdateObstacleAvoidance(delta);
		this.UpdateDirection(delta);
		this.UpdateSpeed(delta);
		this.UpdatePosition(delta);
		base.SetFlag(BaseEntity.Flags.Open, this.HasTarget() && this.CanAttack(), false, true);
	}

	// Token: 0x06001895 RID: 6293 RVA: 0x000B47A0 File Offset: 0x000B29A0
	public Vector3 WaterClamp(Vector3 point)
	{
		float height = WaterSystem.GetHeight(point);
		float min = TerrainMeta.HeightMap.GetHeight(point) + this.minFloorDist;
		float max = height - this.minSurfaceDist;
		if (SimpleShark.forceSurfaceAmount != 0f)
		{
			max = (min = WaterSystem.GetHeight(point) + SimpleShark.forceSurfaceAmount);
		}
		point.y = Mathf.Clamp(point.y, min, max);
		return point;
	}

	// Token: 0x06001896 RID: 6294 RVA: 0x000B4800 File Offset: 0x000B2A00
	public bool ValidTarget(BasePlayer newTarget)
	{
		float maxDistance = Vector3.Distance(newTarget.eyes.position, base.transform.position);
		Vector3 direction = Vector3Ex.Direction(newTarget.eyes.position, base.transform.position);
		int layerMask = 10551552;
		if (Physics.Raycast(base.transform.position, direction, maxDistance, layerMask))
		{
			return false;
		}
		if (newTarget.isMounted)
		{
			if (newTarget.GetMountedVehicle())
			{
				return false;
			}
			if (!newTarget.GetMounted().GetComponent<WaterInflatable>().buoyancy.enabled)
			{
				return false;
			}
		}
		else if (!WaterLevel.Test(newTarget.CenterPoint(), true, newTarget))
		{
			return false;
		}
		return true;
	}

	// Token: 0x06001897 RID: 6295 RVA: 0x000B48A4 File Offset: 0x000B2AA4
	public void ClearTarget()
	{
		this.target = null;
		this.lastSeenTargetTime = 0f;
	}

	// Token: 0x06001898 RID: 6296 RVA: 0x000B48B8 File Offset: 0x000B2AB8
	public override void OnKilled(HitInfo hitInfo = null)
	{
		if (base.isServer)
		{
			if (GameInfo.HasAchievements && hitInfo != null && hitInfo.InitiatorPlayer != null && !hitInfo.InitiatorPlayer.IsNpc && hitInfo.Weapon != null && hitInfo.Weapon.ShortPrefabName.Contains("speargun"))
			{
				hitInfo.InitiatorPlayer.stats.Add("shark_speargun_kills", 1, Stats.All);
				hitInfo.InitiatorPlayer.stats.Save(true);
			}
			BaseCorpse baseCorpse = base.DropCorpse(this.corpsePrefab.resourcePath);
			if (baseCorpse)
			{
				baseCorpse.Spawn();
				baseCorpse.TakeChildren(this);
			}
			base.Invoke(new Action(base.KillMessage), 0.5f);
		}
		base.OnKilled(hitInfo);
	}

	// Token: 0x06001899 RID: 6297 RVA: 0x000B4988 File Offset: 0x000B2B88
	public void UpdateTarget(float delta)
	{
		if (this.target != null)
		{
			bool flag = Vector3.Distance(this.target.eyes.position, base.transform.position) > this.aggroRange * 2f;
			bool flag2 = Time.realtimeSinceStartup > this.lastSeenTargetTime + 4f;
			if (!this.ValidTarget(this.target) || flag || flag2)
			{
				this.ClearTarget();
			}
			else
			{
				this.lastSeenTargetTime = Time.realtimeSinceStartup;
			}
		}
		if (Time.realtimeSinceStartup < this.nextTargetSearchTime)
		{
			return;
		}
		if (this.target == null)
		{
			this.nextTargetSearchTime = Time.realtimeSinceStartup + 1f;
			if (BaseNetworkable.HasCloseConnections(base.transform.position, this.aggroRange))
			{
				int playersInSphere = BaseEntity.Query.Server.GetPlayersInSphere(base.transform.position, this.aggroRange, SimpleShark.playerQueryResults, null);
				for (int i = 0; i < playersInSphere; i++)
				{
					BasePlayer basePlayer = SimpleShark.playerQueryResults[i];
					if (!basePlayer.isClient && this.ValidTarget(basePlayer))
					{
						this.target = basePlayer;
						this.lastSeenTargetTime = Time.realtimeSinceStartup;
						return;
					}
				}
			}
		}
	}

	// Token: 0x0600189A RID: 6298 RVA: 0x000B4AB4 File Offset: 0x000B2CB4
	public float TimeSinceAttacked()
	{
		return Time.realtimeSinceStartup - this.lastTimeAttacked;
	}

	// Token: 0x0600189B RID: 6299 RVA: 0x000B4AC4 File Offset: 0x000B2CC4
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		this.lastTimeAttacked = Time.realtimeSinceStartup;
		if (info.damageTypes.Total() > 20f)
		{
			this.Startle();
		}
		if (info.InitiatorPlayer != null && this.target == null && this.ValidTarget(info.InitiatorPlayer))
		{
			this.target = info.InitiatorPlayer;
			this.lastSeenTargetTime = Time.realtimeSinceStartup;
		}
	}

	// Token: 0x0600189C RID: 6300 RVA: 0x000B4B3C File Offset: 0x000B2D3C
	public bool HasTarget()
	{
		return this.target != null;
	}

	// Token: 0x0600189D RID: 6301 RVA: 0x000B4B4A File Offset: 0x000B2D4A
	public BasePlayer GetTarget()
	{
		return this.target;
	}

	// Token: 0x0600189E RID: 6302 RVA: 0x000B4B52 File Offset: 0x000B2D52
	public override string Categorize()
	{
		return "Shark";
	}

	// Token: 0x0600189F RID: 6303 RVA: 0x000B4B59 File Offset: 0x000B2D59
	public bool CanAttack()
	{
		return Time.realtimeSinceStartup > this.nextAttackTime;
	}

	// Token: 0x060018A0 RID: 6304 RVA: 0x000B4B68 File Offset: 0x000B2D68
	public void DoAttack()
	{
		if (!this.HasTarget())
		{
			return;
		}
		this.GetTarget().Hurt(UnityEngine.Random.Range(30f, 70f), DamageType.Bite, this, true);
		Vector3 posWorld = this.WaterClamp(this.GetTarget().CenterPoint());
		Effect.server.Run(this.bloodCloud.resourcePath, posWorld, Vector3.forward, null, false);
		this.nextAttackTime = Time.realtimeSinceStartup + this.attackCooldown;
	}

	// Token: 0x060018A1 RID: 6305 RVA: 0x000B4BD8 File Offset: 0x000B2DD8
	public void Startle()
	{
		this.lastStartleTime = Time.realtimeSinceStartup;
	}

	// Token: 0x060018A2 RID: 6306 RVA: 0x000B4BE5 File Offset: 0x000B2DE5
	public bool IsStartled()
	{
		return this.lastStartleTime + this.startleDuration > Time.realtimeSinceStartup;
	}

	// Token: 0x060018A3 RID: 6307 RVA: 0x000B4BFB File Offset: 0x000B2DFB
	private float GetDesiredSpeed()
	{
		if (!this.IsStartled())
		{
			return this.minSpeed;
		}
		return this.maxSpeed;
	}

	// Token: 0x060018A4 RID: 6308 RVA: 0x000B4C12 File Offset: 0x000B2E12
	public float GetTurnSpeed()
	{
		if (this.IsStartled())
		{
			return this.maxTurnSpeed;
		}
		if (this.obstacleAvoidanceScale != 0f)
		{
			return Mathf.Lerp(this.minTurnSpeed, this.maxTurnSpeed, this.obstacleAvoidanceScale);
		}
		return this.minTurnSpeed;
	}

	// Token: 0x060018A5 RID: 6309 RVA: 0x000B4C4E File Offset: 0x000B2E4E
	private float GetCurrentSpeed()
	{
		return this.currentSpeed;
	}

	// Token: 0x060018A6 RID: 6310 RVA: 0x000B4C58 File Offset: 0x000B2E58
	private void UpdateObstacleAvoidance(float delta)
	{
		this.timeSinceLastObstacleCheck += delta;
		if (this.timeSinceLastObstacleCheck < 0.5f)
		{
			return;
		}
		Vector3 forward = base.transform.forward;
		Vector3 position = base.transform.position;
		int layerMask = 1503764737;
		RaycastHit raycastHit;
		if (Physics.SphereCast(position, this.obstacleDetectionRadius, forward, out raycastHit, this.obstacleDetectionRange, layerMask))
		{
			Vector3 point = raycastHit.point;
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			RaycastHit raycastHit2;
			if (Physics.SphereCast(position + Vector3.down * 0.25f + base.transform.right * 0.25f, this.obstacleDetectionRadius, forward, out raycastHit2, this.obstacleDetectionRange, layerMask))
			{
				vector = raycastHit2.point;
			}
			RaycastHit raycastHit3;
			if (Physics.SphereCast(position + Vector3.down * 0.25f - base.transform.right * 0.25f, this.obstacleDetectionRadius, forward, out raycastHit3, this.obstacleDetectionRange, layerMask))
			{
				vector2 = raycastHit3.point;
			}
			if (vector != Vector3.zero && vector2 != Vector3.zero)
			{
				Plane plane = new Plane(point, vector, vector2);
				Vector3 normal = plane.normal;
				if (normal != Vector3.zero)
				{
					raycastHit.normal = normal;
				}
			}
			this.cachedObstacleNormal = raycastHit.normal;
			this.cachedObstacleDistance = raycastHit.distance;
			this.obstacleAvoidanceScale = 1f - Mathf.InverseLerp(2f, this.obstacleDetectionRange * 0.75f, raycastHit.distance);
		}
		else
		{
			this.obstacleAvoidanceScale = Mathf.MoveTowards(this.obstacleAvoidanceScale, 0f, this.timeSinceLastObstacleCheck * 2f);
			if (this.obstacleAvoidanceScale == 0f)
			{
				this.cachedObstacleDistance = 0f;
			}
		}
		this.timeSinceLastObstacleCheck = 0f;
	}

	// Token: 0x060018A7 RID: 6311 RVA: 0x000B4E44 File Offset: 0x000B3044
	private void UpdateDirection(float delta)
	{
		Vector3 forward = base.transform.forward;
		Vector3 vector = Vector3Ex.Direction(this.WaterClamp(this.destination), base.transform.position);
		if (this.obstacleAvoidanceScale != 0f)
		{
			Vector3 a;
			if (this.cachedObstacleNormal != Vector3.zero)
			{
				Vector3 lhs = QuaternionEx.LookRotationForcedUp(this.cachedObstacleNormal, Vector3.up) * Vector3.forward;
				if (Vector3.Dot(lhs, base.transform.right) > Vector3.Dot(lhs, -base.transform.right))
				{
					a = base.transform.right;
				}
				else
				{
					a = -base.transform.right;
				}
			}
			else
			{
				a = base.transform.right;
			}
			vector = a * this.obstacleAvoidanceScale;
			vector.Normalize();
		}
		if (vector != Vector3.zero)
		{
			Quaternion b = Quaternion.LookRotation(vector, Vector3.up);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, delta * this.GetTurnSpeed());
		}
	}

	// Token: 0x060018A8 RID: 6312 RVA: 0x000B4F60 File Offset: 0x000B3160
	private void UpdatePosition(float delta)
	{
		Vector3 forward = base.transform.forward;
		Vector3 vector = base.transform.position + forward * this.GetCurrentSpeed() * delta;
		vector = this.WaterClamp(vector);
		base.transform.position = vector;
	}

	// Token: 0x060018A9 RID: 6313 RVA: 0x000B4FB0 File Offset: 0x000B31B0
	private void UpdateSpeed(float delta)
	{
		this.currentSpeed = Mathf.Lerp(this.currentSpeed, this.GetDesiredSpeed(), delta * 4f);
	}

	// Token: 0x060018AA RID: 6314 RVA: 0x000B4FD0 File Offset: 0x000B31D0
	public void Update()
	{
		if (base.isServer)
		{
			this.Think(Time.deltaTime);
		}
	}

	// Token: 0x0400118C RID: 4492
	public Vector3 destination;

	// Token: 0x0400118D RID: 4493
	public float minSpeed;

	// Token: 0x0400118E RID: 4494
	public float maxSpeed;

	// Token: 0x0400118F RID: 4495
	public float idealDepth;

	// Token: 0x04001190 RID: 4496
	public float minTurnSpeed = 0.25f;

	// Token: 0x04001191 RID: 4497
	public float maxTurnSpeed = 2f;

	// Token: 0x04001192 RID: 4498
	public float attackCooldown = 7f;

	// Token: 0x04001193 RID: 4499
	public float aggroRange = 15f;

	// Token: 0x04001194 RID: 4500
	public float obstacleDetectionRadius = 1f;

	// Token: 0x04001195 RID: 4501
	public Animator animator;

	// Token: 0x04001196 RID: 4502
	public GameObjectRef bloodCloud;

	// Token: 0x04001197 RID: 4503
	public GameObjectRef corpsePrefab;

	// Token: 0x04001198 RID: 4504
	private const string SPEARGUN_KILL_STAT = "shark_speargun_kills";

	// Token: 0x04001199 RID: 4505
	[ServerVar]
	public static float forceSurfaceAmount = 0f;

	// Token: 0x0400119A RID: 4506
	[ServerVar]
	public static bool disable = false;

	// Token: 0x0400119B RID: 4507
	private Vector3 spawnPos;

	// Token: 0x0400119C RID: 4508
	private float stoppingDistance = 3f;

	// Token: 0x0400119D RID: 4509
	private float currentSpeed;

	// Token: 0x0400119E RID: 4510
	private float lastStartleTime;

	// Token: 0x0400119F RID: 4511
	private float startleDuration = 1f;

	// Token: 0x040011A0 RID: 4512
	private SimpleShark.SimpleState[] states;

	// Token: 0x040011A1 RID: 4513
	private SimpleShark.SimpleState _currentState;

	// Token: 0x040011A2 RID: 4514
	private bool sleeping;

	// Token: 0x040011A3 RID: 4515
	public List<Vector3> patrolPath = new List<Vector3>();

	// Token: 0x040011A4 RID: 4516
	private BasePlayer target;

	// Token: 0x040011A5 RID: 4517
	private float lastSeenTargetTime;

	// Token: 0x040011A6 RID: 4518
	private float nextTargetSearchTime;

	// Token: 0x040011A7 RID: 4519
	private static BasePlayer[] playerQueryResults = new BasePlayer[64];

	// Token: 0x040011A8 RID: 4520
	private float minFloorDist = 2f;

	// Token: 0x040011A9 RID: 4521
	private float minSurfaceDist = 1f;

	// Token: 0x040011AA RID: 4522
	private float lastTimeAttacked;

	// Token: 0x040011AB RID: 4523
	private float nextAttackTime;

	// Token: 0x040011AC RID: 4524
	private Vector3 cachedObstacleNormal;

	// Token: 0x040011AD RID: 4525
	private float cachedObstacleDistance;

	// Token: 0x040011AE RID: 4526
	private float obstacleAvoidanceScale;

	// Token: 0x040011AF RID: 4527
	private float obstacleDetectionRange = 5f;

	// Token: 0x040011B0 RID: 4528
	private float timeSinceLastObstacleCheck;

	// Token: 0x02000BF8 RID: 3064
	public class SimpleState
	{
		// Token: 0x06004B9B RID: 19355 RVA: 0x00192A13 File Offset: 0x00190C13
		public SimpleState(SimpleShark owner)
		{
			this.entity = owner;
		}

		// Token: 0x06004B9C RID: 19356 RVA: 0x00026FFC File Offset: 0x000251FC
		public virtual float State_Weight()
		{
			return 0f;
		}

		// Token: 0x06004B9D RID: 19357 RVA: 0x00192A22 File Offset: 0x00190C22
		public virtual void State_Enter()
		{
			this.stateEnterTime = Time.realtimeSinceStartup;
		}

		// Token: 0x06004B9E RID: 19358 RVA: 0x000059DD File Offset: 0x00003BDD
		public virtual void State_Think(float delta)
		{
		}

		// Token: 0x06004B9F RID: 19359 RVA: 0x000059DD File Offset: 0x00003BDD
		public virtual void State_Exit()
		{
		}

		// Token: 0x06004BA0 RID: 19360 RVA: 0x00003A54 File Offset: 0x00001C54
		public virtual bool CanInterrupt()
		{
			return true;
		}

		// Token: 0x06004BA1 RID: 19361 RVA: 0x00192A2F File Offset: 0x00190C2F
		public virtual float TimeInState()
		{
			return Time.realtimeSinceStartup - this.stateEnterTime;
		}

		// Token: 0x0400404E RID: 16462
		public SimpleShark entity;

		// Token: 0x0400404F RID: 16463
		private float stateEnterTime;
	}

	// Token: 0x02000BF9 RID: 3065
	public class IdleState : SimpleShark.SimpleState
	{
		// Token: 0x06004BA2 RID: 19362 RVA: 0x00192A3D File Offset: 0x00190C3D
		public IdleState(SimpleShark owner) : base(owner)
		{
		}

		// Token: 0x06004BA3 RID: 19363 RVA: 0x00192A46 File Offset: 0x00190C46
		public Vector3 GetTargetPatrolPosition()
		{
			return this.entity.patrolPath[this.patrolTargetIndex];
		}

		// Token: 0x06004BA4 RID: 19364 RVA: 0x000062DD File Offset: 0x000044DD
		public override float State_Weight()
		{
			return 1f;
		}

		// Token: 0x06004BA5 RID: 19365 RVA: 0x00192A60 File Offset: 0x00190C60
		public override void State_Enter()
		{
			float num = float.PositiveInfinity;
			int num2 = 0;
			for (int i = 0; i < this.entity.patrolPath.Count; i++)
			{
				float num3 = Vector3.Distance(this.entity.patrolPath[i], this.entity.transform.position);
				if (num3 < num)
				{
					num2 = i;
					num = num3;
				}
			}
			this.patrolTargetIndex = num2;
			base.State_Enter();
		}

		// Token: 0x06004BA6 RID: 19366 RVA: 0x00192ACC File Offset: 0x00190CCC
		public override void State_Think(float delta)
		{
			if (Vector3.Distance(this.GetTargetPatrolPosition(), this.entity.transform.position) < this.entity.stoppingDistance)
			{
				this.patrolTargetIndex++;
				if (this.patrolTargetIndex >= this.entity.patrolPath.Count)
				{
					this.patrolTargetIndex = 0;
				}
			}
			if (this.entity.TimeSinceAttacked() >= 120f && this.entity.healthFraction < 1f)
			{
				this.entity.health = this.entity.MaxHealth();
			}
			this.entity.destination = this.entity.WaterClamp(this.GetTargetPatrolPosition());
		}

		// Token: 0x06004BA7 RID: 19367 RVA: 0x00192B84 File Offset: 0x00190D84
		public override void State_Exit()
		{
			base.State_Exit();
		}

		// Token: 0x06004BA8 RID: 19368 RVA: 0x00003A54 File Offset: 0x00001C54
		public override bool CanInterrupt()
		{
			return true;
		}

		// Token: 0x04004050 RID: 16464
		private int patrolTargetIndex;
	}

	// Token: 0x02000BFA RID: 3066
	public class AttackState : SimpleShark.SimpleState
	{
		// Token: 0x06004BA9 RID: 19369 RVA: 0x00192A3D File Offset: 0x00190C3D
		public AttackState(SimpleShark owner) : base(owner)
		{
		}

		// Token: 0x06004BAA RID: 19370 RVA: 0x00192B8C File Offset: 0x00190D8C
		public override float State_Weight()
		{
			if (!this.entity.HasTarget() || !this.entity.CanAttack())
			{
				return 0f;
			}
			return 10f;
		}

		// Token: 0x06004BAB RID: 19371 RVA: 0x00192BB3 File Offset: 0x00190DB3
		public override void State_Enter()
		{
			base.State_Enter();
		}

		// Token: 0x06004BAC RID: 19372 RVA: 0x00192BBC File Offset: 0x00190DBC
		public override void State_Think(float delta)
		{
			BasePlayer target = this.entity.GetTarget();
			if (target == null)
			{
				return;
			}
			if (this.TimeInState() >= 10f)
			{
				this.entity.nextAttackTime = Time.realtimeSinceStartup + 4f;
				this.entity.Startle();
				return;
			}
			if (this.entity.CanAttack())
			{
				this.entity.Startle();
			}
			float num = Vector3.Distance(this.entity.GetTarget().eyes.position, this.entity.transform.position);
			bool flag = num < 4f;
			if (this.entity.CanAttack() && num <= 2f)
			{
				this.entity.DoAttack();
			}
			if (!flag)
			{
				Vector3 a = Vector3Ex.Direction(this.entity.GetTarget().eyes.position, this.entity.transform.position);
				Vector3 vector = target.eyes.position + a * 10f;
				vector = this.entity.WaterClamp(vector);
				this.entity.destination = vector;
			}
		}

		// Token: 0x06004BAD RID: 19373 RVA: 0x00192B84 File Offset: 0x00190D84
		public override void State_Exit()
		{
			base.State_Exit();
		}

		// Token: 0x06004BAE RID: 19374 RVA: 0x00003A54 File Offset: 0x00001C54
		public override bool CanInterrupt()
		{
			return true;
		}
	}
}
