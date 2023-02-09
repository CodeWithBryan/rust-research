using System;

// Token: 0x020001D0 RID: 464
public class Boar : BaseAnimalNPC
{
	// Token: 0x170001D8 RID: 472
	// (get) Token: 0x0600187E RID: 6270 RVA: 0x000B4166 File Offset: 0x000B2366
	public override float RealisticMass
	{
		get
		{
			return 85f;
		}
	}

	// Token: 0x170001D9 RID: 473
	// (get) Token: 0x0600187F RID: 6271 RVA: 0x000B4120 File Offset: 0x000B2320
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x06001880 RID: 6272 RVA: 0x000B4170 File Offset: 0x000B2370
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

	// Token: 0x06001881 RID: 6273 RVA: 0x000B41D2 File Offset: 0x000B23D2
	public override string Categorize()
	{
		return "Boar";
	}

	// Token: 0x04001189 RID: 4489
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population = 5f;
}
