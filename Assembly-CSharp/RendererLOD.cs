using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200051A RID: 1306
public class RendererLOD : LODComponent, IBatchingHandler
{
	// Token: 0x040020C5 RID: 8389
	public RendererLOD.State[] States;

	// Token: 0x02000CEA RID: 3306
	[Serializable]
	public class State
	{
		// Token: 0x04004445 RID: 17477
		public float distance;

		// Token: 0x04004446 RID: 17478
		public Renderer renderer;

		// Token: 0x04004447 RID: 17479
		[NonSerialized]
		public MeshFilter filter;

		// Token: 0x04004448 RID: 17480
		[NonSerialized]
		public ShadowCastingMode shadowMode;

		// Token: 0x04004449 RID: 17481
		[NonSerialized]
		public bool isImpostor;
	}
}
