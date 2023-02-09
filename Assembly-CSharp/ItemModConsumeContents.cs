using System;

// Token: 0x020005BA RID: 1466
public class ItemModConsumeContents : ItemMod
{
	// Token: 0x06002BA2 RID: 11170 RVA: 0x00106860 File Offset: 0x00104A60
	public override void DoAction(Item item, BasePlayer player)
	{
		foreach (Item item2 in item.contents.itemList)
		{
			ItemModConsume component = item2.info.GetComponent<ItemModConsume>();
			if (!(component == null) && component.CanDoAction(item2, player))
			{
				component.DoAction(item2, player);
				break;
			}
		}
	}

	// Token: 0x06002BA3 RID: 11171 RVA: 0x001068DC File Offset: 0x00104ADC
	public override bool CanDoAction(Item item, BasePlayer player)
	{
		if (!player.metabolism.CanConsume())
		{
			return false;
		}
		if (item.contents == null)
		{
			return false;
		}
		foreach (Item item2 in item.contents.itemList)
		{
			ItemModConsume component = item2.info.GetComponent<ItemModConsume>();
			if (!(component == null) && component.CanDoAction(item2, player))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04002369 RID: 9065
	public GameObjectRef consumeEffect;
}
