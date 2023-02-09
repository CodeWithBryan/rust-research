using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000612 RID: 1554
[PostProcess(typeof(PhotoFilterRenderer), PostProcessEvent.AfterStack, "Custom/PhotoFilter", true)]
[Serializable]
public class PhotoFilter : PostProcessEffectSettings
{
	// Token: 0x040024CC RID: 9420
	public ColorParameter color = new ColorParameter
	{
		value = Color.white
	};

	// Token: 0x040024CD RID: 9421
	[Range(0f, 1f)]
	public FloatParameter density = new FloatParameter
	{
		value = 0f
	};
}
