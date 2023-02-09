using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A1B RID: 2587
	[Preserve]
	internal sealed class DepthOfFieldRenderer : PostProcessEffectRenderer<DepthOfField>
	{
		// Token: 0x06003D77 RID: 15735 RVA: 0x001688AC File Offset: 0x00166AAC
		public DepthOfFieldRenderer()
		{
			for (int i = 0; i < 2; i++)
			{
				this.m_CoCHistoryTextures[i] = new RenderTexture[2];
				this.m_HistoryPingPong[i] = 0;
			}
		}

		// Token: 0x06003D78 RID: 15736 RVA: 0x00003A54 File Offset: 0x00001C54
		public override DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth;
		}

		// Token: 0x06003D79 RID: 15737 RVA: 0x001688FA File Offset: 0x00166AFA
		private RenderTextureFormat SelectFormat(RenderTextureFormat primary, RenderTextureFormat secondary)
		{
			if (primary.IsSupported())
			{
				return primary;
			}
			if (secondary.IsSupported())
			{
				return secondary;
			}
			return RenderTextureFormat.Default;
		}

		// Token: 0x06003D7A RID: 15738 RVA: 0x00168914 File Offset: 0x00166B14
		private float CalculateMaxCoCRadius(int screenHeight)
		{
			float num = (float)base.settings.kernelSize.value * 4f + 6f;
			return Mathf.Min(0.05f, num / (float)screenHeight);
		}

		// Token: 0x06003D7B RID: 15739 RVA: 0x00168950 File Offset: 0x00166B50
		private RenderTexture CheckHistory(int eye, int id, PostProcessRenderContext context, RenderTextureFormat format)
		{
			RenderTexture renderTexture = this.m_CoCHistoryTextures[eye][id];
			if (this.m_ResetHistory || renderTexture == null || !renderTexture.IsCreated() || renderTexture.width != context.width || renderTexture.height != context.height)
			{
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = context.GetScreenSpaceTemporaryRT(0, format, RenderTextureReadWrite.Linear, 0, 0);
				renderTexture.name = string.Concat(new object[]
				{
					"CoC History, Eye: ",
					eye,
					", ID: ",
					id
				});
				renderTexture.filterMode = FilterMode.Bilinear;
				renderTexture.Create();
				this.m_CoCHistoryTextures[eye][id] = renderTexture;
			}
			return renderTexture;
		}

		// Token: 0x06003D7C RID: 15740 RVA: 0x00168A00 File Offset: 0x00166C00
		public override void Render(PostProcessRenderContext context)
		{
			RenderTextureFormat sourceFormat = context.sourceFormat;
			RenderTextureFormat renderTextureFormat = this.SelectFormat(RenderTextureFormat.R8, RenderTextureFormat.RHalf);
			float num = 0.024f * ((float)context.height / 1080f);
			float num2 = base.settings.focalLength.value / 1000f;
			float num3 = Mathf.Max(base.settings.focusDistance.value, num2);
			float num4 = (float)context.screenWidth / (float)context.screenHeight;
			float value = num2 * num2 / (base.settings.aperture.value * (num3 - num2) * num * 2f);
			float num5 = this.CalculateMaxCoCRadius(context.screenHeight);
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.depthOfField);
			propertySheet.properties.Clear();
			propertySheet.properties.SetFloat(ShaderIDs.Distance, num3);
			propertySheet.properties.SetFloat(ShaderIDs.LensCoeff, value);
			propertySheet.properties.SetFloat(ShaderIDs.MaxCoC, num5);
			propertySheet.properties.SetFloat(ShaderIDs.RcpMaxCoC, 1f / num5);
			propertySheet.properties.SetFloat(ShaderIDs.RcpAspect, 1f / num4);
			CommandBuffer command = context.command;
			command.BeginSample("DepthOfField");
			context.GetScreenSpaceTemporaryRT(command, ShaderIDs.CoCTex, 0, renderTextureFormat, RenderTextureReadWrite.Linear, FilterMode.Bilinear, 0, 0);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, ShaderIDs.CoCTex, propertySheet, 0, false, null);
			if (context.IsTemporalAntialiasingActive() || context.dlssEnabled)
			{
				float motionBlending = context.temporalAntialiasing.motionBlending;
				float z = this.m_ResetHistory ? 0f : motionBlending;
				Vector2 jitter = context.temporalAntialiasing.jitter;
				propertySheet.properties.SetVector(ShaderIDs.TaaParams, new Vector3(jitter.x, jitter.y, z));
				int num6 = this.m_HistoryPingPong[context.xrActiveEye];
				RenderTexture tex = this.CheckHistory(context.xrActiveEye, ++num6 % 2, context, renderTextureFormat);
				RenderTexture tex2 = this.CheckHistory(context.xrActiveEye, ++num6 % 2, context, renderTextureFormat);
				this.m_HistoryPingPong[context.xrActiveEye] = (num6 + 1) % 2;
				command.BlitFullscreenTriangle(tex, tex2, propertySheet, 1, false, null);
				command.ReleaseTemporaryRT(ShaderIDs.CoCTex);
				command.SetGlobalTexture(ShaderIDs.CoCTex, tex2);
			}
			context.GetScreenSpaceTemporaryRT(command, ShaderIDs.DepthOfFieldTex, 0, sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, context.width / 2, context.height / 2);
			command.BlitFullscreenTriangle(context.source, ShaderIDs.DepthOfFieldTex, propertySheet, 2, false, null);
			context.GetScreenSpaceTemporaryRT(command, ShaderIDs.DepthOfFieldTemp, 0, sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, context.width / 2, context.height / 2);
			command.BlitFullscreenTriangle(ShaderIDs.DepthOfFieldTex, ShaderIDs.DepthOfFieldTemp, propertySheet, (int)(3 + base.settings.kernelSize.value), false, null);
			command.BlitFullscreenTriangle(ShaderIDs.DepthOfFieldTemp, ShaderIDs.DepthOfFieldTex, propertySheet, 7, false, null);
			command.ReleaseTemporaryRT(ShaderIDs.DepthOfFieldTemp);
			if (context.IsDebugOverlayEnabled(DebugOverlay.DepthOfField))
			{
				context.PushDebugOverlay(command, context.source, propertySheet, 9);
			}
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 8, false, null);
			command.ReleaseTemporaryRT(ShaderIDs.DepthOfFieldTex);
			if (!context.IsTemporalAntialiasingActive() || context.dlssEnabled)
			{
				command.ReleaseTemporaryRT(ShaderIDs.CoCTex);
			}
			command.EndSample("DepthOfField");
			this.m_ResetHistory = false;
		}

		// Token: 0x06003D7D RID: 15741 RVA: 0x00168DD8 File Offset: 0x00166FD8
		public override void Release()
		{
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < this.m_CoCHistoryTextures[i].Length; j++)
				{
					RenderTexture.ReleaseTemporary(this.m_CoCHistoryTextures[i][j]);
					this.m_CoCHistoryTextures[i][j] = null;
				}
				this.m_HistoryPingPong[i] = 0;
			}
			this.ResetHistory();
		}

		// Token: 0x040036C0 RID: 14016
		private const int k_NumEyes = 2;

		// Token: 0x040036C1 RID: 14017
		private const int k_NumCoCHistoryTextures = 2;

		// Token: 0x040036C2 RID: 14018
		private readonly RenderTexture[][] m_CoCHistoryTextures = new RenderTexture[2][];

		// Token: 0x040036C3 RID: 14019
		private int[] m_HistoryPingPong = new int[2];

		// Token: 0x040036C4 RID: 14020
		private const float k_FilmHeight = 0.024f;

		// Token: 0x02000EC1 RID: 3777
		private enum Pass
		{
			// Token: 0x04004BD8 RID: 19416
			CoCCalculation,
			// Token: 0x04004BD9 RID: 19417
			CoCTemporalFilter,
			// Token: 0x04004BDA RID: 19418
			DownsampleAndPrefilter,
			// Token: 0x04004BDB RID: 19419
			BokehSmallKernel,
			// Token: 0x04004BDC RID: 19420
			BokehMediumKernel,
			// Token: 0x04004BDD RID: 19421
			BokehLargeKernel,
			// Token: 0x04004BDE RID: 19422
			BokehVeryLargeKernel,
			// Token: 0x04004BDF RID: 19423
			PostFilter,
			// Token: 0x04004BE0 RID: 19424
			Combine,
			// Token: 0x04004BE1 RID: 19425
			DebugOverlay
		}
	}
}
