using System;
using UnityEngine;

// Token: 0x020005A4 RID: 1444
public class ItemSelector : PropertyAttribute
{
	// Token: 0x06002B54 RID: 11092 RVA: 0x00105CA9 File Offset: 0x00103EA9
	public ItemSelector(ItemCategory category = ItemCategory.All)
	{
		this.category = category;
	}

	// Token: 0x0400233B RID: 9019
	public ItemCategory category = ItemCategory.All;
}
