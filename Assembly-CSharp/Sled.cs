using System;
using UnityEngine;

// Token: 0x02000183 RID: 387
public class Sled : BaseVehicle, INotifyTrigger
{
	// Token: 0x060016E5 RID: 5861 RVA: 0x000AC69C File Offset: 0x000AA89C
	public override void ServerInit()
	{
		base.ServerInit();
		this.terrainHandler = new VehicleTerrainHandler(this);
		this.terrainHandler.RayLength = 0.6f;
		this.rigidBody.centerOfMass = this.CentreOfMassTransform.localPosition;
		base.InvokeRandomized(new Action(this.DecayOverTime), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
	}

	// Token: 0x060016E6 RID: 5862 RVA: 0x000AC70C File Offset: 0x000AA90C
	public override void OnDeployed(BaseEntity parent, BasePlayer deployedBy, Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
		this.UpdateGroundedFlag();
		this.UpdatePhysicsMaterial();
	}

	// Token: 0x060016E7 RID: 5863 RVA: 0x000AC734 File Offset: 0x000AA934
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (this.AnyMounted())
		{
			this.terrainHandler.FixedUpdate();
			if (!this.terrainHandler.IsGrounded)
			{
				Quaternion b = Quaternion.FromToRotation(base.transform.up, Vector3.up) * this.rigidBody.rotation;
				if (Quaternion.Angle(this.rigidBody.rotation, b) > this.VerticalAdjustmentAngleThreshold)
				{
					this.rigidBody.MoveRotation(Quaternion.Slerp(this.rigidBody.rotation, b, Time.fixedDeltaTime * this.VerticalAdjustmentForce));
				}
			}
		}
	}

	// Token: 0x060016E8 RID: 5864 RVA: 0x000AC7D0 File Offset: 0x000AA9D0
	private void UpdatePhysicsMaterial()
	{
		this.cachedMaterial = this.GetPhysicMaterial();
		Collider[] physicsMaterialTargets = this.PhysicsMaterialTargets;
		for (int i = 0; i < physicsMaterialTargets.Length; i++)
		{
			physicsMaterialTargets[i].sharedMaterial = this.cachedMaterial;
		}
		if (!this.AnyMounted() && this.rigidBody.IsSleeping())
		{
			base.CancelInvoke(new Action(this.UpdatePhysicsMaterial));
		}
		base.SetFlag(BaseEntity.Flags.Reserved2, this.terrainHandler.IsOnSnowOrIce, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved4, this.terrainHandler.OnSurface == VehicleTerrainHandler.Surface.Sand, false, true);
	}

	// Token: 0x060016E9 RID: 5865 RVA: 0x000AC868 File Offset: 0x000AAA68
	private void UpdateGroundedFlag()
	{
		if (!this.AnyMounted() && this.rigidBody.IsSleeping())
		{
			base.CancelInvoke(new Action(this.UpdateGroundedFlag));
		}
		base.SetFlag(BaseEntity.Flags.Reserved3, this.terrainHandler.IsGrounded, false, true);
	}

	// Token: 0x060016EA RID: 5866 RVA: 0x000AC8B4 File Offset: 0x000AAAB4
	private PhysicMaterial GetPhysicMaterial()
	{
		if (base.HasFlag(BaseEntity.Flags.Reserved1) || !this.AnyMounted())
		{
			return this.BrakeMaterial;
		}
		bool flag = this.terrainHandler.IsOnSnowOrIce || this.terrainHandler.OnSurface == VehicleTerrainHandler.Surface.Sand;
		if (flag)
		{
			this.leftIce = 0f;
		}
		else if (this.leftIce < 2f)
		{
			flag = true;
		}
		if (!flag)
		{
			return this.NonSnowMaterial;
		}
		return this.SnowMaterial;
	}

	// Token: 0x060016EB RID: 5867 RVA: 0x000AC938 File Offset: 0x000AAB38
	public override void PlayerMounted(BasePlayer player, BaseMountable seat)
	{
		base.PlayerMounted(player, seat);
		if (base.HasFlag(BaseEntity.Flags.Reserved1))
		{
			this.initialForceScale = 0f;
			base.InvokeRepeating(new Action(this.ApplyInitialForce), 0f, 0.1f);
			base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
		}
		if (!base.IsInvoking(new Action(this.UpdatePhysicsMaterial)))
		{
			base.InvokeRepeating(new Action(this.UpdatePhysicsMaterial), 0f, 0.5f);
		}
		if (!base.IsInvoking(new Action(this.UpdateGroundedFlag)))
		{
			base.InvokeRepeating(new Action(this.UpdateGroundedFlag), 0f, 0.1f);
		}
		if (this.rigidBody.IsSleeping())
		{
			this.rigidBody.WakeUp();
		}
	}

	// Token: 0x060016EC RID: 5868 RVA: 0x000ACA08 File Offset: 0x000AAC08
	private void ApplyInitialForce()
	{
		Vector3 forward = base.transform.forward;
		Vector3 a = (Vector3.Dot(forward, -Vector3.up) > Vector3.Dot(-forward, -Vector3.up)) ? forward : (-forward);
		this.rigidBody.AddForce(a * this.initialForceScale * (this.terrainHandler.IsOnSnowOrIce ? 1f : 0.25f), ForceMode.Acceleration);
		this.initialForceScale += this.InitialForceIncreaseRate;
		if (this.initialForceScale >= this.InitialForceCutoff && (this.rigidBody.velocity.magnitude > 1f || !this.terrainHandler.IsOnSnowOrIce))
		{
			base.CancelInvoke(new Action(this.ApplyInitialForce));
		}
	}

	// Token: 0x060016ED RID: 5869 RVA: 0x000ACAE4 File Offset: 0x000AACE4
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
		if (Vector3.Dot(base.transform.up, Vector3.up) < 0.1f || this.WaterFactor() > 0.25f)
		{
			this.DismountAllPlayers();
			return;
		}
		float num = inputState.IsDown(BUTTON.LEFT) ? -1f : 0f;
		num += (inputState.IsDown(BUTTON.RIGHT) ? 1f : 0f);
		if (inputState.IsDown(BUTTON.FORWARD) && this.lastNudge > this.NudgeCooldown && this.rigidBody.velocity.magnitude < this.MaxNudgeVelocity)
		{
			this.rigidBody.WakeUp();
			this.rigidBody.AddForce(base.transform.forward * this.NudgeForce, ForceMode.Impulse);
			this.rigidBody.AddForce(base.transform.up * this.NudgeForce * 0.5f, ForceMode.Impulse);
			this.lastNudge = 0f;
		}
		num *= this.TurnForce;
		Vector3 velocity = this.rigidBody.velocity;
		if (num != 0f)
		{
			base.transform.Rotate(Vector3.up * num * Time.deltaTime * velocity.magnitude, Space.Self);
		}
		if (this.terrainHandler.IsGrounded && Vector3.Dot(this.rigidBody.velocity.normalized, base.transform.forward) >= 0.5f)
		{
			this.rigidBody.velocity = Vector3.Lerp(this.rigidBody.velocity, base.transform.forward * velocity.magnitude, Time.deltaTime * this.DirectionMatchForce);
		}
	}

	// Token: 0x060016EE RID: 5870 RVA: 0x000ACCBD File Offset: 0x000AAEBD
	private void DecayOverTime()
	{
		if (this.AnyMounted())
		{
			return;
		}
		base.Hurt(this.DecayAmount);
	}

	// Token: 0x060016EF RID: 5871 RVA: 0x000ACCD4 File Offset: 0x000AAED4
	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && !player.isMounted;
	}

	// Token: 0x060016F0 RID: 5872 RVA: 0x000ACCEC File Offset: 0x000AAEEC
	public void OnObjects(TriggerNotify trigger)
	{
		foreach (BaseEntity baseEntity in trigger.entityContents)
		{
			if (!(baseEntity is Sled))
			{
				BaseVehicleModule baseVehicleModule;
				if ((baseVehicleModule = (baseEntity as BaseVehicleModule)) != null && baseVehicleModule.Vehicle != null && (baseVehicleModule.Vehicle.IsOn() || !baseVehicleModule.Vehicle.IsStationary()))
				{
					base.Kill(BaseNetworkable.DestroyMode.Gib);
					break;
				}
				BaseVehicle baseVehicle;
				if ((baseVehicle = (baseEntity as BaseVehicle)) != null && baseVehicle.HasDriver() && (baseVehicle.IsMoving() || baseVehicle.HasFlag(BaseEntity.Flags.On)))
				{
					base.Kill(BaseNetworkable.DestroyMode.Gib);
					break;
				}
			}
		}
	}

	// Token: 0x060016F1 RID: 5873 RVA: 0x000059DD File Offset: 0x00003BDD
	public void OnEmpty()
	{
	}

	// Token: 0x170001C7 RID: 455
	// (get) Token: 0x060016F2 RID: 5874 RVA: 0x00007074 File Offset: 0x00005274
	public override bool BlocksDoors
	{
		get
		{
			return false;
		}
	}

	// Token: 0x04000FEE RID: 4078
	private const BaseEntity.Flags BrakeOn = BaseEntity.Flags.Reserved1;

	// Token: 0x04000FEF RID: 4079
	private const BaseEntity.Flags OnSnow = BaseEntity.Flags.Reserved2;

	// Token: 0x04000FF0 RID: 4080
	private const BaseEntity.Flags IsGrounded = BaseEntity.Flags.Reserved3;

	// Token: 0x04000FF1 RID: 4081
	private const BaseEntity.Flags OnSand = BaseEntity.Flags.Reserved4;

	// Token: 0x04000FF2 RID: 4082
	public PhysicMaterial BrakeMaterial;

	// Token: 0x04000FF3 RID: 4083
	public PhysicMaterial SnowMaterial;

	// Token: 0x04000FF4 RID: 4084
	public PhysicMaterial NonSnowMaterial;

	// Token: 0x04000FF5 RID: 4085
	public Transform CentreOfMassTransform;

	// Token: 0x04000FF6 RID: 4086
	public Collider[] PhysicsMaterialTargets;

	// Token: 0x04000FF7 RID: 4087
	public float InitialForceCutoff = 3f;

	// Token: 0x04000FF8 RID: 4088
	public float InitialForceIncreaseRate = 0.05f;

	// Token: 0x04000FF9 RID: 4089
	public float TurnForce = 1f;

	// Token: 0x04000FFA RID: 4090
	public float DirectionMatchForce = 1f;

	// Token: 0x04000FFB RID: 4091
	public float VerticalAdjustmentForce = 1f;

	// Token: 0x04000FFC RID: 4092
	public float VerticalAdjustmentAngleThreshold = 15f;

	// Token: 0x04000FFD RID: 4093
	public float NudgeCooldown = 3f;

	// Token: 0x04000FFE RID: 4094
	public float NudgeForce = 2f;

	// Token: 0x04000FFF RID: 4095
	public float MaxNudgeVelocity = 2f;

	// Token: 0x04001000 RID: 4096
	public const float DecayFrequency = 60f;

	// Token: 0x04001001 RID: 4097
	public float DecayAmount = 10f;

	// Token: 0x04001002 RID: 4098
	public ParticleSystemContainer TrailEffects;

	// Token: 0x04001003 RID: 4099
	public SoundDefinition enterSnowSoundDef;

	// Token: 0x04001004 RID: 4100
	public SoundDefinition snowSlideLoopSoundDef;

	// Token: 0x04001005 RID: 4101
	public SoundDefinition dirtSlideLoopSoundDef;

	// Token: 0x04001006 RID: 4102
	public AnimationCurve movementLoopGainCurve;

	// Token: 0x04001007 RID: 4103
	public AnimationCurve movementLoopPitchCurve;

	// Token: 0x04001008 RID: 4104
	private VehicleTerrainHandler terrainHandler;

	// Token: 0x04001009 RID: 4105
	private PhysicMaterial cachedMaterial;

	// Token: 0x0400100A RID: 4106
	private float initialForceScale;

	// Token: 0x0400100B RID: 4107
	private TimeSince leftIce;

	// Token: 0x0400100C RID: 4108
	private TimeSince lastNudge;
}
