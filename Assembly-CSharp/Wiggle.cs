using System;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200061A RID: 1562
[PostProcess(typeof(WiggleRenderer), PostProcessEvent.AfterStack, "Custom/Wiggle", true)]
[Serializable]
public class Wiggle : PostProcessEffectSettings
{
	// Token: 0x040024E4 RID: 9444
	public FloatParameter speed = new FloatParameter
	{
		value = 1f
	};

	// Token: 0x040024E5 RID: 9445
	public FloatParameter scale = new FloatParameter
	{
		value = 12f
	};
}
