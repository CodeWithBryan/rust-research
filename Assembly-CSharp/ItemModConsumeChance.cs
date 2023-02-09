using System;
using UnityEngine;

// Token: 0x020005B9 RID: 1465
public class ItemModConsumeChance : ItemModConsume
{
	// Token: 0x06002B9E RID: 11166 RVA: 0x001067E0 File Offset: 0x001049E0
	private bool GetChance()
	{
		UnityEngine.Random.State state = UnityEngine.Random.state;
		UnityEngine.Random.InitState(Time.frameCount);
		bool result = UnityEngine.Random.Range(0f, 1f) <= this.chanceForSecondaryConsume;
		UnityEngine.Random.state = state;
		return result;
	}

	// Token: 0x06002B9F RID: 11167 RVA: 0x0010681D File Offset: 0x00104A1D
	public override ItemModConsumable GetConsumable()
	{
		if (this.GetChance())
		{
			return this.secondaryConsumable;
		}
		return base.GetConsumable();
	}

	// Token: 0x06002BA0 RID: 11168 RVA: 0x00106834 File Offset: 0x00104A34
	public override GameObjectRef GetConsumeEffect()
	{
		if (this.GetChance())
		{
			return this.secondaryConsumeEffect;
		}
		return base.GetConsumeEffect();
	}

	// Token: 0x04002366 RID: 9062
	public float chanceForSecondaryConsume = 0.5f;

	// Token: 0x04002367 RID: 9063
	public GameObjectRef secondaryConsumeEffect;

	// Token: 0x04002368 RID: 9064
	public ItemModConsumable secondaryConsumable;
}
