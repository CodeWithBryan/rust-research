using System;

// Token: 0x020005D4 RID: 1492
public class ItemModSwitchFlag : ItemMod
{
	// Token: 0x06002BF5 RID: 11253 RVA: 0x00108136 File Offset: 0x00106336
	public override void DoAction(Item item, BasePlayer player)
	{
		if (item.amount < 1)
		{
			return;
		}
		if (item.HasFlag(this.flag) == this.state)
		{
			return;
		}
		item.SetFlag(this.flag, this.state);
		item.MarkDirty();
	}

	// Token: 0x040023D2 RID: 9170
	public Item.Flag flag;

	// Token: 0x040023D3 RID: 9171
	public bool state;
}
