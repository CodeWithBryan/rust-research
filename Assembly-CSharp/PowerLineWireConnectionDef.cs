using System;
using UnityEngine;

// Token: 0x02000659 RID: 1625
[Serializable]
public class PowerLineWireConnectionDef
{
	// Token: 0x06002E2C RID: 11820 RVA: 0x001150BA File Offset: 0x001132BA
	public PowerLineWireConnectionDef()
	{
	}

	// Token: 0x06002E2D RID: 11821 RVA: 0x001150E4 File Offset: 0x001132E4
	public PowerLineWireConnectionDef(PowerLineWireConnectionDef src)
	{
		this.inOffset = src.inOffset;
		this.outOffset = src.outOffset;
		this.radius = src.radius;
	}

	// Token: 0x040025D9 RID: 9689
	public Vector3 inOffset = Vector3.zero;

	// Token: 0x040025DA RID: 9690
	public Vector3 outOffset = Vector3.zero;

	// Token: 0x040025DB RID: 9691
	public float radius = 0.01f;

	// Token: 0x040025DC RID: 9692
	public bool hidden;
}
