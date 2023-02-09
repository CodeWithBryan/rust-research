using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x020005FB RID: 1531
[PostProcess(typeof(BlurOptimizedRenderer), PostProcessEvent.AfterStack, "Custom/BlurOptimized", true)]
[Serializable]
public class BlurOptimized : PostProcessEffectSettings
{
	// Token: 0x0400247F RID: 9343
	[Range(0f, 2f)]
	public FixedIntParameter downsample = new FixedIntParameter
	{
		value = 0
	};

	// Token: 0x04002480 RID: 9344
	[Range(1f, 4f)]
	public FixedIntParameter blurIterations = new FixedIntParameter
	{
		value = 1
	};

	// Token: 0x04002481 RID: 9345
	[Range(0f, 10f)]
	public FloatParameter blurSize = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x04002482 RID: 9346
	public FloatParameter fadeToBlurDistance = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x04002483 RID: 9347
	public BlurTypeParameter blurType = new BlurTypeParameter
	{
		value = BlurType.StandardGauss
	};
}
