using System;

// Token: 0x020001CF RID: 463
public class Polarbear : BaseAnimalNPC
{
	// Token: 0x170001D6 RID: 470
	// (get) Token: 0x06001878 RID: 6264 RVA: 0x000B4119 File Offset: 0x000B2319
	public override float RealisticMass
	{
		get
		{
			return 150f;
		}
	}

	// Token: 0x170001D7 RID: 471
	// (get) Token: 0x06001879 RID: 6265 RVA: 0x000B4120 File Offset: 0x000B2320
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x0600187A RID: 6266 RVA: 0x000B4124 File Offset: 0x000B2324
	public override bool WantsToEat(BaseEntity best)
	{
		return !best.HasTrait(BaseEntity.TraitFlag.Alive) && base.WantsToEat(best);
	}

	// Token: 0x0600187B RID: 6267 RVA: 0x000B4153 File Offset: 0x000B2353
	public override string Categorize()
	{
		return "Polarbear";
	}

	// Token: 0x04001188 RID: 4488
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population = 1f;
}
