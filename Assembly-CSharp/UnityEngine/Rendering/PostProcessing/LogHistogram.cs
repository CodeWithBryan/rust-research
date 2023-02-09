using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A56 RID: 2646
	internal sealed class LogHistogram
	{
		// Token: 0x1700051C RID: 1308
		// (get) Token: 0x06003E99 RID: 16025 RVA: 0x0016F7B6 File Offset: 0x0016D9B6
		// (set) Token: 0x06003E9A RID: 16026 RVA: 0x0016F7BE File Offset: 0x0016D9BE
		public ComputeBuffer data { get; private set; }

		// Token: 0x06003E9B RID: 16027 RVA: 0x0016F7C8 File Offset: 0x0016D9C8
		public void Generate(PostProcessRenderContext context)
		{
			if (this.data == null)
			{
				this.data = new ComputeBuffer(128, 4);
			}
			Vector4 histogramScaleOffsetRes = this.GetHistogramScaleOffsetRes(context);
			ComputeShader exposureHistogram = context.resources.computeShaders.exposureHistogram;
			CommandBuffer command = context.command;
			command.BeginSample("LogHistogram");
			int kernelIndex = exposureHistogram.FindKernel("KEyeHistogramClear");
			command.SetComputeBufferParam(exposureHistogram, kernelIndex, "_HistogramBuffer", this.data);
			uint num;
			uint num2;
			uint num3;
			exposureHistogram.GetKernelThreadGroupSizes(kernelIndex, out num, out num2, out num3);
			command.DispatchCompute(exposureHistogram, kernelIndex, Mathf.CeilToInt(128f / num), 1, 1);
			kernelIndex = exposureHistogram.FindKernel("KEyeHistogram");
			command.SetComputeBufferParam(exposureHistogram, kernelIndex, "_HistogramBuffer", this.data);
			command.SetComputeTextureParam(exposureHistogram, kernelIndex, "_Source", context.source);
			command.SetComputeVectorParam(exposureHistogram, "_ScaleOffsetRes", histogramScaleOffsetRes);
			exposureHistogram.GetKernelThreadGroupSizes(kernelIndex, out num, out num2, out num3);
			command.DispatchCompute(exposureHistogram, kernelIndex, Mathf.CeilToInt(histogramScaleOffsetRes.z / 2f / num), Mathf.CeilToInt(histogramScaleOffsetRes.w / 2f / num2), 1);
			command.EndSample("LogHistogram");
		}

		// Token: 0x06003E9C RID: 16028 RVA: 0x0016F8FC File Offset: 0x0016DAFC
		public Vector4 GetHistogramScaleOffsetRes(PostProcessRenderContext context)
		{
			float num = 18f;
			float num2 = 1f / num;
			float y = 9f * num2;
			return new Vector4(num2, y, (float)context.width, (float)context.height);
		}

		// Token: 0x06003E9D RID: 16029 RVA: 0x0016F934 File Offset: 0x0016DB34
		public void Release()
		{
			if (this.data != null)
			{
				this.data.Release();
			}
			this.data = null;
		}

		// Token: 0x04003795 RID: 14229
		public const int rangeMin = -9;

		// Token: 0x04003796 RID: 14230
		public const int rangeMax = 9;

		// Token: 0x04003797 RID: 14231
		private const int k_Bins = 128;
	}
}
