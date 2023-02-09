using System;

// Token: 0x020005B4 RID: 1460
public class ItemModConditionHasFlag : ItemMod
{
	// Token: 0x06002B91 RID: 11153 RVA: 0x001063F5 File Offset: 0x001045F5
	public override bool Passes(Item item)
	{
		return item.HasFlag(this.flag) == this.requiredState;
	}

	// Token: 0x04002359 RID: 9049
	public Item.Flag flag;

	// Token: 0x0400235A RID: 9050
	public bool requiredState;
}
