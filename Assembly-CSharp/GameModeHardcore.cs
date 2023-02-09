using System;

// Token: 0x020004E9 RID: 1257
public class GameModeHardcore : GameModeVanilla
{
	// Token: 0x060027E4 RID: 10212 RVA: 0x000F4591 File Offset: 0x000F2791
	protected override void OnCreated()
	{
		base.OnCreated();
	}

	// Token: 0x060027E5 RID: 10213 RVA: 0x000F459C File Offset: 0x000F279C
	public override BaseGameMode.ResearchCostResult GetScrapCostForResearch(ItemDefinition item, ResearchTable.ResearchType researchType)
	{
		ItemBlueprint blueprint = item.Blueprint;
		int? num = (blueprint != null) ? new int?(blueprint.workbenchLevelRequired) : null;
		if (num != null)
		{
			switch (num.GetValueOrDefault())
			{
			case 1:
				return new BaseGameMode.ResearchCostResult
				{
					Scale = new float?(1.2f)
				};
			case 2:
				return new BaseGameMode.ResearchCostResult
				{
					Scale = new float?(1.4f)
				};
			case 3:
				return new BaseGameMode.ResearchCostResult
				{
					Scale = new float?(1.6f)
				};
			}
		}
		return default(BaseGameMode.ResearchCostResult);
	}
}
