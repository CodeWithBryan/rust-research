using System;
using ConVar;
using UnityEngine;

// Token: 0x020003C1 RID: 961
public class BuildingBlockDecay : global::Decay
{
	// Token: 0x060020D7 RID: 8407 RVA: 0x000D51C0 File Offset: 0x000D33C0
	public override float GetDecayDelay(BaseEntity entity)
	{
		BuildingBlock buildingBlock = entity as BuildingBlock;
		BuildingGrade.Enum grade = buildingBlock ? buildingBlock.grade : BuildingGrade.Enum.Twigs;
		return base.GetDecayDelay(grade);
	}

	// Token: 0x060020D8 RID: 8408 RVA: 0x000D51F0 File Offset: 0x000D33F0
	public override float GetDecayDuration(BaseEntity entity)
	{
		BuildingBlock buildingBlock = entity as BuildingBlock;
		BuildingGrade.Enum grade = buildingBlock ? buildingBlock.grade : BuildingGrade.Enum.Twigs;
		return base.GetDecayDuration(grade);
	}

	// Token: 0x060020D9 RID: 8409 RVA: 0x000D5220 File Offset: 0x000D3420
	public override bool ShouldDecay(BaseEntity entity)
	{
		if (ConVar.Decay.upkeep)
		{
			return true;
		}
		if (this.isFoundation)
		{
			return true;
		}
		BuildingBlock buildingBlock = entity as BuildingBlock;
		return (buildingBlock ? buildingBlock.grade : BuildingGrade.Enum.Twigs) == BuildingGrade.Enum.Twigs;
	}

	// Token: 0x060020DA RID: 8410 RVA: 0x000D525B File Offset: 0x000D345B
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.isFoundation = name.Contains("foundation");
	}

	// Token: 0x04001973 RID: 6515
	private bool isFoundation;
}
