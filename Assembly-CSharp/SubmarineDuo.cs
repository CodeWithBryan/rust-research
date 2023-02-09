using System;
using Sonar;
using UnityEngine;

// Token: 0x02000484 RID: 1156
public class SubmarineDuo : BaseSubmarine
{
	// Token: 0x04001E43 RID: 7747
	[Header("Duo Sub Seating & Controls")]
	[SerializeField]
	private Transform steeringWheel;

	// Token: 0x04001E44 RID: 7748
	[SerializeField]
	private Transform steeringWheelLeftGrip;

	// Token: 0x04001E45 RID: 7749
	[SerializeField]
	private Transform steeringWheelRightGrip;

	// Token: 0x04001E46 RID: 7750
	[SerializeField]
	private Transform leftPedal;

	// Token: 0x04001E47 RID: 7751
	[SerializeField]
	private Transform rightPedal;

	// Token: 0x04001E48 RID: 7752
	[SerializeField]
	private Transform driverLeftFoot;

	// Token: 0x04001E49 RID: 7753
	[SerializeField]
	private Transform driverRightFoot;

	// Token: 0x04001E4A RID: 7754
	[SerializeField]
	private Transform mphNeedle;

	// Token: 0x04001E4B RID: 7755
	[SerializeField]
	private Transform fuelNeedle;

	// Token: 0x04001E4C RID: 7756
	[SerializeField]
	private Transform waterDepthNeedle;

	// Token: 0x04001E4D RID: 7757
	[SerializeField]
	private Transform ammoFlag;

	// Token: 0x04001E4E RID: 7758
	[SerializeField]
	private SubmarineSonar sonar;

	// Token: 0x04001E4F RID: 7759
	[SerializeField]
	private Transform torpedoTubeHatch;
}
