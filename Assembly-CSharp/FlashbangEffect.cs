using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000604 RID: 1540
[PostProcess(typeof(FlashbangEffectRenderer), PostProcessEvent.AfterStack, "Custom/FlashbangEffect", false)]
[Serializable]
public class FlashbangEffect : PostProcessEffectSettings
{
	// Token: 0x0400249B RID: 9371
	[Range(0f, 1f)]
	public FloatParameter burnIntensity = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x0400249C RID: 9372
	[Range(0f, 1f)]
	public FloatParameter whiteoutIntensity = new FloatParameter
	{
		value = 0f
	};
}
