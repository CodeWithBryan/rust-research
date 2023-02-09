using System;
using UnityEngine;

// Token: 0x0200065A RID: 1626
[Serializable]
public class PowerLineWireConnection
{
	// Token: 0x040025DD RID: 9693
	public Vector3 inOffset = Vector3.zero;

	// Token: 0x040025DE RID: 9694
	public Vector3 outOffset = Vector3.zero;

	// Token: 0x040025DF RID: 9695
	public float radius = 0.01f;

	// Token: 0x040025E0 RID: 9696
	public Transform start;

	// Token: 0x040025E1 RID: 9697
	public Transform end;
}
