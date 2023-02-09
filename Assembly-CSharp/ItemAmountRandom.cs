using System;
using UnityEngine;

// Token: 0x020005DB RID: 1499
[Serializable]
public class ItemAmountRandom
{
	// Token: 0x06002C12 RID: 11282 RVA: 0x001087C2 File Offset: 0x001069C2
	public int RandomAmount()
	{
		return Mathf.RoundToInt(this.amount.Evaluate(UnityEngine.Random.Range(0f, 1f)));
	}

	// Token: 0x040023F2 RID: 9202
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemDef;

	// Token: 0x040023F3 RID: 9203
	public AnimationCurve amount = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});
}
