using System;
using UnityEngine;

// Token: 0x0200093A RID: 2362
public class IronSights : MonoBehaviour
{
	// Token: 0x0400323B RID: 12859
	public bool Enabled;

	// Token: 0x0400323C RID: 12860
	[Header("View Setup")]
	public IronsightAimPoint aimPoint;

	// Token: 0x0400323D RID: 12861
	public float fieldOfViewOffset = -20f;

	// Token: 0x0400323E RID: 12862
	public float zoomFactor = 1f;

	// Token: 0x0400323F RID: 12863
	[Header("Animation")]
	public float introSpeed = 1f;

	// Token: 0x04003240 RID: 12864
	public AnimationCurve introCurve = new AnimationCurve();

	// Token: 0x04003241 RID: 12865
	public float outroSpeed = 1f;

	// Token: 0x04003242 RID: 12866
	public AnimationCurve outroCurve = new AnimationCurve();

	// Token: 0x04003243 RID: 12867
	[Header("Sounds")]
	public SoundDefinition upSound;

	// Token: 0x04003244 RID: 12868
	public SoundDefinition downSound;

	// Token: 0x04003245 RID: 12869
	[Header("Info")]
	public IronSightOverride ironsightsOverride;

	// Token: 0x04003246 RID: 12870
	public bool processUltrawideOffset;
}
