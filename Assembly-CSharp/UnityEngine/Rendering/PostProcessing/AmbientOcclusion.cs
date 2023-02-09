using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A07 RID: 2567
	[PostProcess(typeof(AmbientOcclusionRenderer), "Unity/Ambient Occlusion", true)]
	[Serializable]
	public sealed class AmbientOcclusion : PostProcessEffectSettings
	{
		// Token: 0x06003D42 RID: 15682 RVA: 0x00165C28 File Offset: 0x00163E28
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			bool flag = this.enabled.value && this.intensity.value > 0f;
			if (this.mode.value == AmbientOcclusionMode.ScalableAmbientObscurance)
			{
				flag &= !RuntimeUtilities.scriptableRenderPipelineActive;
				if (context != null)
				{
					flag &= (context.resources.shaders.scalableAO && context.resources.shaders.scalableAO.isSupported);
				}
			}
			else if (this.mode.value == AmbientOcclusionMode.MultiScaleVolumetricObscurance)
			{
				if (context != null)
				{
					flag &= (context.resources.shaders.multiScaleAO && context.resources.shaders.multiScaleAO.isSupported && context.resources.computeShaders.multiScaleAODownsample1 && context.resources.computeShaders.multiScaleAODownsample2 && context.resources.computeShaders.multiScaleAORender && context.resources.computeShaders.multiScaleAOUpsample);
				}
				flag &= (SystemInfo.supportsComputeShaders && !RuntimeUtilities.isAndroidOpenGL && RenderTextureFormat.RFloat.IsSupported() && RenderTextureFormat.RHalf.IsSupported() && RenderTextureFormat.R8.IsSupported());
			}
			return flag;
		}

		// Token: 0x04003655 RID: 13909
		[Tooltip("The ambient occlusion method to use. \"Multi Scale Volumetric Obscurance\" is higher quality and faster on desktop & console platforms but requires compute shader support.")]
		public AmbientOcclusionModeParameter mode = new AmbientOcclusionModeParameter
		{
			value = AmbientOcclusionMode.MultiScaleVolumetricObscurance
		};

		// Token: 0x04003656 RID: 13910
		[Range(0f, 4f)]
		[Tooltip("The degree of darkness added by ambient occlusion. Higher values produce darker areas.")]
		public FloatParameter intensity = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x04003657 RID: 13911
		[ColorUsage(false)]
		[Tooltip("The custom color to use for the ambient occlusion. The default is black.")]
		public ColorParameter color = new ColorParameter
		{
			value = Color.black
		};

		// Token: 0x04003658 RID: 13912
		[Tooltip("Check this box to mark this Volume as to only affect ambient lighting. This mode is only available with the Deferred rendering path and HDR rendering. Objects rendered with the Forward rendering path won't get any ambient occlusion.")]
		public BoolParameter ambientOnly = new BoolParameter
		{
			value = true
		};

		// Token: 0x04003659 RID: 13913
		[Range(-8f, 0f)]
		public FloatParameter noiseFilterTolerance = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x0400365A RID: 13914
		[Range(-8f, -1f)]
		public FloatParameter blurTolerance = new FloatParameter
		{
			value = -4.6f
		};

		// Token: 0x0400365B RID: 13915
		[Range(-12f, -1f)]
		public FloatParameter upsampleTolerance = new FloatParameter
		{
			value = -12f
		};

		// Token: 0x0400365C RID: 13916
		[Range(1f, 10f)]
		[Tooltip("This modifies the thickness of occluders. It increases the size of dark areas and also introduces a dark halo around objects.")]
		public FloatParameter thicknessModifier = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x0400365D RID: 13917
		[Range(0f, 1f)]
		[Tooltip("Modifies the influence of direct lighting on ambient occlusion.")]
		public FloatParameter directLightingStrength = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x0400365E RID: 13918
		[Tooltip("The radius of sample points. This affects the size of darkened areas.")]
		public FloatParameter radius = new FloatParameter
		{
			value = 0.25f
		};

		// Token: 0x0400365F RID: 13919
		[Tooltip("The number of sample points. This affects both quality and performance. For \"Lowest\", \"Low\", and \"Medium\", passes are downsampled. For \"High\" and \"Ultra\", they are not and therefore you should only \"High\" and \"Ultra\" on high-end hardware.")]
		public AmbientOcclusionQualityParameter quality = new AmbientOcclusionQualityParameter
		{
			value = AmbientOcclusionQuality.Medium
		};
	}
}
