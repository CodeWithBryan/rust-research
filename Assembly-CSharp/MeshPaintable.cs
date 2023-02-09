using System;
using UnityEngine;

// Token: 0x020002B2 RID: 690
public class MeshPaintable : BaseMeshPaintable
{
	// Token: 0x040015BC RID: 5564
	public string replacementTextureName = "_MainTex";

	// Token: 0x040015BD RID: 5565
	public int textureWidth = 256;

	// Token: 0x040015BE RID: 5566
	public int textureHeight = 256;

	// Token: 0x040015BF RID: 5567
	public Color clearColor = Color.clear;

	// Token: 0x040015C0 RID: 5568
	public Texture2D targetTexture;

	// Token: 0x040015C1 RID: 5569
	public bool hasChanges;
}
