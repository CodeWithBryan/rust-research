using System;
using UnityEngine;

// Token: 0x020005DC RID: 1500
[Serializable]
public class ItemAmountRanged : ItemAmount
{
	// Token: 0x06002C14 RID: 11284 RVA: 0x00108834 File Offset: 0x00106A34
	public override void OnAfterDeserialize()
	{
		base.OnAfterDeserialize();
	}

	// Token: 0x06002C15 RID: 11285 RVA: 0x0010883C File Offset: 0x00106A3C
	public ItemAmountRanged(ItemDefinition item = null, float amt = 0f, float max = -1f) : base(item, amt)
	{
		this.maxAmount = max;
	}

	// Token: 0x06002C16 RID: 11286 RVA: 0x00108858 File Offset: 0x00106A58
	public override float GetAmount()
	{
		if (this.maxAmount > 0f && this.maxAmount > this.amount)
		{
			return UnityEngine.Random.Range(this.amount, this.maxAmount);
		}
		return this.amount;
	}

	// Token: 0x040023F4 RID: 9204
	public float maxAmount = -1f;
}
