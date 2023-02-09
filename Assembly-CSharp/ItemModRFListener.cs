using System;

// Token: 0x020001A7 RID: 423
public class ItemModRFListener : ItemModAssociatedEntity<PagerEntity>
{
	// Token: 0x060017D0 RID: 6096 RVA: 0x000B1038 File Offset: 0x000AF238
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		base.ServerCommand(item, command, player);
		PagerEntity associatedEntity = ItemModAssociatedEntity<PagerEntity>.GetAssociatedEntity(item, true);
		if (command == "stop")
		{
			associatedEntity.SetOff();
			return;
		}
		if (command == "silenton")
		{
			associatedEntity.SetSilentMode(true);
			return;
		}
		if (command == "silentoff")
		{
			associatedEntity.SetSilentMode(false);
		}
	}

	// Token: 0x040010D2 RID: 4306
	public GameObjectRef frequencyPanelPrefab;
}
