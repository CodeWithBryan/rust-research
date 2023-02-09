using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A4A RID: 2634
	public abstract class PostProcessEffectRenderer
	{
		// Token: 0x06003E43 RID: 15939 RVA: 0x000059DD File Offset: 0x00003BDD
		public virtual void Init()
		{
		}

		// Token: 0x06003E44 RID: 15940 RVA: 0x00007074 File Offset: 0x00005274
		public virtual DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.None;
		}

		// Token: 0x06003E45 RID: 15941 RVA: 0x0016DD90 File Offset: 0x0016BF90
		public virtual void ResetHistory()
		{
			this.m_ResetHistory = true;
		}

		// Token: 0x06003E46 RID: 15942 RVA: 0x0016DD99 File Offset: 0x0016BF99
		public virtual void Release()
		{
			this.ResetHistory();
		}

		// Token: 0x06003E47 RID: 15943
		public abstract void Render(PostProcessRenderContext context);

		// Token: 0x06003E48 RID: 15944
		internal abstract void SetSettings(PostProcessEffectSettings settings);

		// Token: 0x04003768 RID: 14184
		protected bool m_ResetHistory = true;
	}
}
