using System;
using UnityEngine;

// Token: 0x020001AB RID: 427
public class AlignedLineDrawer : MonoBehaviour, IClientComponent
{
	// Token: 0x040010E5 RID: 4325
	public MeshFilter Filter;

	// Token: 0x040010E6 RID: 4326
	public MeshRenderer Renderer;

	// Token: 0x040010E7 RID: 4327
	public float LineWidth = 1f;

	// Token: 0x040010E8 RID: 4328
	public float SurfaceOffset = 0.001f;

	// Token: 0x040010E9 RID: 4329
	public float SprayThickness = 0.4f;

	// Token: 0x040010EA RID: 4330
	public float uvTilingFactor = 1f;

	// Token: 0x040010EB RID: 4331
	public bool DrawEndCaps;

	// Token: 0x040010EC RID: 4332
	public bool DrawSideMesh;

	// Token: 0x040010ED RID: 4333
	public bool DrawBackMesh;

	// Token: 0x040010EE RID: 4334
	public SprayCanSpray_Freehand Spray;

	// Token: 0x02000BEB RID: 3051
	[Serializable]
	public struct LinePoint
	{
		// Token: 0x04004030 RID: 16432
		public Vector3 LocalPosition;

		// Token: 0x04004031 RID: 16433
		public Vector3 WorldNormal;
	}
}
