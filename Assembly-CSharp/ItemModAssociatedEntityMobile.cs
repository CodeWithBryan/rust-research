using System;

// Token: 0x0200037D RID: 893
public class ItemModAssociatedEntityMobile : ItemModAssociatedEntity<MobileInventoryEntity>
{
	// Token: 0x17000261 RID: 609
	// (get) Token: 0x06001F3B RID: 7995 RVA: 0x00003A54 File Offset: 0x00001C54
	protected override bool AllowNullParenting
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001F3C RID: 7996 RVA: 0x000CF530 File Offset: 0x000CD730
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		base.ServerCommand(item, command, player);
		MobileInventoryEntity associatedEntity = ItemModAssociatedEntity<MobileInventoryEntity>.GetAssociatedEntity(item, true);
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
}
