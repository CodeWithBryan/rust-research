using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	// Token: 0x020009D5 RID: 2517
	[ExecuteInEditMode]
	[AddComponentMenu("Image Effects/Other/Scope Overlay")]
	public class ScopeEffect : PostEffectsBase, IImageEffect
	{
		// Token: 0x06003B65 RID: 15205 RVA: 0x00003A54 File Offset: 0x00001C54
		public override bool CheckResources()
		{
			return true;
		}

		// Token: 0x06003B66 RID: 15206 RVA: 0x0015B78E File Offset: 0x0015998E
		public bool IsActive()
		{
			return base.enabled && this.CheckResources();
		}

		// Token: 0x06003B67 RID: 15207 RVA: 0x0015B7A0 File Offset: 0x001599A0
		public void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			this.overlayMaterial.SetVector("_Screen", new Vector2((float)Screen.width, (float)Screen.height));
			Graphics.Blit(source, destination, this.overlayMaterial);
		}

		// Token: 0x04003542 RID: 13634
		public Material overlayMaterial;
	}
}
