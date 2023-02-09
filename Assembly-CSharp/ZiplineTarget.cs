using System;
using UnityEngine;

// Token: 0x020000F1 RID: 241
public class ZiplineTarget : MonoBehaviour
{
	// Token: 0x060014A3 RID: 5283 RVA: 0x000A3578 File Offset: 0x000A1778
	public bool IsValidPosition(Vector3 position)
	{
		float num = Vector3.Dot((position - this.Target.position.WithY(position.y)).normalized, this.Target.forward);
		return num >= this.MonumentConnectionDotMin && num <= this.MonumentConnectionDotMax;
	}

	// Token: 0x060014A4 RID: 5284 RVA: 0x000A35D4 File Offset: 0x000A17D4
	public bool IsValidChainPoint(Vector3 from, Vector3 to)
	{
		float num = Vector3.Dot((from - this.Target.position.WithY(from.y)).normalized, this.Target.forward);
		float num2 = Vector3.Dot((to - this.Target.position.WithY(from.y)).normalized, this.Target.forward);
		if ((num > 0f && num2 > 0f) || (num < 0f && num2 < 0f))
		{
			return false;
		}
		num2 = Mathf.Abs(num2);
		return num2 >= this.MonumentConnectionDotMin && num2 <= this.MonumentConnectionDotMax;
	}

	// Token: 0x04000D2F RID: 3375
	public Transform Target;

	// Token: 0x04000D30 RID: 3376
	public bool IsChainPoint;

	// Token: 0x04000D31 RID: 3377
	public float MonumentConnectionDotMin = 0.2f;

	// Token: 0x04000D32 RID: 3378
	public float MonumentConnectionDotMax = 1f;
}
