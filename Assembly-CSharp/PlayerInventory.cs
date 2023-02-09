using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000A9 RID: 169
public class PlayerInventory : EntityComponent<global::BasePlayer>
{
	// Token: 0x06000F63 RID: 3939 RVA: 0x0007F374 File Offset: 0x0007D574
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PlayerInventory.OnRpcMessage", 0))
		{
			if (rpc == 3482449460U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ItemCmd ");
				}
				using (TimeWarning.New("ItemCmd", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(3482449460U, "ItemCmd", this.GetBaseEntity(), player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ItemCmd(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ItemCmd");
					}
				}
				return true;
			}
			if (rpc == 3041092525U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - MoveItem ");
				}
				using (TimeWarning.New("MoveItem", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.FromOwner.Test(3041092525U, "MoveItem", this.GetBaseEntity(), player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.MoveItem(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in MoveItem");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000F64 RID: 3940 RVA: 0x0007F674 File Offset: 0x0007D874
	protected void Initialize()
	{
		this.containerMain = new global::ItemContainer();
		this.containerMain.SetFlag(global::ItemContainer.Flag.IsPlayer, true);
		this.containerBelt = new global::ItemContainer();
		this.containerBelt.SetFlag(global::ItemContainer.Flag.IsPlayer, true);
		this.containerBelt.SetFlag(global::ItemContainer.Flag.Belt, true);
		this.containerWear = new global::ItemContainer();
		this.containerWear.SetFlag(global::ItemContainer.Flag.IsPlayer, true);
		this.containerWear.SetFlag(global::ItemContainer.Flag.Clothing, true);
		this.crafting = base.GetComponent<ItemCrafter>();
		if (this.crafting != null)
		{
			this.crafting.AddContainer(this.containerMain);
			this.crafting.AddContainer(this.containerBelt);
		}
		this.loot = base.GetComponent<PlayerLoot>();
		if (!this.loot)
		{
			this.loot = base.gameObject.AddComponent<PlayerLoot>();
		}
	}

	// Token: 0x06000F65 RID: 3941 RVA: 0x0007F74C File Offset: 0x0007D94C
	public void DoDestroy()
	{
		if (this.containerMain != null)
		{
			this.containerMain.Kill();
			this.containerMain = null;
		}
		if (this.containerBelt != null)
		{
			this.containerBelt.Kill();
			this.containerBelt = null;
		}
		if (this.containerWear != null)
		{
			this.containerWear.Kill();
			this.containerWear = null;
		}
	}

	// Token: 0x06000F66 RID: 3942 RVA: 0x0007F7A8 File Offset: 0x0007D9A8
	public void ServerInit(global::BasePlayer owner)
	{
		this.Initialize();
		this.containerMain.ServerInitialize(null, 24);
		if (this.containerMain.uid == 0U)
		{
			this.containerMain.GiveUID();
		}
		this.containerBelt.ServerInitialize(null, 6);
		if (this.containerBelt.uid == 0U)
		{
			this.containerBelt.GiveUID();
		}
		this.containerWear.ServerInitialize(null, 7);
		if (this.containerWear.uid == 0U)
		{
			this.containerWear.GiveUID();
		}
		this.containerMain.playerOwner = owner;
		this.containerBelt.playerOwner = owner;
		this.containerWear.playerOwner = owner;
		this.containerWear.onItemAddedRemoved = new Action<global::Item, bool>(this.OnClothingChanged);
		this.containerWear.canAcceptItem = new Func<global::Item, int, bool>(this.CanWearItem);
		this.containerBelt.canAcceptItem = new Func<global::Item, int, bool>(this.CanEquipItem);
		this.containerMain.onPreItemRemove = new Action<global::Item>(this.OnItemRemoved);
		this.containerWear.onPreItemRemove = new Action<global::Item>(this.OnItemRemoved);
		this.containerBelt.onPreItemRemove = new Action<global::Item>(this.OnItemRemoved);
		this.containerMain.onDirty += this.OnContentsDirty;
		this.containerBelt.onDirty += this.OnContentsDirty;
		this.containerWear.onDirty += this.OnContentsDirty;
		this.containerBelt.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedOrRemoved);
		this.containerMain.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedOrRemoved);
	}

	// Token: 0x06000F67 RID: 3943 RVA: 0x0007F94C File Offset: 0x0007DB4C
	public void OnItemAddedOrRemoved(global::Item item, bool bAdded)
	{
		if (item.info.isHoldable)
		{
			base.Invoke(new Action(this.UpdatedVisibleHolsteredItems), 0.1f);
		}
		if (!bAdded)
		{
			return;
		}
		global::BasePlayer baseEntity = base.baseEntity;
		if (!baseEntity.HasPlayerFlag(global::BasePlayer.PlayerFlags.DisplaySash) && baseEntity.IsHostileItem(item))
		{
			base.baseEntity.SetPlayerFlag(global::BasePlayer.PlayerFlags.DisplaySash, true);
		}
		if (bAdded)
		{
			baseEntity.ProcessMissionEvent(BaseMission.MissionEventType.ACQUIRE_ITEM, item.info.shortname, (float)item.amount);
		}
	}

	// Token: 0x06000F68 RID: 3944 RVA: 0x0007F9CC File Offset: 0x0007DBCC
	public void UpdatedVisibleHolsteredItems()
	{
		List<global::HeldEntity> list = Facepunch.Pool.GetList<global::HeldEntity>();
		List<global::Item> list2 = Facepunch.Pool.GetList<global::Item>();
		this.AllItemsNoAlloc(ref list2);
		foreach (global::Item item in list2)
		{
			if (item.info.isHoldable && !(item.GetHeldEntity() == null))
			{
				global::HeldEntity component = item.GetHeldEntity().GetComponent<global::HeldEntity>();
				if (!(component == null))
				{
					list.Add(component);
				}
			}
		}
		Facepunch.Pool.FreeList<global::Item>(ref list2);
		IEnumerable<global::HeldEntity> enumerable = from x in list
		orderby x.hostileScore descending
		select x;
		bool flag = true;
		bool flag2 = true;
		bool flag3 = true;
		foreach (global::HeldEntity heldEntity in enumerable)
		{
			if (!(heldEntity == null) && heldEntity.holsterInfo.displayWhenHolstered)
			{
				if (flag3 && !heldEntity.IsDeployed() && heldEntity.holsterInfo.slot == global::HeldEntity.HolsterInfo.HolsterSlot.BACK)
				{
					heldEntity.SetVisibleWhileHolstered(true);
					flag3 = false;
				}
				else if (flag2 && !heldEntity.IsDeployed() && heldEntity.holsterInfo.slot == global::HeldEntity.HolsterInfo.HolsterSlot.RIGHT_THIGH)
				{
					heldEntity.SetVisibleWhileHolstered(true);
					flag2 = false;
				}
				else if (flag && !heldEntity.IsDeployed() && heldEntity.holsterInfo.slot == global::HeldEntity.HolsterInfo.HolsterSlot.LEFT_THIGH)
				{
					heldEntity.SetVisibleWhileHolstered(true);
					flag = false;
				}
				else
				{
					heldEntity.SetVisibleWhileHolstered(false);
				}
			}
		}
		Facepunch.Pool.FreeList<global::HeldEntity>(ref list);
	}

	// Token: 0x06000F69 RID: 3945 RVA: 0x0007FB78 File Offset: 0x0007DD78
	private void OnContentsDirty()
	{
		if (base.baseEntity != null)
		{
			base.baseEntity.InvalidateNetworkCache();
		}
	}

	// Token: 0x06000F6A RID: 3946 RVA: 0x0007FB94 File Offset: 0x0007DD94
	private bool CanMoveItemsFrom(global::BaseEntity entity, global::Item item)
	{
		global::PlayerInventory.ICanMoveFrom canMoveFrom;
		return ((canMoveFrom = (entity as global::PlayerInventory.ICanMoveFrom)) == null || canMoveFrom.CanMoveFrom(base.baseEntity, item)) && (!BaseGameMode.GetActiveGameMode(true) || BaseGameMode.GetActiveGameMode(true).CanMoveItemsFrom(this, entity, item));
	}

	// Token: 0x06000F6B RID: 3947 RVA: 0x0007FBDC File Offset: 0x0007DDDC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	private void ItemCmd(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != null && msg.player.IsWounded())
		{
			return;
		}
		uint id = msg.read.UInt32();
		string text = msg.read.String(256);
		global::Item item = this.FindItemUID(id);
		if (item == null)
		{
			return;
		}
		if (item.IsLocked() || (item.parent != null && item.parent.IsLocked()))
		{
			return;
		}
		if (!this.CanMoveItemsFrom(item.GetEntityOwner(), item))
		{
			return;
		}
		if (text == "drop")
		{
			int num = item.amount;
			if (msg.read.Unread >= 4)
			{
				num = msg.read.Int32();
			}
			base.baseEntity.stats.Add("item_drop", 1, (global::Stats)5);
			if (num < item.amount)
			{
				global::Item item2 = item.SplitItem(num);
				if (item2 != null)
				{
					item2.Drop(base.baseEntity.GetDropPosition(), base.baseEntity.GetDropVelocity(), default(Quaternion));
				}
			}
			else
			{
				item.Drop(base.baseEntity.GetDropPosition(), base.baseEntity.GetDropVelocity(), default(Quaternion));
			}
			base.baseEntity.SignalBroadcast(global::BaseEntity.Signal.Gesture, "drop_item", null);
			return;
		}
		item.ServerCommand(text, base.baseEntity);
		ItemManager.DoRemoves();
		this.ServerUpdate(0f);
	}

	// Token: 0x06000F6C RID: 3948 RVA: 0x0007FD3C File Offset: 0x0007DF3C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.FromOwner]
	public void MoveItem(global::BaseEntity.RPCMessage msg)
	{
		uint num = msg.read.UInt32();
		uint num2 = msg.read.UInt32();
		int iTargetPos = (int)msg.read.Int8();
		int num3 = (int)msg.read.UInt32();
		global::Item item = this.FindItemUID(num);
		if (item == null)
		{
			msg.player.ChatMessage("Invalid item (" + num + ")");
			return;
		}
		global::BaseEntity entityOwner = item.GetEntityOwner();
		if (!this.CanMoveItemsFrom(entityOwner, item))
		{
			msg.player.ChatMessage("Cannot move item!");
			return;
		}
		if (num3 <= 0)
		{
			num3 = item.amount;
		}
		num3 = Mathf.Clamp(num3, 1, item.MaxStackable());
		if (msg.player.GetActiveItem() == item)
		{
			msg.player.UpdateActiveItem(0U);
		}
		if (num2 == 0U)
		{
			global::BaseEntity baseEntity = entityOwner;
			if (this.loot.containers.Count > 0)
			{
				baseEntity = ((entityOwner == base.baseEntity) ? this.loot.entitySource : base.baseEntity);
			}
			IIdealSlotEntity idealSlotEntity;
			if ((idealSlotEntity = (baseEntity as IIdealSlotEntity)) != null)
			{
				num2 = idealSlotEntity.GetIdealContainer(base.baseEntity, item);
			}
			global::ItemContainer parent = item.parent;
			if (parent != null && parent.IsLocked())
			{
				msg.player.ChatMessage("Container is locked!");
				return;
			}
			if (num2 == 0U)
			{
				if (baseEntity == this.loot.entitySource)
				{
					foreach (global::ItemContainer itemContainer in this.loot.containers)
					{
						if (!itemContainer.PlayerItemInputBlocked() && !itemContainer.IsLocked() && item.MoveToContainer(itemContainer, -1, true, false, base.baseEntity, true))
						{
							break;
						}
					}
					return;
				}
				if (!this.GiveItem(item, null))
				{
					msg.player.ChatMessage("GiveItem failed!");
				}
				return;
			}
		}
		global::ItemContainer itemContainer2 = this.FindContainer(num2);
		if (itemContainer2 == null)
		{
			msg.player.ChatMessage("Invalid container (" + num2 + ")");
			return;
		}
		if (itemContainer2.IsLocked())
		{
			msg.player.ChatMessage("Container is locked!");
			return;
		}
		if (itemContainer2.PlayerItemInputBlocked())
		{
			msg.player.ChatMessage("Container does not accept player items!");
			return;
		}
		using (TimeWarning.New("Split", 0))
		{
			if (item.amount > num3)
			{
				int split_Amount = num3;
				if (itemContainer2.maxStackSize > 0)
				{
					split_Amount = Mathf.Min(num3, itemContainer2.maxStackSize);
				}
				global::Item item2 = item.SplitItem(split_Amount);
				if (!item2.MoveToContainer(itemContainer2, iTargetPos, true, false, base.baseEntity, true))
				{
					item.amount += item2.amount;
					item2.Remove(0f);
				}
				ItemManager.DoRemoves();
				this.ServerUpdate(0f);
				return;
			}
		}
		if (!item.MoveToContainer(itemContainer2, iTargetPos, true, false, base.baseEntity, true))
		{
			return;
		}
		ItemManager.DoRemoves();
		this.ServerUpdate(0f);
	}

	// Token: 0x06000F6D RID: 3949 RVA: 0x00080058 File Offset: 0x0007E258
	private void OnClothingChanged(global::Item item, bool bAdded)
	{
		base.baseEntity.SV_ClothingChanged();
		ItemManager.DoRemoves();
		this.ServerUpdate(0f);
	}

	// Token: 0x06000F6E RID: 3950 RVA: 0x00080075 File Offset: 0x0007E275
	private void OnItemRemoved(global::Item item)
	{
		base.baseEntity.InvalidateNetworkCache();
	}

	// Token: 0x06000F6F RID: 3951 RVA: 0x00080084 File Offset: 0x0007E284
	private bool CanEquipItem(global::Item item, int targetSlot)
	{
		ItemModContainerRestriction component = item.info.GetComponent<ItemModContainerRestriction>();
		if (component == null)
		{
			return true;
		}
		foreach (global::Item item2 in this.containerBelt.itemList.ToArray())
		{
			if (item2 != item)
			{
				ItemModContainerRestriction component2 = item2.info.GetComponent<ItemModContainerRestriction>();
				if (!(component2 == null) && !component.CanExistWith(component2) && !item2.MoveToContainer(this.containerMain, -1, true, false, null, true))
				{
					item2.Drop(base.baseEntity.GetDropPosition(), base.baseEntity.GetDropVelocity(), default(Quaternion));
				}
			}
		}
		return true;
	}

	// Token: 0x06000F70 RID: 3952 RVA: 0x0008012C File Offset: 0x0007E32C
	private bool CanWearItem(global::Item item, int targetSlot)
	{
		ItemModWearable component = item.info.GetComponent<ItemModWearable>();
		if (component == null)
		{
			return false;
		}
		if (component.npcOnly && !Inventory.disableAttireLimitations)
		{
			global::BasePlayer baseEntity = base.baseEntity;
			if (baseEntity != null && !baseEntity.IsNpc)
			{
				return false;
			}
		}
		foreach (global::Item item2 in this.containerWear.itemList.ToArray())
		{
			if (item2 != item)
			{
				ItemModWearable component2 = item2.info.GetComponent<ItemModWearable>();
				if (!(component2 == null) && !Inventory.disableAttireLimitations && !component.CanExistWith(component2))
				{
					bool flag = false;
					if (item.parent == this.containerBelt)
					{
						flag = item2.MoveToContainer(this.containerBelt, -1, true, false, null, true);
					}
					if (!flag && !item2.MoveToContainer(this.containerMain, -1, true, false, null, true))
					{
						item2.Drop(base.baseEntity.GetDropPosition(), base.baseEntity.GetDropVelocity(), default(Quaternion));
					}
				}
			}
		}
		return true;
	}

	// Token: 0x06000F71 RID: 3953 RVA: 0x0008023C File Offset: 0x0007E43C
	public void ServerUpdate(float delta)
	{
		this.loot.Check();
		if (delta > 0f)
		{
			this.crafting.ServerUpdate(delta);
		}
		float currentTemperature = base.baseEntity.currentTemperature;
		this.UpdateContainer(delta, global::PlayerInventory.Type.Main, this.containerMain, false, currentTemperature);
		this.UpdateContainer(delta, global::PlayerInventory.Type.Belt, this.containerBelt, true, currentTemperature);
		this.UpdateContainer(delta, global::PlayerInventory.Type.Wear, this.containerWear, true, currentTemperature);
	}

	// Token: 0x06000F72 RID: 3954 RVA: 0x000802A4 File Offset: 0x0007E4A4
	public void UpdateContainer(float delta, global::PlayerInventory.Type type, global::ItemContainer container, bool bSendInventoryToEveryone, float temperature)
	{
		if (container == null)
		{
			return;
		}
		container.temperature = temperature;
		if (delta > 0f)
		{
			container.OnCycle(delta);
		}
		if (container.dirty)
		{
			this.SendUpdatedInventory(type, container, bSendInventoryToEveryone);
			base.baseEntity.InvalidateNetworkCache();
		}
	}

	// Token: 0x06000F73 RID: 3955 RVA: 0x000802E0 File Offset: 0x0007E4E0
	public void SendSnapshot()
	{
		using (TimeWarning.New("PlayerInventory.SendSnapshot", 0))
		{
			this.SendUpdatedInventory(global::PlayerInventory.Type.Main, this.containerMain, false);
			this.SendUpdatedInventory(global::PlayerInventory.Type.Belt, this.containerBelt, true);
			this.SendUpdatedInventory(global::PlayerInventory.Type.Wear, this.containerWear, true);
		}
	}

	// Token: 0x06000F74 RID: 3956 RVA: 0x00080340 File Offset: 0x0007E540
	public void SendUpdatedInventory(global::PlayerInventory.Type type, global::ItemContainer container, bool bSendInventoryToEveryone = false)
	{
		using (UpdateItemContainer updateItemContainer = Facepunch.Pool.Get<UpdateItemContainer>())
		{
			updateItemContainer.type = (int)type;
			if (container != null)
			{
				container.dirty = false;
				updateItemContainer.container = Facepunch.Pool.Get<List<ProtoBuf.ItemContainer>>();
				updateItemContainer.container.Add(container.Save());
			}
			if (bSendInventoryToEveryone)
			{
				base.baseEntity.ClientRPC<UpdateItemContainer>(null, "UpdatedItemContainer", updateItemContainer);
			}
			else
			{
				base.baseEntity.ClientRPCPlayer<UpdateItemContainer>(null, base.baseEntity, "UpdatedItemContainer", updateItemContainer);
			}
		}
	}

	// Token: 0x06000F75 RID: 3957 RVA: 0x000803CC File Offset: 0x0007E5CC
	public global::Item FindItemUID(uint id)
	{
		if (id == 0U)
		{
			return null;
		}
		if (this.containerMain != null)
		{
			global::Item item = this.containerMain.FindItemByUID(id);
			if (item != null && item.IsValid())
			{
				return item;
			}
		}
		if (this.containerBelt != null)
		{
			global::Item item2 = this.containerBelt.FindItemByUID(id);
			if (item2 != null && item2.IsValid())
			{
				return item2;
			}
		}
		if (this.containerWear != null)
		{
			global::Item item3 = this.containerWear.FindItemByUID(id);
			if (item3 != null && item3.IsValid())
			{
				return item3;
			}
		}
		return this.loot.FindItem(id);
	}

	// Token: 0x06000F76 RID: 3958 RVA: 0x00080450 File Offset: 0x0007E650
	public global::Item FindItemID(string itemName)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemName);
		if (itemDefinition == null)
		{
			return null;
		}
		return this.FindItemID(itemDefinition.itemid);
	}

	// Token: 0x06000F77 RID: 3959 RVA: 0x0008047C File Offset: 0x0007E67C
	public global::Item FindItemID(int id)
	{
		if (this.containerMain != null)
		{
			global::Item item = this.containerMain.FindItemByItemID(id);
			if (item != null && item.IsValid())
			{
				return item;
			}
		}
		if (this.containerBelt != null)
		{
			global::Item item2 = this.containerBelt.FindItemByItemID(id);
			if (item2 != null && item2.IsValid())
			{
				return item2;
			}
		}
		if (this.containerWear != null)
		{
			global::Item item3 = this.containerWear.FindItemByItemID(id);
			if (item3 != null && item3.IsValid())
			{
				return item3;
			}
		}
		return null;
	}

	// Token: 0x06000F78 RID: 3960 RVA: 0x000804F0 File Offset: 0x0007E6F0
	public global::Item FindBySubEntityID(uint subEntityID)
	{
		if (this.containerMain != null)
		{
			global::Item item = this.containerMain.FindBySubEntityID(subEntityID);
			if (item != null && item.IsValid())
			{
				return item;
			}
		}
		if (this.containerBelt != null)
		{
			global::Item item2 = this.containerBelt.FindBySubEntityID(subEntityID);
			if (item2 != null && item2.IsValid())
			{
				return item2;
			}
		}
		if (this.containerWear != null)
		{
			global::Item item3 = this.containerWear.FindBySubEntityID(subEntityID);
			if (item3 != null && item3.IsValid())
			{
				return item3;
			}
		}
		return null;
	}

	// Token: 0x06000F79 RID: 3961 RVA: 0x00080564 File Offset: 0x0007E764
	public List<global::Item> FindItemIDs(int id)
	{
		List<global::Item> list = new List<global::Item>();
		if (this.containerMain != null)
		{
			list.AddRange(this.containerMain.FindItemsByItemID(id));
		}
		if (this.containerBelt != null)
		{
			list.AddRange(this.containerBelt.FindItemsByItemID(id));
		}
		if (this.containerWear != null)
		{
			list.AddRange(this.containerWear.FindItemsByItemID(id));
		}
		return list;
	}

	// Token: 0x06000F7A RID: 3962 RVA: 0x000805C8 File Offset: 0x0007E7C8
	public global::ItemContainer FindContainer(uint id)
	{
		global::ItemContainer result;
		using (TimeWarning.New("FindContainer", 0))
		{
			global::ItemContainer itemContainer = this.containerMain.FindContainer(id);
			if (itemContainer != null)
			{
				result = itemContainer;
			}
			else
			{
				itemContainer = this.containerBelt.FindContainer(id);
				if (itemContainer != null)
				{
					result = itemContainer;
				}
				else
				{
					itemContainer = this.containerWear.FindContainer(id);
					if (itemContainer != null)
					{
						result = itemContainer;
					}
					else
					{
						result = this.loot.FindContainer(id);
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06000F7B RID: 3963 RVA: 0x00080648 File Offset: 0x0007E848
	public global::ItemContainer GetContainer(global::PlayerInventory.Type id)
	{
		if (id == global::PlayerInventory.Type.Main)
		{
			return this.containerMain;
		}
		if (global::PlayerInventory.Type.Belt == id)
		{
			return this.containerBelt;
		}
		if (global::PlayerInventory.Type.Wear == id)
		{
			return this.containerWear;
		}
		return null;
	}

	// Token: 0x06000F7C RID: 3964 RVA: 0x0008066C File Offset: 0x0007E86C
	public bool GiveItem(global::Item item, global::ItemContainer container = null)
	{
		if (item == null)
		{
			return false;
		}
		int iTargetPos = -1;
		this.GetIdealPickupContainer(item, ref container, ref iTargetPos);
		return (container != null && item.MoveToContainer(container, iTargetPos, true, false, null, true)) || item.MoveToContainer(this.containerMain, -1, true, false, null, true) || item.MoveToContainer(this.containerBelt, -1, true, false, null, true);
	}

	// Token: 0x06000F7D RID: 3965 RVA: 0x000806CC File Offset: 0x0007E8CC
	protected void GetIdealPickupContainer(global::Item item, ref global::ItemContainer container, ref int position)
	{
		if (item.MaxStackable() > 1)
		{
			if (this.containerBelt != null && this.containerBelt.FindItemByItemID(item.info.itemid) != null)
			{
				container = this.containerBelt;
				return;
			}
			if (this.containerMain != null && this.containerMain.FindItemByItemID(item.info.itemid) != null)
			{
				container = this.containerMain;
				return;
			}
		}
		if (item.info.isUsable && !item.info.HasFlag(ItemDefinition.Flag.NotStraightToBelt))
		{
			container = this.containerBelt;
			return;
		}
	}

	// Token: 0x06000F7E RID: 3966 RVA: 0x00080758 File Offset: 0x0007E958
	public void Strip()
	{
		this.containerMain.Clear();
		this.containerBelt.Clear();
		this.containerWear.Clear();
		ItemManager.DoRemoves();
	}

	// Token: 0x06000F7F RID: 3967 RVA: 0x00080780 File Offset: 0x0007E980
	public static bool IsBirthday()
	{
		if (global::PlayerInventory.forceBirthday)
		{
			return true;
		}
		if (UnityEngine.Time.time < global::PlayerInventory.nextCheckTime)
		{
			return global::PlayerInventory.wasBirthday;
		}
		global::PlayerInventory.nextCheckTime = UnityEngine.Time.time + 60f;
		DateTime now = DateTime.Now;
		global::PlayerInventory.wasBirthday = (now.Day == 11 && now.Month == 12);
		return global::PlayerInventory.wasBirthday;
	}

	// Token: 0x06000F80 RID: 3968 RVA: 0x000807E1 File Offset: 0x0007E9E1
	public static bool IsChristmas()
	{
		return XMas.enabled;
	}

	// Token: 0x06000F81 RID: 3969 RVA: 0x000807E8 File Offset: 0x0007E9E8
	public void GiveDefaultItems()
	{
		this.Strip();
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode != null && activeGameMode.HasLoadouts())
		{
			BaseGameMode.GetActiveGameMode(true).LoadoutPlayer(base.baseEntity);
			return;
		}
		this.<GiveDefaultItems>g__GiveDefaultItemWithSkin|40_0("client.rockskin", "rock");
		this.<GiveDefaultItems>g__GiveDefaultItemWithSkin|40_0("client.torchskin", "torch");
		if (global::PlayerInventory.IsBirthday())
		{
			this.GiveItem(ItemManager.CreateByName("cakefiveyear", 1, 0UL), this.containerBelt);
			this.GiveItem(ItemManager.CreateByName("partyhat", 1, 0UL), this.containerWear);
		}
		if (global::PlayerInventory.IsChristmas())
		{
			this.GiveItem(ItemManager.CreateByName("snowball", 1, 0UL), this.containerBelt);
			this.GiveItem(ItemManager.CreateByName("snowball", 1, 0UL), this.containerBelt);
			this.GiveItem(ItemManager.CreateByName("snowball", 1, 0UL), this.containerBelt);
		}
	}

	// Token: 0x06000F82 RID: 3970 RVA: 0x000808D8 File Offset: 0x0007EAD8
	public ProtoBuf.PlayerInventory Save(bool bForDisk)
	{
		ProtoBuf.PlayerInventory playerInventory = Facepunch.Pool.Get<ProtoBuf.PlayerInventory>();
		if (bForDisk)
		{
			playerInventory.invMain = this.containerMain.Save();
		}
		playerInventory.invBelt = this.containerBelt.Save();
		playerInventory.invWear = this.containerWear.Save();
		return playerInventory;
	}

	// Token: 0x06000F83 RID: 3971 RVA: 0x00080924 File Offset: 0x0007EB24
	public void Load(ProtoBuf.PlayerInventory msg)
	{
		if (msg.invMain != null)
		{
			this.containerMain.Load(msg.invMain);
		}
		if (msg.invBelt != null)
		{
			this.containerBelt.Load(msg.invBelt);
		}
		if (msg.invWear != null)
		{
			this.containerWear.Load(msg.invWear);
		}
	}

	// Token: 0x06000F84 RID: 3972 RVA: 0x0008097C File Offset: 0x0007EB7C
	public int Take(List<global::Item> collect, int itemid, int amount)
	{
		int num = 0;
		if (this.containerMain != null)
		{
			int num2 = this.containerMain.Take(collect, itemid, amount);
			num += num2;
			amount -= num2;
		}
		if (amount <= 0)
		{
			return num;
		}
		if (this.containerBelt != null)
		{
			int num3 = this.containerBelt.Take(collect, itemid, amount);
			num += num3;
			amount -= num3;
		}
		if (amount <= 0)
		{
			return num;
		}
		if (this.containerWear != null)
		{
			int num4 = this.containerWear.Take(collect, itemid, amount);
			num += num4;
			amount -= num4;
		}
		return num;
	}

	// Token: 0x06000F85 RID: 3973 RVA: 0x000809F8 File Offset: 0x0007EBF8
	public int GetAmount(int itemid)
	{
		if (itemid == 0)
		{
			return 0;
		}
		int num = 0;
		if (this.containerMain != null)
		{
			num += this.containerMain.GetAmount(itemid, true);
		}
		if (this.containerBelt != null)
		{
			num += this.containerBelt.GetAmount(itemid, true);
		}
		if (this.containerWear != null)
		{
			num += this.containerWear.GetAmount(itemid, true);
		}
		return num;
	}

	// Token: 0x06000F86 RID: 3974 RVA: 0x00080A58 File Offset: 0x0007EC58
	public global::Item[] AllItems()
	{
		List<global::Item> list = new List<global::Item>();
		if (this.containerMain != null)
		{
			list.AddRange(this.containerMain.itemList);
		}
		if (this.containerBelt != null)
		{
			list.AddRange(this.containerBelt.itemList);
		}
		if (this.containerWear != null)
		{
			list.AddRange(this.containerWear.itemList);
		}
		return list.ToArray();
	}

	// Token: 0x06000F87 RID: 3975 RVA: 0x00080ABC File Offset: 0x0007ECBC
	public int AllItemsNoAlloc(ref List<global::Item> items)
	{
		items.Clear();
		if (this.containerMain != null)
		{
			items.AddRange(this.containerMain.itemList);
		}
		if (this.containerBelt != null)
		{
			items.AddRange(this.containerBelt.itemList);
		}
		if (this.containerWear != null)
		{
			items.AddRange(this.containerWear.itemList);
		}
		return items.Count;
	}

	// Token: 0x06000F88 RID: 3976 RVA: 0x00080B25 File Offset: 0x0007ED25
	public void FindAmmo(List<global::Item> list, AmmoTypes ammoType)
	{
		if (this.containerMain != null)
		{
			this.containerMain.FindAmmo(list, ammoType);
		}
		if (this.containerBelt != null)
		{
			this.containerBelt.FindAmmo(list, ammoType);
		}
	}

	// Token: 0x06000F89 RID: 3977 RVA: 0x00080B51 File Offset: 0x0007ED51
	public bool HasAmmo(AmmoTypes ammoType)
	{
		return this.containerMain.HasAmmo(ammoType) || this.containerBelt.HasAmmo(ammoType);
	}

	// Token: 0x06000F8C RID: 3980 RVA: 0x00080B78 File Offset: 0x0007ED78
	[CompilerGenerated]
	private void <GiveDefaultItems>g__GiveDefaultItemWithSkin|40_0(string convarSkinName, string itemShortName)
	{
		ulong num = 0UL;
		int infoInt = base.baseEntity.GetInfoInt(convarSkinName, 0);
		bool flag = false;
		bool flag2 = false;
		if (infoInt > 0 && (base.baseEntity.blueprints.steamInventory.HasItem(infoInt) || flag2))
		{
			ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemShortName);
			if (itemDefinition != null && ItemDefinition.FindSkin(itemDefinition.itemid, infoInt) != 0UL)
			{
				IPlayerItemDefinition itemDefinition2 = PlatformService.Instance.GetItemDefinition(infoInt);
				if (itemDefinition2 != null)
				{
					num = itemDefinition2.WorkshopDownload;
				}
				if (num == 0UL && itemDefinition.skins != null)
				{
					foreach (ItemSkinDirectory.Skin skin in itemDefinition.skins)
					{
						ItemSkin itemSkin;
						if (skin.id == infoInt && skin.invItem != null && (itemSkin = (skin.invItem as ItemSkin)) != null && itemSkin.Redirect != null)
						{
							this.GiveItem(ItemManager.CreateByName(itemSkin.Redirect.shortname, 1, 0UL), this.containerBelt);
							flag = true;
							break;
						}
					}
				}
			}
		}
		if (!flag)
		{
			this.GiveItem(ItemManager.CreateByName(itemShortName, 1, num), this.containerBelt);
		}
	}

	// Token: 0x040009E7 RID: 2535
	public global::ItemContainer containerMain;

	// Token: 0x040009E8 RID: 2536
	public global::ItemContainer containerBelt;

	// Token: 0x040009E9 RID: 2537
	public global::ItemContainer containerWear;

	// Token: 0x040009EA RID: 2538
	public ItemCrafter crafting;

	// Token: 0x040009EB RID: 2539
	public PlayerLoot loot;

	// Token: 0x040009EC RID: 2540
	[ServerVar]
	public static bool forceBirthday;

	// Token: 0x040009ED RID: 2541
	private static float nextCheckTime;

	// Token: 0x040009EE RID: 2542
	private static bool wasBirthday;

	// Token: 0x02000BA4 RID: 2980
	public enum Type
	{
		// Token: 0x04003F03 RID: 16131
		Main,
		// Token: 0x04003F04 RID: 16132
		Belt,
		// Token: 0x04003F05 RID: 16133
		Wear
	}

	// Token: 0x02000BA5 RID: 2981
	public interface ICanMoveFrom
	{
		// Token: 0x06004B0A RID: 19210
		bool CanMoveFrom(global::BasePlayer player, global::Item item);
	}
}
