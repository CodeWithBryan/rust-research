using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A33 RID: 2611
	[Serializable]
	public sealed class HistogramMonitor : Monitor
	{
		// Token: 0x06003DD8 RID: 15832 RVA: 0x0016C393 File Offset: 0x0016A593
		internal override void OnDisable()
		{
			base.OnDisable();
			if (this.m_Data != null)
			{
				this.m_Data.Release();
			}
			this.m_Data = null;
		}

		// Token: 0x06003DD9 RID: 15833 RVA: 0x00003A54 File Offset: 0x00001C54
		internal override bool NeedsHalfRes()
		{
			return true;
		}

		// Token: 0x06003DDA RID: 15834 RVA: 0x0016C3B5 File Offset: 0x0016A5B5
		internal override bool ShaderResourcesAvailable(PostProcessRenderContext context)
		{
			return context.resources.computeShaders.gammaHistogram;
		}

		// Token: 0x06003DDB RID: 15835 RVA: 0x0016C3CC File Offset: 0x0016A5CC
		internal override void Render(PostProcessRenderContext context)
		{
			base.CheckOutput(this.width, this.height);
			if (this.m_Data == null)
			{
				this.m_Data = new ComputeBuffer(256, 4);
			}
			ComputeShader gammaHistogram = context.resources.computeShaders.gammaHistogram;
			CommandBuffer command = context.command;
			command.BeginSample("GammaHistogram");
			int kernelIndex = gammaHistogram.FindKernel("KHistogramClear");
			command.SetComputeBufferParam(gammaHistogram, kernelIndex, "_HistogramBuffer", this.m_Data);
			command.DispatchCompute(gammaHistogram, kernelIndex, Mathf.CeilToInt(16f), 1, 1);
			kernelIndex = gammaHistogram.FindKernel("KHistogramGather");
			Vector4 vector = new Vector4((float)(context.width / 2), (float)(context.height / 2), (float)(RuntimeUtilities.isLinearColorSpace ? 1 : 0), (float)this.channel);
			command.SetComputeVectorParam(gammaHistogram, "_Params", vector);
			command.SetComputeTextureParam(gammaHistogram, kernelIndex, "_Source", ShaderIDs.HalfResFinalCopy);
			command.SetComputeBufferParam(gammaHistogram, kernelIndex, "_HistogramBuffer", this.m_Data);
			command.DispatchCompute(gammaHistogram, kernelIndex, Mathf.CeilToInt(vector.x / 16f), Mathf.CeilToInt(vector.y / 16f), 1);
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.gammaHistogram);
			propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4((float)this.width, (float)this.height, 0f, 0f));
			propertySheet.properties.SetBuffer(ShaderIDs.HistogramBuffer, this.m_Data);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, base.output, propertySheet, 0, false, null);
			command.EndSample("GammaHistogram");
		}

		// Token: 0x0400371C RID: 14108
		public int width = 512;

		// Token: 0x0400371D RID: 14109
		public int height = 256;

		// Token: 0x0400371E RID: 14110
		public HistogramMonitor.Channel channel = HistogramMonitor.Channel.Master;

		// Token: 0x0400371F RID: 14111
		private ComputeBuffer m_Data;

		// Token: 0x04003720 RID: 14112
		private const int k_NumBins = 256;

		// Token: 0x04003721 RID: 14113
		private const int k_ThreadGroupSizeX = 16;

		// Token: 0x04003722 RID: 14114
		private const int k_ThreadGroupSizeY = 16;

		// Token: 0x02000ECB RID: 3787
		public enum Channel
		{
			// Token: 0x04004C13 RID: 19475
			Red,
			// Token: 0x04004C14 RID: 19476
			Green,
			// Token: 0x04004C15 RID: 19477
			Blue,
			// Token: 0x04004C16 RID: 19478
			Master
		}
	}
}
