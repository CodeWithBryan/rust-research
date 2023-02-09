using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A26 RID: 2598
	[Preserve]
	[Serializable]
	internal sealed class ScalableAO : IAmbientOcclusionMethod
	{
		// Token: 0x06003DAB RID: 15787 RVA: 0x0016AB54 File Offset: 0x00168D54
		public ScalableAO(AmbientOcclusion settings)
		{
			this.m_Settings = settings;
		}

		// Token: 0x06003DAC RID: 15788 RVA: 0x00002E0E File Offset: 0x0000100E
		public DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth | DepthTextureMode.DepthNormals;
		}

		// Token: 0x06003DAD RID: 15789 RVA: 0x0016ABAC File Offset: 0x00168DAC
		private void DoLazyInitialization(PostProcessRenderContext context)
		{
			this.m_PropertySheet = context.propertySheets.Get(context.resources.shaders.scalableAO);
			bool flag = false;
			if (this.m_Result == null || !this.m_Result.IsCreated())
			{
				this.m_Result = context.GetScreenSpaceTemporaryRT(0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear, 0, 0);
				this.m_Result.hideFlags = HideFlags.DontSave;
				this.m_Result.filterMode = FilterMode.Bilinear;
				flag = true;
			}
			else if (this.m_Result.width != context.width || this.m_Result.height != context.height)
			{
				this.m_Result.Release();
				this.m_Result.width = context.width;
				this.m_Result.height = context.height;
				flag = true;
			}
			if (flag)
			{
				this.m_Result.Create();
			}
		}

		// Token: 0x06003DAE RID: 15790 RVA: 0x0016AC8C File Offset: 0x00168E8C
		private void Render(PostProcessRenderContext context, CommandBuffer cmd, int occlusionSource)
		{
			this.DoLazyInitialization(context);
			this.m_Settings.radius.value = Mathf.Max(this.m_Settings.radius.value, 0.0001f);
			bool flag = this.m_Settings.quality.value < AmbientOcclusionQuality.High;
			float value = this.m_Settings.intensity.value;
			float value2 = this.m_Settings.radius.value;
			float z = flag ? 0.5f : 1f;
			float w = (float)this.m_SampleCount[(int)this.m_Settings.quality.value];
			PropertySheet propertySheet = this.m_PropertySheet;
			propertySheet.ClearKeywords();
			propertySheet.properties.SetVector(ShaderIDs.AOParams, new Vector4(value, value2, z, w));
			propertySheet.properties.SetVector(ShaderIDs.AOColor, Color.white - this.m_Settings.color.value);
			if (context.camera.actualRenderingPath == RenderingPath.Forward && RenderSettings.fog)
			{
				propertySheet.EnableKeyword("APPLY_FORWARD_FOG");
				propertySheet.properties.SetVector(ShaderIDs.FogParams, new Vector3(RenderSettings.fogDensity, RenderSettings.fogStartDistance, RenderSettings.fogEndDistance));
			}
			int num = flag ? 2 : 1;
			int occlusionTexture = ShaderIDs.OcclusionTexture1;
			int widthOverride = context.width / num;
			int heightOverride = context.height / num;
			context.GetScreenSpaceTemporaryRT(cmd, occlusionTexture, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear, FilterMode.Bilinear, widthOverride, heightOverride);
			cmd.BlitFullscreenTriangle(BuiltinRenderTextureType.None, occlusionTexture, propertySheet, occlusionSource, false, null);
			int occlusionTexture2 = ShaderIDs.OcclusionTexture2;
			context.GetScreenSpaceTemporaryRT(cmd, occlusionTexture2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear, FilterMode.Bilinear, 0, 0);
			cmd.BlitFullscreenTriangle(occlusionTexture, occlusionTexture2, propertySheet, 2 + occlusionSource, false, null);
			cmd.ReleaseTemporaryRT(occlusionTexture);
			cmd.BlitFullscreenTriangle(occlusionTexture2, this.m_Result, propertySheet, 4, false, null);
			cmd.ReleaseTemporaryRT(occlusionTexture2);
			if (context.IsDebugOverlayEnabled(DebugOverlay.AmbientOcclusion))
			{
				context.PushDebugOverlay(cmd, this.m_Result, propertySheet, 7);
			}
		}

		// Token: 0x06003DAF RID: 15791 RVA: 0x0016AEB4 File Offset: 0x001690B4
		public void RenderAfterOpaque(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			command.BeginSample("Ambient Occlusion");
			this.Render(context, command, 0);
			command.SetGlobalTexture(ShaderIDs.SAOcclusionTexture, this.m_Result);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, BuiltinRenderTextureType.CameraTarget, this.m_PropertySheet, 5, RenderBufferLoadAction.Load, null);
			command.EndSample("Ambient Occlusion");
		}

		// Token: 0x06003DB0 RID: 15792 RVA: 0x0016AF20 File Offset: 0x00169120
		public void RenderAmbientOnly(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			command.BeginSample("Ambient Occlusion Render");
			this.Render(context, command, 1);
			command.EndSample("Ambient Occlusion Render");
		}

		// Token: 0x06003DB1 RID: 15793 RVA: 0x0016AF54 File Offset: 0x00169154
		public void CompositeAmbientOnly(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			command.BeginSample("Ambient Occlusion Composite");
			command.SetGlobalTexture(ShaderIDs.SAOcclusionTexture, this.m_Result);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, this.m_MRT, BuiltinRenderTextureType.CameraTarget, this.m_PropertySheet, 6, false, null);
			command.EndSample("Ambient Occlusion Composite");
		}

		// Token: 0x06003DB2 RID: 15794 RVA: 0x0016AFBB File Offset: 0x001691BB
		public void Release()
		{
			RuntimeUtilities.Destroy(this.m_Result);
			this.m_Result = null;
		}

		// Token: 0x040036E3 RID: 14051
		private RenderTexture m_Result;

		// Token: 0x040036E4 RID: 14052
		private PropertySheet m_PropertySheet;

		// Token: 0x040036E5 RID: 14053
		private AmbientOcclusion m_Settings;

		// Token: 0x040036E6 RID: 14054
		private readonly RenderTargetIdentifier[] m_MRT = new RenderTargetIdentifier[]
		{
			BuiltinRenderTextureType.GBuffer0,
			BuiltinRenderTextureType.CameraTarget
		};

		// Token: 0x040036E7 RID: 14055
		private readonly int[] m_SampleCount = new int[]
		{
			4,
			6,
			10,
			8,
			12
		};

		// Token: 0x02000EC5 RID: 3781
		private enum Pass
		{
			// Token: 0x04004BF7 RID: 19447
			OcclusionEstimationForward,
			// Token: 0x04004BF8 RID: 19448
			OcclusionEstimationDeferred,
			// Token: 0x04004BF9 RID: 19449
			HorizontalBlurForward,
			// Token: 0x04004BFA RID: 19450
			HorizontalBlurDeferred,
			// Token: 0x04004BFB RID: 19451
			VerticalBlur,
			// Token: 0x04004BFC RID: 19452
			CompositionForward,
			// Token: 0x04004BFD RID: 19453
			CompositionDeferred,
			// Token: 0x04004BFE RID: 19454
			DebugOverlay
		}
	}
}
