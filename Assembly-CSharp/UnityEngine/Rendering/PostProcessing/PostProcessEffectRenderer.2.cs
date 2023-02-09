using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A4B RID: 2635
	public abstract class PostProcessEffectRenderer<T> : PostProcessEffectRenderer where T : PostProcessEffectSettings
	{
		// Token: 0x17000514 RID: 1300
		// (get) Token: 0x06003E4A RID: 15946 RVA: 0x0016DDB0 File Offset: 0x0016BFB0
		// (set) Token: 0x06003E4B RID: 15947 RVA: 0x0016DDB8 File Offset: 0x0016BFB8
		public T settings { get; internal set; }

		// Token: 0x06003E4C RID: 15948 RVA: 0x0016DDC1 File Offset: 0x0016BFC1
		internal override void SetSettings(PostProcessEffectSettings settings)
		{
			this.settings = (T)((object)settings);
		}
	}
}
