using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A23 RID: 2595
	[PostProcess(typeof(MotionBlurRenderer), "Unity/Motion Blur", false)]
	[Serializable]
	public sealed class MotionBlur : PostProcessEffectSettings
	{
		// Token: 0x06003D8F RID: 15759 RVA: 0x00169559 File Offset: 0x00167759
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled.value && this.shutterAngle.value > 0f && SystemInfo.supportsMotionVectors && RenderTextureFormat.RGHalf.IsSupported() && !RuntimeUtilities.isVREnabled;
		}

		// Token: 0x040036D7 RID: 14039
		[Range(0f, 360f)]
		[Tooltip("The angle of rotary shutter. Larger values give longer exposure.")]
		public FloatParameter shutterAngle = new FloatParameter
		{
			value = 270f
		};

		// Token: 0x040036D8 RID: 14040
		[Range(4f, 32f)]
		[Tooltip("The amount of sample points. This affects quality and performance.")]
		public IntParameter sampleCount = new IntParameter
		{
			value = 10
		};
	}
}
