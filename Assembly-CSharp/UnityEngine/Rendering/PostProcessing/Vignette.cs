using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A31 RID: 2609
	[PostProcess(typeof(VignetteRenderer), "Unity/Vignette", true)]
	[Serializable]
	public sealed class Vignette : PostProcessEffectSettings
	{
		// Token: 0x06003DD4 RID: 15828 RVA: 0x0016C0B8 File Offset: 0x0016A2B8
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled.value && ((this.mode.value == VignetteMode.Classic && this.intensity.value > 0f) || (this.mode.value == VignetteMode.Masked && this.opacity.value > 0f && this.mask.value != null));
		}

		// Token: 0x04003713 RID: 14099
		[Tooltip("Use the \"Classic\" mode for parametric controls. Use the \"Masked\" mode to use your own texture mask.")]
		public VignetteModeParameter mode = new VignetteModeParameter
		{
			value = VignetteMode.Classic
		};

		// Token: 0x04003714 RID: 14100
		[Tooltip("Vignette color.")]
		public ColorParameter color = new ColorParameter
		{
			value = new Color(0f, 0f, 0f, 1f)
		};

		// Token: 0x04003715 RID: 14101
		[Tooltip("Sets the vignette center point (screen center is [0.5, 0.5]).")]
		public Vector2Parameter center = new Vector2Parameter
		{
			value = new Vector2(0.5f, 0.5f)
		};

		// Token: 0x04003716 RID: 14102
		[Range(0f, 1f)]
		[Tooltip("Amount of vignetting on screen.")]
		public FloatParameter intensity = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x04003717 RID: 14103
		[Range(0.01f, 1f)]
		[Tooltip("Smoothness of the vignette borders.")]
		public FloatParameter smoothness = new FloatParameter
		{
			value = 0.2f
		};

		// Token: 0x04003718 RID: 14104
		[Range(0f, 1f)]
		[Tooltip("Lower values will make a square-ish vignette.")]
		public FloatParameter roundness = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x04003719 RID: 14105
		[Tooltip("Set to true to mark the vignette to be perfectly round. False will make its shape dependent on the current aspect ratio.")]
		public BoolParameter rounded = new BoolParameter
		{
			value = false
		};

		// Token: 0x0400371A RID: 14106
		[Tooltip("A black and white mask to use as a vignette.")]
		public TextureParameter mask = new TextureParameter
		{
			value = null
		};

		// Token: 0x0400371B RID: 14107
		[Range(0f, 1f)]
		[Tooltip("Mask opacity.")]
		public FloatParameter opacity = new FloatParameter
		{
			value = 1f
		};
	}
}
