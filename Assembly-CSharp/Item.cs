using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200059A RID: 1434
public class Item
{
	// Token: 0x17000346 RID: 838
	// (get) Token: 0x06002AB0 RID: 10928 RVA: 0x00102793 File Offset: 0x00100993
	// (set) Token: 0x06002AAF RID: 10927 RVA: 0x0010274C File Offset: 0x0010094C
	public float condition
	{
		get
		{
			return this._condition;
		}
		set
		{
			float condition = this._condition;
			this._condition = Mathf.Clamp(value, 0f, this.maxCondition);
			if (this.isServer && Mathf.Ceil(value) != Mathf.Ceil(condition))
			{
				this.MarkDirty();
			}
		}
	}

	// Token: 0x17000347 RID: 839
	// (get) Token: 0x06002AB2 RID: 10930 RVA: 0x001027CC File Offset: 0x001009CC
	// (set) Token: 0x06002AB1 RID: 10929 RVA: 0x0010279B File Offset: 0x0010099B
	public float maxCondition
	{
		get
		{
			return this._maxCondition;
		}
		set
		{
			this._maxCondition = Mathf.Clamp(value, 0f, this.info.condition.max);
			if (this.isServer)
			{
				this.MarkDirty();
			}
		}
	}

	// Token: 0x17000348 RID: 840
	// (get) Token: 0x06002AB3 RID: 10931 RVA: 0x001027D4 File Offset: 0x001009D4
	public float maxConditionNormalized
	{
		get
		{
			return this._maxCondition / this.info.condition.max;
		}
	}

	// Token: 0x17000349 RID: 841
	// (get) Token: 0x06002AB4 RID: 10932 RVA: 0x001027ED File Offset: 0x001009ED
	// (set) Token: 0x06002AB5 RID: 10933 RVA: 0x0010280A File Offset: 0x00100A0A
	public float conditionNormalized
	{
		get
		{
			if (!this.hasCondition)
			{
				return 1f;
			}
			return this.condition / this.maxCondition;
		}
		set
		{
			if (!this.hasCondition)
			{
				return;
			}
			this.condition = value * this.maxCondition;
		}
	}

	// Token: 0x1700034A RID: 842
	// (get) Token: 0x06002AB6 RID: 10934 RVA: 0x00102823 File Offset: 0x00100A23
	public bool hasCondition
	{
		get
		{
			return this.info != null && this.info.condition.enabled && this.info.condition.max > 0f;
		}
	}

	// Token: 0x1700034B RID: 843
	// (get) Token: 0x06002AB7 RID: 10935 RVA: 0x0010285E File Offset: 0x00100A5E
	public bool isBroken
	{
		get
		{
			return this.hasCondition && this.condition <= 0f;
		}
	}

	// Token: 0x06002AB8 RID: 10936 RVA: 0x0010287C File Offset: 0x00100A7C
	public void LoseCondition(float amount)
	{
		if (!this.hasCondition)
		{
			return;
		}
		if (Debugging.disablecondition)
		{
			return;
		}
		float condition = this.condition;
		this.condition -= amount;
		if (ConVar.Global.developer > 0)
		{
			Debug.Log(string.Concat(new object[]
			{
				this.info.shortname,
				" was damaged by: ",
				amount,
				"cond is: ",
				this.condition,
				"/",
				this.maxCondition
			}));
		}
		if (this.condition <= 0f && this.condition < condition)
		{
			this.OnBroken();
		}
	}

	// Token: 0x06002AB9 RID: 10937 RVA: 0x0010292E File Offset: 0x00100B2E
	public void RepairCondition(float amount)
	{
		if (!this.hasCondition)
		{
			return;
		}
		this.condition += amount;
	}

	// Token: 0x06002ABA RID: 10938 RVA: 0x00102948 File Offset: 0x00100B48
	public void DoRepair(float maxLossFraction)
	{
		if (!this.hasCondition)
		{
			return;
		}
		if (this.info.condition.maintainMaxCondition)
		{
			maxLossFraction = 0f;
		}
		float num = 1f - this.condition / this.maxCondition;
		maxLossFraction = Mathf.Clamp(maxLossFraction, 0f, this.info.condition.max);
		this.maxCondition *= 1f - maxLossFraction * num;
		this.condition = this.maxCondition;
		global::BaseEntity baseEntity = this.GetHeldEntity();
		if (baseEntity != null)
		{
			baseEntity.SetFlag(global::BaseEntity.Flags.Broken, false, false, true);
		}
		if (ConVar.Global.developer > 0)
		{
			Debug.Log(string.Concat(new object[]
			{
				this.info.shortname,
				" was repaired! new cond is: ",
				this.condition,
				"/",
				this.maxCondition
			}));
		}
	}

	// Token: 0x06002ABB RID: 10939 RVA: 0x00102A3C File Offset: 0x00100C3C
	public global::ItemContainer GetRootContainer()
	{
		global::ItemContainer itemContainer = this.parent;
		int num = 0;
		while (itemContainer != null && num <= 8 && itemContainer.parent != null && itemContainer.parent.parent != null)
		{
			itemContainer = itemContainer.parent.parent;
			num++;
		}
		if (num == 8)
		{
			Debug.LogWarning("GetRootContainer failed with 8 iterations");
		}
		return itemContainer;
	}

	// Token: 0x06002ABC RID: 10940 RVA: 0x00102A90 File Offset: 0x00100C90
	public virtual void OnBroken()
	{
		if (!this.hasCondition)
		{
			return;
		}
		global::BaseEntity baseEntity = this.GetHeldEntity();
		if (baseEntity != null)
		{
			baseEntity.SetFlag(global::BaseEntity.Flags.Broken, true, false, true);
		}
		global::BasePlayer ownerPlayer = this.GetOwnerPlayer();
		if (ownerPlayer)
		{
			if (ownerPlayer.GetActiveItem() == this)
			{
				Effect.server.Run("assets/bundled/prefabs/fx/item_break.prefab", ownerPlayer, 0U, Vector3.zero, Vector3.zero, null, false);
				ownerPlayer.ChatMessage("Your active item was broken!");
			}
			ItemModWearable itemModWearable;
			if (this.info.TryGetComponent<ItemModWearable>(out itemModWearable) && ownerPlayer.inventory.containerWear.itemList.Contains(this))
			{
				if (itemModWearable.breakEffect.isValid)
				{
					Effect.server.Run(itemModWearable.breakEffect.resourcePath, ownerPlayer, 0U, Vector3.zero, Vector3.zero, null, false);
				}
				else
				{
					Effect.server.Run("assets/bundled/prefabs/fx/armor_break.prefab", ownerPlayer, 0U, Vector3.zero, Vector3.zero, null, false);
				}
			}
		}
		if ((!this.info.condition.repairable && !this.info.GetComponent<ItemModRepair>()) || this.maxCondition <= 5f)
		{
			this.Remove(0f);
		}
		else if (this.parent != null && this.parent.HasFlag(global::ItemContainer.Flag.NoBrokenItems))
		{
			global::ItemContainer rootContainer = this.GetRootContainer();
			if (rootContainer.HasFlag(global::ItemContainer.Flag.NoBrokenItems))
			{
				this.Remove(0f);
			}
			else
			{
				global::BasePlayer playerOwner = rootContainer.playerOwner;
				if (playerOwner != null && !this.MoveToContainer(playerOwner.inventory.containerMain, -1, true, false, null, true))
				{
					this.Drop(playerOwner.transform.position, playerOwner.eyes.BodyForward() * 1.5f, default(Quaternion));
				}
			}
		}
		this.MarkDirty();
	}

	// Token: 0x1700034C RID: 844
	// (get) Token: 0x06002ABD RID: 10941 RVA: 0x00102C4C File Offset: 0x00100E4C
	public int despawnMultiplier
	{
		get
		{
			Rarity rarity = this.info.despawnRarity;
			if (rarity == Rarity.None)
			{
				rarity = this.info.rarity;
			}
			if (!(this.info != null))
			{
				return 1;
			}
			return Mathf.Clamp((rarity - Rarity.Common) * 4, 1, 100);
		}
	}

	// Token: 0x1700034D RID: 845
	// (get) Token: 0x06002ABE RID: 10942 RVA: 0x00102C91 File Offset: 0x00100E91
	public ItemDefinition blueprintTargetDef
	{
		get
		{
			if (!this.IsBlueprint())
			{
				return null;
			}
			return ItemManager.FindItemDefinition(this.blueprintTarget);
		}
	}

	// Token: 0x1700034E RID: 846
	// (get) Token: 0x06002ABF RID: 10943 RVA: 0x00102CA8 File Offset: 0x00100EA8
	// (set) Token: 0x06002AC0 RID: 10944 RVA: 0x00102CBF File Offset: 0x00100EBF
	public int blueprintTarget
	{
		get
		{
			if (this.instanceData == null)
			{
				return 0;
			}
			return this.instanceData.blueprintTarget;
		}
		set
		{
			if (this.instanceData == null)
			{
				this.instanceData = new ProtoBuf.Item.InstanceData();
			}
			this.instanceData.ShouldPool = false;
			this.instanceData.blueprintTarget = value;
		}
	}

	// Token: 0x1700034F RID: 847
	// (get) Token: 0x06002AC1 RID: 10945 RVA: 0x00102CEC File Offset: 0x00100EEC
	// (set) Token: 0x06002AC2 RID: 10946 RVA: 0x00102CF4 File Offset: 0x00100EF4
	public int blueprintAmount
	{
		get
		{
			return this.amount;
		}
		set
		{
			this.amount = value;
		}
	}

	// Token: 0x06002AC3 RID: 10947 RVA: 0x00102CFD File Offset: 0x00100EFD
	public bool IsBlueprint()
	{
		return this.blueprintTarget != 0;
	}

	// Token: 0x14000002 RID: 2
	// (add) Token: 0x06002AC4 RID: 10948 RVA: 0x00102D08 File Offset: 0x00100F08
	// (remove) Token: 0x06002AC5 RID: 10949 RVA: 0x00102D40 File Offset: 0x00100F40
	public event Action<global::Item> OnDirty;

	// Token: 0x06002AC6 RID: 10950 RVA: 0x00102D75 File Offset: 0x00100F75
	public bool HasFlag(global::Item.Flag f)
	{
		return (this.flags & f) == f;
	}

	// Token: 0x06002AC7 RID: 10951 RVA: 0x00102D82 File Offset: 0x00100F82
	public void SetFlag(global::Item.Flag f, bool b)
	{
		if (b)
		{
			this.flags |= f;
			return;
		}
		this.flags &= ~f;
	}

	// Token: 0x06002AC8 RID: 10952 RVA: 0x00102DA5 File Offset: 0x00100FA5
	public bool IsOn()
	{
		return this.HasFlag(global::Item.Flag.IsOn);
	}

	// Token: 0x06002AC9 RID: 10953 RVA: 0x00102DAE File Offset: 0x00100FAE
	public bool IsOnFire()
	{
		return this.HasFlag(global::Item.Flag.OnFire);
	}

	// Token: 0x06002ACA RID: 10954 RVA: 0x00102DB7 File Offset: 0x00100FB7
	public bool IsCooking()
	{
		return this.HasFlag(global::Item.Flag.Cooking);
	}

	// Token: 0x06002ACB RID: 10955 RVA: 0x00102DC1 File Offset: 0x00100FC1
	public bool IsLocked()
	{
		return this.HasFlag(global::Item.Flag.IsLocked) || (this.parent != null && this.parent.IsLocked());
	}

	// Token: 0x17000350 RID: 848
	// (get) Token: 0x06002ACC RID: 10956 RVA: 0x00102DE3 File Offset: 0x00100FE3
	public global::Item parentItem
	{
		get
		{
			if (this.parent == null)
			{
				return null;
			}
			return this.parent.parent;
		}
	}

	// Token: 0x06002ACD RID: 10957 RVA: 0x00102DFA File Offset: 0x00100FFA
	public void MarkDirty()
	{
		this.OnChanged();
		this.dirty = true;
		if (this.parent != null)
		{
			this.parent.MarkDirty();
		}
		if (this.OnDirty != null)
		{
			this.OnDirty(this);
		}
	}

	// Token: 0x06002ACE RID: 10958 RVA: 0x00102E30 File Offset: 0x00101030
	public void OnChanged()
	{
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnChanged(this);
		}
		if (this.contents != null)
		{
			this.contents.OnChanged();
		}
	}

	// Token: 0x06002ACF RID: 10959 RVA: 0x00102E74 File Offset: 0x00101074
	public void CollectedForCrafting(global::BasePlayer crafter)
	{
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].CollectedForCrafting(this, crafter);
		}
	}

	// Token: 0x06002AD0 RID: 10960 RVA: 0x00102EA8 File Offset: 0x001010A8
	public void ReturnedFromCancelledCraft(global::BasePlayer crafter)
	{
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].ReturnedFromCancelledCraft(this, crafter);
		}
	}

	// Token: 0x06002AD1 RID: 10961 RVA: 0x00102EDC File Offset: 0x001010DC
	public void Initialize(ItemDefinition template)
	{
		this.uid = Network.Net.sv.TakeUID();
		this.condition = (this.maxCondition = this.info.condition.max);
		this.OnItemCreated();
	}

	// Token: 0x06002AD2 RID: 10962 RVA: 0x00102F20 File Offset: 0x00101120
	public void OnItemCreated()
	{
		this.onCycle = null;
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnItemCreated(this);
		}
	}

	// Token: 0x06002AD3 RID: 10963 RVA: 0x00102F58 File Offset: 0x00101158
	public void OnVirginSpawn()
	{
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnVirginItem(this);
		}
	}

	// Token: 0x06002AD4 RID: 10964 RVA: 0x00102F88 File Offset: 0x00101188
	public float GetDespawnDuration()
	{
		if (this.info.quickDespawn)
		{
			return ConVar.Server.itemdespawn_quick;
		}
		return ConVar.Server.itemdespawn * (float)this.despawnMultiplier;
	}

	// Token: 0x06002AD5 RID: 10965 RVA: 0x00102FAC File Offset: 0x001011AC
	protected void RemoveFromWorld()
	{
		global::BaseEntity worldEntity = this.GetWorldEntity();
		if (worldEntity == null)
		{
			return;
		}
		this.SetWorldEntity(null);
		this.OnRemovedFromWorld();
		if (this.contents != null)
		{
			this.contents.OnRemovedFromWorld();
		}
		if (!worldEntity.IsValid())
		{
			return;
		}
		global::WorldItem worldItem;
		if ((worldItem = (worldEntity as global::WorldItem)) != null)
		{
			worldItem.RemoveItem();
		}
		worldEntity.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06002AD6 RID: 10966 RVA: 0x0010300C File Offset: 0x0010120C
	public void OnRemovedFromWorld()
	{
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnRemovedFromWorld(this);
		}
	}

	// Token: 0x06002AD7 RID: 10967 RVA: 0x0010303C File Offset: 0x0010123C
	public void RemoveFromContainer()
	{
		if (this.parent == null)
		{
			return;
		}
		this.SetParent(null);
	}

	// Token: 0x06002AD8 RID: 10968 RVA: 0x0010304E File Offset: 0x0010124E
	public bool DoItemSlotsConflict(global::Item other)
	{
		return (this.info.occupySlots & other.info.occupySlots) > (ItemSlot)0;
	}

	// Token: 0x06002AD9 RID: 10969 RVA: 0x0010306C File Offset: 0x0010126C
	public void SetParent(global::ItemContainer target)
	{
		if (target == this.parent)
		{
			return;
		}
		if (this.parent != null)
		{
			this.parent.Remove(this);
			this.parent = null;
		}
		if (target == null)
		{
			this.position = 0;
		}
		else
		{
			this.parent = target;
			if (!this.parent.Insert(this))
			{
				this.Remove(0f);
				Debug.LogError("Item.SetParent caused remove - this shouldn't ever happen");
			}
		}
		this.MarkDirty();
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnParentChanged(this);
		}
	}

	// Token: 0x06002ADA RID: 10970 RVA: 0x00103100 File Offset: 0x00101300
	public void OnAttacked(HitInfo hitInfo)
	{
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnAttacked(this, hitInfo);
		}
	}

	// Token: 0x06002ADB RID: 10971 RVA: 0x00103131 File Offset: 0x00101331
	public global::BaseEntity GetEntityOwner()
	{
		global::ItemContainer itemContainer = this.parent;
		if (itemContainer == null)
		{
			return null;
		}
		return itemContainer.GetEntityOwner(false);
	}

	// Token: 0x06002ADC RID: 10972 RVA: 0x00103148 File Offset: 0x00101348
	public bool IsChildContainer(global::ItemContainer c)
	{
		if (this.contents == null)
		{
			return false;
		}
		if (this.contents == c)
		{
			return true;
		}
		using (List<global::Item>.Enumerator enumerator = this.contents.itemList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsChildContainer(c))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002ADD RID: 10973 RVA: 0x001031BC File Offset: 0x001013BC
	public bool CanMoveTo(global::ItemContainer newcontainer, int iTargetPos = -1)
	{
		return !this.IsChildContainer(newcontainer) && newcontainer.CanAcceptItem(this, iTargetPos) == global::ItemContainer.CanAcceptResult.CanAccept && iTargetPos < newcontainer.capacity && (this.parent == null || newcontainer != this.parent || iTargetPos != this.position);
	}

	// Token: 0x06002ADE RID: 10974 RVA: 0x00103208 File Offset: 0x00101408
	public bool MoveToContainer(global::ItemContainer newcontainer, int iTargetPos = -1, bool allowStack = true, bool ignoreStackLimit = false, global::BasePlayer sourcePlayer = null, bool allowSwap = true)
	{
		bool result;
		using (TimeWarning.New("MoveToContainer", 0))
		{
			bool flag = iTargetPos == -1;
			global::ItemContainer itemContainer = this.parent;
			if (iTargetPos == -1)
			{
				if (allowStack && this.info.stackable > 1)
				{
					foreach (global::Item item in from x in newcontainer.FindItemsByItemID(this.info.itemid)
					orderby x.position
					select x)
					{
						if (item.CanStack(this) && (ignoreStackLimit || item.amount < item.MaxStackable()))
						{
							iTargetPos = item.position;
						}
					}
				}
				if (iTargetPos == -1)
				{
					IItemContainerEntity itemContainerEntity = newcontainer.GetEntityOwner(true) as IItemContainerEntity;
					if (itemContainerEntity != null)
					{
						iTargetPos = itemContainerEntity.GetIdealSlot(sourcePlayer, this);
						if (iTargetPos == -2147483648)
						{
							return false;
						}
					}
				}
				if (iTargetPos == -1)
				{
					if (newcontainer == this.parent)
					{
						return false;
					}
					bool flag2 = newcontainer.HasFlag(global::ItemContainer.Flag.Clothing) && this.info.isWearable;
					ItemModWearable itemModWearable = this.info.ItemModWearable;
					for (int i = 0; i < newcontainer.capacity; i++)
					{
						global::Item slot = newcontainer.GetSlot(i);
						if (slot == null)
						{
							if (this.CanMoveTo(newcontainer, i))
							{
								iTargetPos = i;
								break;
							}
						}
						else
						{
							if (flag2 && slot != null && !slot.info.ItemModWearable.CanExistWith(itemModWearable))
							{
								iTargetPos = i;
								break;
							}
							if (newcontainer.availableSlots != null && newcontainer.availableSlots.Count > 0 && this.DoItemSlotsConflict(slot))
							{
								iTargetPos = i;
								break;
							}
						}
					}
					if (flag2 && iTargetPos == -1)
					{
						iTargetPos = newcontainer.capacity - 1;
					}
				}
			}
			if (iTargetPos == -1)
			{
				result = false;
			}
			else if (!this.CanMoveTo(newcontainer, iTargetPos))
			{
				result = false;
			}
			else if (iTargetPos >= 0 && newcontainer.SlotTaken(this, iTargetPos))
			{
				global::Item slot2 = newcontainer.GetSlot(iTargetPos);
				if (slot2 == this)
				{
					result = false;
				}
				else
				{
					if (allowStack && slot2 != null)
					{
						int num = slot2.MaxStackable();
						if (slot2.CanStack(this))
						{
							if (ignoreStackLimit)
							{
								num = int.MaxValue;
							}
							if (slot2.amount >= num)
							{
								return false;
							}
							int num2 = Mathf.Min(num - slot2.amount, this.amount);
							slot2.amount += num2;
							this.amount -= num2;
							slot2.MarkDirty();
							this.MarkDirty();
							if (this.amount <= 0)
							{
								this.RemoveFromWorld();
								this.RemoveFromContainer();
								this.Remove(0f);
								return true;
							}
							if (flag)
							{
								return this.MoveToContainer(newcontainer, -1, allowStack, ignoreStackLimit, sourcePlayer, true);
							}
							return false;
						}
					}
					if (this.parent != null && allowSwap && slot2 != null)
					{
						global::ItemContainer itemContainer2 = this.parent;
						int iTargetPos2 = this.position;
						global::ItemContainer itemContainer3 = slot2.parent;
						int num3 = slot2.position;
						if (!slot2.CanMoveTo(itemContainer2, iTargetPos2))
						{
							result = false;
						}
						else
						{
							global::BaseEntity entityOwner = this.GetEntityOwner();
							global::BaseEntity entityOwner2 = slot2.GetEntityOwner();
							this.RemoveFromContainer();
							slot2.RemoveFromContainer();
							this.RemoveConflictingSlots(newcontainer, entityOwner, sourcePlayer);
							slot2.RemoveConflictingSlots(itemContainer2, entityOwner2, sourcePlayer);
							if (!slot2.MoveToContainer(itemContainer2, iTargetPos2, true, false, sourcePlayer, true) || !this.MoveToContainer(newcontainer, iTargetPos, true, false, sourcePlayer, true))
							{
								this.RemoveFromContainer();
								slot2.RemoveFromContainer();
								this.SetParent(itemContainer2);
								this.position = iTargetPos2;
								slot2.SetParent(itemContainer3);
								slot2.position = num3;
								result = true;
							}
							else
							{
								result = true;
							}
						}
					}
					else
					{
						result = false;
					}
				}
			}
			else if (this.parent == newcontainer)
			{
				if (iTargetPos >= 0 && iTargetPos != this.position && !this.parent.SlotTaken(this, iTargetPos))
				{
					this.position = iTargetPos;
					this.MarkDirty();
					result = true;
				}
				else
				{
					result = false;
				}
			}
			else if (newcontainer.maxStackSize > 0 && newcontainer.maxStackSize < this.amount)
			{
				global::Item item2 = this.SplitItem(newcontainer.maxStackSize);
				if (item2 != null && !item2.MoveToContainer(newcontainer, iTargetPos, false, false, sourcePlayer, true) && (itemContainer == null || !item2.MoveToContainer(itemContainer, -1, true, false, sourcePlayer, true)))
				{
					item2.Drop(newcontainer.dropPosition, newcontainer.dropVelocity, default(Quaternion));
				}
				result = true;
			}
			else if (!newcontainer.CanAccept(this))
			{
				result = false;
			}
			else
			{
				global::BaseEntity entityOwner3 = this.GetEntityOwner();
				this.RemoveFromContainer();
				this.RemoveFromWorld();
				this.RemoveConflictingSlots(newcontainer, entityOwner3, sourcePlayer);
				this.position = iTargetPos;
				this.SetParent(newcontainer);
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06002ADF RID: 10975 RVA: 0x001036FC File Offset: 0x001018FC
	private void RemoveConflictingSlots(global::ItemContainer container, global::BaseEntity entityOwner, global::BasePlayer sourcePlayer)
	{
		if (this.isServer && container.availableSlots != null && container.availableSlots.Count > 0)
		{
			List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
			list.AddRange(container.itemList);
			foreach (global::Item item in list)
			{
				if (item.DoItemSlotsConflict(this))
				{
					item.RemoveFromContainer();
					global::BasePlayer basePlayer;
					IItemContainerEntity itemContainerEntity;
					if ((basePlayer = (entityOwner as global::BasePlayer)) != null)
					{
						basePlayer.GiveItem(item, global::BaseEntity.GiveItemReason.Generic);
					}
					else if ((itemContainerEntity = (entityOwner as IItemContainerEntity)) != null)
					{
						item.MoveToContainer(itemContainerEntity.inventory, -1, true, false, sourcePlayer, true);
					}
				}
			}
			Facepunch.Pool.FreeList<global::Item>(ref list);
		}
	}

	// Token: 0x06002AE0 RID: 10976 RVA: 0x001037C4 File Offset: 0x001019C4
	public global::BaseEntity CreateWorldObject(Vector3 pos, Quaternion rotation = default(Quaternion), global::BaseEntity parentEnt = null, uint parentBone = 0U)
	{
		global::BaseEntity baseEntity = this.GetWorldEntity();
		if (baseEntity != null)
		{
			return baseEntity;
		}
		baseEntity = GameManager.server.CreateEntity("assets/prefabs/misc/burlap sack/generic_world.prefab", pos, rotation, true);
		if (baseEntity == null)
		{
			Debug.LogWarning("Couldn't create world object for prefab: items/generic_world");
			return null;
		}
		global::WorldItem worldItem = baseEntity as global::WorldItem;
		if (worldItem != null)
		{
			worldItem.InitializeItem(this);
		}
		if (parentEnt != null)
		{
			baseEntity.SetParent(parentEnt, parentBone, false, false);
		}
		baseEntity.Spawn();
		this.SetWorldEntity(baseEntity);
		return this.GetWorldEntity();
	}

	// Token: 0x06002AE1 RID: 10977 RVA: 0x0010384C File Offset: 0x00101A4C
	public global::BaseEntity Drop(Vector3 vPos, Vector3 vVelocity, Quaternion rotation = default(Quaternion))
	{
		this.RemoveFromWorld();
		global::BaseEntity baseEntity = null;
		if (vPos != Vector3.zero && !this.info.HasFlag(ItemDefinition.Flag.NoDropping))
		{
			baseEntity = this.CreateWorldObject(vPos, rotation, null, 0U);
			if (baseEntity)
			{
				baseEntity.SetVelocity(vVelocity);
			}
		}
		else
		{
			this.Remove(0f);
		}
		this.RemoveFromContainer();
		return baseEntity;
	}

	// Token: 0x06002AE2 RID: 10978 RVA: 0x001038AC File Offset: 0x00101AAC
	public global::BaseEntity DropAndTossUpwards(Vector3 vPos, float force = 2f)
	{
		float f = UnityEngine.Random.value * 3.1415927f * 2f;
		Vector3 a = new Vector3(Mathf.Sin(f), 1f, Mathf.Cos(f));
		return this.Drop(vPos + Vector3.up * 0.1f, a * force, default(Quaternion));
	}

	// Token: 0x06002AE3 RID: 10979 RVA: 0x0010390E File Offset: 0x00101B0E
	public bool IsBusy()
	{
		return this.busyTime > UnityEngine.Time.time;
	}

	// Token: 0x06002AE4 RID: 10980 RVA: 0x00103920 File Offset: 0x00101B20
	public void BusyFor(float fTime)
	{
		this.busyTime = UnityEngine.Time.time + fTime;
	}

	// Token: 0x06002AE5 RID: 10981 RVA: 0x00103930 File Offset: 0x00101B30
	public void Remove(float fTime = 0f)
	{
		if (this.removeTime > 0f)
		{
			return;
		}
		if (this.isServer)
		{
			ItemMod[] itemMods = this.info.itemMods;
			for (int i = 0; i < itemMods.Length; i++)
			{
				itemMods[i].OnRemove(this);
			}
		}
		this.onCycle = null;
		this.removeTime = UnityEngine.Time.time + fTime;
		this.OnDirty = null;
		this.position = -1;
		if (this.isServer)
		{
			ItemManager.RemoveItem(this, fTime);
		}
	}

	// Token: 0x06002AE6 RID: 10982 RVA: 0x001039A8 File Offset: 0x00101BA8
	internal void DoRemove()
	{
		this.OnDirty = null;
		this.onCycle = null;
		if (this.isServer && this.uid > 0U && Network.Net.sv != null)
		{
			Network.Net.sv.ReturnUID(this.uid);
			this.uid = 0U;
		}
		if (this.contents != null)
		{
			this.contents.Kill();
			this.contents = null;
		}
		if (this.isServer)
		{
			this.RemoveFromWorld();
			this.RemoveFromContainer();
		}
		global::BaseEntity baseEntity = this.GetHeldEntity();
		if (baseEntity.IsValid())
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				"Item's Held Entity not removed!",
				this.info.displayName.english,
				" -> ",
				baseEntity
			}), baseEntity);
		}
	}

	// Token: 0x06002AE7 RID: 10983 RVA: 0x00103A67 File Offset: 0x00101C67
	public void SwitchOnOff(bool bNewState)
	{
		if (this.HasFlag(global::Item.Flag.IsOn) == bNewState)
		{
			return;
		}
		this.SetFlag(global::Item.Flag.IsOn, bNewState);
		this.MarkDirty();
	}

	// Token: 0x06002AE8 RID: 10984 RVA: 0x00103A82 File Offset: 0x00101C82
	public void LockUnlock(bool bNewState)
	{
		if (this.HasFlag(global::Item.Flag.IsLocked) == bNewState)
		{
			return;
		}
		this.SetFlag(global::Item.Flag.IsLocked, bNewState);
		this.MarkDirty();
	}

	// Token: 0x17000351 RID: 849
	// (get) Token: 0x06002AE9 RID: 10985 RVA: 0x00103A9D File Offset: 0x00101C9D
	public float temperature
	{
		get
		{
			if (this.parent != null)
			{
				return this.parent.GetTemperature(this.position);
			}
			return 15f;
		}
	}

	// Token: 0x06002AEA RID: 10986 RVA: 0x00103ABE File Offset: 0x00101CBE
	public global::BasePlayer GetOwnerPlayer()
	{
		if (this.parent == null)
		{
			return null;
		}
		return this.parent.GetOwnerPlayer();
	}

	// Token: 0x06002AEB RID: 10987 RVA: 0x00103AD8 File Offset: 0x00101CD8
	public global::Item SplitItem(int split_Amount)
	{
		Assert.IsTrue(split_Amount > 0, "split_Amount <= 0");
		if (split_Amount <= 0)
		{
			return null;
		}
		if (split_Amount >= this.amount)
		{
			return null;
		}
		this.amount -= split_Amount;
		global::Item item = ItemManager.CreateByItemID(this.info.itemid, 1, 0UL);
		item.amount = split_Amount;
		item.skin = this.skin;
		if (this.IsBlueprint())
		{
			item.blueprintTarget = this.blueprintTarget;
		}
		if (this.info.amountType == ItemDefinition.AmountType.Genetics && this.instanceData != null && this.instanceData.dataInt != 0)
		{
			item.instanceData = new ProtoBuf.Item.InstanceData();
			item.instanceData.dataInt = this.instanceData.dataInt;
			item.instanceData.ShouldPool = false;
		}
		this.MarkDirty();
		return item;
	}

	// Token: 0x06002AEC RID: 10988 RVA: 0x00103BA4 File Offset: 0x00101DA4
	public bool CanBeHeld()
	{
		return !this.isBroken;
	}

	// Token: 0x06002AED RID: 10989 RVA: 0x00103BB4 File Offset: 0x00101DB4
	public bool CanStack(global::Item item)
	{
		if (item == this)
		{
			return false;
		}
		if (this.MaxStackable() <= 1)
		{
			return false;
		}
		if (item.MaxStackable() <= 1)
		{
			return false;
		}
		if (item.info.itemid != this.info.itemid)
		{
			return false;
		}
		if (this.hasCondition && this.condition != item.info.condition.max)
		{
			return false;
		}
		if (item.hasCondition && item.condition != item.info.condition.max)
		{
			return false;
		}
		if (!this.IsValid())
		{
			return false;
		}
		if (this.IsBlueprint() && this.blueprintTarget != item.blueprintTarget)
		{
			return false;
		}
		if (item.skin != this.skin)
		{
			return false;
		}
		if (item.info.amountType == ItemDefinition.AmountType.Genetics || this.info.amountType == ItemDefinition.AmountType.Genetics)
		{
			int num = (item.instanceData != null) ? item.instanceData.dataInt : -1;
			int num2 = (this.instanceData != null) ? this.instanceData.dataInt : -1;
			if (num != num2)
			{
				return false;
			}
		}
		return (this.instanceData == null || this.instanceData.subEntity == 0U || !this.info.GetComponent<ItemModSign>()) && (item.instanceData == null || item.instanceData.subEntity == 0U || !item.info.GetComponent<ItemModSign>());
	}

	// Token: 0x06002AEE RID: 10990 RVA: 0x00103D0E File Offset: 0x00101F0E
	public bool IsValid()
	{
		return this.removeTime <= 0f;
	}

	// Token: 0x06002AEF RID: 10991 RVA: 0x00103D20 File Offset: 0x00101F20
	public void SetWorldEntity(global::BaseEntity ent)
	{
		if (!ent.IsValid())
		{
			this.worldEnt.Set(null);
			this.MarkDirty();
			return;
		}
		if (this.worldEnt.uid == ent.net.ID)
		{
			return;
		}
		this.worldEnt.Set(ent);
		this.MarkDirty();
		this.OnMovedToWorld();
		if (this.contents != null)
		{
			this.contents.OnMovedToWorld();
		}
	}

	// Token: 0x06002AF0 RID: 10992 RVA: 0x00103D8C File Offset: 0x00101F8C
	public void OnMovedToWorld()
	{
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnMovedToWorld(this);
		}
	}

	// Token: 0x06002AF1 RID: 10993 RVA: 0x00103DBC File Offset: 0x00101FBC
	public global::BaseEntity GetWorldEntity()
	{
		return this.worldEnt.Get(this.isServer);
	}

	// Token: 0x06002AF2 RID: 10994 RVA: 0x00103DD0 File Offset: 0x00101FD0
	public void SetHeldEntity(global::BaseEntity ent)
	{
		if (!ent.IsValid())
		{
			this.heldEntity.Set(null);
			this.MarkDirty();
			return;
		}
		if (this.heldEntity.uid == ent.net.ID)
		{
			return;
		}
		this.heldEntity.Set(ent);
		this.MarkDirty();
		if (ent.IsValid())
		{
			global::HeldEntity heldEntity = ent as global::HeldEntity;
			if (heldEntity != null)
			{
				heldEntity.SetupHeldEntity(this);
			}
		}
	}

	// Token: 0x06002AF3 RID: 10995 RVA: 0x00103E42 File Offset: 0x00102042
	public global::BaseEntity GetHeldEntity()
	{
		return this.heldEntity.Get(this.isServer);
	}

	// Token: 0x14000003 RID: 3
	// (add) Token: 0x06002AF4 RID: 10996 RVA: 0x00103E58 File Offset: 0x00102058
	// (remove) Token: 0x06002AF5 RID: 10997 RVA: 0x00103E90 File Offset: 0x00102090
	public event Action<global::Item, float> onCycle;

	// Token: 0x06002AF6 RID: 10998 RVA: 0x00103EC5 File Offset: 0x001020C5
	public void OnCycle(float delta)
	{
		if (this.onCycle != null)
		{
			this.onCycle(this, delta);
		}
	}

	// Token: 0x06002AF7 RID: 10999 RVA: 0x00103EDC File Offset: 0x001020DC
	public void ServerCommand(string command, global::BasePlayer player)
	{
		global::HeldEntity heldEntity = this.GetHeldEntity() as global::HeldEntity;
		if (heldEntity != null)
		{
			heldEntity.ServerCommand(this, command, player);
		}
		ItemMod[] itemMods = this.info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].ServerCommand(this, command, player);
		}
	}

	// Token: 0x06002AF8 RID: 11000 RVA: 0x00103F2C File Offset: 0x0010212C
	public void UseItem(int amountToConsume = 1)
	{
		if (amountToConsume <= 0)
		{
			return;
		}
		this.amount -= amountToConsume;
		if (this.amount <= 0)
		{
			this.amount = 0;
			this.Remove(0f);
			return;
		}
		this.MarkDirty();
	}

	// Token: 0x06002AF9 RID: 11001 RVA: 0x00103F64 File Offset: 0x00102164
	public bool HasAmmo(AmmoTypes ammoType)
	{
		ItemModProjectile itemModProjectile;
		return (this.info.TryGetComponent<ItemModProjectile>(out itemModProjectile) && itemModProjectile.IsAmmo(ammoType)) || (this.contents != null && this.contents.HasAmmo(ammoType));
	}

	// Token: 0x06002AFA RID: 11002 RVA: 0x00103FA4 File Offset: 0x001021A4
	public void FindAmmo(List<global::Item> list, AmmoTypes ammoType)
	{
		ItemModProjectile itemModProjectile;
		if (this.info.TryGetComponent<ItemModProjectile>(out itemModProjectile) && itemModProjectile.IsAmmo(ammoType))
		{
			list.Add(this);
			return;
		}
		if (this.contents != null)
		{
			this.contents.FindAmmo(list, ammoType);
		}
	}

	// Token: 0x06002AFB RID: 11003 RVA: 0x00103FE8 File Offset: 0x001021E8
	public int GetAmmoAmount(AmmoTypes ammoType)
	{
		int num = 0;
		ItemModProjectile itemModProjectile;
		if (this.info.TryGetComponent<ItemModProjectile>(out itemModProjectile) && itemModProjectile.IsAmmo(ammoType))
		{
			num += this.amount;
		}
		if (this.contents != null)
		{
			num += this.contents.GetAmmoAmount(ammoType);
		}
		return num;
	}

	// Token: 0x06002AFC RID: 11004 RVA: 0x00104030 File Offset: 0x00102230
	public override string ToString()
	{
		return string.Concat(new object[]
		{
			"Item.",
			this.info.shortname,
			"x",
			this.amount,
			".",
			this.uid
		});
	}

	// Token: 0x06002AFD RID: 11005 RVA: 0x0010408A File Offset: 0x0010228A
	public global::Item FindItem(uint iUID)
	{
		if (this.uid == iUID)
		{
			return this;
		}
		if (this.contents == null)
		{
			return null;
		}
		return this.contents.FindItemByUID(iUID);
	}

	// Token: 0x06002AFE RID: 11006 RVA: 0x001040B0 File Offset: 0x001022B0
	public int MaxStackable()
	{
		int num = this.info.stackable;
		if (this.parent != null && this.parent.maxStackSize > 0)
		{
			num = Mathf.Min(this.parent.maxStackSize, num);
		}
		return num;
	}

	// Token: 0x17000352 RID: 850
	// (get) Token: 0x06002AFF RID: 11007 RVA: 0x001040F2 File Offset: 0x001022F2
	public global::BaseEntity.TraitFlag Traits
	{
		get
		{
			return this.info.Traits;
		}
	}

	// Token: 0x06002B00 RID: 11008 RVA: 0x00104100 File Offset: 0x00102300
	public virtual ProtoBuf.Item Save(bool bIncludeContainer = false, bool bIncludeOwners = true)
	{
		this.dirty = false;
		ProtoBuf.Item item = Facepunch.Pool.Get<ProtoBuf.Item>();
		item.UID = this.uid;
		item.itemid = this.info.itemid;
		item.slot = this.position;
		item.amount = this.amount;
		item.flags = (int)this.flags;
		item.removetime = this.removeTime;
		item.locktime = this.busyTime;
		item.instanceData = this.instanceData;
		item.worldEntity = this.worldEnt.uid;
		item.heldEntity = this.heldEntity.uid;
		item.skinid = this.skin;
		item.name = this.name;
		item.text = this.text;
		item.cooktime = this.cookTimeLeft;
		if (this.hasCondition)
		{
			item.conditionData = Facepunch.Pool.Get<ProtoBuf.Item.ConditionData>();
			item.conditionData.maxCondition = this._maxCondition;
			item.conditionData.condition = this._condition;
		}
		if (this.contents != null && bIncludeContainer)
		{
			item.contents = this.contents.Save();
		}
		return item;
	}

	// Token: 0x06002B01 RID: 11009 RVA: 0x00104228 File Offset: 0x00102428
	public virtual void Load(ProtoBuf.Item load)
	{
		if (this.info == null || this.info.itemid != load.itemid)
		{
			this.info = ItemManager.FindItemDefinition(load.itemid);
		}
		this.uid = load.UID;
		this.name = load.name;
		this.text = load.text;
		this.cookTimeLeft = load.cooktime;
		this.amount = load.amount;
		this.position = load.slot;
		this.busyTime = load.locktime;
		this.removeTime = load.removetime;
		this.flags = (global::Item.Flag)load.flags;
		this.worldEnt.uid = load.worldEntity;
		this.heldEntity.uid = load.heldEntity;
		if (this.isServer)
		{
			Network.Net.sv.RegisterUID(this.uid);
		}
		if (this.instanceData != null)
		{
			this.instanceData.ShouldPool = true;
			this.instanceData.ResetToPool();
			this.instanceData = null;
		}
		this.instanceData = load.instanceData;
		if (this.instanceData != null)
		{
			this.instanceData.ShouldPool = false;
		}
		this.skin = load.skinid;
		if (this.info == null || this.info.itemid != load.itemid)
		{
			this.info = ItemManager.FindItemDefinition(load.itemid);
		}
		if (this.info == null)
		{
			return;
		}
		this._condition = 0f;
		this._maxCondition = 0f;
		if (load.conditionData != null)
		{
			this._condition = load.conditionData.condition;
			this._maxCondition = load.conditionData.maxCondition;
		}
		else if (this.info.condition.enabled)
		{
			this._condition = this.info.condition.max;
			this._maxCondition = this.info.condition.max;
		}
		if (load.contents != null)
		{
			if (this.contents == null)
			{
				this.contents = new global::ItemContainer();
				if (this.isServer)
				{
					this.contents.ServerInitialize(this, load.contents.slots);
				}
			}
			this.contents.Load(load.contents);
		}
		if (this.isServer)
		{
			this.removeTime = 0f;
			this.OnItemCreated();
		}
	}

	// Token: 0x040022B8 RID: 8888
	private const string DefaultArmourBreakEffectPath = "assets/bundled/prefabs/fx/armor_break.prefab";

	// Token: 0x040022B9 RID: 8889
	private float _condition;

	// Token: 0x040022BA RID: 8890
	private float _maxCondition = 100f;

	// Token: 0x040022BB RID: 8891
	public ItemDefinition info;

	// Token: 0x040022BC RID: 8892
	public uint uid;

	// Token: 0x040022BD RID: 8893
	public bool dirty;

	// Token: 0x040022BE RID: 8894
	public int amount = 1;

	// Token: 0x040022BF RID: 8895
	public int position;

	// Token: 0x040022C0 RID: 8896
	public float busyTime;

	// Token: 0x040022C1 RID: 8897
	public float removeTime;

	// Token: 0x040022C2 RID: 8898
	public float fuel;

	// Token: 0x040022C3 RID: 8899
	public bool isServer;

	// Token: 0x040022C4 RID: 8900
	public ProtoBuf.Item.InstanceData instanceData;

	// Token: 0x040022C5 RID: 8901
	public ulong skin;

	// Token: 0x040022C6 RID: 8902
	public string name;

	// Token: 0x040022C7 RID: 8903
	public string text;

	// Token: 0x040022C8 RID: 8904
	public float cookTimeLeft;

	// Token: 0x040022CA RID: 8906
	public global::Item.Flag flags;

	// Token: 0x040022CB RID: 8907
	public global::ItemContainer contents;

	// Token: 0x040022CC RID: 8908
	public global::ItemContainer parent;

	// Token: 0x040022CD RID: 8909
	private EntityRef worldEnt;

	// Token: 0x040022CE RID: 8910
	private EntityRef heldEntity;

	// Token: 0x02000D16 RID: 3350
	[Flags]
	public enum Flag
	{
		// Token: 0x040044F7 RID: 17655
		None = 0,
		// Token: 0x040044F8 RID: 17656
		Placeholder = 1,
		// Token: 0x040044F9 RID: 17657
		IsOn = 2,
		// Token: 0x040044FA RID: 17658
		OnFire = 4,
		// Token: 0x040044FB RID: 17659
		IsLocked = 8,
		// Token: 0x040044FC RID: 17660
		Cooking = 16
	}
}
