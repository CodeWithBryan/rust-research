using System;
using UnityEngine;

// Token: 0x02000639 RID: 1593
public class DecorSpawn : MonoBehaviour, IClientComponent
{
	// Token: 0x04002563 RID: 9571
	public SpawnFilter Filter;

	// Token: 0x04002564 RID: 9572
	public string ResourceFolder = string.Empty;

	// Token: 0x04002565 RID: 9573
	public uint Seed;

	// Token: 0x04002566 RID: 9574
	public float ObjectCutoff = 0.2f;

	// Token: 0x04002567 RID: 9575
	public float ObjectTapering = 0.2f;

	// Token: 0x04002568 RID: 9576
	public int ObjectsPerPatch = 10;

	// Token: 0x04002569 RID: 9577
	public float ClusterRadius = 2f;

	// Token: 0x0400256A RID: 9578
	public int ClusterSizeMin = 1;

	// Token: 0x0400256B RID: 9579
	public int ClusterSizeMax = 10;

	// Token: 0x0400256C RID: 9580
	public int PatchCount = 8;

	// Token: 0x0400256D RID: 9581
	public int PatchSize = 100;

	// Token: 0x0400256E RID: 9582
	public bool LOD = true;
}
