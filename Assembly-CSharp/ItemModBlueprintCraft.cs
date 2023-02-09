using System;
using UnityEngine;

// Token: 0x020005AC RID: 1452
public class ItemModBlueprintCraft : ItemMod
{
	// Token: 0x06002B81 RID: 11137 RVA: 0x001060DC File Offset: 0x001042DC
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (item.GetOwnerPlayer() != player)
		{
			return;
		}
		if (command == "craft")
		{
			if (!item.IsBlueprint())
			{
				return;
			}
			if (!player.inventory.crafting.CanCraft(item.blueprintTargetDef.Blueprint, 1, false))
			{
				return;
			}
			Item fromTempBlueprint = item;
			if (item.amount > 1)
			{
				fromTempBlueprint = item.SplitItem(1);
			}
			player.inventory.crafting.CraftItem(item.blueprintTargetDef.Blueprint, player, null, 1, 0, fromTempBlueprint, false);
			if (this.successEffect.isValid)
			{
				Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
			}
		}
		if (command == "craft_all")
		{
			if (!item.IsBlueprint())
			{
				return;
			}
			if (!player.inventory.crafting.CanCraft(item.blueprintTargetDef.Blueprint, item.amount, false))
			{
				return;
			}
			player.inventory.crafting.CraftItem(item.blueprintTargetDef.Blueprint, player, null, item.amount, 0, item, false);
			if (this.successEffect.isValid)
			{
				Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
			}
		}
	}

	// Token: 0x04002349 RID: 9033
	public GameObjectRef successEffect;
}
