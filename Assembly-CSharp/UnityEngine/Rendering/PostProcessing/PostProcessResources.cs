using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A51 RID: 2641
	public sealed class PostProcessResources : ScriptableObject
	{
		// Token: 0x0400377A RID: 14202
		public Texture2D[] blueNoise64;

		// Token: 0x0400377B RID: 14203
		public Texture2D[] blueNoise256;

		// Token: 0x0400377C RID: 14204
		public PostProcessResources.SMAALuts smaaLuts;

		// Token: 0x0400377D RID: 14205
		public PostProcessResources.Shaders shaders;

		// Token: 0x0400377E RID: 14206
		public PostProcessResources.ComputeShaders computeShaders;

		// Token: 0x02000ED0 RID: 3792
		[Serializable]
		public sealed class Shaders
		{
			// Token: 0x0600513F RID: 20799 RVA: 0x001A42EB File Offset: 0x001A24EB
			public PostProcessResources.Shaders Clone()
			{
				return (PostProcessResources.Shaders)base.MemberwiseClone();
			}

			// Token: 0x04004C23 RID: 19491
			public Shader bloom;

			// Token: 0x04004C24 RID: 19492
			public Shader copy;

			// Token: 0x04004C25 RID: 19493
			public Shader copyStd;

			// Token: 0x04004C26 RID: 19494
			public Shader copyStdFromTexArray;

			// Token: 0x04004C27 RID: 19495
			public Shader copyStdFromDoubleWide;

			// Token: 0x04004C28 RID: 19496
			public Shader discardAlpha;

			// Token: 0x04004C29 RID: 19497
			public Shader depthOfField;

			// Token: 0x04004C2A RID: 19498
			public Shader finalPass;

			// Token: 0x04004C2B RID: 19499
			public Shader grainBaker;

			// Token: 0x04004C2C RID: 19500
			public Shader motionBlur;

			// Token: 0x04004C2D RID: 19501
			public Shader temporalAntialiasing;

			// Token: 0x04004C2E RID: 19502
			public Shader subpixelMorphologicalAntialiasing;

			// Token: 0x04004C2F RID: 19503
			public Shader texture2dLerp;

			// Token: 0x04004C30 RID: 19504
			public Shader uber;

			// Token: 0x04004C31 RID: 19505
			public Shader lut2DBaker;

			// Token: 0x04004C32 RID: 19506
			public Shader lightMeter;

			// Token: 0x04004C33 RID: 19507
			public Shader gammaHistogram;

			// Token: 0x04004C34 RID: 19508
			public Shader waveform;

			// Token: 0x04004C35 RID: 19509
			public Shader vectorscope;

			// Token: 0x04004C36 RID: 19510
			public Shader debugOverlays;

			// Token: 0x04004C37 RID: 19511
			public Shader deferredFog;

			// Token: 0x04004C38 RID: 19512
			public Shader scalableAO;

			// Token: 0x04004C39 RID: 19513
			public Shader multiScaleAO;

			// Token: 0x04004C3A RID: 19514
			public Shader screenSpaceReflections;
		}

		// Token: 0x02000ED1 RID: 3793
		[Serializable]
		public sealed class ComputeShaders
		{
			// Token: 0x06005141 RID: 20801 RVA: 0x001A42F8 File Offset: 0x001A24F8
			public PostProcessResources.ComputeShaders Clone()
			{
				return (PostProcessResources.ComputeShaders)base.MemberwiseClone();
			}

			// Token: 0x04004C3B RID: 19515
			public ComputeShader autoExposure;

			// Token: 0x04004C3C RID: 19516
			public ComputeShader exposureHistogram;

			// Token: 0x04004C3D RID: 19517
			public ComputeShader lut3DBaker;

			// Token: 0x04004C3E RID: 19518
			public ComputeShader texture3dLerp;

			// Token: 0x04004C3F RID: 19519
			public ComputeShader gammaHistogram;

			// Token: 0x04004C40 RID: 19520
			public ComputeShader waveform;

			// Token: 0x04004C41 RID: 19521
			public ComputeShader vectorscope;

			// Token: 0x04004C42 RID: 19522
			public ComputeShader multiScaleAODownsample1;

			// Token: 0x04004C43 RID: 19523
			public ComputeShader multiScaleAODownsample2;

			// Token: 0x04004C44 RID: 19524
			public ComputeShader multiScaleAORender;

			// Token: 0x04004C45 RID: 19525
			public ComputeShader multiScaleAOUpsample;

			// Token: 0x04004C46 RID: 19526
			public ComputeShader gaussianDownsample;
		}

		// Token: 0x02000ED2 RID: 3794
		[Serializable]
		public sealed class SMAALuts
		{
			// Token: 0x04004C47 RID: 19527
			public Texture2D area;

			// Token: 0x04004C48 RID: 19528
			public Texture2D search;
		}
	}
}
