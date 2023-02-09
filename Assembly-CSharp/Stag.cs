using System;

// Token: 0x020001E8 RID: 488
public class Stag : BaseAnimalNPC
{
	// Token: 0x170001E9 RID: 489
	// (get) Token: 0x0600198D RID: 6541 RVA: 0x000B7B4D File Offset: 0x000B5D4D
	public override float RealisticMass
	{
		get
		{
			return 200f;
		}
	}

	// Token: 0x170001EA RID: 490
	// (get) Token: 0x0600198E RID: 6542 RVA: 0x000B4120 File Offset: 0x000B2320
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x0600198F RID: 6543 RVA: 0x000B7B54 File Offset: 0x000B5D54
	public override bool WantsToEat(BaseEntity best)
	{
		if (best.HasTrait(BaseEntity.TraitFlag.Alive))
		{
			return false;
		}
		if (best.HasTrait(BaseEntity.TraitFlag.Meat))
		{
			return false;
		}
		CollectibleEntity collectibleEntity = best as CollectibleEntity;
		if (collectibleEntity != null)
		{
			ItemAmount[] itemList = collectibleEntity.itemList;
			for (int i = 0; i < itemList.Length; i++)
			{
				if (itemList[i].itemDef.category == ItemCategory.Food)
				{
					return true;
				}
			}
		}
		return base.WantsToEat(best);
	}

	// Token: 0x06001990 RID: 6544 RVA: 0x000B7BB6 File Offset: 0x000B5DB6
	public override string Categorize()
	{
		return "Stag";
	}

	// Token: 0x04001204 RID: 4612
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population = 3f;
}
