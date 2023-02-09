using System;
using UnityEngine;

// Token: 0x0200019A RID: 410
public class BradleyMoveTest : MonoBehaviour
{
	// Token: 0x06001797 RID: 6039 RVA: 0x000B009E File Offset: 0x000AE29E
	public void Awake()
	{
		this.Initialize();
	}

	// Token: 0x06001798 RID: 6040 RVA: 0x000B00A6 File Offset: 0x000AE2A6
	public void Initialize()
	{
		this.myRigidBody.centerOfMass = this.centerOfMass.localPosition;
		this.destination = base.transform.position;
	}

	// Token: 0x06001799 RID: 6041 RVA: 0x000B00CF File Offset: 0x000AE2CF
	public void SetDestination(Vector3 dest)
	{
		this.destination = dest;
	}

	// Token: 0x0600179A RID: 6042 RVA: 0x000B00D8 File Offset: 0x000AE2D8
	public void FixedUpdate()
	{
		Vector3 velocity = this.myRigidBody.velocity;
		this.SetDestination(this.followTest.transform.position);
		float num = Vector3.Distance(base.transform.position, this.destination);
		if (num > this.stoppingDist)
		{
			Vector3 zero = Vector3.zero;
			float num2 = Vector3.Dot(zero, base.transform.right);
			float num3 = Vector3.Dot(zero, -base.transform.right);
			float num4 = Vector3.Dot(zero, base.transform.right);
			if (Vector3.Dot(zero, -base.transform.forward) > num4)
			{
				if (num2 >= num3)
				{
					this.turning = 1f;
				}
				else
				{
					this.turning = -1f;
				}
			}
			else
			{
				this.turning = num4;
			}
			this.throttle = Mathf.InverseLerp(this.stoppingDist, 30f, num);
		}
		this.throttle = Mathf.Clamp(this.throttle, -1f, 1f);
		float num5 = this.throttle;
		float num6 = this.throttle;
		if (this.turning > 0f)
		{
			num6 = -this.turning;
			num5 = this.turning;
		}
		else if (this.turning < 0f)
		{
			num5 = this.turning;
			num6 = this.turning * -1f;
		}
		this.ApplyBrakes(this.brake ? 1f : 0f);
		float num7 = this.throttle;
		num5 = Mathf.Clamp(num5 + num7, -1f, 1f);
		num6 = Mathf.Clamp(num6 + num7, -1f, 1f);
		this.AdjustFriction();
		float t = Mathf.InverseLerp(3f, 1f, velocity.magnitude * Mathf.Abs(Vector3.Dot(velocity.normalized, base.transform.forward)));
		float torqueAmount = Mathf.Lerp(this.moveForceMax, this.turnForce, t);
		this.SetMotorTorque(num5, false, torqueAmount);
		this.SetMotorTorque(num6, true, torqueAmount);
	}

	// Token: 0x0600179B RID: 6043 RVA: 0x000B02DB File Offset: 0x000AE4DB
	public void ApplyBrakes(float amount)
	{
		this.ApplyBrakeTorque(amount, true);
		this.ApplyBrakeTorque(amount, false);
	}

	// Token: 0x0600179C RID: 6044 RVA: 0x000B02F0 File Offset: 0x000AE4F0
	public float GetMotorTorque(bool rightSide)
	{
		float num = 0f;
		foreach (WheelCollider wheelCollider in rightSide ? this.rightWheels : this.leftWheels)
		{
			num += wheelCollider.motorTorque;
		}
		return num / (float)this.rightWheels.Length;
	}

	// Token: 0x0600179D RID: 6045 RVA: 0x000B0340 File Offset: 0x000AE540
	public void SetMotorTorque(float newThrottle, bool rightSide, float torqueAmount)
	{
		newThrottle = Mathf.Clamp(newThrottle, -1f, 1f);
		float motorTorque = torqueAmount * newThrottle;
		WheelCollider[] array = rightSide ? this.rightWheels : this.leftWheels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].motorTorque = motorTorque;
		}
	}

	// Token: 0x0600179E RID: 6046 RVA: 0x000B038C File Offset: 0x000AE58C
	public void ApplyBrakeTorque(float amount, bool rightSide)
	{
		WheelCollider[] array = rightSide ? this.rightWheels : this.leftWheels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].brakeTorque = this.brakeForce * amount;
		}
	}

	// Token: 0x0600179F RID: 6047 RVA: 0x000059DD File Offset: 0x00003BDD
	public void AdjustFriction()
	{
	}

	// Token: 0x0400108A RID: 4234
	public WheelCollider[] leftWheels;

	// Token: 0x0400108B RID: 4235
	public WheelCollider[] rightWheels;

	// Token: 0x0400108C RID: 4236
	public float moveForceMax = 2000f;

	// Token: 0x0400108D RID: 4237
	public float brakeForce = 100f;

	// Token: 0x0400108E RID: 4238
	public float throttle = 1f;

	// Token: 0x0400108F RID: 4239
	public float turnForce = 2000f;

	// Token: 0x04001090 RID: 4240
	public float sideStiffnessMax = 1f;

	// Token: 0x04001091 RID: 4241
	public float sideStiffnessMin = 0.5f;

	// Token: 0x04001092 RID: 4242
	public Transform centerOfMass;

	// Token: 0x04001093 RID: 4243
	public float turning;

	// Token: 0x04001094 RID: 4244
	public bool brake;

	// Token: 0x04001095 RID: 4245
	public Rigidbody myRigidBody;

	// Token: 0x04001096 RID: 4246
	public Vector3 destination;

	// Token: 0x04001097 RID: 4247
	public float stoppingDist = 5f;

	// Token: 0x04001098 RID: 4248
	public GameObject followTest;
}
