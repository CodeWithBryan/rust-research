using System;
using UnityEngine;

// Token: 0x020003F3 RID: 1011
public class M2BradleyPhysics : MonoBehaviour
{
	// Token: 0x04001A5D RID: 6749
	private m2bradleyAnimator m2Animator;

	// Token: 0x04001A5E RID: 6750
	public WheelCollider[] Wheels;

	// Token: 0x04001A5F RID: 6751
	public WheelCollider[] TurningWheels;

	// Token: 0x04001A60 RID: 6752
	public Rigidbody mainRigidbody;

	// Token: 0x04001A61 RID: 6753
	public Transform[] waypoints;

	// Token: 0x04001A62 RID: 6754
	private Vector3 currentWaypoint;

	// Token: 0x04001A63 RID: 6755
	private Vector3 nextWaypoint;
}
