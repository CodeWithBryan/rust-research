using System;
using UnityEngine;

// Token: 0x02000488 RID: 1160
public class LocomotiveExtraVisuals : MonoBehaviour
{
	// Token: 0x04001E76 RID: 7798
	[Header("Gauges")]
	[SerializeField]
	private TrainEngine trainEngine;

	// Token: 0x04001E77 RID: 7799
	[SerializeField]
	private Transform needleA;

	// Token: 0x04001E78 RID: 7800
	[SerializeField]
	private Transform needleB;

	// Token: 0x04001E79 RID: 7801
	[SerializeField]
	private Transform needleC;

	// Token: 0x04001E7A RID: 7802
	[SerializeField]
	private float maxAngle = 240f;

	// Token: 0x04001E7B RID: 7803
	[SerializeField]
	private float speedoMoveSpeed = 75f;

	// Token: 0x04001E7C RID: 7804
	[SerializeField]
	private float pressureMoveSpeed = 25f;

	// Token: 0x04001E7D RID: 7805
	[SerializeField]
	private float fanAcceleration = 50f;

	// Token: 0x04001E7E RID: 7806
	[SerializeField]
	private float fanMaxSpeed = 1000f;

	// Token: 0x04001E7F RID: 7807
	[SerializeField]
	private float speedoMax = 80f;

	// Token: 0x04001E80 RID: 7808
	[Header("Fans")]
	[SerializeField]
	private Transform[] engineFans;
}
