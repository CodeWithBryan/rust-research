using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A37 RID: 2615
	[Serializable]
	public sealed class VectorscopeMonitor : Monitor
	{
		// Token: 0x06003DEA RID: 15850 RVA: 0x0016C87F File Offset: 0x0016AA7F
		internal override void OnDisable()
		{
			base.OnDisable();
			if (this.m_Data != null)
			{
				this.m_Data.Release();
			}
			this.m_Data = null;
		}

		// Token: 0x06003DEB RID: 15851 RVA: 0x00003A54 File Offset: 0x00001C54
		internal override bool NeedsHalfRes()
		{
			return true;
		}

		// Token: 0x06003DEC RID: 15852 RVA: 0x0016C8A1 File Offset: 0x0016AAA1
		internal override bool ShaderResourcesAvailable(PostProcessRenderContext context)
		{
			return context.resources.computeShaders.vectorscope;
		}

		// Token: 0x06003DED RID: 15853 RVA: 0x0016C8B8 File Offset: 0x0016AAB8
		internal override void Render(PostProcessRenderContext context)
		{
			base.CheckOutput(this.size, this.size);
			this.exposure = Mathf.Max(0f, this.exposure);
			int num = this.size * this.size;
			if (this.m_Data == null)
			{
				this.m_Data = new ComputeBuffer(num, 4);
			}
			else if (this.m_Data.count != num)
			{
				this.m_Data.Release();
				this.m_Data = new ComputeBuffer(num, 4);
			}
			ComputeShader vectorscope = context.resources.computeShaders.vectorscope;
			CommandBuffer command = context.command;
			command.BeginSample("Vectorscope");
			Vector4 vector = new Vector4((float)(context.width / 2), (float)(context.height / 2), (float)this.size, (float)(RuntimeUtilities.isLinearColorSpace ? 1 : 0));
			int kernelIndex = vectorscope.FindKernel("KVectorscopeClear");
			command.SetComputeBufferParam(vectorscope, kernelIndex, "_VectorscopeBuffer", this.m_Data);
			command.SetComputeVectorParam(vectorscope, "_Params", vector);
			command.DispatchCompute(vectorscope, kernelIndex, Mathf.CeilToInt((float)this.size / 16f), Mathf.CeilToInt((float)this.size / 16f), 1);
			kernelIndex = vectorscope.FindKernel("KVectorscopeGather");
			command.SetComputeBufferParam(vectorscope, kernelIndex, "_VectorscopeBuffer", this.m_Data);
			command.SetComputeTextureParam(vectorscope, kernelIndex, "_Source", ShaderIDs.HalfResFinalCopy);
			command.DispatchCompute(vectorscope, kernelIndex, Mathf.CeilToInt(vector.x / 16f), Mathf.CeilToInt(vector.y / 16f), 1);
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.vectorscope);
			propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4((float)this.size, (float)this.size, this.exposure, 0f));
			propertySheet.properties.SetBuffer(ShaderIDs.VectorscopeBuffer, this.m_Data);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, base.output, propertySheet, 0, false, null);
			command.EndSample("Vectorscope");
		}

		// Token: 0x0400372D RID: 14125
		public int size = 256;

		// Token: 0x0400372E RID: 14126
		public float exposure = 0.12f;

		// Token: 0x0400372F RID: 14127
		private ComputeBuffer m_Data;

		// Token: 0x04003730 RID: 14128
		private const int k_ThreadGroupSizeX = 16;

		// Token: 0x04003731 RID: 14129
		private const int k_ThreadGroupSizeY = 16;
	}
}
