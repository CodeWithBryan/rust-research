using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A0F RID: 2575
	[Preserve]
	internal sealed class BloomRenderer : PostProcessEffectRenderer<Bloom>
	{
		// Token: 0x06003D5B RID: 15707 RVA: 0x00166598 File Offset: 0x00164798
		public override void Init()
		{
			this.m_Pyramid = new BloomRenderer.Level[16];
			for (int i = 0; i < 16; i++)
			{
				this.m_Pyramid[i] = new BloomRenderer.Level
				{
					down = Shader.PropertyToID("_BloomMipDown" + i),
					up = Shader.PropertyToID("_BloomMipUp" + i)
				};
			}
		}

		// Token: 0x06003D5C RID: 15708 RVA: 0x0016660C File Offset: 0x0016480C
		public override void Render(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			command.BeginSample("BloomPyramid");
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.bloom);
			propertySheet.properties.SetTexture(ShaderIDs.AutoExposureTex, context.autoExposureTexture);
			float num = Mathf.Clamp(base.settings.anamorphicRatio, -1f, 1f);
			float num2 = (num < 0f) ? (-num) : 0f;
			float num3 = (num > 0f) ? num : 0f;
			int num4 = Mathf.FloorToInt((float)context.screenWidth / (2f - num2));
			int num5 = Mathf.FloorToInt((float)context.screenHeight / (2f - num3));
			bool flag = context.stereoActive && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass && context.camera.stereoTargetEye == StereoTargetEyeMask.Both;
			int num6 = flag ? (num4 * 2) : num4;
			float num7 = Mathf.Log((float)Mathf.Max(num4, num5), 2f) + Mathf.Min(base.settings.diffusion.value, 10f) - 10f;
			int num8 = Mathf.FloorToInt(num7);
			int num9 = Mathf.Clamp(num8, 1, 16);
			float num10 = 0.5f + num7 - (float)num8;
			propertySheet.properties.SetFloat(ShaderIDs.SampleScale, num10);
			float num11 = Mathf.GammaToLinearSpace(base.settings.threshold.value);
			float num12 = num11 * base.settings.softKnee.value + 1E-05f;
			Vector4 value = new Vector4(num11, num11 - num12, num12 * 2f, 0.25f / num12);
			propertySheet.properties.SetVector(ShaderIDs.Threshold, value);
			float x = Mathf.GammaToLinearSpace(base.settings.clamp.value);
			propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4(x, 0f, 0f, 0f));
			int num13 = base.settings.fastMode ? 1 : 0;
			RenderTargetIdentifier source = context.source;
			for (int i = 0; i < num9; i++)
			{
				int down = this.m_Pyramid[i].down;
				int up = this.m_Pyramid[i].up;
				int pass = (i == 0) ? num13 : (2 + num13);
				context.GetScreenSpaceTemporaryRT(command, down, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, num6, num5);
				context.GetScreenSpaceTemporaryRT(command, up, 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, num6, num5);
				command.BlitFullscreenTriangle(source, down, propertySheet, pass, false, null);
				source = down;
				num6 = ((flag && num6 / 2 % 2 > 0) ? (1 + num6 / 2) : (num6 / 2));
				num6 = Mathf.Max(num6, 1);
				num5 = Mathf.Max(num5 / 2, 1);
			}
			int num14 = this.m_Pyramid[num9 - 1].down;
			for (int j = num9 - 2; j >= 0; j--)
			{
				int down2 = this.m_Pyramid[j].down;
				int up2 = this.m_Pyramid[j].up;
				command.SetGlobalTexture(ShaderIDs.BloomTex, down2);
				command.BlitFullscreenTriangle(num14, up2, propertySheet, 4 + num13, false, null);
				num14 = up2;
			}
			Color linear = base.settings.color.value.linear;
			float num15 = RuntimeUtilities.Exp2(base.settings.intensity.value / 10f) - 1f;
			Vector4 value2 = new Vector4(num10, num15, base.settings.dirtIntensity.value, (float)num9);
			if (context.IsDebugOverlayEnabled(DebugOverlay.BloomThreshold))
			{
				context.PushDebugOverlay(command, context.source, propertySheet, 6);
			}
			else if (context.IsDebugOverlayEnabled(DebugOverlay.BloomBuffer))
			{
				propertySheet.properties.SetVector(ShaderIDs.ColorIntensity, new Vector4(linear.r, linear.g, linear.b, num15));
				context.PushDebugOverlay(command, this.m_Pyramid[0].up, propertySheet, 7 + num13);
			}
			Texture texture = (base.settings.dirtTexture.value == null) ? RuntimeUtilities.blackTexture : base.settings.dirtTexture.value;
			float num16 = (float)texture.width / (float)texture.height;
			float num17 = (float)context.screenWidth / (float)context.screenHeight;
			Vector4 vector = new Vector4(1f, 1f, 0f, 0f);
			if (num16 > num17)
			{
				vector.x = num17 / num16;
				vector.z = (1f - vector.x) * 0.5f;
			}
			else if (num17 > num16)
			{
				vector.y = num16 / num17;
				vector.w = (1f - vector.y) * 0.5f;
			}
			PropertySheet uberSheet = context.uberSheet;
			if (base.settings.fastMode)
			{
				uberSheet.EnableKeyword("BLOOM_LOW");
			}
			else
			{
				uberSheet.EnableKeyword("BLOOM");
			}
			uberSheet.properties.SetVector(ShaderIDs.Bloom_DirtTileOffset, vector);
			uberSheet.properties.SetVector(ShaderIDs.Bloom_Settings, value2);
			uberSheet.properties.SetColor(ShaderIDs.Bloom_Color, linear);
			uberSheet.properties.SetTexture(ShaderIDs.Bloom_DirtTex, texture);
			command.SetGlobalTexture(ShaderIDs.BloomTex, num14);
			for (int k = 0; k < num9; k++)
			{
				if (this.m_Pyramid[k].down != num14)
				{
					command.ReleaseTemporaryRT(this.m_Pyramid[k].down);
				}
				if (this.m_Pyramid[k].up != num14)
				{
					command.ReleaseTemporaryRT(this.m_Pyramid[k].up);
				}
			}
			command.EndSample("BloomPyramid");
			context.bloomBufferNameID = num14;
		}

		// Token: 0x0400367A RID: 13946
		private BloomRenderer.Level[] m_Pyramid;

		// Token: 0x0400367B RID: 13947
		private const int k_MaxPyramidSize = 16;

		// Token: 0x02000EBE RID: 3774
		private enum Pass
		{
			// Token: 0x04004BC8 RID: 19400
			Prefilter13,
			// Token: 0x04004BC9 RID: 19401
			Prefilter4,
			// Token: 0x04004BCA RID: 19402
			Downsample13,
			// Token: 0x04004BCB RID: 19403
			Downsample4,
			// Token: 0x04004BCC RID: 19404
			UpsampleTent,
			// Token: 0x04004BCD RID: 19405
			UpsampleBox,
			// Token: 0x04004BCE RID: 19406
			DebugOverlayThreshold,
			// Token: 0x04004BCF RID: 19407
			DebugOverlayTent,
			// Token: 0x04004BD0 RID: 19408
			DebugOverlayBox
		}

		// Token: 0x02000EBF RID: 3775
		private struct Level
		{
			// Token: 0x04004BD1 RID: 19409
			internal int down;

			// Token: 0x04004BD2 RID: 19410
			internal int up;
		}
	}
}
