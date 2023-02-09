using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A1F RID: 2591
	[PostProcess(typeof(GrainRenderer), "Unity/Grain", true)]
	[Serializable]
	public sealed class Grain : PostProcessEffectSettings
	{
		// Token: 0x06003D85 RID: 15749 RVA: 0x0016900E File Offset: 0x0016720E
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled.value && this.intensity.value > 0f;
		}

		// Token: 0x040036CA RID: 14026
		[Tooltip("Enable the use of colored grain.")]
		public BoolParameter colored = new BoolParameter
		{
			value = true
		};

		// Token: 0x040036CB RID: 14027
		[Range(0f, 1f)]
		[Tooltip("Grain strength. Higher values mean more visible grain.")]
		public FloatParameter intensity = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040036CC RID: 14028
		[Range(0.3f, 3f)]
		[Tooltip("Grain particle size.")]
		public FloatParameter size = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x040036CD RID: 14029
		[Range(0f, 1f)]
		[DisplayName("Luminance Contribution")]
		[Tooltip("Controls the noise response curve based on scene luminance. Lower values mean less noise in dark areas.")]
		public FloatParameter lumContrib = new FloatParameter
		{
			value = 0.8f
		};
	}
}
