using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200027D RID: 637
public class FlashbangOverlay : MonoBehaviour, IClientComponent
{
	// Token: 0x04001512 RID: 5394
	public static FlashbangOverlay Instance;

	// Token: 0x04001513 RID: 5395
	public PostProcessVolume postProcessVolume;

	// Token: 0x04001514 RID: 5396
	public AnimationCurve burnIntensityCurve;

	// Token: 0x04001515 RID: 5397
	public AnimationCurve whiteoutIntensityCurve;

	// Token: 0x04001516 RID: 5398
	public SoundDefinition deafLoopDef;
}
