using System;
using UnityEngine;

// Token: 0x020002EE RID: 750
[ExecuteInEditMode]
public class MeshTrimTester : MonoBehaviour
{
	// Token: 0x040016C6 RID: 5830
	public MeshTrimSettings Settings = MeshTrimSettings.Default;

	// Token: 0x040016C7 RID: 5831
	public Mesh SourceMesh;

	// Token: 0x040016C8 RID: 5832
	public MeshFilter TargetMeshFilter;

	// Token: 0x040016C9 RID: 5833
	public int SubtractIndex;
}
