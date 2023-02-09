using System;
using UnityEngine;

// Token: 0x0200002A RID: 42
public class sedanAnimation : MonoBehaviour
{
	// Token: 0x0600010B RID: 267 RVA: 0x0000754E File Offset: 0x0000574E
	private void Start()
	{
		this.myRigidbody = base.GetComponent<Rigidbody>();
	}

	// Token: 0x0600010C RID: 268 RVA: 0x0000755C File Offset: 0x0000575C
	private void Update()
	{
		this.DoSteering();
		this.ApplyForceAtWheels();
		this.UpdateTireAnimation();
		this.InputPlayer();
	}

	// Token: 0x0600010D RID: 269 RVA: 0x00007578 File Offset: 0x00005778
	private void InputPlayer()
	{
		if (Input.GetKey(KeyCode.W))
		{
			this.gasPedal = Mathf.Clamp(this.gasPedal + Time.deltaTime * this.GasLerpTime, -100f, 100f);
			this.brakePedal = Mathf.Lerp(this.brakePedal, 0f, Time.deltaTime * this.GasLerpTime);
		}
		else if (Input.GetKey(KeyCode.S))
		{
			this.gasPedal = Mathf.Clamp(this.gasPedal - Time.deltaTime * this.GasLerpTime, -100f, 100f);
			this.brakePedal = Mathf.Lerp(this.brakePedal, 0f, Time.deltaTime * this.GasLerpTime);
		}
		else
		{
			this.gasPedal = Mathf.Lerp(this.gasPedal, 0f, Time.deltaTime * this.GasLerpTime);
			this.brakePedal = Mathf.Lerp(this.brakePedal, 100f, Time.deltaTime * this.GasLerpTime / 5f);
		}
		if (Input.GetKey(KeyCode.A))
		{
			this.steering = Mathf.Clamp(this.steering - Time.deltaTime * this.SteeringLerpTime, -60f, 60f);
			return;
		}
		if (Input.GetKey(KeyCode.D))
		{
			this.steering = Mathf.Clamp(this.steering + Time.deltaTime * this.SteeringLerpTime, -60f, 60f);
			return;
		}
		this.steering = Mathf.Lerp(this.steering, 0f, Time.deltaTime * this.SteeringLerpTime);
	}

	// Token: 0x0600010E RID: 270 RVA: 0x00007702 File Offset: 0x00005902
	private void DoSteering()
	{
		this.FL_wheelCollider.steerAngle = this.steering;
		this.FR_wheelCollider.steerAngle = this.steering;
	}

	// Token: 0x0600010F RID: 271 RVA: 0x00007728 File Offset: 0x00005928
	private void ApplyForceAtWheels()
	{
		if (this.FL_wheelCollider.isGrounded)
		{
			this.FL_wheelCollider.motorTorque = this.gasPedal * this.motorForceConstant;
			this.FL_wheelCollider.brakeTorque = this.brakePedal * this.brakeForceConstant;
		}
		if (this.FR_wheelCollider.isGrounded)
		{
			this.FR_wheelCollider.motorTorque = this.gasPedal * this.motorForceConstant;
			this.FR_wheelCollider.brakeTorque = this.brakePedal * this.brakeForceConstant;
		}
		if (this.RL_wheelCollider.isGrounded)
		{
			this.RL_wheelCollider.motorTorque = this.gasPedal * this.motorForceConstant;
			this.RL_wheelCollider.brakeTorque = this.brakePedal * this.brakeForceConstant;
		}
		if (this.RR_wheelCollider.isGrounded)
		{
			this.RR_wheelCollider.motorTorque = this.gasPedal * this.motorForceConstant;
			this.RR_wheelCollider.brakeTorque = this.brakePedal * this.brakeForceConstant;
		}
	}

	// Token: 0x06000110 RID: 272 RVA: 0x0000782C File Offset: 0x00005A2C
	private void UpdateTireAnimation()
	{
		float num = Vector3.Dot(this.myRigidbody.velocity, this.myRigidbody.transform.forward);
		if (this.FL_wheelCollider.isGrounded)
		{
			this.FL_shock.localPosition = new Vector3(this.FL_shock.localPosition.x, this.shockRestingPosY + this.GetShockHeightDelta(this.FL_wheelCollider), this.FL_shock.localPosition.z);
			this.FL_wheel.localEulerAngles = new Vector3(this.FL_wheel.localEulerAngles.x, this.FL_wheel.localEulerAngles.y, this.FL_wheel.localEulerAngles.z - num * Time.deltaTime * this.wheelSpinConstant);
		}
		else
		{
			this.FL_shock.localPosition = Vector3.Lerp(this.FL_shock.localPosition, new Vector3(this.FL_shock.localPosition.x, this.shockRestingPosY, this.FL_shock.localPosition.z), Time.deltaTime * 2f);
		}
		if (this.FR_wheelCollider.isGrounded)
		{
			this.FR_shock.localPosition = new Vector3(this.FR_shock.localPosition.x, this.shockRestingPosY + this.GetShockHeightDelta(this.FR_wheelCollider), this.FR_shock.localPosition.z);
			this.FR_wheel.localEulerAngles = new Vector3(this.FR_wheel.localEulerAngles.x, this.FR_wheel.localEulerAngles.y, this.FR_wheel.localEulerAngles.z - num * Time.deltaTime * this.wheelSpinConstant);
		}
		else
		{
			this.FR_shock.localPosition = Vector3.Lerp(this.FR_shock.localPosition, new Vector3(this.FR_shock.localPosition.x, this.shockRestingPosY, this.FR_shock.localPosition.z), Time.deltaTime * 2f);
		}
		if (this.RL_wheelCollider.isGrounded)
		{
			this.RL_shock.localPosition = new Vector3(this.RL_shock.localPosition.x, this.shockRestingPosY + this.GetShockHeightDelta(this.RL_wheelCollider), this.RL_shock.localPosition.z);
			this.RL_wheel.localEulerAngles = new Vector3(this.RL_wheel.localEulerAngles.x, this.RL_wheel.localEulerAngles.y, this.RL_wheel.localEulerAngles.z - num * Time.deltaTime * this.wheelSpinConstant);
		}
		else
		{
			this.RL_shock.localPosition = Vector3.Lerp(this.RL_shock.localPosition, new Vector3(this.RL_shock.localPosition.x, this.shockRestingPosY, this.RL_shock.localPosition.z), Time.deltaTime * 2f);
		}
		if (this.RR_wheelCollider.isGrounded)
		{
			this.RR_shock.localPosition = new Vector3(this.RR_shock.localPosition.x, this.shockRestingPosY + this.GetShockHeightDelta(this.RR_wheelCollider), this.RR_shock.localPosition.z);
			this.RR_wheel.localEulerAngles = new Vector3(this.RR_wheel.localEulerAngles.x, this.RR_wheel.localEulerAngles.y, this.RR_wheel.localEulerAngles.z - num * Time.deltaTime * this.wheelSpinConstant);
		}
		else
		{
			this.RR_shock.localPosition = Vector3.Lerp(this.RR_shock.localPosition, new Vector3(this.RR_shock.localPosition.x, this.shockRestingPosY, this.RR_shock.localPosition.z), Time.deltaTime * 2f);
		}
		foreach (Transform transform in this.frontAxles)
		{
			transform.localEulerAngles = new Vector3(this.steering, transform.localEulerAngles.y, transform.localEulerAngles.z);
		}
	}

	// Token: 0x06000111 RID: 273 RVA: 0x00007C70 File Offset: 0x00005E70
	private float GetShockHeightDelta(WheelCollider wheel)
	{
		int mask = LayerMask.GetMask(new string[]
		{
			"Terrain",
			"World",
			"Construction"
		});
		RaycastHit raycastHit;
		Physics.Linecast(wheel.transform.position, wheel.transform.position - Vector3.up * 10f, out raycastHit, mask);
		return Mathx.RemapValClamped(raycastHit.distance, this.traceDistanceNeutralPoint - this.shockDistance, this.traceDistanceNeutralPoint + this.shockDistance, this.shockDistance * 0.75f, -0.75f * this.shockDistance);
	}

	// Token: 0x0400015C RID: 348
	public Transform[] frontAxles;

	// Token: 0x0400015D RID: 349
	public Transform FL_shock;

	// Token: 0x0400015E RID: 350
	public Transform FL_wheel;

	// Token: 0x0400015F RID: 351
	public Transform FR_shock;

	// Token: 0x04000160 RID: 352
	public Transform FR_wheel;

	// Token: 0x04000161 RID: 353
	public Transform RL_shock;

	// Token: 0x04000162 RID: 354
	public Transform RL_wheel;

	// Token: 0x04000163 RID: 355
	public Transform RR_shock;

	// Token: 0x04000164 RID: 356
	public Transform RR_wheel;

	// Token: 0x04000165 RID: 357
	public WheelCollider FL_wheelCollider;

	// Token: 0x04000166 RID: 358
	public WheelCollider FR_wheelCollider;

	// Token: 0x04000167 RID: 359
	public WheelCollider RL_wheelCollider;

	// Token: 0x04000168 RID: 360
	public WheelCollider RR_wheelCollider;

	// Token: 0x04000169 RID: 361
	public Transform steeringWheel;

	// Token: 0x0400016A RID: 362
	public float motorForceConstant = 150f;

	// Token: 0x0400016B RID: 363
	public float brakeForceConstant = 500f;

	// Token: 0x0400016C RID: 364
	public float brakePedal;

	// Token: 0x0400016D RID: 365
	public float gasPedal;

	// Token: 0x0400016E RID: 366
	public float steering;

	// Token: 0x0400016F RID: 367
	private Rigidbody myRigidbody;

	// Token: 0x04000170 RID: 368
	public float GasLerpTime = 20f;

	// Token: 0x04000171 RID: 369
	public float SteeringLerpTime = 20f;

	// Token: 0x04000172 RID: 370
	private float wheelSpinConstant = 120f;

	// Token: 0x04000173 RID: 371
	private float shockRestingPosY = -0.27f;

	// Token: 0x04000174 RID: 372
	private float shockDistance = 0.3f;

	// Token: 0x04000175 RID: 373
	private float traceDistanceNeutralPoint = 0.7f;
}
