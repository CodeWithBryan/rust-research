using System;
using UnityEngine;

// Token: 0x020008F8 RID: 2296
public class BlurTexture : ProcessedTexture
{
	// Token: 0x060036B3 RID: 14003 RVA: 0x00145509 File Offset: 0x00143709
	public BlurTexture(int width, int height, bool linear = true)
	{
		this.material = base.CreateMaterial("Hidden/Rust/SeparableBlur");
		this.result = base.CreateRenderTexture("Blur Texture", width, height, linear);
	}

	// Token: 0x060036B4 RID: 14004 RVA: 0x00145536 File Offset: 0x00143736
	public void Blur(float radius)
	{
		this.Blur(this.result, radius);
	}

	// Token: 0x060036B5 RID: 14005 RVA: 0x00145548 File Offset: 0x00143748
	public void Blur(Texture source, float radius)
	{
		RenderTexture renderTexture = base.CreateTemporary();
		this.material.SetVector("offsets", new Vector4(radius / (float)Screen.width, 0f, 0f, 0f));
		Graphics.Blit(source, renderTexture, this.material, 0);
		this.material.SetVector("offsets", new Vector4(0f, radius / (float)Screen.height, 0f, 0f));
		Graphics.Blit(renderTexture, this.result, this.material, 0);
		base.ReleaseTemporary(renderTexture);
	}
}
