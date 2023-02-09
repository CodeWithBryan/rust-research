using System;
using UnityEngine;

// Token: 0x020006EE RID: 1774
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(CommandBufferManager))]
public class DeferredExtension : MonoBehaviour
{
	// Token: 0x04002820 RID: 10272
	public ExtendGBufferParams extendGBuffer = ExtendGBufferParams.Default;

	// Token: 0x04002821 RID: 10273
	public SubsurfaceScatteringParams subsurfaceScattering = SubsurfaceScatteringParams.Default;

	// Token: 0x04002822 RID: 10274
	public Texture2D blueNoise;

	// Token: 0x04002823 RID: 10275
	public float depthScale = 100f;

	// Token: 0x04002824 RID: 10276
	public bool debug;

	// Token: 0x04002825 RID: 10277
	public bool forceToCameraResolution;
}
