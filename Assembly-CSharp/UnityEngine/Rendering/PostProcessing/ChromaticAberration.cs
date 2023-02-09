using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A10 RID: 2576
	[PostProcess(typeof(ChromaticAberrationRenderer), "Unity/Chromatic Aberration", true)]
	[Serializable]
	public sealed class ChromaticAberration : PostProcessEffectSettings
	{
		// Token: 0x06003D5E RID: 15710 RVA: 0x00166C34 File Offset: 0x00164E34
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled.value && this.intensity.value > 0f;
		}

		// Token: 0x0400367C RID: 13948
		[Tooltip("Shifts the hue of chromatic aberrations.")]
		public TextureParameter spectralLut = new TextureParameter
		{
			value = null
		};

		// Token: 0x0400367D RID: 13949
		[Range(0f, 1f)]
		[Tooltip("Amount of tangential distortion.")]
		public FloatParameter intensity = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x0400367E RID: 13950
		[FormerlySerializedAs("mobileOptimized")]
		[Tooltip("Boost performances by lowering the effect quality. This settings is meant to be used on mobile and other low-end platforms but can also provide a nice performance boost on desktops and consoles.")]
		public BoolParameter fastMode = new BoolParameter
		{
			value = false
		};
	}
}
