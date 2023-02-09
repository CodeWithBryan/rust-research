using System;

// Token: 0x020005D6 RID: 1494
public class ItemModUseContent : ItemMod
{
	// Token: 0x06002BF9 RID: 11257 RVA: 0x001082A6 File Offset: 0x001064A6
	public override void DoAction(Item item, BasePlayer player)
	{
		if (item.contents == null)
		{
			return;
		}
		if (item.contents.itemList.Count == 0)
		{
			return;
		}
		item.contents.itemList[0].UseItem(this.amountToConsume);
	}

	// Token: 0x040023DB RID: 9179
	public int amountToConsume = 1;
}
