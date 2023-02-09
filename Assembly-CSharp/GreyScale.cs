using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200060E RID: 1550
[PostProcess(typeof(GreyScaleRenderer), PostProcessEvent.AfterStack, "Custom/GreyScale", true)]
[Serializable]
public class GreyScale : PostProcessEffectSettings
{
	// Token: 0x040024BB RID: 9403
	[Range(0f, 1f)]
	public FloatParameter redLuminance = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x040024BC RID: 9404
	[Range(0f, 1f)]
	public FloatParameter greenLuminance = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x040024BD RID: 9405
	[Range(0f, 1f)]
	public FloatParameter blueLuminance = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x040024BE RID: 9406
	[Range(0f, 1f)]
	public FloatParameter amount = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x040024BF RID: 9407
	[ColorUsage(false, true)]
	public ColorParameter color = new ColorParameter
	{
		value = Color.white
	};
}
