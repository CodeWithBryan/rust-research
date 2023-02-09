using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200058E RID: 1422
public class OutlineManager : MonoBehaviour, IClientComponent
{
	// Token: 0x04002274 RID: 8820
	public static Material blurMat;

	// Token: 0x04002275 RID: 8821
	public List<OutlineObject> objectsToRender;

	// Token: 0x04002276 RID: 8822
	public float blurAmount = 2f;

	// Token: 0x04002277 RID: 8823
	public Material glowSolidMaterial;

	// Token: 0x04002278 RID: 8824
	public Material blendGlowMaterial;
}
