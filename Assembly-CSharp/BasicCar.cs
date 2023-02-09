using System;
using UnityEngine;

// Token: 0x02000449 RID: 1097
public class BasicCar : BaseVehicle
{
	// Token: 0x060023EB RID: 9195 RVA: 0x000E2A59 File Offset: 0x000E0C59
	public override float MaxVelocity()
	{
		return 50f;
	}

	// Token: 0x060023EC RID: 9196 RVA: 0x000E2A60 File Offset: 0x000E0C60
	public override Vector3 EyePositionForPlayer(BasePlayer player, Quaternion viewRot)
	{
		if (this.PlayerIsMounted(player))
		{
			return this.driverEye.transform.position;
		}
		return Vector3.zero;
	}

	// Token: 0x060023ED RID: 9197 RVA: 0x000E2A84 File Offset: 0x000E0C84
	public override void ServerInit()
	{
		if (base.isClient)
		{
			return;
		}
		base.ServerInit();
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.rigidBody.centerOfMass = this.centerOfMass.localPosition;
		this.rigidBody.isKinematic = false;
		if (BasicCar.chairtest)
		{
			this.SpawnChairTest();
		}
	}

	// Token: 0x060023EE RID: 9198 RVA: 0x000E2ADC File Offset: 0x000E0CDC
	public void SpawnChairTest()
	{
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.chairRef.resourcePath, this.chairAnchorTest.transform.localPosition, default(Quaternion), true);
		baseEntity.Spawn();
		DestroyOnGroundMissing component = baseEntity.GetComponent<DestroyOnGroundMissing>();
		if (component != null)
		{
			component.enabled = false;
		}
		MeshCollider component2 = baseEntity.GetComponent<MeshCollider>();
		if (component2)
		{
			component2.convex = true;
		}
		baseEntity.SetParent(this, false, false);
	}

	// Token: 0x060023EF RID: 9199 RVA: 0x000E2B54 File Offset: 0x000E0D54
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (!base.HasDriver())
		{
			this.NoDriverInput();
		}
		this.ConvertInputToThrottle();
		this.DoSteering();
		this.ApplyForceAtWheels();
		base.SetFlag(BaseEntity.Flags.Reserved1, base.HasDriver(), false, true);
		base.SetFlag(BaseEntity.Flags.Reserved2, base.HasDriver() && this.lightsOn, false, true);
	}

	// Token: 0x060023F0 RID: 9200 RVA: 0x000E2BB8 File Offset: 0x000E0DB8
	private void DoSteering()
	{
		foreach (BasicCar.VehicleWheel vehicleWheel in this.wheels)
		{
			if (vehicleWheel.steerWheel)
			{
				vehicleWheel.wheelCollider.steerAngle = this.steering;
			}
		}
		base.SetFlag(BaseEntity.Flags.Reserved4, this.steering < -2f, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved5, this.steering > 2f, false, true);
	}

	// Token: 0x060023F1 RID: 9201 RVA: 0x000059DD File Offset: 0x00003BDD
	public void ConvertInputToThrottle()
	{
	}

	// Token: 0x060023F2 RID: 9202 RVA: 0x000E2C2C File Offset: 0x000E0E2C
	private void ApplyForceAtWheels()
	{
		if (this.rigidBody == null)
		{
			return;
		}
		Vector3 velocity = this.rigidBody.velocity;
		float num = velocity.magnitude * Vector3.Dot(velocity.normalized, base.transform.forward);
		float num2 = this.brakePedal;
		float num3 = this.gasPedal;
		if (num > 0f && num3 < 0f)
		{
			num2 = 100f;
		}
		else if (num < 0f && num3 > 0f)
		{
			num2 = 100f;
		}
		foreach (BasicCar.VehicleWheel vehicleWheel in this.wheels)
		{
			if (vehicleWheel.wheelCollider.isGrounded)
			{
				if (vehicleWheel.powerWheel)
				{
					vehicleWheel.wheelCollider.motorTorque = num3 * this.motorForceConstant;
				}
				if (vehicleWheel.brakeWheel)
				{
					vehicleWheel.wheelCollider.brakeTorque = num2 * this.brakeForceConstant;
				}
			}
		}
		base.SetFlag(BaseEntity.Flags.Reserved3, num2 >= 100f && this.AnyMounted(), false, true);
	}

	// Token: 0x060023F3 RID: 9203 RVA: 0x000E2D38 File Offset: 0x000E0F38
	public void NoDriverInput()
	{
		if (BasicCar.chairtest)
		{
			this.gasPedal = Mathf.Sin(Time.time) * 50f;
			return;
		}
		this.gasPedal = 0f;
		this.brakePedal = Mathf.Lerp(this.brakePedal, 100f, Time.deltaTime * this.GasLerpTime / 5f);
	}

	// Token: 0x060023F4 RID: 9204 RVA: 0x000E2D96 File Offset: 0x000E0F96
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		if (base.IsDriver(player))
		{
			this.DriverInput(inputState, player);
		}
	}

	// Token: 0x060023F5 RID: 9205 RVA: 0x000E2DAC File Offset: 0x000E0FAC
	public void DriverInput(InputState inputState, BasePlayer player)
	{
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			this.gasPedal = 100f;
			this.brakePedal = 0f;
		}
		else if (inputState.IsDown(BUTTON.BACKWARD))
		{
			this.gasPedal = -30f;
			this.brakePedal = 0f;
		}
		else
		{
			this.gasPedal = 0f;
			this.brakePedal = 30f;
		}
		if (inputState.IsDown(BUTTON.LEFT))
		{
			this.steering = -60f;
			return;
		}
		if (inputState.IsDown(BUTTON.RIGHT))
		{
			this.steering = 60f;
			return;
		}
		this.steering = 0f;
	}

	// Token: 0x060023F6 RID: 9206 RVA: 0x000E2E47 File Offset: 0x000E1047
	public override void LightToggle(BasePlayer player)
	{
		if (base.IsDriver(player))
		{
			this.lightsOn = !this.lightsOn;
		}
	}

	// Token: 0x04001C75 RID: 7285
	public BasicCar.VehicleWheel[] wheels;

	// Token: 0x04001C76 RID: 7286
	public float brakePedal;

	// Token: 0x04001C77 RID: 7287
	public float gasPedal;

	// Token: 0x04001C78 RID: 7288
	public float steering;

	// Token: 0x04001C79 RID: 7289
	public Transform centerOfMass;

	// Token: 0x04001C7A RID: 7290
	public Transform steeringWheel;

	// Token: 0x04001C7B RID: 7291
	public float motorForceConstant = 150f;

	// Token: 0x04001C7C RID: 7292
	public float brakeForceConstant = 500f;

	// Token: 0x04001C7D RID: 7293
	public float GasLerpTime = 20f;

	// Token: 0x04001C7E RID: 7294
	public float SteeringLerpTime = 20f;

	// Token: 0x04001C7F RID: 7295
	public Transform driverEye;

	// Token: 0x04001C80 RID: 7296
	public GameObjectRef chairRef;

	// Token: 0x04001C81 RID: 7297
	public Transform chairAnchorTest;

	// Token: 0x04001C82 RID: 7298
	public SoundPlayer idleLoopPlayer;

	// Token: 0x04001C83 RID: 7299
	public Transform engineOffset;

	// Token: 0x04001C84 RID: 7300
	public SoundDefinition engineSoundDef;

	// Token: 0x04001C85 RID: 7301
	private static bool chairtest;

	// Token: 0x04001C86 RID: 7302
	private float throttle;

	// Token: 0x04001C87 RID: 7303
	private float brake;

	// Token: 0x04001C88 RID: 7304
	private bool lightsOn = true;

	// Token: 0x02000CA1 RID: 3233
	[Serializable]
	public class VehicleWheel
	{
		// Token: 0x04004342 RID: 17218
		public Transform shock;

		// Token: 0x04004343 RID: 17219
		public WheelCollider wheelCollider;

		// Token: 0x04004344 RID: 17220
		public Transform wheel;

		// Token: 0x04004345 RID: 17221
		public Transform axle;

		// Token: 0x04004346 RID: 17222
		public bool steerWheel;

		// Token: 0x04004347 RID: 17223
		public bool brakeWheel = true;

		// Token: 0x04004348 RID: 17224
		public bool powerWheel = true;
	}
}
