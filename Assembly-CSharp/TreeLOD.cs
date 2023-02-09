using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200051D RID: 1309
public class TreeLOD : LODComponent
{
	// Token: 0x040020C9 RID: 8393
	[Horizontal(1, 0)]
	public TreeLOD.State[] States;

	// Token: 0x02000CEB RID: 3307
	[Serializable]
	public class State
	{
		// Token: 0x0400444A RID: 17482
		public float distance;

		// Token: 0x0400444B RID: 17483
		public Renderer renderer;

		// Token: 0x0400444C RID: 17484
		[NonSerialized]
		public MeshFilter filter;

		// Token: 0x0400444D RID: 17485
		[NonSerialized]
		public ShadowCastingMode shadowMode;

		// Token: 0x0400444E RID: 17486
		[NonSerialized]
		public bool isImpostor;
	}
}
