using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000600 RID: 1536
[PostProcess(typeof(DepthOfFieldEffectRenderer), "Unity/Depth of Field (Custom)", false)]
[Serializable]
public class DepthOfFieldEffect : PostProcessEffectSettings
{
	// Token: 0x0400248C RID: 9356
	public FloatParameter focalLength = new FloatParameter
	{
		value = 10f
	};

	// Token: 0x0400248D RID: 9357
	public FloatParameter focalSize = new FloatParameter
	{
		value = 0.05f
	};

	// Token: 0x0400248E RID: 9358
	public FloatParameter aperture = new FloatParameter
	{
		value = 11.5f
	};

	// Token: 0x0400248F RID: 9359
	public FloatParameter maxBlurSize = new FloatParameter
	{
		value = 2f
	};

	// Token: 0x04002490 RID: 9360
	public BoolParameter highResolution = new BoolParameter
	{
		value = true
	};

	// Token: 0x04002491 RID: 9361
	public DOFBlurSampleCountParameter blurSampleCount = new DOFBlurSampleCountParameter
	{
		value = DOFBlurSampleCount.Low
	};

	// Token: 0x04002492 RID: 9362
	public Transform focalTransform;
}
