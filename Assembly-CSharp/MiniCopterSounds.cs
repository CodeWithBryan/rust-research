using System;
using UnityEngine;

// Token: 0x02000026 RID: 38
public class MiniCopterSounds : MonoBehaviour, IClientComponent
{
	// Token: 0x04000128 RID: 296
	public MiniCopter miniCopter;

	// Token: 0x04000129 RID: 297
	public GameObject soundAttachPoint;

	// Token: 0x0400012A RID: 298
	public SoundDefinition engineStartDef;

	// Token: 0x0400012B RID: 299
	public SoundDefinition engineLoopDef;

	// Token: 0x0400012C RID: 300
	public SoundDefinition engineStopDef;

	// Token: 0x0400012D RID: 301
	public SoundDefinition rotorLoopDef;

	// Token: 0x0400012E RID: 302
	public float engineStartFadeOutTime = 1f;

	// Token: 0x0400012F RID: 303
	public float engineLoopFadeInTime = 0.7f;

	// Token: 0x04000130 RID: 304
	public float engineLoopFadeOutTime = 0.25f;

	// Token: 0x04000131 RID: 305
	public float engineStopFadeOutTime = 0.25f;

	// Token: 0x04000132 RID: 306
	public float rotorLoopFadeInTime = 0.7f;

	// Token: 0x04000133 RID: 307
	public float rotorLoopFadeOutTime = 0.25f;

	// Token: 0x04000134 RID: 308
	public float enginePitchInterpRate = 0.5f;

	// Token: 0x04000135 RID: 309
	public float rotorPitchInterpRate = 1f;

	// Token: 0x04000136 RID: 310
	public float rotorGainInterpRate = 0.5f;

	// Token: 0x04000137 RID: 311
	public float rotorStartStopPitchRateUp = 7f;

	// Token: 0x04000138 RID: 312
	public float rotorStartStopPitchRateDown = 9f;

	// Token: 0x04000139 RID: 313
	public float rotorStartStopGainRateUp = 5f;

	// Token: 0x0400013A RID: 314
	public float rotorStartStopGainRateDown = 4f;

	// Token: 0x0400013B RID: 315
	public AnimationCurve engineUpDotPitchCurve;

	// Token: 0x0400013C RID: 316
	public AnimationCurve rotorUpDotPitchCurve;
}
