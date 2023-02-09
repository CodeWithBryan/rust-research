using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x020009FC RID: 2556
	public class PostProcessRenderContext
	{
		// Token: 0x06003D06 RID: 15622 RVA: 0x00165664 File Offset: 0x00163864
		public void Resize(int width, int height, bool dlssEnabled)
		{
			this.screenWidth = width;
			this.width = width;
			this.screenHeight = height;
			this.height = height;
			this.dlssEnabled = dlssEnabled;
			this.m_sourceDescriptor.width = width;
			this.m_sourceDescriptor.height = height;
		}

		// Token: 0x170004F4 RID: 1268
		// (get) Token: 0x06003D07 RID: 15623 RVA: 0x001656B0 File Offset: 0x001638B0
		// (set) Token: 0x06003D08 RID: 15624 RVA: 0x001656B8 File Offset: 0x001638B8
		public Camera camera
		{
			get
			{
				return this.m_Camera;
			}
			set
			{
				this.m_Camera = value;
				if (!this.m_Camera.stereoEnabled)
				{
					this.width = this.m_Camera.pixelWidth;
					this.height = this.m_Camera.pixelHeight;
					this.m_sourceDescriptor.width = this.width;
					this.m_sourceDescriptor.height = this.height;
					this.screenWidth = this.width;
					this.screenHeight = this.height;
					this.stereoActive = false;
					this.numberOfEyes = 1;
				}
			}
		}

		// Token: 0x170004F5 RID: 1269
		// (get) Token: 0x06003D09 RID: 15625 RVA: 0x00165743 File Offset: 0x00163943
		// (set) Token: 0x06003D0A RID: 15626 RVA: 0x0016574B File Offset: 0x0016394B
		public CommandBuffer command { get; set; }

		// Token: 0x170004F6 RID: 1270
		// (get) Token: 0x06003D0B RID: 15627 RVA: 0x00165754 File Offset: 0x00163954
		// (set) Token: 0x06003D0C RID: 15628 RVA: 0x0016575C File Offset: 0x0016395C
		public RenderTargetIdentifier source { get; set; }

		// Token: 0x170004F7 RID: 1271
		// (get) Token: 0x06003D0D RID: 15629 RVA: 0x00165765 File Offset: 0x00163965
		// (set) Token: 0x06003D0E RID: 15630 RVA: 0x0016576D File Offset: 0x0016396D
		public RenderTargetIdentifier destination { get; set; }

		// Token: 0x170004F8 RID: 1272
		// (get) Token: 0x06003D0F RID: 15631 RVA: 0x00165776 File Offset: 0x00163976
		// (set) Token: 0x06003D10 RID: 15632 RVA: 0x0016577E File Offset: 0x0016397E
		public RenderTextureFormat sourceFormat { get; set; }

		// Token: 0x170004F9 RID: 1273
		// (get) Token: 0x06003D11 RID: 15633 RVA: 0x00165787 File Offset: 0x00163987
		// (set) Token: 0x06003D12 RID: 15634 RVA: 0x0016578F File Offset: 0x0016398F
		public bool flip { get; set; }

		// Token: 0x170004FA RID: 1274
		// (get) Token: 0x06003D13 RID: 15635 RVA: 0x00165798 File Offset: 0x00163998
		// (set) Token: 0x06003D14 RID: 15636 RVA: 0x001657A0 File Offset: 0x001639A0
		public PostProcessResources resources { get; internal set; }

		// Token: 0x170004FB RID: 1275
		// (get) Token: 0x06003D15 RID: 15637 RVA: 0x001657A9 File Offset: 0x001639A9
		// (set) Token: 0x06003D16 RID: 15638 RVA: 0x001657B1 File Offset: 0x001639B1
		public PropertySheetFactory propertySheets { get; internal set; }

		// Token: 0x170004FC RID: 1276
		// (get) Token: 0x06003D17 RID: 15639 RVA: 0x001657BA File Offset: 0x001639BA
		// (set) Token: 0x06003D18 RID: 15640 RVA: 0x001657C2 File Offset: 0x001639C2
		public Dictionary<string, object> userData { get; private set; }

		// Token: 0x170004FD RID: 1277
		// (get) Token: 0x06003D19 RID: 15641 RVA: 0x001657CB File Offset: 0x001639CB
		// (set) Token: 0x06003D1A RID: 15642 RVA: 0x001657D3 File Offset: 0x001639D3
		public PostProcessDebugLayer debugLayer { get; internal set; }

		// Token: 0x170004FE RID: 1278
		// (get) Token: 0x06003D1B RID: 15643 RVA: 0x001657DC File Offset: 0x001639DC
		// (set) Token: 0x06003D1C RID: 15644 RVA: 0x001657E4 File Offset: 0x001639E4
		public int width { get; set; }

		// Token: 0x170004FF RID: 1279
		// (get) Token: 0x06003D1D RID: 15645 RVA: 0x001657ED File Offset: 0x001639ED
		// (set) Token: 0x06003D1E RID: 15646 RVA: 0x001657F5 File Offset: 0x001639F5
		public int height { get; set; }

		// Token: 0x17000500 RID: 1280
		// (get) Token: 0x06003D1F RID: 15647 RVA: 0x001657FE File Offset: 0x001639FE
		// (set) Token: 0x06003D20 RID: 15648 RVA: 0x00165806 File Offset: 0x00163A06
		public bool stereoActive { get; private set; }

		// Token: 0x17000501 RID: 1281
		// (get) Token: 0x06003D21 RID: 15649 RVA: 0x0016580F File Offset: 0x00163A0F
		// (set) Token: 0x06003D22 RID: 15650 RVA: 0x00165817 File Offset: 0x00163A17
		public int xrActiveEye { get; private set; }

		// Token: 0x17000502 RID: 1282
		// (get) Token: 0x06003D23 RID: 15651 RVA: 0x00165820 File Offset: 0x00163A20
		// (set) Token: 0x06003D24 RID: 15652 RVA: 0x00165828 File Offset: 0x00163A28
		public int numberOfEyes { get; private set; }

		// Token: 0x17000503 RID: 1283
		// (get) Token: 0x06003D25 RID: 15653 RVA: 0x00165831 File Offset: 0x00163A31
		// (set) Token: 0x06003D26 RID: 15654 RVA: 0x00165839 File Offset: 0x00163A39
		public PostProcessRenderContext.StereoRenderingMode stereoRenderingMode { get; private set; }

		// Token: 0x17000504 RID: 1284
		// (get) Token: 0x06003D27 RID: 15655 RVA: 0x00165842 File Offset: 0x00163A42
		// (set) Token: 0x06003D28 RID: 15656 RVA: 0x0016584A File Offset: 0x00163A4A
		public int screenWidth { get; set; }

		// Token: 0x17000505 RID: 1285
		// (get) Token: 0x06003D29 RID: 15657 RVA: 0x00165853 File Offset: 0x00163A53
		// (set) Token: 0x06003D2A RID: 15658 RVA: 0x0016585B File Offset: 0x00163A5B
		public int screenHeight { get; set; }

		// Token: 0x17000506 RID: 1286
		// (get) Token: 0x06003D2B RID: 15659 RVA: 0x00165864 File Offset: 0x00163A64
		// (set) Token: 0x06003D2C RID: 15660 RVA: 0x0016586C File Offset: 0x00163A6C
		public bool isSceneView { get; internal set; }

		// Token: 0x17000507 RID: 1287
		// (get) Token: 0x06003D2D RID: 15661 RVA: 0x00165875 File Offset: 0x00163A75
		// (set) Token: 0x06003D2E RID: 15662 RVA: 0x0016587D File Offset: 0x00163A7D
		public PostProcessLayer.Antialiasing antialiasing { get; internal set; }

		// Token: 0x17000508 RID: 1288
		// (get) Token: 0x06003D2F RID: 15663 RVA: 0x00165886 File Offset: 0x00163A86
		// (set) Token: 0x06003D30 RID: 15664 RVA: 0x0016588E File Offset: 0x00163A8E
		public TemporalAntialiasing temporalAntialiasing { get; internal set; }

		// Token: 0x06003D31 RID: 15665 RVA: 0x00165898 File Offset: 0x00163A98
		public void Reset()
		{
			this.m_Camera = null;
			this.width = 0;
			this.height = 0;
			this.dlssEnabled = false;
			this.m_sourceDescriptor = new RenderTextureDescriptor(0, 0);
			this.physicalCamera = false;
			this.stereoActive = false;
			this.xrActiveEye = 0;
			this.screenWidth = 0;
			this.screenHeight = 0;
			this.command = null;
			this.source = 0;
			this.destination = 0;
			this.sourceFormat = RenderTextureFormat.ARGB32;
			this.flip = false;
			this.resources = null;
			this.propertySheets = null;
			this.debugLayer = null;
			this.isSceneView = false;
			this.antialiasing = PostProcessLayer.Antialiasing.None;
			this.temporalAntialiasing = null;
			this.uberSheet = null;
			this.autoExposureTexture = null;
			this.logLut = null;
			this.autoExposure = null;
			this.bloomBufferNameID = -1;
			if (this.userData == null)
			{
				this.userData = new Dictionary<string, object>();
			}
			this.userData.Clear();
		}

		// Token: 0x06003D32 RID: 15666 RVA: 0x00165989 File Offset: 0x00163B89
		public bool IsTemporalAntialiasingActive()
		{
			return this.antialiasing == PostProcessLayer.Antialiasing.TemporalAntialiasing && !this.isSceneView && this.temporalAntialiasing.IsSupported();
		}

		// Token: 0x06003D33 RID: 15667 RVA: 0x001659A9 File Offset: 0x00163BA9
		public bool IsDebugOverlayEnabled(DebugOverlay overlay)
		{
			return this.debugLayer.debugOverlay == overlay;
		}

		// Token: 0x06003D34 RID: 15668 RVA: 0x001659B9 File Offset: 0x00163BB9
		public void PushDebugOverlay(CommandBuffer cmd, RenderTargetIdentifier source, PropertySheet sheet, int pass)
		{
			this.debugLayer.PushDebugOverlay(cmd, source, sheet, pass);
		}

		// Token: 0x06003D35 RID: 15669 RVA: 0x001659CC File Offset: 0x00163BCC
		private RenderTextureDescriptor GetDescriptor(int depthBufferBits = 0, RenderTextureFormat colorFormat = RenderTextureFormat.Default, RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default)
		{
			RenderTextureDescriptor result = new RenderTextureDescriptor(this.m_sourceDescriptor.width, this.m_sourceDescriptor.height, this.m_sourceDescriptor.colorFormat, depthBufferBits);
			result.dimension = this.m_sourceDescriptor.dimension;
			result.volumeDepth = this.m_sourceDescriptor.volumeDepth;
			result.vrUsage = this.m_sourceDescriptor.vrUsage;
			result.msaaSamples = this.m_sourceDescriptor.msaaSamples;
			result.memoryless = this.m_sourceDescriptor.memoryless;
			result.useMipMap = this.m_sourceDescriptor.useMipMap;
			result.autoGenerateMips = this.m_sourceDescriptor.autoGenerateMips;
			result.enableRandomWrite = this.m_sourceDescriptor.enableRandomWrite;
			result.shadowSamplingMode = this.m_sourceDescriptor.shadowSamplingMode;
			if (colorFormat != RenderTextureFormat.Default)
			{
				result.colorFormat = colorFormat;
			}
			if (readWrite == RenderTextureReadWrite.sRGB)
			{
				result.sRGB = true;
			}
			else if (readWrite == RenderTextureReadWrite.Linear)
			{
				result.sRGB = false;
			}
			else if (readWrite == RenderTextureReadWrite.Default)
			{
				result.sRGB = (QualitySettings.activeColorSpace > ColorSpace.Gamma);
			}
			return result;
		}

		// Token: 0x06003D36 RID: 15670 RVA: 0x00165AE0 File Offset: 0x00163CE0
		public void GetScreenSpaceTemporaryRT(CommandBuffer cmd, int nameID, int depthBufferBits = 0, RenderTextureFormat colorFormat = RenderTextureFormat.Default, RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default, FilterMode filter = FilterMode.Bilinear, int widthOverride = 0, int heightOverride = 0)
		{
			RenderTextureDescriptor descriptor = this.GetDescriptor(depthBufferBits, colorFormat, readWrite);
			if (widthOverride > 0)
			{
				descriptor.width = widthOverride;
			}
			if (heightOverride > 0)
			{
				descriptor.height = heightOverride;
			}
			if (this.stereoActive && descriptor.dimension == TextureDimension.Tex2DArray)
			{
				descriptor.dimension = TextureDimension.Tex2D;
			}
			cmd.GetTemporaryRT(nameID, descriptor, filter);
		}

		// Token: 0x06003D37 RID: 15671 RVA: 0x00165B3C File Offset: 0x00163D3C
		public RenderTexture GetScreenSpaceTemporaryRT(int depthBufferBits = 0, RenderTextureFormat colorFormat = RenderTextureFormat.Default, RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default, int widthOverride = 0, int heightOverride = 0)
		{
			RenderTextureDescriptor descriptor = this.GetDescriptor(depthBufferBits, colorFormat, readWrite);
			if (widthOverride > 0)
			{
				descriptor.width = widthOverride;
			}
			if (heightOverride > 0)
			{
				descriptor.height = heightOverride;
			}
			return RenderTexture.GetTemporary(descriptor);
		}

		// Token: 0x04003623 RID: 13859
		public bool dlssEnabled;

		// Token: 0x04003624 RID: 13860
		private Camera m_Camera;

		// Token: 0x04003639 RID: 13881
		internal PropertySheet uberSheet;

		// Token: 0x0400363A RID: 13882
		internal Texture autoExposureTexture;

		// Token: 0x0400363B RID: 13883
		internal LogHistogram logHistogram;

		// Token: 0x0400363C RID: 13884
		internal Texture logLut;

		// Token: 0x0400363D RID: 13885
		internal AutoExposure autoExposure;

		// Token: 0x0400363E RID: 13886
		internal int bloomBufferNameID;

		// Token: 0x0400363F RID: 13887
		internal bool physicalCamera;

		// Token: 0x04003640 RID: 13888
		private RenderTextureDescriptor m_sourceDescriptor;

		// Token: 0x02000EBC RID: 3772
		public enum StereoRenderingMode
		{
			// Token: 0x04004BBE RID: 19390
			MultiPass,
			// Token: 0x04004BBF RID: 19391
			SinglePass,
			// Token: 0x04004BC0 RID: 19392
			SinglePassInstanced,
			// Token: 0x04004BC1 RID: 19393
			SinglePassMultiview
		}
	}
}
