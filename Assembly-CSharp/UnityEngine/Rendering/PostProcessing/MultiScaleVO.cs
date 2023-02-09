using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A25 RID: 2597
	[Preserve]
	[Serializable]
	internal sealed class MultiScaleVO : IAmbientOcclusionMethod
	{
		// Token: 0x06003D94 RID: 15764 RVA: 0x0016993C File Offset: 0x00167B3C
		public MultiScaleVO(AmbientOcclusion settings)
		{
			this.m_Settings = settings;
		}

		// Token: 0x06003D95 RID: 15765 RVA: 0x00003A54 File Offset: 0x00001C54
		public DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth;
		}

		// Token: 0x06003D96 RID: 15766 RVA: 0x00169A5B File Offset: 0x00167C5B
		public void SetResources(PostProcessResources resources)
		{
			this.m_Resources = resources;
		}

		// Token: 0x06003D97 RID: 15767 RVA: 0x00169A64 File Offset: 0x00167C64
		private void Alloc(CommandBuffer cmd, int id, MultiScaleVO.MipLevel size, RenderTextureFormat format, bool uav)
		{
			cmd.GetTemporaryRT(id, new RenderTextureDescriptor
			{
				width = this.m_Widths[(int)size],
				height = this.m_Heights[(int)size],
				colorFormat = format,
				depthBufferBits = 0,
				volumeDepth = 1,
				autoGenerateMips = false,
				msaaSamples = 1,
				enableRandomWrite = uav,
				dimension = TextureDimension.Tex2D,
				sRGB = false
			}, FilterMode.Point);
		}

		// Token: 0x06003D98 RID: 15768 RVA: 0x00169AE4 File Offset: 0x00167CE4
		private void AllocArray(CommandBuffer cmd, int id, MultiScaleVO.MipLevel size, RenderTextureFormat format, bool uav)
		{
			cmd.GetTemporaryRT(id, new RenderTextureDescriptor
			{
				width = this.m_Widths[(int)size],
				height = this.m_Heights[(int)size],
				colorFormat = format,
				depthBufferBits = 0,
				volumeDepth = 16,
				autoGenerateMips = false,
				msaaSamples = 1,
				enableRandomWrite = uav,
				dimension = TextureDimension.Tex2DArray,
				sRGB = false
			}, FilterMode.Point);
		}

		// Token: 0x06003D99 RID: 15769 RVA: 0x00169B65 File Offset: 0x00167D65
		private void Release(CommandBuffer cmd, int id)
		{
			cmd.ReleaseTemporaryRT(id);
		}

		// Token: 0x06003D9A RID: 15770 RVA: 0x00169B70 File Offset: 0x00167D70
		private Vector4 CalculateZBufferParams(Camera camera)
		{
			float num = camera.farClipPlane / camera.nearClipPlane;
			if (SystemInfo.usesReversedZBuffer)
			{
				return new Vector4(num - 1f, 1f, 0f, 0f);
			}
			return new Vector4(1f - num, num, 0f, 0f);
		}

		// Token: 0x06003D9B RID: 15771 RVA: 0x00169BC8 File Offset: 0x00167DC8
		private float CalculateTanHalfFovHeight(Camera camera)
		{
			return 1f / camera.projectionMatrix[0, 0];
		}

		// Token: 0x06003D9C RID: 15772 RVA: 0x00169BEB File Offset: 0x00167DEB
		private Vector2 GetSize(MultiScaleVO.MipLevel mip)
		{
			return new Vector2((float)this.m_Widths[(int)mip], (float)this.m_Heights[(int)mip]);
		}

		// Token: 0x06003D9D RID: 15773 RVA: 0x00169C04 File Offset: 0x00167E04
		private Vector3 GetSizeArray(MultiScaleVO.MipLevel mip)
		{
			return new Vector3((float)this.m_Widths[(int)mip], (float)this.m_Heights[(int)mip], 16f);
		}

		// Token: 0x06003D9E RID: 15774 RVA: 0x00169C24 File Offset: 0x00167E24
		public void GenerateAOMap(CommandBuffer cmd, Camera camera, RenderTargetIdentifier destination, RenderTargetIdentifier? depthMap, bool invert, bool isMSAA)
		{
			this.m_Widths[0] = camera.pixelWidth * (RuntimeUtilities.isSinglePassStereoEnabled ? 2 : 1);
			this.m_Heights[0] = camera.pixelHeight;
			for (int i = 1; i < 7; i++)
			{
				int num = 1 << i;
				this.m_Widths[i] = (this.m_Widths[0] + (num - 1)) / num;
				this.m_Heights[i] = (this.m_Heights[0] + (num - 1)) / num;
			}
			this.PushAllocCommands(cmd, isMSAA);
			this.PushDownsampleCommands(cmd, camera, depthMap, isMSAA);
			float tanHalfFovH = this.CalculateTanHalfFovHeight(camera);
			this.PushRenderCommands(cmd, ShaderIDs.TiledDepth1, ShaderIDs.Occlusion1, this.GetSizeArray(MultiScaleVO.MipLevel.L3), tanHalfFovH, isMSAA);
			this.PushRenderCommands(cmd, ShaderIDs.TiledDepth2, ShaderIDs.Occlusion2, this.GetSizeArray(MultiScaleVO.MipLevel.L4), tanHalfFovH, isMSAA);
			this.PushRenderCommands(cmd, ShaderIDs.TiledDepth3, ShaderIDs.Occlusion3, this.GetSizeArray(MultiScaleVO.MipLevel.L5), tanHalfFovH, isMSAA);
			this.PushRenderCommands(cmd, ShaderIDs.TiledDepth4, ShaderIDs.Occlusion4, this.GetSizeArray(MultiScaleVO.MipLevel.L6), tanHalfFovH, isMSAA);
			this.PushUpsampleCommands(cmd, ShaderIDs.LowDepth4, ShaderIDs.Occlusion4, ShaderIDs.LowDepth3, new int?(ShaderIDs.Occlusion3), ShaderIDs.Combined3, this.GetSize(MultiScaleVO.MipLevel.L4), this.GetSize(MultiScaleVO.MipLevel.L3), isMSAA, false);
			this.PushUpsampleCommands(cmd, ShaderIDs.LowDepth3, ShaderIDs.Combined3, ShaderIDs.LowDepth2, new int?(ShaderIDs.Occlusion2), ShaderIDs.Combined2, this.GetSize(MultiScaleVO.MipLevel.L3), this.GetSize(MultiScaleVO.MipLevel.L2), isMSAA, false);
			this.PushUpsampleCommands(cmd, ShaderIDs.LowDepth2, ShaderIDs.Combined2, ShaderIDs.LowDepth1, new int?(ShaderIDs.Occlusion1), ShaderIDs.Combined1, this.GetSize(MultiScaleVO.MipLevel.L2), this.GetSize(MultiScaleVO.MipLevel.L1), isMSAA, false);
			this.PushUpsampleCommands(cmd, ShaderIDs.LowDepth1, ShaderIDs.Combined1, ShaderIDs.LinearDepth, null, destination, this.GetSize(MultiScaleVO.MipLevel.L1), this.GetSize(MultiScaleVO.MipLevel.Original), isMSAA, invert);
			this.PushReleaseCommands(cmd);
		}

		// Token: 0x06003D9F RID: 15775 RVA: 0x00169E20 File Offset: 0x00168020
		private void PushAllocCommands(CommandBuffer cmd, bool isMSAA)
		{
			if (isMSAA)
			{
				this.Alloc(cmd, ShaderIDs.LinearDepth, MultiScaleVO.MipLevel.Original, RenderTextureFormat.RGHalf, true);
				this.Alloc(cmd, ShaderIDs.LowDepth1, MultiScaleVO.MipLevel.L1, RenderTextureFormat.RGFloat, true);
				this.Alloc(cmd, ShaderIDs.LowDepth2, MultiScaleVO.MipLevel.L2, RenderTextureFormat.RGFloat, true);
				this.Alloc(cmd, ShaderIDs.LowDepth3, MultiScaleVO.MipLevel.L3, RenderTextureFormat.RGFloat, true);
				this.Alloc(cmd, ShaderIDs.LowDepth4, MultiScaleVO.MipLevel.L4, RenderTextureFormat.RGFloat, true);
				this.AllocArray(cmd, ShaderIDs.TiledDepth1, MultiScaleVO.MipLevel.L3, RenderTextureFormat.RGHalf, true);
				this.AllocArray(cmd, ShaderIDs.TiledDepth2, MultiScaleVO.MipLevel.L4, RenderTextureFormat.RGHalf, true);
				this.AllocArray(cmd, ShaderIDs.TiledDepth3, MultiScaleVO.MipLevel.L5, RenderTextureFormat.RGHalf, true);
				this.AllocArray(cmd, ShaderIDs.TiledDepth4, MultiScaleVO.MipLevel.L6, RenderTextureFormat.RGHalf, true);
				this.Alloc(cmd, ShaderIDs.Occlusion1, MultiScaleVO.MipLevel.L1, RenderTextureFormat.RG16, true);
				this.Alloc(cmd, ShaderIDs.Occlusion2, MultiScaleVO.MipLevel.L2, RenderTextureFormat.RG16, true);
				this.Alloc(cmd, ShaderIDs.Occlusion3, MultiScaleVO.MipLevel.L3, RenderTextureFormat.RG16, true);
				this.Alloc(cmd, ShaderIDs.Occlusion4, MultiScaleVO.MipLevel.L4, RenderTextureFormat.RG16, true);
				this.Alloc(cmd, ShaderIDs.Combined1, MultiScaleVO.MipLevel.L1, RenderTextureFormat.RG16, true);
				this.Alloc(cmd, ShaderIDs.Combined2, MultiScaleVO.MipLevel.L2, RenderTextureFormat.RG16, true);
				this.Alloc(cmd, ShaderIDs.Combined3, MultiScaleVO.MipLevel.L3, RenderTextureFormat.RG16, true);
				return;
			}
			this.Alloc(cmd, ShaderIDs.LinearDepth, MultiScaleVO.MipLevel.Original, RenderTextureFormat.RHalf, true);
			this.Alloc(cmd, ShaderIDs.LowDepth1, MultiScaleVO.MipLevel.L1, RenderTextureFormat.RFloat, true);
			this.Alloc(cmd, ShaderIDs.LowDepth2, MultiScaleVO.MipLevel.L2, RenderTextureFormat.RFloat, true);
			this.Alloc(cmd, ShaderIDs.LowDepth3, MultiScaleVO.MipLevel.L3, RenderTextureFormat.RFloat, true);
			this.Alloc(cmd, ShaderIDs.LowDepth4, MultiScaleVO.MipLevel.L4, RenderTextureFormat.RFloat, true);
			this.AllocArray(cmd, ShaderIDs.TiledDepth1, MultiScaleVO.MipLevel.L3, RenderTextureFormat.RHalf, true);
			this.AllocArray(cmd, ShaderIDs.TiledDepth2, MultiScaleVO.MipLevel.L4, RenderTextureFormat.RHalf, true);
			this.AllocArray(cmd, ShaderIDs.TiledDepth3, MultiScaleVO.MipLevel.L5, RenderTextureFormat.RHalf, true);
			this.AllocArray(cmd, ShaderIDs.TiledDepth4, MultiScaleVO.MipLevel.L6, RenderTextureFormat.RHalf, true);
			this.Alloc(cmd, ShaderIDs.Occlusion1, MultiScaleVO.MipLevel.L1, RenderTextureFormat.R8, true);
			this.Alloc(cmd, ShaderIDs.Occlusion2, MultiScaleVO.MipLevel.L2, RenderTextureFormat.R8, true);
			this.Alloc(cmd, ShaderIDs.Occlusion3, MultiScaleVO.MipLevel.L3, RenderTextureFormat.R8, true);
			this.Alloc(cmd, ShaderIDs.Occlusion4, MultiScaleVO.MipLevel.L4, RenderTextureFormat.R8, true);
			this.Alloc(cmd, ShaderIDs.Combined1, MultiScaleVO.MipLevel.L1, RenderTextureFormat.R8, true);
			this.Alloc(cmd, ShaderIDs.Combined2, MultiScaleVO.MipLevel.L2, RenderTextureFormat.R8, true);
			this.Alloc(cmd, ShaderIDs.Combined3, MultiScaleVO.MipLevel.L3, RenderTextureFormat.R8, true);
		}

		// Token: 0x06003DA0 RID: 15776 RVA: 0x0016A034 File Offset: 0x00168234
		private void PushDownsampleCommands(CommandBuffer cmd, Camera camera, RenderTargetIdentifier? depthMap, bool isMSAA)
		{
			bool flag = false;
			RenderTargetIdentifier renderTargetIdentifier;
			if (depthMap != null)
			{
				renderTargetIdentifier = depthMap.Value;
			}
			else if (!RuntimeUtilities.IsResolvedDepthAvailable(camera))
			{
				this.Alloc(cmd, ShaderIDs.DepthCopy, MultiScaleVO.MipLevel.Original, RenderTextureFormat.RFloat, false);
				renderTargetIdentifier = new RenderTargetIdentifier(ShaderIDs.DepthCopy);
				cmd.BlitFullscreenTriangle(BuiltinRenderTextureType.None, renderTargetIdentifier, this.m_PropertySheet, 0, false, null);
				flag = true;
			}
			else
			{
				renderTargetIdentifier = BuiltinRenderTextureType.ResolvedDepth;
			}
			ComputeShader computeShader = this.m_Resources.computeShaders.multiScaleAODownsample1;
			int kernelIndex = computeShader.FindKernel(isMSAA ? "MultiScaleVODownsample1_MSAA" : "MultiScaleVODownsample1");
			cmd.SetComputeTextureParam(computeShader, kernelIndex, "LinearZ", ShaderIDs.LinearDepth);
			cmd.SetComputeTextureParam(computeShader, kernelIndex, "DS2x", ShaderIDs.LowDepth1);
			cmd.SetComputeTextureParam(computeShader, kernelIndex, "DS4x", ShaderIDs.LowDepth2);
			cmd.SetComputeTextureParam(computeShader, kernelIndex, "DS2xAtlas", ShaderIDs.TiledDepth1);
			cmd.SetComputeTextureParam(computeShader, kernelIndex, "DS4xAtlas", ShaderIDs.TiledDepth2);
			cmd.SetComputeVectorParam(computeShader, "ZBufferParams", this.CalculateZBufferParams(camera));
			cmd.SetComputeTextureParam(computeShader, kernelIndex, "Depth", renderTargetIdentifier);
			cmd.DispatchCompute(computeShader, kernelIndex, this.m_Widths[4], this.m_Heights[4], 1);
			if (flag)
			{
				this.Release(cmd, ShaderIDs.DepthCopy);
			}
			computeShader = this.m_Resources.computeShaders.multiScaleAODownsample2;
			kernelIndex = (isMSAA ? computeShader.FindKernel("MultiScaleVODownsample2_MSAA") : computeShader.FindKernel("MultiScaleVODownsample2"));
			cmd.SetComputeTextureParam(computeShader, kernelIndex, "DS4x", ShaderIDs.LowDepth2);
			cmd.SetComputeTextureParam(computeShader, kernelIndex, "DS8x", ShaderIDs.LowDepth3);
			cmd.SetComputeTextureParam(computeShader, kernelIndex, "DS16x", ShaderIDs.LowDepth4);
			cmd.SetComputeTextureParam(computeShader, kernelIndex, "DS8xAtlas", ShaderIDs.TiledDepth3);
			cmd.SetComputeTextureParam(computeShader, kernelIndex, "DS16xAtlas", ShaderIDs.TiledDepth4);
			cmd.DispatchCompute(computeShader, kernelIndex, this.m_Widths[6], this.m_Heights[6], 1);
		}

		// Token: 0x06003DA1 RID: 15777 RVA: 0x0016A244 File Offset: 0x00168444
		private void PushRenderCommands(CommandBuffer cmd, int source, int destination, Vector3 sourceSize, float tanHalfFovH, bool isMSAA)
		{
			float num = 2f * tanHalfFovH * 10f / sourceSize.x;
			if (RuntimeUtilities.isSinglePassStereoEnabled)
			{
				num *= 2f;
			}
			float num2 = 1f / num;
			for (int i = 0; i < 12; i++)
			{
				this.m_InvThicknessTable[i] = num2 / this.m_SampleThickness[i];
			}
			this.m_SampleWeightTable[0] = 4f * this.m_SampleThickness[0];
			this.m_SampleWeightTable[1] = 4f * this.m_SampleThickness[1];
			this.m_SampleWeightTable[2] = 4f * this.m_SampleThickness[2];
			this.m_SampleWeightTable[3] = 4f * this.m_SampleThickness[3];
			this.m_SampleWeightTable[4] = 4f * this.m_SampleThickness[4];
			this.m_SampleWeightTable[5] = 8f * this.m_SampleThickness[5];
			this.m_SampleWeightTable[6] = 8f * this.m_SampleThickness[6];
			this.m_SampleWeightTable[7] = 8f * this.m_SampleThickness[7];
			this.m_SampleWeightTable[8] = 4f * this.m_SampleThickness[8];
			this.m_SampleWeightTable[9] = 8f * this.m_SampleThickness[9];
			this.m_SampleWeightTable[10] = 8f * this.m_SampleThickness[10];
			this.m_SampleWeightTable[11] = 4f * this.m_SampleThickness[11];
			this.m_SampleWeightTable[0] = 0f;
			this.m_SampleWeightTable[2] = 0f;
			this.m_SampleWeightTable[5] = 0f;
			this.m_SampleWeightTable[7] = 0f;
			this.m_SampleWeightTable[9] = 0f;
			float num3 = 0f;
			foreach (float num4 in this.m_SampleWeightTable)
			{
				num3 += num4;
			}
			for (int k = 0; k < this.m_SampleWeightTable.Length; k++)
			{
				this.m_SampleWeightTable[k] /= num3;
			}
			ComputeShader multiScaleAORender = this.m_Resources.computeShaders.multiScaleAORender;
			int kernelIndex = isMSAA ? multiScaleAORender.FindKernel("MultiScaleVORender_MSAA_interleaved") : multiScaleAORender.FindKernel("MultiScaleVORender_interleaved");
			cmd.SetComputeFloatParams(multiScaleAORender, "gInvThicknessTable", this.m_InvThicknessTable);
			cmd.SetComputeFloatParams(multiScaleAORender, "gSampleWeightTable", this.m_SampleWeightTable);
			cmd.SetComputeVectorParam(multiScaleAORender, "gInvSliceDimension", new Vector2(1f / sourceSize.x, 1f / sourceSize.y));
			cmd.SetComputeVectorParam(multiScaleAORender, "AdditionalParams", new Vector2(-1f / this.m_Settings.thicknessModifier.value, this.m_Settings.intensity.value));
			cmd.SetComputeTextureParam(multiScaleAORender, kernelIndex, "DepthTex", source);
			cmd.SetComputeTextureParam(multiScaleAORender, kernelIndex, "Occlusion", destination);
			uint num5;
			uint num6;
			uint num7;
			multiScaleAORender.GetKernelThreadGroupSizes(kernelIndex, out num5, out num6, out num7);
			cmd.DispatchCompute(multiScaleAORender, kernelIndex, ((int)sourceSize.x + (int)num5 - 1) / (int)num5, ((int)sourceSize.y + (int)num6 - 1) / (int)num6, ((int)sourceSize.z + (int)num7 - 1) / (int)num7);
		}

		// Token: 0x06003DA2 RID: 15778 RVA: 0x0016A578 File Offset: 0x00168778
		private void PushUpsampleCommands(CommandBuffer cmd, int lowResDepth, int interleavedAO, int highResDepth, int? highResAO, RenderTargetIdentifier dest, Vector3 lowResDepthSize, Vector2 highResDepthSize, bool isMSAA, bool invert = false)
		{
			ComputeShader multiScaleAOUpsample = this.m_Resources.computeShaders.multiScaleAOUpsample;
			int kernelIndex;
			if (!isMSAA)
			{
				kernelIndex = multiScaleAOUpsample.FindKernel((highResAO == null) ? (invert ? "MultiScaleVOUpSample_invert" : "MultiScaleVOUpSample") : "MultiScaleVOUpSample_blendout");
			}
			else
			{
				kernelIndex = multiScaleAOUpsample.FindKernel((highResAO == null) ? (invert ? "MultiScaleVOUpSample_MSAA_invert" : "MultiScaleVOUpSample_MSAA") : "MultiScaleVOUpSample_MSAA_blendout");
			}
			float num = 1920f / lowResDepthSize.x;
			float num2 = 1f - Mathf.Pow(10f, this.m_Settings.blurTolerance.value) * num;
			num2 *= num2;
			float num3 = Mathf.Pow(10f, this.m_Settings.upsampleTolerance.value);
			float x = 1f / (Mathf.Pow(10f, this.m_Settings.noiseFilterTolerance.value) + num3);
			cmd.SetComputeVectorParam(multiScaleAOUpsample, "InvLowResolution", new Vector2(1f / lowResDepthSize.x, 1f / lowResDepthSize.y));
			cmd.SetComputeVectorParam(multiScaleAOUpsample, "InvHighResolution", new Vector2(1f / highResDepthSize.x, 1f / highResDepthSize.y));
			cmd.SetComputeVectorParam(multiScaleAOUpsample, "AdditionalParams", new Vector4(x, num, num2, num3));
			cmd.SetComputeTextureParam(multiScaleAOUpsample, kernelIndex, "LoResDB", lowResDepth);
			cmd.SetComputeTextureParam(multiScaleAOUpsample, kernelIndex, "HiResDB", highResDepth);
			cmd.SetComputeTextureParam(multiScaleAOUpsample, kernelIndex, "LoResAO1", interleavedAO);
			if (highResAO != null)
			{
				cmd.SetComputeTextureParam(multiScaleAOUpsample, kernelIndex, "HiResAO", highResAO.Value);
			}
			cmd.SetComputeTextureParam(multiScaleAOUpsample, kernelIndex, "AoResult", dest);
			int threadGroupsX = ((int)highResDepthSize.x + 17) / 16;
			int threadGroupsY = ((int)highResDepthSize.y + 17) / 16;
			cmd.DispatchCompute(multiScaleAOUpsample, kernelIndex, threadGroupsX, threadGroupsY, 1);
		}

		// Token: 0x06003DA3 RID: 15779 RVA: 0x0016A774 File Offset: 0x00168974
		private void PushReleaseCommands(CommandBuffer cmd)
		{
			this.Release(cmd, ShaderIDs.LinearDepth);
			this.Release(cmd, ShaderIDs.LowDepth1);
			this.Release(cmd, ShaderIDs.LowDepth2);
			this.Release(cmd, ShaderIDs.LowDepth3);
			this.Release(cmd, ShaderIDs.LowDepth4);
			this.Release(cmd, ShaderIDs.TiledDepth1);
			this.Release(cmd, ShaderIDs.TiledDepth2);
			this.Release(cmd, ShaderIDs.TiledDepth3);
			this.Release(cmd, ShaderIDs.TiledDepth4);
			this.Release(cmd, ShaderIDs.Occlusion1);
			this.Release(cmd, ShaderIDs.Occlusion2);
			this.Release(cmd, ShaderIDs.Occlusion3);
			this.Release(cmd, ShaderIDs.Occlusion4);
			this.Release(cmd, ShaderIDs.Combined1);
			this.Release(cmd, ShaderIDs.Combined2);
			this.Release(cmd, ShaderIDs.Combined3);
		}

		// Token: 0x06003DA4 RID: 15780 RVA: 0x0016A844 File Offset: 0x00168A44
		private void PreparePropertySheet(PostProcessRenderContext context)
		{
			PropertySheet propertySheet = context.propertySheets.Get(this.m_Resources.shaders.multiScaleAO);
			propertySheet.ClearKeywords();
			propertySheet.properties.SetVector(ShaderIDs.AOColor, Color.white - this.m_Settings.color.value);
			this.m_PropertySheet = propertySheet;
		}

		// Token: 0x06003DA5 RID: 15781 RVA: 0x0016A8AC File Offset: 0x00168AAC
		private void CheckAOTexture(PostProcessRenderContext context)
		{
			if (this.m_AmbientOnlyAO == null || !this.m_AmbientOnlyAO.IsCreated() || this.m_AmbientOnlyAO.width != context.width || this.m_AmbientOnlyAO.height != context.height)
			{
				RuntimeUtilities.Destroy(this.m_AmbientOnlyAO);
				this.m_AmbientOnlyAO = new RenderTexture(context.width, context.height, 0, RenderTextureFormat.R8, RenderTextureReadWrite.Linear)
				{
					hideFlags = HideFlags.DontSave,
					filterMode = FilterMode.Point,
					enableRandomWrite = true
				};
				this.m_AmbientOnlyAO.Create();
			}
		}

		// Token: 0x06003DA6 RID: 15782 RVA: 0x0016A942 File Offset: 0x00168B42
		private void PushDebug(PostProcessRenderContext context)
		{
			if (context.IsDebugOverlayEnabled(DebugOverlay.AmbientOcclusion))
			{
				context.PushDebugOverlay(context.command, this.m_AmbientOnlyAO, this.m_PropertySheet, 3);
			}
		}

		// Token: 0x06003DA7 RID: 15783 RVA: 0x0016A96C File Offset: 0x00168B6C
		public void RenderAfterOpaque(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			command.BeginSample("Ambient Occlusion");
			this.SetResources(context.resources);
			this.PreparePropertySheet(context);
			this.CheckAOTexture(context);
			if (context.camera.actualRenderingPath == RenderingPath.Forward && RenderSettings.fog)
			{
				this.m_PropertySheet.EnableKeyword("APPLY_FORWARD_FOG");
				this.m_PropertySheet.properties.SetVector(ShaderIDs.FogParams, new Vector3(RenderSettings.fogDensity, RenderSettings.fogStartDistance, RenderSettings.fogEndDistance));
			}
			this.GenerateAOMap(command, context.camera, this.m_AmbientOnlyAO, null, false, false);
			this.PushDebug(context);
			command.SetGlobalTexture(ShaderIDs.MSVOcclusionTexture, this.m_AmbientOnlyAO);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, BuiltinRenderTextureType.CameraTarget, this.m_PropertySheet, 2, RenderBufferLoadAction.Load, null);
			command.EndSample("Ambient Occlusion");
		}

		// Token: 0x06003DA8 RID: 15784 RVA: 0x0016AA68 File Offset: 0x00168C68
		public void RenderAmbientOnly(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			command.BeginSample("Ambient Occlusion Render");
			this.SetResources(context.resources);
			this.PreparePropertySheet(context);
			this.CheckAOTexture(context);
			this.GenerateAOMap(command, context.camera, this.m_AmbientOnlyAO, null, false, false);
			this.PushDebug(context);
			command.EndSample("Ambient Occlusion Render");
		}

		// Token: 0x06003DA9 RID: 15785 RVA: 0x0016AAD8 File Offset: 0x00168CD8
		public void CompositeAmbientOnly(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			command.BeginSample("Ambient Occlusion Composite");
			command.SetGlobalTexture(ShaderIDs.MSVOcclusionTexture, this.m_AmbientOnlyAO);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, this.m_MRT, BuiltinRenderTextureType.CameraTarget, this.m_PropertySheet, 1, false, null);
			command.EndSample("Ambient Occlusion Composite");
		}

		// Token: 0x06003DAA RID: 15786 RVA: 0x0016AB3F File Offset: 0x00168D3F
		public void Release()
		{
			RuntimeUtilities.Destroy(this.m_AmbientOnlyAO);
			this.m_AmbientOnlyAO = null;
		}

		// Token: 0x040036D9 RID: 14041
		private readonly float[] m_SampleThickness = new float[]
		{
			Mathf.Sqrt(0.96f),
			Mathf.Sqrt(0.84f),
			Mathf.Sqrt(0.64f),
			Mathf.Sqrt(0.35999995f),
			Mathf.Sqrt(0.91999996f),
			Mathf.Sqrt(0.79999995f),
			Mathf.Sqrt(0.59999996f),
			Mathf.Sqrt(0.31999993f),
			Mathf.Sqrt(0.67999995f),
			Mathf.Sqrt(0.47999996f),
			Mathf.Sqrt(0.19999993f),
			Mathf.Sqrt(0.27999997f)
		};

		// Token: 0x040036DA RID: 14042
		private readonly float[] m_InvThicknessTable = new float[12];

		// Token: 0x040036DB RID: 14043
		private readonly float[] m_SampleWeightTable = new float[12];

		// Token: 0x040036DC RID: 14044
		private readonly int[] m_Widths = new int[7];

		// Token: 0x040036DD RID: 14045
		private readonly int[] m_Heights = new int[7];

		// Token: 0x040036DE RID: 14046
		private AmbientOcclusion m_Settings;

		// Token: 0x040036DF RID: 14047
		private PropertySheet m_PropertySheet;

		// Token: 0x040036E0 RID: 14048
		private PostProcessResources m_Resources;

		// Token: 0x040036E1 RID: 14049
		private RenderTexture m_AmbientOnlyAO;

		// Token: 0x040036E2 RID: 14050
		private readonly RenderTargetIdentifier[] m_MRT = new RenderTargetIdentifier[]
		{
			BuiltinRenderTextureType.GBuffer0,
			BuiltinRenderTextureType.CameraTarget
		};

		// Token: 0x02000EC3 RID: 3779
		internal enum MipLevel
		{
			// Token: 0x04004BEA RID: 19434
			Original,
			// Token: 0x04004BEB RID: 19435
			L1,
			// Token: 0x04004BEC RID: 19436
			L2,
			// Token: 0x04004BED RID: 19437
			L3,
			// Token: 0x04004BEE RID: 19438
			L4,
			// Token: 0x04004BEF RID: 19439
			L5,
			// Token: 0x04004BF0 RID: 19440
			L6
		}

		// Token: 0x02000EC4 RID: 3780
		private enum Pass
		{
			// Token: 0x04004BF2 RID: 19442
			DepthCopy,
			// Token: 0x04004BF3 RID: 19443
			CompositionDeferred,
			// Token: 0x04004BF4 RID: 19444
			CompositionForward,
			// Token: 0x04004BF5 RID: 19445
			DebugOverlay
		}
	}
}
