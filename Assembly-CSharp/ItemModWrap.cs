using System;
using UnityEngine;

// Token: 0x0200017F RID: 383
public class ItemModWrap : ItemMod
{
	// Token: 0x060016DA RID: 5850 RVA: 0x000AC2AC File Offset: 0x000AA4AC
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command == "wrap")
		{
			if (item.amount <= 0)
			{
				return;
			}
			Item slot = item.contents.GetSlot(0);
			if (slot == null)
			{
				return;
			}
			int position = item.position;
			ItemContainer rootContainer = item.GetRootContainer();
			item.RemoveFromContainer();
			Item item2 = ItemManager.Create(this.wrappedDefinition, 1, 0UL);
			slot.MoveToContainer(item2.contents, -1, true, false, null, true);
			item2.MoveToContainer(rootContainer, position, true, false, null, true);
			item.Remove(0f);
			if (this.successEffect.isValid)
			{
				Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
			}
		}
	}

	// Token: 0x04000FE1 RID: 4065
	public GameObjectRef successEffect;

	// Token: 0x04000FE2 RID: 4066
	public ItemDefinition wrappedDefinition;

	// Token: 0x04000FE3 RID: 4067
	public static Translate.Phrase wrap_gift = new Translate.Phrase("wrap_gift", "Wrap Gift");

	// Token: 0x04000FE4 RID: 4068
	public static Translate.Phrase wrap_gift_desc = new Translate.Phrase("wrap_gift_desc", "Wrap this item and turn it in to an openable gift");
}
