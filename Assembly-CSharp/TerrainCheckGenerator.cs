using System;
using UnityEngine;

// Token: 0x02000664 RID: 1636
public class TerrainCheckGenerator : MonoBehaviour, IEditorComponent
{
	// Token: 0x040025F9 RID: 9721
	public float PlacementRadius = 32f;

	// Token: 0x040025FA RID: 9722
	public float PlacementPadding;

	// Token: 0x040025FB RID: 9723
	public float PlacementFade = 16f;

	// Token: 0x040025FC RID: 9724
	public float PlacementDistance = 8f;

	// Token: 0x040025FD RID: 9725
	public float CheckExtentsMin = 8f;

	// Token: 0x040025FE RID: 9726
	public float CheckExtentsMax = 16f;

	// Token: 0x040025FF RID: 9727
	public bool CheckRotate = true;
}
