using System;
using UnityEngine;

// Token: 0x020008C0 RID: 2240
public class MinMaxAttribute : PropertyAttribute
{
	// Token: 0x06003616 RID: 13846 RVA: 0x001432A1 File Offset: 0x001414A1
	public MinMaxAttribute(float min, float max)
	{
		this.min = min;
		this.max = max;
	}

	// Token: 0x0400310B RID: 12555
	public float min;

	// Token: 0x0400310C RID: 12556
	public float max;
}
