using System;

// Token: 0x020001CE RID: 462
public class Bear : BaseAnimalNPC
{
	// Token: 0x170001D4 RID: 468
	// (get) Token: 0x06001872 RID: 6258 RVA: 0x000B4119 File Offset: 0x000B2319
	public override float RealisticMass
	{
		get
		{
			return 150f;
		}
	}

	// Token: 0x170001D5 RID: 469
	// (get) Token: 0x06001873 RID: 6259 RVA: 0x000B4120 File Offset: 0x000B2320
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x06001874 RID: 6260 RVA: 0x000B4124 File Offset: 0x000B2324
	public override bool WantsToEat(BaseEntity best)
	{
		return !best.HasTrait(BaseEntity.TraitFlag.Alive) && base.WantsToEat(best);
	}

	// Token: 0x06001875 RID: 6261 RVA: 0x000B4138 File Offset: 0x000B2338
	public override string Categorize()
	{
		return "Bear";
	}

	// Token: 0x04001187 RID: 4487
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population = 2f;
}
