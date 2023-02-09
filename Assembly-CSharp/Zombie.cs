using System;

// Token: 0x020001EA RID: 490
public class Zombie : BaseAnimalNPC
{
	// Token: 0x170001ED RID: 493
	// (get) Token: 0x06001999 RID: 6553 RVA: 0x000B4120 File Offset: 0x000B2320
	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.Alive | BaseEntity.TraitFlag.Animal | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
		}
	}

	// Token: 0x0600199A RID: 6554 RVA: 0x000B4124 File Offset: 0x000B2324
	public override bool WantsToEat(BaseEntity best)
	{
		return !best.HasTrait(BaseEntity.TraitFlag.Alive) && base.WantsToEat(best);
	}

	// Token: 0x0600199B RID: 6555 RVA: 0x000B7C03 File Offset: 0x000B5E03
	protected override void TickSleep()
	{
		this.Sleep = 100f;
	}

	// Token: 0x0600199C RID: 6556 RVA: 0x000B7C10 File Offset: 0x000B5E10
	public override string Categorize()
	{
		return "Zombie";
	}

	// Token: 0x04001206 RID: 4614
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population;
}
