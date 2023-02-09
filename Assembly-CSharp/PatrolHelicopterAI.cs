using System;
using System.Collections.Generic;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x020003FB RID: 1019
public class PatrolHelicopterAI : BaseMonoBehaviour
{
	// Token: 0x0600222B RID: 8747 RVA: 0x000DAB58 File Offset: 0x000D8D58
	public void UpdateTargetList()
	{
		Vector3 strafePos = Vector3.zero;
		bool flag = false;
		bool shouldUseNapalm = false;
		for (int i = this._targetList.Count - 1; i >= 0; i--)
		{
			PatrolHelicopterAI.targetinfo targetinfo = this._targetList[i];
			if (targetinfo == null || targetinfo.ent == null)
			{
				this._targetList.Remove(targetinfo);
			}
			else
			{
				if (UnityEngine.Time.realtimeSinceStartup > targetinfo.nextLOSCheck)
				{
					targetinfo.nextLOSCheck = UnityEngine.Time.realtimeSinceStartup + 1f;
					if (this.PlayerVisible(targetinfo.ply))
					{
						targetinfo.lastSeenTime = UnityEngine.Time.realtimeSinceStartup;
						targetinfo.visibleFor += 1f;
					}
					else
					{
						targetinfo.visibleFor = 0f;
					}
				}
				bool flag2 = targetinfo.ply ? targetinfo.ply.IsDead() : (targetinfo.ent.Health() <= 0f);
				if (targetinfo.TimeSinceSeen() >= 6f || flag2)
				{
					bool flag3 = UnityEngine.Random.Range(0f, 1f) >= 0f;
					if ((this.CanStrafe() || this.CanUseNapalm()) && this.IsAlive() && !flag && !flag2 && (targetinfo.ply == this.leftGun._target || targetinfo.ply == this.rightGun._target) && flag3)
					{
						shouldUseNapalm = (!this.ValidStrafeTarget(targetinfo.ply) || UnityEngine.Random.Range(0f, 1f) > 0.75f);
						flag = true;
						strafePos = targetinfo.ply.transform.position;
					}
					this._targetList.Remove(targetinfo);
				}
			}
		}
		foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
		{
			if (!basePlayer.InSafeZone() && Vector3Ex.Distance2D(base.transform.position, basePlayer.transform.position) <= 150f)
			{
				bool flag4 = false;
				using (List<PatrolHelicopterAI.targetinfo>.Enumerator enumerator2 = this._targetList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.ply == basePlayer)
						{
							flag4 = true;
							break;
						}
					}
				}
				if (!flag4 && basePlayer.GetThreatLevel() > 0.5f && this.PlayerVisible(basePlayer))
				{
					this._targetList.Add(new PatrolHelicopterAI.targetinfo(basePlayer, basePlayer));
				}
			}
		}
		if (flag)
		{
			this.ExitCurrentState();
			this.State_Strafe_Enter(strafePos, shouldUseNapalm);
		}
	}

	// Token: 0x0600222C RID: 8748 RVA: 0x000DAE34 File Offset: 0x000D9034
	public bool PlayerVisible(BasePlayer ply)
	{
		Vector3 position = ply.eyes.position;
		if (TOD_Sky.Instance.IsNight && Vector3.Distance(position, this.interestZoneOrigin) > 40f)
		{
			return false;
		}
		Vector3 vector = base.transform.position - Vector3.up * 6f;
		float num = Vector3.Distance(position, vector);
		Vector3 normalized = (position - vector).normalized;
		RaycastHit raycastHit;
		return GamePhysics.Trace(new Ray(vector + normalized * 5f, normalized), 0f, out raycastHit, num * 1.1f, 1218652417, QueryTriggerInteraction.UseGlobal, null) && raycastHit.collider.gameObject.ToBaseEntity() == ply;
	}

	// Token: 0x0600222D RID: 8749 RVA: 0x000DAEFC File Offset: 0x000D90FC
	public void WasAttacked(HitInfo info)
	{
		BasePlayer basePlayer = info.Initiator as BasePlayer;
		if (basePlayer != null)
		{
			this._targetList.Add(new PatrolHelicopterAI.targetinfo(basePlayer, basePlayer));
		}
	}

	// Token: 0x0600222E RID: 8750 RVA: 0x000DAF30 File Offset: 0x000D9130
	public void Awake()
	{
		if (PatrolHelicopter.lifetimeMinutes == 0f)
		{
			base.Invoke(new Action(this.DestroyMe), 1f);
			return;
		}
		base.InvokeRepeating(new Action(this.UpdateWind), 0f, 1f / this.windFrequency);
		this._lastPos = base.transform.position;
		this.spawnTime = UnityEngine.Time.realtimeSinceStartup;
		this.InitializeAI();
	}

	// Token: 0x0600222F RID: 8751 RVA: 0x000DAFA8 File Offset: 0x000D91A8
	public void SetInitialDestination(Vector3 dest, float mapScaleDistance = 0.25f)
	{
		this.hasInterestZone = true;
		this.interestZoneOrigin = dest;
		float x = TerrainMeta.Size.x;
		float y = dest.y + 25f;
		Vector3 vector = Vector3Ex.Range(-1f, 1f);
		vector.y = 0f;
		vector.Normalize();
		vector *= x * mapScaleDistance;
		vector.y = y;
		if (mapScaleDistance == 0f)
		{
			vector = this.interestZoneOrigin + new Vector3(0f, 10f, 0f);
		}
		base.transform.position = vector;
		this.ExitCurrentState();
		this.State_Move_Enter(dest);
	}

	// Token: 0x06002230 RID: 8752 RVA: 0x000DB054 File Offset: 0x000D9254
	public void Retire()
	{
		if (this.isRetiring)
		{
			return;
		}
		this.isRetiring = true;
		base.Invoke(new Action(this.DestroyMe), 240f);
		float x = TerrainMeta.Size.x;
		float y = 200f;
		Vector3 vector = Vector3Ex.Range(-1f, 1f);
		vector.y = 0f;
		vector.Normalize();
		vector *= x * 20f;
		vector.y = y;
		this.ExitCurrentState();
		this.State_Move_Enter(vector);
	}

	// Token: 0x06002231 RID: 8753 RVA: 0x000DB0E0 File Offset: 0x000D92E0
	public void SetIdealRotation(Quaternion newTargetRot, float rotationSpeedOverride = -1f)
	{
		float num = (rotationSpeedOverride == -1f) ? Mathf.Clamp01(this.moveSpeed / (this.maxSpeed * 0.5f)) : rotationSpeedOverride;
		this.rotationSpeed = num * this.maxRotationSpeed;
		this.targetRotation = newTargetRot;
	}

	// Token: 0x06002232 RID: 8754 RVA: 0x000DB128 File Offset: 0x000D9328
	public Quaternion GetYawRotationTo(Vector3 targetDest)
	{
		Vector3 a = targetDest;
		a.y = 0f;
		Vector3 position = base.transform.position;
		position.y = 0f;
		return Quaternion.LookRotation((a - position).normalized);
	}

	// Token: 0x06002233 RID: 8755 RVA: 0x000DB170 File Offset: 0x000D9370
	public void SetTargetDestination(Vector3 targetDest, float minDist = 5f, float minDistForFacingRotation = 30f)
	{
		this.destination = targetDest;
		this.destination_min_dist = minDist;
		float num = Vector3.Distance(targetDest, base.transform.position);
		if (num > minDistForFacingRotation && !this.IsTargeting())
		{
			this.SetIdealRotation(this.GetYawRotationTo(this.destination), -1f);
		}
		this.targetThrottleSpeed = this.GetThrottleForDistance(num);
	}

	// Token: 0x06002234 RID: 8756 RVA: 0x000DB1CD File Offset: 0x000D93CD
	public bool AtDestination()
	{
		return Vector3.Distance(base.transform.position, this.destination) < this.destination_min_dist;
	}

	// Token: 0x06002235 RID: 8757 RVA: 0x000DB1F0 File Offset: 0x000D93F0
	public void MoveToDestination()
	{
		Vector3 vector = Vector3.Lerp(this._lastMoveDir, (this.destination - base.transform.position).normalized, UnityEngine.Time.deltaTime / this.courseAdjustLerpTime);
		this._lastMoveDir = vector;
		this.throttleSpeed = Mathf.Lerp(this.throttleSpeed, this.targetThrottleSpeed, UnityEngine.Time.deltaTime / 3f);
		float d = this.throttleSpeed * this.maxSpeed;
		this.TerrainPushback();
		base.transform.position += vector * d * UnityEngine.Time.deltaTime;
		this.windVec = Vector3.Lerp(this.windVec, this.targetWindVec, UnityEngine.Time.deltaTime);
		base.transform.position += this.windVec * this.windForce * UnityEngine.Time.deltaTime;
		this.moveSpeed = Mathf.Lerp(this.moveSpeed, Vector3.Distance(this._lastPos, base.transform.position) / UnityEngine.Time.deltaTime, UnityEngine.Time.deltaTime * 2f);
		this._lastPos = base.transform.position;
	}

	// Token: 0x06002236 RID: 8758 RVA: 0x000DB330 File Offset: 0x000D9530
	public void TerrainPushback()
	{
		if (this._currentState == PatrolHelicopterAI.aiState.DEATH)
		{
			return;
		}
		Vector3 vector = base.transform.position + new Vector3(0f, 2f, 0f);
		Vector3 normalized = (this.destination - vector).normalized;
		float b = Vector3.Distance(this.destination, base.transform.position);
		Ray ray = new Ray(vector, normalized);
		float num = 5f;
		float num2 = Mathf.Min(100f, b);
		int mask = LayerMask.GetMask(new string[]
		{
			"Terrain",
			"World",
			"Construction"
		});
		Vector3 vector2 = Vector3.zero;
		RaycastHit raycastHit;
		if (UnityEngine.Physics.SphereCast(ray, num, out raycastHit, num2 - num * 0.5f, mask))
		{
			float num3 = 1f - raycastHit.distance / num2;
			float d = this.terrainPushForce * num3;
			vector2 = Vector3.up * d;
		}
		Ray ray2 = new Ray(vector, this._lastMoveDir);
		float num4 = Mathf.Min(10f, b);
		RaycastHit raycastHit2;
		if (UnityEngine.Physics.SphereCast(ray2, num, out raycastHit2, num4 - num * 0.5f, mask))
		{
			float num5 = 1f - raycastHit2.distance / num4;
			float d2 = this.obstaclePushForce * num5;
			vector2 += this._lastMoveDir * d2 * -1f;
			vector2 += Vector3.up * d2;
		}
		this.pushVec = Vector3.Lerp(this.pushVec, vector2, UnityEngine.Time.deltaTime);
		base.transform.position += this.pushVec * UnityEngine.Time.deltaTime;
	}

	// Token: 0x06002237 RID: 8759 RVA: 0x000DB4E8 File Offset: 0x000D96E8
	public void UpdateRotation()
	{
		if (this.hasAimTarget)
		{
			Vector3 position = base.transform.position;
			position.y = 0f;
			Vector3 aimTarget = this._aimTarget;
			aimTarget.y = 0f;
			Vector3 normalized = (aimTarget - position).normalized;
			Vector3 vector = Vector3.Cross(normalized, Vector3.up);
			float num = Vector3.Angle(normalized, base.transform.right);
			float num2 = Vector3.Angle(normalized, -base.transform.right);
			if (this.aimDoorSide)
			{
				if (num < num2)
				{
					this.targetRotation = Quaternion.LookRotation(vector);
				}
				else
				{
					this.targetRotation = Quaternion.LookRotation(-vector);
				}
			}
			else
			{
				this.targetRotation = Quaternion.LookRotation(normalized);
			}
		}
		this.rotationSpeed = Mathf.Lerp(this.rotationSpeed, this.maxRotationSpeed, UnityEngine.Time.deltaTime / 2f);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.targetRotation, this.rotationSpeed * UnityEngine.Time.deltaTime);
	}

	// Token: 0x06002238 RID: 8760 RVA: 0x000DB600 File Offset: 0x000D9800
	public void UpdateSpotlight()
	{
		if (this.hasInterestZone)
		{
			this.helicopterBase.spotlightTarget = new Vector3(this.interestZoneOrigin.x, TerrainMeta.HeightMap.GetHeight(this.interestZoneOrigin), this.interestZoneOrigin.z);
			return;
		}
		this.helicopterBase.spotlightTarget = Vector3.zero;
	}

	// Token: 0x06002239 RID: 8761 RVA: 0x000DB65C File Offset: 0x000D985C
	public void Update()
	{
		if (this.helicopterBase.isClient)
		{
			return;
		}
		PatrolHelicopterAI.heliInstance = this;
		this.UpdateTargetList();
		this.MoveToDestination();
		this.UpdateRotation();
		this.UpdateSpotlight();
		this.AIThink();
		this.DoMachineGuns();
		if (!this.isRetiring)
		{
			float num = Mathf.Max(this.spawnTime + PatrolHelicopter.lifetimeMinutes * 60f, this.lastDamageTime + 120f);
			if (UnityEngine.Time.realtimeSinceStartup > num)
			{
				this.Retire();
			}
		}
	}

	// Token: 0x0600223A RID: 8762 RVA: 0x000DB6DC File Offset: 0x000D98DC
	public void WeakspotDamaged(BaseHelicopter.weakspot weak, HitInfo info)
	{
		float num = UnityEngine.Time.realtimeSinceStartup - this.lastDamageTime;
		this.lastDamageTime = UnityEngine.Time.realtimeSinceStartup;
		BasePlayer basePlayer = info.Initiator as BasePlayer;
		bool flag = this.ValidStrafeTarget(basePlayer);
		bool flag2 = flag && this.CanStrafe();
		bool flag3 = !flag && this.CanUseNapalm();
		if (num < 5f && basePlayer != null && (flag2 || flag3))
		{
			this.ExitCurrentState();
			this.State_Strafe_Enter(info.Initiator.transform.position, flag3);
		}
	}

	// Token: 0x0600223B RID: 8763 RVA: 0x000DB75E File Offset: 0x000D995E
	public void CriticalDamage()
	{
		this.isDead = true;
		this.ExitCurrentState();
		this.State_Death_Enter();
	}

	// Token: 0x0600223C RID: 8764 RVA: 0x000DB774 File Offset: 0x000D9974
	public void DoMachineGuns()
	{
		if (this._targetList.Count > 0)
		{
			if (this.leftGun.NeedsNewTarget())
			{
				this.leftGun.UpdateTargetFromList(this._targetList);
			}
			if (this.rightGun.NeedsNewTarget())
			{
				this.rightGun.UpdateTargetFromList(this._targetList);
			}
		}
		this.leftGun.TurretThink();
		this.rightGun.TurretThink();
	}

	// Token: 0x0600223D RID: 8765 RVA: 0x000DB7E4 File Offset: 0x000D99E4
	public void FireGun(Vector3 targetPos, float aimCone, bool left)
	{
		if (PatrolHelicopter.guns == 0)
		{
			return;
		}
		Vector3 vector = (left ? this.helicopterBase.left_gun_muzzle.transform : this.helicopterBase.right_gun_muzzle.transform).position;
		Vector3 normalized = (targetPos - vector).normalized;
		vector += normalized * 2f;
		Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(aimCone, normalized, true);
		RaycastHit hit;
		if (GamePhysics.Trace(new Ray(vector, modifiedAimConeDirection), 0f, out hit, 300f, 1219701521, QueryTriggerInteraction.UseGlobal, null))
		{
			targetPos = hit.point;
			if (hit.collider)
			{
				BaseEntity entity = hit.GetEntity();
				if (entity && entity != this.helicopterBase)
				{
					BaseCombatEntity baseCombatEntity = entity as BaseCombatEntity;
					HitInfo info = new HitInfo(this.helicopterBase, entity, DamageType.Bullet, this.helicopterBase.bulletDamage * PatrolHelicopter.bulletDamageScale, hit.point);
					if (baseCombatEntity)
					{
						baseCombatEntity.OnAttacked(info);
						if (baseCombatEntity is BasePlayer)
						{
							Effect.server.ImpactEffect(new HitInfo
							{
								HitPositionWorld = hit.point - modifiedAimConeDirection * 0.25f,
								HitNormalWorld = -modifiedAimConeDirection,
								HitMaterial = StringPool.Get("Flesh")
							});
						}
					}
					else
					{
						entity.OnAttacked(info);
					}
				}
			}
		}
		else
		{
			targetPos = vector + modifiedAimConeDirection * 300f;
		}
		this.helicopterBase.ClientRPC<bool, Vector3>(null, "FireGun", left, targetPos);
	}

	// Token: 0x0600223E RID: 8766 RVA: 0x000DB977 File Offset: 0x000D9B77
	public bool CanInterruptState()
	{
		return this._currentState != PatrolHelicopterAI.aiState.STRAFE && this._currentState != PatrolHelicopterAI.aiState.DEATH;
	}

	// Token: 0x0600223F RID: 8767 RVA: 0x000DB990 File Offset: 0x000D9B90
	public bool IsAlive()
	{
		return !this.isDead;
	}

	// Token: 0x06002240 RID: 8768 RVA: 0x000DB99B File Offset: 0x000D9B9B
	public void DestroyMe()
	{
		this.helicopterBase.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06002241 RID: 8769 RVA: 0x000DB9A9 File Offset: 0x000D9BA9
	public Vector3 GetLastMoveDir()
	{
		return this._lastMoveDir;
	}

	// Token: 0x06002242 RID: 8770 RVA: 0x000DB9B4 File Offset: 0x000D9BB4
	public Vector3 GetMoveDirection()
	{
		return (this.destination - base.transform.position).normalized;
	}

	// Token: 0x06002243 RID: 8771 RVA: 0x000DB9DF File Offset: 0x000D9BDF
	public float GetMoveSpeed()
	{
		return this.moveSpeed;
	}

	// Token: 0x06002244 RID: 8772 RVA: 0x000DB9E7 File Offset: 0x000D9BE7
	public float GetMaxRotationSpeed()
	{
		return this.maxRotationSpeed;
	}

	// Token: 0x06002245 RID: 8773 RVA: 0x000DB9EF File Offset: 0x000D9BEF
	public bool IsTargeting()
	{
		return this.hasAimTarget;
	}

	// Token: 0x06002246 RID: 8774 RVA: 0x000DB9F7 File Offset: 0x000D9BF7
	public void UpdateWind()
	{
		this.targetWindVec = UnityEngine.Random.onUnitSphere;
	}

	// Token: 0x06002247 RID: 8775 RVA: 0x000DBA04 File Offset: 0x000D9C04
	public void SetAimTarget(Vector3 aimTarg, bool isDoorSide)
	{
		if (this.movementLockingAiming)
		{
			return;
		}
		this.hasAimTarget = true;
		this._aimTarget = aimTarg;
		this.aimDoorSide = isDoorSide;
	}

	// Token: 0x06002248 RID: 8776 RVA: 0x000DBA24 File Offset: 0x000D9C24
	public void ClearAimTarget()
	{
		this.hasAimTarget = false;
		this._aimTarget = Vector3.zero;
	}

	// Token: 0x06002249 RID: 8777 RVA: 0x000DBA38 File Offset: 0x000D9C38
	public void State_Death_Think(float timePassed)
	{
		float num = UnityEngine.Time.realtimeSinceStartup * 0.25f;
		float x = Mathf.Sin(6.2831855f * num) * 10f;
		float z = Mathf.Cos(6.2831855f * num) * 10f;
		Vector3 b = new Vector3(x, 0f, z);
		this.SetAimTarget(base.transform.position + b, true);
		Ray ray = new Ray(base.transform.position, this.GetLastMoveDir());
		int mask = LayerMask.GetMask(new string[]
		{
			"Terrain",
			"World",
			"Construction",
			"Water"
		});
		RaycastHit raycastHit;
		if (UnityEngine.Physics.SphereCast(ray, 3f, out raycastHit, 5f, mask) || UnityEngine.Time.realtimeSinceStartup > this.deathTimeout)
		{
			this.helicopterBase.Hurt(this.helicopterBase.health * 2f, DamageType.Generic, null, false);
		}
	}

	// Token: 0x0600224A RID: 8778 RVA: 0x000DBB28 File Offset: 0x000D9D28
	public void State_Death_Enter()
	{
		this.maxRotationSpeed *= 8f;
		this._currentState = PatrolHelicopterAI.aiState.DEATH;
		Vector3 randomOffset = this.GetRandomOffset(base.transform.position, 20f, 60f, 20f, 30f);
		int intVal = 1236478737;
		Vector3 targetDest;
		Vector3 vector;
		TransformUtil.GetGroundInfo(randomOffset - Vector3.up * 2f, out targetDest, out vector, 500f, intVal, null);
		this.SetTargetDestination(targetDest, 5f, 30f);
		this.targetThrottleSpeed = 0.5f;
		this.deathTimeout = UnityEngine.Time.realtimeSinceStartup + 10f;
	}

	// Token: 0x0600224B RID: 8779 RVA: 0x000059DD File Offset: 0x00003BDD
	public void State_Death_Leave()
	{
	}

	// Token: 0x0600224C RID: 8780 RVA: 0x000DBBD0 File Offset: 0x000D9DD0
	public void State_Idle_Think(float timePassed)
	{
		this.ExitCurrentState();
		this.State_Patrol_Enter();
	}

	// Token: 0x0600224D RID: 8781 RVA: 0x000DBBDE File Offset: 0x000D9DDE
	public void State_Idle_Enter()
	{
		this._currentState = PatrolHelicopterAI.aiState.IDLE;
	}

	// Token: 0x0600224E RID: 8782 RVA: 0x000059DD File Offset: 0x00003BDD
	public void State_Idle_Leave()
	{
	}

	// Token: 0x0600224F RID: 8783 RVA: 0x000DBBE8 File Offset: 0x000D9DE8
	public void State_Move_Think(float timePassed)
	{
		float distToTarget = Vector3.Distance(base.transform.position, this.destination);
		this.targetThrottleSpeed = this.GetThrottleForDistance(distToTarget);
		if (this.AtDestination())
		{
			this.ExitCurrentState();
			this.State_Idle_Enter();
		}
	}

	// Token: 0x06002250 RID: 8784 RVA: 0x000DBC30 File Offset: 0x000D9E30
	public void State_Move_Enter(Vector3 newPos)
	{
		this._currentState = PatrolHelicopterAI.aiState.MOVE;
		this.destination_min_dist = 5f;
		this.SetTargetDestination(newPos, 5f, 30f);
		float distToTarget = Vector3.Distance(base.transform.position, this.destination);
		this.targetThrottleSpeed = this.GetThrottleForDistance(distToTarget);
	}

	// Token: 0x06002251 RID: 8785 RVA: 0x000059DD File Offset: 0x00003BDD
	public void State_Move_Leave()
	{
	}

	// Token: 0x06002252 RID: 8786 RVA: 0x000DBC84 File Offset: 0x000D9E84
	public void State_Orbit_Think(float timePassed)
	{
		if (this.breakingOrbit)
		{
			if (this.AtDestination())
			{
				this.ExitCurrentState();
				this.State_Idle_Enter();
			}
		}
		else
		{
			if (Vector3Ex.Distance2D(base.transform.position, this.destination) > 15f)
			{
				return;
			}
			if (!this.hasEnteredOrbit)
			{
				this.hasEnteredOrbit = true;
				this.orbitStartTime = UnityEngine.Time.realtimeSinceStartup;
			}
			float num = 6.2831855f * this.currentOrbitDistance;
			float num2 = 0.5f * this.maxSpeed;
			float num3 = num / num2;
			this.currentOrbitTime += timePassed / (num3 * 1.01f);
			float rate = this.currentOrbitTime;
			Vector3 orbitPosition = this.GetOrbitPosition(rate);
			this.ClearAimTarget();
			this.SetTargetDestination(orbitPosition, 0f, 1f);
			this.targetThrottleSpeed = 0.5f;
		}
		if (UnityEngine.Time.realtimeSinceStartup - this.orbitStartTime > this.maxOrbitDuration && !this.breakingOrbit)
		{
			this.breakingOrbit = true;
			Vector3 appropriatePosition = this.GetAppropriatePosition(base.transform.position + base.transform.forward * 75f, 40f, 50f);
			this.SetTargetDestination(appropriatePosition, 10f, 0f);
		}
	}

	// Token: 0x06002253 RID: 8787 RVA: 0x000DBDBC File Offset: 0x000D9FBC
	public Vector3 GetOrbitPosition(float rate)
	{
		float x = Mathf.Sin(6.2831855f * rate) * this.currentOrbitDistance;
		float z = Mathf.Cos(6.2831855f * rate) * this.currentOrbitDistance;
		Vector3 vector = new Vector3(x, 20f, z);
		vector = this.interestZoneOrigin + vector;
		return vector;
	}

	// Token: 0x06002254 RID: 8788 RVA: 0x000DBE10 File Offset: 0x000DA010
	public void State_Orbit_Enter(float orbitDistance)
	{
		this._currentState = PatrolHelicopterAI.aiState.ORBIT;
		this.breakingOrbit = false;
		this.hasEnteredOrbit = false;
		this.orbitStartTime = UnityEngine.Time.realtimeSinceStartup;
		Vector3 vector = base.transform.position - this.interestZoneOrigin;
		this.currentOrbitTime = Mathf.Atan2(vector.x, vector.z);
		this.currentOrbitDistance = orbitDistance;
		this.ClearAimTarget();
		this.SetTargetDestination(this.GetOrbitPosition(this.currentOrbitTime), 20f, 0f);
	}

	// Token: 0x06002255 RID: 8789 RVA: 0x000DBE94 File Offset: 0x000DA094
	public void State_Orbit_Leave()
	{
		this.breakingOrbit = false;
		this.hasEnteredOrbit = false;
		this.currentOrbitTime = 0f;
		this.ClearAimTarget();
	}

	// Token: 0x06002256 RID: 8790 RVA: 0x000DBEB8 File Offset: 0x000DA0B8
	public Vector3 GetRandomPatrolDestination()
	{
		Vector3 vector = Vector3.zero;
		if (TerrainMeta.Path != null && TerrainMeta.Path.Monuments != null && TerrainMeta.Path.Monuments.Count > 0)
		{
			MonumentInfo monumentInfo = null;
			if (this._visitedMonuments.Count > 0)
			{
				foreach (MonumentInfo monumentInfo2 in TerrainMeta.Path.Monuments)
				{
					if (!monumentInfo2.IsSafeZone)
					{
						bool flag = false;
						foreach (MonumentInfo y in this._visitedMonuments)
						{
							if (monumentInfo2 == y)
							{
								flag = true;
							}
						}
						if (!flag)
						{
							monumentInfo = monumentInfo2;
							break;
						}
					}
				}
			}
			if (monumentInfo == null)
			{
				this._visitedMonuments.Clear();
				for (int i = 0; i < 5; i++)
				{
					monumentInfo = TerrainMeta.Path.Monuments[UnityEngine.Random.Range(0, TerrainMeta.Path.Monuments.Count)];
					if (!monumentInfo.IsSafeZone)
					{
						break;
					}
				}
			}
			if (monumentInfo)
			{
				vector = monumentInfo.transform.position;
				this._visitedMonuments.Add(monumentInfo);
				vector.y = TerrainMeta.HeightMap.GetHeight(vector) + 200f;
				RaycastHit raycastHit;
				if (TransformUtil.GetGroundInfo(vector, out raycastHit, 300f, 1235288065, null))
				{
					vector.y = raycastHit.point.y;
				}
				vector.y += 30f;
			}
		}
		else
		{
			float x = TerrainMeta.Size.x;
			float y2 = 30f;
			vector = Vector3Ex.Range(-1f, 1f);
			vector.y = 0f;
			vector.Normalize();
			vector *= x * UnityEngine.Random.Range(0f, 0.75f);
			vector.y = y2;
		}
		return vector;
	}

	// Token: 0x06002257 RID: 8791 RVA: 0x000DC0D8 File Offset: 0x000DA2D8
	public void State_Patrol_Think(float timePassed)
	{
		float num = Vector3Ex.Distance2D(base.transform.position, this.destination);
		if (num <= 25f)
		{
			this.targetThrottleSpeed = this.GetThrottleForDistance(num);
		}
		else
		{
			this.targetThrottleSpeed = 0.5f;
		}
		if (this.AtDestination() && this.arrivalTime == 0f)
		{
			this.arrivalTime = UnityEngine.Time.realtimeSinceStartup;
			this.ExitCurrentState();
			this.maxOrbitDuration = 20f;
			this.State_Orbit_Enter(75f);
		}
		if (this._targetList.Count > 0)
		{
			this.interestZoneOrigin = this._targetList[0].ply.transform.position + new Vector3(0f, 20f, 0f);
			this.ExitCurrentState();
			this.maxOrbitDuration = 10f;
			this.State_Orbit_Enter(75f);
		}
	}

	// Token: 0x06002258 RID: 8792 RVA: 0x000DC1C0 File Offset: 0x000DA3C0
	public void State_Patrol_Enter()
	{
		this._currentState = PatrolHelicopterAI.aiState.PATROL;
		Vector3 randomPatrolDestination = this.GetRandomPatrolDestination();
		this.SetTargetDestination(randomPatrolDestination, 10f, 30f);
		this.interestZoneOrigin = randomPatrolDestination;
		this.arrivalTime = 0f;
	}

	// Token: 0x06002259 RID: 8793 RVA: 0x000059DD File Offset: 0x00003BDD
	public void State_Patrol_Leave()
	{
	}

	// Token: 0x0600225A RID: 8794 RVA: 0x000DC1FE File Offset: 0x000DA3FE
	public int ClipRocketsLeft()
	{
		return this.numRocketsLeft;
	}

	// Token: 0x0600225B RID: 8795 RVA: 0x000DC206 File Offset: 0x000DA406
	public bool CanStrafe()
	{
		return UnityEngine.Time.realtimeSinceStartup - this.lastStrafeTime >= 20f && this.CanInterruptState();
	}

	// Token: 0x0600225C RID: 8796 RVA: 0x000DC223 File Offset: 0x000DA423
	public bool CanUseNapalm()
	{
		return UnityEngine.Time.realtimeSinceStartup - this.lastNapalmTime >= 30f;
	}

	// Token: 0x0600225D RID: 8797 RVA: 0x000DC23C File Offset: 0x000DA43C
	public void State_Strafe_Enter(Vector3 strafePos, bool shouldUseNapalm = false)
	{
		if (this.CanUseNapalm() && shouldUseNapalm)
		{
			this.useNapalm = shouldUseNapalm;
			this.lastNapalmTime = UnityEngine.Time.realtimeSinceStartup;
		}
		this.lastStrafeTime = UnityEngine.Time.realtimeSinceStartup;
		this._currentState = PatrolHelicopterAI.aiState.STRAFE;
		int mask = LayerMask.GetMask(new string[]
		{
			"Terrain",
			"World",
			"Construction",
			"Water"
		});
		Vector3 vector;
		Vector3 vector2;
		if (TransformUtil.GetGroundInfo(strafePos, out vector, out vector2, 100f, mask, base.transform))
		{
			this.strafe_target_position = vector;
		}
		else
		{
			this.strafe_target_position = strafePos;
		}
		this.numRocketsLeft = 12;
		this.lastRocketTime = 0f;
		this.movementLockingAiming = true;
		Vector3 randomOffset = this.GetRandomOffset(strafePos, 175f, 192.5f, 20f, 30f);
		this.SetTargetDestination(randomOffset, 10f, 30f);
		this.SetIdealRotation(this.GetYawRotationTo(randomOffset), -1f);
		this.puttingDistance = true;
	}

	// Token: 0x0600225E RID: 8798 RVA: 0x000DC330 File Offset: 0x000DA530
	public void State_Strafe_Think(float timePassed)
	{
		if (this.puttingDistance)
		{
			if (this.AtDestination())
			{
				this.puttingDistance = false;
				this.SetTargetDestination(this.strafe_target_position + new Vector3(0f, 40f, 0f), 10f, 30f);
				this.SetIdealRotation(this.GetYawRotationTo(this.strafe_target_position), -1f);
				return;
			}
		}
		else
		{
			this.SetIdealRotation(this.GetYawRotationTo(this.strafe_target_position), -1f);
			float num = Vector3Ex.Distance2D(this.strafe_target_position, base.transform.position);
			if (num <= 150f && this.ClipRocketsLeft() > 0 && UnityEngine.Time.realtimeSinceStartup - this.lastRocketTime > this.timeBetweenRockets)
			{
				float num2 = Vector3.Distance(this.strafe_target_position, base.transform.position) - 10f;
				if (num2 < 0f)
				{
					num2 = 0f;
				}
				if (!UnityEngine.Physics.Raycast(base.transform.position, (this.strafe_target_position - base.transform.position).normalized, num2, LayerMask.GetMask(new string[]
				{
					"Terrain",
					"World"
				})))
				{
					this.FireRocket();
				}
			}
			if (this.ClipRocketsLeft() <= 0 || num <= 15f)
			{
				this.ExitCurrentState();
				this.State_Move_Enter(this.GetAppropriatePosition(this.strafe_target_position + base.transform.forward * 120f, 20f, 30f));
			}
		}
	}

	// Token: 0x0600225F RID: 8799 RVA: 0x000DC4C7 File Offset: 0x000DA6C7
	public bool ValidStrafeTarget(BasePlayer ply)
	{
		return !ply.IsNearEnemyBase();
	}

	// Token: 0x06002260 RID: 8800 RVA: 0x000DC4D2 File Offset: 0x000DA6D2
	public void State_Strafe_Leave()
	{
		this.lastStrafeTime = UnityEngine.Time.realtimeSinceStartup;
		if (this.useNapalm)
		{
			this.lastNapalmTime = UnityEngine.Time.realtimeSinceStartup;
		}
		this.useNapalm = false;
		this.movementLockingAiming = false;
	}

	// Token: 0x06002261 RID: 8801 RVA: 0x000DC500 File Offset: 0x000DA700
	public void FireRocket()
	{
		this.numRocketsLeft--;
		this.lastRocketTime = UnityEngine.Time.realtimeSinceStartup;
		float num = 4f;
		bool flag = this.leftTubeFiredLast;
		this.leftTubeFiredLast = !this.leftTubeFiredLast;
		Transform transform = flag ? this.helicopterBase.rocket_tube_left.transform : this.helicopterBase.rocket_tube_right.transform;
		Vector3 vector = transform.position + transform.forward * 1f;
		Vector3 vector2 = (this.strafe_target_position - vector).normalized;
		if (num > 0f)
		{
			vector2 = AimConeUtil.GetModifiedAimConeDirection(num, vector2, true);
		}
		float maxDistance = 1f;
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(vector, vector2, out raycastHit, maxDistance, 1236478737))
		{
			maxDistance = raycastHit.distance - 0.1f;
		}
		Effect.server.Run(this.helicopterBase.rocket_fire_effect.resourcePath, this.helicopterBase, StringPool.Get(flag ? "rocket_tube_left" : "rocket_tube_right"), Vector3.zero, Vector3.forward, null, true);
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.useNapalm ? this.rocketProjectile_Napalm.resourcePath : this.rocketProjectile.resourcePath, vector, default(Quaternion), true);
		if (baseEntity == null)
		{
			return;
		}
		ServerProjectile component = baseEntity.GetComponent<ServerProjectile>();
		if (component)
		{
			component.InitializeVelocity(vector2 * component.speed);
		}
		baseEntity.Spawn();
	}

	// Token: 0x06002262 RID: 8802 RVA: 0x000DC683 File Offset: 0x000DA883
	public void InitializeAI()
	{
		this._lastThinkTime = UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x06002263 RID: 8803 RVA: 0x000DC690 File Offset: 0x000DA890
	public void OnCurrentStateExit()
	{
		switch (this._currentState)
		{
		default:
			this.State_Idle_Leave();
			return;
		case PatrolHelicopterAI.aiState.MOVE:
			this.State_Move_Leave();
			return;
		case PatrolHelicopterAI.aiState.ORBIT:
			this.State_Orbit_Leave();
			return;
		case PatrolHelicopterAI.aiState.STRAFE:
			this.State_Strafe_Leave();
			return;
		case PatrolHelicopterAI.aiState.PATROL:
			this.State_Patrol_Leave();
			return;
		}
	}

	// Token: 0x06002264 RID: 8804 RVA: 0x000DC6E0 File Offset: 0x000DA8E0
	public void ExitCurrentState()
	{
		this.OnCurrentStateExit();
		this._currentState = PatrolHelicopterAI.aiState.IDLE;
	}

	// Token: 0x06002265 RID: 8805 RVA: 0x000DC6EF File Offset: 0x000DA8EF
	public float GetTime()
	{
		return UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x06002266 RID: 8806 RVA: 0x000DC6F8 File Offset: 0x000DA8F8
	public void AIThink()
	{
		float time = this.GetTime();
		float timePassed = time - this._lastThinkTime;
		this._lastThinkTime = time;
		switch (this._currentState)
		{
		default:
			this.State_Idle_Think(timePassed);
			return;
		case PatrolHelicopterAI.aiState.MOVE:
			this.State_Move_Think(timePassed);
			return;
		case PatrolHelicopterAI.aiState.ORBIT:
			this.State_Orbit_Think(timePassed);
			return;
		case PatrolHelicopterAI.aiState.STRAFE:
			this.State_Strafe_Think(timePassed);
			return;
		case PatrolHelicopterAI.aiState.PATROL:
			this.State_Patrol_Think(timePassed);
			return;
		case PatrolHelicopterAI.aiState.DEATH:
			this.State_Death_Think(timePassed);
			return;
		}
	}

	// Token: 0x06002267 RID: 8807 RVA: 0x000DC774 File Offset: 0x000DA974
	public Vector3 GetRandomOffset(Vector3 origin, float minRange, float maxRange = 0f, float minHeight = 20f, float maxHeight = 30f)
	{
		Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
		onUnitSphere.y = 0f;
		onUnitSphere.Normalize();
		maxRange = Mathf.Max(minRange, maxRange);
		Vector3 origin2 = origin + onUnitSphere * UnityEngine.Random.Range(minRange, maxRange);
		return this.GetAppropriatePosition(origin2, minHeight, maxHeight);
	}

	// Token: 0x06002268 RID: 8808 RVA: 0x000DC7C4 File Offset: 0x000DA9C4
	public Vector3 GetAppropriatePosition(Vector3 origin, float minHeight = 20f, float maxHeight = 30f)
	{
		float num = 100f;
		Ray ray = new Ray(origin + new Vector3(0f, num, 0f), Vector3.down);
		float num2 = 5f;
		int mask = LayerMask.GetMask(new string[]
		{
			"Terrain",
			"World",
			"Construction",
			"Water"
		});
		RaycastHit raycastHit;
		if (UnityEngine.Physics.SphereCast(ray, num2, out raycastHit, num * 2f - num2, mask))
		{
			origin = raycastHit.point;
		}
		origin.y += UnityEngine.Random.Range(minHeight, maxHeight);
		return origin;
	}

	// Token: 0x06002269 RID: 8809 RVA: 0x000DC85C File Offset: 0x000DAA5C
	public float GetThrottleForDistance(float distToTarget)
	{
		float result;
		if (distToTarget >= 75f)
		{
			result = 1f;
		}
		else if (distToTarget >= 50f)
		{
			result = 0.75f;
		}
		else if (distToTarget >= 25f)
		{
			result = 0.33f;
		}
		else if (distToTarget >= 5f)
		{
			result = 0.05f;
		}
		else
		{
			result = 0.05f * (1f - distToTarget / 5f);
		}
		return result;
	}

	// Token: 0x04001AB4 RID: 6836
	public List<PatrolHelicopterAI.targetinfo> _targetList = new List<PatrolHelicopterAI.targetinfo>();

	// Token: 0x04001AB5 RID: 6837
	public Vector3 interestZoneOrigin;

	// Token: 0x04001AB6 RID: 6838
	public Vector3 destination;

	// Token: 0x04001AB7 RID: 6839
	public bool hasInterestZone;

	// Token: 0x04001AB8 RID: 6840
	public float moveSpeed;

	// Token: 0x04001AB9 RID: 6841
	public float maxSpeed = 25f;

	// Token: 0x04001ABA RID: 6842
	public float courseAdjustLerpTime = 2f;

	// Token: 0x04001ABB RID: 6843
	public Quaternion targetRotation;

	// Token: 0x04001ABC RID: 6844
	public Vector3 windVec;

	// Token: 0x04001ABD RID: 6845
	public Vector3 targetWindVec;

	// Token: 0x04001ABE RID: 6846
	public float windForce = 5f;

	// Token: 0x04001ABF RID: 6847
	public float windFrequency = 1f;

	// Token: 0x04001AC0 RID: 6848
	public float targetThrottleSpeed;

	// Token: 0x04001AC1 RID: 6849
	public float throttleSpeed;

	// Token: 0x04001AC2 RID: 6850
	public float maxRotationSpeed = 90f;

	// Token: 0x04001AC3 RID: 6851
	public float rotationSpeed;

	// Token: 0x04001AC4 RID: 6852
	public float terrainPushForce = 100f;

	// Token: 0x04001AC5 RID: 6853
	public float obstaclePushForce = 100f;

	// Token: 0x04001AC6 RID: 6854
	public HelicopterTurret leftGun;

	// Token: 0x04001AC7 RID: 6855
	public HelicopterTurret rightGun;

	// Token: 0x04001AC8 RID: 6856
	public static PatrolHelicopterAI heliInstance;

	// Token: 0x04001AC9 RID: 6857
	public BaseHelicopter helicopterBase;

	// Token: 0x04001ACA RID: 6858
	public PatrolHelicopterAI.aiState _currentState;

	// Token: 0x04001ACB RID: 6859
	private Vector3 _aimTarget;

	// Token: 0x04001ACC RID: 6860
	private bool movementLockingAiming;

	// Token: 0x04001ACD RID: 6861
	private bool hasAimTarget;

	// Token: 0x04001ACE RID: 6862
	private bool aimDoorSide;

	// Token: 0x04001ACF RID: 6863
	private Vector3 pushVec = Vector3.zero;

	// Token: 0x04001AD0 RID: 6864
	private Vector3 _lastPos;

	// Token: 0x04001AD1 RID: 6865
	private Vector3 _lastMoveDir;

	// Token: 0x04001AD2 RID: 6866
	private bool isDead;

	// Token: 0x04001AD3 RID: 6867
	private bool isRetiring;

	// Token: 0x04001AD4 RID: 6868
	private float spawnTime;

	// Token: 0x04001AD5 RID: 6869
	private float lastDamageTime;

	// Token: 0x04001AD6 RID: 6870
	private float deathTimeout;

	// Token: 0x04001AD7 RID: 6871
	private float destination_min_dist = 2f;

	// Token: 0x04001AD8 RID: 6872
	private float currentOrbitDistance;

	// Token: 0x04001AD9 RID: 6873
	private float currentOrbitTime;

	// Token: 0x04001ADA RID: 6874
	private bool hasEnteredOrbit;

	// Token: 0x04001ADB RID: 6875
	private float orbitStartTime;

	// Token: 0x04001ADC RID: 6876
	private float maxOrbitDuration = 30f;

	// Token: 0x04001ADD RID: 6877
	private bool breakingOrbit;

	// Token: 0x04001ADE RID: 6878
	public List<MonumentInfo> _visitedMonuments;

	// Token: 0x04001ADF RID: 6879
	public float arrivalTime;

	// Token: 0x04001AE0 RID: 6880
	public GameObjectRef rocketProjectile;

	// Token: 0x04001AE1 RID: 6881
	public GameObjectRef rocketProjectile_Napalm;

	// Token: 0x04001AE2 RID: 6882
	private bool leftTubeFiredLast;

	// Token: 0x04001AE3 RID: 6883
	private float lastRocketTime;

	// Token: 0x04001AE4 RID: 6884
	private float timeBetweenRockets = 0.2f;

	// Token: 0x04001AE5 RID: 6885
	private int numRocketsLeft = 12;

	// Token: 0x04001AE6 RID: 6886
	private const int maxRockets = 12;

	// Token: 0x04001AE7 RID: 6887
	private Vector3 strafe_target_position;

	// Token: 0x04001AE8 RID: 6888
	private bool puttingDistance;

	// Token: 0x04001AE9 RID: 6889
	private const float strafe_approach_range = 175f;

	// Token: 0x04001AEA RID: 6890
	private const float strafe_firing_range = 150f;

	// Token: 0x04001AEB RID: 6891
	private bool useNapalm;

	// Token: 0x04001AEC RID: 6892
	[NonSerialized]
	private float lastNapalmTime = float.NegativeInfinity;

	// Token: 0x04001AED RID: 6893
	[NonSerialized]
	private float lastStrafeTime = float.NegativeInfinity;

	// Token: 0x04001AEE RID: 6894
	private float _lastThinkTime;

	// Token: 0x02000C89 RID: 3209
	public class targetinfo
	{
		// Token: 0x06004D05 RID: 19717 RVA: 0x00196DDD File Offset: 0x00194FDD
		public targetinfo(BaseEntity initEnt, BasePlayer initPly = null)
		{
			this.ply = initPly;
			this.ent = initEnt;
			this.lastSeenTime = float.PositiveInfinity;
			this.nextLOSCheck = UnityEngine.Time.realtimeSinceStartup + 1.5f;
		}

		// Token: 0x06004D06 RID: 19718 RVA: 0x00196E1A File Offset: 0x0019501A
		public bool IsVisible()
		{
			return this.TimeSinceSeen() < 1.5f;
		}

		// Token: 0x06004D07 RID: 19719 RVA: 0x00196E29 File Offset: 0x00195029
		public float TimeSinceSeen()
		{
			return UnityEngine.Time.realtimeSinceStartup - this.lastSeenTime;
		}

		// Token: 0x040042C6 RID: 17094
		public BasePlayer ply;

		// Token: 0x040042C7 RID: 17095
		public BaseEntity ent;

		// Token: 0x040042C8 RID: 17096
		public float lastSeenTime = float.PositiveInfinity;

		// Token: 0x040042C9 RID: 17097
		public float visibleFor;

		// Token: 0x040042CA RID: 17098
		public float nextLOSCheck;
	}

	// Token: 0x02000C8A RID: 3210
	public enum aiState
	{
		// Token: 0x040042CC RID: 17100
		IDLE,
		// Token: 0x040042CD RID: 17101
		MOVE,
		// Token: 0x040042CE RID: 17102
		ORBIT,
		// Token: 0x040042CF RID: 17103
		STRAFE,
		// Token: 0x040042D0 RID: 17104
		PATROL,
		// Token: 0x040042D1 RID: 17105
		GUARD,
		// Token: 0x040042D2 RID: 17106
		DEATH
	}
}
