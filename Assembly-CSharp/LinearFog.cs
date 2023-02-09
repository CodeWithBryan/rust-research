using System;
using UnityEngine;

// Token: 0x02000591 RID: 1425
[ExecuteInEditMode]
public class LinearFog : MonoBehaviour
{
	// Token: 0x06002A8C RID: 10892 RVA: 0x00101730 File Offset: 0x000FF930
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.fogMaterial)
		{
			Graphics.Blit(source, destination);
			return;
		}
		this.fogMaterial.SetColor("_FogColor", this.fogColor);
		this.fogMaterial.SetFloat("_Start", this.fogStart);
		this.fogMaterial.SetFloat("_Range", this.fogRange);
		this.fogMaterial.SetFloat("_Density", this.fogDensity);
		if (this.fogSky)
		{
			this.fogMaterial.SetFloat("_CutOff", 2f);
		}
		else
		{
			this.fogMaterial.SetFloat("_CutOff", 1f);
		}
		for (int i = 0; i < this.fogMaterial.passCount; i++)
		{
			Graphics.Blit(source, destination, this.fogMaterial, i);
		}
	}

	// Token: 0x0400227B RID: 8827
	public Material fogMaterial;

	// Token: 0x0400227C RID: 8828
	public Color fogColor = Color.white;

	// Token: 0x0400227D RID: 8829
	public float fogStart;

	// Token: 0x0400227E RID: 8830
	public float fogRange = 1f;

	// Token: 0x0400227F RID: 8831
	public float fogDensity = 1f;

	// Token: 0x04002280 RID: 8832
	public bool fogSky;
}
