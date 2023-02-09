using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A0E RID: 2574
	[PostProcess(typeof(BloomRenderer), "Unity/Bloom", true)]
	[Serializable]
	public sealed class Bloom : PostProcessEffectSettings
	{
		// Token: 0x06003D59 RID: 15705 RVA: 0x0016648C File Offset: 0x0016468C
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled.value && this.intensity.value > 0f;
		}

		// Token: 0x04003670 RID: 13936
		[Min(0f)]
		[Tooltip("Strength of the bloom filter. Values higher than 1 will make bloom contribute more energy to the final render.")]
		public FloatParameter intensity = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x04003671 RID: 13937
		[Min(0f)]
		[Tooltip("Filters out pixels under this level of brightness. Value is in gamma-space.")]
		public FloatParameter threshold = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x04003672 RID: 13938
		[Range(0f, 1f)]
		[Tooltip("Makes transitions between under/over-threshold gradual. 0 for a hard threshold, 1 for a soft threshold).")]
		public FloatParameter softKnee = new FloatParameter
		{
			value = 0.5f
		};

		// Token: 0x04003673 RID: 13939
		[Tooltip("Clamps pixels to control the bloom amount. Value is in gamma-space.")]
		public FloatParameter clamp = new FloatParameter
		{
			value = 65472f
		};

		// Token: 0x04003674 RID: 13940
		[Range(1f, 10f)]
		[Tooltip("Changes the extent of veiling effects. For maximum quality, use integer values. Because this value changes the internal iteration count, You should not animating it as it may introduce issues with the perceived radius.")]
		public FloatParameter diffusion = new FloatParameter
		{
			value = 7f
		};

		// Token: 0x04003675 RID: 13941
		[Range(-1f, 1f)]
		[Tooltip("Distorts the bloom to give an anamorphic look. Negative values distort vertically, positive values distort horizontally.")]
		public FloatParameter anamorphicRatio = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x04003676 RID: 13942
		[ColorUsage(false, true)]
		[Tooltip("Global tint of the bloom filter.")]
		public ColorParameter color = new ColorParameter
		{
			value = Color.white
		};

		// Token: 0x04003677 RID: 13943
		[FormerlySerializedAs("mobileOptimized")]
		[Tooltip("Boost performance by lowering the effect quality. This settings is meant to be used on mobile and other low-end platforms but can also provide a nice performance boost on desktops and consoles.")]
		public BoolParameter fastMode = new BoolParameter
		{
			value = false
		};

		// Token: 0x04003678 RID: 13944
		[Tooltip("The lens dirt texture used to add smudges or dust to the bloom effect.")]
		[DisplayName("Texture")]
		public TextureParameter dirtTexture = new TextureParameter
		{
			value = null
		};

		// Token: 0x04003679 RID: 13945
		[Min(0f)]
		[Tooltip("The intensity of the lens dirtiness.")]
		[DisplayName("Intensity")]
		public FloatParameter dirtIntensity = new FloatParameter
		{
			value = 0f
		};
	}
}
