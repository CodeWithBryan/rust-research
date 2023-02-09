using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using Network;
using ProtoBuf;
using Rust;
using Rust.UI;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000094 RID: 148
public class MarketTerminal : StorageContainer
{
	// Token: 0x06000D76 RID: 3446 RVA: 0x00070ED0 File Offset: 0x0006F0D0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MarketTerminal.OnRpcMessage", 0))
		{
			if (rpc == 3793918752U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_Purchase ");
				}
				using (TimeWarning.New("Server_Purchase", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3793918752U, "Server_Purchase", this, player, 10UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3793918752U, "Server_Purchase", this, player, 3f))
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
							this.Server_Purchase(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_Purchase");
					}
				}
				return true;
			}
			if (rpc == 1382511247U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_TryOpenMarket ");
				}
				using (TimeWarning.New("Server_TryOpenMarket", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1382511247U, "Server_TryOpenMarket", this, player, 3UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1382511247U, "Server_TryOpenMarket", this, player, 3f))
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
							this.Server_TryOpenMarket(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in Server_TryOpenMarket");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000D77 RID: 3447 RVA: 0x00071208 File Offset: 0x0006F408
	public void Setup(Marketplace marketplace)
	{
		this._marketplace = new EntityRef<Marketplace>(marketplace.net.ID);
	}

	// Token: 0x06000D78 RID: 3448 RVA: 0x00071220 File Offset: 0x0006F420
	public override void ServerInit()
	{
		base.ServerInit();
		this._onCurrencyRemovedCached = new Action<global::BasePlayer, global::Item>(this.OnCurrencyRemoved);
		this._onItemPurchasedCached = new Action<global::BasePlayer, global::Item>(this.OnItemPurchased);
		this._checkForExpiredOrdersCached = new Action(this.CheckForExpiredOrders);
	}

	// Token: 0x06000D79 RID: 3449 RVA: 0x00071260 File Offset: 0x0006F460
	private void RegisterOrder(global::BasePlayer player, global::VendingMachine vendingMachine)
	{
		if (this.pendingOrders == null)
		{
			this.pendingOrders = Facepunch.Pool.GetList<ProtoBuf.MarketTerminal.PendingOrder>();
		}
		if (this.HasPendingOrderFor(vendingMachine.net.ID))
		{
			return;
		}
		Marketplace marketplace;
		if (!this._marketplace.TryGet(true, out marketplace))
		{
			Debug.LogError("Marketplace is not set", this);
			return;
		}
		uint num = marketplace.SendDrone(player, this, vendingMachine);
		if (num == 0U)
		{
			Debug.LogError("Failed to spawn delivery drone");
			return;
		}
		ProtoBuf.MarketTerminal.PendingOrder pendingOrder = Facepunch.Pool.Get<ProtoBuf.MarketTerminal.PendingOrder>();
		pendingOrder.vendingMachineId = vendingMachine.net.ID;
		pendingOrder.timeUntilExpiry = this.orderTimeout;
		pendingOrder.droneId = num;
		this.pendingOrders.Add(pendingOrder);
		this.CheckForExpiredOrders();
		this.UpdateHasItems(false);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000D7A RID: 3450 RVA: 0x0007131C File Offset: 0x0006F51C
	public void CompleteOrder(uint vendingMachineId)
	{
		if (this.pendingOrders == null)
		{
			return;
		}
		int num = this.pendingOrders.FindIndexWith((ProtoBuf.MarketTerminal.PendingOrder o) => o.vendingMachineId, vendingMachineId);
		if (num < 0)
		{
			Debug.LogError("Completed market order that doesn't exist?");
			return;
		}
		this.pendingOrders[num].Dispose();
		this.pendingOrders.RemoveAt(num);
		this.CheckForExpiredOrders();
		this.UpdateHasItems(false);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000D7B RID: 3451 RVA: 0x000713A0 File Offset: 0x0006F5A0
	private void CheckForExpiredOrders()
	{
		if (this.pendingOrders != null && this.pendingOrders.Count > 0)
		{
			bool flag = false;
			float? num = null;
			for (int i = 0; i < this.pendingOrders.Count; i++)
			{
				ProtoBuf.MarketTerminal.PendingOrder pendingOrder = this.pendingOrders[i];
				if (pendingOrder.timeUntilExpiry <= 0f)
				{
					EntityRef<global::DeliveryDrone> entityRef = new EntityRef<global::DeliveryDrone>(pendingOrder.droneId);
					global::DeliveryDrone deliveryDrone;
					if (entityRef.TryGet(true, out deliveryDrone))
					{
						Debug.LogError("Delivery timed out waiting for drone (too slow speed?)", this);
						deliveryDrone.Kill(global::BaseNetworkable.DestroyMode.None);
					}
					else
					{
						Debug.LogError("Delivery timed out waiting for drone, and the drone seems to have been destroyed?", this);
					}
					this.pendingOrders.RemoveAt(i);
					i--;
					flag = true;
				}
				else if (num == null || pendingOrder.timeUntilExpiry < num.Value)
				{
					num = new float?(pendingOrder.timeUntilExpiry);
				}
			}
			if (flag)
			{
				this.UpdateHasItems(false);
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
			if (num != null)
			{
				base.Invoke(this._checkForExpiredOrdersCached, num.Value);
				return;
			}
		}
		else
		{
			base.CancelInvoke(this._checkForExpiredOrdersCached);
		}
	}

	// Token: 0x06000D7C RID: 3452 RVA: 0x000714C4 File Offset: 0x0006F6C4
	private void RestrictToPlayer(global::BasePlayer player)
	{
		if (this._customerSteamId == player.userID)
		{
			this._timeUntilCustomerExpiry = this.lockToCustomerDuration;
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			return;
		}
		if (this._customerSteamId != 0UL)
		{
			Debug.LogError("Overwriting player restriction! It should be cleared first.", this);
		}
		this._customerSteamId = player.userID;
		this._customerName = player.displayName;
		this._timeUntilCustomerExpiry = this.lockToCustomerDuration;
		base.SendNetworkUpdateImmediate(false);
		base.ClientRPC<ulong>(null, "Client_CloseMarketUI", this._customerSteamId);
		this.RemoveAnyLooters();
		if (base.IsOpen())
		{
			Debug.LogError("Market terminal's inventory is still open after removing looters!", this);
		}
	}

	// Token: 0x06000D7D RID: 3453 RVA: 0x0007156A File Offset: 0x0006F76A
	private void ClearRestriction()
	{
		if (this._customerSteamId == 0UL)
		{
			return;
		}
		this._customerSteamId = 0UL;
		this._customerName = null;
		this._timeUntilCustomerExpiry = 0f;
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000D7E RID: 3454 RVA: 0x0007159C File Offset: 0x0006F79C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(3UL)]
	public void Server_TryOpenMarket(global::BaseEntity.RPCMessage msg)
	{
		if (!this.CanPlayerInteract(msg.player))
		{
			return;
		}
		if (!this._marketplace.IsValid(true))
		{
			Debug.LogError("Marketplace is not set", this);
			return;
		}
		using (EntityIdList entityIdList = Facepunch.Pool.Get<EntityIdList>())
		{
			entityIdList.entityIds = Facepunch.Pool.GetList<uint>();
			this.GetDeliveryEligibleVendingMachines(entityIdList.entityIds);
			base.ClientRPCPlayer<EntityIdList>(null, msg.player, "Client_OpenMarket", entityIdList);
		}
	}

	// Token: 0x06000D7F RID: 3455 RVA: 0x00071620 File Offset: 0x0006F820
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(10UL)]
	public void Server_Purchase(global::BaseEntity.RPCMessage msg)
	{
		if (!this.CanPlayerInteract(msg.player))
		{
			return;
		}
		if (!this._marketplace.IsValid(true))
		{
			Debug.LogError("Marketplace is not set", this);
			return;
		}
		uint num = msg.read.UInt32();
		int num2 = msg.read.Int32();
		int num3 = msg.read.Int32();
		global::VendingMachine vendingMachine = global::BaseNetworkable.serverEntities.Find(num) as global::VendingMachine;
		if (vendingMachine == null || !vendingMachine.IsValid() || num2 < 0 || num2 >= vendingMachine.sellOrders.sellOrders.Count || num3 <= 0 || base.inventory.IsFull())
		{
			return;
		}
		this.GetDeliveryEligibleVendingMachines(null);
		if (global::MarketTerminal._deliveryEligible == null || !global::MarketTerminal._deliveryEligible.Contains(num))
		{
			return;
		}
		try
		{
			this._transactionActive = true;
			int num4 = this.deliveryFeeAmount;
			ProtoBuf.VendingMachine.SellOrder sellOrder = vendingMachine.sellOrders.sellOrders[num2];
			if (this.CanPlayerAffordOrderAndDeliveryFee(msg.player, sellOrder, num3))
			{
				int num5 = msg.player.inventory.Take(null, this.deliveryFeeCurrency.itemid, num4);
				if (num5 != num4)
				{
					Debug.LogError(string.Format("Took an incorrect number of items for the delivery fee (took {0}, should have taken {1})", num5, num4));
				}
				base.ClientRPCPlayer<int, int, bool>(null, msg.player, "Client_ShowItemNotice", this.deliveryFeeCurrency.itemid, -num4, false);
				if (!vendingMachine.DoTransaction(msg.player, num2, num3, base.inventory, this._onCurrencyRemovedCached, this._onItemPurchasedCached))
				{
					global::Item item = ItemManager.CreateByItemID(this.deliveryFeeCurrency.itemid, num4, 0UL);
					if (!msg.player.inventory.GiveItem(item, null))
					{
						item.Drop(msg.player.inventory.containerMain.dropPosition, msg.player.inventory.containerMain.dropVelocity, default(Quaternion));
					}
				}
				else
				{
					this.RestrictToPlayer(msg.player);
					this.RegisterOrder(msg.player, vendingMachine);
				}
			}
		}
		finally
		{
			this._transactionActive = false;
		}
	}

	// Token: 0x06000D80 RID: 3456 RVA: 0x00071848 File Offset: 0x0006FA48
	private void UpdateHasItems(bool sendNetworkUpdate = true)
	{
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		bool flag = base.inventory.itemList.Count > 0;
		bool flag2 = this.pendingOrders != null && this.pendingOrders.Count != 0;
		base.SetFlag(global::BaseEntity.Flags.Reserved1, flag && !flag2, false, sendNetworkUpdate);
		base.SetFlag(global::BaseEntity.Flags.Reserved2, base.inventory.IsFull(), false, sendNetworkUpdate);
		if (!flag && !flag2)
		{
			this.ClearRestriction();
		}
	}

	// Token: 0x06000D81 RID: 3457 RVA: 0x000718C6 File Offset: 0x0006FAC6
	private void OnCurrencyRemoved(global::BasePlayer player, global::Item currencyItem)
	{
		if (player != null && currencyItem != null)
		{
			base.ClientRPCPlayer<int, int, bool>(null, player, "Client_ShowItemNotice", currencyItem.info.itemid, -currencyItem.amount, false);
		}
	}

	// Token: 0x06000D82 RID: 3458 RVA: 0x000718F4 File Offset: 0x0006FAF4
	private void OnItemPurchased(global::BasePlayer player, global::Item purchasedItem)
	{
		if (player != null && purchasedItem != null)
		{
			base.ClientRPCPlayer<int, int, bool>(null, player, "Client_ShowItemNotice", purchasedItem.info.itemid, purchasedItem.amount, true);
		}
	}

	// Token: 0x06000D83 RID: 3459 RVA: 0x00071924 File Offset: 0x0006FB24
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.marketTerminal = Facepunch.Pool.Get<ProtoBuf.MarketTerminal>();
		info.msg.marketTerminal.customerSteamId = this._customerSteamId;
		info.msg.marketTerminal.customerName = this._customerName;
		info.msg.marketTerminal.timeUntilExpiry = this._timeUntilCustomerExpiry;
		info.msg.marketTerminal.marketplaceId = this._marketplace.uid;
		info.msg.marketTerminal.orders = Facepunch.Pool.GetList<ProtoBuf.MarketTerminal.PendingOrder>();
		if (this.pendingOrders != null)
		{
			foreach (ProtoBuf.MarketTerminal.PendingOrder pendingOrder in this.pendingOrders)
			{
				ProtoBuf.MarketTerminal.PendingOrder item = pendingOrder.Copy();
				info.msg.marketTerminal.orders.Add(item);
			}
		}
	}

	// Token: 0x06000D84 RID: 3460 RVA: 0x00071A1C File Offset: 0x0006FC1C
	public override bool ItemFilter(global::Item item, int targetSlot)
	{
		return this._transactionActive || item.parent == null || item.parent == base.inventory;
	}

	// Token: 0x06000D85 RID: 3461 RVA: 0x00071A43 File Offset: 0x0006FC43
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		this.UpdateHasItems(true);
	}

	// Token: 0x06000D86 RID: 3462 RVA: 0x00071A4C File Offset: 0x0006FC4C
	public override bool CanOpenLootPanel(global::BasePlayer player, string panelName)
	{
		return this.CanPlayerInteract(player) && base.HasFlag(global::BaseEntity.Flags.Reserved1) && base.CanOpenLootPanel(player, panelName);
	}

	// Token: 0x06000D87 RID: 3463 RVA: 0x00071A70 File Offset: 0x0006FC70
	private void RemoveAnyLooters()
	{
		global::ItemContainer inventory = base.inventory;
		foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
		{
			if (!(basePlayer == null) && !(basePlayer.inventory == null) && !(basePlayer.inventory.loot == null) && basePlayer.inventory.loot.containers.Contains(inventory))
			{
				basePlayer.inventory.loot.Clear();
			}
		}
	}

	// Token: 0x06000D88 RID: 3464 RVA: 0x00071B14 File Offset: 0x0006FD14
	public void GetDeliveryEligibleVendingMachines(List<uint> vendingMachineIds)
	{
		if (global::MarketTerminal._deliveryEligibleLastCalculated < 5f)
		{
			if (vendingMachineIds != null)
			{
				foreach (uint item in global::MarketTerminal._deliveryEligible)
				{
					vendingMachineIds.Add(item);
				}
			}
			return;
		}
		global::MarketTerminal._deliveryEligibleLastCalculated = 0f;
		global::MarketTerminal._deliveryEligible.Clear();
		using (List<MapMarker>.Enumerator enumerator2 = MapMarker.serverMapMarkers.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				VendingMachineMapMarker vendingMachineMapMarker;
				if ((vendingMachineMapMarker = (enumerator2.Current as VendingMachineMapMarker)) != null && !(vendingMachineMapMarker.server_vendingMachine == null))
				{
					global::VendingMachine server_vendingMachine = vendingMachineMapMarker.server_vendingMachine;
					if (!(server_vendingMachine == null) && (this.<GetDeliveryEligibleVendingMachines>g__IsEligible|24_0(server_vendingMachine, this.config.vendingMachineOffset, 1) || this.<GetDeliveryEligibleVendingMachines>g__IsEligible|24_0(server_vendingMachine, this.config.vendingMachineOffset + Vector3.forward * this.config.maxDistanceFromVendingMachine, 2)))
					{
						global::MarketTerminal._deliveryEligible.Add(server_vendingMachine.net.ID);
					}
				}
			}
		}
		if (vendingMachineIds != null)
		{
			foreach (uint item2 in global::MarketTerminal._deliveryEligible)
			{
				vendingMachineIds.Add(item2);
			}
		}
	}

	// Token: 0x06000D89 RID: 3465 RVA: 0x00071CA0 File Offset: 0x0006FEA0
	public bool CanPlayerAffordOrderAndDeliveryFee(global::BasePlayer player, ProtoBuf.VendingMachine.SellOrder sellOrder, int numberOfTransactions)
	{
		int num = player.inventory.FindItemIDs(this.deliveryFeeCurrency.itemid).Sum((global::Item i) => i.amount);
		int num2 = this.deliveryFeeAmount;
		if (num < num2)
		{
			return false;
		}
		if (sellOrder != null)
		{
			int num3 = sellOrder.currencyAmountPerItem * numberOfTransactions;
			if (sellOrder.currencyID == this.deliveryFeeCurrency.itemid && !sellOrder.currencyIsBP && num < num2 + num3)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000D8A RID: 3466 RVA: 0x00071D25 File Offset: 0x0006FF25
	public bool HasPendingOrderFor(uint vendingMachineId)
	{
		List<ProtoBuf.MarketTerminal.PendingOrder> list = this.pendingOrders;
		object obj;
		if (list == null)
		{
			obj = null;
		}
		else
		{
			obj = list.FindWith((ProtoBuf.MarketTerminal.PendingOrder o) => o.vendingMachineId, vendingMachineId, null);
		}
		return obj != null;
	}

	// Token: 0x06000D8B RID: 3467 RVA: 0x00071D5D File Offset: 0x0006FF5D
	public bool CanPlayerInteract(global::BasePlayer player)
	{
		return !(player == null) && (this._customerSteamId == 0UL || this._timeUntilCustomerExpiry <= 0f || player.userID == this._customerSteamId);
	}

	// Token: 0x06000D8C RID: 3468 RVA: 0x00071D94 File Offset: 0x0006FF94
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.marketTerminal != null)
		{
			this._customerSteamId = info.msg.marketTerminal.customerSteamId;
			this._customerName = info.msg.marketTerminal.customerName;
			this._timeUntilCustomerExpiry = info.msg.marketTerminal.timeUntilExpiry;
			this._marketplace = new EntityRef<Marketplace>(info.msg.marketTerminal.marketplaceId);
			if (this.pendingOrders == null)
			{
				this.pendingOrders = Facepunch.Pool.GetList<ProtoBuf.MarketTerminal.PendingOrder>();
			}
			if (this.pendingOrders.Count > 0)
			{
				foreach (ProtoBuf.MarketTerminal.PendingOrder pendingOrder in this.pendingOrders)
				{
					Facepunch.Pool.Free<ProtoBuf.MarketTerminal.PendingOrder>(ref pendingOrder);
				}
				this.pendingOrders.Clear();
			}
			foreach (ProtoBuf.MarketTerminal.PendingOrder pendingOrder2 in info.msg.marketTerminal.orders)
			{
				ProtoBuf.MarketTerminal.PendingOrder item = pendingOrder2.Copy();
				this.pendingOrders.Add(item);
			}
		}
	}

	// Token: 0x06000D8F RID: 3471 RVA: 0x00071F14 File Offset: 0x00070114
	[CompilerGenerated]
	private bool <GetDeliveryEligibleVendingMachines>g__IsEligible|24_0(global::VendingMachine vendingMachine, Vector3 offset, int n)
	{
		RaycastHit raycastHit;
		return vendingMachine is NPCVendingMachine || (vendingMachine.IsBroadcasting() && this.config.IsVendingMachineAccessible(vendingMachine, offset, out raycastHit));
	}

	// Token: 0x040008AB RID: 2219
	private Action<global::BasePlayer, global::Item> _onCurrencyRemovedCached;

	// Token: 0x040008AC RID: 2220
	private Action<global::BasePlayer, global::Item> _onItemPurchasedCached;

	// Token: 0x040008AD RID: 2221
	private Action _checkForExpiredOrdersCached;

	// Token: 0x040008AE RID: 2222
	private bool _transactionActive;

	// Token: 0x040008AF RID: 2223
	private static readonly List<uint> _deliveryEligible = new List<uint>(128);

	// Token: 0x040008B0 RID: 2224
	private static RealTimeSince _deliveryEligibleLastCalculated;

	// Token: 0x040008B1 RID: 2225
	public const global::BaseEntity.Flags Flag_HasItems = global::BaseEntity.Flags.Reserved1;

	// Token: 0x040008B2 RID: 2226
	public const global::BaseEntity.Flags Flag_InventoryFull = global::BaseEntity.Flags.Reserved2;

	// Token: 0x040008B3 RID: 2227
	[Header("Market Terminal")]
	public GameObjectRef menuPrefab;

	// Token: 0x040008B4 RID: 2228
	public ulong lockToCustomerDuration = 300UL;

	// Token: 0x040008B5 RID: 2229
	public ulong orderTimeout = 180UL;

	// Token: 0x040008B6 RID: 2230
	public ItemDefinition deliveryFeeCurrency;

	// Token: 0x040008B7 RID: 2231
	public int deliveryFeeAmount;

	// Token: 0x040008B8 RID: 2232
	public DeliveryDroneConfig config;

	// Token: 0x040008B9 RID: 2233
	public RustText userLabel;

	// Token: 0x040008BA RID: 2234
	private ulong _customerSteamId;

	// Token: 0x040008BB RID: 2235
	private string _customerName;

	// Token: 0x040008BC RID: 2236
	private TimeUntil _timeUntilCustomerExpiry;

	// Token: 0x040008BD RID: 2237
	private EntityRef<Marketplace> _marketplace;

	// Token: 0x040008BE RID: 2238
	public List<ProtoBuf.MarketTerminal.PendingOrder> pendingOrders;
}
