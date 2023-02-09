using System;
using Facepunch.Rust;
using Rust;
using UnityEngine;

// Token: 0x020005B8 RID: 1464
[RequireComponent(typeof(ItemModConsumable))]
public class ItemModConsume : ItemMod
{
	// Token: 0x06002B99 RID: 11161 RVA: 0x001064DF File Offset: 0x001046DF
	public virtual ItemModConsumable GetConsumable()
	{
		if (this.primaryConsumable)
		{
			return this.primaryConsumable;
		}
		return base.GetComponent<ItemModConsumable>();
	}

	// Token: 0x06002B9A RID: 11162 RVA: 0x001064FB File Offset: 0x001046FB
	public virtual GameObjectRef GetConsumeEffect()
	{
		return this.consumeEffect;
	}

	// Token: 0x06002B9B RID: 11163 RVA: 0x00106504 File Offset: 0x00104704
	public override void DoAction(Item item, BasePlayer player)
	{
		if (item.amount < 1)
		{
			return;
		}
		GameObjectRef gameObjectRef = this.GetConsumeEffect();
		if (gameObjectRef.isValid)
		{
			Vector3 posLocal = player.IsDucked() ? new Vector3(0f, 1f, 0f) : new Vector3(0f, 2f, 0f);
			Effect.server.Run(gameObjectRef.resourcePath, player, 0U, posLocal, Vector3.zero, null, false);
		}
		player.metabolism.MarkConsumption();
		ItemModConsumable consumable = this.GetConsumable();
		if (!string.IsNullOrEmpty(consumable.achievementWhenEaten))
		{
			player.GiveAchievement(consumable.achievementWhenEaten);
		}
		float num = (float)Mathf.Max(consumable.amountToConsume, 1);
		float num2 = Mathf.Min((float)item.amount, num);
		float num3 = num2 / num;
		float num4 = item.conditionNormalized;
		if (consumable.conditionFractionToLose > 0f)
		{
			num4 = consumable.conditionFractionToLose;
		}
		foreach (ItemModConsumable.ConsumableEffect consumableEffect in consumable.effects)
		{
			if (Mathf.Clamp01(player.healthFraction + player.metabolism.pending_health.Fraction()) <= consumableEffect.onlyIfHealthLessThan)
			{
				if (consumableEffect.type == MetabolismAttribute.Type.Health)
				{
					if (consumableEffect.amount < 0f)
					{
						player.OnAttacked(new HitInfo(player, player, DamageType.Generic, -consumableEffect.amount * num3 * num4, player.transform.position + player.transform.forward * 1f));
					}
					else
					{
						player.health += consumableEffect.amount * num3 * num4;
					}
				}
				else
				{
					player.metabolism.ApplyChange(consumableEffect.type, consumableEffect.amount * num3 * num4, consumableEffect.time * num3 * num4);
				}
			}
		}
		if (player.modifiers != null)
		{
			player.modifiers.Add(consumable.modifiers);
		}
		if (this.product != null)
		{
			foreach (ItemAmountRandom itemAmountRandom in this.product)
			{
				int num5 = Mathf.RoundToInt((float)itemAmountRandom.RandomAmount() * num4);
				if (num5 > 0)
				{
					Item item2 = ItemManager.Create(itemAmountRandom.itemDef, num5, 0UL);
					player.GiveItem(item2, BaseEntity.GiveItemReason.Generic);
				}
			}
		}
		if (string.IsNullOrEmpty(this.eatGesture))
		{
			player.SignalBroadcast(BaseEntity.Signal.Gesture, this.eatGesture, null);
		}
		Analytics.Server.Consume(base.gameObject.name);
		if (consumable.conditionFractionToLose > 0f)
		{
			item.LoseCondition(consumable.conditionFractionToLose * item.maxCondition);
			return;
		}
		item.UseItem((int)num2);
	}

	// Token: 0x06002B9C RID: 11164 RVA: 0x001067C0 File Offset: 0x001049C0
	public override bool CanDoAction(Item item, BasePlayer player)
	{
		return player.metabolism.CanConsume();
	}

	// Token: 0x04002362 RID: 9058
	public GameObjectRef consumeEffect;

	// Token: 0x04002363 RID: 9059
	public string eatGesture = "eat_2hand";

	// Token: 0x04002364 RID: 9060
	[Tooltip("Items that are given on consumption of this item")]
	public ItemAmountRandom[] product;

	// Token: 0x04002365 RID: 9061
	public ItemModConsumable primaryConsumable;
}
