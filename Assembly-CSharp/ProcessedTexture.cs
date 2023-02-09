using System;
using UnityEngine;

// Token: 0x020008FC RID: 2300
public class ProcessedTexture
{
	// Token: 0x060036C5 RID: 14021 RVA: 0x00146224 File Offset: 0x00144424
	public void Dispose()
	{
		this.DestroyRenderTexture(ref this.result);
		this.DestroyMaterial(ref this.material);
	}

	// Token: 0x060036C6 RID: 14022 RVA: 0x0014623E File Offset: 0x0014443E
	protected RenderTexture CreateRenderTexture(string name, int width, int height, bool linear)
	{
		RenderTexture renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, linear ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB);
		renderTexture.hideFlags = HideFlags.DontSave;
		renderTexture.name = name;
		renderTexture.filterMode = FilterMode.Bilinear;
		renderTexture.anisoLevel = 0;
		renderTexture.Create();
		return renderTexture;
	}

	// Token: 0x060036C7 RID: 14023 RVA: 0x00146275 File Offset: 0x00144475
	protected void DestroyRenderTexture(ref RenderTexture rt)
	{
		if (rt == null)
		{
			return;
		}
		UnityEngine.Object.Destroy(rt);
		rt = null;
	}

	// Token: 0x060036C8 RID: 14024 RVA: 0x0014628C File Offset: 0x0014448C
	protected RenderTexture CreateTemporary()
	{
		return RenderTexture.GetTemporary(this.result.width, this.result.height, this.result.depth, this.result.format, this.result.sRGB ? RenderTextureReadWrite.sRGB : RenderTextureReadWrite.Linear);
	}

	// Token: 0x060036C9 RID: 14025 RVA: 0x001462DB File Offset: 0x001444DB
	protected void ReleaseTemporary(RenderTexture rt)
	{
		RenderTexture.ReleaseTemporary(rt);
	}

	// Token: 0x060036CA RID: 14026 RVA: 0x001462E3 File Offset: 0x001444E3
	protected Material CreateMaterial(string shader)
	{
		return this.CreateMaterial(Shader.Find(shader));
	}

	// Token: 0x060036CB RID: 14027 RVA: 0x001462F1 File Offset: 0x001444F1
	protected Material CreateMaterial(Shader shader)
	{
		return new Material(shader)
		{
			hideFlags = HideFlags.DontSave
		};
	}

	// Token: 0x060036CC RID: 14028 RVA: 0x00146275 File Offset: 0x00144475
	protected void DestroyMaterial(ref Material mat)
	{
		if (mat == null)
		{
			return;
		}
		UnityEngine.Object.Destroy(mat);
		mat = null;
	}

	// Token: 0x060036CD RID: 14029 RVA: 0x00146301 File Offset: 0x00144501
	public static implicit operator Texture(ProcessedTexture t)
	{
		return t.result;
	}

	// Token: 0x04003193 RID: 12691
	protected RenderTexture result;

	// Token: 0x04003194 RID: 12692
	protected Material material;
}
