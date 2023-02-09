using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x020000F6 RID: 246
public class NVGCamera : FacepunchBehaviour, IClothingChanged
{
	// Token: 0x04000D4A RID: 3402
	public static NVGCamera instance;

	// Token: 0x04000D4B RID: 3403
	public PostProcessVolume postProcessVolume;

	// Token: 0x04000D4C RID: 3404
	public GameObject lights;
}
