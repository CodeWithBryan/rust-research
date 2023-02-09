using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x02000454 RID: 1108
public abstract class GroundVehicle : BaseVehicle, IEngineControllerUser, IEntity, TriggerHurtNotChild.IHurtTriggerUser
{
	// Token: 0x170002C5 RID: 709
	// (get) Token: 0x06002445 RID: 9285 RVA: 0x000E5AFB File Offset: 0x000E3CFB
	// (set) Token: 0x06002446 RID: 9286 RVA: 0x000E5B03 File Offset: 0x000E3D03
	public Vector3 Velocity { get; private set; }

	// Token: 0x170002C6 RID: 710
	// (get) Token: 0x06002447 RID: 9287
	public abstract float DriveWheelVelocity { get; }

	// Token: 0x170002C7 RID: 711
	// (get) Token: 0x06002448 RID: 9288 RVA: 0x000035F8 File Offset: 0x000017F8
	public bool LightsAreOn
	{
		get
		{
			return base.HasFlag(BaseEntity.Flags.Reserved5);
		}
	}

	// Token: 0x170002C8 RID: 712
	// (get) Token: 0x06002449 RID: 9289 RVA: 0x000E5B0C File Offset: 0x000E3D0C
	public VehicleEngineController<GroundVehicle>.EngineState CurEngineState
	{
		get
		{
			return this.engineController.CurEngineState;
		}
	}

	// Token: 0x0600244A RID: 9290 RVA: 0x000E5B19 File Offset: 0x000E3D19
	public override void InitShared()
	{
		base.InitShared();
		this.engineController = new VehicleEngineController<GroundVehicle>(this, base.isServer, this.engineStartupTime, this.fuelStoragePrefab, this.waterloggedPoint, BaseEntity.Flags.Reserved1);
	}

	// Token: 0x0600244B RID: 9291 RVA: 0x000E5B4A File Offset: 0x000E3D4A
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (old == next)
		{
			return;
		}
		if (base.isServer)
		{
			this.ServerFlagsChanged(old, next);
		}
	}

	// Token: 0x0600244C RID: 9292 RVA: 0x000E5B69 File Offset: 0x000E3D69
	public float GetSpeed()
	{
		if (base.IsStationary())
		{
			return 0f;
		}
		return Vector3.Dot(this.Velocity, base.transform.forward);
	}

	// Token: 0x0600244D RID: 9293
	public abstract float GetMaxForwardSpeed();

	// Token: 0x0600244E RID: 9294
	public abstract float GetThrottleInput();

	// Token: 0x0600244F RID: 9295
	public abstract float GetBrakeInput();

	// Token: 0x06002450 RID: 9296 RVA: 0x000E5B8F File Offset: 0x000E3D8F
	protected override bool CanPushNow(BasePlayer pusher)
	{
		return base.CanPushNow(pusher) && !pusher.isMounted && !pusher.IsSwimming() && !pusher.IsStandingOnEntity(this, 8192);
	}

	// Token: 0x06002451 RID: 9297 RVA: 0x000E5BBD File Offset: 0x000E3DBD
	public override void ServerInit()
	{
		base.ServerInit();
		this.timeSinceDragModSet = default(TimeSince);
		this.timeSinceDragModSet = float.MaxValue;
	}

	// Token: 0x06002452 RID: 9298
	public abstract void OnEngineStartFailed();

	// Token: 0x06002453 RID: 9299
	public abstract bool MeetsEngineRequirements();

	// Token: 0x06002454 RID: 9300 RVA: 0x000059DD File Offset: 0x00003BDD
	protected virtual void ServerFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
	}

	// Token: 0x06002455 RID: 9301 RVA: 0x000E5BE1 File Offset: 0x000E3DE1
	protected void OnCollisionEnter(Collision collision)
	{
		if (base.isServer)
		{
			this.ProcessCollision(collision);
		}
	}

	// Token: 0x06002456 RID: 9302 RVA: 0x000E5BF4 File Offset: 0x000E3DF4
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (base.IsMovingOrOn)
		{
			this.Velocity = base.GetLocalVelocity();
		}
		else
		{
			this.Velocity = Vector3.zero;
		}
		if (this.LightsAreOn && !this.AnyMounted())
		{
			base.SetFlag(BaseEntity.Flags.Reserved5, false, false, true);
		}
		if (Time.time >= this.nextCollisionDamageTime)
		{
			this.nextCollisionDamageTime = Time.time + 0.33f;
			foreach (KeyValuePair<BaseEntity, float> keyValuePair in this.damageSinceLastTick)
			{
				this.DoCollisionDamage(keyValuePair.Key, keyValuePair.Value);
			}
			this.damageSinceLastTick.Clear();
		}
	}

	// Token: 0x06002457 RID: 9303 RVA: 0x000E5CC4 File Offset: 0x000E3EC4
	public override void LightToggle(BasePlayer player)
	{
		if (!base.IsDriver(player))
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Reserved5, !this.LightsAreOn, false, true);
	}

	// Token: 0x06002458 RID: 9304 RVA: 0x000E5CE6 File Offset: 0x000E3EE6
	public float GetPlayerDamageMultiplier()
	{
		return Mathf.Abs(this.GetSpeed()) * 1f;
	}

	// Token: 0x06002459 RID: 9305 RVA: 0x000E5CFC File Offset: 0x000E3EFC
	public void OnHurtTriggerOccupant(BaseEntity hurtEntity, DamageType damageType, float damageTotal)
	{
		if (base.isClient)
		{
			return;
		}
		if (hurtEntity.IsDestroyed)
		{
			return;
		}
		Vector3 a = hurtEntity.GetLocalVelocity() - this.Velocity;
		Vector3 position = base.ClosestPoint(hurtEntity.transform.position);
		Vector3 a2 = hurtEntity.RealisticMass * a;
		this.rigidBody.AddForceAtPosition(a2 * 1.25f, position, ForceMode.Impulse);
		this.QueueCollisionDamage(this, a2.magnitude * 0.75f / Time.deltaTime);
		this.SetTempDrag(2.25f, 1f);
	}

	// Token: 0x0600245A RID: 9306 RVA: 0x000E5D90 File Offset: 0x000E3F90
	private float QueueCollisionDamage(BaseEntity hitEntity, float forceMagnitude)
	{
		float num = Mathf.InverseLerp(this.minCollisionDamageForce, this.maxCollisionDamageForce, forceMagnitude);
		if (num > 0f)
		{
			float num2 = Mathf.Lerp(1f, 200f, num) * this.collisionDamageMultiplier;
			float num3;
			if (this.damageSinceLastTick.TryGetValue(hitEntity, out num3))
			{
				if (num3 < num2)
				{
					this.damageSinceLastTick[hitEntity] = num2;
				}
			}
			else
			{
				this.damageSinceLastTick[hitEntity] = num2;
			}
		}
		return num;
	}

	// Token: 0x0600245B RID: 9307 RVA: 0x000E5E01 File Offset: 0x000E4001
	protected virtual void DoCollisionDamage(BaseEntity hitEntity, float damage)
	{
		base.Hurt(damage, DamageType.Collision, this, false);
	}

	// Token: 0x0600245C RID: 9308 RVA: 0x000E5E10 File Offset: 0x000E4010
	private void ProcessCollision(Collision collision)
	{
		if (base.isClient || collision == null || collision.gameObject == null || collision.gameObject == null)
		{
			return;
		}
		ContactPoint contact = collision.GetContact(0);
		BaseEntity baseEntity = null;
		if (contact.otherCollider.attachedRigidbody == this.rigidBody)
		{
			baseEntity = contact.otherCollider.ToBaseEntity();
		}
		else if (contact.thisCollider.attachedRigidbody == this.rigidBody)
		{
			baseEntity = contact.thisCollider.ToBaseEntity();
		}
		if (baseEntity != null)
		{
			float forceMagnitude = collision.impulse.magnitude / Time.fixedDeltaTime;
			if (this.QueueCollisionDamage(baseEntity, forceMagnitude) > 0f)
			{
				base.TryShowCollisionFX(collision, this.collisionEffect);
			}
		}
	}

	// Token: 0x0600245D RID: 9309 RVA: 0x000E5ED7 File Offset: 0x000E40D7
	public virtual float GetModifiedDrag()
	{
		return (1f - Mathf.InverseLerp(0f, this.dragModDuration, this.timeSinceDragModSet)) * this.dragMod;
	}

	// Token: 0x0600245E RID: 9310 RVA: 0x000E5F01 File Offset: 0x000E4101
	public override EntityFuelSystem GetFuelSystem()
	{
		return this.engineController.FuelSystem;
	}

	// Token: 0x0600245F RID: 9311 RVA: 0x000066ED File Offset: 0x000048ED
	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer && this.isSpawned)
		{
			this.GetFuelSystem().CheckNewChild(child);
		}
	}

	// Token: 0x06002460 RID: 9312 RVA: 0x000E5F0E File Offset: 0x000E410E
	private void SetTempDrag(float drag, float duration)
	{
		this.dragMod = Mathf.Clamp(drag, 0f, 1000f);
		this.timeSinceDragModSet = 0f;
		this.dragModDuration = duration;
	}

	// Token: 0x06002462 RID: 9314 RVA: 0x00007260 File Offset: 0x00005460
	void IEngineControllerUser.Invoke(Action action, float time)
	{
		base.Invoke(action, time);
	}

	// Token: 0x06002463 RID: 9315 RVA: 0x0000726A File Offset: 0x0000546A
	void IEngineControllerUser.CancelInvoke(Action action)
	{
		base.CancelInvoke(action);
	}

	// Token: 0x04001CEE RID: 7406
	[Header("GroundVehicle")]
	[SerializeField]
	protected GroundVehicleAudio gvAudio;

	// Token: 0x04001CEF RID: 7407
	[SerializeField]
	private GameObjectRef fuelStoragePrefab;

	// Token: 0x04001CF0 RID: 7408
	[SerializeField]
	private Transform waterloggedPoint;

	// Token: 0x04001CF1 RID: 7409
	[SerializeField]
	private GameObjectRef collisionEffect;

	// Token: 0x04001CF2 RID: 7410
	[SerializeField]
	private float engineStartupTime = 0.5f;

	// Token: 0x04001CF3 RID: 7411
	[SerializeField]
	private float minCollisionDamageForce = 20000f;

	// Token: 0x04001CF4 RID: 7412
	[SerializeField]
	private float maxCollisionDamageForce = 2500000f;

	// Token: 0x04001CF5 RID: 7413
	[SerializeField]
	private float collisionDamageMultiplier = 1f;

	// Token: 0x04001CF7 RID: 7415
	protected VehicleEngineController<GroundVehicle> engineController;

	// Token: 0x04001CF8 RID: 7416
	private Dictionary<BaseEntity, float> damageSinceLastTick = new Dictionary<BaseEntity, float>();

	// Token: 0x04001CF9 RID: 7417
	private float nextCollisionDamageTime;

	// Token: 0x04001CFA RID: 7418
	private float dragMod;

	// Token: 0x04001CFB RID: 7419
	private float dragModDuration;

	// Token: 0x04001CFC RID: 7420
	private TimeSince timeSinceDragModSet;
}
