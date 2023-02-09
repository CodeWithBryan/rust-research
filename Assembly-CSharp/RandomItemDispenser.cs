using System;
using UnityEngine;

// Token: 0x02000436 RID: 1078
public class RandomItemDispenser : PrefabAttribute, IServerComponent
{
	// Token: 0x06002379 RID: 9081 RVA: 0x000E0E13 File Offset: 0x000DF013
	protected override Type GetIndexedType()
	{
		return typeof(RandomItemDispenser);
	}

	// Token: 0x0600237A RID: 9082 RVA: 0x000E0E20 File Offset: 0x000DF020
	public void DistributeItems(BasePlayer forPlayer, Vector3 distributorPosition)
	{
		foreach (RandomItemDispenser.RandomItemChance itemChance in this.Chances)
		{
			bool flag = this.TryAward(itemChance, forPlayer, distributorPosition);
			if (this.OnlyAwardOne && flag)
			{
				break;
			}
		}
	}

	// Token: 0x0600237B RID: 9083 RVA: 0x000E0E60 File Offset: 0x000DF060
	private bool TryAward(RandomItemDispenser.RandomItemChance itemChance, BasePlayer forPlayer, Vector3 distributorPosition)
	{
		float num = UnityEngine.Random.Range(0f, 1f);
		if (itemChance.Chance >= num)
		{
			Item item = ItemManager.Create(itemChance.Item, itemChance.Amount, 0UL);
			if (item != null)
			{
				if (forPlayer)
				{
					forPlayer.GiveItem(item, BaseEntity.GiveItemReason.ResourceHarvested);
				}
				else
				{
					item.Drop(distributorPosition + Vector3.up * 0.5f, Vector3.up, default(Quaternion));
				}
			}
			return true;
		}
		return false;
	}

	// Token: 0x04001C2F RID: 7215
	public RandomItemDispenser.RandomItemChance[] Chances;

	// Token: 0x04001C30 RID: 7216
	public bool OnlyAwardOne = true;

	// Token: 0x02000C96 RID: 3222
	[Serializable]
	public struct RandomItemChance
	{
		// Token: 0x0400432D RID: 17197
		public ItemDefinition Item;

		// Token: 0x0400432E RID: 17198
		public int Amount;

		// Token: 0x0400432F RID: 17199
		[Range(0f, 1f)]
		public float Chance;
	}
}
