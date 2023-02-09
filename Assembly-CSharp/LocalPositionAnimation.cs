using System;
using UnityEngine;

// Token: 0x020002AC RID: 684
public class LocalPositionAnimation : MonoBehaviour, IClientComponent
{
	// Token: 0x0400159A RID: 5530
	public Vector3 centerPosition;

	// Token: 0x0400159B RID: 5531
	public bool worldSpace;

	// Token: 0x0400159C RID: 5532
	public float scaleX = 1f;

	// Token: 0x0400159D RID: 5533
	public float timeScaleX = 1f;

	// Token: 0x0400159E RID: 5534
	public AnimationCurve movementX = new AnimationCurve();

	// Token: 0x0400159F RID: 5535
	public float scaleY = 1f;

	// Token: 0x040015A0 RID: 5536
	public float timeScaleY = 1f;

	// Token: 0x040015A1 RID: 5537
	public AnimationCurve movementY = new AnimationCurve();

	// Token: 0x040015A2 RID: 5538
	public float scaleZ = 1f;

	// Token: 0x040015A3 RID: 5539
	public float timeScaleZ = 1f;

	// Token: 0x040015A4 RID: 5540
	public AnimationCurve movementZ = new AnimationCurve();
}
