using System;
using UnityEngine;

// Token: 0x02000320 RID: 800
public class EyeBlink : MonoBehaviour
{
	// Token: 0x04001757 RID: 5975
	public Transform LeftEye;

	// Token: 0x04001758 RID: 5976
	public Transform LeftEyelid;

	// Token: 0x04001759 RID: 5977
	public Vector3 LeftEyeOffset = new Vector3(0.01f, -0.002f, 0f);

	// Token: 0x0400175A RID: 5978
	public Transform RightEye;

	// Token: 0x0400175B RID: 5979
	public Transform RightEyelid;

	// Token: 0x0400175C RID: 5980
	public Vector3 RightEyeOffset = new Vector3(0.01f, -0.002f, 0f);

	// Token: 0x0400175D RID: 5981
	public Vector3 ClosedEyelidPosition;

	// Token: 0x0400175E RID: 5982
	public Vector3 ClosedEyelidRotation;

	// Token: 0x0400175F RID: 5983
	public Vector2 TimeWithoutBlinking = new Vector2(1f, 10f);

	// Token: 0x04001760 RID: 5984
	public float BlinkSpeed = 0.2f;

	// Token: 0x04001761 RID: 5985
	public Vector3 LeftEyeInitial;

	// Token: 0x04001762 RID: 5986
	public Vector3 RightEyeInitial;
}
