using System;
using UnityEngine;

// Token: 0x020005CD RID: 1485
public class ItemModRecycleInto : ItemMod
{
	// Token: 0x06002BE6 RID: 11238 RVA: 0x00107A10 File Offset: 0x00105C10
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command == "recycle_item")
		{
			int num = UnityEngine.Random.Range(this.numRecycledItemMin, this.numRecycledItemMax + 1);
			item.UseItem(1);
			if (num > 0)
			{
				Item item2 = ItemManager.Create(this.recycleIntoItem, num, 0UL);
				if (!item2.MoveToContainer(player.inventory.containerMain, -1, true, false, null, true))
				{
					item2.Drop(player.GetDropPosition(), player.GetDropVelocity(), default(Quaternion));
				}
				if (this.successEffect.isValid)
				{
					Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
				}
			}
		}
	}

	// Token: 0x040023BA RID: 9146
	public ItemDefinition recycleIntoItem;

	// Token: 0x040023BB RID: 9147
	public int numRecycledItemMin = 1;

	// Token: 0x040023BC RID: 9148
	public int numRecycledItemMax = 1;

	// Token: 0x040023BD RID: 9149
	public GameObjectRef successEffect;
}
