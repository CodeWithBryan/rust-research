using System;
using Rust;
using UnityEngine;

// Token: 0x0200045A RID: 1114
public class CH47HelicopterAIController : CH47Helicopter
{
	// Token: 0x0600248C RID: 9356 RVA: 0x000E6C40 File Offset: 0x000E4E40
	public void DropCrate()
	{
		if (this.numCrates <= 0)
		{
			return;
		}
		Vector3 pos = base.transform.position + Vector3.down * 5f;
		Quaternion rot = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.lockedCratePrefab.resourcePath, pos, rot, true);
		if (baseEntity)
		{
			baseEntity.SendMessage("SetWasDropped");
			baseEntity.Spawn();
		}
		this.numCrates--;
	}

	// Token: 0x0600248D RID: 9357 RVA: 0x000E6CD6 File Offset: 0x000E4ED6
	public bool OutOfCrates()
	{
		return this.numCrates <= 0;
	}

	// Token: 0x0600248E RID: 9358 RVA: 0x000E6CE4 File Offset: 0x000E4EE4
	public bool CanDropCrate()
	{
		return this.numCrates > 0;
	}

	// Token: 0x0600248F RID: 9359 RVA: 0x000028C8 File Offset: 0x00000AC8
	public bool IsDropDoorOpen()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved8);
	}

	// Token: 0x06002490 RID: 9360 RVA: 0x0006B907 File Offset: 0x00069B07
	public void SetDropDoorOpen(bool open)
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, open, false, true);
	}

	// Token: 0x06002491 RID: 9361 RVA: 0x000E6CEF File Offset: 0x000E4EEF
	public bool ShouldLand()
	{
		return this.shouldLand;
	}

	// Token: 0x06002492 RID: 9362 RVA: 0x000E6CF7 File Offset: 0x000E4EF7
	public void SetLandingTarget(Vector3 target)
	{
		this.shouldLand = true;
		this.landingTarget = target;
		this.numCrates = 0;
	}

	// Token: 0x06002493 RID: 9363 RVA: 0x000E6D0E File Offset: 0x000E4F0E
	public void ClearLandingTarget()
	{
		this.shouldLand = false;
	}

	// Token: 0x06002494 RID: 9364 RVA: 0x000E6D18 File Offset: 0x000E4F18
	public void TriggeredEventSpawn()
	{
		float x = TerrainMeta.Size.x;
		float y = 30f;
		Vector3 vector = Vector3Ex.Range(-1f, 1f);
		vector.y = 0f;
		vector.Normalize();
		vector *= x * 1f;
		vector.y = y;
		base.transform.position = vector;
	}

	// Token: 0x06002495 RID: 9365 RVA: 0x000E6D7B File Offset: 0x000E4F7B
	public override void AttemptMount(BasePlayer player, bool doMountChecks = true)
	{
		if (!player.IsNpc && !player.IsAdmin)
		{
			return;
		}
		base.AttemptMount(player, doMountChecks);
	}

	// Token: 0x06002496 RID: 9366 RVA: 0x000E6D96 File Offset: 0x000E4F96
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.SpawnScientists), 0.25f);
		this.SetMoveTarget(base.transform.position);
	}

	// Token: 0x06002497 RID: 9367 RVA: 0x000E6DC8 File Offset: 0x000E4FC8
	public void SpawnPassenger(Vector3 spawnPos, string prefabPath)
	{
		Quaternion identity = Quaternion.identity;
		HumanNPC component = GameManager.server.CreateEntity(prefabPath, spawnPos, identity, true).GetComponent<HumanNPC>();
		component.Spawn();
		this.AttemptMount(component, true);
	}

	// Token: 0x06002498 RID: 9368 RVA: 0x000E6E00 File Offset: 0x000E5000
	public void SpawnPassenger(Vector3 spawnPos)
	{
		Quaternion identity = Quaternion.identity;
		HumanNPC component = GameManager.server.CreateEntity(this.dismountablePrefab.resourcePath, spawnPos, identity, true).GetComponent<HumanNPC>();
		component.Spawn();
		this.AttemptMount(component, true);
	}

	// Token: 0x06002499 RID: 9369 RVA: 0x000E6E40 File Offset: 0x000E5040
	public void SpawnScientist(Vector3 spawnPos)
	{
		Quaternion identity = Quaternion.identity;
		HumanNPC component = GameManager.server.CreateEntity(this.scientistPrefab.resourcePath, spawnPos, identity, true).GetComponent<HumanNPC>();
		component.Spawn();
		this.AttemptMount(component, true);
		component.Brain.SetEnabled(false);
	}

	// Token: 0x0600249A RID: 9370 RVA: 0x000E6E8C File Offset: 0x000E508C
	public void SpawnScientists()
	{
		if (this.shouldLand)
		{
			float dropoffScale = CH47LandingZone.GetClosest(this.landingTarget).dropoffScale;
			int num = Mathf.FloorToInt((float)(this.mountPoints.Count - 2) * dropoffScale);
			for (int i = 0; i < num; i++)
			{
				Vector3 spawnPos = base.transform.position + base.transform.forward * 10f;
				this.SpawnPassenger(spawnPos, this.dismountablePrefab.resourcePath);
			}
			for (int j = 0; j < 1; j++)
			{
				Vector3 spawnPos2 = base.transform.position - base.transform.forward * 15f;
				this.SpawnPassenger(spawnPos2);
			}
			return;
		}
		for (int k = 0; k < 4; k++)
		{
			Vector3 spawnPos3 = base.transform.position + base.transform.forward * 10f;
			this.SpawnScientist(spawnPos3);
		}
		for (int l = 0; l < 1; l++)
		{
			Vector3 spawnPos4 = base.transform.position - base.transform.forward * 15f;
			this.SpawnScientist(spawnPos4);
		}
	}

	// Token: 0x0600249B RID: 9371 RVA: 0x000E6FCD File Offset: 0x000E51CD
	public void EnableFacingOverride(bool enabled)
	{
		this.aimDirOverride = enabled;
	}

	// Token: 0x0600249C RID: 9372 RVA: 0x000E6FD6 File Offset: 0x000E51D6
	public void SetMoveTarget(Vector3 position)
	{
		this._moveTarget = position;
	}

	// Token: 0x0600249D RID: 9373 RVA: 0x000E6FDF File Offset: 0x000E51DF
	public Vector3 GetMoveTarget()
	{
		return this._moveTarget;
	}

	// Token: 0x0600249E RID: 9374 RVA: 0x000E6FE7 File Offset: 0x000E51E7
	public void SetAimDirection(Vector3 dir)
	{
		this._aimDirection = dir;
	}

	// Token: 0x0600249F RID: 9375 RVA: 0x000E6FF0 File Offset: 0x000E51F0
	public Vector3 GetAimDirectionOverride()
	{
		return this._aimDirection;
	}

	// Token: 0x060024A0 RID: 9376 RVA: 0x000299AB File Offset: 0x00027BAB
	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	// Token: 0x060024A1 RID: 9377 RVA: 0x000E6FF8 File Offset: 0x000E51F8
	public override void MounteeTookDamage(BasePlayer mountee, HitInfo info)
	{
		this.InitiateAnger();
	}

	// Token: 0x060024A2 RID: 9378 RVA: 0x000E7000 File Offset: 0x000E5200
	public void CancelAnger()
	{
		if (base.SecondsSinceAttacked > 120f)
		{
			this.UnHostile();
			base.CancelInvoke(new Action(this.UnHostile));
		}
	}

	// Token: 0x060024A3 RID: 9379 RVA: 0x000E7028 File Offset: 0x000E5228
	public void InitiateAnger()
	{
		base.CancelInvoke(new Action(this.UnHostile));
		base.Invoke(new Action(this.UnHostile), 120f);
		foreach (BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
		{
			if (mountPointInfo.mountable != null)
			{
				BasePlayer mounted = mountPointInfo.mountable.GetMounted();
				if (mounted)
				{
					ScientistNPC scientistNPC = mounted as ScientistNPC;
					if (scientistNPC != null)
					{
						scientistNPC.Brain.SetEnabled(true);
					}
				}
			}
		}
	}

	// Token: 0x060024A4 RID: 9380 RVA: 0x000E70DC File Offset: 0x000E52DC
	public void UnHostile()
	{
		foreach (BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
		{
			if (mountPointInfo.mountable != null)
			{
				BasePlayer mounted = mountPointInfo.mountable.GetMounted();
				if (mounted)
				{
					ScientistNPC scientistNPC = mounted as ScientistNPC;
					if (scientistNPC != null)
					{
						scientistNPC.Brain.SetEnabled(false);
					}
				}
			}
		}
	}

	// Token: 0x060024A5 RID: 9381 RVA: 0x000E7168 File Offset: 0x000E5368
	public override void OnKilled(HitInfo info)
	{
		if (!this.OutOfCrates())
		{
			this.DropCrate();
		}
		base.OnKilled(info);
	}

	// Token: 0x060024A6 RID: 9382 RVA: 0x000E7180 File Offset: 0x000E5380
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		this.InitiateAnger();
		base.SetFlag(BaseEntity.Flags.Reserved7, base.healthFraction <= 0.8f, false, true);
		base.SetFlag(BaseEntity.Flags.OnFire, base.healthFraction <= 0.33f, false, true);
	}

	// Token: 0x060024A7 RID: 9383 RVA: 0x000E71D0 File Offset: 0x000E53D0
	public void DelayedKill()
	{
		foreach (BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
		{
			if (mountPointInfo.mountable != null)
			{
				BasePlayer mounted = mountPointInfo.mountable.GetMounted();
				if (mounted && mounted.transform != null && !mounted.IsDestroyed && !mounted.IsDead() && mounted.IsNpc)
				{
					mounted.Kill(BaseNetworkable.DestroyMode.None);
				}
			}
		}
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x060024A8 RID: 9384 RVA: 0x000E7274 File Offset: 0x000E5474
	public override void DismountAllPlayers()
	{
		foreach (BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
		{
			if (mountPointInfo.mountable != null)
			{
				BasePlayer mounted = mountPointInfo.mountable.GetMounted();
				if (mounted)
				{
					mounted.Hurt(10000f, DamageType.Explosion, this, false);
				}
			}
		}
	}

	// Token: 0x060024A9 RID: 9385 RVA: 0x000E72F4 File Offset: 0x000E54F4
	public void SetAltitudeProtection(bool on)
	{
		this.altitudeProtection = on;
	}

	// Token: 0x060024AA RID: 9386 RVA: 0x000E7300 File Offset: 0x000E5500
	public void CalculateDesiredAltitude()
	{
		this.CalculateOverrideAltitude();
		if (this.altOverride > this.currentDesiredAltitude)
		{
			this.currentDesiredAltitude = this.altOverride;
			return;
		}
		this.currentDesiredAltitude = Mathf.MoveTowards(this.currentDesiredAltitude, this.altOverride, Time.fixedDeltaTime * 5f);
	}

	// Token: 0x060024AB RID: 9387 RVA: 0x000E7351 File Offset: 0x000E5551
	public void SetMinHoverHeight(float newHeight)
	{
		this.hoverHeight = newHeight;
	}

	// Token: 0x060024AC RID: 9388 RVA: 0x000E735C File Offset: 0x000E555C
	public float CalculateOverrideAltitude()
	{
		if (Time.frameCount == this.lastAltitudeCheckFrame)
		{
			return this.altOverride;
		}
		this.lastAltitudeCheckFrame = Time.frameCount;
		float y = this.GetMoveTarget().y;
		float num = Mathf.Max(TerrainMeta.WaterMap.GetHeight(this.GetMoveTarget()), TerrainMeta.HeightMap.GetHeight(this.GetMoveTarget()));
		float num2 = Mathf.Max(y, num + this.hoverHeight);
		if (this.altitudeProtection)
		{
			Vector3 rhs = (this.rigidBody.velocity.magnitude < 0.1f) ? base.transform.forward : this.rigidBody.velocity.normalized;
			Vector3 normalized = (Vector3.Cross(Vector3.Cross(base.transform.up, rhs), Vector3.up) + Vector3.down * 0.3f).normalized;
			RaycastHit raycastHit;
			RaycastHit raycastHit2;
			if (Physics.SphereCast(base.transform.position - normalized * 20f, 20f, normalized, out raycastHit, 75f, 1218511105) && Physics.SphereCast(raycastHit.point + Vector3.up * 200f, 20f, Vector3.down, out raycastHit2, 200f, 1218511105))
			{
				num2 = raycastHit2.point.y + this.hoverHeight;
			}
		}
		this.altOverride = num2;
		return this.altOverride;
	}

	// Token: 0x060024AD RID: 9389 RVA: 0x000E74D8 File Offset: 0x000E56D8
	public override void SetDefaultInputState()
	{
		this.currentInputState.Reset();
		Vector3 moveTarget = this.GetMoveTarget();
		Vector3 vector = Vector3.Cross(base.transform.right, Vector3.up);
		Vector3 vector2 = Vector3.Cross(Vector3.up, vector);
		float num = -Vector3.Dot(Vector3.up, base.transform.right);
		float num2 = Vector3.Dot(Vector3.up, base.transform.forward);
		float num3 = Vector3Ex.Distance2D(base.transform.position, moveTarget);
		float y = base.transform.position.y;
		float num4 = this.currentDesiredAltitude;
		(base.transform.position + base.transform.forward * 10f).y = num4;
		Vector3 lhs = Vector3Ex.Direction2D(moveTarget, base.transform.position);
		float num5 = -Vector3.Dot(lhs, vector2);
		float num6 = Vector3.Dot(lhs, vector);
		float num7 = Mathf.InverseLerp(0f, 25f, num3);
		if (num6 > 0f)
		{
			float num8 = Mathf.InverseLerp(-this.maxTiltAngle, 0f, num2);
			this.currentInputState.pitch = 1f * num6 * num8 * num7;
		}
		else
		{
			float num9 = 1f - Mathf.InverseLerp(0f, this.maxTiltAngle, num2);
			this.currentInputState.pitch = 1f * num6 * num9 * num7;
		}
		if (num5 > 0f)
		{
			float num10 = Mathf.InverseLerp(-this.maxTiltAngle, 0f, num);
			this.currentInputState.roll = 1f * num5 * num10 * num7;
		}
		else
		{
			float num11 = 1f - Mathf.InverseLerp(0f, this.maxTiltAngle, num);
			this.currentInputState.roll = 1f * num5 * num11 * num7;
		}
		float value = Mathf.Abs(num4 - y);
		float num12 = 1f - Mathf.InverseLerp(10f, 30f, value);
		this.currentInputState.pitch *= num12;
		this.currentInputState.roll *= num12;
		float num13 = this.maxTiltAngle;
		float num14 = Mathf.InverseLerp(0f + Mathf.Abs(this.currentInputState.pitch) * num13, num13 + Mathf.Abs(this.currentInputState.pitch) * num13, Mathf.Abs(num2));
		this.currentInputState.pitch += num14 * ((num2 < 0f) ? -1f : 1f);
		float num15 = Mathf.InverseLerp(0f + Mathf.Abs(this.currentInputState.roll) * num13, num13 + Mathf.Abs(this.currentInputState.roll) * num13, Mathf.Abs(num));
		this.currentInputState.roll += num15 * ((num < 0f) ? -1f : 1f);
		if (this.aimDirOverride || num3 > 30f)
		{
			Vector3 rhs = this.aimDirOverride ? this.GetAimDirectionOverride() : Vector3Ex.Direction2D(this.GetMoveTarget(), base.transform.position);
			Vector3 to = this.aimDirOverride ? this.GetAimDirectionOverride() : Vector3Ex.Direction2D(this.GetMoveTarget(), base.transform.position);
			float num16 = Vector3.Dot(vector2, rhs);
			float f = Vector3.Angle(vector, to);
			float num17 = Mathf.InverseLerp(0f, 70f, Mathf.Abs(f));
			this.currentInputState.yaw = ((num16 > 0f) ? 1f : 0f);
			this.currentInputState.yaw -= ((num16 < 0f) ? 1f : 0f);
			this.currentInputState.yaw *= num17;
		}
		float throttle = Mathf.InverseLerp(5f, 30f, num3);
		this.currentInputState.throttle = throttle;
	}

	// Token: 0x060024AE RID: 9390 RVA: 0x000E78DC File Offset: 0x000E5ADC
	public void MaintainAIAltutide()
	{
		ref Vector3 ptr = base.transform.position + this.rigidBody.velocity;
		float num = this.currentDesiredAltitude;
		float y = ptr.y;
		float value = Mathf.Abs(num - y);
		bool flag = num > y;
		float d = Mathf.InverseLerp(0f, 10f, value) * this.AiAltitudeForce * (flag ? 1f : -1f);
		this.rigidBody.AddForce(Vector3.up * d, ForceMode.Force);
	}

	// Token: 0x060024AF RID: 9391 RVA: 0x000E7960 File Offset: 0x000E5B60
	public override void VehicleFixedUpdate()
	{
		this.hoverForceScale = 1f;
		base.VehicleFixedUpdate();
		base.SetFlag(BaseEntity.Flags.Reserved5, TOD_Sky.Instance.IsNight, false, true);
		this.CalculateDesiredAltitude();
		this.MaintainAIAltutide();
	}

	// Token: 0x060024B0 RID: 9392 RVA: 0x000E7998 File Offset: 0x000E5B98
	public override void DestroyShared()
	{
		if (base.isServer)
		{
			foreach (BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
			{
				if (mountPointInfo.mountable != null)
				{
					BasePlayer mounted = mountPointInfo.mountable.GetMounted();
					if (mounted && mounted.transform != null && !mounted.IsDestroyed && !mounted.IsDead() && mounted.IsNpc)
					{
						mounted.Kill(BaseNetworkable.DestroyMode.None);
					}
				}
			}
		}
		base.DestroyShared();
	}

	// Token: 0x04001D2D RID: 7469
	public GameObjectRef scientistPrefab;

	// Token: 0x04001D2E RID: 7470
	public GameObjectRef dismountablePrefab;

	// Token: 0x04001D2F RID: 7471
	public GameObjectRef weakDismountablePrefab;

	// Token: 0x04001D30 RID: 7472
	public float maxTiltAngle = 0.3f;

	// Token: 0x04001D31 RID: 7473
	public float AiAltitudeForce = 10000f;

	// Token: 0x04001D32 RID: 7474
	public GameObjectRef lockedCratePrefab;

	// Token: 0x04001D33 RID: 7475
	public const BaseEntity.Flags Flag_Damaged = BaseEntity.Flags.Reserved7;

	// Token: 0x04001D34 RID: 7476
	public const BaseEntity.Flags Flag_NearDeath = BaseEntity.Flags.OnFire;

	// Token: 0x04001D35 RID: 7477
	public const BaseEntity.Flags Flag_DropDoorOpen = BaseEntity.Flags.Reserved8;

	// Token: 0x04001D36 RID: 7478
	public GameObject triggerHurt;

	// Token: 0x04001D37 RID: 7479
	public Vector3 landingTarget;

	// Token: 0x04001D38 RID: 7480
	private int numCrates = 1;

	// Token: 0x04001D39 RID: 7481
	private bool shouldLand;

	// Token: 0x04001D3A RID: 7482
	private bool aimDirOverride;

	// Token: 0x04001D3B RID: 7483
	private Vector3 _aimDirection = Vector3.forward;

	// Token: 0x04001D3C RID: 7484
	private Vector3 _moveTarget = Vector3.zero;

	// Token: 0x04001D3D RID: 7485
	private int lastAltitudeCheckFrame;

	// Token: 0x04001D3E RID: 7486
	private float altOverride;

	// Token: 0x04001D3F RID: 7487
	private float currentDesiredAltitude;

	// Token: 0x04001D40 RID: 7488
	private bool altitudeProtection = true;

	// Token: 0x04001D41 RID: 7489
	private float hoverHeight = 30f;
}
