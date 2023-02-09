using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A1E RID: 2590
	[Preserve]
	[Serializable]
	public sealed class Fog
	{
		// Token: 0x06003D81 RID: 15745 RVA: 0x00003A54 File Offset: 0x00001C54
		internal DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth;
		}

		// Token: 0x06003D82 RID: 15746 RVA: 0x00168EDC File Offset: 0x001670DC
		internal bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled && RenderSettings.fog && !RuntimeUtilities.scriptableRenderPipelineActive && context.resources.shaders.deferredFog && context.resources.shaders.deferredFog.isSupported && context.camera.actualRenderingPath == RenderingPath.DeferredShading;
		}

		// Token: 0x06003D83 RID: 15747 RVA: 0x00168F40 File Offset: 0x00167140
		internal void Render(PostProcessRenderContext context)
		{
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.deferredFog);
			propertySheet.ClearKeywords();
			Color c = RuntimeUtilities.isLinearColorSpace ? RenderSettings.fogColor.linear : RenderSettings.fogColor;
			propertySheet.properties.SetVector(ShaderIDs.FogColor, c);
			propertySheet.properties.SetVector(ShaderIDs.FogParams, new Vector3(RenderSettings.fogDensity, RenderSettings.fogStartDistance, RenderSettings.fogEndDistance));
			context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, this.excludeSkybox ? 1 : 0, false, null);
		}

		// Token: 0x040036C8 RID: 14024
		[Tooltip("Enables the internal deferred fog pass. Actual fog settings should be set in the Lighting panel.")]
		public bool enabled = true;

		// Token: 0x040036C9 RID: 14025
		[Tooltip("Mark true for the fog to ignore the skybox")]
		public bool excludeSkybox = true;
	}
}
