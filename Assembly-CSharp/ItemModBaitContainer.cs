using System;

// Token: 0x020005AB RID: 1451
public class ItemModBaitContainer : ItemModContainer
{
	// Token: 0x17000365 RID: 869
	// (get) Token: 0x06002B7D RID: 11133 RVA: 0x00003A54 File Offset: 0x00001C54
	protected override bool ForceAcceptItemCheck
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002B7E RID: 11134 RVA: 0x00106090 File Offset: 0x00104290
	protected override bool CanAcceptItem(Item item, int count)
	{
		ItemModCompostable component = item.info.GetComponent<ItemModCompostable>();
		return component != null && component.BaitValue > 0f;
	}

	// Token: 0x06002B7F RID: 11135 RVA: 0x001060C1 File Offset: 0x001042C1
	protected override void SetAllowedItems(ItemContainer container)
	{
		FishLookup.LoadFish();
		container.SetOnlyAllowedItems(FishLookup.BaitItems);
	}
}
