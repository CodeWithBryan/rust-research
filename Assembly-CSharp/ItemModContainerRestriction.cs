using System;

// Token: 0x020005BC RID: 1468
public class ItemModContainerRestriction : ItemMod
{
	// Token: 0x06002BAC RID: 11180 RVA: 0x00106C09 File Offset: 0x00104E09
	public bool CanExistWith(ItemModContainerRestriction other)
	{
		return other == null || (this.slotFlags & other.slotFlags) == (ItemModContainerRestriction.SlotFlags)0;
	}

	// Token: 0x04002374 RID: 9076
	[InspectorFlags]
	public ItemModContainerRestriction.SlotFlags slotFlags;

	// Token: 0x02000D24 RID: 3364
	[Flags]
	public enum SlotFlags
	{
		// Token: 0x04004536 RID: 17718
		Map = 1
	}
}
