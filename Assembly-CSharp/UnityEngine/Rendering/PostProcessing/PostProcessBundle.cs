using System;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A45 RID: 2629
	public sealed class PostProcessBundle
	{
		// Token: 0x1700050E RID: 1294
		// (get) Token: 0x06003E1E RID: 15902 RVA: 0x0016D34E File Offset: 0x0016B54E
		// (set) Token: 0x06003E1F RID: 15903 RVA: 0x0016D356 File Offset: 0x0016B556
		public PostProcessAttribute attribute { get; private set; }

		// Token: 0x1700050F RID: 1295
		// (get) Token: 0x06003E20 RID: 15904 RVA: 0x0016D35F File Offset: 0x0016B55F
		// (set) Token: 0x06003E21 RID: 15905 RVA: 0x0016D367 File Offset: 0x0016B567
		public PostProcessEffectSettings settings { get; private set; }

		// Token: 0x17000510 RID: 1296
		// (get) Token: 0x06003E22 RID: 15906 RVA: 0x0016D370 File Offset: 0x0016B570
		internal PostProcessEffectRenderer renderer
		{
			get
			{
				if (this.m_Renderer == null)
				{
					Assert.IsNotNull<Type>(this.attribute.renderer);
					Type renderer = this.attribute.renderer;
					this.m_Renderer = (PostProcessEffectRenderer)Activator.CreateInstance(renderer);
					this.m_Renderer.SetSettings(this.settings);
					this.m_Renderer.Init();
				}
				return this.m_Renderer;
			}
		}

		// Token: 0x06003E23 RID: 15907 RVA: 0x0016D3D4 File Offset: 0x0016B5D4
		internal PostProcessBundle(PostProcessEffectSettings settings)
		{
			Assert.IsNotNull<PostProcessEffectSettings>(settings);
			this.settings = settings;
			this.attribute = settings.GetType().GetAttribute<PostProcessAttribute>();
		}

		// Token: 0x06003E24 RID: 15908 RVA: 0x0016D3FA File Offset: 0x0016B5FA
		internal void Release()
		{
			if (this.m_Renderer != null)
			{
				this.m_Renderer.Release();
			}
			RuntimeUtilities.Destroy(this.settings);
		}

		// Token: 0x06003E25 RID: 15909 RVA: 0x0016D41A File Offset: 0x0016B61A
		internal void ResetHistory()
		{
			if (this.m_Renderer != null)
			{
				this.m_Renderer.ResetHistory();
			}
		}

		// Token: 0x06003E26 RID: 15910 RVA: 0x0016D42F File Offset: 0x0016B62F
		internal T CastSettings<T>() where T : PostProcessEffectSettings
		{
			return (T)((object)this.settings);
		}

		// Token: 0x06003E27 RID: 15911 RVA: 0x0016D43C File Offset: 0x0016B63C
		internal T CastRenderer<T>() where T : PostProcessEffectRenderer
		{
			return (T)((object)this.renderer);
		}

		// Token: 0x04003743 RID: 14147
		private PostProcessEffectRenderer m_Renderer;
	}
}
