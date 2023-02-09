using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005B7 RID: 1463
public class ItemModConsumable : MonoBehaviour
{
	// Token: 0x06002B97 RID: 11159 RVA: 0x0010646C File Offset: 0x0010466C
	public float GetIfType(MetabolismAttribute.Type typeToPick)
	{
		for (int i = 0; i < this.effects.Count; i++)
		{
			if (this.effects[i].type == typeToPick)
			{
				return this.effects[i].amount;
			}
		}
		return 0f;
	}

	// Token: 0x0400235D RID: 9053
	public int amountToConsume = 1;

	// Token: 0x0400235E RID: 9054
	public float conditionFractionToLose;

	// Token: 0x0400235F RID: 9055
	public string achievementWhenEaten;

	// Token: 0x04002360 RID: 9056
	public List<ItemModConsumable.ConsumableEffect> effects = new List<ItemModConsumable.ConsumableEffect>();

	// Token: 0x04002361 RID: 9057
	public List<ModifierDefintion> modifiers = new List<ModifierDefintion>();

	// Token: 0x02000D23 RID: 3363
	[Serializable]
	public class ConsumableEffect
	{
		// Token: 0x04004531 RID: 17713
		public MetabolismAttribute.Type type;

		// Token: 0x04004532 RID: 17714
		public float amount;

		// Token: 0x04004533 RID: 17715
		public float time;

		// Token: 0x04004534 RID: 17716
		public float onlyIfHealthLessThan = 1f;
	}
}
