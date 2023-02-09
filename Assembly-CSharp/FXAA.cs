using System;
using UnityEngine;

// Token: 0x02000958 RID: 2392
[AddComponentMenu("Image Effects/FXAA")]
public class FXAA : FXAAPostEffectsBase, IImageEffect
{
	// Token: 0x06003870 RID: 14448 RVA: 0x0014D0E3 File Offset: 0x0014B2E3
	private void CreateMaterials()
	{
		if (this.mat == null)
		{
			this.mat = base.CheckShaderAndCreateMaterial(this.shader, this.mat);
		}
	}

	// Token: 0x06003871 RID: 14449 RVA: 0x0014D10B File Offset: 0x0014B30B
	private void Start()
	{
		this.CreateMaterials();
		base.CheckSupport(false);
	}

	// Token: 0x06003872 RID: 14450 RVA: 0x0014D11B File Offset: 0x0014B31B
	public bool IsActive()
	{
		return base.enabled;
	}

	// Token: 0x06003873 RID: 14451 RVA: 0x0014D124 File Offset: 0x0014B324
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.CreateMaterials();
		float num = 1f / (float)Screen.width;
		float num2 = 1f / (float)Screen.height;
		this.mat.SetVector("_rcpFrame", new Vector4(num, num2, 0f, 0f));
		this.mat.SetVector("_rcpFrameOpt", new Vector4(num * 2f, num2 * 2f, num * 0.5f, num2 * 0.5f));
		Graphics.Blit(source, destination, this.mat);
	}

	// Token: 0x040032C5 RID: 12997
	public Shader shader;

	// Token: 0x040032C6 RID: 12998
	private Material mat;
}
