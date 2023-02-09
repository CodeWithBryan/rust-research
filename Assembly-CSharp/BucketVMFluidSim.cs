using System;
using UnityEngine;

// Token: 0x02000313 RID: 787
public class BucketVMFluidSim : MonoBehaviour
{
	// Token: 0x0400172C RID: 5932
	public Animator waterbucketAnim;

	// Token: 0x0400172D RID: 5933
	public ParticleSystem waterPour;

	// Token: 0x0400172E RID: 5934
	public ParticleSystem waterTurbulence;

	// Token: 0x0400172F RID: 5935
	public ParticleSystem waterFill;

	// Token: 0x04001730 RID: 5936
	public float waterLevel;

	// Token: 0x04001731 RID: 5937
	public float targetWaterLevel;

	// Token: 0x04001732 RID: 5938
	public AudioSource waterSpill;

	// Token: 0x04001733 RID: 5939
	private float PlayerEyePitch;

	// Token: 0x04001734 RID: 5940
	private float turb_forward;

	// Token: 0x04001735 RID: 5941
	private float turb_side;

	// Token: 0x04001736 RID: 5942
	private Vector3 lastPosition;

	// Token: 0x04001737 RID: 5943
	protected Vector3 groundSpeedLast;

	// Token: 0x04001738 RID: 5944
	private Vector3 lastAngle;

	// Token: 0x04001739 RID: 5945
	protected Vector3 vecAngleSpeedLast;

	// Token: 0x0400173A RID: 5946
	private Vector3 initialPosition;
}
