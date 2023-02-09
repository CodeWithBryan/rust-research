using System;
using UnityEngine;

// Token: 0x020002BD RID: 701
public class PlayerModelSkin : MonoBehaviour, IPrefabPreProcess
{
	// Token: 0x06001C81 RID: 7297 RVA: 0x000C400C File Offset: 0x000C220C
	public void Setup(SkinSetCollection skin, float hairNum, float meshNum)
	{
		if (!this.SkinRenderer)
		{
			return;
		}
		if (!skin)
		{
			return;
		}
		switch (this.MaterialType)
		{
		case PlayerModelSkin.SkinMaterialType.HEAD:
			this.SkinRenderer.sharedMaterial = skin.Get(meshNum).HeadMaterial;
			return;
		case PlayerModelSkin.SkinMaterialType.EYE:
			this.SkinRenderer.sharedMaterial = skin.Get(meshNum).EyeMaterial;
			return;
		case PlayerModelSkin.SkinMaterialType.BODY:
			this.SkinRenderer.sharedMaterial = skin.Get(meshNum).BodyMaterial;
			return;
		default:
			this.SkinRenderer.sharedMaterial = skin.Get(meshNum).BodyMaterial;
			return;
		}
	}

	// Token: 0x06001C82 RID: 7298 RVA: 0x000C40AA File Offset: 0x000C22AA
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (!clientside)
		{
			return;
		}
		this.SkinRenderer = base.GetComponent<Renderer>();
	}

	// Token: 0x040015F5 RID: 5621
	public PlayerModelSkin.SkinMaterialType MaterialType;

	// Token: 0x040015F6 RID: 5622
	public Renderer SkinRenderer;

	// Token: 0x02000C46 RID: 3142
	public enum SkinMaterialType
	{
		// Token: 0x04004199 RID: 16793
		HEAD,
		// Token: 0x0400419A RID: 16794
		EYE,
		// Token: 0x0400419B RID: 16795
		BODY
	}
}
