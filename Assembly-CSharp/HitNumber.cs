using System;
using UnityEngine;

// Token: 0x02000130 RID: 304
public class HitNumber : MonoBehaviour
{
	// Token: 0x060015F6 RID: 5622 RVA: 0x000A84F5 File Offset: 0x000A66F5
	public int ColorToMultiplier(HitNumber.HitType type)
	{
		switch (type)
		{
		case HitNumber.HitType.Yellow:
			return 1;
		case HitNumber.HitType.Green:
			return 3;
		case HitNumber.HitType.Blue:
			return 5;
		case HitNumber.HitType.Purple:
			return 10;
		case HitNumber.HitType.Red:
			return 20;
		default:
			return 0;
		}
	}

	// Token: 0x060015F7 RID: 5623 RVA: 0x000A8520 File Offset: 0x000A6720
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawSphere(base.transform.position, 0.025f);
	}

	// Token: 0x04000E70 RID: 3696
	public HitNumber.HitType hitType;

	// Token: 0x02000BD7 RID: 3031
	public enum HitType
	{
		// Token: 0x04003FE3 RID: 16355
		Yellow,
		// Token: 0x04003FE4 RID: 16356
		Green,
		// Token: 0x04003FE5 RID: 16357
		Blue,
		// Token: 0x04003FE6 RID: 16358
		Purple,
		// Token: 0x04003FE7 RID: 16359
		Red
	}
}
