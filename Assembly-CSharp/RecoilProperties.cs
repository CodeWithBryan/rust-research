using System;
using UnityEngine;

// Token: 0x02000730 RID: 1840
[CreateAssetMenu(menuName = "Rust/Recoil Properties")]
public class RecoilProperties : ScriptableObject
{
	// Token: 0x060032F1 RID: 13041 RVA: 0x0013ABA7 File Offset: 0x00138DA7
	public RecoilProperties GetRecoil()
	{
		if (!(this.newRecoilOverride != null))
		{
			return this;
		}
		return this.newRecoilOverride;
	}

	// Token: 0x04002946 RID: 10566
	public float recoilYawMin;

	// Token: 0x04002947 RID: 10567
	public float recoilYawMax;

	// Token: 0x04002948 RID: 10568
	public float recoilPitchMin;

	// Token: 0x04002949 RID: 10569
	public float recoilPitchMax;

	// Token: 0x0400294A RID: 10570
	public float timeToTakeMin;

	// Token: 0x0400294B RID: 10571
	public float timeToTakeMax = 0.1f;

	// Token: 0x0400294C RID: 10572
	public float ADSScale = 0.5f;

	// Token: 0x0400294D RID: 10573
	public float movementPenalty;

	// Token: 0x0400294E RID: 10574
	public float clampPitch = float.NegativeInfinity;

	// Token: 0x0400294F RID: 10575
	public AnimationCurve pitchCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04002950 RID: 10576
	public AnimationCurve yawCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04002951 RID: 10577
	public bool useCurves;

	// Token: 0x04002952 RID: 10578
	public bool curvesAsScalar;

	// Token: 0x04002953 RID: 10579
	public int shotsUntilMax = 30;

	// Token: 0x04002954 RID: 10580
	public float maxRecoilRadius = 5f;

	// Token: 0x04002955 RID: 10581
	[Header("AimCone")]
	public bool overrideAimconeWithCurve;

	// Token: 0x04002956 RID: 10582
	public float aimconeCurveScale = 1f;

	// Token: 0x04002957 RID: 10583
	[Tooltip("How much to scale aimcone by based on how far into the shot sequence we are (shots v shotsUntilMax)")]
	public AnimationCurve aimconeCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04002958 RID: 10584
	[Tooltip("Randomly select how much to scale final aimcone by per shot, you can use this to weigh a fraction of shots closer to the center")]
	public AnimationCurve aimconeProbabilityCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(0.5f, 0f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x04002959 RID: 10585
	public RecoilProperties newRecoilOverride;
}
