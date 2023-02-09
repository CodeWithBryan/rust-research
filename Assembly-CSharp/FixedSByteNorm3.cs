using System;
using UnityEngine;

// Token: 0x02000927 RID: 2343
public struct FixedSByteNorm3
{
	// Token: 0x060037E3 RID: 14307 RVA: 0x0014B21B File Offset: 0x0014941B
	public FixedSByteNorm3(Vector3 vec)
	{
		this.x = (sbyte)(vec.x * 128f);
		this.y = (sbyte)(vec.y * 128f);
		this.z = (sbyte)(vec.z * 128f);
	}

	// Token: 0x060037E4 RID: 14308 RVA: 0x0014B256 File Offset: 0x00149456
	public static explicit operator Vector3(FixedSByteNorm3 vec)
	{
		return new Vector3((float)vec.x * 0.0078125f, (float)vec.y * 0.0078125f, (float)vec.z * 0.0078125f);
	}

	// Token: 0x040031F9 RID: 12793
	private const int FracBits = 7;

	// Token: 0x040031FA RID: 12794
	private const float MaxFrac = 128f;

	// Token: 0x040031FB RID: 12795
	private const float RcpMaxFrac = 0.0078125f;

	// Token: 0x040031FC RID: 12796
	public sbyte x;

	// Token: 0x040031FD RID: 12797
	public sbyte y;

	// Token: 0x040031FE RID: 12798
	public sbyte z;
}
