using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200059D RID: 1437
public sealed class ItemContainer
{
	// Token: 0x06002B05 RID: 11013 RVA: 0x0010449E File Offset: 0x0010269E
	public bool HasFlag(global::ItemContainer.Flag f)
	{
		return (this.flags & f) == f;
	}

	// Token: 0x06002B06 RID: 11014 RVA: 0x001044AB File Offset: 0x001026AB
	public void SetFlag(global::ItemContainer.Flag f, bool b)
	{
		if (b)
		{
			this.flags |= f;
			return;
		}
		this.flags &= ~f;
	}

	// Token: 0x06002B07 RID: 11015 RVA: 0x001044CE File Offset: 0x001026CE
	public bool IsLocked()
	{
		return this.HasFlag(global::ItemContainer.Flag.IsLocked);
	}

	// Token: 0x06002B08 RID: 11016 RVA: 0x001044D8 File Offset: 0x001026D8
	public bool PlayerItemInputBlocked()
	{
		return this.HasFlag(global::ItemContainer.Flag.NoItemInput);
	}

	// Token: 0x17000353 RID: 851
	// (get) Token: 0x06002B09 RID: 11017 RVA: 0x001044E5 File Offset: 0x001026E5
	public bool HasLimitedAllowedItems
	{
		get
		{
			return this.onlyAllowedItems != null && this.onlyAllowedItems.Length != 0;
		}
	}

	// Token: 0x14000004 RID: 4
	// (add) Token: 0x06002B0A RID: 11018 RVA: 0x001044FC File Offset: 0x001026FC
	// (remove) Token: 0x06002B0B RID: 11019 RVA: 0x00104534 File Offset: 0x00102734
	public event Action onDirty;

	// Token: 0x06002B0C RID: 11020 RVA: 0x0010456C File Offset: 0x0010276C
	public float GetTemperature(int slot)
	{
		global::BaseOven baseOven;
		if ((baseOven = (this.entityOwner as global::BaseOven)) != null)
		{
			return baseOven.GetTemperature(slot);
		}
		return this.temperature;
	}

	// Token: 0x06002B0D RID: 11021 RVA: 0x00104596 File Offset: 0x00102796
	public void ServerInitialize(global::Item parentItem, int iMaxCapacity)
	{
		this.parent = parentItem;
		this.capacity = iMaxCapacity;
		this.uid = 0U;
		this.isServer = true;
		if (this.allowedContents == (global::ItemContainer.ContentsType)0)
		{
			this.allowedContents = global::ItemContainer.ContentsType.Generic;
		}
		this.MarkDirty();
	}

	// Token: 0x06002B0E RID: 11022 RVA: 0x001045C9 File Offset: 0x001027C9
	public void GiveUID()
	{
		Assert.IsTrue(this.uid == 0U, "Calling GiveUID - but already has a uid!");
		this.uid = Net.sv.TakeUID();
	}

	// Token: 0x06002B0F RID: 11023 RVA: 0x001045EE File Offset: 0x001027EE
	public void MarkDirty()
	{
		this.dirty = true;
		if (this.parent != null)
		{
			this.parent.MarkDirty();
		}
		if (this.onDirty != null)
		{
			this.onDirty();
		}
	}

	// Token: 0x06002B10 RID: 11024 RVA: 0x00104620 File Offset: 0x00102820
	public DroppedItemContainer Drop(string prefab, Vector3 pos, Quaternion rot)
	{
		if (this.itemList == null || this.itemList.Count == 0)
		{
			return null;
		}
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(prefab, pos, rot, true);
		if (baseEntity == null)
		{
			return null;
		}
		DroppedItemContainer droppedItemContainer = baseEntity as DroppedItemContainer;
		if (droppedItemContainer != null)
		{
			droppedItemContainer.TakeFrom(new global::ItemContainer[]
			{
				this
			});
		}
		droppedItemContainer.Spawn();
		return droppedItemContainer;
	}

	// Token: 0x06002B11 RID: 11025 RVA: 0x00104688 File Offset: 0x00102888
	public static DroppedItemContainer Drop(string prefab, Vector3 pos, Quaternion rot, params global::ItemContainer[] containers)
	{
		int num = 0;
		foreach (global::ItemContainer itemContainer in containers)
		{
			num += ((itemContainer.itemList != null) ? itemContainer.itemList.Count : 0);
		}
		if (num == 0)
		{
			return null;
		}
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(prefab, pos, rot, true);
		if (baseEntity == null)
		{
			return null;
		}
		DroppedItemContainer droppedItemContainer = baseEntity as DroppedItemContainer;
		if (droppedItemContainer != null)
		{
			droppedItemContainer.TakeFrom(containers);
		}
		droppedItemContainer.Spawn();
		return droppedItemContainer;
	}

	// Token: 0x06002B12 RID: 11026 RVA: 0x00104704 File Offset: 0x00102904
	public global::BaseEntity GetEntityOwner(bool returnHeldEntity = false)
	{
		global::ItemContainer itemContainer = this;
		for (int i = 0; i < 10; i++)
		{
			if (itemContainer.entityOwner != null)
			{
				return itemContainer.entityOwner;
			}
			if (itemContainer.playerOwner != null)
			{
				return itemContainer.playerOwner;
			}
			if (returnHeldEntity)
			{
				global::Item item = itemContainer.parent;
				global::BaseEntity baseEntity = (item != null) ? item.GetHeldEntity() : null;
				if (baseEntity != null)
				{
					return baseEntity;
				}
			}
			global::Item item2 = itemContainer.parent;
			global::ItemContainer itemContainer2 = (item2 != null) ? item2.parent : null;
			if (itemContainer2 == null || itemContainer2 == itemContainer)
			{
				return null;
			}
			itemContainer = itemContainer2;
		}
		return null;
	}

	// Token: 0x06002B13 RID: 11027 RVA: 0x0010478C File Offset: 0x0010298C
	public void OnChanged()
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			this.itemList[i].OnChanged();
		}
	}

	// Token: 0x06002B14 RID: 11028 RVA: 0x001047C0 File Offset: 0x001029C0
	public global::Item FindItemByUID(uint iUID)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			global::Item item = this.itemList[i];
			if (item.IsValid())
			{
				global::Item item2 = item.FindItem(iUID);
				if (item2 != null)
				{
					return item2;
				}
			}
		}
		return null;
	}

	// Token: 0x06002B15 RID: 11029 RVA: 0x00104806 File Offset: 0x00102A06
	public bool IsFull()
	{
		return this.itemList.Count >= this.capacity;
	}

	// Token: 0x06002B16 RID: 11030 RVA: 0x0010481E File Offset: 0x00102A1E
	public bool IsEmpty()
	{
		return this.itemList.Count == 0;
	}

	// Token: 0x06002B17 RID: 11031 RVA: 0x0010482E File Offset: 0x00102A2E
	public bool CanAccept(global::Item item)
	{
		return !this.IsFull();
	}

	// Token: 0x06002B18 RID: 11032 RVA: 0x0010483C File Offset: 0x00102A3C
	public int GetMaxTransferAmount(ItemDefinition def)
	{
		int num = this.ContainerMaxStackSize();
		foreach (global::Item item in this.itemList)
		{
			if (item.info == def)
			{
				num -= item.amount;
				if (num <= 0)
				{
					return 0;
				}
			}
		}
		return num;
	}

	// Token: 0x06002B19 RID: 11033 RVA: 0x001048B4 File Offset: 0x00102AB4
	public void SetOnlyAllowedItem(ItemDefinition def)
	{
		this.SetOnlyAllowedItems(new ItemDefinition[]
		{
			def
		});
	}

	// Token: 0x06002B1A RID: 11034 RVA: 0x001048C8 File Offset: 0x00102AC8
	public void SetOnlyAllowedItems(params ItemDefinition[] defs)
	{
		int num = 0;
		for (int i = 0; i < defs.Length; i++)
		{
			if (defs[i] != null)
			{
				num++;
			}
		}
		this.onlyAllowedItems = new ItemDefinition[num];
		int num2 = 0;
		foreach (ItemDefinition itemDefinition in defs)
		{
			if (itemDefinition != null)
			{
				this.onlyAllowedItems[num2] = itemDefinition;
				num2++;
			}
		}
	}

	// Token: 0x06002B1B RID: 11035 RVA: 0x00104934 File Offset: 0x00102B34
	internal bool Insert(global::Item item)
	{
		if (this.itemList.Contains(item))
		{
			return false;
		}
		if (this.IsFull())
		{
			return false;
		}
		this.itemList.Add(item);
		item.parent = this;
		if (!this.FindPosition(item))
		{
			return false;
		}
		this.MarkDirty();
		if (this.onItemAddedRemoved != null)
		{
			this.onItemAddedRemoved(item, true);
		}
		return true;
	}

	// Token: 0x06002B1C RID: 11036 RVA: 0x00104995 File Offset: 0x00102B95
	public bool SlotTaken(global::Item item, int i)
	{
		return (this.slotIsReserved != null && this.slotIsReserved(item, i)) || this.GetSlot(i) != null;
	}

	// Token: 0x06002B1D RID: 11037 RVA: 0x001049BC File Offset: 0x00102BBC
	public global::Item GetSlot(int slot)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (this.itemList[i].position == slot)
			{
				return this.itemList[i];
			}
		}
		return null;
	}

	// Token: 0x06002B1E RID: 11038 RVA: 0x00104A04 File Offset: 0x00102C04
	internal bool FindPosition(global::Item item)
	{
		int position = item.position;
		item.position = -1;
		if (position >= 0 && !this.SlotTaken(item, position))
		{
			item.position = position;
			return true;
		}
		for (int i = 0; i < this.capacity; i++)
		{
			if (!this.SlotTaken(item, i))
			{
				item.position = i;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002B1F RID: 11039 RVA: 0x00104A5B File Offset: 0x00102C5B
	public void SetLocked(bool isLocked)
	{
		this.SetFlag(global::ItemContainer.Flag.IsLocked, isLocked);
		this.MarkDirty();
	}

	// Token: 0x06002B20 RID: 11040 RVA: 0x00104A6C File Offset: 0x00102C6C
	internal bool Remove(global::Item item)
	{
		if (!this.itemList.Contains(item))
		{
			return false;
		}
		if (this.onPreItemRemove != null)
		{
			this.onPreItemRemove(item);
		}
		this.itemList.Remove(item);
		item.parent = null;
		this.MarkDirty();
		if (this.onItemAddedRemoved != null)
		{
			this.onItemAddedRemoved(item, false);
		}
		return true;
	}

	// Token: 0x06002B21 RID: 11041 RVA: 0x00104AD0 File Offset: 0x00102CD0
	internal void Clear()
	{
		global::Item[] array = this.itemList.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Remove(0f);
		}
	}

	// Token: 0x06002B22 RID: 11042 RVA: 0x00104B04 File Offset: 0x00102D04
	public void Kill()
	{
		this.onDirty = null;
		this.canAcceptItem = null;
		this.slotIsReserved = null;
		this.onItemAddedRemoved = null;
		if (Net.sv != null)
		{
			Net.sv.ReturnUID(this.uid);
			this.uid = 0U;
		}
		List<global::Item> list = Pool.GetList<global::Item>();
		foreach (global::Item item in this.itemList)
		{
			list.Add(item);
		}
		foreach (global::Item item2 in list)
		{
			item2.Remove(0f);
		}
		Pool.FreeList<global::Item>(ref list);
		this.itemList.Clear();
	}

	// Token: 0x06002B23 RID: 11043 RVA: 0x00104BE8 File Offset: 0x00102DE8
	public int GetAmount(int itemid, bool onlyUsableAmounts)
	{
		int num = 0;
		foreach (global::Item item in this.itemList)
		{
			if (item.info.itemid == itemid && (!onlyUsableAmounts || !item.IsBusy()))
			{
				num += item.amount;
			}
		}
		return num;
	}

	// Token: 0x06002B24 RID: 11044 RVA: 0x00104C5C File Offset: 0x00102E5C
	public global::Item FindItemByItemID(int itemid)
	{
		foreach (global::Item item in this.itemList)
		{
			if (item.info.itemid == itemid)
			{
				return item;
			}
		}
		return null;
	}

	// Token: 0x06002B25 RID: 11045 RVA: 0x00104CC0 File Offset: 0x00102EC0
	public global::Item FindItemsByItemName(string name)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(name);
		if (itemDefinition == null)
		{
			return null;
		}
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (this.itemList[i].info == itemDefinition)
			{
				return this.itemList[i];
			}
		}
		return null;
	}

	// Token: 0x06002B26 RID: 11046 RVA: 0x00104D1C File Offset: 0x00102F1C
	public global::Item FindBySubEntityID(uint subEntityID)
	{
		if (subEntityID == 0U)
		{
			return null;
		}
		foreach (global::Item item in this.itemList)
		{
			if (item.instanceData != null && item.instanceData.subEntity == subEntityID)
			{
				return item;
			}
		}
		return null;
	}

	// Token: 0x06002B27 RID: 11047 RVA: 0x00104D8C File Offset: 0x00102F8C
	public List<global::Item> FindItemsByItemID(int itemid)
	{
		return this.itemList.FindAll((global::Item x) => x.info.itemid == itemid);
	}

	// Token: 0x06002B28 RID: 11048 RVA: 0x00104DC0 File Offset: 0x00102FC0
	public ProtoBuf.ItemContainer Save()
	{
		ProtoBuf.ItemContainer itemContainer = Pool.Get<ProtoBuf.ItemContainer>();
		itemContainer.contents = Pool.GetList<ProtoBuf.Item>();
		itemContainer.UID = this.uid;
		itemContainer.slots = this.capacity;
		itemContainer.temperature = this.temperature;
		itemContainer.allowedContents = (int)this.allowedContents;
		if (this.HasLimitedAllowedItems)
		{
			itemContainer.allowedItems = Pool.GetList<int>();
			for (int i = 0; i < this.onlyAllowedItems.Length; i++)
			{
				if (this.onlyAllowedItems[i] != null)
				{
					itemContainer.allowedItems.Add(this.onlyAllowedItems[i].itemid);
				}
			}
		}
		itemContainer.flags = (int)this.flags;
		itemContainer.maxStackSize = this.maxStackSize;
		if (this.availableSlots != null && this.availableSlots.Count > 0)
		{
			itemContainer.availableSlots = Pool.GetList<int>();
			for (int j = 0; j < this.availableSlots.Count; j++)
			{
				itemContainer.availableSlots.Add((int)this.availableSlots[j]);
			}
		}
		for (int k = 0; k < this.itemList.Count; k++)
		{
			global::Item item = this.itemList[k];
			if (item.IsValid())
			{
				itemContainer.contents.Add(item.Save(true, true));
			}
		}
		return itemContainer;
	}

	// Token: 0x06002B29 RID: 11049 RVA: 0x00104F04 File Offset: 0x00103104
	public void Load(ProtoBuf.ItemContainer container)
	{
		using (TimeWarning.New("ItemContainer.Load", 0))
		{
			this.uid = container.UID;
			this.capacity = container.slots;
			List<global::Item> list = this.itemList;
			this.itemList = Pool.GetList<global::Item>();
			this.temperature = container.temperature;
			this.flags = (global::ItemContainer.Flag)container.flags;
			this.allowedContents = (global::ItemContainer.ContentsType)((container.allowedContents == 0) ? 1 : container.allowedContents);
			if (container.allowedItems != null && container.allowedItems.Count > 0)
			{
				this.onlyAllowedItems = new ItemDefinition[container.allowedItems.Count];
				for (int i = 0; i < container.allowedItems.Count; i++)
				{
					this.onlyAllowedItems[i] = ItemManager.FindItemDefinition(container.allowedItems[i]);
				}
			}
			else
			{
				this.onlyAllowedItems = null;
			}
			this.maxStackSize = container.maxStackSize;
			this.availableSlots.Clear();
			for (int j = 0; j < container.availableSlots.Count; j++)
			{
				this.availableSlots.Add((ItemSlot)container.availableSlots[j]);
			}
			using (TimeWarning.New("container.contents", 0))
			{
				foreach (ProtoBuf.Item item in container.contents)
				{
					global::Item item2 = null;
					foreach (global::Item item3 in list)
					{
						if (item3.uid == item.UID)
						{
							item2 = item3;
							break;
						}
					}
					item2 = ItemManager.Load(item, item2, this.isServer);
					if (item2 != null)
					{
						item2.parent = this;
						item2.position = item.slot;
						this.Insert(item2);
					}
				}
			}
			using (TimeWarning.New("Delete old items", 0))
			{
				foreach (global::Item item4 in list)
				{
					if (!this.itemList.Contains(item4))
					{
						item4.Remove(0f);
					}
				}
			}
			this.dirty = true;
			Pool.FreeList<global::Item>(ref list);
		}
	}

	// Token: 0x06002B2A RID: 11050 RVA: 0x001051F8 File Offset: 0x001033F8
	public global::BasePlayer GetOwnerPlayer()
	{
		return this.playerOwner;
	}

	// Token: 0x06002B2B RID: 11051 RVA: 0x00105200 File Offset: 0x00103400
	public int ContainerMaxStackSize()
	{
		if (this.maxStackSize <= 0)
		{
			return int.MaxValue;
		}
		return this.maxStackSize;
	}

	// Token: 0x06002B2C RID: 11052 RVA: 0x00105218 File Offset: 0x00103418
	public int Take(List<global::Item> collect, int itemid, int iAmount)
	{
		int num = 0;
		if (iAmount == 0)
		{
			return num;
		}
		List<global::Item> list = Pool.GetList<global::Item>();
		foreach (global::Item item in this.itemList)
		{
			if (item.info.itemid == itemid)
			{
				int num2 = iAmount - num;
				if (num2 > 0)
				{
					if (item.amount > num2)
					{
						item.MarkDirty();
						item.amount -= num2;
						num += num2;
						global::Item item2 = ItemManager.CreateByItemID(itemid, 1, 0UL);
						item2.amount = num2;
						item2.CollectedForCrafting(this.playerOwner);
						if (collect != null)
						{
							collect.Add(item2);
							break;
						}
						break;
					}
					else
					{
						if (item.amount <= num2)
						{
							num += item.amount;
							list.Add(item);
							if (collect != null)
							{
								collect.Add(item);
							}
						}
						if (num == iAmount)
						{
							break;
						}
					}
				}
			}
		}
		foreach (global::Item item3 in list)
		{
			item3.RemoveFromContainer();
		}
		Pool.FreeList<global::Item>(ref list);
		return num;
	}

	// Token: 0x17000354 RID: 852
	// (get) Token: 0x06002B2D RID: 11053 RVA: 0x00105350 File Offset: 0x00103550
	public Vector3 dropPosition
	{
		get
		{
			if (this.playerOwner)
			{
				return this.playerOwner.GetDropPosition();
			}
			if (this.entityOwner)
			{
				return this.entityOwner.GetDropPosition();
			}
			if (this.parent != null)
			{
				global::BaseEntity worldEntity = this.parent.GetWorldEntity();
				if (worldEntity != null)
				{
					return worldEntity.GetDropPosition();
				}
			}
			Debug.LogWarning("ItemContainer.dropPosition dropped through");
			return Vector3.zero;
		}
	}

	// Token: 0x17000355 RID: 853
	// (get) Token: 0x06002B2E RID: 11054 RVA: 0x001053C4 File Offset: 0x001035C4
	public Vector3 dropVelocity
	{
		get
		{
			if (this.playerOwner)
			{
				return this.playerOwner.GetDropVelocity();
			}
			if (this.entityOwner)
			{
				return this.entityOwner.GetDropVelocity();
			}
			if (this.parent != null)
			{
				global::BaseEntity worldEntity = this.parent.GetWorldEntity();
				if (worldEntity != null)
				{
					return worldEntity.GetDropVelocity();
				}
			}
			Debug.LogWarning("ItemContainer.dropVelocity dropped through");
			return Vector3.zero;
		}
	}

	// Token: 0x06002B2F RID: 11055 RVA: 0x00105438 File Offset: 0x00103638
	public void OnCycle(float delta)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (this.itemList[i].IsValid())
			{
				this.itemList[i].OnCycle(delta);
			}
		}
	}

	// Token: 0x06002B30 RID: 11056 RVA: 0x00105480 File Offset: 0x00103680
	public void FindAmmo(List<global::Item> list, AmmoTypes ammoType)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			this.itemList[i].FindAmmo(list, ammoType);
		}
	}

	// Token: 0x06002B31 RID: 11057 RVA: 0x001054B8 File Offset: 0x001036B8
	public bool HasAmmo(AmmoTypes ammoType)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (this.itemList[i].HasAmmo(ammoType))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002B32 RID: 11058 RVA: 0x001054F4 File Offset: 0x001036F4
	public int GetAmmoAmount(AmmoTypes ammoType)
	{
		int num = 0;
		for (int i = 0; i < this.itemList.Count; i++)
		{
			num += this.itemList[i].GetAmmoAmount(ammoType);
		}
		return num;
	}

	// Token: 0x06002B33 RID: 11059 RVA: 0x00105530 File Offset: 0x00103730
	public int TotalItemAmount()
	{
		int num = 0;
		for (int i = 0; i < this.itemList.Count; i++)
		{
			num += this.itemList[i].amount;
		}
		return num;
	}

	// Token: 0x06002B34 RID: 11060 RVA: 0x0010556C File Offset: 0x0010376C
	public int GetTotalItemAmount(ItemDefinition def, int slotStartInclusive, int slotEndInclusive)
	{
		int num = 0;
		for (int i = slotStartInclusive; i <= slotEndInclusive; i++)
		{
			global::Item slot = this.GetSlot(i);
			if (slot != null && slot.info == def)
			{
				num += slot.amount;
			}
		}
		return num;
	}

	// Token: 0x06002B35 RID: 11061 RVA: 0x001055AC File Offset: 0x001037AC
	public int GetTotalCategoryAmount(ItemCategory category, int slotStartInclusive, int slotEndInclusive)
	{
		int num = 0;
		for (int i = slotStartInclusive; i <= slotEndInclusive; i++)
		{
			global::Item slot = this.GetSlot(i);
			if (slot != null && slot.info.category == category)
			{
				num += slot.amount;
			}
		}
		return num;
	}

	// Token: 0x06002B36 RID: 11062 RVA: 0x001055EC File Offset: 0x001037EC
	public void AddItem(ItemDefinition itemToCreate, int amount, ulong skin = 0UL, global::ItemContainer.LimitStack limitStack = global::ItemContainer.LimitStack.Existing)
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (amount == 0)
			{
				return;
			}
			if (!(this.itemList[i].info != itemToCreate))
			{
				int num = this.itemList[i].MaxStackable();
				if (num > this.itemList[i].amount || limitStack == global::ItemContainer.LimitStack.None)
				{
					this.MarkDirty();
					this.itemList[i].amount += amount;
					amount -= amount;
					if (this.itemList[i].amount > num && limitStack != global::ItemContainer.LimitStack.None)
					{
						amount = this.itemList[i].amount - num;
						if (amount > 0)
						{
							this.itemList[i].amount -= amount;
						}
					}
				}
			}
		}
		if (amount == 0)
		{
			return;
		}
		int num2 = (limitStack == global::ItemContainer.LimitStack.All) ? Mathf.Min(itemToCreate.stackable, this.ContainerMaxStackSize()) : int.MaxValue;
		if (num2 > 0)
		{
			while (amount > 0)
			{
				int num3 = Mathf.Min(amount, num2);
				global::Item item = ItemManager.Create(itemToCreate, num3, skin);
				amount -= num3;
				if (!item.MoveToContainer(this, -1, true, false, null, true))
				{
					item.Remove(0f);
				}
			}
		}
	}

	// Token: 0x06002B37 RID: 11063 RVA: 0x0010572C File Offset: 0x0010392C
	public void OnMovedToWorld()
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			this.itemList[i].OnMovedToWorld();
		}
	}

	// Token: 0x06002B38 RID: 11064 RVA: 0x00105760 File Offset: 0x00103960
	public void OnRemovedFromWorld()
	{
		for (int i = 0; i < this.itemList.Count; i++)
		{
			this.itemList[i].OnRemovedFromWorld();
		}
	}

	// Token: 0x06002B39 RID: 11065 RVA: 0x00105794 File Offset: 0x00103994
	public uint ContentsHash()
	{
		uint num = 0U;
		for (int i = 0; i < this.capacity; i++)
		{
			global::Item slot = this.GetSlot(i);
			if (slot != null)
			{
				num = CRC.Compute32(num, slot.info.itemid);
				num = CRC.Compute32(num, slot.skin);
			}
		}
		return num;
	}

	// Token: 0x06002B3A RID: 11066 RVA: 0x001057E0 File Offset: 0x001039E0
	internal global::ItemContainer FindContainer(uint id)
	{
		if (id == this.uid)
		{
			return this;
		}
		for (int i = 0; i < this.itemList.Count; i++)
		{
			global::Item item = this.itemList[i];
			if (item.contents != null)
			{
				global::ItemContainer itemContainer = item.contents.FindContainer(id);
				if (itemContainer != null)
				{
					return itemContainer;
				}
			}
		}
		return null;
	}

	// Token: 0x06002B3B RID: 11067 RVA: 0x00105838 File Offset: 0x00103A38
	public global::ItemContainer.CanAcceptResult CanAcceptItem(global::Item item, int targetPos)
	{
		if (this.canAcceptItem != null && !this.canAcceptItem(item, targetPos))
		{
			return global::ItemContainer.CanAcceptResult.CannotAccept;
		}
		if (this.isServer && this.availableSlots != null && this.availableSlots.Count > 0)
		{
			if (item.info.occupySlots == (ItemSlot)0 || item.info.occupySlots == ItemSlot.None)
			{
				return global::ItemContainer.CanAcceptResult.CannotAccept;
			}
			if (item.isBroken)
			{
				return global::ItemContainer.CanAcceptResult.CannotAccept;
			}
			int num = 0;
			foreach (ItemSlot itemSlot in this.availableSlots)
			{
				num |= (int)itemSlot;
			}
			if ((num & (int)item.info.occupySlots) != (int)item.info.occupySlots)
			{
				return global::ItemContainer.CanAcceptResult.CannotAcceptRightNow;
			}
		}
		if ((this.allowedContents & item.info.itemType) != item.info.itemType)
		{
			return global::ItemContainer.CanAcceptResult.CannotAccept;
		}
		if (this.HasLimitedAllowedItems)
		{
			bool flag = false;
			for (int i = 0; i < this.onlyAllowedItems.Length; i++)
			{
				if (this.onlyAllowedItems[i] == item.info)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return global::ItemContainer.CanAcceptResult.CannotAccept;
			}
		}
		return global::ItemContainer.CanAcceptResult.CanAccept;
	}

	// Token: 0x040022D0 RID: 8912
	public global::ItemContainer.Flag flags;

	// Token: 0x040022D1 RID: 8913
	public global::ItemContainer.ContentsType allowedContents;

	// Token: 0x040022D2 RID: 8914
	public ItemDefinition[] onlyAllowedItems;

	// Token: 0x040022D3 RID: 8915
	public List<ItemSlot> availableSlots = new List<ItemSlot>();

	// Token: 0x040022D4 RID: 8916
	public int capacity = 2;

	// Token: 0x040022D5 RID: 8917
	public uint uid;

	// Token: 0x040022D6 RID: 8918
	public bool dirty;

	// Token: 0x040022D7 RID: 8919
	public List<global::Item> itemList = new List<global::Item>();

	// Token: 0x040022D8 RID: 8920
	public float temperature = 15f;

	// Token: 0x040022D9 RID: 8921
	public global::Item parent;

	// Token: 0x040022DA RID: 8922
	public global::BasePlayer playerOwner;

	// Token: 0x040022DB RID: 8923
	public global::BaseEntity entityOwner;

	// Token: 0x040022DC RID: 8924
	public bool isServer;

	// Token: 0x040022DD RID: 8925
	public int maxStackSize;

	// Token: 0x040022DF RID: 8927
	public Func<global::Item, int, bool> canAcceptItem;

	// Token: 0x040022E0 RID: 8928
	public Func<global::Item, int, bool> slotIsReserved;

	// Token: 0x040022E1 RID: 8929
	public Action<global::Item, bool> onItemAddedRemoved;

	// Token: 0x040022E2 RID: 8930
	public Action<global::Item> onPreItemRemove;

	// Token: 0x02000D18 RID: 3352
	[Flags]
	public enum Flag
	{
		// Token: 0x04004500 RID: 17664
		IsPlayer = 1,
		// Token: 0x04004501 RID: 17665
		Clothing = 2,
		// Token: 0x04004502 RID: 17666
		Belt = 4,
		// Token: 0x04004503 RID: 17667
		SingleType = 8,
		// Token: 0x04004504 RID: 17668
		IsLocked = 16,
		// Token: 0x04004505 RID: 17669
		ShowSlotsOnIcon = 32,
		// Token: 0x04004506 RID: 17670
		NoBrokenItems = 64,
		// Token: 0x04004507 RID: 17671
		NoItemInput = 128,
		// Token: 0x04004508 RID: 17672
		ContentsHidden = 256
	}

	// Token: 0x02000D19 RID: 3353
	[Flags]
	public enum ContentsType
	{
		// Token: 0x0400450A RID: 17674
		Generic = 1,
		// Token: 0x0400450B RID: 17675
		Liquid = 2
	}

	// Token: 0x02000D1A RID: 3354
	public enum LimitStack
	{
		// Token: 0x0400450D RID: 17677
		None,
		// Token: 0x0400450E RID: 17678
		Existing,
		// Token: 0x0400450F RID: 17679
		All
	}

	// Token: 0x02000D1B RID: 3355
	public enum CanAcceptResult
	{
		// Token: 0x04004511 RID: 17681
		CanAccept,
		// Token: 0x04004512 RID: 17682
		CannotAccept,
		// Token: 0x04004513 RID: 17683
		CannotAcceptRightNow
	}
}
