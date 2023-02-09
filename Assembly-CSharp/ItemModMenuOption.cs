using System;
using UnityEngine;

// Token: 0x020005C6 RID: 1478
public class ItemModMenuOption : ItemMod
{
	// Token: 0x06002BCF RID: 11215 RVA: 0x00107429 File Offset: 0x00105629
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command != this.commandName)
		{
			return;
		}
		if (!this.actionTarget.CanDoAction(item, player))
		{
			return;
		}
		this.actionTarget.DoAction(item, player);
	}

	// Token: 0x06002BD0 RID: 11216 RVA: 0x00107458 File Offset: 0x00105658
	private void OnValidate()
	{
		if (this.actionTarget == null)
		{
			Debug.LogWarning("ItemModMenuOption: actionTarget is null!", base.gameObject);
		}
		if (string.IsNullOrEmpty(this.commandName))
		{
			Debug.LogWarning("ItemModMenuOption: commandName can't be empty!", base.gameObject);
		}
		if (this.option.icon == null)
		{
			Debug.LogWarning("No icon set for ItemModMenuOption " + base.gameObject.name, base.gameObject);
		}
	}

	// Token: 0x0400239A RID: 9114
	public string commandName;

	// Token: 0x0400239B RID: 9115
	public ItemMod actionTarget;

	// Token: 0x0400239C RID: 9116
	public BaseEntity.Menu.Option option;

	// Token: 0x0400239D RID: 9117
	[Tooltip("If true, this is the command that will run when an item is 'selected' on the toolbar")]
	public bool isPrimaryOption = true;
}
