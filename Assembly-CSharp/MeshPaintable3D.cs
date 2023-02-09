using System;
using UnityEngine;

// Token: 0x020002B3 RID: 691
public class MeshPaintable3D : BaseMeshPaintable
{
	// Token: 0x040015C2 RID: 5570
	[ClientVar]
	public static float brushScale = 2f;

	// Token: 0x040015C3 RID: 5571
	[ClientVar]
	public static float uvBufferScale = 2f;

	// Token: 0x040015C4 RID: 5572
	public string replacementTextureName = "_MainTex";

	// Token: 0x040015C5 RID: 5573
	public int textureWidth = 256;

	// Token: 0x040015C6 RID: 5574
	public int textureHeight = 256;

	// Token: 0x040015C7 RID: 5575
	public Camera cameraPreview;

	// Token: 0x040015C8 RID: 5576
	public Camera camera3D;
}
