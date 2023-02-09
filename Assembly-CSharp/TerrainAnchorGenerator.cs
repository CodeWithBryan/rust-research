using System;
using UnityEngine;

// Token: 0x02000661 RID: 1633
public class TerrainAnchorGenerator : MonoBehaviour, IEditorComponent
{
	// Token: 0x040025EF RID: 9711
	public float PlacementRadius = 32f;

	// Token: 0x040025F0 RID: 9712
	public float PlacementPadding;

	// Token: 0x040025F1 RID: 9713
	public float PlacementFade = 16f;

	// Token: 0x040025F2 RID: 9714
	public float PlacementDistance = 8f;

	// Token: 0x040025F3 RID: 9715
	public float AnchorExtentsMin = 8f;

	// Token: 0x040025F4 RID: 9716
	public float AnchorExtentsMax = 16f;

	// Token: 0x040025F5 RID: 9717
	public float AnchorOffsetMin;

	// Token: 0x040025F6 RID: 9718
	public float AnchorOffsetMax;
}
