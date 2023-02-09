using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A21 RID: 2593
	[PostProcess(typeof(LensDistortionRenderer), "Unity/Lens Distortion", true)]
	[Serializable]
	public sealed class LensDistortion : PostProcessEffectSettings
	{
		// Token: 0x06003D8B RID: 15755 RVA: 0x00169324 File Offset: 0x00167524
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled.value && !Mathf.Approximately(this.intensity, 0f) && (this.intensityX > 0f || this.intensityY > 0f) && !RuntimeUtilities.isVREnabled;
		}

		// Token: 0x040036D1 RID: 14033
		[Range(-100f, 100f)]
		[Tooltip("Total distortion amount.")]
		public FloatParameter intensity = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040036D2 RID: 14034
		[Range(0f, 1f)]
		[DisplayName("X Multiplier")]
		[Tooltip("Intensity multiplier on the x-axis. Set it to 0 to disable distortion on this axis.")]
		public FloatParameter intensityX = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x040036D3 RID: 14035
		[Range(0f, 1f)]
		[DisplayName("Y Multiplier")]
		[Tooltip("Intensity multiplier on the y-axis. Set it to 0 to disable distortion on this axis.")]
		public FloatParameter intensityY = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x040036D4 RID: 14036
		[Space]
		[Range(-1f, 1f)]
		[Tooltip("Distortion center point (x-axis).")]
		public FloatParameter centerX = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040036D5 RID: 14037
		[Range(-1f, 1f)]
		[Tooltip("Distortion center point (y-axis).")]
		public FloatParameter centerY = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040036D6 RID: 14038
		[Space]
		[Range(0.01f, 5f)]
		[Tooltip("Global screen scaling.")]
		public FloatParameter scale = new FloatParameter
		{
			value = 1f
		};
	}
}
