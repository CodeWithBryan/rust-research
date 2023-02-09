using System;
using UnityEngine;

// Token: 0x020005A7 RID: 1447
public class ItemModActionContainerChange : ItemMod
{
	// Token: 0x06002B68 RID: 11112 RVA: 0x00105D64 File Offset: 0x00103F64
	public override void OnParentChanged(Item item)
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

	// Token: 0x06002B69 RID: 11113 RVA: 0x00105DAC File Offset: 0x00103FAC
	private void OnValidate()
	{
		if (this.actions == null)
		{
			Debug.LogWarning("ItemModMenuOption: actions is null!", base.gameObject);
		}
	}

	// Token: 0x0400233E RID: 9022
	public ItemMod[] actions;
}
