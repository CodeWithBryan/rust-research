using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A2E RID: 2606
	[Preserve]
	[Serializable]
	public sealed class TemporalAntialiasing
	{
		// Token: 0x17000509 RID: 1289
		// (get) Token: 0x06003DBF RID: 15807 RVA: 0x0016BA8C File Offset: 0x00169C8C
		// (set) Token: 0x06003DC0 RID: 15808 RVA: 0x0016BA94 File Offset: 0x00169C94
		public Vector2 jitter { get; private set; }

		// Token: 0x1700050A RID: 1290
		// (get) Token: 0x06003DC1 RID: 15809 RVA: 0x0016BA9D File Offset: 0x00169C9D
		// (set) Token: 0x06003DC2 RID: 15810 RVA: 0x0016BAA5 File Offset: 0x00169CA5
		public Vector2 jitterRaw { get; private set; }

		// Token: 0x1700050B RID: 1291
		// (get) Token: 0x06003DC3 RID: 15811 RVA: 0x0016BAAE File Offset: 0x00169CAE
		// (set) Token: 0x06003DC4 RID: 15812 RVA: 0x0016BAB6 File Offset: 0x00169CB6
		public int sampleIndex { get; private set; }

		// Token: 0x1700050C RID: 1292
		// (get) Token: 0x06003DC5 RID: 15813 RVA: 0x0016BABF File Offset: 0x00169CBF
		// (set) Token: 0x06003DC6 RID: 15814 RVA: 0x0016BAC7 File Offset: 0x00169CC7
		public int sampleCount { get; set; }

		// Token: 0x06003DC7 RID: 15815 RVA: 0x0016BAD0 File Offset: 0x00169CD0
		public bool IsSupported()
		{
			return SystemInfo.supportedRenderTargetCount >= 2 && SystemInfo.supportsMotionVectors && SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES2;
		}

		// Token: 0x06003DC8 RID: 15816 RVA: 0x0001F1CE File Offset: 0x0001D3CE
		internal DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
		}

		// Token: 0x06003DC9 RID: 15817 RVA: 0x0016BAEE File Offset: 0x00169CEE
		internal void ResetHistory()
		{
			this.m_ResetHistory = true;
		}

		// Token: 0x06003DCA RID: 15818 RVA: 0x0016BAF8 File Offset: 0x00169CF8
		private Vector2 GenerateRandomOffset()
		{
			Vector2 result = new Vector2(HaltonSeq.Get((this.sampleIndex & 1023) + 1, 2) - 0.5f, HaltonSeq.Get((this.sampleIndex & 1023) + 1, 3) - 0.5f);
			int num = this.sampleIndex + 1;
			this.sampleIndex = num;
			if (num >= this.sampleCount)
			{
				this.sampleIndex = 0;
			}
			return result;
		}

		// Token: 0x06003DCB RID: 15819 RVA: 0x0016BB60 File Offset: 0x00169D60
		public Matrix4x4 GetJitteredProjectionMatrix(Camera camera)
		{
			this.jitter = this.GenerateRandomOffset();
			this.jitter *= this.jitterSpread;
			Matrix4x4 result;
			if (this.jitteredMatrixFunc != null)
			{
				result = this.jitteredMatrixFunc(camera, this.jitter);
			}
			else
			{
				result = (camera.orthographic ? RuntimeUtilities.GetJitteredOrthographicProjectionMatrix(camera, this.jitter) : RuntimeUtilities.GetJitteredPerspectiveProjectionMatrix(camera, this.jitter));
			}
			this.jitterRaw = this.jitter;
			this.jitter = new Vector2(this.jitter.x / (float)camera.pixelWidth, this.jitter.y / (float)camera.pixelHeight);
			return result;
		}

		// Token: 0x06003DCC RID: 15820 RVA: 0x0016BC10 File Offset: 0x00169E10
		public void ConfigureJitteredProjectionMatrix(PostProcessRenderContext context)
		{
			Camera camera = context.camera;
			camera.nonJitteredProjectionMatrix = camera.projectionMatrix;
			camera.projectionMatrix = this.GetJitteredProjectionMatrix(camera);
			camera.useJitteredProjectionMatrixForTransparentRendering = true;
		}

		// Token: 0x06003DCD RID: 15821 RVA: 0x0016BC44 File Offset: 0x00169E44
		public void ConfigureStereoJitteredProjectionMatrices(PostProcessRenderContext context)
		{
			Camera camera = context.camera;
			this.jitter = this.GenerateRandomOffset();
			this.jitter *= this.jitterSpread;
			for (Camera.StereoscopicEye stereoscopicEye = Camera.StereoscopicEye.Left; stereoscopicEye <= Camera.StereoscopicEye.Right; stereoscopicEye++)
			{
				context.camera.CopyStereoDeviceProjectionMatrixToNonJittered(stereoscopicEye);
				Matrix4x4 stereoNonJitteredProjectionMatrix = context.camera.GetStereoNonJitteredProjectionMatrix(stereoscopicEye);
				Matrix4x4 matrix = RuntimeUtilities.GenerateJitteredProjectionMatrixFromOriginal(context, stereoNonJitteredProjectionMatrix, this.jitter);
				context.camera.SetStereoProjectionMatrix(stereoscopicEye, matrix);
			}
			this.jitter = new Vector2(this.jitter.x / (float)context.screenWidth, this.jitter.y / (float)context.screenHeight);
			camera.useJitteredProjectionMatrixForTransparentRendering = true;
		}

		// Token: 0x06003DCE RID: 15822 RVA: 0x0016BCF4 File Offset: 0x00169EF4
		private void GenerateHistoryName(RenderTexture rt, int id, PostProcessRenderContext context)
		{
			rt.name = "Temporal Anti-aliasing History id #" + id;
			if (context.stereoActive)
			{
				rt.name = rt.name + " for eye " + context.xrActiveEye;
			}
		}

		// Token: 0x06003DCF RID: 15823 RVA: 0x0016BD40 File Offset: 0x00169F40
		private RenderTexture CheckHistory(int id, PostProcessRenderContext context)
		{
			int xrActiveEye = context.xrActiveEye;
			if (this.m_HistoryTextures[xrActiveEye] == null)
			{
				this.m_HistoryTextures[xrActiveEye] = new RenderTexture[2];
			}
			RenderTexture renderTexture = this.m_HistoryTextures[xrActiveEye][id];
			if (this.m_ResetHistory || renderTexture == null || !renderTexture.IsCreated())
			{
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = context.GetScreenSpaceTemporaryRT(0, context.sourceFormat, RenderTextureReadWrite.Default, 0, 0);
				this.GenerateHistoryName(renderTexture, id, context);
				renderTexture.filterMode = FilterMode.Bilinear;
				this.m_HistoryTextures[xrActiveEye][id] = renderTexture;
				context.command.BlitFullscreenTriangle(context.source, renderTexture, false, null);
			}
			else if (renderTexture.width != context.width || renderTexture.height != context.height)
			{
				RenderTexture screenSpaceTemporaryRT = context.GetScreenSpaceTemporaryRT(0, context.sourceFormat, RenderTextureReadWrite.Default, 0, 0);
				this.GenerateHistoryName(screenSpaceTemporaryRT, id, context);
				screenSpaceTemporaryRT.filterMode = FilterMode.Bilinear;
				this.m_HistoryTextures[xrActiveEye][id] = screenSpaceTemporaryRT;
				context.command.BlitFullscreenTriangle(renderTexture, screenSpaceTemporaryRT, false, null);
				RenderTexture.ReleaseTemporary(renderTexture);
			}
			return this.m_HistoryTextures[xrActiveEye][id];
		}

		// Token: 0x06003DD0 RID: 15824 RVA: 0x0016BE60 File Offset: 0x0016A060
		internal void Render(PostProcessRenderContext context)
		{
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.temporalAntialiasing);
			CommandBuffer command = context.command;
			command.BeginSample("TemporalAntialiasing");
			int num = this.m_HistoryPingPong[context.xrActiveEye];
			RenderTexture value = this.CheckHistory(++num % 2, context);
			RenderTexture tex = this.CheckHistory(++num % 2, context);
			this.m_HistoryPingPong[context.xrActiveEye] = (num + 1) % 2;
			propertySheet.properties.SetVector(ShaderIDs.Jitter, this.jitter);
			propertySheet.properties.SetFloat(ShaderIDs.Sharpness, this.sharpness);
			propertySheet.properties.SetVector(ShaderIDs.FinalBlendParameters, new Vector4(this.stationaryBlending, this.motionBlending, 6000f, 0f));
			propertySheet.properties.SetTexture(ShaderIDs.HistoryTex, value);
			int pass = context.camera.orthographic ? 1 : 0;
			this.m_Mrt[0] = context.destination;
			this.m_Mrt[1] = tex;
			command.BlitFullscreenTriangle(context.source, this.m_Mrt, context.source, propertySheet, pass, false, null);
			command.EndSample("TemporalAntialiasing");
			this.m_ResetHistory = false;
		}

		// Token: 0x06003DD1 RID: 15825 RVA: 0x0016BFB8 File Offset: 0x0016A1B8
		internal void Release()
		{
			if (this.m_HistoryTextures != null)
			{
				for (int i = 0; i < this.m_HistoryTextures.Length; i++)
				{
					if (this.m_HistoryTextures[i] != null)
					{
						for (int j = 0; j < this.m_HistoryTextures[i].Length; j++)
						{
							RenderTexture.ReleaseTemporary(this.m_HistoryTextures[i][j]);
							this.m_HistoryTextures[i][j] = null;
						}
						this.m_HistoryTextures[i] = null;
					}
				}
			}
			this.sampleIndex = 0;
			this.m_HistoryPingPong[0] = 0;
			this.m_HistoryPingPong[1] = 0;
			this.ResetHistory();
		}

		// Token: 0x04003701 RID: 14081
		[Tooltip("The diameter (in texels) inside which jitter samples are spread. Smaller values result in crisper but more aliased output, while larger values result in more stable, but blurrier, output.")]
		[Range(0.1f, 1f)]
		public float jitterSpread = 0.75f;

		// Token: 0x04003702 RID: 14082
		[Tooltip("Controls the amount of sharpening applied to the color buffer. High values may introduce dark-border artifacts.")]
		[Range(0f, 3f)]
		public float sharpness = 0.25f;

		// Token: 0x04003703 RID: 14083
		[Tooltip("The blend coefficient for a stationary fragment. Controls the percentage of history sample blended into the final color.")]
		[Range(0f, 0.99f)]
		public float stationaryBlending = 0.95f;

		// Token: 0x04003704 RID: 14084
		[Tooltip("The blend coefficient for a fragment with significant motion. Controls the percentage of history sample blended into the final color.")]
		[Range(0f, 0.99f)]
		public float motionBlending = 0.85f;

		// Token: 0x04003705 RID: 14085
		public Func<Camera, Vector2, Matrix4x4> jitteredMatrixFunc;

		// Token: 0x04003708 RID: 14088
		private readonly RenderTargetIdentifier[] m_Mrt = new RenderTargetIdentifier[2];

		// Token: 0x04003709 RID: 14089
		private bool m_ResetHistory = true;

		// Token: 0x0400370C RID: 14092
		private const int k_NumEyes = 2;

		// Token: 0x0400370D RID: 14093
		private const int k_NumHistoryTextures = 2;

		// Token: 0x0400370E RID: 14094
		private readonly RenderTexture[][] m_HistoryTextures = new RenderTexture[2][];

		// Token: 0x0400370F RID: 14095
		private readonly int[] m_HistoryPingPong = new int[2];

		// Token: 0x02000ECA RID: 3786
		private enum Pass
		{
			// Token: 0x04004C10 RID: 19472
			SolverDilate,
			// Token: 0x04004C11 RID: 19473
			SolverNoDilate
		}
	}
}
