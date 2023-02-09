using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000606 RID: 1542
[PostProcess(typeof(FrostRenderer), PostProcessEvent.AfterStack, "Custom/Frost", true)]
[Serializable]
public class Frost : PostProcessEffectSettings
{
	// Token: 0x040024A0 RID: 9376
	[Range(0f, 16f)]
	public FloatParameter scale = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x040024A1 RID: 9377
	public BoolParameter enableVignette = new BoolParameter
	{
		value = true
	};

	// Token: 0x040024A2 RID: 9378
	[Range(0f, 100f)]
	public FloatParameter sharpness = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x040024A3 RID: 9379
	[Range(0f, 100f)]
	public FloatParameter darkness = new FloatParameter
	{
		value = 0f
	};
}
