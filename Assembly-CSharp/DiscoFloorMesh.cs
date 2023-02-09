using System;
using UnityEngine;

// Token: 0x0200037B RID: 891
public class DiscoFloorMesh : MonoBehaviour, IClientComponent
{
	// Token: 0x04001899 RID: 6297
	public int GridRows = 5;

	// Token: 0x0400189A RID: 6298
	public int GridColumns = 5;

	// Token: 0x0400189B RID: 6299
	public float GridSize = 1f;

	// Token: 0x0400189C RID: 6300
	[Range(0f, 10f)]
	public float TestOffset;

	// Token: 0x0400189D RID: 6301
	public Color OffColor = Color.grey;

	// Token: 0x0400189E RID: 6302
	public MeshRenderer Renderer;

	// Token: 0x0400189F RID: 6303
	public bool DrawInEditor;

	// Token: 0x040018A0 RID: 6304
	public MeshFilter Filter;

	// Token: 0x040018A1 RID: 6305
	public AnimationCurve customCurveX = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040018A2 RID: 6306
	public AnimationCurve customCurveY = AnimationCurve.Linear(0f, 0f, 1f, 1f);
}
