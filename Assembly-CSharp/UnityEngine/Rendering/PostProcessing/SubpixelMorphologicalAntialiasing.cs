using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A2D RID: 2605
	[Preserve]
	[Serializable]
	public sealed class SubpixelMorphologicalAntialiasing
	{
		// Token: 0x06003DBC RID: 15804 RVA: 0x0016B910 File Offset: 0x00169B10
		public bool IsSupported()
		{
			return !RuntimeUtilities.isSinglePassStereoEnabled;
		}

		// Token: 0x06003DBD RID: 15805 RVA: 0x0016B91C File Offset: 0x00169B1C
		internal void Render(PostProcessRenderContext context)
		{
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.subpixelMorphologicalAntialiasing);
			propertySheet.properties.SetTexture("_AreaTex", context.resources.smaaLuts.area);
			propertySheet.properties.SetTexture("_SearchTex", context.resources.smaaLuts.search);
			CommandBuffer command = context.command;
			command.BeginSample("SubpixelMorphologicalAntialiasing");
			command.GetTemporaryRT(ShaderIDs.SMAA_Flip, context.width, context.height, 0, FilterMode.Bilinear, context.sourceFormat, RenderTextureReadWrite.Linear);
			command.GetTemporaryRT(ShaderIDs.SMAA_Flop, context.width, context.height, 0, FilterMode.Bilinear, context.sourceFormat, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(context.source, ShaderIDs.SMAA_Flip, propertySheet, (int)this.quality, true, null);
			command.BlitFullscreenTriangle(ShaderIDs.SMAA_Flip, ShaderIDs.SMAA_Flop, propertySheet, (int)(3 + this.quality), false, null);
			command.SetGlobalTexture("_BlendTex", ShaderIDs.SMAA_Flop);
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 6, false, null);
			command.ReleaseTemporaryRT(ShaderIDs.SMAA_Flip);
			command.ReleaseTemporaryRT(ShaderIDs.SMAA_Flop);
			command.EndSample("SubpixelMorphologicalAntialiasing");
		}

		// Token: 0x04003700 RID: 14080
		[Tooltip("Lower quality is faster at the expense of visual quality (Low = ~60%, Medium = ~80%).")]
		public SubpixelMorphologicalAntialiasing.Quality quality = SubpixelMorphologicalAntialiasing.Quality.High;

		// Token: 0x02000EC8 RID: 3784
		private enum Pass
		{
			// Token: 0x04004C08 RID: 19464
			EdgeDetection,
			// Token: 0x04004C09 RID: 19465
			BlendWeights = 3,
			// Token: 0x04004C0A RID: 19466
			NeighborhoodBlending = 6
		}

		// Token: 0x02000EC9 RID: 3785
		public enum Quality
		{
			// Token: 0x04004C0C RID: 19468
			Low,
			// Token: 0x04004C0D RID: 19469
			Medium,
			// Token: 0x04004C0E RID: 19470
			High
		}
	}
}
