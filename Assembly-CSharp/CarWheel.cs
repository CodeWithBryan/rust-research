using System;
using UnityEngine;

// Token: 0x02000450 RID: 1104
[Serializable]
public class CarWheel
{
	// Token: 0x04001CC7 RID: 7367
	public WheelCollider wheelCollider;

	// Token: 0x04001CC8 RID: 7368
	[Range(0.1f, 3f)]
	public float tyreFriction = 1f;

	// Token: 0x04001CC9 RID: 7369
	public bool steerWheel;

	// Token: 0x04001CCA RID: 7370
	public bool brakeWheel = true;

	// Token: 0x04001CCB RID: 7371
	public bool powerWheel = true;
}
