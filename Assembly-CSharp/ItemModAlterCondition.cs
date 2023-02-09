using System;

// Token: 0x020005A8 RID: 1448
public class ItemModAlterCondition : ItemMod
{
	// Token: 0x06002B6B RID: 11115 RVA: 0x00105DC6 File Offset: 0x00103FC6
	public override void DoAction(Item item, BasePlayer player)
	{
		if (item.amount < 1)
		{
			return;
		}
		if (this.conditionChange < 0f)
		{
			item.LoseCondition(this.conditionChange * -1f);
			return;
		}
		item.RepairCondition(this.conditionChange);
	}

	// Token: 0x0400233F RID: 9023
	public float conditionChange;
}
