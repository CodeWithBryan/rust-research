using System;
using System.Collections.Generic;
using System.Linq;
using Rust;
using UnityEngine;

// Token: 0x020003AF RID: 943
public class ResourceDispenser : EntityComponent<BaseEntity>, IServerComponent
{
	// Token: 0x06002062 RID: 8290 RVA: 0x000D33AC File Offset: 0x000D15AC
	public void Start()
	{
		this.Initialize();
	}

	// Token: 0x06002063 RID: 8291 RVA: 0x000D33B4 File Offset: 0x000D15B4
	public void Initialize()
	{
		this.CacheResourceTypeItems();
		this.UpdateFraction();
		this.UpdateRemainingCategories();
		this.CountAllItems();
	}

	// Token: 0x06002064 RID: 8292 RVA: 0x000D33D0 File Offset: 0x000D15D0
	private void CacheResourceTypeItems()
	{
		if (ResourceDispenser.cachedResourceItemTypes == null)
		{
			ResourceDispenser.cachedResourceItemTypes = new Dictionary<ResourceDispenser.GatherType, HashSet<int>>();
			HashSet<int> hashSet = new HashSet<int>();
			hashSet.Add(ItemManager.FindItemDefinition("wood").itemid);
			ResourceDispenser.cachedResourceItemTypes.Add(ResourceDispenser.GatherType.Tree, hashSet);
			HashSet<int> hashSet2 = new HashSet<int>();
			hashSet2.Add(ItemManager.FindItemDefinition("stones").itemid);
			hashSet2.Add(ItemManager.FindItemDefinition("sulfur.ore").itemid);
			hashSet2.Add(ItemManager.FindItemDefinition("metal.ore").itemid);
			hashSet2.Add(ItemManager.FindItemDefinition("hq.metal.ore").itemid);
			ResourceDispenser.cachedResourceItemTypes.Add(ResourceDispenser.GatherType.Ore, hashSet2);
		}
	}

	// Token: 0x06002065 RID: 8293 RVA: 0x000D3484 File Offset: 0x000D1684
	public void DoGather(HitInfo info)
	{
		if (!base.baseEntity.isServer)
		{
			return;
		}
		if (!info.CanGather || info.DidGather)
		{
			return;
		}
		if (this.gatherType == ResourceDispenser.GatherType.UNSET)
		{
			Debug.LogWarning("Object :" + base.gameObject.name + ": has unset gathertype!");
			return;
		}
		BaseMelee baseMelee = (info.Weapon == null) ? null : (info.Weapon as BaseMelee);
		float num;
		float num2;
		if (baseMelee != null)
		{
			ResourceDispenser.GatherPropertyEntry gatherInfoFromIndex = baseMelee.GetGatherInfoFromIndex(this.gatherType);
			num = gatherInfoFromIndex.gatherDamage * info.gatherScale;
			num2 = gatherInfoFromIndex.destroyFraction;
			if (num == 0f)
			{
				return;
			}
			baseMelee.SendPunch(new Vector3(UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(-0.25f, -0.5f), 0f) * -30f * (gatherInfoFromIndex.conditionLost / 6f), 0.05f);
			baseMelee.LoseCondition(gatherInfoFromIndex.conditionLost);
			if (!baseMelee.IsValid() || baseMelee.IsBroken())
			{
				return;
			}
			info.DidGather = true;
		}
		else
		{
			num = info.damageTypes.Total();
			num2 = 0.5f;
		}
		float num3 = this.fractionRemaining;
		this.GiveResources(info.InitiatorPlayer, num, num2, info.Weapon);
		this.UpdateFraction();
		float damageAmount;
		if (this.fractionRemaining <= 0f)
		{
			damageAmount = base.baseEntity.MaxHealth();
			if (info.DidGather && num2 < this.maxDestroyFractionForFinishBonus)
			{
				this.AssignFinishBonus(info.InitiatorPlayer, 1f - num2, info.Weapon);
			}
		}
		else
		{
			damageAmount = (num3 - this.fractionRemaining) * base.baseEntity.MaxHealth();
		}
		HitInfo hitInfo = new HitInfo(info.Initiator, base.baseEntity, DamageType.Generic, damageAmount, base.transform.position);
		hitInfo.gatherScale = 0f;
		hitInfo.PointStart = info.PointStart;
		hitInfo.PointEnd = info.PointEnd;
		hitInfo.WeaponPrefab = info.WeaponPrefab;
		hitInfo.Weapon = info.Weapon;
		base.baseEntity.OnAttacked(hitInfo);
	}

	// Token: 0x06002066 RID: 8294 RVA: 0x000D36B8 File Offset: 0x000D18B8
	public void AssignFinishBonus(BasePlayer player, float fraction, AttackEntity weapon)
	{
		base.SendMessage("FinishBonusAssigned", SendMessageOptions.DontRequireReceiver);
		if (fraction <= 0f)
		{
			return;
		}
		if (this.finishBonus != null)
		{
			foreach (ItemAmount itemAmount in this.finishBonus)
			{
				int num = Mathf.CeilToInt((float)((int)itemAmount.amount) * Mathf.Clamp01(fraction));
				int num2 = this.CalculateGatherBonus(player, itemAmount, (float)num);
				Item item = ItemManager.Create(itemAmount.itemDef, num + num2, 0UL);
				if (item != null)
				{
					player.GiveItem(item, BaseEntity.GiveItemReason.ResourceHarvested);
				}
			}
		}
	}

	// Token: 0x06002067 RID: 8295 RVA: 0x000D3760 File Offset: 0x000D1960
	public void OnAttacked(HitInfo info)
	{
		this.DoGather(info);
	}

	// Token: 0x06002068 RID: 8296 RVA: 0x000D376C File Offset: 0x000D196C
	private void GiveResources(BasePlayer entity, float gatherDamage, float destroyFraction, AttackEntity attackWeapon)
	{
		if (!entity.IsValid())
		{
			return;
		}
		if (gatherDamage <= 0f)
		{
			return;
		}
		ItemAmount itemAmount = null;
		int i = this.containedItems.Count;
		int num = UnityEngine.Random.Range(0, this.containedItems.Count);
		while (i > 0)
		{
			if (num >= this.containedItems.Count)
			{
				num = 0;
			}
			if (this.containedItems[num].amount > 0f)
			{
				itemAmount = this.containedItems[num];
				break;
			}
			num++;
			i--;
		}
		if (itemAmount == null)
		{
			return;
		}
		this.GiveResourceFromItem(entity, itemAmount, gatherDamage, destroyFraction, attackWeapon);
		this.UpdateVars();
		if (entity)
		{
			Debug.Assert(attackWeapon.GetItem() != null, "Attack Weapon " + attackWeapon + " has no Item");
			Debug.Assert(attackWeapon.ownerItemUID > 0U, "Attack Weapon " + attackWeapon + " ownerItemUID is 0");
			Debug.Assert(attackWeapon.GetParentEntity() != null, "Attack Weapon " + attackWeapon + " GetParentEntity is null");
			Debug.Assert(attackWeapon.GetParentEntity().IsValid(), "Attack Weapon " + attackWeapon + " GetParentEntity is not valid");
			Debug.Assert(attackWeapon.GetParentEntity().ToPlayer() != null, "Attack Weapon " + attackWeapon + " GetParentEntity is not a player");
			Debug.Assert(!attackWeapon.GetParentEntity().ToPlayer().IsDead(), "Attack Weapon " + attackWeapon + " GetParentEntity is not valid");
			BasePlayer ownerPlayer = attackWeapon.GetOwnerPlayer();
			Debug.Assert(ownerPlayer != null, "Attack Weapon " + attackWeapon + " ownerPlayer is null");
			Debug.Assert(ownerPlayer == entity, "Attack Weapon " + attackWeapon + " ownerPlayer is not player");
			if (ownerPlayer != null)
			{
				Debug.Assert(ownerPlayer.inventory != null, "Attack Weapon " + attackWeapon + " ownerPlayer inventory is null");
				Debug.Assert(ownerPlayer.inventory.FindItemUID(attackWeapon.ownerItemUID) != null, "Attack Weapon " + attackWeapon + " FindItemUID is null");
			}
		}
	}

	// Token: 0x06002069 RID: 8297 RVA: 0x000D3980 File Offset: 0x000D1B80
	public void DestroyFraction(float fraction)
	{
		foreach (ItemAmount itemAmount in this.containedItems)
		{
			if (itemAmount.amount > 0f)
			{
				itemAmount.amount -= fraction / this.categoriesRemaining;
			}
		}
		this.UpdateVars();
	}

	// Token: 0x0600206A RID: 8298 RVA: 0x000D39F4 File Offset: 0x000D1BF4
	private void GiveResourceFromItem(BasePlayer entity, ItemAmount itemAmt, float gatherDamage, float destroyFraction, AttackEntity attackWeapon)
	{
		if (itemAmt.amount == 0f)
		{
			return;
		}
		float num = Mathf.Min(gatherDamage, base.baseEntity.Health()) / base.baseEntity.MaxHealth();
		float num2 = itemAmt.startAmount / this.startingItemCounts;
		float num3 = Mathf.Clamp(itemAmt.startAmount * num / num2, 0f, itemAmt.amount);
		num3 = Mathf.Round(num3);
		float num4 = num3 * destroyFraction * 2f;
		if (itemAmt.amount <= num3 + num4)
		{
			float num5 = (num3 + num4) / itemAmt.amount;
			num3 /= num5;
			num4 /= num5;
		}
		itemAmt.amount -= Mathf.Floor(num3);
		itemAmt.amount -= Mathf.Floor(num4);
		if (num3 < 1f)
		{
			num3 = ((UnityEngine.Random.Range(0f, 1f) <= num3) ? 1f : 0f);
			itemAmt.amount = 0f;
		}
		if (itemAmt.amount < 0f)
		{
			itemAmt.amount = 0f;
		}
		if (num3 >= 1f)
		{
			int num6 = this.CalculateGatherBonus(entity, itemAmt, num3);
			int iAmount = Mathf.FloorToInt(num3) + num6;
			Item item = ItemManager.CreateByItemID(itemAmt.itemid, iAmount, 0UL);
			if (item == null)
			{
				return;
			}
			this.OverrideOwnership(item, attackWeapon);
			entity.GiveItem(item, BaseEntity.GiveItemReason.ResourceHarvested);
		}
	}

	// Token: 0x0600206B RID: 8299 RVA: 0x000D3B44 File Offset: 0x000D1D44
	private int CalculateGatherBonus(BaseEntity entity, ItemAmount item, float amountToGive)
	{
		if (entity == null)
		{
			return 0;
		}
		BasePlayer basePlayer = entity.ToPlayer();
		if (basePlayer == null)
		{
			return 0;
		}
		if (basePlayer.modifiers == null)
		{
			return 0;
		}
		amountToGive = (float)Mathf.FloorToInt(amountToGive);
		float num = 1f;
		ResourceDispenser.GatherType gatherType = this.gatherType;
		Modifier.ModifierType type;
		if (gatherType != ResourceDispenser.GatherType.Tree)
		{
			if (gatherType != ResourceDispenser.GatherType.Ore)
			{
				return 0;
			}
			type = Modifier.ModifierType.Ore_Yield;
		}
		else
		{
			type = Modifier.ModifierType.Wood_Yield;
		}
		if (!this.IsProducedItemOfGatherType(item))
		{
			return 0;
		}
		num += basePlayer.modifiers.GetValue(type, 0f);
		float num2 = basePlayer.modifiers.GetVariableValue(type, 0f);
		float num3 = (num > 1f) ? Mathf.Max(amountToGive * num - amountToGive, 0f) : 0f;
		num2 += num3;
		int num4 = 0;
		if (num2 >= 1f)
		{
			num4 = (int)num2;
			num2 -= (float)num4;
		}
		basePlayer.modifiers.SetVariableValue(type, num2);
		return num4;
	}

	// Token: 0x0600206C RID: 8300 RVA: 0x000D3C24 File Offset: 0x000D1E24
	private bool IsProducedItemOfGatherType(ItemAmount item)
	{
		if (this.gatherType == ResourceDispenser.GatherType.Tree)
		{
			return ResourceDispenser.cachedResourceItemTypes[ResourceDispenser.GatherType.Tree].Contains(item.itemid);
		}
		return this.gatherType == ResourceDispenser.GatherType.Ore && ResourceDispenser.cachedResourceItemTypes[ResourceDispenser.GatherType.Ore].Contains(item.itemid);
	}

	// Token: 0x0600206D RID: 8301 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool OverrideOwnership(Item item, AttackEntity weapon)
	{
		return false;
	}

	// Token: 0x0600206E RID: 8302 RVA: 0x000D3C71 File Offset: 0x000D1E71
	private void UpdateVars()
	{
		this.UpdateFraction();
		this.UpdateRemainingCategories();
	}

	// Token: 0x0600206F RID: 8303 RVA: 0x000D3C80 File Offset: 0x000D1E80
	public void UpdateRemainingCategories()
	{
		int num = 0;
		using (List<ItemAmount>.Enumerator enumerator = this.containedItems.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.amount > 0f)
				{
					num++;
				}
			}
		}
		this.categoriesRemaining = (float)num;
	}

	// Token: 0x06002070 RID: 8304 RVA: 0x000D3CE8 File Offset: 0x000D1EE8
	public void CountAllItems()
	{
		this.startingItemCounts = this.containedItems.Sum((ItemAmount x) => x.startAmount);
	}

	// Token: 0x06002071 RID: 8305 RVA: 0x000D3D1C File Offset: 0x000D1F1C
	private void UpdateFraction()
	{
		float num = this.containedItems.Sum((ItemAmount x) => x.startAmount);
		float num2 = this.containedItems.Sum((ItemAmount x) => x.amount);
		if (num == 0f)
		{
			this.fractionRemaining = 0f;
			return;
		}
		this.fractionRemaining = num2 / num;
	}

	// Token: 0x0400193E RID: 6462
	public ResourceDispenser.GatherType gatherType = ResourceDispenser.GatherType.UNSET;

	// Token: 0x0400193F RID: 6463
	public List<ItemAmount> containedItems;

	// Token: 0x04001940 RID: 6464
	public float maxDestroyFractionForFinishBonus = 0.2f;

	// Token: 0x04001941 RID: 6465
	public List<ItemAmount> finishBonus;

	// Token: 0x04001942 RID: 6466
	public float fractionRemaining = 1f;

	// Token: 0x04001943 RID: 6467
	private float categoriesRemaining;

	// Token: 0x04001944 RID: 6468
	private float startingItemCounts;

	// Token: 0x04001945 RID: 6469
	private static Dictionary<ResourceDispenser.GatherType, HashSet<int>> cachedResourceItemTypes;

	// Token: 0x02000C6D RID: 3181
	public enum GatherType
	{
		// Token: 0x0400424B RID: 16971
		Tree,
		// Token: 0x0400424C RID: 16972
		Ore,
		// Token: 0x0400424D RID: 16973
		Flesh,
		// Token: 0x0400424E RID: 16974
		UNSET,
		// Token: 0x0400424F RID: 16975
		LAST
	}

	// Token: 0x02000C6E RID: 3182
	[Serializable]
	public class GatherPropertyEntry
	{
		// Token: 0x04004250 RID: 16976
		public float gatherDamage;

		// Token: 0x04004251 RID: 16977
		public float destroyFraction;

		// Token: 0x04004252 RID: 16978
		public float conditionLost;
	}

	// Token: 0x02000C6F RID: 3183
	[Serializable]
	public class GatherProperties
	{
		// Token: 0x06004CEC RID: 19692 RVA: 0x00196B34 File Offset: 0x00194D34
		public float GetProficiency()
		{
			float num = 0f;
			for (int i = 0; i < 3; i++)
			{
				ResourceDispenser.GatherPropertyEntry fromIndex = this.GetFromIndex(i);
				float num2 = fromIndex.gatherDamage * fromIndex.destroyFraction;
				if (num2 > 0f)
				{
					num += fromIndex.gatherDamage / num2;
				}
			}
			return num;
		}

		// Token: 0x06004CED RID: 19693 RVA: 0x00196B80 File Offset: 0x00194D80
		public bool Any()
		{
			for (int i = 0; i < 3; i++)
			{
				ResourceDispenser.GatherPropertyEntry fromIndex = this.GetFromIndex(i);
				if (fromIndex.gatherDamage > 0f || fromIndex.conditionLost > 0f)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004CEE RID: 19694 RVA: 0x00196BBE File Offset: 0x00194DBE
		public ResourceDispenser.GatherPropertyEntry GetFromIndex(int index)
		{
			return this.GetFromIndex((ResourceDispenser.GatherType)index);
		}

		// Token: 0x06004CEF RID: 19695 RVA: 0x00196BC7 File Offset: 0x00194DC7
		public ResourceDispenser.GatherPropertyEntry GetFromIndex(ResourceDispenser.GatherType index)
		{
			switch (index)
			{
			case ResourceDispenser.GatherType.Tree:
				return this.Tree;
			case ResourceDispenser.GatherType.Ore:
				return this.Ore;
			case ResourceDispenser.GatherType.Flesh:
				return this.Flesh;
			default:
				return null;
			}
		}

		// Token: 0x04004253 RID: 16979
		public ResourceDispenser.GatherPropertyEntry Tree;

		// Token: 0x04004254 RID: 16980
		public ResourceDispenser.GatherPropertyEntry Ore;

		// Token: 0x04004255 RID: 16981
		public ResourceDispenser.GatherPropertyEntry Flesh;
	}
}
