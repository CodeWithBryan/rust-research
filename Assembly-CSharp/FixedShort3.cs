using System;
using UnityEngine;

// Token: 0x02000926 RID: 2342
public struct FixedShort3
{
	// Token: 0x060037E1 RID: 14305 RVA: 0x0014B1B2 File Offset: 0x001493B2
	public FixedShort3(Vector3 vec)
	{
		this.x = (short)(vec.x * 1024f);
		this.y = (short)(vec.y * 1024f);
		this.z = (short)(vec.z * 1024f);
	}

	// Token: 0x060037E2 RID: 14306 RVA: 0x0014B1ED File Offset: 0x001493ED
	public static explicit operator Vector3(FixedShort3 vec)
	{
		return new Vector3((float)vec.x * 0.0009765625f, (float)vec.y * 0.0009765625f, (float)vec.z * 0.0009765625f);
	}

	// Token: 0x040031F3 RID: 12787
	private const int FracBits = 10;

	// Token: 0x040031F4 RID: 12788
	private const float MaxFrac = 1024f;

	// Token: 0x040031F5 RID: 12789
	private const float RcpMaxFrac = 0.0009765625f;

	// Token: 0x040031F6 RID: 12790
	public short x;

	// Token: 0x040031F7 RID: 12791
	public short y;

	// Token: 0x040031F8 RID: 12792
	public short z;
}
