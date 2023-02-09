using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000602 RID: 1538
[PostProcess(typeof(DoubleVisionRenderer), PostProcessEvent.AfterStack, "Custom/DoubleVision", true)]
[Serializable]
public class DoubleVision : PostProcessEffectSettings
{
	// Token: 0x04002496 RID: 9366
	[Range(0f, 1f)]
	public Vector2Parameter displace = new Vector2Parameter
	{
		value = Vector2.zero
	};

	// Token: 0x04002497 RID: 9367
	[Range(0f, 1f)]
	public FloatParameter amount = new FloatParameter
	{
		value = 0f
	};
}
