using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000618 RID: 1560
[PostProcess(typeof(SharpenAndVignetteRenderer), PostProcessEvent.AfterStack, "Custom/SharpenAndVignette", true)]
[Serializable]
public class SharpenAndVignette : PostProcessEffectSettings
{
	// Token: 0x040024DD RID: 9437
	[Header("Sharpen")]
	public BoolParameter applySharpen = new BoolParameter
	{
		value = true
	};

	// Token: 0x040024DE RID: 9438
	[Range(0f, 5f)]
	public FloatParameter strength = new FloatParameter
	{
		value = 1f
	};

	// Token: 0x040024DF RID: 9439
	[Range(0f, 1f)]
	public FloatParameter clamp = new FloatParameter
	{
		value = 1f
	};

	// Token: 0x040024E0 RID: 9440
	[Header("Vignette")]
	public BoolParameter applyVignette = new BoolParameter
	{
		value = true
	};

	// Token: 0x040024E1 RID: 9441
	[Range(-100f, 100f)]
	public FloatParameter sharpness = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x040024E2 RID: 9442
	[Range(0f, 100f)]
	public FloatParameter darkness = new FloatParameter
	{
		value = 0f
	};
}
