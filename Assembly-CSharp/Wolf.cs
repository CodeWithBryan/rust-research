using System;

// Token: 0x020001E9 RID: 489
public class Wolf : BaseAnimalNPC
{
	// Token: 0x170001EB RID: 491
	// (get) Token: 0x06001993 RID: 6547 RVA: 0x000B7BC9 File Offset: 0x000B5DC9
	public override float RealisticMass
	{
		get
		{
			return 45f;
		}
	}

	// Token: 0x170001EC RID: 492
	// (get) Token: 0x06001994 RID: 6548 RVA: 0x000B4120 File Offset: 0x000B2320
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x06001995 RID: 6549 RVA: 0x000B7BD0 File Offset: 0x000B5DD0
	public override bool WantsToEat(BaseEntity best)
	{
		return !best.HasTrait(BaseEntity.TraitFlag.Alive) && (best.HasTrait(BaseEntity.TraitFlag.Meat) || base.WantsToEat(best));
	}

	// Token: 0x06001996 RID: 6550 RVA: 0x000B7BF0 File Offset: 0x000B5DF0
	public override string Categorize()
	{
		return "Wolf";
	}

	// Token: 0x04001205 RID: 4613
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population = 2f;
}
