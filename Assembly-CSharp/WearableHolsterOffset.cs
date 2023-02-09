using System;
using UnityEngine;

// Token: 0x0200057B RID: 1403
public class WearableHolsterOffset : MonoBehaviour
{
	// Token: 0x04002236 RID: 8758
	public WearableHolsterOffset.offsetInfo[] Offsets;

	// Token: 0x02000D0D RID: 3341
	[Serializable]
	public class offsetInfo
	{
		// Token: 0x040044DF RID: 17631
		public HeldEntity.HolsterInfo.HolsterSlot type;

		// Token: 0x040044E0 RID: 17632
		public Vector3 offset;

		// Token: 0x040044E1 RID: 17633
		public Vector3 rotationOffset;

		// Token: 0x040044E2 RID: 17634
		public int priority;
	}
}
