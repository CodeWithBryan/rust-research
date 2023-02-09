using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A49 RID: 2633
	[Serializable]
	public sealed class PostProcessDebugLayer
	{
		// Token: 0x17000511 RID: 1297
		// (get) Token: 0x06003E31 RID: 15921 RVA: 0x0016D747 File Offset: 0x0016B947
		// (set) Token: 0x06003E32 RID: 15922 RVA: 0x0016D74F File Offset: 0x0016B94F
		public RenderTexture debugOverlayTarget { get; private set; }

		// Token: 0x17000512 RID: 1298
		// (get) Token: 0x06003E33 RID: 15923 RVA: 0x0016D758 File Offset: 0x0016B958
		// (set) Token: 0x06003E34 RID: 15924 RVA: 0x0016D760 File Offset: 0x0016B960
		public bool debugOverlayActive { get; private set; }

		// Token: 0x17000513 RID: 1299
		// (get) Token: 0x06003E35 RID: 15925 RVA: 0x0016D769 File Offset: 0x0016B969
		// (set) Token: 0x06003E36 RID: 15926 RVA: 0x0016D771 File Offset: 0x0016B971
		public DebugOverlay debugOverlay { get; private set; }

		// Token: 0x06003E37 RID: 15927 RVA: 0x0016D77C File Offset: 0x0016B97C
		internal void OnEnable()
		{
			RuntimeUtilities.CreateIfNull<LightMeterMonitor>(ref this.lightMeter);
			RuntimeUtilities.CreateIfNull<HistogramMonitor>(ref this.histogram);
			RuntimeUtilities.CreateIfNull<WaveformMonitor>(ref this.waveform);
			RuntimeUtilities.CreateIfNull<VectorscopeMonitor>(ref this.vectorscope);
			RuntimeUtilities.CreateIfNull<PostProcessDebugLayer.OverlaySettings>(ref this.overlaySettings);
			this.m_Monitors = new Dictionary<MonitorType, Monitor>
			{
				{
					MonitorType.LightMeter,
					this.lightMeter
				},
				{
					MonitorType.Histogram,
					this.histogram
				},
				{
					MonitorType.Waveform,
					this.waveform
				},
				{
					MonitorType.Vectorscope,
					this.vectorscope
				}
			};
			foreach (KeyValuePair<MonitorType, Monitor> keyValuePair in this.m_Monitors)
			{
				keyValuePair.Value.OnEnable();
			}
		}

		// Token: 0x06003E38 RID: 15928 RVA: 0x0016D84C File Offset: 0x0016BA4C
		internal void OnDisable()
		{
			foreach (KeyValuePair<MonitorType, Monitor> keyValuePair in this.m_Monitors)
			{
				keyValuePair.Value.OnDisable();
			}
			this.DestroyDebugOverlayTarget();
		}

		// Token: 0x06003E39 RID: 15929 RVA: 0x0016D8AC File Offset: 0x0016BAAC
		private void DestroyDebugOverlayTarget()
		{
			RuntimeUtilities.Destroy(this.debugOverlayTarget);
			this.debugOverlayTarget = null;
		}

		// Token: 0x06003E3A RID: 15930 RVA: 0x0016D8C0 File Offset: 0x0016BAC0
		public void RequestMonitorPass(MonitorType monitor)
		{
			this.m_Monitors[monitor].requested = true;
		}

		// Token: 0x06003E3B RID: 15931 RVA: 0x0016D8D4 File Offset: 0x0016BAD4
		public void RequestDebugOverlay(DebugOverlay mode)
		{
			this.debugOverlay = mode;
		}

		// Token: 0x06003E3C RID: 15932 RVA: 0x0016D8DD File Offset: 0x0016BADD
		internal void SetFrameSize(int width, int height)
		{
			this.frameWidth = width;
			this.frameHeight = height;
			this.debugOverlayActive = false;
		}

		// Token: 0x06003E3D RID: 15933 RVA: 0x0016D8F4 File Offset: 0x0016BAF4
		public void PushDebugOverlay(CommandBuffer cmd, RenderTargetIdentifier source, PropertySheet sheet, int pass)
		{
			if (this.debugOverlayTarget == null || !this.debugOverlayTarget.IsCreated() || this.debugOverlayTarget.width != this.frameWidth || this.debugOverlayTarget.height != this.frameHeight)
			{
				RuntimeUtilities.Destroy(this.debugOverlayTarget);
				this.debugOverlayTarget = new RenderTexture(this.frameWidth, this.frameHeight, 0, RenderTextureFormat.ARGB32)
				{
					name = "Debug Overlay Target",
					anisoLevel = 1,
					filterMode = FilterMode.Bilinear,
					wrapMode = TextureWrapMode.Clamp,
					hideFlags = HideFlags.HideAndDontSave
				};
				this.debugOverlayTarget.Create();
			}
			cmd.BlitFullscreenTriangle(source, this.debugOverlayTarget, sheet, pass, false, null);
			this.debugOverlayActive = true;
		}

		// Token: 0x06003E3E RID: 15934 RVA: 0x0016D9C0 File Offset: 0x0016BBC0
		internal DepthTextureMode GetCameraFlags()
		{
			if (this.debugOverlay == DebugOverlay.Depth)
			{
				return DepthTextureMode.Depth;
			}
			if (this.debugOverlay == DebugOverlay.Normals)
			{
				return DepthTextureMode.DepthNormals;
			}
			if (this.debugOverlay == DebugOverlay.MotionVectors)
			{
				return DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
			}
			return DepthTextureMode.None;
		}

		// Token: 0x06003E3F RID: 15935 RVA: 0x0016D9E4 File Offset: 0x0016BBE4
		internal void RenderMonitors(PostProcessRenderContext context)
		{
			bool flag = false;
			bool flag2 = false;
			foreach (KeyValuePair<MonitorType, Monitor> keyValuePair in this.m_Monitors)
			{
				bool flag3 = keyValuePair.Value.IsRequestedAndSupported(context);
				flag = (flag || flag3);
				flag2 |= (flag3 && keyValuePair.Value.NeedsHalfRes());
			}
			if (!flag)
			{
				return;
			}
			CommandBuffer command = context.command;
			command.BeginSample("Monitors");
			if (flag2)
			{
				command.GetTemporaryRT(ShaderIDs.HalfResFinalCopy, context.width / 2, context.height / 2, 0, FilterMode.Bilinear, context.sourceFormat);
				command.Blit(context.destination, ShaderIDs.HalfResFinalCopy);
			}
			foreach (KeyValuePair<MonitorType, Monitor> keyValuePair2 in this.m_Monitors)
			{
				Monitor value = keyValuePair2.Value;
				if (value.requested)
				{
					value.Render(context);
				}
			}
			if (flag2)
			{
				command.ReleaseTemporaryRT(ShaderIDs.HalfResFinalCopy);
			}
			command.EndSample("Monitors");
		}

		// Token: 0x06003E40 RID: 15936 RVA: 0x0016DB20 File Offset: 0x0016BD20
		internal void RenderSpecialOverlays(PostProcessRenderContext context)
		{
			if (this.debugOverlay == DebugOverlay.Depth)
			{
				PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.debugOverlays);
				propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4(this.overlaySettings.linearDepth ? 1f : 0f, 0f, 0f, 0f));
				this.PushDebugOverlay(context.command, BuiltinRenderTextureType.None, propertySheet, 0);
				return;
			}
			if (this.debugOverlay == DebugOverlay.Normals)
			{
				PropertySheet propertySheet2 = context.propertySheets.Get(context.resources.shaders.debugOverlays);
				propertySheet2.ClearKeywords();
				if (context.camera.actualRenderingPath == RenderingPath.DeferredLighting)
				{
					propertySheet2.EnableKeyword("SOURCE_GBUFFER");
				}
				this.PushDebugOverlay(context.command, BuiltinRenderTextureType.None, propertySheet2, 1);
				return;
			}
			if (this.debugOverlay == DebugOverlay.MotionVectors)
			{
				PropertySheet propertySheet3 = context.propertySheets.Get(context.resources.shaders.debugOverlays);
				propertySheet3.properties.SetVector(ShaderIDs.Params, new Vector4(this.overlaySettings.motionColorIntensity, (float)this.overlaySettings.motionGridSize, 0f, 0f));
				this.PushDebugOverlay(context.command, context.source, propertySheet3, 2);
				return;
			}
			if (this.debugOverlay == DebugOverlay.NANTracker)
			{
				PropertySheet sheet = context.propertySheets.Get(context.resources.shaders.debugOverlays);
				this.PushDebugOverlay(context.command, context.source, sheet, 3);
				return;
			}
			if (this.debugOverlay == DebugOverlay.ColorBlindnessSimulation)
			{
				PropertySheet propertySheet4 = context.propertySheets.Get(context.resources.shaders.debugOverlays);
				propertySheet4.properties.SetVector(ShaderIDs.Params, new Vector4(this.overlaySettings.colorBlindnessStrength, 0f, 0f, 0f));
				this.PushDebugOverlay(context.command, context.source, propertySheet4, (int)(4 + this.overlaySettings.colorBlindnessType));
			}
		}

		// Token: 0x06003E41 RID: 15937 RVA: 0x0016DD20 File Offset: 0x0016BF20
		internal void EndFrame()
		{
			foreach (KeyValuePair<MonitorType, Monitor> keyValuePair in this.m_Monitors)
			{
				keyValuePair.Value.requested = false;
			}
			if (!this.debugOverlayActive)
			{
				this.DestroyDebugOverlayTarget();
			}
			this.debugOverlay = DebugOverlay.None;
		}

		// Token: 0x0400375D RID: 14173
		public LightMeterMonitor lightMeter;

		// Token: 0x0400375E RID: 14174
		public HistogramMonitor histogram;

		// Token: 0x0400375F RID: 14175
		public WaveformMonitor waveform;

		// Token: 0x04003760 RID: 14176
		public VectorscopeMonitor vectorscope;

		// Token: 0x04003761 RID: 14177
		private Dictionary<MonitorType, Monitor> m_Monitors;

		// Token: 0x04003762 RID: 14178
		private int frameWidth;

		// Token: 0x04003763 RID: 14179
		private int frameHeight;

		// Token: 0x04003767 RID: 14183
		public PostProcessDebugLayer.OverlaySettings overlaySettings;

		// Token: 0x02000ECC RID: 3788
		[Serializable]
		public class OverlaySettings
		{
			// Token: 0x04004C17 RID: 19479
			public bool linearDepth;

			// Token: 0x04004C18 RID: 19480
			[Range(0f, 16f)]
			public float motionColorIntensity = 4f;

			// Token: 0x04004C19 RID: 19481
			[Range(4f, 128f)]
			public int motionGridSize = 64;

			// Token: 0x04004C1A RID: 19482
			public ColorBlindnessType colorBlindnessType;

			// Token: 0x04004C1B RID: 19483
			[Range(0f, 1f)]
			public float colorBlindnessStrength = 1f;
		}
	}
}
