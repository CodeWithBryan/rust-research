using System;
using UnityEngine;

// Token: 0x020005CF RID: 1487
public class ItemModReveal : ItemMod
{
	// Token: 0x06002BEB RID: 11243 RVA: 0x00107B90 File Offset: 0x00105D90
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command == "reveal")
		{
			if (item.amount < this.numForReveal)
			{
				return;
			}
			int position = item.position;
			item.UseItem(this.numForReveal);
			Item item2 = null;
			if (this.revealedItemOverride)
			{
				item2 = ItemManager.Create(this.revealedItemOverride, this.revealedItemAmount, 0UL);
			}
			if (item2 != null && !item2.MoveToContainer(player.inventory.containerMain, (item.amount == 0) ? position : -1, true, false, null, true))
			{
				item2.Drop(player.GetDropPosition(), player.GetDropVelocity(), default(Quaternion));
			}
			if (this.successEffect.isValid)
			{
				Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
			}
		}
	}

	// Token: 0x040023C1 RID: 9153
	public int numForReveal = 10;

	// Token: 0x040023C2 RID: 9154
	public ItemDefinition revealedItemOverride;

	// Token: 0x040023C3 RID: 9155
	public int revealedItemAmount = 1;

	// Token: 0x040023C4 RID: 9156
	public LootSpawn revealList;

	// Token: 0x040023C5 RID: 9157
	public GameObjectRef successEffect;
}
