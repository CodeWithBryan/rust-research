using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A1A RID: 2586
	[PostProcess(typeof(DepthOfFieldRenderer), "Unity/Depth of Field", false)]
	[Serializable]
	public sealed class DepthOfField : PostProcessEffectSettings
	{
		// Token: 0x06003D75 RID: 15733 RVA: 0x00168826 File Offset: 0x00166A26
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled.value && SystemInfo.graphicsShaderLevel >= 35;
		}

		// Token: 0x040036BC RID: 14012
		[Min(0.1f)]
		[Tooltip("Distance to the point of focus.")]
		public FloatParameter focusDistance = new FloatParameter
		{
			value = 10f
		};

		// Token: 0x040036BD RID: 14013
		[Range(0.05f, 32f)]
		[Tooltip("Ratio of aperture (known as f-stop or f-number). The smaller the value is, the shallower the depth of field is.")]
		public FloatParameter aperture = new FloatParameter
		{
			value = 5.6f
		};

		// Token: 0x040036BE RID: 14014
		[Range(1f, 300f)]
		[Tooltip("Distance between the lens and the film. The larger the value is, the shallower the depth of field is.")]
		public FloatParameter focalLength = new FloatParameter
		{
			value = 50f
		};

		// Token: 0x040036BF RID: 14015
		[DisplayName("Max Blur Size")]
		[Tooltip("Convolution kernel size of the bokeh filter, which determines the maximum radius of bokeh. It also affects performances (the larger the kernel is, the longer the GPU time is required).")]
		public KernelSizeParameter kernelSize = new KernelSizeParameter
		{
			value = KernelSize.Medium
		};
	}
}
