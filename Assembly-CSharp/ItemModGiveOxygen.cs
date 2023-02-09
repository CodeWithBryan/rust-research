using System;
using UnityEngine;

// Token: 0x020005C4 RID: 1476
public class ItemModGiveOxygen : ItemMod, IAirSupply
{
	// Token: 0x17000369 RID: 873
	// (get) Token: 0x06002BC8 RID: 11208 RVA: 0x00107269 File Offset: 0x00105469
	public ItemModGiveOxygen.AirSupplyType AirType
	{
		get
		{
			return this.airType;
		}
	}

	// Token: 0x06002BC9 RID: 11209 RVA: 0x00107271 File Offset: 0x00105471
	public float GetAirTimeRemaining()
	{
		return this.timeRemaining;
	}

	// Token: 0x06002BCA RID: 11210 RVA: 0x0010727C File Offset: 0x0010547C
	public override void ModInit()
	{
		base.ModInit();
		this.cycleTime = 1f;
		ItemMod[] siblingMods = this.siblingMods;
		for (int i = 0; i < siblingMods.Length; i++)
		{
			ItemModCycle itemModCycle;
			if ((itemModCycle = (siblingMods[i] as ItemModCycle)) != null)
			{
				this.cycleTime = itemModCycle.timeBetweenCycles;
			}
		}
	}

	// Token: 0x06002BCB RID: 11211 RVA: 0x001072C8 File Offset: 0x001054C8
	public override void DoAction(Item item, BasePlayer player)
	{
		if (!item.hasCondition)
		{
			return;
		}
		if (item.conditionNormalized == 0f)
		{
			return;
		}
		if (player == null)
		{
			return;
		}
		float num = Mathf.Clamp01(0.525f);
		if (player.AirFactor() > num)
		{
			return;
		}
		if (item.parent == null)
		{
			return;
		}
		if (item.parent != player.inventory.containerWear)
		{
			return;
		}
		Effect.server.Run((!this.inhaled) ? this.inhaleEffect.resourcePath : this.exhaleEffect.resourcePath, player, StringPool.Get("jaw"), Vector3.zero, Vector3.forward, null, false);
		this.inhaled = !this.inhaled;
		if (!this.inhaled && WaterLevel.GetWaterDepth(player.eyes.position, player, null) > 3f)
		{
			Effect.server.Run(this.bubblesEffect.resourcePath, player, StringPool.Get("jaw"), Vector3.zero, Vector3.forward, null, false);
		}
		item.LoseCondition((float)this.amountToConsume);
		player.metabolism.oxygen.Add(1f);
	}

	// Token: 0x06002BCC RID: 11212 RVA: 0x001073E2 File Offset: 0x001055E2
	public override void OnChanged(Item item)
	{
		if (item.hasCondition)
		{
			this.timeRemaining = item.condition * ((float)this.amountToConsume / this.cycleTime);
			return;
		}
		this.timeRemaining = 0f;
	}

	// Token: 0x04002391 RID: 9105
	public ItemModGiveOxygen.AirSupplyType airType = ItemModGiveOxygen.AirSupplyType.ScubaTank;

	// Token: 0x04002392 RID: 9106
	public int amountToConsume = 1;

	// Token: 0x04002393 RID: 9107
	public GameObjectRef inhaleEffect;

	// Token: 0x04002394 RID: 9108
	public GameObjectRef exhaleEffect;

	// Token: 0x04002395 RID: 9109
	public GameObjectRef bubblesEffect;

	// Token: 0x04002396 RID: 9110
	private float timeRemaining;

	// Token: 0x04002397 RID: 9111
	private float cycleTime;

	// Token: 0x04002398 RID: 9112
	private bool inhaled;

	// Token: 0x02000D26 RID: 3366
	public enum AirSupplyType
	{
		// Token: 0x0400453A RID: 17722
		Lungs,
		// Token: 0x0400453B RID: 17723
		ScubaTank,
		// Token: 0x0400453C RID: 17724
		Submarine
	}
}
