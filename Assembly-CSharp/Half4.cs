using System;
using UnityEngine;

// Token: 0x0200092A RID: 2346
public struct Half4
{
	// Token: 0x060037E9 RID: 14313 RVA: 0x0014B378 File Offset: 0x00149578
	public Half4(Vector4 vec)
	{
		this.x = Mathf.FloatToHalf(vec.x);
		this.y = Mathf.FloatToHalf(vec.y);
		this.z = Mathf.FloatToHalf(vec.z);
		this.w = Mathf.FloatToHalf(vec.w);
	}

	// Token: 0x060037EA RID: 14314 RVA: 0x0014B3C9 File Offset: 0x001495C9
	public static explicit operator Vector4(Half4 vec)
	{
		return new Vector4(Mathf.HalfToFloat(vec.x), Mathf.HalfToFloat(vec.y), Mathf.HalfToFloat(vec.z), Mathf.HalfToFloat(vec.w));
	}

	// Token: 0x04003209 RID: 12809
	public ushort x;

	// Token: 0x0400320A RID: 12810
	public ushort y;

	// Token: 0x0400320B RID: 12811
	public ushort z;

	// Token: 0x0400320C RID: 12812
	public ushort w;
}
