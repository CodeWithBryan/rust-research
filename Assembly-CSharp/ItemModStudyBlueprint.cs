using System;
using UnityEngine;

// Token: 0x020005D1 RID: 1489
public class ItemModStudyBlueprint : ItemMod
{
	// Token: 0x06002BEF RID: 11247 RVA: 0x00107D0C File Offset: 0x00105F0C
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (item.GetOwnerPlayer() != player)
		{
			bool flag = false;
			foreach (ItemContainer itemContainer in player.inventory.loot.containers)
			{
				if (item.GetRootContainer() == itemContainer)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
		}
		if (command == "study")
		{
			if (!item.IsBlueprint())
			{
				return;
			}
			ItemDefinition blueprintTargetDef = item.blueprintTargetDef;
			ItemBlueprint blueprint = blueprintTargetDef.Blueprint;
			bool flag2 = player.blueprints.IsUnlocked(blueprintTargetDef);
			if (flag2 && blueprint != null && blueprint.additionalUnlocks != null && blueprint.additionalUnlocks.Count > 0)
			{
				foreach (ItemDefinition itemDef in blueprint.additionalUnlocks)
				{
					if (!player.blueprints.IsUnlocked(itemDef))
					{
						flag2 = false;
					}
				}
			}
			if (blueprint != null && blueprint.defaultBlueprint)
			{
				flag2 = true;
			}
			if (flag2)
			{
				return;
			}
			Item item2 = item;
			if (item.amount > 1)
			{
				item2 = item.SplitItem(1);
			}
			item2.UseItem(1);
			player.blueprints.Unlock(blueprintTargetDef);
			if (blueprint != null && blueprint.additionalUnlocks != null && blueprint.additionalUnlocks.Count > 0)
			{
				foreach (ItemDefinition itemDef2 in blueprint.additionalUnlocks)
				{
					player.blueprints.Unlock(itemDef2);
				}
			}
			if (this.studyEffect.isValid)
			{
				Effect.server.Run(this.studyEffect.resourcePath, player, StringPool.Get("head"), Vector3.zero, Vector3.zero, null, false);
			}
		}
	}

	// Token: 0x040023C8 RID: 9160
	public GameObjectRef studyEffect;
}
