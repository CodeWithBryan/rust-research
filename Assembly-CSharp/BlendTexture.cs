using System;
using UnityEngine;

// Token: 0x020008F7 RID: 2295
public class BlendTexture : ProcessedTexture
{
	// Token: 0x060036B0 RID: 14000 RVA: 0x0014548E File Offset: 0x0014368E
	public BlendTexture(int width, int height, bool linear = true)
	{
		this.material = base.CreateMaterial("Hidden/BlitCopyAlpha");
		this.result = base.CreateRenderTexture("Blend Texture", width, height, linear);
	}

	// Token: 0x060036B1 RID: 14001 RVA: 0x001454BB File Offset: 0x001436BB
	public void Blend(Texture source, Texture target, float alpha)
	{
		this.material.SetTexture("_BlendTex", target);
		this.material.SetFloat("_Alpha", Mathf.Clamp01(alpha));
		Graphics.Blit(source, this.result, this.material);
	}

	// Token: 0x060036B2 RID: 14002 RVA: 0x001454F6 File Offset: 0x001436F6
	public void CopyTo(BlendTexture target)
	{
		Graphics.Blit(this.result, target.result);
	}
}
