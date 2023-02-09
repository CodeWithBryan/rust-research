using System;
using ConVar;

// Token: 0x020003C2 RID: 962
public class BuildingGradeDecay : global::Decay
{
	// Token: 0x060020DC RID: 8412 RVA: 0x000D5276 File Offset: 0x000D3476
	public override float GetDecayDelay(BaseEntity entity)
	{
		return base.GetDecayDelay(this.decayGrade);
	}

	// Token: 0x060020DD RID: 8413 RVA: 0x000D5284 File Offset: 0x000D3484
	public override float GetDecayDuration(BaseEntity entity)
	{
		return base.GetDecayDuration(this.decayGrade);
	}

	// Token: 0x060020DE RID: 8414 RVA: 0x000D5292 File Offset: 0x000D3492
	public override bool ShouldDecay(BaseEntity entity)
	{
		return ConVar.Decay.upkeep || entity.IsOutside();
	}

	// Token: 0x04001974 RID: 6516
	public BuildingGrade.Enum decayGrade;
}
