using System;
using UnityEngine;

// Token: 0x02000134 RID: 308
[CreateAssetMenu(menuName = "Rust/Slot Machine Payouts")]
public class SlotMachinePayoutSettings : ScriptableObject
{
	// Token: 0x04000E78 RID: 3704
	public ItemAmount SpinCost;

	// Token: 0x04000E79 RID: 3705
	public SlotMachinePayoutSettings.PayoutInfo[] Payouts;

	// Token: 0x04000E7A RID: 3706
	public int[] VirtualFaces = new int[16];

	// Token: 0x04000E7B RID: 3707
	public SlotMachinePayoutSettings.IndividualPayouts[] FacePayouts = new SlotMachinePayoutSettings.IndividualPayouts[0];

	// Token: 0x04000E7C RID: 3708
	public int TotalStops;

	// Token: 0x04000E7D RID: 3709
	public GameObjectRef DefaultWinEffect;

	// Token: 0x02000BD8 RID: 3032
	[Serializable]
	public struct PayoutInfo
	{
		// Token: 0x04003FE8 RID: 16360
		public ItemAmount Item;

		// Token: 0x04003FE9 RID: 16361
		[Range(0f, 15f)]
		public int Result1;

		// Token: 0x04003FEA RID: 16362
		[Range(0f, 15f)]
		public int Result2;

		// Token: 0x04003FEB RID: 16363
		[Range(0f, 15f)]
		public int Result3;

		// Token: 0x04003FEC RID: 16364
		public GameObjectRef OverrideWinEffect;
	}

	// Token: 0x02000BD9 RID: 3033
	[Serializable]
	public struct IndividualPayouts
	{
		// Token: 0x04003FED RID: 16365
		public ItemAmount Item;

		// Token: 0x04003FEE RID: 16366
		[Range(0f, 15f)]
		public int Result;
	}
}
