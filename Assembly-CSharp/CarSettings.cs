using System;
using UnityEngine;

// Token: 0x0200044F RID: 1103
[Serializable]
public class CarSettings
{
	// Token: 0x04001CB4 RID: 7348
	[Header("Vehicle Setup")]
	[Range(0f, 1f)]
	public float rollingResistance = 0.05f;

	// Token: 0x04001CB5 RID: 7349
	[Range(0f, 1f)]
	public float antiRoll;

	// Token: 0x04001CB6 RID: 7350
	public bool canSleep = true;

	// Token: 0x04001CB7 RID: 7351
	[Header("Wheels")]
	public bool tankSteering;

	// Token: 0x04001CB8 RID: 7352
	[Range(0f, 50f)]
	public float maxSteerAngle = 35f;

	// Token: 0x04001CB9 RID: 7353
	public bool steeringAssist = true;

	// Token: 0x04001CBA RID: 7354
	[Range(0f, 1f)]
	public float steeringAssistRatio = 0.5f;

	// Token: 0x04001CBB RID: 7355
	public bool steeringLimit;

	// Token: 0x04001CBC RID: 7356
	[Range(0f, 50f)]
	public float minSteerLimitAngle = 6f;

	// Token: 0x04001CBD RID: 7357
	[Range(10f, 50f)]
	public float minSteerLimitSpeed = 30f;

	// Token: 0x04001CBE RID: 7358
	[Range(0f, 1f)]
	public float rearWheelSteer = 1f;

	// Token: 0x04001CBF RID: 7359
	public float steerMinLerpSpeed = 75f;

	// Token: 0x04001CC0 RID: 7360
	public float steerMaxLerpSpeed = 150f;

	// Token: 0x04001CC1 RID: 7361
	public float steerReturnLerpSpeed = 200f;

	// Token: 0x04001CC2 RID: 7362
	[Header("Motor")]
	public float maxDriveSlip = 4f;

	// Token: 0x04001CC3 RID: 7363
	public float driveForceToMaxSlip = 1000f;

	// Token: 0x04001CC4 RID: 7364
	public float reversePercentSpeed = 0.3f;

	// Token: 0x04001CC5 RID: 7365
	[Header("Brakes")]
	public float brakeForceMultiplier = 1000f;

	// Token: 0x04001CC6 RID: 7366
	[Header("Front/Rear Vehicle Balance")]
	[Range(0f, 1f)]
	public float handlingBias = 0.5f;
}
