using System;
using System.Collections.Generic;
using System.Linq;
using Rust;
using UnityEngine;

// Token: 0x02000443 RID: 1091
public class WildlifeTrap : StorageContainer
{
	// Token: 0x060023C9 RID: 9161 RVA: 0x00020A80 File Offset: 0x0001EC80
	public bool HasCatch()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x060023CA RID: 9162 RVA: 0x0002782C File Offset: 0x00025A2C
	public bool IsTrapActive()
	{
		return base.HasFlag(BaseEntity.Flags.On);
	}

	// Token: 0x060023CB RID: 9163 RVA: 0x0008467C File Offset: 0x0008287C
	public override void ResetState()
	{
		base.ResetState();
	}

	// Token: 0x060023CC RID: 9164 RVA: 0x000E2414 File Offset: 0x000E0614
	public void SetTrapActive(bool trapOn)
	{
		if (trapOn == this.IsTrapActive())
		{
			return;
		}
		base.CancelInvoke(new Action(this.TrapThink));
		base.SetFlag(BaseEntity.Flags.On, trapOn, false, true);
		if (trapOn)
		{
			base.InvokeRepeating(new Action(this.TrapThink), this.tickRate * 0.8f + this.tickRate * UnityEngine.Random.Range(0f, 0.4f), this.tickRate);
		}
	}

	// Token: 0x060023CD RID: 9165 RVA: 0x000E2488 File Offset: 0x000E0688
	public int GetBaitCalories()
	{
		int num = 0;
		foreach (Item item in base.inventory.itemList)
		{
			ItemModConsumable component = item.info.GetComponent<ItemModConsumable>();
			if (!(component == null) && !this.ignoreBait.Contains(item.info))
			{
				foreach (ItemModConsumable.ConsumableEffect consumableEffect in component.effects)
				{
					if (consumableEffect.type == MetabolismAttribute.Type.Calories && consumableEffect.amount > 0f)
					{
						num += Mathf.CeilToInt(consumableEffect.amount * (float)item.amount);
					}
				}
			}
		}
		return num;
	}

	// Token: 0x060023CE RID: 9166 RVA: 0x000E2578 File Offset: 0x000E0778
	public void DestroyRandomFoodItem()
	{
		int count = base.inventory.itemList.Count;
		int num = UnityEngine.Random.Range(0, count);
		for (int i = 0; i < count; i++)
		{
			int num2 = num + i;
			if (num2 >= count)
			{
				num2 -= count;
			}
			Item item = base.inventory.itemList[num2];
			if (item != null && !(item.info.GetComponent<ItemModConsumable>() == null))
			{
				item.UseItem(1);
				return;
			}
		}
	}

	// Token: 0x060023CF RID: 9167 RVA: 0x000E25EC File Offset: 0x000E07EC
	public void UseBaitCalories(int numToUse)
	{
		foreach (Item item in base.inventory.itemList)
		{
			int itemCalories = this.GetItemCalories(item);
			if (itemCalories > 0)
			{
				numToUse -= itemCalories;
				item.UseItem(1);
				if (numToUse <= 0)
				{
					break;
				}
			}
		}
	}

	// Token: 0x060023D0 RID: 9168 RVA: 0x000E265C File Offset: 0x000E085C
	public int GetItemCalories(Item item)
	{
		ItemModConsumable component = item.info.GetComponent<ItemModConsumable>();
		if (component == null)
		{
			return 0;
		}
		foreach (ItemModConsumable.ConsumableEffect consumableEffect in component.effects)
		{
			if (consumableEffect.type == MetabolismAttribute.Type.Calories && consumableEffect.amount > 0f)
			{
				return Mathf.CeilToInt(consumableEffect.amount);
			}
		}
		return 0;
	}

	// Token: 0x060023D1 RID: 9169 RVA: 0x000E26E8 File Offset: 0x000E08E8
	public virtual void TrapThink()
	{
		int baitCalories = this.GetBaitCalories();
		if (baitCalories <= 0)
		{
			return;
		}
		TrappableWildlife randomWildlife = this.GetRandomWildlife();
		if (baitCalories >= randomWildlife.caloriesForInterest && UnityEngine.Random.Range(0f, 1f) <= randomWildlife.successRate)
		{
			this.UseBaitCalories(randomWildlife.caloriesForInterest);
			if (UnityEngine.Random.Range(0f, 1f) <= this.trapSuccessRate)
			{
				this.TrapWildlife(randomWildlife);
			}
		}
	}

	// Token: 0x060023D2 RID: 9170 RVA: 0x000E2754 File Offset: 0x000E0954
	public void TrapWildlife(TrappableWildlife trapped)
	{
		Item item = ItemManager.Create(trapped.inventoryObject, UnityEngine.Random.Range(trapped.minToCatch, trapped.maxToCatch + 1), 0UL);
		if (!item.MoveToContainer(base.inventory, -1, true, false, null, true))
		{
			item.Remove(0f);
		}
		else
		{
			base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
		}
		this.SetTrapActive(false);
		base.Hurt(this.StartMaxHealth() * 0.1f, DamageType.Decay, null, false);
	}

	// Token: 0x060023D3 RID: 9171 RVA: 0x0006EB88 File Offset: 0x0006CD88
	public void ClearTrap()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
	}

	// Token: 0x060023D4 RID: 9172 RVA: 0x000E27CD File Offset: 0x000E09CD
	public bool HasBait()
	{
		return this.GetBaitCalories() > 0;
	}

	// Token: 0x060023D5 RID: 9173 RVA: 0x000E27D8 File Offset: 0x000E09D8
	public override void PlayerStoppedLooting(BasePlayer player)
	{
		this.SetTrapActive(this.HasBait());
		this.ClearTrap();
		base.PlayerStoppedLooting(player);
	}

	// Token: 0x060023D6 RID: 9174 RVA: 0x000E27F3 File Offset: 0x000E09F3
	public override bool OnStartBeingLooted(BasePlayer baseEntity)
	{
		this.ClearTrap();
		return base.OnStartBeingLooted(baseEntity);
	}

	// Token: 0x060023D7 RID: 9175 RVA: 0x000E2804 File Offset: 0x000E0A04
	public TrappableWildlife GetRandomWildlife()
	{
		int num = this.targetWildlife.Sum((WildlifeTrap.WildlifeWeight x) => x.weight);
		int num2 = UnityEngine.Random.Range(0, num);
		for (int i = 0; i < this.targetWildlife.Count; i++)
		{
			num -= this.targetWildlife[i].weight;
			if (num2 >= num)
			{
				return this.targetWildlife[i].wildlife;
			}
		}
		return null;
	}

	// Token: 0x04001C6A RID: 7274
	public float tickRate = 60f;

	// Token: 0x04001C6B RID: 7275
	public GameObjectRef trappedEffect;

	// Token: 0x04001C6C RID: 7276
	public float trappedEffectRepeatRate = 30f;

	// Token: 0x04001C6D RID: 7277
	public float trapSuccessRate = 0.5f;

	// Token: 0x04001C6E RID: 7278
	public List<ItemDefinition> ignoreBait;

	// Token: 0x04001C6F RID: 7279
	public List<WildlifeTrap.WildlifeWeight> targetWildlife;

	// Token: 0x02000C9E RID: 3230
	public static class WildlifeTrapFlags
	{
		// Token: 0x0400433D RID: 17213
		public const BaseEntity.Flags Occupied = BaseEntity.Flags.Reserved1;
	}

	// Token: 0x02000C9F RID: 3231
	[Serializable]
	public class WildlifeWeight
	{
		// Token: 0x0400433E RID: 17214
		public TrappableWildlife wildlife;

		// Token: 0x0400433F RID: 17215
		public int weight;
	}
}
