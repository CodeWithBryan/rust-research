using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A36 RID: 2614
	public abstract class Monitor
	{
		// Token: 0x1700050D RID: 1293
		// (get) Token: 0x06003DE0 RID: 15840 RVA: 0x0016C7C6 File Offset: 0x0016A9C6
		// (set) Token: 0x06003DE1 RID: 15841 RVA: 0x0016C7CE File Offset: 0x0016A9CE
		public RenderTexture output { get; protected set; }

		// Token: 0x06003DE2 RID: 15842 RVA: 0x0016C7D7 File Offset: 0x0016A9D7
		public bool IsRequestedAndSupported(PostProcessRenderContext context)
		{
			return this.requested && SystemInfo.supportsComputeShaders && !RuntimeUtilities.isAndroidOpenGL && this.ShaderResourcesAvailable(context);
		}

		// Token: 0x06003DE3 RID: 15843
		internal abstract bool ShaderResourcesAvailable(PostProcessRenderContext context);

		// Token: 0x06003DE4 RID: 15844 RVA: 0x00007074 File Offset: 0x00005274
		internal virtual bool NeedsHalfRes()
		{
			return false;
		}

		// Token: 0x06003DE5 RID: 15845 RVA: 0x0016C7F8 File Offset: 0x0016A9F8
		protected void CheckOutput(int width, int height)
		{
			if (this.output == null || !this.output.IsCreated() || this.output.width != width || this.output.height != height)
			{
				RuntimeUtilities.Destroy(this.output);
				this.output = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32)
				{
					anisoLevel = 0,
					filterMode = FilterMode.Bilinear,
					wrapMode = TextureWrapMode.Clamp,
					useMipMap = false
				};
			}
		}

		// Token: 0x06003DE6 RID: 15846 RVA: 0x000059DD File Offset: 0x00003BDD
		internal virtual void OnEnable()
		{
		}

		// Token: 0x06003DE7 RID: 15847 RVA: 0x0016C872 File Offset: 0x0016AA72
		internal virtual void OnDisable()
		{
			RuntimeUtilities.Destroy(this.output);
		}

		// Token: 0x06003DE8 RID: 15848
		internal abstract void Render(PostProcessRenderContext context);

		// Token: 0x0400372C RID: 14124
		internal bool requested;
	}
}
