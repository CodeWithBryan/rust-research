using System;
using UnityEngine;

// Token: 0x020006E2 RID: 1762
[ExecuteInEditMode]
public class AdaptMeshToTerrain : MonoBehaviour
{
	// Token: 0x040027FC RID: 10236
	public LayerMask LayerMask = -1;

	// Token: 0x040027FD RID: 10237
	public float RayHeight = 10f;

	// Token: 0x040027FE RID: 10238
	public float RayMaxDistance = 20f;

	// Token: 0x040027FF RID: 10239
	public float MinDisplacement = 0.01f;

	// Token: 0x04002800 RID: 10240
	public float MaxDisplacement = 0.33f;

	// Token: 0x04002801 RID: 10241
	[Range(8f, 64f)]
	public int PlaneResolution = 24;
}
