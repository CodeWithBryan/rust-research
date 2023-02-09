using System;

// Token: 0x020001D1 RID: 465
public class Chicken : BaseAnimalNPC
{
	// Token: 0x170001DA RID: 474
	// (get) Token: 0x06001884 RID: 6276 RVA: 0x000A4DF3 File Offset: 0x000A2FF3
	public override float RealisticMass
	{
		get
		{
			return 3f;
		}
	}

	// Token: 0x170001DB RID: 475
	// (get) Token: 0x06001885 RID: 6277 RVA: 0x000B4120 File Offset: 0x000B2320
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x06001886 RID: 6278 RVA: 0x000B41E8 File Offset: 0x000B23E8
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

	// Token: 0x06001887 RID: 6279 RVA: 0x000B424A File Offset: 0x000B244A
	public override string Categorize()
	{
		return "Chicken";
	}

	// Token: 0x0400118A RID: 4490
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population = 3f;
}
