using System;
using UnityEngine;

// Token: 0x020005A6 RID: 1446
public class ItemModActionChange : ItemMod
{
	// Token: 0x06002B65 RID: 11109 RVA: 0x00105D00 File Offset: 0x00103F00
	public override void OnChanged(Item item)
	{
		if (!item.isServer)
		{
			return;
		}
		BasePlayer ownerPlayer = item.GetOwnerPlayer();
		foreach (ItemMod itemMod in this.actions)
		{
			if (itemMod.CanDoAction(item, ownerPlayer))
			{
				itemMod.DoAction(item, ownerPlayer);
			}
		}
	}

	// Token: 0x06002B66 RID: 11110 RVA: 0x00105D48 File Offset: 0x00103F48
	private void OnValidate()
	{
		if (this.actions == null)
		{
			Debug.LogWarning("ItemModMenuOption: actions is null!", base.gameObject);
		}
	}

	// Token: 0x0400233D RID: 9021
	public ItemMod[] actions;
}
