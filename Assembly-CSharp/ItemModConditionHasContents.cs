using System;
using System.Linq;
using UnityEngine;

// Token: 0x020005B3 RID: 1459
public class ItemModConditionHasContents : ItemMod
{
	// Token: 0x06002B8E RID: 11150 RVA: 0x0010636C File Offset: 0x0010456C
	public override bool Passes(Item item)
	{
		if (item.contents == null)
		{
			return !this.requiredState;
		}
		if (item.contents.itemList.Count == 0)
		{
			return !this.requiredState;
		}
		if (this.itemDef && !item.contents.itemList.Any((Item x) => x.info == this.itemDef))
		{
			return !this.requiredState;
		}
		return this.requiredState;
	}

	// Token: 0x04002357 RID: 9047
	[Tooltip("Can be null to mean any item")]
	public ItemDefinition itemDef;

	// Token: 0x04002358 RID: 9048
	public bool requiredState;
}
