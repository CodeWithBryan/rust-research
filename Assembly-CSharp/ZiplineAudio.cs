using System;
using UnityEngine;

// Token: 0x020000EF RID: 239
public class ZiplineAudio : MonoBehaviour
{
	// Token: 0x04000D0E RID: 3342
	public ZiplineMountable zipline;

	// Token: 0x04000D0F RID: 3343
	public SoundDefinition movementLoopDef;

	// Token: 0x04000D10 RID: 3344
	public SoundDefinition frictionLoopDef;

	// Token: 0x04000D11 RID: 3345
	public SoundDefinition sparksLoopDef;

	// Token: 0x04000D12 RID: 3346
	public AnimationCurve movementGainCurve;

	// Token: 0x04000D13 RID: 3347
	public AnimationCurve movementPitchCurve;

	// Token: 0x04000D14 RID: 3348
	public AnimationCurve frictionGainCurve;

	// Token: 0x04000D15 RID: 3349
	public AnimationCurve sparksGainCurve;
}
