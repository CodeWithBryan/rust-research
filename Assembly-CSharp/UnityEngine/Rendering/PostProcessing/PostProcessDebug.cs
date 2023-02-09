using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A46 RID: 2630
	[ExecuteAlways]
	[AddComponentMenu("Rendering/Post-process Debug", 1002)]
	public sealed class PostProcessDebug : MonoBehaviour
	{
		// Token: 0x06003E28 RID: 15912 RVA: 0x0016D449 File Offset: 0x0016B649
		private void OnEnable()
		{
			this.m_CmdAfterEverything = new CommandBuffer
			{
				name = "Post-processing Debug Overlay"
			};
		}

		// Token: 0x06003E29 RID: 15913 RVA: 0x0016D461 File Offset: 0x0016B661
		private void OnDisable()
		{
			if (this.m_CurrentCamera != null)
			{
				this.m_CurrentCamera.RemoveCommandBuffer(CameraEvent.AfterImageEffects, this.m_CmdAfterEverything);
			}
			this.m_CurrentCamera = null;
			this.m_PreviousPostProcessLayer = null;
		}

		// Token: 0x06003E2A RID: 15914 RVA: 0x0016D492 File Offset: 0x0016B692
		private void Update()
		{
			this.UpdateStates();
		}

		// Token: 0x06003E2B RID: 15915 RVA: 0x0016D49A File Offset: 0x0016B69A
		private void Reset()
		{
			this.postProcessLayer = base.GetComponent<PostProcessLayer>();
		}

		// Token: 0x06003E2C RID: 15916 RVA: 0x0016D4A8 File Offset: 0x0016B6A8
		private void UpdateStates()
		{
			if (this.m_PreviousPostProcessLayer != this.postProcessLayer)
			{
				if (this.m_CurrentCamera != null)
				{
					this.m_CurrentCamera.RemoveCommandBuffer(CameraEvent.AfterImageEffects, this.m_CmdAfterEverything);
					this.m_CurrentCamera = null;
				}
				this.m_PreviousPostProcessLayer = this.postProcessLayer;
				if (this.postProcessLayer != null)
				{
					this.m_CurrentCamera = this.postProcessLayer.GetComponent<Camera>();
					this.m_CurrentCamera.AddCommandBuffer(CameraEvent.AfterImageEffects, this.m_CmdAfterEverything);
				}
			}
			if (this.postProcessLayer == null || !this.postProcessLayer.enabled)
			{
				return;
			}
			if (this.lightMeter)
			{
				this.postProcessLayer.debugLayer.RequestMonitorPass(MonitorType.LightMeter);
			}
			if (this.histogram)
			{
				this.postProcessLayer.debugLayer.RequestMonitorPass(MonitorType.Histogram);
			}
			if (this.waveform)
			{
				this.postProcessLayer.debugLayer.RequestMonitorPass(MonitorType.Waveform);
			}
			if (this.vectorscope)
			{
				this.postProcessLayer.debugLayer.RequestMonitorPass(MonitorType.Vectorscope);
			}
			this.postProcessLayer.debugLayer.RequestDebugOverlay(this.debugOverlay);
		}

		// Token: 0x06003E2D RID: 15917 RVA: 0x0016D5C4 File Offset: 0x0016B7C4
		private void OnPostRender()
		{
			this.m_CmdAfterEverything.Clear();
			if (this.postProcessLayer == null || !this.postProcessLayer.enabled || !this.postProcessLayer.debugLayer.debugOverlayActive)
			{
				return;
			}
			this.m_CmdAfterEverything.Blit(this.postProcessLayer.debugLayer.debugOverlayTarget, BuiltinRenderTextureType.CameraTarget);
		}

		// Token: 0x06003E2E RID: 15918 RVA: 0x0016D62C File Offset: 0x0016B82C
		private void OnGUI()
		{
			if (this.postProcessLayer == null || !this.postProcessLayer.enabled)
			{
				return;
			}
			RenderTexture.active = null;
			Rect rect = new Rect(5f, 5f, 0f, 0f);
			PostProcessDebugLayer debugLayer = this.postProcessLayer.debugLayer;
			this.DrawMonitor(ref rect, debugLayer.lightMeter, this.lightMeter);
			this.DrawMonitor(ref rect, debugLayer.histogram, this.histogram);
			this.DrawMonitor(ref rect, debugLayer.waveform, this.waveform);
			this.DrawMonitor(ref rect, debugLayer.vectorscope, this.vectorscope);
		}

		// Token: 0x06003E2F RID: 15919 RVA: 0x0016D6D4 File Offset: 0x0016B8D4
		private void DrawMonitor(ref Rect rect, Monitor monitor, bool enabled)
		{
			if (!enabled || monitor.output == null)
			{
				return;
			}
			rect.width = (float)monitor.output.width;
			rect.height = (float)monitor.output.height;
			GUI.DrawTexture(rect, monitor.output);
			rect.x += (float)monitor.output.width + 5f;
		}

		// Token: 0x04003744 RID: 14148
		public PostProcessLayer postProcessLayer;

		// Token: 0x04003745 RID: 14149
		private PostProcessLayer m_PreviousPostProcessLayer;

		// Token: 0x04003746 RID: 14150
		public bool lightMeter;

		// Token: 0x04003747 RID: 14151
		public bool histogram;

		// Token: 0x04003748 RID: 14152
		public bool waveform;

		// Token: 0x04003749 RID: 14153
		public bool vectorscope;

		// Token: 0x0400374A RID: 14154
		public DebugOverlay debugOverlay;

		// Token: 0x0400374B RID: 14155
		private Camera m_CurrentCamera;

		// Token: 0x0400374C RID: 14156
		private CommandBuffer m_CmdAfterEverything;
	}
}
