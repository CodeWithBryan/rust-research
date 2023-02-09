using System;
using UnityEngine;

// Token: 0x02000178 RID: 376
public class ItemModUnwrap : ItemMod
{
	// Token: 0x060016CD RID: 5837 RVA: 0x000AC080 File Offset: 0x000AA280
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command == "unwrap")
		{
			if (item.amount <= 0)
			{
				return;
			}
			item.UseItem(1);
			int num = UnityEngine.Random.Range(this.minTries, this.maxTries + 1);
			for (int i = 0; i < num; i++)
			{
				this.revealList.SpawnIntoContainer(player.inventory.containerMain);
			}
			if (this.successEffect.isValid)
			{
				Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
			}
		}
	}

	// Token: 0x04000FD6 RID: 4054
	public LootSpawn revealList;

	// Token: 0x04000FD7 RID: 4055
	public GameObjectRef successEffect;

	// Token: 0x04000FD8 RID: 4056
	public int minTries = 1;

	// Token: 0x04000FD9 RID: 4057
	public int maxTries = 1;
}
