using System;
using UnityEngine;

// Token: 0x020005B2 RID: 1458
public class ItemModConditionHasCondition : ItemMod
{
	// Token: 0x06002B8C RID: 11148 RVA: 0x001062C8 File Offset: 0x001044C8
	public override bool Passes(Item item)
	{
		if (!item.hasCondition)
		{
			return false;
		}
		if (this.conditionFractionTarget > 0f)
		{
			return (!this.lessThan && item.conditionNormalized > this.conditionFractionTarget) || (this.lessThan && item.conditionNormalized < this.conditionFractionTarget);
		}
		return (!this.lessThan && item.condition >= this.conditionTarget) || (this.lessThan && item.condition < this.conditionTarget);
	}

	// Token: 0x04002354 RID: 9044
	public float conditionTarget = 1f;

	// Token: 0x04002355 RID: 9045
	[Tooltip("If set to above 0 will check for fraction instead of raw value")]
	public float conditionFractionTarget = -1f;

	// Token: 0x04002356 RID: 9046
	public bool lessThan;
}
