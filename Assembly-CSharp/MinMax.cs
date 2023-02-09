using System;
using UnityEngine;

// Token: 0x020008BF RID: 2239
[Serializable]
public class MinMax
{
	// Token: 0x06003612 RID: 13842 RVA: 0x0014323E File Offset: 0x0014143E
	public MinMax(float x, float y)
	{
		this.x = x;
		this.y = y;
	}

	// Token: 0x06003613 RID: 13843 RVA: 0x0014325F File Offset: 0x0014145F
	public float Random()
	{
		return UnityEngine.Random.Range(this.x, this.y);
	}

	// Token: 0x06003614 RID: 13844 RVA: 0x00143272 File Offset: 0x00141472
	public float Lerp(float t)
	{
		return Mathf.Lerp(this.x, this.y, t);
	}

	// Token: 0x06003615 RID: 13845 RVA: 0x00143286 File Offset: 0x00141486
	public float Lerp(float a, float b, float t)
	{
		return Mathf.Lerp(this.x, this.y, Mathf.InverseLerp(a, b, t));
	}

	// Token: 0x04003109 RID: 12553
	public float x;

	// Token: 0x0400310A RID: 12554
	public float y = 1f;
}
