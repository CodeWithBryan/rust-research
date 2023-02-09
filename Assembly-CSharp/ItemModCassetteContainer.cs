using System;

// Token: 0x020005AE RID: 1454
public class ItemModCassetteContainer : ItemModContainer
{
	// Token: 0x17000366 RID: 870
	// (get) Token: 0x06002B85 RID: 11141 RVA: 0x00003A54 File Offset: 0x00001C54
	protected override bool ForceAcceptItemCheck
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002B86 RID: 11142 RVA: 0x00106264 File Offset: 0x00104464
	protected override void SetAllowedItems(ItemContainer container)
	{
		container.SetOnlyAllowedItems(this.CassetteItems);
	}

	// Token: 0x0400234E RID: 9038
	public ItemDefinition[] CassetteItems;
}
