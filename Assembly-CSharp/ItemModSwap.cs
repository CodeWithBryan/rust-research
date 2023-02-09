using System;
using UnityEngine;

// Token: 0x020005D3 RID: 1491
public class ItemModSwap : ItemMod
{
	// Token: 0x06002BF3 RID: 11251 RVA: 0x00107F7C File Offset: 0x0010617C
	public override void DoAction(Item item, BasePlayer player)
	{
		if (item.amount < 1)
		{
			return;
		}
		foreach (ItemAmount itemAmount in this.becomeItem)
		{
			Item item2 = ItemManager.Create(itemAmount.itemDef, (int)itemAmount.amount, 0UL);
			if (item2 != null)
			{
				if (!item2.MoveToContainer(item.parent, -1, true, false, null, true))
				{
					player.GiveItem(item2, BaseEntity.GiveItemReason.Generic);
				}
				if (this.sendPlayerPickupNotification)
				{
					player.Command("note.inv", new object[]
					{
						item2.info.itemid,
						item2.amount
					});
				}
			}
		}
		if (this.RandomOptions.Length != 0)
		{
			int num = UnityEngine.Random.Range(0, this.RandomOptions.Length);
			Item item3 = ItemManager.Create(this.RandomOptions[num].itemDef, (int)this.RandomOptions[num].amount, 0UL);
			if (item3 != null)
			{
				if (!item3.MoveToContainer(item.parent, -1, true, false, null, true))
				{
					player.GiveItem(item3, BaseEntity.GiveItemReason.Generic);
				}
				if (this.sendPlayerPickupNotification)
				{
					player.Command("note.inv", new object[]
					{
						item3.info.itemid,
						item3.amount
					});
				}
			}
		}
		if (this.sendPlayerDropNotification)
		{
			player.Command("note.inv", new object[]
			{
				item.info.itemid,
				-1
			});
		}
		if (this.actionEffect.isValid)
		{
			Effect.server.Run(this.actionEffect.resourcePath, player.transform.position, Vector3.up, null, false);
		}
		item.UseItem(1);
	}

	// Token: 0x040023CC RID: 9164
	public GameObjectRef actionEffect;

	// Token: 0x040023CD RID: 9165
	public ItemAmount[] becomeItem;

	// Token: 0x040023CE RID: 9166
	public bool sendPlayerPickupNotification;

	// Token: 0x040023CF RID: 9167
	public bool sendPlayerDropNotification;

	// Token: 0x040023D0 RID: 9168
	public float xpScale = 1f;

	// Token: 0x040023D1 RID: 9169
	public ItemAmount[] RandomOptions;
}
