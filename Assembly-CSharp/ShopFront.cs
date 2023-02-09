using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000C3 RID: 195
public class ShopFront : StorageContainer
{
	// Token: 0x0600112F RID: 4399 RVA: 0x0008B4FC File Offset: 0x000896FC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ShopFront.OnRpcMessage", 0))
		{
			if (rpc == 1159607245U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - AcceptClicked ");
				}
				using (TimeWarning.New("AcceptClicked", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1159607245U, "AcceptClicked", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.AcceptClicked(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in AcceptClicked");
					}
				}
				return true;
			}
			if (rpc == 3168107540U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - CancelClicked ");
				}
				using (TimeWarning.New("CancelClicked", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(3168107540U, "CancelClicked", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.CancelClicked(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in CancelClicked");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000168 RID: 360
	// (get) Token: 0x06001130 RID: 4400 RVA: 0x0008B7FC File Offset: 0x000899FC
	private float AngleDotProduct
	{
		get
		{
			return 1f - this.maxUseAngle / 90f;
		}
	}

	// Token: 0x17000169 RID: 361
	// (get) Token: 0x06001131 RID: 4401 RVA: 0x00035524 File Offset: 0x00033724
	public ItemContainer vendorInventory
	{
		get
		{
			return base.inventory;
		}
	}

	// Token: 0x06001132 RID: 4402 RVA: 0x00007074 File Offset: 0x00005274
	public bool TradeLocked()
	{
		return false;
	}

	// Token: 0x06001133 RID: 4403 RVA: 0x0008B810 File Offset: 0x00089A10
	public bool IsTradingPlayer(BasePlayer player)
	{
		return player != null && (this.IsPlayerCustomer(player) || this.IsPlayerVendor(player));
	}

	// Token: 0x06001134 RID: 4404 RVA: 0x0008B82F File Offset: 0x00089A2F
	public bool IsPlayerCustomer(BasePlayer player)
	{
		return player == this.customerPlayer;
	}

	// Token: 0x06001135 RID: 4405 RVA: 0x0008B83D File Offset: 0x00089A3D
	public bool IsPlayerVendor(BasePlayer player)
	{
		return player == this.vendorPlayer;
	}

	// Token: 0x06001136 RID: 4406 RVA: 0x0008B84C File Offset: 0x00089A4C
	public bool PlayerInVendorPos(BasePlayer player)
	{
		return Vector3.Dot(base.transform.right, (player.transform.position - base.transform.position).normalized) <= -this.AngleDotProduct;
	}

	// Token: 0x06001137 RID: 4407 RVA: 0x0008B898 File Offset: 0x00089A98
	public bool PlayerInCustomerPos(BasePlayer player)
	{
		return Vector3.Dot(base.transform.right, (player.transform.position - base.transform.position).normalized) >= this.AngleDotProduct;
	}

	// Token: 0x06001138 RID: 4408 RVA: 0x0008B8E3 File Offset: 0x00089AE3
	public bool LootEligable(BasePlayer player)
	{
		return !(player == null) && ((this.PlayerInVendorPos(player) && this.vendorPlayer == null) || (this.PlayerInCustomerPos(player) && this.customerPlayer == null));
	}

	// Token: 0x06001139 RID: 4409 RVA: 0x0008B924 File Offset: 0x00089B24
	public void ResetTrade()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved2, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
		this.vendorInventory.SetLocked(false);
		this.customerInventory.SetLocked(false);
		base.CancelInvoke(new Action(this.CompleteTrade));
	}

	// Token: 0x0600113A RID: 4410 RVA: 0x0008B988 File Offset: 0x00089B88
	public void CompleteTrade()
	{
		if (this.vendorPlayer != null && this.customerPlayer != null && base.HasFlag(BaseEntity.Flags.Reserved1) && base.HasFlag(BaseEntity.Flags.Reserved2))
		{
			try
			{
				this.swappingItems = true;
				for (int i = this.vendorInventory.capacity - 1; i >= 0; i--)
				{
					Item slot = this.vendorInventory.GetSlot(i);
					Item slot2 = this.customerInventory.GetSlot(i);
					if (this.customerPlayer && slot != null)
					{
						this.customerPlayer.GiveItem(slot, BaseEntity.GiveItemReason.Generic);
					}
					if (this.vendorPlayer && slot2 != null)
					{
						this.vendorPlayer.GiveItem(slot2, BaseEntity.GiveItemReason.Generic);
					}
				}
			}
			finally
			{
				this.swappingItems = false;
			}
			Effect.server.Run(this.transactionCompleteEffect.resourcePath, this, 0U, new Vector3(0f, 1f, 0f), Vector3.zero, null, false);
		}
		this.ResetTrade();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600113B RID: 4411 RVA: 0x0008BAA0 File Offset: 0x00089CA0
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void AcceptClicked(BaseEntity.RPCMessage msg)
	{
		if (!this.IsTradingPlayer(msg.player))
		{
			return;
		}
		if (this.vendorPlayer == null || this.customerPlayer == null)
		{
			return;
		}
		if (this.IsPlayerVendor(msg.player))
		{
			base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
			this.vendorInventory.SetLocked(true);
		}
		else if (this.IsPlayerCustomer(msg.player))
		{
			base.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
			this.customerInventory.SetLocked(true);
		}
		if (base.HasFlag(BaseEntity.Flags.Reserved1) && base.HasFlag(BaseEntity.Flags.Reserved2))
		{
			base.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
			base.Invoke(new Action(this.CompleteTrade), 2f);
		}
	}

	// Token: 0x0600113C RID: 4412 RVA: 0x0008BB6A File Offset: 0x00089D6A
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void CancelClicked(BaseEntity.RPCMessage msg)
	{
		if (!this.IsTradingPlayer(msg.player))
		{
			return;
		}
		this.vendorPlayer;
		this.customerPlayer;
		this.ResetTrade();
	}

	// Token: 0x0600113D RID: 4413 RVA: 0x0008BB99 File Offset: 0x00089D99
	public override void PreServerLoad()
	{
		base.PreServerLoad();
	}

	// Token: 0x0600113E RID: 4414 RVA: 0x0008BBA4 File Offset: 0x00089DA4
	public override void ServerInit()
	{
		base.ServerInit();
		ItemContainer vendorInventory = this.vendorInventory;
		vendorInventory.canAcceptItem = (Func<Item, int, bool>)Delegate.Combine(vendorInventory.canAcceptItem, new Func<Item, int, bool>(this.CanAcceptVendorItem));
		if (this.customerInventory == null)
		{
			this.customerInventory = new ItemContainer();
			this.customerInventory.allowedContents = ((this.allowedContents == (ItemContainer.ContentsType)0) ? ItemContainer.ContentsType.Generic : this.allowedContents);
			this.customerInventory.SetOnlyAllowedItem(this.allowedItem);
			this.customerInventory.entityOwner = this;
			this.customerInventory.maxStackSize = this.maxStackSize;
			this.customerInventory.ServerInitialize(null, this.inventorySlots);
			this.customerInventory.GiveUID();
			this.customerInventory.onDirty += this.OnInventoryDirty;
			this.customerInventory.onItemAddedRemoved = new Action<Item, bool>(this.OnItemAddedOrRemoved);
			ItemContainer itemContainer = this.customerInventory;
			itemContainer.canAcceptItem = (Func<Item, int, bool>)Delegate.Combine(itemContainer.canAcceptItem, new Func<Item, int, bool>(this.CanAcceptCustomerItem));
			this.OnInventoryFirstCreated(this.customerInventory);
		}
	}

	// Token: 0x0600113F RID: 4415 RVA: 0x0008BCBE File Offset: 0x00089EBE
	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		this.ResetTrade();
	}

	// Token: 0x06001140 RID: 4416 RVA: 0x0008BCD0 File Offset: 0x00089ED0
	private bool CanAcceptVendorItem(Item item, int targetSlot)
	{
		return this.swappingItems || (this.vendorPlayer != null && item.GetOwnerPlayer() == this.vendorPlayer) || this.vendorInventory.itemList.Contains(item);
	}

	// Token: 0x06001141 RID: 4417 RVA: 0x0008BD1C File Offset: 0x00089F1C
	private bool CanAcceptCustomerItem(Item item, int targetSlot)
	{
		return this.swappingItems || (this.customerPlayer != null && item.GetOwnerPlayer() == this.customerPlayer) || this.customerInventory.itemList.Contains(item);
	}

	// Token: 0x06001142 RID: 4418 RVA: 0x0008BD68 File Offset: 0x00089F68
	public override bool CanMoveFrom(BasePlayer player, Item item)
	{
		if (this.TradeLocked())
		{
			return false;
		}
		if (this.IsTradingPlayer(player))
		{
			if (this.IsPlayerCustomer(player) && this.customerInventory.itemList.Contains(item) && !this.customerInventory.IsLocked())
			{
				return true;
			}
			if (this.IsPlayerVendor(player) && this.vendorInventory.itemList.Contains(item) && !this.vendorInventory.IsLocked())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001143 RID: 4419 RVA: 0x0008BDDF File Offset: 0x00089FDF
	public override bool CanOpenLootPanel(BasePlayer player, string panelName)
	{
		return base.CanOpenLootPanel(player, panelName) && this.LootEligable(player);
	}

	// Token: 0x06001144 RID: 4420 RVA: 0x0008BDF4 File Offset: 0x00089FF4
	public void ReturnPlayerItems(BasePlayer player)
	{
		if (this.IsTradingPlayer(player))
		{
			ItemContainer itemContainer = null;
			if (this.IsPlayerVendor(player))
			{
				itemContainer = this.vendorInventory;
			}
			else if (this.IsPlayerCustomer(player))
			{
				itemContainer = this.customerInventory;
			}
			if (itemContainer != null)
			{
				for (int i = itemContainer.itemList.Count - 1; i >= 0; i--)
				{
					Item item = itemContainer.itemList[i];
					player.GiveItem(item, BaseEntity.GiveItemReason.Generic);
				}
			}
		}
	}

	// Token: 0x06001145 RID: 4421 RVA: 0x0008BE60 File Offset: 0x0008A060
	public override void PlayerStoppedLooting(BasePlayer player)
	{
		if (!this.IsTradingPlayer(player))
		{
			return;
		}
		this.ReturnPlayerItems(player);
		if (player == this.vendorPlayer)
		{
			this.vendorPlayer = null;
		}
		if (player == this.customerPlayer)
		{
			this.customerPlayer = null;
		}
		this.UpdatePlayers();
		this.ResetTrade();
		base.PlayerStoppedLooting(player);
	}

	// Token: 0x06001146 RID: 4422 RVA: 0x0008BEBC File Offset: 0x0008A0BC
	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		bool flag = base.PlayerOpenLoot(player, panelToOpen, true);
		if (flag)
		{
			player.inventory.loot.AddContainer(this.customerInventory);
			player.inventory.loot.SendImmediate();
		}
		if (this.PlayerInVendorPos(player) && this.vendorPlayer == null)
		{
			this.vendorPlayer = player;
		}
		else
		{
			if (!this.PlayerInCustomerPos(player) || !(this.customerPlayer == null))
			{
				return false;
			}
			this.customerPlayer = player;
		}
		this.ResetTrade();
		this.UpdatePlayers();
		return flag;
	}

	// Token: 0x06001147 RID: 4423 RVA: 0x0008BF4C File Offset: 0x0008A14C
	public void UpdatePlayers()
	{
		base.ClientRPC<uint, uint>(null, "CLIENT_ReceivePlayers", (this.vendorPlayer == null) ? 0U : this.vendorPlayer.net.ID, (this.customerPlayer == null) ? 0U : this.customerPlayer.net.ID);
	}

	// Token: 0x04000ACC RID: 2764
	public float maxUseAngle = 27f;

	// Token: 0x04000ACD RID: 2765
	public BasePlayer vendorPlayer;

	// Token: 0x04000ACE RID: 2766
	public BasePlayer customerPlayer;

	// Token: 0x04000ACF RID: 2767
	public GameObjectRef transactionCompleteEffect;

	// Token: 0x04000AD0 RID: 2768
	public ItemContainer customerInventory;

	// Token: 0x04000AD1 RID: 2769
	private bool swappingItems;

	// Token: 0x02000BB0 RID: 2992
	public static class ShopFrontFlags
	{
		// Token: 0x04003F27 RID: 16167
		public const BaseEntity.Flags VendorAccepted = BaseEntity.Flags.Reserved1;

		// Token: 0x04003F28 RID: 16168
		public const BaseEntity.Flags CustomerAccepted = BaseEntity.Flags.Reserved2;

		// Token: 0x04003F29 RID: 16169
		public const BaseEntity.Flags Exchanging = BaseEntity.Flags.Reserved3;
	}
}
