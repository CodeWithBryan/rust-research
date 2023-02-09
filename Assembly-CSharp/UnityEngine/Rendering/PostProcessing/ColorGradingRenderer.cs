using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A17 RID: 2583
	[Preserve]
	internal sealed class ColorGradingRenderer : PostProcessEffectRenderer<ColorGrading>
	{
		// Token: 0x06003D67 RID: 15719 RVA: 0x001673E4 File Offset: 0x001655E4
		public override void Render(PostProcessRenderContext context)
		{
			GradingMode value = base.settings.gradingMode.value;
			bool flag = SystemInfo.supports3DRenderTextures && SystemInfo.supportsComputeShaders && context.resources.computeShaders.lut3DBaker != null && SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLCore && SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES3;
			if (value == GradingMode.External)
			{
				this.RenderExternalPipeline3D(context);
				return;
			}
			if (value == GradingMode.HighDefinitionRange && flag)
			{
				this.RenderHDRPipeline3D(context);
				return;
			}
			if (value == GradingMode.HighDefinitionRange)
			{
				this.RenderHDRPipeline2D(context);
				return;
			}
			this.RenderLDRPipeline2D(context);
		}

		// Token: 0x06003D68 RID: 15720 RVA: 0x00167470 File Offset: 0x00165670
		private void RenderExternalPipeline3D(PostProcessRenderContext context)
		{
			Texture value = base.settings.externalLut.value;
			if (value == null)
			{
				return;
			}
			PropertySheet uberSheet = context.uberSheet;
			uberSheet.EnableKeyword("COLOR_GRADING_HDR_3D");
			uberSheet.properties.SetTexture(ShaderIDs.Lut3D, value);
			uberSheet.properties.SetVector(ShaderIDs.Lut3D_Params, new Vector2(1f / (float)value.width, (float)value.width - 1f));
			uberSheet.properties.SetFloat(ShaderIDs.PostExposure, RuntimeUtilities.Exp2(base.settings.postExposure.value));
			context.logLut = value;
		}

		// Token: 0x06003D69 RID: 15721 RVA: 0x0016751C File Offset: 0x0016571C
		private void RenderHDRPipeline3D(PostProcessRenderContext context)
		{
			this.CheckInternalLogLut();
			ComputeShader lut3DBaker = context.resources.computeShaders.lut3DBaker;
			int kernelIndex = 0;
			switch (base.settings.tonemapper.value)
			{
			case Tonemapper.None:
				kernelIndex = lut3DBaker.FindKernel("KGenLut3D_NoTonemap");
				break;
			case Tonemapper.Neutral:
				kernelIndex = lut3DBaker.FindKernel("KGenLut3D_NeutralTonemap");
				break;
			case Tonemapper.ACES:
				kernelIndex = lut3DBaker.FindKernel("KGenLut3D_AcesTonemap");
				break;
			case Tonemapper.Custom:
				kernelIndex = lut3DBaker.FindKernel("KGenLut3D_CustomTonemap");
				break;
			}
			CommandBuffer command = context.command;
			command.SetComputeTextureParam(lut3DBaker, kernelIndex, "_Output", this.m_InternalLogLut);
			command.SetComputeVectorParam(lut3DBaker, "_Size", new Vector4(33f, 0.03125f, 0f, 0f));
			Vector3 v = ColorUtilities.ComputeColorBalance(base.settings.temperature.value, base.settings.tint.value);
			command.SetComputeVectorParam(lut3DBaker, "_ColorBalance", v);
			command.SetComputeVectorParam(lut3DBaker, "_ColorFilter", base.settings.colorFilter.value);
			float x = base.settings.hueShift.value / 360f;
			float y = base.settings.saturation.value / 100f + 1f;
			float z = base.settings.contrast.value / 100f + 1f;
			command.SetComputeVectorParam(lut3DBaker, "_HueSatCon", new Vector4(x, y, z, 0f));
			Vector4 a = new Vector4(base.settings.mixerRedOutRedIn, base.settings.mixerRedOutGreenIn, base.settings.mixerRedOutBlueIn, 0f);
			Vector4 a2 = new Vector4(base.settings.mixerGreenOutRedIn, base.settings.mixerGreenOutGreenIn, base.settings.mixerGreenOutBlueIn, 0f);
			Vector4 a3 = new Vector4(base.settings.mixerBlueOutRedIn, base.settings.mixerBlueOutGreenIn, base.settings.mixerBlueOutBlueIn, 0f);
			command.SetComputeVectorParam(lut3DBaker, "_ChannelMixerRed", a / 100f);
			command.SetComputeVectorParam(lut3DBaker, "_ChannelMixerGreen", a2 / 100f);
			command.SetComputeVectorParam(lut3DBaker, "_ChannelMixerBlue", a3 / 100f);
			Vector3 vector = ColorUtilities.ColorToLift(base.settings.lift.value * 0.2f);
			Vector3 vector2 = ColorUtilities.ColorToGain(base.settings.gain.value * 0.8f);
			Vector3 vector3 = ColorUtilities.ColorToInverseGamma(base.settings.gamma.value * 0.8f);
			command.SetComputeVectorParam(lut3DBaker, "_Lift", new Vector4(vector.x, vector.y, vector.z, 0f));
			command.SetComputeVectorParam(lut3DBaker, "_InvGamma", new Vector4(vector3.x, vector3.y, vector3.z, 0f));
			command.SetComputeVectorParam(lut3DBaker, "_Gain", new Vector4(vector2.x, vector2.y, vector2.z, 0f));
			command.SetComputeTextureParam(lut3DBaker, kernelIndex, "_Curves", this.GetCurveTexture(true));
			if (base.settings.tonemapper.value == Tonemapper.Custom)
			{
				this.m_HableCurve.Init(base.settings.toneCurveToeStrength.value, base.settings.toneCurveToeLength.value, base.settings.toneCurveShoulderStrength.value, base.settings.toneCurveShoulderLength.value, base.settings.toneCurveShoulderAngle.value, base.settings.toneCurveGamma.value);
				command.SetComputeVectorParam(lut3DBaker, "_CustomToneCurve", this.m_HableCurve.uniforms.curve);
				command.SetComputeVectorParam(lut3DBaker, "_ToeSegmentA", this.m_HableCurve.uniforms.toeSegmentA);
				command.SetComputeVectorParam(lut3DBaker, "_ToeSegmentB", this.m_HableCurve.uniforms.toeSegmentB);
				command.SetComputeVectorParam(lut3DBaker, "_MidSegmentA", this.m_HableCurve.uniforms.midSegmentA);
				command.SetComputeVectorParam(lut3DBaker, "_MidSegmentB", this.m_HableCurve.uniforms.midSegmentB);
				command.SetComputeVectorParam(lut3DBaker, "_ShoSegmentA", this.m_HableCurve.uniforms.shoSegmentA);
				command.SetComputeVectorParam(lut3DBaker, "_ShoSegmentB", this.m_HableCurve.uniforms.shoSegmentB);
			}
			context.command.BeginSample("HdrColorGradingLut3D");
			int num = Mathf.CeilToInt(8.25f);
			command.DispatchCompute(lut3DBaker, kernelIndex, num, num, num);
			context.command.EndSample("HdrColorGradingLut3D");
			RenderTexture internalLogLut = this.m_InternalLogLut;
			PropertySheet uberSheet = context.uberSheet;
			uberSheet.EnableKeyword("COLOR_GRADING_HDR_3D");
			uberSheet.properties.SetTexture(ShaderIDs.Lut3D, internalLogLut);
			uberSheet.properties.SetVector(ShaderIDs.Lut3D_Params, new Vector2(1f / (float)internalLogLut.width, (float)internalLogLut.width - 1f));
			uberSheet.properties.SetFloat(ShaderIDs.PostExposure, RuntimeUtilities.Exp2(base.settings.postExposure.value));
			context.logLut = internalLogLut;
		}

		// Token: 0x06003D6A RID: 15722 RVA: 0x00167AB4 File Offset: 0x00165CB4
		private void RenderHDRPipeline2D(PostProcessRenderContext context)
		{
			this.CheckInternalStripLut();
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.lut2DBaker);
			propertySheet.ClearKeywords();
			propertySheet.properties.SetVector(ShaderIDs.Lut2D_Params, new Vector4(32f, 0.00048828125f, 0.015625f, 1.032258f));
			Vector3 v = ColorUtilities.ComputeColorBalance(base.settings.temperature.value, base.settings.tint.value);
			propertySheet.properties.SetVector(ShaderIDs.ColorBalance, v);
			propertySheet.properties.SetVector(ShaderIDs.ColorFilter, base.settings.colorFilter.value);
			float x = base.settings.hueShift.value / 360f;
			float y = base.settings.saturation.value / 100f + 1f;
			float z = base.settings.contrast.value / 100f + 1f;
			propertySheet.properties.SetVector(ShaderIDs.HueSatCon, new Vector3(x, y, z));
			Vector3 a = new Vector3(base.settings.mixerRedOutRedIn, base.settings.mixerRedOutGreenIn, base.settings.mixerRedOutBlueIn);
			Vector3 a2 = new Vector3(base.settings.mixerGreenOutRedIn, base.settings.mixerGreenOutGreenIn, base.settings.mixerGreenOutBlueIn);
			Vector3 a3 = new Vector3(base.settings.mixerBlueOutRedIn, base.settings.mixerBlueOutGreenIn, base.settings.mixerBlueOutBlueIn);
			propertySheet.properties.SetVector(ShaderIDs.ChannelMixerRed, a / 100f);
			propertySheet.properties.SetVector(ShaderIDs.ChannelMixerGreen, a2 / 100f);
			propertySheet.properties.SetVector(ShaderIDs.ChannelMixerBlue, a3 / 100f);
			Vector3 v2 = ColorUtilities.ColorToLift(base.settings.lift.value * 0.2f);
			Vector3 v3 = ColorUtilities.ColorToGain(base.settings.gain.value * 0.8f);
			Vector3 v4 = ColorUtilities.ColorToInverseGamma(base.settings.gamma.value * 0.8f);
			propertySheet.properties.SetVector(ShaderIDs.Lift, v2);
			propertySheet.properties.SetVector(ShaderIDs.InvGamma, v4);
			propertySheet.properties.SetVector(ShaderIDs.Gain, v3);
			propertySheet.properties.SetTexture(ShaderIDs.Curves, this.GetCurveTexture(true));
			Tonemapper value = base.settings.tonemapper.value;
			if (value == Tonemapper.Custom)
			{
				propertySheet.EnableKeyword("TONEMAPPING_CUSTOM");
				this.m_HableCurve.Init(base.settings.toneCurveToeStrength.value, base.settings.toneCurveToeLength.value, base.settings.toneCurveShoulderStrength.value, base.settings.toneCurveShoulderLength.value, base.settings.toneCurveShoulderAngle.value, base.settings.toneCurveGamma.value);
				propertySheet.properties.SetVector(ShaderIDs.CustomToneCurve, this.m_HableCurve.uniforms.curve);
				propertySheet.properties.SetVector(ShaderIDs.ToeSegmentA, this.m_HableCurve.uniforms.toeSegmentA);
				propertySheet.properties.SetVector(ShaderIDs.ToeSegmentB, this.m_HableCurve.uniforms.toeSegmentB);
				propertySheet.properties.SetVector(ShaderIDs.MidSegmentA, this.m_HableCurve.uniforms.midSegmentA);
				propertySheet.properties.SetVector(ShaderIDs.MidSegmentB, this.m_HableCurve.uniforms.midSegmentB);
				propertySheet.properties.SetVector(ShaderIDs.ShoSegmentA, this.m_HableCurve.uniforms.shoSegmentA);
				propertySheet.properties.SetVector(ShaderIDs.ShoSegmentB, this.m_HableCurve.uniforms.shoSegmentB);
			}
			else if (value == Tonemapper.ACES)
			{
				propertySheet.EnableKeyword("TONEMAPPING_ACES");
			}
			else if (value == Tonemapper.Neutral)
			{
				propertySheet.EnableKeyword("TONEMAPPING_NEUTRAL");
			}
			context.command.BeginSample("HdrColorGradingLut2D");
			context.command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, this.m_InternalLdrLut, propertySheet, 2, false, null);
			context.command.EndSample("HdrColorGradingLut2D");
			RenderTexture internalLdrLut = this.m_InternalLdrLut;
			PropertySheet uberSheet = context.uberSheet;
			uberSheet.EnableKeyword("COLOR_GRADING_HDR_2D");
			uberSheet.properties.SetVector(ShaderIDs.Lut2D_Params, new Vector3(1f / (float)internalLdrLut.width, 1f / (float)internalLdrLut.height, (float)internalLdrLut.height - 1f));
			uberSheet.properties.SetTexture(ShaderIDs.Lut2D, internalLdrLut);
			uberSheet.properties.SetFloat(ShaderIDs.PostExposure, RuntimeUtilities.Exp2(base.settings.postExposure.value));
		}

		// Token: 0x06003D6B RID: 15723 RVA: 0x00168018 File Offset: 0x00166218
		private void RenderLDRPipeline2D(PostProcessRenderContext context)
		{
			this.CheckInternalStripLut();
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.lut2DBaker);
			propertySheet.ClearKeywords();
			propertySheet.properties.SetVector(ShaderIDs.Lut2D_Params, new Vector4(32f, 0.00048828125f, 0.015625f, 1.032258f));
			Vector3 v = ColorUtilities.ComputeColorBalance(base.settings.temperature.value, base.settings.tint.value);
			propertySheet.properties.SetVector(ShaderIDs.ColorBalance, v);
			propertySheet.properties.SetVector(ShaderIDs.ColorFilter, base.settings.colorFilter.value);
			float x = base.settings.hueShift.value / 360f;
			float y = base.settings.saturation.value / 100f + 1f;
			float z = base.settings.contrast.value / 100f + 1f;
			propertySheet.properties.SetVector(ShaderIDs.HueSatCon, new Vector3(x, y, z));
			Vector3 a = new Vector3(base.settings.mixerRedOutRedIn, base.settings.mixerRedOutGreenIn, base.settings.mixerRedOutBlueIn);
			Vector3 a2 = new Vector3(base.settings.mixerGreenOutRedIn, base.settings.mixerGreenOutGreenIn, base.settings.mixerGreenOutBlueIn);
			Vector3 a3 = new Vector3(base.settings.mixerBlueOutRedIn, base.settings.mixerBlueOutGreenIn, base.settings.mixerBlueOutBlueIn);
			propertySheet.properties.SetVector(ShaderIDs.ChannelMixerRed, a / 100f);
			propertySheet.properties.SetVector(ShaderIDs.ChannelMixerGreen, a2 / 100f);
			propertySheet.properties.SetVector(ShaderIDs.ChannelMixerBlue, a3 / 100f);
			Vector3 v2 = ColorUtilities.ColorToLift(base.settings.lift.value);
			Vector3 v3 = ColorUtilities.ColorToGain(base.settings.gain.value);
			Vector3 v4 = ColorUtilities.ColorToInverseGamma(base.settings.gamma.value);
			propertySheet.properties.SetVector(ShaderIDs.Lift, v2);
			propertySheet.properties.SetVector(ShaderIDs.InvGamma, v4);
			propertySheet.properties.SetVector(ShaderIDs.Gain, v3);
			propertySheet.properties.SetFloat(ShaderIDs.Brightness, (base.settings.brightness.value + 100f) / 100f);
			propertySheet.properties.SetTexture(ShaderIDs.Curves, this.GetCurveTexture(false));
			context.command.BeginSample("LdrColorGradingLut2D");
			Texture value = base.settings.ldrLut.value;
			if (value == null || value.width != value.height * value.height)
			{
				context.command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, this.m_InternalLdrLut, propertySheet, 0, false, null);
			}
			else
			{
				propertySheet.properties.SetVector(ShaderIDs.UserLut2D_Params, new Vector4(1f / (float)value.width, 1f / (float)value.height, (float)value.height - 1f, base.settings.ldrLutContribution));
				context.command.BlitFullscreenTriangle(value, this.m_InternalLdrLut, propertySheet, 1, false, null);
			}
			context.command.EndSample("LdrColorGradingLut2D");
			RenderTexture internalLdrLut = this.m_InternalLdrLut;
			PropertySheet uberSheet = context.uberSheet;
			uberSheet.EnableKeyword("COLOR_GRADING_LDR_2D");
			uberSheet.properties.SetVector(ShaderIDs.Lut2D_Params, new Vector3(1f / (float)internalLdrLut.width, 1f / (float)internalLdrLut.height, (float)internalLdrLut.height - 1f));
			uberSheet.properties.SetTexture(ShaderIDs.Lut2D, internalLdrLut);
		}

		// Token: 0x06003D6C RID: 15724 RVA: 0x00168480 File Offset: 0x00166680
		private void CheckInternalLogLut()
		{
			if (this.m_InternalLogLut == null || !this.m_InternalLogLut.IsCreated())
			{
				RuntimeUtilities.Destroy(this.m_InternalLogLut);
				RenderTextureFormat lutFormat = ColorGradingRenderer.GetLutFormat();
				this.m_InternalLogLut = new RenderTexture(33, 33, 0, lutFormat, RenderTextureReadWrite.Linear)
				{
					name = "Color Grading Log Lut",
					dimension = TextureDimension.Tex3D,
					hideFlags = HideFlags.DontSave,
					filterMode = FilterMode.Bilinear,
					wrapMode = TextureWrapMode.Clamp,
					anisoLevel = 0,
					enableRandomWrite = true,
					volumeDepth = 33,
					autoGenerateMips = false,
					useMipMap = false
				};
				this.m_InternalLogLut.Create();
			}
		}

		// Token: 0x06003D6D RID: 15725 RVA: 0x00168524 File Offset: 0x00166724
		private void CheckInternalStripLut()
		{
			if (this.m_InternalLdrLut == null || !this.m_InternalLdrLut.IsCreated())
			{
				RuntimeUtilities.Destroy(this.m_InternalLdrLut);
				RenderTextureFormat lutFormat = ColorGradingRenderer.GetLutFormat();
				this.m_InternalLdrLut = new RenderTexture(1024, 32, 0, lutFormat, RenderTextureReadWrite.Linear)
				{
					name = "Color Grading Strip Lut",
					hideFlags = HideFlags.DontSave,
					filterMode = FilterMode.Bilinear,
					wrapMode = TextureWrapMode.Clamp,
					anisoLevel = 0,
					autoGenerateMips = false,
					useMipMap = false
				};
				this.m_InternalLdrLut.Create();
			}
		}

		// Token: 0x06003D6E RID: 15726 RVA: 0x001685B4 File Offset: 0x001667B4
		private Texture2D GetCurveTexture(bool hdr)
		{
			if (this.m_GradingCurves == null)
			{
				TextureFormat curveFormat = ColorGradingRenderer.GetCurveFormat();
				this.m_GradingCurves = new Texture2D(128, 2, curveFormat, false, true)
				{
					name = "Internal Curves Texture",
					hideFlags = HideFlags.DontSave,
					anisoLevel = 0,
					wrapMode = TextureWrapMode.Clamp,
					filterMode = FilterMode.Bilinear
				};
			}
			Spline value = base.settings.hueVsHueCurve.value;
			Spline value2 = base.settings.hueVsSatCurve.value;
			Spline value3 = base.settings.satVsSatCurve.value;
			Spline value4 = base.settings.lumVsSatCurve.value;
			Spline value5 = base.settings.masterCurve.value;
			Spline value6 = base.settings.redCurve.value;
			Spline value7 = base.settings.greenCurve.value;
			Spline value8 = base.settings.blueCurve.value;
			Color[] pixels = this.m_Pixels;
			for (int i = 0; i < 128; i++)
			{
				float r = value.cachedData[i];
				float g = value2.cachedData[i];
				float b = value3.cachedData[i];
				float a = value4.cachedData[i];
				pixels[i] = new Color(r, g, b, a);
				if (!hdr)
				{
					float a2 = value5.cachedData[i];
					float r2 = value6.cachedData[i];
					float g2 = value7.cachedData[i];
					float b2 = value8.cachedData[i];
					pixels[i + 128] = new Color(r2, g2, b2, a2);
				}
			}
			this.m_GradingCurves.SetPixels(pixels);
			this.m_GradingCurves.Apply(false, false);
			return this.m_GradingCurves;
		}

		// Token: 0x06003D6F RID: 15727 RVA: 0x00168772 File Offset: 0x00166972
		private static bool IsRenderTextureFormatSupportedForLinearFiltering(RenderTextureFormat format)
		{
			return SystemInfo.IsFormatSupported(GraphicsFormatUtility.GetGraphicsFormat(format, RenderTextureReadWrite.Linear), FormatUsage.Linear);
		}

		// Token: 0x06003D70 RID: 15728 RVA: 0x00168784 File Offset: 0x00166984
		private static RenderTextureFormat GetLutFormat()
		{
			RenderTextureFormat renderTextureFormat = RenderTextureFormat.ARGBHalf;
			if (!ColorGradingRenderer.IsRenderTextureFormatSupportedForLinearFiltering(renderTextureFormat))
			{
				renderTextureFormat = RenderTextureFormat.ARGB2101010;
				if (!ColorGradingRenderer.IsRenderTextureFormatSupportedForLinearFiltering(renderTextureFormat))
				{
					renderTextureFormat = RenderTextureFormat.ARGB32;
				}
			}
			return renderTextureFormat;
		}

		// Token: 0x06003D71 RID: 15729 RVA: 0x001687A8 File Offset: 0x001669A8
		private static TextureFormat GetCurveFormat()
		{
			TextureFormat textureFormat = TextureFormat.RGBAHalf;
			if (!SystemInfo.SupportsTextureFormat(textureFormat))
			{
				textureFormat = TextureFormat.ARGB32;
			}
			return textureFormat;
		}

		// Token: 0x06003D72 RID: 15730 RVA: 0x001687C3 File Offset: 0x001669C3
		public override void Release()
		{
			RuntimeUtilities.Destroy(this.m_InternalLdrLut);
			this.m_InternalLdrLut = null;
			RuntimeUtilities.Destroy(this.m_InternalLogLut);
			this.m_InternalLogLut = null;
			RuntimeUtilities.Destroy(this.m_GradingCurves);
			this.m_GradingCurves = null;
		}

		// Token: 0x040036B0 RID: 14000
		private Texture2D m_GradingCurves;

		// Token: 0x040036B1 RID: 14001
		private readonly Color[] m_Pixels = new Color[256];

		// Token: 0x040036B2 RID: 14002
		private RenderTexture m_InternalLdrLut;

		// Token: 0x040036B3 RID: 14003
		private RenderTexture m_InternalLogLut;

		// Token: 0x040036B4 RID: 14004
		private const int k_Lut2DSize = 32;

		// Token: 0x040036B5 RID: 14005
		private const int k_Lut3DSize = 33;

		// Token: 0x040036B6 RID: 14006
		private readonly HableCurve m_HableCurve = new HableCurve();

		// Token: 0x02000EC0 RID: 3776
		private enum Pass
		{
			// Token: 0x04004BD4 RID: 19412
			LutGenLDRFromScratch,
			// Token: 0x04004BD5 RID: 19413
			LutGenLDR,
			// Token: 0x04004BD6 RID: 19414
			LutGenHDR2D
		}
	}
}
