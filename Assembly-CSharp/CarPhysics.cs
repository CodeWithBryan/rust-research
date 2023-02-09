using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000451 RID: 1105
public class CarPhysics<TCar> where TCar : BaseVehicle, CarPhysics<TCar>.ICar
{
	// Token: 0x170002BF RID: 703
	// (get) Token: 0x06002424 RID: 9252 RVA: 0x000E43F8 File Offset: 0x000E25F8
	// (set) Token: 0x06002425 RID: 9253 RVA: 0x000E4400 File Offset: 0x000E2600
	public float DriveWheelVelocity { get; private set; }

	// Token: 0x170002C0 RID: 704
	// (get) Token: 0x06002426 RID: 9254 RVA: 0x000E4409 File Offset: 0x000E2609
	// (set) Token: 0x06002427 RID: 9255 RVA: 0x000E4411 File Offset: 0x000E2611
	public float DriveWheelSlip { get; private set; }

	// Token: 0x170002C1 RID: 705
	// (get) Token: 0x06002428 RID: 9256 RVA: 0x000E441A File Offset: 0x000E261A
	// (set) Token: 0x06002429 RID: 9257 RVA: 0x000E4422 File Offset: 0x000E2622
	public float SteerAngle { get; private set; }

	// Token: 0x170002C2 RID: 706
	// (get) Token: 0x0600242A RID: 9258 RVA: 0x000E442B File Offset: 0x000E262B
	// (set) Token: 0x0600242B RID: 9259 RVA: 0x000E4433 File Offset: 0x000E2633
	public float TankThrottleLeft { get; private set; }

	// Token: 0x170002C3 RID: 707
	// (get) Token: 0x0600242C RID: 9260 RVA: 0x000E443C File Offset: 0x000E263C
	// (set) Token: 0x0600242D RID: 9261 RVA: 0x000E4444 File Offset: 0x000E2644
	public float TankThrottleRight { get; private set; }

	// Token: 0x170002C4 RID: 708
	// (get) Token: 0x0600242E RID: 9262 RVA: 0x000E444D File Offset: 0x000E264D
	private bool InSlowSpeedExitMode
	{
		get
		{
			return !this.hasDriver && this.slowSpeedExitFlag;
		}
	}

	// Token: 0x0600242F RID: 9263 RVA: 0x000E4460 File Offset: 0x000E2660
	public CarPhysics(TCar car, Transform transform, Rigidbody rBody, CarSettings vehicleSettings)
	{
		CarPhysics<TCar>.<>c__DisplayClass47_0 CS$<>8__locals1;
		CS$<>8__locals1.transform = transform;
		base..ctor();
		CS$<>8__locals1.<>4__this = this;
		this.car = car;
		this.transform = CS$<>8__locals1.transform;
		this.rBody = rBody;
		this.vehicleSettings = vehicleSettings;
		this.timeSinceWaterCheck = default(TimeSince);
		this.timeSinceWaterCheck = float.MaxValue;
		this.prevLocalCOM = rBody.centerOfMass;
		CarWheel[] wheels = car.GetWheels();
		this.wheelData = new CarPhysics<TCar>.ServerWheelData[wheels.Length];
		for (int i = 0; i < this.wheelData.Length; i++)
		{
			this.wheelData[i] = this.<.ctor>g__AddWheel|47_0(wheels[i], ref CS$<>8__locals1);
		}
		this.midWheelPos = car.GetWheelsMidPos();
		this.wheelData[0].wheel.wheelCollider.ConfigureVehicleSubsteps(1000f, 1, 1);
		this.lastMovingTime = Time.realtimeSinceStartup;
	}

	// Token: 0x06002430 RID: 9264 RVA: 0x000E4574 File Offset: 0x000E2774
	public void FixedUpdate(float dt, float speed)
	{
		if (this.rBody.centerOfMass != this.prevLocalCOM)
		{
			this.COMChanged();
		}
		float num = Mathf.Abs(speed);
		this.hasDriver = this.car.HasDriver();
		if (!this.hasDriver && this.hadDriver)
		{
			if (num <= 4f)
			{
				this.slowSpeedExitFlag = true;
			}
		}
		else if (this.hasDriver && !this.hadDriver)
		{
			this.slowSpeedExitFlag = false;
		}
		if ((this.hasDriver || !this.vehicleSettings.canSleep) && this.rBody.IsSleeping())
		{
			this.rBody.WakeUp();
		}
		if (!this.rBody.IsSleeping())
		{
			if ((this.wasSleeping && !this.rBody.isKinematic) || num > 0.25f || Mathf.Abs(this.rBody.angularVelocity.magnitude) > 0.25f)
			{
				this.lastMovingTime = Time.time;
			}
			bool flag = this.vehicleSettings.canSleep && !this.hasDriver && Time.time > this.lastMovingTime + 10f;
			if (flag && (this.car.GetParentEntity() as BaseVehicle).IsValid())
			{
				flag = false;
			}
			if (flag)
			{
				for (int i = 0; i < this.wheelData.Length; i++)
				{
					CarPhysics<TCar>.ServerWheelData serverWheelData = this.wheelData[i];
					serverWheelData.wheelCollider.motorTorque = 0f;
					serverWheelData.wheelCollider.brakeTorque = 0f;
					serverWheelData.wheelCollider.steerAngle = 0f;
				}
				this.rBody.Sleep();
			}
			else
			{
				this.speedAngle = Vector3.Angle(this.rBody.velocity, this.transform.forward) * Mathf.Sign(Vector3.Dot(this.rBody.velocity, this.transform.right));
				float num2 = this.car.GetMaxDriveForce();
				float maxForwardSpeed = this.car.GetMaxForwardSpeed();
				float num3 = this.car.IsOn() ? this.car.GetThrottleInput() : 0f;
				float steerInput = this.car.GetSteerInput();
				float brakeInput = this.InSlowSpeedExitMode ? 1f : this.car.GetBrakeInput();
				float num4 = 1f;
				if (num < 3f)
				{
					num4 = 2.75f;
				}
				else if (num < 9f)
				{
					float t = Mathf.InverseLerp(9f, 3f, num);
					num4 = Mathf.Lerp(1f, 2.75f, t);
				}
				num2 *= num4;
				this.ComputeSteerAngle(num3, steerInput, dt, speed);
				if (this.timeSinceWaterCheck > 0.25f)
				{
					float a = this.car.WaterFactor();
					float b = 0f;
					TriggerVehicleDrag triggerVehicleDrag;
					if (this.car.FindTrigger<TriggerVehicleDrag>(out triggerVehicleDrag))
					{
						b = triggerVehicleDrag.vehicleDrag;
					}
					float a2 = (num3 != 0f) ? 0f : 0.25f;
					float num5 = Mathf.Max(a, b);
					num5 = Mathf.Max(num5, this.car.GetModifiedDrag());
					this.rBody.drag = Mathf.Max(a2, num5);
					this.rBody.angularDrag = num5 * 0.5f;
					this.timeSinceWaterCheck = 0f;
				}
				int num6 = 0;
				float num7 = 0f;
				bool flag2 = !this.hasDriver && this.rBody.velocity.magnitude < 2.5f && this.car.timeSinceLastPush > 2f;
				for (int j = 0; j < this.wheelData.Length; j++)
				{
					CarPhysics<TCar>.ServerWheelData serverWheelData2 = this.wheelData[j];
					serverWheelData2.wheelCollider.motorTorque = 1E-05f;
					if (flag2 && this.car.OnSurface != VehicleTerrainHandler.Surface.Frictionless)
					{
						serverWheelData2.wheelCollider.brakeTorque = 10000f;
					}
					else
					{
						serverWheelData2.wheelCollider.brakeTorque = 0f;
					}
					if (serverWheelData2.wheel.steerWheel)
					{
						serverWheelData2.wheel.wheelCollider.steerAngle = (serverWheelData2.isFrontWheel ? this.SteerAngle : (this.vehicleSettings.rearWheelSteer * -this.SteerAngle));
					}
					this.UpdateSuspension(serverWheelData2);
					if (serverWheelData2.isGrounded)
					{
						num6++;
						num7 += this.wheelData[j].downforce;
					}
				}
				this.AdjustHitForces(num6, num7 / (float)num6);
				for (int k = 0; k < this.wheelData.Length; k++)
				{
					CarPhysics<TCar>.ServerWheelData wd = this.wheelData[k];
					this.UpdateLocalFrame(wd, dt);
					this.ComputeTyreForces(wd, speed, num2, maxForwardSpeed, num3, steerInput, brakeInput, num4);
					this.ApplyTyreForces(wd, num3, steerInput, speed);
				}
				this.ComputeOverallForces();
			}
			this.wasSleeping = false;
		}
		else
		{
			this.wasSleeping = true;
		}
		this.hadDriver = this.hasDriver;
	}

	// Token: 0x06002431 RID: 9265 RVA: 0x000E4AA4 File Offset: 0x000E2CA4
	public bool IsGrounded()
	{
		int num = 0;
		for (int i = 0; i < this.wheelData.Length; i++)
		{
			if (this.wheelData[i].isGrounded)
			{
				num++;
			}
			if (num >= 2)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002432 RID: 9266 RVA: 0x000E4AE0 File Offset: 0x000E2CE0
	private void COMChanged()
	{
		for (int i = 0; i < this.wheelData.Length; i++)
		{
			CarPhysics<TCar>.ServerWheelData serverWheelData = this.wheelData[i];
			serverWheelData.forceDistance = this.GetWheelForceDistance(serverWheelData.wheel.wheelCollider);
		}
		this.prevLocalCOM = this.rBody.centerOfMass;
	}

	// Token: 0x06002433 RID: 9267 RVA: 0x000E4B34 File Offset: 0x000E2D34
	private void ComputeSteerAngle(float throttleInput, float steerInput, float dt, float speed)
	{
		if (this.vehicleSettings.tankSteering)
		{
			this.SteerAngle = 0f;
			this.ComputeTankSteeringThrottle(throttleInput, steerInput, speed);
			return;
		}
		float num = this.vehicleSettings.maxSteerAngle * steerInput;
		float num2 = Mathf.InverseLerp(0f, this.vehicleSettings.minSteerLimitSpeed, speed);
		if (this.vehicleSettings.steeringLimit)
		{
			float num3 = Mathf.Lerp(this.vehicleSettings.maxSteerAngle, this.vehicleSettings.minSteerLimitAngle, num2);
			num = Mathf.Clamp(num, -num3, num3);
		}
		float num4 = 0f;
		if (this.vehicleSettings.steeringAssist)
		{
			float num5 = Mathf.InverseLerp(0.1f, 3f, speed);
			num4 = this.speedAngle * this.vehicleSettings.steeringAssistRatio * num5 * Mathf.InverseLerp(2f, 3f, Mathf.Abs(this.speedAngle));
		}
		float num6 = Mathf.Clamp(num + num4, -this.vehicleSettings.maxSteerAngle, this.vehicleSettings.maxSteerAngle);
		if (this.SteerAngle != num6)
		{
			float num7 = 1f - num2 * 0.7f;
			float num9;
			if ((this.SteerAngle == 0f || Mathf.Sign(num6) == Mathf.Sign(this.SteerAngle)) && Mathf.Abs(num6) > Mathf.Abs(this.SteerAngle))
			{
				float num8 = this.SteerAngle / this.vehicleSettings.maxSteerAngle;
				num9 = Mathf.Lerp(this.vehicleSettings.steerMinLerpSpeed * num7, this.vehicleSettings.steerMaxLerpSpeed * num7, num8 * num8);
			}
			else
			{
				num9 = this.vehicleSettings.steerReturnLerpSpeed * num7;
			}
			if (this.car.GetSteerModInput())
			{
				num9 *= 1.5f;
			}
			this.SteerAngle = Mathf.MoveTowards(this.SteerAngle, num6, dt * num9);
		}
	}

	// Token: 0x06002434 RID: 9268 RVA: 0x000E4D08 File Offset: 0x000E2F08
	private float GetWheelForceDistance(WheelCollider col)
	{
		return this.rBody.centerOfMass.y - this.transform.InverseTransformPoint(col.transform.position).y + col.radius + (1f - col.suspensionSpring.targetPosition) * col.suspensionDistance;
	}

	// Token: 0x06002435 RID: 9269 RVA: 0x000E4D64 File Offset: 0x000E2F64
	private void UpdateSuspension(CarPhysics<TCar>.ServerWheelData wd)
	{
		wd.isGrounded = wd.wheelCollider.GetGroundHit(out wd.hit);
		wd.origin = wd.wheelColliderTransform.TransformPoint(wd.wheelCollider.center);
		RaycastHit raycastHit;
		if (wd.isGrounded && GamePhysics.Trace(new Ray(wd.origin, -wd.wheelColliderTransform.up), 0f, out raycastHit, wd.wheelCollider.suspensionDistance + wd.wheelCollider.radius, 1235321089, QueryTriggerInteraction.Ignore, null))
		{
			wd.hit.point = raycastHit.point;
			wd.hit.normal = raycastHit.normal;
		}
		if (wd.isGrounded)
		{
			if (wd.hit.force < 0f)
			{
				wd.hit.force = 0f;
			}
			wd.downforce = wd.hit.force;
			return;
		}
		wd.downforce = 0f;
	}

	// Token: 0x06002436 RID: 9270 RVA: 0x000E4E60 File Offset: 0x000E3060
	private void AdjustHitForces(int groundedWheels, float neutralForcePerWheel)
	{
		float num = neutralForcePerWheel * 0.25f;
		for (int i = 0; i < this.wheelData.Length; i++)
		{
			CarPhysics<TCar>.ServerWheelData serverWheelData = this.wheelData[i];
			if (serverWheelData.isGrounded && serverWheelData.downforce < num)
			{
				if (groundedWheels == 1)
				{
					serverWheelData.downforce = num;
				}
				else
				{
					float a = (num - serverWheelData.downforce) / (float)(groundedWheels - 1);
					serverWheelData.downforce = num;
					for (int j = 0; j < this.wheelData.Length; j++)
					{
						CarPhysics<TCar>.ServerWheelData serverWheelData2 = this.wheelData[j];
						if (serverWheelData2.isGrounded && serverWheelData2.downforce > num)
						{
							float num2 = Mathf.Min(a, serverWheelData2.downforce - num);
							serverWheelData2.downforce -= num2;
						}
					}
				}
			}
		}
	}

	// Token: 0x06002437 RID: 9271 RVA: 0x000E4F24 File Offset: 0x000E3124
	private void UpdateLocalFrame(CarPhysics<TCar>.ServerWheelData wd, float dt)
	{
		if (!wd.isGrounded)
		{
			wd.hit.point = wd.origin - wd.wheelColliderTransform.up * (wd.wheelCollider.suspensionDistance + wd.wheelCollider.radius);
			wd.hit.normal = wd.wheelColliderTransform.up;
			wd.hit.collider = null;
		}
		Vector3 pointVelocity = this.rBody.GetPointVelocity(wd.hit.point);
		wd.velocity = pointVelocity - Vector3.Project(pointVelocity, wd.hit.normal);
		wd.localVelocity.y = Vector3.Dot(wd.hit.forwardDir, wd.velocity);
		wd.localVelocity.x = Vector3.Dot(wd.hit.sidewaysDir, wd.velocity);
		if (!wd.isGrounded)
		{
			wd.localRigForce = Vector2.zero;
			return;
		}
		float num = Mathf.InverseLerp(1f, 0.25f, wd.velocity.sqrMagnitude);
		Vector2 vector2;
		if (num > 0f)
		{
			float num2 = Vector3.Dot(Vector3.up, wd.hit.normal);
			Vector3 rhs;
			if (num2 > 1E-06f)
			{
				Vector3 vector = Vector3.up * wd.downforce / num2;
				rhs = vector - Vector3.Project(vector, wd.hit.normal);
			}
			else
			{
				rhs = Vector3.up * 100000f;
			}
			vector2.y = Vector3.Dot(wd.hit.forwardDir, rhs);
			vector2.x = Vector3.Dot(wd.hit.sidewaysDir, rhs);
			vector2 *= num;
		}
		else
		{
			vector2 = Vector2.zero;
		}
		Vector2 a = -(Mathf.Clamp(wd.downforce / -Physics.gravity.y, 0f, wd.wheelCollider.sprungMass) * 0.5f) * wd.localVelocity / dt;
		wd.localRigForce = a + vector2;
	}

	// Token: 0x06002438 RID: 9272 RVA: 0x000E513C File Offset: 0x000E333C
	private void ComputeTyreForces(CarPhysics<TCar>.ServerWheelData wd, float speed, float maxDriveForce, float maxSpeed, float throttleInput, float steerInput, float brakeInput, float driveForceMultiplier)
	{
		float absSpeed = Mathf.Abs(speed);
		if (this.vehicleSettings.tankSteering && brakeInput == 0f)
		{
			if (wd.isLeftWheel)
			{
				throttleInput = this.TankThrottleLeft;
			}
			else
			{
				throttleInput = this.TankThrottleRight;
			}
		}
		float num = wd.wheel.powerWheel ? throttleInput : 0f;
		wd.hasThrottleInput = (num != 0f);
		float num2 = this.vehicleSettings.maxDriveSlip;
		if (Mathf.Sign(num) != Mathf.Sign(wd.localVelocity.y))
		{
			num2 -= wd.localVelocity.y * Mathf.Sign(num);
		}
		float num3 = Mathf.Abs(num);
		float num4 = -this.vehicleSettings.rollingResistance + num3 * (1f + this.vehicleSettings.rollingResistance) - brakeInput * (1f - this.vehicleSettings.rollingResistance);
		if (this.InSlowSpeedExitMode || num4 < 0f || maxDriveForce == 0f)
		{
			num4 *= -1f;
			wd.isBraking = true;
		}
		else
		{
			num4 *= Mathf.Sign(num);
			wd.isBraking = false;
		}
		float num6;
		if (wd.isBraking)
		{
			float num5 = Mathf.Clamp(this.car.GetMaxForwardSpeed() * this.vehicleSettings.brakeForceMultiplier, 10f * this.vehicleSettings.brakeForceMultiplier, 50f * this.vehicleSettings.brakeForceMultiplier);
			num5 += this.rBody.mass * 1.5f;
			num6 = num4 * num5;
		}
		else
		{
			num6 = this.ComputeDriveForce(speed, absSpeed, num4 * maxDriveForce, maxDriveForce, maxSpeed, driveForceMultiplier);
		}
		if (wd.isGrounded)
		{
			wd.tyreSlip.x = wd.localVelocity.x;
			wd.tyreSlip.y = wd.localVelocity.y - wd.angularVelocity * wd.wheelCollider.radius;
			float num7;
			switch (this.car.OnSurface)
			{
			case VehicleTerrainHandler.Surface.Road:
				num7 = 1f;
				goto IL_230;
			case VehicleTerrainHandler.Surface.Ice:
				num7 = 0.25f;
				goto IL_230;
			case VehicleTerrainHandler.Surface.Frictionless:
				num7 = 0f;
				goto IL_230;
			}
			num7 = 0.75f;
			IL_230:
			float num8 = wd.wheel.tyreFriction * wd.downforce * num7;
			float num9 = 0f;
			if (!wd.isBraking)
			{
				num9 = Mathf.Min(Mathf.Abs(num6 * wd.tyreSlip.x) / num8, num2);
				if (num6 != 0f && num9 < 0.1f)
				{
					num9 = 0.1f;
				}
			}
			if (Mathf.Abs(wd.tyreSlip.y) < num9)
			{
				wd.tyreSlip.y = num9 * Mathf.Sign(wd.tyreSlip.y);
			}
			Vector2 vector = -num8 * wd.tyreSlip.normalized;
			vector.x = Mathf.Abs(vector.x) * 1.5f;
			vector.y = Mathf.Abs(vector.y);
			wd.tyreForce.x = Mathf.Clamp(wd.localRigForce.x, -vector.x, vector.x);
			if (wd.isBraking)
			{
				float num10 = Mathf.Min(vector.y, num6);
				wd.tyreForce.y = Mathf.Clamp(wd.localRigForce.y, -num10, num10);
			}
			else
			{
				wd.tyreForce.y = Mathf.Clamp(num6, -vector.y, vector.y);
			}
		}
		else
		{
			wd.tyreSlip = Vector2.zero;
			wd.tyreForce = Vector2.zero;
		}
		if (wd.isGrounded)
		{
			float num11;
			if (wd.isBraking)
			{
				num11 = 0f;
			}
			else
			{
				float driveForceToMaxSlip = this.vehicleSettings.driveForceToMaxSlip;
				num11 = Mathf.Clamp01((Mathf.Abs(num6) - Mathf.Abs(wd.tyreForce.y)) / driveForceToMaxSlip) * num2 * Mathf.Sign(num6);
			}
			wd.angularVelocity = (wd.localVelocity.y + num11) / wd.wheelCollider.radius;
			return;
		}
		float num12 = 50f;
		float num13 = 10f;
		if (num > 0f)
		{
			wd.angularVelocity += num12 * num;
		}
		else
		{
			wd.angularVelocity -= num13;
		}
		wd.angularVelocity -= num12 * brakeInput;
		wd.angularVelocity = Mathf.Clamp(wd.angularVelocity, 0f, maxSpeed / wd.wheelCollider.radius);
	}

	// Token: 0x06002439 RID: 9273 RVA: 0x000E55C8 File Offset: 0x000E37C8
	private void ComputeTankSteeringThrottle(float throttleInput, float steerInput, float speed)
	{
		this.TankThrottleLeft = throttleInput;
		this.TankThrottleRight = throttleInput;
		float tankSteerInvert = this.GetTankSteerInvert(throttleInput, speed);
		if (throttleInput == 0f)
		{
			this.TankThrottleLeft = -steerInput;
			this.TankThrottleRight = steerInput;
			return;
		}
		if (steerInput > 0f)
		{
			this.TankThrottleLeft = Mathf.Lerp(throttleInput, -1f * tankSteerInvert, steerInput);
			this.TankThrottleRight = Mathf.Lerp(throttleInput, 1f * tankSteerInvert, steerInput);
			return;
		}
		if (steerInput < 0f)
		{
			this.TankThrottleLeft = Mathf.Lerp(throttleInput, 1f * tankSteerInvert, -steerInput);
			this.TankThrottleRight = Mathf.Lerp(throttleInput, -1f * tankSteerInvert, -steerInput);
		}
	}

	// Token: 0x0600243A RID: 9274 RVA: 0x000E5668 File Offset: 0x000E3868
	private float ComputeDriveForce(float speed, float absSpeed, float demandedForce, float maxForce, float maxForwardSpeed, float driveForceMultiplier)
	{
		float num = (speed >= 0f) ? maxForwardSpeed : (maxForwardSpeed * this.vehicleSettings.reversePercentSpeed);
		if (absSpeed < num)
		{
			if ((speed >= 0f || demandedForce <= 0f) && (speed <= 0f || demandedForce >= 0f))
			{
				maxForce = this.car.GetAdjustedDriveForce(absSpeed, maxForwardSpeed) * driveForceMultiplier;
			}
			return Mathf.Clamp(demandedForce, -maxForce, maxForce);
		}
		float num2 = maxForce * Mathf.Max(1f - absSpeed / num, -1f) * Mathf.Sign(speed);
		if ((speed < 0f && demandedForce > 0f) || (speed > 0f && demandedForce < 0f))
		{
			num2 = Mathf.Clamp(num2 + demandedForce, -maxForce, maxForce);
		}
		return num2;
	}

	// Token: 0x0600243B RID: 9275 RVA: 0x000E5728 File Offset: 0x000E3928
	private void ComputeOverallForces()
	{
		this.DriveWheelVelocity = 0f;
		this.DriveWheelSlip = 0f;
		int num = 0;
		for (int i = 0; i < this.wheelData.Length; i++)
		{
			CarPhysics<TCar>.ServerWheelData serverWheelData = this.wheelData[i];
			if (serverWheelData.wheel.powerWheel)
			{
				this.DriveWheelVelocity += serverWheelData.angularVelocity;
				if (serverWheelData.isGrounded)
				{
					float num2 = CarPhysics<TCar>.ComputeCombinedSlip(serverWheelData.localVelocity, serverWheelData.tyreSlip);
					this.DriveWheelSlip += num2;
				}
				num++;
			}
		}
		if (num > 0)
		{
			this.DriveWheelVelocity /= (float)num;
			this.DriveWheelSlip /= (float)num;
		}
	}

	// Token: 0x0600243C RID: 9276 RVA: 0x000E57D8 File Offset: 0x000E39D8
	private static float ComputeCombinedSlip(Vector2 localVelocity, Vector2 tyreSlip)
	{
		float magnitude = localVelocity.magnitude;
		if (magnitude > 0.01f)
		{
			float num = tyreSlip.x * localVelocity.x / magnitude;
			float y = tyreSlip.y;
			return Mathf.Sqrt(num * num + y * y);
		}
		return tyreSlip.magnitude;
	}

	// Token: 0x0600243D RID: 9277 RVA: 0x000E5820 File Offset: 0x000E3A20
	private void ApplyTyreForces(CarPhysics<TCar>.ServerWheelData wd, float throttleInput, float steerInput, float speed)
	{
		if (wd.isGrounded)
		{
			Vector3 force = wd.hit.forwardDir * wd.tyreForce.y;
			Vector3 force2 = wd.hit.sidewaysDir * wd.tyreForce.x;
			Vector3 sidewaysForceAppPoint = this.GetSidewaysForceAppPoint(wd, wd.hit.point);
			this.rBody.AddForceAtPosition(force, wd.hit.point, ForceMode.Force);
			this.rBody.AddForceAtPosition(force2, sidewaysForceAppPoint, ForceMode.Force);
		}
	}

	// Token: 0x0600243E RID: 9278 RVA: 0x000E58A8 File Offset: 0x000E3AA8
	private Vector3 GetSidewaysForceAppPoint(CarPhysics<TCar>.ServerWheelData wd, Vector3 contactPoint)
	{
		Vector3 vector = contactPoint + wd.wheelColliderTransform.up * this.vehicleSettings.antiRoll * wd.forceDistance;
		float num = wd.wheel.steerWheel ? this.SteerAngle : 0f;
		if (num != 0f && Mathf.Sign(num) != Mathf.Sign(wd.tyreSlip.x))
		{
			vector += wd.wheelColliderTransform.forward * this.midWheelPos * (this.vehicleSettings.handlingBias - 0.5f);
		}
		return vector;
	}

	// Token: 0x0600243F RID: 9279 RVA: 0x000E5954 File Offset: 0x000E3B54
	private float GetTankSteerInvert(float throttleInput, float speed)
	{
		float result = 1f;
		if (throttleInput < 0f && speed < 1.75f)
		{
			result = -1f;
		}
		else if (throttleInput == 0f && speed < -1f)
		{
			result = -1f;
		}
		else if (speed < -1f)
		{
			result = -1f;
		}
		return result;
	}

	// Token: 0x06002440 RID: 9280 RVA: 0x000E59A8 File Offset: 0x000E3BA8
	[CompilerGenerated]
	private CarPhysics<TCar>.ServerWheelData <.ctor>g__AddWheel|47_0(CarWheel wheel, ref CarPhysics<TCar>.<>c__DisplayClass47_0 A_2)
	{
		CarPhysics<TCar>.ServerWheelData serverWheelData = new CarPhysics<TCar>.ServerWheelData();
		serverWheelData.wheelCollider = wheel.wheelCollider;
		serverWheelData.wheelColliderTransform = wheel.wheelCollider.transform;
		serverWheelData.forceDistance = this.GetWheelForceDistance(wheel.wheelCollider);
		serverWheelData.wheel = wheel;
		serverWheelData.wheelCollider.sidewaysFriction = this.zeroFriction;
		serverWheelData.wheelCollider.forwardFriction = this.zeroFriction;
		Vector3 vector = A_2.transform.InverseTransformPoint(wheel.wheelCollider.transform.position);
		serverWheelData.isFrontWheel = (vector.z > 0f);
		serverWheelData.isLeftWheel = (vector.x < 0f);
		return serverWheelData;
	}

	// Token: 0x04001CD1 RID: 7377
	private readonly CarPhysics<TCar>.ServerWheelData[] wheelData;

	// Token: 0x04001CD2 RID: 7378
	private readonly TCar car;

	// Token: 0x04001CD3 RID: 7379
	private readonly Transform transform;

	// Token: 0x04001CD4 RID: 7380
	private readonly Rigidbody rBody;

	// Token: 0x04001CD5 RID: 7381
	private readonly CarSettings vehicleSettings;

	// Token: 0x04001CD6 RID: 7382
	private float speedAngle;

	// Token: 0x04001CD7 RID: 7383
	private bool wasSleeping = true;

	// Token: 0x04001CD8 RID: 7384
	private bool hasDriver;

	// Token: 0x04001CD9 RID: 7385
	private bool hadDriver;

	// Token: 0x04001CDA RID: 7386
	private float lastMovingTime = float.MinValue;

	// Token: 0x04001CDB RID: 7387
	private WheelFrictionCurve zeroFriction = new WheelFrictionCurve
	{
		stiffness = 0f
	};

	// Token: 0x04001CDC RID: 7388
	private Vector3 prevLocalCOM;

	// Token: 0x04001CDD RID: 7389
	private readonly float midWheelPos;

	// Token: 0x04001CDE RID: 7390
	private const bool WHEEL_HIT_CORRECTION = true;

	// Token: 0x04001CDF RID: 7391
	private const float SLEEP_SPEED = 0.25f;

	// Token: 0x04001CE0 RID: 7392
	private const float SLEEP_DELAY = 10f;

	// Token: 0x04001CE1 RID: 7393
	private const float AIR_DRAG = 0.25f;

	// Token: 0x04001CE2 RID: 7394
	private const float DEFAULT_GROUND_GRIP = 0.75f;

	// Token: 0x04001CE3 RID: 7395
	private const float ROAD_GROUND_GRIP = 1f;

	// Token: 0x04001CE4 RID: 7396
	private const float ICE_GROUND_GRIP = 0.25f;

	// Token: 0x04001CE5 RID: 7397
	private bool slowSpeedExitFlag;

	// Token: 0x04001CE6 RID: 7398
	private const float SLOW_SPEED_EXIT_SPEED = 4f;

	// Token: 0x04001CE7 RID: 7399
	private TimeSince timeSinceWaterCheck;

	// Token: 0x02000CA3 RID: 3235
	public interface ICar
	{
		// Token: 0x1700062B RID: 1579
		// (get) Token: 0x06004D20 RID: 19744
		VehicleTerrainHandler.Surface OnSurface { get; }

		// Token: 0x06004D21 RID: 19745
		float GetThrottleInput();

		// Token: 0x06004D22 RID: 19746
		float GetBrakeInput();

		// Token: 0x06004D23 RID: 19747
		float GetSteerInput();

		// Token: 0x06004D24 RID: 19748
		bool GetSteerModInput();

		// Token: 0x06004D25 RID: 19749
		float GetMaxForwardSpeed();

		// Token: 0x06004D26 RID: 19750
		float GetMaxDriveForce();

		// Token: 0x06004D27 RID: 19751
		float GetAdjustedDriveForce(float absSpeed, float topSpeed);

		// Token: 0x06004D28 RID: 19752
		float GetModifiedDrag();

		// Token: 0x06004D29 RID: 19753
		CarWheel[] GetWheels();

		// Token: 0x06004D2A RID: 19754
		float GetWheelsMidPos();
	}

	// Token: 0x02000CA4 RID: 3236
	private class ServerWheelData
	{
		// Token: 0x0400434D RID: 17229
		public CarWheel wheel;

		// Token: 0x0400434E RID: 17230
		public Transform wheelColliderTransform;

		// Token: 0x0400434F RID: 17231
		public WheelCollider wheelCollider;

		// Token: 0x04004350 RID: 17232
		public bool isGrounded;

		// Token: 0x04004351 RID: 17233
		public float downforce;

		// Token: 0x04004352 RID: 17234
		public float forceDistance;

		// Token: 0x04004353 RID: 17235
		public WheelHit hit;

		// Token: 0x04004354 RID: 17236
		public Vector2 localRigForce;

		// Token: 0x04004355 RID: 17237
		public Vector2 localVelocity;

		// Token: 0x04004356 RID: 17238
		public float angularVelocity;

		// Token: 0x04004357 RID: 17239
		public Vector3 origin;

		// Token: 0x04004358 RID: 17240
		public Vector2 tyreForce;

		// Token: 0x04004359 RID: 17241
		public Vector2 tyreSlip;

		// Token: 0x0400435A RID: 17242
		public Vector3 velocity;

		// Token: 0x0400435B RID: 17243
		public bool isBraking;

		// Token: 0x0400435C RID: 17244
		public bool hasThrottleInput;

		// Token: 0x0400435D RID: 17245
		public bool isFrontWheel;

		// Token: 0x0400435E RID: 17246
		public bool isLeftWheel;
	}
}
