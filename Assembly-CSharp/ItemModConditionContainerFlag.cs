using System;

// Token: 0x020005B1 RID: 1457
public class ItemModConditionContainerFlag : ItemMod
{
	// Token: 0x06002B8A RID: 11146 RVA: 0x00106290 File Offset: 0x00104490
	public override bool Passes(Item item)
	{
		if (item.parent == null)
		{
			return !this.requiredState;
		}
		if (!item.parent.HasFlag(this.flag))
		{
			return !this.requiredState;
		}
		return this.requiredState;
	}

	// Token: 0x04002352 RID: 9042
	public ItemContainer.Flag flag;

	// Token: 0x04002353 RID: 9043
	public bool requiredState;
}
