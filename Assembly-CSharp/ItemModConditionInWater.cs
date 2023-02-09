using System;

// Token: 0x020005B5 RID: 1461
public class ItemModConditionInWater : ItemMod
{
	// Token: 0x06002B93 RID: 11155 RVA: 0x0010640C File Offset: 0x0010460C
	public override bool Passes(Item item)
	{
		BasePlayer ownerPlayer = item.GetOwnerPlayer();
		return !(ownerPlayer == null) && ownerPlayer.IsHeadUnderwater() == this.requiredState;
	}

	// Token: 0x0400235B RID: 9051
	public bool requiredState;
}
