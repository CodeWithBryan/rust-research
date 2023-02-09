using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020002FD RID: 765
public class DevDressPlayer : MonoBehaviour
{
	// Token: 0x06001D93 RID: 7571 RVA: 0x000CA268 File Offset: 0x000C8468
	private void ServerInitComponent()
	{
		BasePlayer component = base.GetComponent<BasePlayer>();
		if (this.DressRandomly)
		{
			this.DoRandomClothes(component);
		}
		foreach (ItemAmount itemAmount in this.clothesToWear)
		{
			if (!(itemAmount.itemDef == null))
			{
				ItemManager.Create(itemAmount.itemDef, 1, 0UL).MoveToContainer(component.inventory.containerWear, -1, true, false, null, true);
			}
		}
	}

	// Token: 0x06001D94 RID: 7572 RVA: 0x000CA2FC File Offset: 0x000C84FC
	private void DoRandomClothes(BasePlayer player)
	{
		string text = "";
		foreach (ItemDefinition itemDefinition in (from x in ItemManager.GetItemDefinitions()
		where x.GetComponent<ItemModWearable>()
		orderby Guid.NewGuid()
		select x).Take(UnityEngine.Random.Range(0, 4)))
		{
			ItemManager.Create(itemDefinition, 1, 0UL).MoveToContainer(player.inventory.containerWear, -1, true, false, null, true);
			text = text + itemDefinition.shortname + " ";
		}
		text = text.Trim();
		if (text == "")
		{
			text = "naked";
		}
		player.displayName = text;
	}

	// Token: 0x040016FC RID: 5884
	public bool DressRandomly;

	// Token: 0x040016FD RID: 5885
	public List<ItemAmount> clothesToWear;
}
