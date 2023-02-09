using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A5F RID: 2655
	internal class TextureLerper
	{
		// Token: 0x17000534 RID: 1332
		// (get) Token: 0x06003EF5 RID: 16117 RVA: 0x001717FC File Offset: 0x0016F9FC
		internal static TextureLerper instance
		{
			get
			{
				if (TextureLerper.m_Instance == null)
				{
					TextureLerper.m_Instance = new TextureLerper();
				}
				return TextureLerper.m_Instance;
			}
		}

		// Token: 0x06003EF6 RID: 16118 RVA: 0x00171814 File Offset: 0x0016FA14
		private TextureLerper()
		{
			this.m_Recycled = new List<RenderTexture>();
			this.m_Actives = new List<RenderTexture>();
		}

		// Token: 0x06003EF7 RID: 16119 RVA: 0x00171832 File Offset: 0x0016FA32
		internal void BeginFrame(PostProcessRenderContext context)
		{
			this.m_Command = context.command;
			this.m_PropertySheets = context.propertySheets;
			this.m_Resources = context.resources;
		}

		// Token: 0x06003EF8 RID: 16120 RVA: 0x00171858 File Offset: 0x0016FA58
		internal void EndFrame()
		{
			if (this.m_Recycled.Count > 0)
			{
				foreach (RenderTexture obj in this.m_Recycled)
				{
					RuntimeUtilities.Destroy(obj);
				}
				this.m_Recycled.Clear();
			}
			if (this.m_Actives.Count > 0)
			{
				foreach (RenderTexture item in this.m_Actives)
				{
					this.m_Recycled.Add(item);
				}
				this.m_Actives.Clear();
			}
		}

		// Token: 0x06003EF9 RID: 16121 RVA: 0x00171924 File Offset: 0x0016FB24
		private RenderTexture Get(RenderTextureFormat format, int w, int h, int d = 1, bool enableRandomWrite = false, bool force3D = false)
		{
			RenderTexture renderTexture = null;
			int count = this.m_Recycled.Count;
			int i;
			for (i = 0; i < count; i++)
			{
				RenderTexture renderTexture2 = this.m_Recycled[i];
				if (renderTexture2.width == w && renderTexture2.height == h && renderTexture2.volumeDepth == d && renderTexture2.format == format && renderTexture2.enableRandomWrite == enableRandomWrite && (!force3D || renderTexture2.dimension == TextureDimension.Tex3D))
				{
					renderTexture = renderTexture2;
					break;
				}
			}
			if (renderTexture == null)
			{
				TextureDimension dimension = (d > 1 || force3D) ? TextureDimension.Tex3D : TextureDimension.Tex2D;
				renderTexture = new RenderTexture(w, h, 0, format)
				{
					dimension = dimension,
					filterMode = FilterMode.Bilinear,
					wrapMode = TextureWrapMode.Clamp,
					anisoLevel = 0,
					volumeDepth = d,
					enableRandomWrite = enableRandomWrite
				};
				renderTexture.Create();
			}
			else
			{
				this.m_Recycled.RemoveAt(i);
			}
			this.m_Actives.Add(renderTexture);
			return renderTexture;
		}

		// Token: 0x06003EFA RID: 16122 RVA: 0x00171A0C File Offset: 0x0016FC0C
		internal Texture Lerp(Texture from, Texture to, float t)
		{
			Assert.IsNotNull<Texture>(from);
			Assert.IsNotNull<Texture>(to);
			Assert.AreEqual(from.width, to.width);
			Assert.AreEqual(from.height, to.height);
			if (from == to)
			{
				return from;
			}
			if (t <= 0f)
			{
				return from;
			}
			if (t >= 1f)
			{
				return to;
			}
			RenderTexture renderTexture;
			if (from is Texture3D || (from is RenderTexture && ((RenderTexture)from).volumeDepth > 1))
			{
				int num = (from is Texture3D) ? ((Texture3D)from).depth : ((RenderTexture)from).volumeDepth;
				int num2 = Mathf.Max(Mathf.Max(from.width, from.height), num);
				renderTexture = this.Get(RenderTextureFormat.ARGBHalf, from.width, from.height, num, true, true);
				ComputeShader texture3dLerp = this.m_Resources.computeShaders.texture3dLerp;
				int kernelIndex = texture3dLerp.FindKernel("KTexture3DLerp");
				this.m_Command.SetComputeVectorParam(texture3dLerp, "_DimensionsAndLerp", new Vector4((float)from.width, (float)from.height, (float)num, t));
				this.m_Command.SetComputeTextureParam(texture3dLerp, kernelIndex, "_Output", renderTexture);
				this.m_Command.SetComputeTextureParam(texture3dLerp, kernelIndex, "_From", from);
				this.m_Command.SetComputeTextureParam(texture3dLerp, kernelIndex, "_To", to);
				uint num3;
				uint actual;
				uint num4;
				texture3dLerp.GetKernelThreadGroupSizes(kernelIndex, out num3, out actual, out num4);
				Assert.AreEqual(num3, actual);
				int num5 = Mathf.CeilToInt((float)num2 / num3);
				int threadGroupsZ = Mathf.CeilToInt((float)num2 / num4);
				this.m_Command.DispatchCompute(texture3dLerp, kernelIndex, num5, num5, threadGroupsZ);
				return renderTexture;
			}
			RenderTextureFormat uncompressedRenderTextureFormat = TextureFormatUtilities.GetUncompressedRenderTextureFormat(to);
			renderTexture = this.Get(uncompressedRenderTextureFormat, to.width, to.height, 1, false, false);
			PropertySheet propertySheet = this.m_PropertySheets.Get(this.m_Resources.shaders.texture2dLerp);
			propertySheet.properties.SetTexture(ShaderIDs.To, to);
			propertySheet.properties.SetFloat(ShaderIDs.Interp, t);
			this.m_Command.BlitFullscreenTriangle(from, renderTexture, propertySheet, 0, false, null);
			return renderTexture;
		}

		// Token: 0x06003EFB RID: 16123 RVA: 0x00171C40 File Offset: 0x0016FE40
		internal Texture Lerp(Texture from, Color to, float t)
		{
			Assert.IsNotNull<Texture>(from);
			if ((double)t < 1E-05)
			{
				return from;
			}
			RenderTexture renderTexture;
			if (from is Texture3D || (from is RenderTexture && ((RenderTexture)from).volumeDepth > 1))
			{
				int num = (from is Texture3D) ? ((Texture3D)from).depth : ((RenderTexture)from).volumeDepth;
				float num2 = (float)Mathf.Max(Mathf.Max(from.width, from.height), num);
				renderTexture = this.Get(RenderTextureFormat.ARGBHalf, from.width, from.height, num, true, true);
				ComputeShader texture3dLerp = this.m_Resources.computeShaders.texture3dLerp;
				int kernelIndex = texture3dLerp.FindKernel("KTexture3DLerpToColor");
				this.m_Command.SetComputeVectorParam(texture3dLerp, "_DimensionsAndLerp", new Vector4((float)from.width, (float)from.height, (float)num, t));
				this.m_Command.SetComputeVectorParam(texture3dLerp, "_TargetColor", new Vector4(to.r, to.g, to.b, to.a));
				this.m_Command.SetComputeTextureParam(texture3dLerp, kernelIndex, "_Output", renderTexture);
				this.m_Command.SetComputeTextureParam(texture3dLerp, kernelIndex, "_From", from);
				int num3 = Mathf.CeilToInt(num2 / 4f);
				this.m_Command.DispatchCompute(texture3dLerp, kernelIndex, num3, num3, num3);
				return renderTexture;
			}
			RenderTextureFormat uncompressedRenderTextureFormat = TextureFormatUtilities.GetUncompressedRenderTextureFormat(from);
			renderTexture = this.Get(uncompressedRenderTextureFormat, from.width, from.height, 1, false, false);
			PropertySheet propertySheet = this.m_PropertySheets.Get(this.m_Resources.shaders.texture2dLerp);
			propertySheet.properties.SetVector(ShaderIDs.TargetColor, new Vector4(to.r, to.g, to.b, to.a));
			propertySheet.properties.SetFloat(ShaderIDs.Interp, t);
			this.m_Command.BlitFullscreenTriangle(from, renderTexture, propertySheet, 1, false, null);
			return renderTexture;
		}

		// Token: 0x06003EFC RID: 16124 RVA: 0x00171E48 File Offset: 0x00170048
		internal void Clear()
		{
			foreach (RenderTexture obj in this.m_Actives)
			{
				RuntimeUtilities.Destroy(obj);
			}
			foreach (RenderTexture obj2 in this.m_Recycled)
			{
				RuntimeUtilities.Destroy(obj2);
			}
			this.m_Actives.Clear();
			this.m_Recycled.Clear();
		}

		// Token: 0x0400383E RID: 14398
		private static TextureLerper m_Instance;

		// Token: 0x0400383F RID: 14399
		private CommandBuffer m_Command;

		// Token: 0x04003840 RID: 14400
		private PropertySheetFactory m_PropertySheets;

		// Token: 0x04003841 RID: 14401
		private PostProcessResources m_Resources;

		// Token: 0x04003842 RID: 14402
		private List<RenderTexture> m_Recycled;

		// Token: 0x04003843 RID: 14403
		private List<RenderTexture> m_Actives;
	}
}
