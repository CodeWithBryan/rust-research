using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000610 RID: 1552
[PostProcess(typeof(LensDirtinessRenderer), PostProcessEvent.AfterStack, "Custom/LensDirtiness", true)]
[Serializable]
public class LensDirtinessEffect : PostProcessEffectSettings
{
	// Token: 0x040024C3 RID: 9411
	public TextureParameter dirtinessTexture = new TextureParameter();

	// Token: 0x040024C4 RID: 9412
	public BoolParameter sceneTintsBloom = new BoolParameter
	{
		value = false
	};

	// Token: 0x040024C5 RID: 9413
	public FloatParameter gain = new FloatParameter
	{
		value = 1f
	};

	// Token: 0x040024C6 RID: 9414
	public FloatParameter threshold = new FloatParameter
	{
		value = 1f
	};

	// Token: 0x040024C7 RID: 9415
	public FloatParameter bloomSize = new FloatParameter
	{
		value = 5f
	};

	// Token: 0x040024C8 RID: 9416
	public FloatParameter dirtiness = new FloatParameter
	{
		value = 1f
	};

	// Token: 0x040024C9 RID: 9417
	public ColorParameter bloomColor = new ColorParameter
	{
		value = Color.white
	};
}
