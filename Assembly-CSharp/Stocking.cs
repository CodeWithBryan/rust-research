using System;
using Rust;
using UnityEngine;

// Token: 0x02000186 RID: 390
public class Stocking : LootContainer
{
	// Token: 0x060016F9 RID: 5881 RVA: 0x000ACE6B File Offset: 0x000AB06B
	public override void ServerInit()
	{
		base.ServerInit();
		if (Stocking.stockings == null)
		{
			Stocking.stockings = new ListHashSet<Stocking>(8);
		}
		Stocking.stockings.Add(this);
	}

	// Token: 0x060016FA RID: 5882 RVA: 0x000ACE90 File Offset: 0x000AB090
	internal override void DoServerDestroy()
	{
		Stocking.stockings.Remove(this);
		base.DoServerDestroy();
	}

	// Token: 0x060016FB RID: 5883 RVA: 0x000ACEA4 File Offset: 0x000AB0A4
	public bool IsEmpty()
	{
		if (base.inventory == null)
		{
			return false;
		}
		for (int i = base.inventory.itemList.Count - 1; i >= 0; i--)
		{
			if (base.inventory.itemList[i] != null)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060016FC RID: 5884 RVA: 0x000ACEF0 File Offset: 0x000AB0F0
	public override void SpawnLoot()
	{
		if (base.inventory == null)
		{
			Debug.Log("CONTACT DEVELOPERS! Stocking::PopulateLoot has null inventory!!! " + base.name);
			return;
		}
		if (this.IsEmpty())
		{
			base.SpawnLoot();
			base.SetFlag(BaseEntity.Flags.On, true, false, true);
			base.Hurt(this.MaxHealth() * 0.1f, DamageType.Generic, null, false);
		}
	}

	// Token: 0x060016FD RID: 5885 RVA: 0x000ACF48 File Offset: 0x000AB148
	public override void PlayerStoppedLooting(BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		if (this.IsEmpty() && base.healthFraction <= 0.1f)
		{
			base.Hurt(base.health, DamageType.Generic, this, false);
		}
	}

	// Token: 0x04001013 RID: 4115
	public static ListHashSet<Stocking> stockings;
}
