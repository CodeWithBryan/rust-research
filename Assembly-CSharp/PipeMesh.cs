using System;
using UnityEngine;

// Token: 0x020001A9 RID: 425
public class PipeMesh : MonoBehaviour
{
	// Token: 0x040010D5 RID: 4309
	public bool UseJobs;

	// Token: 0x040010D6 RID: 4310
	public float PipeRadius = 0.04f;

	// Token: 0x040010D7 RID: 4311
	public Material PipeMaterial;

	// Token: 0x040010D8 RID: 4312
	public float StraightLength = 0.3f;

	// Token: 0x040010D9 RID: 4313
	public int PipeSubdivisions = 8;

	// Token: 0x040010DA RID: 4314
	public int BendTesselation = 6;

	// Token: 0x040010DB RID: 4315
	public float RidgeHeight = 0.05f;

	// Token: 0x040010DC RID: 4316
	public float UvScaleMultiplier = 2f;

	// Token: 0x040010DD RID: 4317
	public float RidgeIncrements = 0.5f;

	// Token: 0x040010DE RID: 4318
	public float RidgeLength = 0.05f;

	// Token: 0x040010DF RID: 4319
	public Vector2 HorizontalUvRange = new Vector2(0f, 0.2f);
}
