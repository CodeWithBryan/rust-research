using System;
using UnityEngine;

// Token: 0x0200017E RID: 382
public class ItemModOpenWrapped : ItemMod
{
	// Token: 0x060016D7 RID: 5847 RVA: 0x000AC1E4 File Offset: 0x000AA3E4
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command == "open")
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
			slot.MoveToContainer(rootContainer, position, true, false, null, true);
			item.Remove(0f);
			if (this.successEffect.isValid)
			{
				Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
			}
		}
	}

	// Token: 0x04000FDE RID: 4062
	public GameObjectRef successEffect;

	// Token: 0x04000FDF RID: 4063
	public static Translate.Phrase open_wrapped_gift = new Translate.Phrase("open_wrapped_gift", "Unwrap");

	// Token: 0x04000FE0 RID: 4064
	public static Translate.Phrase open_wrapped_gift_desc = new Translate.Phrase("open_wrapped_gift_desc", "Unwrap the gift and reveal its contents");
}
