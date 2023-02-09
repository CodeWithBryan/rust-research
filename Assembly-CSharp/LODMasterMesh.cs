using System;
using UnityEngine;

// Token: 0x0200050E RID: 1294
public class LODMasterMesh : LODComponent
{
	// Token: 0x040020AF RID: 8367
	public MeshRenderer ReplacementMesh;

	// Token: 0x040020B0 RID: 8368
	public float Distance = 100f;

	// Token: 0x040020B1 RID: 8369
	public LODComponent[] ChildComponents;

	// Token: 0x040020B2 RID: 8370
	public bool Block;

	// Token: 0x040020B3 RID: 8371
	public Bounds MeshBounds;
}
