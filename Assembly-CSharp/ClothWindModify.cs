using System;
using UnityEngine;

// Token: 0x02000708 RID: 1800
public class ClothWindModify : FacepunchBehaviour
{
	// Token: 0x0400287A RID: 10362
	public Cloth cloth;

	// Token: 0x0400287B RID: 10363
	private Vector3 initialClothForce;

	// Token: 0x0400287C RID: 10364
	public Vector3 worldWindScale = Vector3.one;

	// Token: 0x0400287D RID: 10365
	public Vector3 turbulenceScale = Vector3.one;
}
