using System;

// Token: 0x020004AF RID: 1199
public interface IIndustrialStorage
{
	// Token: 0x17000314 RID: 788
	// (get) Token: 0x060026B4 RID: 9908
	ItemContainer Container { get; }

	// Token: 0x060026B5 RID: 9909
	Vector2i InputSlotRange(int slotIndex);

	// Token: 0x060026B6 RID: 9910
	Vector2i OutputSlotRange(int slotIndex);

	// Token: 0x060026B7 RID: 9911
	void OnStorageItemTransferBegin();

	// Token: 0x060026B8 RID: 9912
	void OnStorageItemTransferEnd();

	// Token: 0x17000315 RID: 789
	// (get) Token: 0x060026B9 RID: 9913
	BaseEntity IndustrialEntity { get; }
}
