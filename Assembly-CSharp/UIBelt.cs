using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x02000823 RID: 2083
public class UIBelt : SingletonComponent<UIBelt>
{
	// Token: 0x06003479 RID: 13433 RVA: 0x0013E115 File Offset: 0x0013C315
	protected override void Awake()
	{
		this.ItemIcons = (from s in base.GetComponentsInChildren<ItemIcon>()
		orderby s.slot
		select s).ToList<ItemIcon>();
	}

	// Token: 0x0600347A RID: 13434 RVA: 0x0013E14C File Offset: 0x0013C34C
	public ItemIcon GetItemIconAtSlot(int slot)
	{
		if (slot < 0 || slot >= this.ItemIcons.Count)
		{
			return null;
		}
		return this.ItemIcons[slot];
	}

	// Token: 0x04002E3E RID: 11838
	public List<ItemIcon> ItemIcons;
}
