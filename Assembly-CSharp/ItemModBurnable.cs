using System;

// Token: 0x020005AD RID: 1453
public class ItemModBurnable : ItemMod
{
	// Token: 0x06002B83 RID: 11139 RVA: 0x00106231 File Offset: 0x00104431
	public override void OnItemCreated(Item item)
	{
		item.fuel = this.fuelAmount;
	}

	// Token: 0x0400234A RID: 9034
	public float fuelAmount = 10f;

	// Token: 0x0400234B RID: 9035
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition byproductItem;

	// Token: 0x0400234C RID: 9036
	public int byproductAmount = 1;

	// Token: 0x0400234D RID: 9037
	public float byproductChance = 0.5f;
}
