using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200087B RID: 2171
public class UIInvertedMaskImage : Image
{
	// Token: 0x17000404 RID: 1028
	// (get) Token: 0x06003565 RID: 13669 RVA: 0x0014188E File Offset: 0x0013FA8E
	public override Material materialForRendering
	{
		get
		{
			if (this.cachedMaterial == null)
			{
				this.cachedMaterial = UnityEngine.Object.Instantiate<Material>(base.materialForRendering);
				this.cachedMaterial.SetInt("_StencilComp", 6);
			}
			return this.cachedMaterial;
		}
	}

	// Token: 0x04003031 RID: 12337
	private Material cachedMaterial;
}
