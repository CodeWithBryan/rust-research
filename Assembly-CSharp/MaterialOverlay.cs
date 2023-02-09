using System;
using UnityEngine;

// Token: 0x02000592 RID: 1426
[ExecuteInEditMode]
public class MaterialOverlay : MonoBehaviour
{
	// Token: 0x06002A8E RID: 10894 RVA: 0x0010182C File Offset: 0x000FFA2C
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.material)
		{
			Graphics.Blit(source, destination);
			return;
		}
		for (int i = 0; i < this.material.passCount; i++)
		{
			Graphics.Blit(source, destination, this.material, i);
		}
	}

	// Token: 0x04002281 RID: 8833
	public Material material;
}
