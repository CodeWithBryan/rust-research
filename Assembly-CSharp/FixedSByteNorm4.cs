using System;
using UnityEngine;

// Token: 0x02000928 RID: 2344
public struct FixedSByteNorm4
{
	// Token: 0x060037E5 RID: 14309 RVA: 0x0014B284 File Offset: 0x00149484
	public FixedSByteNorm4(Vector4 vec)
	{
		this.x = (sbyte)(vec.x * 128f);
		this.y = (sbyte)(vec.y * 128f);
		this.z = (sbyte)(vec.z * 128f);
		this.w = (sbyte)(vec.w * 128f);
	}

	// Token: 0x060037E6 RID: 14310 RVA: 0x0014B2DD File Offset: 0x001494DD
	public static explicit operator Vector4(FixedSByteNorm4 vec)
	{
		return new Vector4((float)vec.x * 0.0078125f, (float)vec.y * 0.0078125f, (float)vec.z * 0.0078125f, (float)vec.w * 0.0078125f);
	}

	// Token: 0x040031FF RID: 12799
	private const int FracBits = 7;

	// Token: 0x04003200 RID: 12800
	private const float MaxFrac = 128f;

	// Token: 0x04003201 RID: 12801
	private const float RcpMaxFrac = 0.0078125f;

	// Token: 0x04003202 RID: 12802
	public sbyte x;

	// Token: 0x04003203 RID: 12803
	public sbyte y;

	// Token: 0x04003204 RID: 12804
	public sbyte z;

	// Token: 0x04003205 RID: 12805
	public sbyte w;
}
