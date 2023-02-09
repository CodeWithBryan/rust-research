using System;

// Token: 0x020001D5 RID: 469
public class Horse : BaseAnimalNPC
{
	// Token: 0x170001DD RID: 477
	// (get) Token: 0x060018AE RID: 6318 RVA: 0x000B509A File Offset: 0x000B329A
	public override float RealisticMass
	{
		get
		{
			return 500f;
		}
	}

	// Token: 0x170001DE RID: 478
	// (get) Token: 0x060018AF RID: 6319 RVA: 0x000B4120 File Offset: 0x000B2320
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x060018B0 RID: 6320 RVA: 0x000B50A1 File Offset: 0x000B32A1
	public override void ServerInit()
	{
		base.ServerInit();
	}

	// Token: 0x060018B1 RID: 6321 RVA: 0x000B50AC File Offset: 0x000B32AC
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

	// Token: 0x060018B2 RID: 6322 RVA: 0x000B510E File Offset: 0x000B330E
	public override string Categorize()
	{
		return "Horse";
	}

	// Token: 0x040011B3 RID: 4531
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population;
}
