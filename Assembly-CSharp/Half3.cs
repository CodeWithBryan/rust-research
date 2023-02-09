using System;
using UnityEngine;

// Token: 0x02000929 RID: 2345
public struct Half3
{
	// Token: 0x060037E7 RID: 14311 RVA: 0x0014B318 File Offset: 0x00149518
	public Half3(Vector3 vec)
	{
		this.x = Mathf.FloatToHalf(vec.x);
		this.y = Mathf.FloatToHalf(vec.y);
		this.z = Mathf.FloatToHalf(vec.z);
	}

	// Token: 0x060037E8 RID: 14312 RVA: 0x0014B34D File Offset: 0x0014954D
	public static explicit operator Vector3(Half3 vec)
	{
		return new Vector3(Mathf.HalfToFloat(vec.x), Mathf.HalfToFloat(vec.y), Mathf.HalfToFloat(vec.z));
	}

	// Token: 0x04003206 RID: 12806
	public ushort x;

	// Token: 0x04003207 RID: 12807
	public ushort y;

	// Token: 0x04003208 RID: 12808
	public ushort z;
}
