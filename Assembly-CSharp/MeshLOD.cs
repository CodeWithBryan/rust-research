using System;
using UnityEngine;

// Token: 0x02000514 RID: 1300
public class MeshLOD : LODComponent, IBatchingHandler
{
	// Token: 0x040020BE RID: 8382
	[Horizontal(1, 0)]
	public MeshLOD.State[] States;

	// Token: 0x02000CE6 RID: 3302
	[Serializable]
	public class State
	{
		// Token: 0x0400443A RID: 17466
		public float distance;

		// Token: 0x0400443B RID: 17467
		public Mesh mesh;
	}
}
