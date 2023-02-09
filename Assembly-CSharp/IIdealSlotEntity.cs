using System;

// Token: 0x020003D6 RID: 982
public interface IIdealSlotEntity
{
	// Token: 0x0600217E RID: 8574
	int GetIdealSlot(BasePlayer player, Item item);

	// Token: 0x0600217F RID: 8575
	uint GetIdealContainer(BasePlayer player, Item item);
}
