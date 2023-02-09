using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000D5 RID: 213
public class StorageContainer : global::DecayEntity, IItemContainerEntity, IIdealSlotEntity, LootPanel.IHasLootPanel, IContainerSounds, global::PlayerInventory.ICanMoveFrom
{
	// Token: 0x06001267 RID: 4711 RVA: 0x00093B0C File Offset: 0x00091D0C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("StorageContainer.OnRpcMessage", 0))
		{
			if (rpc == 331989034U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenLoot ");
				}
				using (TimeWarning.New("RPC_OpenLoot", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(331989034U, "RPC_OpenLoot", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_OpenLoot(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_OpenLoot");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000186 RID: 390
	// (get) Token: 0x06001268 RID: 4712 RVA: 0x00093C74 File Offset: 0x00091E74
	public Translate.Phrase LootPanelTitle
	{
		get
		{
			return this.panelTitle;
		}
	}

	// Token: 0x06001269 RID: 4713 RVA: 0x00093C7C File Offset: 0x00091E7C
	public override void ResetState()
	{
		base.ResetState();
		if (base.isServer && this.inventory != null)
		{
			this.inventory.Clear();
			this.inventory = null;
		}
	}

	// Token: 0x0600126A RID: 4714 RVA: 0x00093CA8 File Offset: 0x00091EA8
	public virtual void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.yellow;
		Gizmos.DrawCube(this.dropPosition, Vector3.one * 0.1f);
		Gizmos.color = Color.white;
		Gizmos.DrawRay(this.dropPosition, this.dropVelocity);
	}

	// Token: 0x17000187 RID: 391
	// (get) Token: 0x0600126B RID: 4715 RVA: 0x00093D04 File Offset: 0x00091F04
	// (set) Token: 0x0600126C RID: 4716 RVA: 0x00093D0C File Offset: 0x00091F0C
	public global::ItemContainer inventory { get; private set; }

	// Token: 0x17000188 RID: 392
	// (get) Token: 0x0600126D RID: 4717 RVA: 0x00059891 File Offset: 0x00057A91
	public Transform Transform
	{
		get
		{
			return base.transform;
		}
	}

	// Token: 0x17000189 RID: 393
	// (get) Token: 0x0600126E RID: 4718 RVA: 0x00093D15 File Offset: 0x00091F15
	public bool DropsLoot
	{
		get
		{
			return this.dropsLoot;
		}
	}

	// Token: 0x1700018A RID: 394
	// (get) Token: 0x0600126F RID: 4719 RVA: 0x00093D1D File Offset: 0x00091F1D
	public bool DropFloats
	{
		get
		{
			return this.dropFloats;
		}
	}

	// Token: 0x06001270 RID: 4720 RVA: 0x00093D25 File Offset: 0x00091F25
	public bool MoveAllInventoryItems(global::ItemContainer from)
	{
		return StorageContainer.MoveAllInventoryItems(from, this.inventory);
	}

	// Token: 0x06001271 RID: 4721 RVA: 0x00093D34 File Offset: 0x00091F34
	public static bool MoveAllInventoryItems(global::ItemContainer source, global::ItemContainer dest)
	{
		bool flag = true;
		for (int i = 0; i < Mathf.Min(source.capacity, dest.capacity); i++)
		{
			global::Item slot = source.GetSlot(i);
			if (slot != null)
			{
				bool flag2 = slot.MoveToContainer(dest, -1, true, false, null, true);
				if (flag && !flag2)
				{
					flag = false;
				}
			}
		}
		return flag;
	}

	// Token: 0x06001272 RID: 4722 RVA: 0x00093D80 File Offset: 0x00091F80
	public virtual void ReceiveInventoryFromItem(global::Item item)
	{
		if (item.contents != null)
		{
			StorageContainer.MoveAllInventoryItems(item.contents, this.inventory);
		}
	}

	// Token: 0x06001273 RID: 4723 RVA: 0x00093D9C File Offset: 0x00091F9C
	public override bool CanPickup(global::BasePlayer player)
	{
		bool flag = base.GetSlot(global::BaseEntity.Slot.Lock) != null;
		if (base.isClient)
		{
			return base.CanPickup(player) && !flag;
		}
		return (!this.pickup.requireEmptyInv || this.inventory == null || this.inventory.itemList.Count == 0) && base.CanPickup(player) && !flag;
	}

	// Token: 0x06001274 RID: 4724 RVA: 0x00093E08 File Offset: 0x00092008
	public override void OnPickedUp(global::Item createdItem, global::BasePlayer player)
	{
		base.OnPickedUp(createdItem, player);
		if (createdItem != null && createdItem.contents != null)
		{
			StorageContainer.MoveAllInventoryItems(this.inventory, createdItem.contents);
			return;
		}
		for (int i = 0; i < this.inventory.capacity; i++)
		{
			global::Item slot = this.inventory.GetSlot(i);
			if (slot != null)
			{
				slot.RemoveFromContainer();
				player.GiveItem(slot, global::BaseEntity.GiveItemReason.PickedUp);
			}
		}
	}

	// Token: 0x06001275 RID: 4725 RVA: 0x00093E6F File Offset: 0x0009206F
	public override void ServerInit()
	{
		if (this.inventory == null)
		{
			this.CreateInventory(true);
			this.OnInventoryFirstCreated(this.inventory);
		}
		base.ServerInit();
	}

	// Token: 0x06001276 RID: 4726 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnInventoryFirstCreated(global::ItemContainer container)
	{
	}

	// Token: 0x06001277 RID: 4727 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnItemAddedOrRemoved(global::Item item, bool added)
	{
	}

	// Token: 0x06001278 RID: 4728 RVA: 0x00093E92 File Offset: 0x00092092
	public virtual bool ItemFilter(global::Item item, int targetSlot)
	{
		return this.onlyAcceptCategory == ItemCategory.All || item.info.category == this.onlyAcceptCategory;
	}

	// Token: 0x06001279 RID: 4729 RVA: 0x00093EB4 File Offset: 0x000920B4
	public void CreateInventory(bool giveUID)
	{
		this.inventory = new global::ItemContainer();
		this.inventory.entityOwner = this;
		this.inventory.allowedContents = ((this.allowedContents == (global::ItemContainer.ContentsType)0) ? global::ItemContainer.ContentsType.Generic : this.allowedContents);
		this.inventory.SetOnlyAllowedItems(new ItemDefinition[]
		{
			this.allowedItem,
			this.allowedItem2
		});
		this.inventory.maxStackSize = this.maxStackSize;
		this.inventory.ServerInitialize(null, this.inventorySlots);
		if (giveUID)
		{
			this.inventory.GiveUID();
		}
		this.inventory.onDirty += this.OnInventoryDirty;
		this.inventory.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedOrRemoved);
		this.inventory.canAcceptItem = new Func<global::Item, int, bool>(this.ItemFilter);
	}

	// Token: 0x0600127A RID: 4730 RVA: 0x00093F90 File Offset: 0x00092190
	public override void PreServerLoad()
	{
		base.PreServerLoad();
		this.CreateInventory(false);
	}

	// Token: 0x0600127B RID: 4731 RVA: 0x00093F9F File Offset: 0x0009219F
	protected virtual void OnInventoryDirty()
	{
		base.InvalidateNetworkCache();
	}

	// Token: 0x0600127C RID: 4732 RVA: 0x00093FA7 File Offset: 0x000921A7
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.inventory != null && this.inventory.uid == 0U)
		{
			this.inventory.GiveUID();
		}
		base.SetFlag(global::BaseEntity.Flags.Open, false, false, true);
	}

	// Token: 0x0600127D RID: 4733 RVA: 0x00093FD9 File Offset: 0x000921D9
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (this.inventory != null)
		{
			this.inventory.Kill();
			this.inventory = null;
		}
	}

	// Token: 0x0600127E RID: 4734 RVA: 0x00093FFC File Offset: 0x000921FC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_OpenLoot(global::BaseEntity.RPCMessage rpc)
	{
		if (!this.isLootable)
		{
			return;
		}
		global::BasePlayer player = rpc.player;
		if (!player || !player.CanInteract())
		{
			return;
		}
		this.PlayerOpenLoot(player, "", true);
	}

	// Token: 0x0600127F RID: 4735 RVA: 0x00094038 File Offset: 0x00092238
	public virtual string GetPanelName()
	{
		return this.panelName;
	}

	// Token: 0x06001280 RID: 4736 RVA: 0x00094040 File Offset: 0x00092240
	public virtual bool CanMoveFrom(global::BasePlayer player, global::Item item)
	{
		return !this.inventory.IsLocked();
	}

	// Token: 0x06001281 RID: 4737 RVA: 0x00094050 File Offset: 0x00092250
	public virtual bool CanOpenLootPanel(global::BasePlayer player, string panelName)
	{
		if (!this.CanBeLooted(player))
		{
			return false;
		}
		BaseLock baseLock = base.GetSlot(global::BaseEntity.Slot.Lock) as BaseLock;
		if (baseLock != null && !baseLock.OnTryToOpen(player))
		{
			player.ChatMessage("It is locked...");
			return false;
		}
		return true;
	}

	// Token: 0x06001282 RID: 4738 RVA: 0x00094095 File Offset: 0x00092295
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return (!this.needsBuildingPrivilegeToUse || player.CanBuild()) && (!this.mustBeMountedToUse || player.isMounted) && base.CanBeLooted(player);
	}

	// Token: 0x06001283 RID: 4739 RVA: 0x000940C2 File Offset: 0x000922C2
	public virtual void AddContainers(PlayerLoot loot)
	{
		loot.AddContainer(this.inventory);
	}

	// Token: 0x06001284 RID: 4740 RVA: 0x000940D0 File Offset: 0x000922D0
	public virtual bool PlayerOpenLoot(global::BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		if (base.IsLocked())
		{
			player.ShowToast(GameTip.Styles.Red_Normal, StorageContainer.LockedMessage, Array.Empty<string>());
			return false;
		}
		if (this.onlyOneUser && base.IsOpen())
		{
			player.ShowToast(GameTip.Styles.Red_Normal, StorageContainer.InUseMessage, Array.Empty<string>());
			return false;
		}
		if (panelToOpen == "")
		{
			panelToOpen = this.panelName;
		}
		if (!this.CanOpenLootPanel(player, panelToOpen))
		{
			return false;
		}
		if (player.inventory.loot.StartLootingEntity(this, doPositionChecks))
		{
			base.SetFlag(global::BaseEntity.Flags.Open, true, false, true);
			this.AddContainers(player.inventory.loot);
			player.inventory.loot.SendImmediate();
			player.ClientRPCPlayer<string>(null, player, "RPC_OpenLootPanel", panelToOpen);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			return true;
		}
		return false;
	}

	// Token: 0x06001285 RID: 4741 RVA: 0x00059B66 File Offset: 0x00057D66
	public virtual void PlayerStoppedLooting(global::BasePlayer player)
	{
		base.SetFlag(global::BaseEntity.Flags.Open, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001286 RID: 4742 RVA: 0x00094194 File Offset: 0x00092394
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			if (this.inventory != null)
			{
				info.msg.storageBox = Facepunch.Pool.Get<StorageBox>();
				info.msg.storageBox.contents = this.inventory.Save();
				return;
			}
			Debug.LogWarning("Storage container without inventory: " + this.ToString());
		}
	}

	// Token: 0x06001287 RID: 4743 RVA: 0x000941F9 File Offset: 0x000923F9
	public override void OnKilled(HitInfo info)
	{
		this.DropItems((info != null) ? info.Initiator : null);
		base.OnKilled(info);
	}

	// Token: 0x06001288 RID: 4744 RVA: 0x00059A69 File Offset: 0x00057C69
	public void DropItems(global::BaseEntity initiator = null)
	{
		StorageContainer.DropItems(this, initiator);
	}

	// Token: 0x06001289 RID: 4745 RVA: 0x00094214 File Offset: 0x00092414
	public static void DropItems(IItemContainerEntity containerEntity, global::BaseEntity initiator = null)
	{
		global::ItemContainer inventory = containerEntity.inventory;
		if (inventory == null || inventory.itemList == null || inventory.itemList.Count == 0)
		{
			return;
		}
		if (!containerEntity.DropsLoot)
		{
			return;
		}
		if (containerEntity.ShouldDropItemsIndividually() || inventory.itemList.Count == 1)
		{
			if (initiator != null)
			{
				containerEntity.DropBonusItems(initiator, inventory);
			}
			DropUtil.DropItems(inventory, containerEntity.GetDropPosition());
			return;
		}
		string prefab = containerEntity.DropFloats ? "assets/prefabs/misc/item drop/item_drop_buoyant.prefab" : "assets/prefabs/misc/item drop/item_drop.prefab";
		inventory.Drop(prefab, containerEntity.GetDropPosition(), containerEntity.Transform.rotation) != null;
	}

	// Token: 0x0600128A RID: 4746 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void DropBonusItems(global::BaseEntity initiator, global::ItemContainer container)
	{
	}

	// Token: 0x0600128B RID: 4747 RVA: 0x000942B4 File Offset: 0x000924B4
	public override Vector3 GetDropPosition()
	{
		return base.transform.localToWorldMatrix.MultiplyPoint(this.dropPosition);
	}

	// Token: 0x0600128C RID: 4748 RVA: 0x000942DC File Offset: 0x000924DC
	public override Vector3 GetDropVelocity()
	{
		return this.GetInheritedDropVelocity() + base.transform.localToWorldMatrix.MultiplyVector(this.dropPosition);
	}

	// Token: 0x0600128D RID: 4749 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool ShouldDropItemsIndividually()
	{
		return false;
	}

	// Token: 0x0600128E RID: 4750 RVA: 0x00094310 File Offset: 0x00092510
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.storageBox != null)
		{
			if (this.inventory != null)
			{
				this.inventory.Load(info.msg.storageBox.contents);
				this.inventory.capacity = this.inventorySlots;
				return;
			}
			Debug.LogWarning("Storage container without inventory: " + this.ToString());
		}
	}

	// Token: 0x0600128F RID: 4751 RVA: 0x00040DA9 File Offset: 0x0003EFA9
	public virtual int GetIdealSlot(global::BasePlayer player, global::Item item)
	{
		return -1;
	}

	// Token: 0x06001290 RID: 4752 RVA: 0x00007074 File Offset: 0x00005274
	public virtual uint GetIdealContainer(global::BasePlayer player, global::Item item)
	{
		return 0U;
	}

	// Token: 0x06001291 RID: 4753 RVA: 0x0009437B File Offset: 0x0009257B
	public override bool HasSlot(global::BaseEntity.Slot slot)
	{
		return (this.isLockable && slot == global::BaseEntity.Slot.Lock) || (this.isMonitorable && slot == global::BaseEntity.Slot.StorageMonitor) || base.HasSlot(slot);
	}

	// Token: 0x06001292 RID: 4754 RVA: 0x0009439F File Offset: 0x0009259F
	public bool OccupiedCheck(global::BasePlayer player = null)
	{
		return (player != null && player.inventory.loot.entitySource == this) || !this.onlyOneUser || !base.IsOpen();
	}

	// Token: 0x06001293 RID: 4755 RVA: 0x000943D8 File Offset: 0x000925D8
	protected bool HasAttachedStorageAdaptor()
	{
		if (this.children == null)
		{
			return false;
		}
		using (List<global::BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current is IndustrialStorageAdaptor)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x04000B87 RID: 2951
	[Header("Storage Container")]
	public static readonly Translate.Phrase LockedMessage = new Translate.Phrase("storage.locked", "Can't loot right now");

	// Token: 0x04000B88 RID: 2952
	public static readonly Translate.Phrase InUseMessage = new Translate.Phrase("storage.in_use", "Already in use");

	// Token: 0x04000B89 RID: 2953
	public int inventorySlots = 6;

	// Token: 0x04000B8A RID: 2954
	public bool dropsLoot = true;

	// Token: 0x04000B8B RID: 2955
	public bool dropFloats;

	// Token: 0x04000B8C RID: 2956
	public bool isLootable = true;

	// Token: 0x04000B8D RID: 2957
	public bool isLockable = true;

	// Token: 0x04000B8E RID: 2958
	public bool isMonitorable;

	// Token: 0x04000B8F RID: 2959
	public string panelName = "generic";

	// Token: 0x04000B90 RID: 2960
	public Translate.Phrase panelTitle = new Translate.Phrase("loot", "Loot");

	// Token: 0x04000B91 RID: 2961
	public global::ItemContainer.ContentsType allowedContents;

	// Token: 0x04000B92 RID: 2962
	public ItemDefinition allowedItem;

	// Token: 0x04000B93 RID: 2963
	public ItemDefinition allowedItem2;

	// Token: 0x04000B94 RID: 2964
	public int maxStackSize;

	// Token: 0x04000B95 RID: 2965
	public bool needsBuildingPrivilegeToUse;

	// Token: 0x04000B96 RID: 2966
	public bool mustBeMountedToUse;

	// Token: 0x04000B97 RID: 2967
	public SoundDefinition openSound;

	// Token: 0x04000B98 RID: 2968
	public SoundDefinition closeSound;

	// Token: 0x04000B99 RID: 2969
	[Header("Item Dropping")]
	public Vector3 dropPosition;

	// Token: 0x04000B9A RID: 2970
	public Vector3 dropVelocity = Vector3.forward;

	// Token: 0x04000B9B RID: 2971
	public ItemCategory onlyAcceptCategory = ItemCategory.All;

	// Token: 0x04000B9C RID: 2972
	public bool onlyOneUser;
}
