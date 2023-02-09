using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A09 RID: 2569
	[Preserve]
	internal sealed class AmbientOcclusionRenderer : PostProcessEffectRenderer<AmbientOcclusion>
	{
		// Token: 0x06003D49 RID: 15689 RVA: 0x00165E79 File Offset: 0x00164079
		public override void Init()
		{
			if (this.m_Methods == null)
			{
				this.m_Methods = new IAmbientOcclusionMethod[]
				{
					new ScalableAO(base.settings),
					new MultiScaleVO(base.settings)
				};
			}
		}

		// Token: 0x06003D4A RID: 15690 RVA: 0x00165EAC File Offset: 0x001640AC
		public bool IsAmbientOnly(PostProcessRenderContext context)
		{
			Camera camera = context.camera;
			return base.settings.ambientOnly.value && camera.actualRenderingPath == RenderingPath.DeferredShading && camera.allowHDR;
		}

		// Token: 0x06003D4B RID: 15691 RVA: 0x00165EE3 File Offset: 0x001640E3
		public IAmbientOcclusionMethod Get()
		{
			return this.m_Methods[(int)base.settings.mode.value];
		}

		// Token: 0x06003D4C RID: 15692 RVA: 0x00165EFC File Offset: 0x001640FC
		public override DepthTextureMode GetCameraFlags()
		{
			return this.Get().GetCameraFlags();
		}

		// Token: 0x06003D4D RID: 15693 RVA: 0x00165F0C File Offset: 0x0016410C
		public override void Release()
		{
			IAmbientOcclusionMethod[] methods = this.m_Methods;
			for (int i = 0; i < methods.Length; i++)
			{
				methods[i].Release();
			}
		}

		// Token: 0x06003D4E RID: 15694 RVA: 0x00165F36 File Offset: 0x00164136
		public ScalableAO GetScalableAO()
		{
			return (ScalableAO)this.m_Methods[0];
		}

		// Token: 0x06003D4F RID: 15695 RVA: 0x00165F45 File Offset: 0x00164145
		public MultiScaleVO GetMultiScaleVO()
		{
			return (MultiScaleVO)this.m_Methods[1];
		}

		// Token: 0x06003D50 RID: 15696 RVA: 0x000059DD File Offset: 0x00003BDD
		public override void Render(PostProcessRenderContext context)
		{
		}

		// Token: 0x04003660 RID: 13920
		private IAmbientOcclusionMethod[] m_Methods;
	}
}
