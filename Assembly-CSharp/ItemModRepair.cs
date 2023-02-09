using System;
using UnityEngine;

// Token: 0x020005CE RID: 1486
public class ItemModRepair : ItemMod
{
	// Token: 0x06002BE8 RID: 11240 RVA: 0x00107AD8 File Offset: 0x00105CD8
	public bool HasCraftLevel(BasePlayer player = null)
	{
		return player != null && player.isServer && player.currentCraftLevel >= (float)this.workbenchLvlRequired;
	}

	// Token: 0x06002BE9 RID: 11241 RVA: 0x00107B00 File Offset: 0x00105D00
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command == "refill")
		{
			if (player.IsSwimming())
			{
				return;
			}
			if (!this.HasCraftLevel(player))
			{
				return;
			}
			if (item.conditionNormalized >= 1f)
			{
				return;
			}
			item.DoRepair(this.conditionLost);
			if (this.successEffect.isValid)
			{
				Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
			}
		}
	}

	// Token: 0x040023BE RID: 9150
	public float conditionLost = 0.05f;

	// Token: 0x040023BF RID: 9151
	public GameObjectRef successEffect;

	// Token: 0x040023C0 RID: 9152
	public int workbenchLvlRequired;
}
