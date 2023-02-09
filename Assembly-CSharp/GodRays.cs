using System;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200060C RID: 1548
[PostProcess(typeof(GodRaysRenderer), PostProcessEvent.BeforeStack, "Custom/GodRays", true)]
[Serializable]
public class GodRays : PostProcessEffectSettings
{
	// Token: 0x040024AF RID: 9391
	public BoolParameter UseDepth = new BoolParameter
	{
		value = true
	};

	// Token: 0x040024B0 RID: 9392
	public BlendModeTypeParameter BlendMode = new BlendModeTypeParameter
	{
		value = BlendModeType.Screen
	};

	// Token: 0x040024B1 RID: 9393
	public FloatParameter Intensity = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x040024B2 RID: 9394
	public ResolutionTypeParameter Resolution = new ResolutionTypeParameter
	{
		value = ResolutionType.High
	};

	// Token: 0x040024B3 RID: 9395
	public IntParameter BlurIterations = new IntParameter
	{
		value = 2
	};

	// Token: 0x040024B4 RID: 9396
	public FloatParameter BlurRadius = new FloatParameter
	{
		value = 2f
	};

	// Token: 0x040024B5 RID: 9397
	public FloatParameter MaxRadius = new FloatParameter
	{
		value = 0.5f
	};
}
