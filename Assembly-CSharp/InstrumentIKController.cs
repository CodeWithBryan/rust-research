using System;
using UnityEngine;

// Token: 0x020003D8 RID: 984
public class InstrumentIKController : MonoBehaviour
{
	// Token: 0x040019BB RID: 6587
	public Vector3 HitRotationVector = Vector3.forward;

	// Token: 0x040019BC RID: 6588
	public Transform[] LeftHandIkTargets = new Transform[0];

	// Token: 0x040019BD RID: 6589
	public Transform[] LeftHandIKTargetHitRotations = new Transform[0];

	// Token: 0x040019BE RID: 6590
	public Transform[] RightHandIkTargets = new Transform[0];

	// Token: 0x040019BF RID: 6591
	public Transform[] RightHandIKTargetHitRotations = new Transform[0];

	// Token: 0x040019C0 RID: 6592
	public Transform[] RightFootIkTargets = new Transform[0];

	// Token: 0x040019C1 RID: 6593
	public AnimationCurve HandHeightCurve = AnimationCurve.Constant(0f, 1f, 0f);

	// Token: 0x040019C2 RID: 6594
	public float HandHeightMultiplier = 1f;

	// Token: 0x040019C3 RID: 6595
	public float HandMoveLerpSpeed = 50f;

	// Token: 0x040019C4 RID: 6596
	public bool DebugHitRotation;

	// Token: 0x040019C5 RID: 6597
	public AnimationCurve HandHitCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040019C6 RID: 6598
	public float NoteHitTime = 0.5f;

	// Token: 0x040019C7 RID: 6599
	[Header("Look IK")]
	public float BodyLookWeight;

	// Token: 0x040019C8 RID: 6600
	public float HeadLookWeight;

	// Token: 0x040019C9 RID: 6601
	public float LookWeightLimit;

	// Token: 0x040019CA RID: 6602
	public bool HoldHandsAtPlay;
}
