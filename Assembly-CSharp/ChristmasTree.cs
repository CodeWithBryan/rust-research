using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000180 RID: 384
public class ChristmasTree : StorageContainer
{
	// Token: 0x060016DD RID: 5853 RVA: 0x000AC390 File Offset: 0x000AA590
	public override bool ItemFilter(Item item, int targetSlot)
	{
		if (item.info.GetComponent<ItemModXMasTreeDecoration>() == null)
		{
			return false;
		}
		using (List<Item>.Enumerator enumerator = base.inventory.itemList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.info == item.info)
				{
					return false;
				}
			}
		}
		return base.ItemFilter(item, targetSlot);
	}

	// Token: 0x060016DE RID: 5854 RVA: 0x000AC418 File Offset: 0x000AA618
	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		ItemModXMasTreeDecoration component = item.info.GetComponent<ItemModXMasTreeDecoration>();
		if (component != null)
		{
			base.SetFlag((BaseEntity.Flags)component.flagsToChange, added, false, true);
		}
		base.OnItemAddedOrRemoved(item, added);
	}

	// Token: 0x04000FE5 RID: 4069
	public GameObject[] decorations;
}
