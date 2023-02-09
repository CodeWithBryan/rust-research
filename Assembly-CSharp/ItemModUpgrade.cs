using System;
using UnityEngine;

// Token: 0x020005D5 RID: 1493
public class ItemModUpgrade : ItemMod
{
	// Token: 0x06002BF7 RID: 11255 RVA: 0x00108170 File Offset: 0x00106370
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command == "upgrade_item")
		{
			if (item.amount < this.numForUpgrade)
			{
				return;
			}
			if (UnityEngine.Random.Range(0f, 1f) <= this.upgradeSuccessChance)
			{
				item.UseItem(this.numForUpgrade);
				Item item2 = ItemManager.Create(this.upgradedItem, this.numUpgradedItem, 0UL);
				if (!item2.MoveToContainer(player.inventory.containerMain, -1, true, false, null, true))
				{
					item2.Drop(player.GetDropPosition(), player.GetDropVelocity(), default(Quaternion));
				}
				if (this.successEffect.isValid)
				{
					Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
					return;
				}
			}
			else
			{
				item.UseItem(this.numToLoseOnFail);
				if (this.failEffect.isValid)
				{
					Effect.server.Run(this.failEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
				}
			}
		}
	}

	// Token: 0x040023D4 RID: 9172
	public int numForUpgrade = 10;

	// Token: 0x040023D5 RID: 9173
	public float upgradeSuccessChance = 1f;

	// Token: 0x040023D6 RID: 9174
	public int numToLoseOnFail = 2;

	// Token: 0x040023D7 RID: 9175
	public ItemDefinition upgradedItem;

	// Token: 0x040023D8 RID: 9176
	public int numUpgradedItem = 1;

	// Token: 0x040023D9 RID: 9177
	public GameObjectRef successEffect;

	// Token: 0x040023DA RID: 9178
	public GameObjectRef failEffect;
}
