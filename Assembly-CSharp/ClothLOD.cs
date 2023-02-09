using System;
using UnityEngine;

// Token: 0x02000504 RID: 1284
public class ClothLOD : FacepunchBehaviour
{
	// Token: 0x04002094 RID: 8340
	[ServerVar(Help = "distance cloth will simulate until")]
	public static float clothLODDist = 20f;

	// Token: 0x04002095 RID: 8341
	public Cloth cloth;
}
