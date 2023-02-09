using System;
using UnityEngine;

// Token: 0x0200042C RID: 1068
public class PlayerWalkMovement : BaseMovement
{
	// Token: 0x04001BFF RID: 7167
	public const float WaterLevelHead = 0.75f;

	// Token: 0x04001C00 RID: 7168
	public const float WaterLevelNeck = 0.65f;

	// Token: 0x04001C01 RID: 7169
	public PhysicMaterial zeroFrictionMaterial;

	// Token: 0x04001C02 RID: 7170
	public PhysicMaterial highFrictionMaterial;

	// Token: 0x04001C03 RID: 7171
	public float capsuleHeight = 1f;

	// Token: 0x04001C04 RID: 7172
	public float capsuleCenter = 1f;

	// Token: 0x04001C05 RID: 7173
	public float capsuleHeightDucked = 1f;

	// Token: 0x04001C06 RID: 7174
	public float capsuleCenterDucked = 1f;

	// Token: 0x04001C07 RID: 7175
	public float capsuleHeightCrawling = 0.5f;

	// Token: 0x04001C08 RID: 7176
	public float capsuleCenterCrawling = 0.5f;

	// Token: 0x04001C09 RID: 7177
	public float gravityTestRadius = 0.2f;

	// Token: 0x04001C0A RID: 7178
	public float gravityMultiplier = 2.5f;

	// Token: 0x04001C0B RID: 7179
	public float gravityMultiplierSwimming = 0.1f;

	// Token: 0x04001C0C RID: 7180
	public float maxAngleWalking = 50f;

	// Token: 0x04001C0D RID: 7181
	public float maxAngleClimbing = 60f;

	// Token: 0x04001C0E RID: 7182
	public float maxAngleSliding = 90f;

	// Token: 0x04001C0F RID: 7183
	public float maxStepHeight = 0.25f;
}
