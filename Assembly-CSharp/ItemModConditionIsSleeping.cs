using System;

// Token: 0x020005B6 RID: 1462
public class ItemModConditionIsSleeping : ItemMod
{
	// Token: 0x06002B95 RID: 11157 RVA: 0x0010643C File Offset: 0x0010463C
	public override bool Passes(Item item)
	{
		BasePlayer ownerPlayer = item.GetOwnerPlayer();
		return !(ownerPlayer == null) && ownerPlayer.IsSleeping() == this.requiredState;
	}

	// Token: 0x0400235C RID: 9052
	public bool requiredState;
}
