using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A24 RID: 2596
	[Preserve]
	internal sealed class MotionBlurRenderer : PostProcessEffectRenderer<MotionBlur>
	{
		// Token: 0x06003D91 RID: 15761 RVA: 0x0001F1CE File Offset: 0x0001D3CE
		public override DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
		}

		// Token: 0x06003D92 RID: 15762 RVA: 0x001695C8 File Offset: 0x001677C8
		public override void Render(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			if (this.m_ResetHistory)
			{
				command.BlitFullscreenTriangle(context.source, context.destination, false, null);
				this.m_ResetHistory = false;
				return;
			}
			RenderTextureFormat format = RenderTextureFormat.RGHalf;
			RenderTextureFormat format2 = RenderTextureFormat.ARGB2101010.IsSupported() ? RenderTextureFormat.ARGB2101010 : RenderTextureFormat.ARGB32;
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.motionBlur);
			command.BeginSample("MotionBlur");
			int num = (int)(5f * (float)context.height / 100f);
			int num2 = ((num - 1) / 8 + 1) * 8;
			float value = base.settings.shutterAngle / 360f;
			propertySheet.properties.SetFloat(ShaderIDs.VelocityScale, value);
			propertySheet.properties.SetFloat(ShaderIDs.MaxBlurRadius, (float)num);
			propertySheet.properties.SetFloat(ShaderIDs.RcpMaxBlurRadius, 1f / (float)num);
			int velocityTex = ShaderIDs.VelocityTex;
			command.GetTemporaryRT(velocityTex, context.width, context.height, 0, FilterMode.Point, format2, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, velocityTex, propertySheet, 0, false, null);
			int tile2RT = ShaderIDs.Tile2RT;
			command.GetTemporaryRT(tile2RT, context.width / 2, context.height / 2, 0, FilterMode.Point, format, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(velocityTex, tile2RT, propertySheet, 1, false, null);
			int tile4RT = ShaderIDs.Tile4RT;
			command.GetTemporaryRT(tile4RT, context.width / 4, context.height / 4, 0, FilterMode.Point, format, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(tile2RT, tile4RT, propertySheet, 2, false, null);
			command.ReleaseTemporaryRT(tile2RT);
			int tile8RT = ShaderIDs.Tile8RT;
			command.GetTemporaryRT(tile8RT, context.width / 8, context.height / 8, 0, FilterMode.Point, format, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(tile4RT, tile8RT, propertySheet, 2, false, null);
			command.ReleaseTemporaryRT(tile4RT);
			Vector2 v = Vector2.one * ((float)num2 / 8f - 1f) * -0.5f;
			propertySheet.properties.SetVector(ShaderIDs.TileMaxOffs, v);
			propertySheet.properties.SetFloat(ShaderIDs.TileMaxLoop, (float)((int)((float)num2 / 8f)));
			int tileVRT = ShaderIDs.TileVRT;
			command.GetTemporaryRT(tileVRT, context.width / num2, context.height / num2, 0, FilterMode.Point, format, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(tile8RT, tileVRT, propertySheet, 3, false, null);
			command.ReleaseTemporaryRT(tile8RT);
			int neighborMaxTex = ShaderIDs.NeighborMaxTex;
			int width = context.width / num2;
			int height = context.height / num2;
			command.GetTemporaryRT(neighborMaxTex, width, height, 0, FilterMode.Point, format, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(tileVRT, neighborMaxTex, propertySheet, 4, false, null);
			command.ReleaseTemporaryRT(tileVRT);
			propertySheet.properties.SetFloat(ShaderIDs.LoopCount, (float)Mathf.Clamp(base.settings.sampleCount / 2, 1, 64));
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 5, false, null);
			command.ReleaseTemporaryRT(velocityTex);
			command.ReleaseTemporaryRT(neighborMaxTex);
			command.EndSample("MotionBlur");
		}

		// Token: 0x02000EC2 RID: 3778
		private enum Pass
		{
			// Token: 0x04004BE3 RID: 19427
			VelocitySetup,
			// Token: 0x04004BE4 RID: 19428
			TileMax1,
			// Token: 0x04004BE5 RID: 19429
			TileMax2,
			// Token: 0x04004BE6 RID: 19430
			TileMaxV,
			// Token: 0x04004BE7 RID: 19431
			NeighborMax,
			// Token: 0x04004BE8 RID: 19432
			Reconstruction
		}
	}
}
