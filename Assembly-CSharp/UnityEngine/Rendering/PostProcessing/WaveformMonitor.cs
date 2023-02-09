using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A38 RID: 2616
	[Serializable]
	public sealed class WaveformMonitor : Monitor
	{
		// Token: 0x06003DEF RID: 15855 RVA: 0x0016CAF0 File Offset: 0x0016ACF0
		internal override void OnDisable()
		{
			base.OnDisable();
			if (this.m_Data != null)
			{
				this.m_Data.Release();
			}
			this.m_Data = null;
		}

		// Token: 0x06003DF0 RID: 15856 RVA: 0x00003A54 File Offset: 0x00001C54
		internal override bool NeedsHalfRes()
		{
			return true;
		}

		// Token: 0x06003DF1 RID: 15857 RVA: 0x0016CB12 File Offset: 0x0016AD12
		internal override bool ShaderResourcesAvailable(PostProcessRenderContext context)
		{
			return context.resources.computeShaders.waveform;
		}

		// Token: 0x06003DF2 RID: 15858 RVA: 0x0016CB2C File Offset: 0x0016AD2C
		internal override void Render(PostProcessRenderContext context)
		{
			float num = (float)context.width / 2f / ((float)context.height / 2f);
			int num2 = Mathf.FloorToInt((float)this.height * num);
			base.CheckOutput(num2, this.height);
			this.exposure = Mathf.Max(0f, this.exposure);
			int num3 = num2 * this.height;
			if (this.m_Data == null)
			{
				this.m_Data = new ComputeBuffer(num3, 16);
			}
			else if (this.m_Data.count < num3)
			{
				this.m_Data.Release();
				this.m_Data = new ComputeBuffer(num3, 16);
			}
			ComputeShader waveform = context.resources.computeShaders.waveform;
			CommandBuffer command = context.command;
			command.BeginSample("Waveform");
			Vector4 val = new Vector4((float)num2, (float)this.height, (float)(RuntimeUtilities.isLinearColorSpace ? 1 : 0), 0f);
			int kernelIndex = waveform.FindKernel("KWaveformClear");
			command.SetComputeBufferParam(waveform, kernelIndex, "_WaveformBuffer", this.m_Data);
			command.SetComputeVectorParam(waveform, "_Params", val);
			command.DispatchCompute(waveform, kernelIndex, Mathf.CeilToInt((float)num2 / 16f), Mathf.CeilToInt((float)this.height / 16f), 1);
			command.GetTemporaryRT(ShaderIDs.WaveformSource, num2, this.height, 0, FilterMode.Bilinear, context.sourceFormat);
			command.BlitFullscreenTriangle(ShaderIDs.HalfResFinalCopy, ShaderIDs.WaveformSource, false, null);
			kernelIndex = waveform.FindKernel("KWaveformGather");
			command.SetComputeBufferParam(waveform, kernelIndex, "_WaveformBuffer", this.m_Data);
			command.SetComputeTextureParam(waveform, kernelIndex, "_Source", ShaderIDs.WaveformSource);
			command.SetComputeVectorParam(waveform, "_Params", val);
			command.DispatchCompute(waveform, kernelIndex, num2, Mathf.CeilToInt((float)this.height / 256f), 1);
			command.ReleaseTemporaryRT(ShaderIDs.WaveformSource);
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.waveform);
			propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4((float)num2, (float)this.height, this.exposure, 0f));
			propertySheet.properties.SetBuffer(ShaderIDs.WaveformBuffer, this.m_Data);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, base.output, propertySheet, 0, false, null);
			command.EndSample("Waveform");
		}

		// Token: 0x04003732 RID: 14130
		public float exposure = 0.12f;

		// Token: 0x04003733 RID: 14131
		public int height = 256;

		// Token: 0x04003734 RID: 14132
		private ComputeBuffer m_Data;

		// Token: 0x04003735 RID: 14133
		private const int k_ThreadGroupSize = 256;

		// Token: 0x04003736 RID: 14134
		private const int k_ThreadGroupSizeX = 16;

		// Token: 0x04003737 RID: 14135
		private const int k_ThreadGroupSizeY = 16;
	}
}
