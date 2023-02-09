using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000E5 RID: 229
public class VendingMachine : StorageContainer
{
	// Token: 0x060013D7 RID: 5079 RVA: 0x0009C2C0 File Offset: 0x0009A4C0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("VendingMachine.OnRpcMessage", 0))
		{
			if (rpc == 3011053703U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - BuyItem ");
				}
				using (TimeWarning.New("BuyItem", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3011053703U, "BuyItem", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3011053703U, "BuyItem", this, player, 3f))
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
							this.BuyItem(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in BuyItem");
					}
				}
				return true;
			}
			if (rpc == 1626480840U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_AddSellOrder ");
				}
				using (TimeWarning.New("RPC_AddSellOrder", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1626480840U, "RPC_AddSellOrder", this, player, 3f))
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
							this.RPC_AddSellOrder(msg2);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_AddSellOrder");
					}
				}
				return true;
			}
			if (rpc == 169239598U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Broadcast ");
				}
				using (TimeWarning.New("RPC_Broadcast", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(169239598U, "RPC_Broadcast", this, player, 3f))
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
							this.RPC_Broadcast(msg3);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in RPC_Broadcast");
					}
				}
				return true;
			}
			if (rpc == 3680901137U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_DeleteSellOrder ");
				}
				using (TimeWarning.New("RPC_DeleteSellOrder", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3680901137U, "RPC_DeleteSellOrder", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_DeleteSellOrder(msg4);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in RPC_DeleteSellOrder");
					}
				}
				return true;
			}
			if (rpc == 2555993359U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenAdmin ");
				}
				using (TimeWarning.New("RPC_OpenAdmin", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2555993359U, "RPC_OpenAdmin", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_OpenAdmin(msg5);
						}
					}
					catch (Exception exception5)
					{
						Debug.LogException(exception5);
						player.Kick("RPC Error in RPC_OpenAdmin");
					}
				}
				return true;
			}
			if (rpc == 36164441U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenShop ");
				}
				using (TimeWarning.New("RPC_OpenShop", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(36164441U, "RPC_OpenShop", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg6 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_OpenShop(msg6);
						}
					}
					catch (Exception exception6)
					{
						Debug.LogException(exception6);
						player.Kick("RPC Error in RPC_OpenShop");
					}
				}
				return true;
			}
			if (rpc == 3346513099U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_RotateVM ");
				}
				using (TimeWarning.New("RPC_RotateVM", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3346513099U, "RPC_RotateVM", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg7 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_RotateVM(msg7);
						}
					}
					catch (Exception exception7)
					{
						Debug.LogException(exception7);
						player.Kick("RPC Error in RPC_RotateVM");
					}
				}
				return true;
			}
			if (rpc == 1012779214U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_UpdateShopName ");
				}
				using (TimeWarning.New("RPC_UpdateShopName", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1012779214U, "RPC_UpdateShopName", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg8 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_UpdateShopName(msg8);
						}
					}
					catch (Exception exception8)
					{
						Debug.LogException(exception8);
						player.Kick("RPC Error in RPC_UpdateShopName");
					}
				}
				return true;
			}
			if (rpc == 3559014831U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - TransactionStart ");
				}
				using (TimeWarning.New("TransactionStart", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3559014831U, "TransactionStart", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.TransactionStart(rpc3);
						}
					}
					catch (Exception exception9)
					{
						Debug.LogException(exception9);
						player.Kick("RPC Error in TransactionStart");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060013D8 RID: 5080 RVA: 0x0009CF58 File Offset: 0x0009B158
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.vendingMachine != null)
		{
			this.shopName = info.msg.vendingMachine.shopName;
			if (info.msg.vendingMachine.sellOrderContainer != null)
			{
				this.sellOrders = info.msg.vendingMachine.sellOrderContainer;
				this.sellOrders.ShouldPool = false;
			}
			if (info.fromDisk && base.isServer)
			{
				this.RefreshSellOrderStockLevel(null);
			}
		}
	}

	// Token: 0x060013D9 RID: 5081 RVA: 0x0009CFDC File Offset: 0x0009B1DC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.vendingMachine = new ProtoBuf.VendingMachine();
		info.msg.vendingMachine.ShouldPool = false;
		info.msg.vendingMachine.shopName = this.shopName;
		if (this.sellOrders != null)
		{
			info.msg.vendingMachine.sellOrderContainer = new ProtoBuf.VendingMachine.SellOrderContainer();
			info.msg.vendingMachine.sellOrderContainer.ShouldPool = false;
			info.msg.vendingMachine.sellOrderContainer.sellOrders = new List<ProtoBuf.VendingMachine.SellOrder>();
			foreach (ProtoBuf.VendingMachine.SellOrder sellOrder in this.sellOrders.sellOrders)
			{
				ProtoBuf.VendingMachine.SellOrder sellOrder2 = new ProtoBuf.VendingMachine.SellOrder();
				sellOrder2.ShouldPool = false;
				sellOrder.CopyTo(sellOrder2);
				info.msg.vendingMachine.sellOrderContainer.sellOrders.Add(sellOrder2);
			}
		}
	}

	// Token: 0x060013DA RID: 5082 RVA: 0x0009D0E8 File Offset: 0x0009B2E8
	public override void ServerInit()
	{
		base.ServerInit();
		if (base.isServer)
		{
			this.InstallDefaultSellOrders();
			base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
			base.inventory.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedOrRemoved);
			this.RefreshSellOrderStockLevel(null);
			global::ItemContainer inventory = base.inventory;
			inventory.canAcceptItem = (Func<global::Item, int, bool>)Delegate.Combine(inventory.canAcceptItem, new Func<global::Item, int, bool>(this.CanAcceptItem));
			this.UpdateMapMarker();
			this.fullUpdateCached = new Action(this.FullUpdate);
		}
	}

	// Token: 0x060013DB RID: 5083 RVA: 0x0009D175 File Offset: 0x0009B375
	public override void DestroyShared()
	{
		if (this.myMarker)
		{
			this.myMarker.Kill(global::BaseNetworkable.DestroyMode.None);
			this.myMarker = null;
		}
		base.DestroyShared();
	}

	// Token: 0x060013DC RID: 5084 RVA: 0x0009D19D File Offset: 0x0009B39D
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
	}

	// Token: 0x060013DD RID: 5085 RVA: 0x0009D1A7 File Offset: 0x0009B3A7
	public void FullUpdate()
	{
		this.RefreshSellOrderStockLevel(null);
		this.UpdateMapMarker();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060013DE RID: 5086 RVA: 0x0009D1BD File Offset: 0x0009B3BD
	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		base.CancelInvoke(this.fullUpdateCached);
		base.Invoke(this.fullUpdateCached, 0.2f);
	}

	// Token: 0x060013DF RID: 5087 RVA: 0x0009D1E4 File Offset: 0x0009B3E4
	public void RefreshSellOrderStockLevel(ItemDefinition itemDef = null)
	{
		foreach (ProtoBuf.VendingMachine.SellOrder sellOrder in this.sellOrders.sellOrders)
		{
			if (itemDef == null || itemDef.itemid == sellOrder.itemToSellID)
			{
				List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
				this.GetItemsToSell(sellOrder, list);
				ProtoBuf.VendingMachine.SellOrder sellOrder2 = sellOrder;
				int inStock;
				if (list.Count < 0)
				{
					inStock = 0;
				}
				else
				{
					inStock = list.Sum((global::Item x) => x.amount) / sellOrder.itemToSellAmount;
				}
				sellOrder2.inStock = inStock;
				float itemCondition = 0f;
				float itemConditionMax = 0f;
				int instanceData = 0;
				if (list.Count > 0)
				{
					if (list[0].hasCondition)
					{
						itemCondition = list[0].condition;
						itemConditionMax = list[0].maxCondition;
					}
					if (list[0].info != null && list[0].info.amountType == ItemDefinition.AmountType.Genetics && list[0].instanceData != null)
					{
						instanceData = list[0].instanceData.dataInt;
						sellOrder.inStock = list[0].amount;
					}
				}
				sellOrder.itemCondition = itemCondition;
				sellOrder.itemConditionMax = itemConditionMax;
				sellOrder.instanceData = instanceData;
				Facepunch.Pool.FreeList<global::Item>(ref list);
			}
		}
	}

	// Token: 0x060013E0 RID: 5088 RVA: 0x0009D36C File Offset: 0x0009B56C
	public bool OutOfStock()
	{
		using (List<ProtoBuf.VendingMachine.SellOrder>.Enumerator enumerator = this.sellOrders.sellOrders.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.inStock > 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060013E1 RID: 5089 RVA: 0x0009D3CC File Offset: 0x0009B5CC
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		this.RefreshSellOrderStockLevel(null);
		this.UpdateMapMarker();
	}

	// Token: 0x060013E2 RID: 5090 RVA: 0x0009D3EF File Offset: 0x0009B5EF
	public void UpdateEmptyFlag()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved1, base.inventory.itemList.Count == 0, false, true);
	}

	// Token: 0x060013E3 RID: 5091 RVA: 0x0009D411 File Offset: 0x0009B611
	public override void PlayerStoppedLooting(global::BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		this.UpdateEmptyFlag();
		if (this.vend_Player != null && this.vend_Player == player)
		{
			this.ClearPendingOrder();
		}
	}

	// Token: 0x060013E4 RID: 5092 RVA: 0x0009D442 File Offset: 0x0009B642
	public virtual void InstallDefaultSellOrders()
	{
		this.sellOrders = new ProtoBuf.VendingMachine.SellOrderContainer();
		this.sellOrders.ShouldPool = false;
		this.sellOrders.sellOrders = new List<ProtoBuf.VendingMachine.SellOrder>();
	}

	// Token: 0x060013E5 RID: 5093 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool HasVendingSounds()
	{
		return true;
	}

	// Token: 0x060013E6 RID: 5094 RVA: 0x0009D46B File Offset: 0x0009B66B
	public virtual float GetBuyDuration()
	{
		return 2.5f;
	}

	// Token: 0x060013E7 RID: 5095 RVA: 0x0009D472 File Offset: 0x0009B672
	public void SetPendingOrder(global::BasePlayer buyer, int sellOrderId, int numberOfTransactions)
	{
		this.ClearPendingOrder();
		this.vend_Player = buyer;
		this.vend_sellOrderID = sellOrderId;
		this.vend_numberOfTransactions = numberOfTransactions;
		base.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
		if (this.HasVendingSounds())
		{
			base.ClientRPC<int>(null, "CLIENT_StartVendingSounds", sellOrderId);
		}
	}

	// Token: 0x060013E8 RID: 5096 RVA: 0x0009D4B4 File Offset: 0x0009B6B4
	public void ClearPendingOrder()
	{
		base.CancelInvoke(new Action(this.CompletePendingOrder));
		this.vend_Player = null;
		this.vend_sellOrderID = -1;
		this.vend_numberOfTransactions = -1;
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		base.ClientRPC(null, "CLIENT_CancelVendingSounds");
	}

	// Token: 0x060013E9 RID: 5097 RVA: 0x0009D504 File Offset: 0x0009B704
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void BuyItem(global::BaseEntity.RPCMessage rpc)
	{
		if (!base.OccupiedCheck(rpc.player))
		{
			return;
		}
		int sellOrderId = rpc.read.Int32();
		int numberOfTransactions = rpc.read.Int32();
		if (this.IsVending())
		{
			rpc.player.ShowToast(GameTip.Styles.Red_Normal, global::VendingMachine.WaitForVendingMessage, Array.Empty<string>());
			return;
		}
		this.SetPendingOrder(rpc.player, sellOrderId, numberOfTransactions);
		base.Invoke(new Action(this.CompletePendingOrder), this.GetBuyDuration());
	}

	// Token: 0x060013EA RID: 5098 RVA: 0x0009D57E File Offset: 0x0009B77E
	public virtual void CompletePendingOrder()
	{
		this.DoTransaction(this.vend_Player, this.vend_sellOrderID, this.vend_numberOfTransactions, null, null, null);
		this.ClearPendingOrder();
		global::Decay.RadialDecayTouch(base.transform.position, 40f, 2097408);
	}

	// Token: 0x060013EB RID: 5099 RVA: 0x000059DD File Offset: 0x00003BDD
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void TransactionStart(global::BaseEntity.RPCMessage rpc)
	{
	}

	// Token: 0x060013EC RID: 5100 RVA: 0x0009D5BC File Offset: 0x0009B7BC
	private void GetItemsToSell(ProtoBuf.VendingMachine.SellOrder sellOrder, List<global::Item> items)
	{
		if (sellOrder.itemToSellIsBP)
		{
			using (List<global::Item>.Enumerator enumerator = base.inventory.itemList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::Item item = enumerator.Current;
					if (item.info.itemid == this.blueprintBaseDef.itemid && item.blueprintTarget == sellOrder.itemToSellID)
					{
						items.Add(item);
					}
				}
				return;
			}
		}
		foreach (global::Item item2 in base.inventory.itemList)
		{
			if (item2.info.itemid == sellOrder.itemToSellID)
			{
				items.Add(item2);
			}
		}
	}

	// Token: 0x060013ED RID: 5101 RVA: 0x0009D69C File Offset: 0x0009B89C
	public bool DoTransaction(global::BasePlayer buyer, int sellOrderId, int numberOfTransactions = 1, global::ItemContainer targetContainer = null, Action<global::BasePlayer, global::Item> onCurrencyRemoved = null, Action<global::BasePlayer, global::Item> onItemPurchased = null)
	{
		if (sellOrderId < 0 || sellOrderId >= this.sellOrders.sellOrders.Count)
		{
			return false;
		}
		if (targetContainer == null && Vector3.Distance(buyer.transform.position, base.transform.position) > 4f)
		{
			return false;
		}
		ProtoBuf.VendingMachine.SellOrder sellOrder = this.sellOrders.sellOrders[sellOrderId];
		List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
		this.GetItemsToSell(sellOrder, list);
		if (list == null || list.Count == 0)
		{
			Facepunch.Pool.FreeList<global::Item>(ref list);
			return false;
		}
		numberOfTransactions = Mathf.Clamp(numberOfTransactions, 1, list[0].hasCondition ? 1 : 1000000);
		int num = sellOrder.itemToSellAmount * numberOfTransactions;
		int num2 = list.Sum((global::Item x) => x.amount);
		if (num > num2)
		{
			Facepunch.Pool.FreeList<global::Item>(ref list);
			return false;
		}
		List<global::Item> list2 = buyer.inventory.FindItemIDs(sellOrder.currencyID);
		if (sellOrder.currencyIsBP)
		{
			list2 = (from x in buyer.inventory.FindItemIDs(this.blueprintBaseDef.itemid)
			where x.blueprintTarget == sellOrder.currencyID
			select x).ToList<global::Item>();
		}
		list2 = (from x in list2
		where !x.hasCondition || (x.conditionNormalized >= 0.5f && x.maxConditionNormalized > 0.5f)
		select x).ToList<global::Item>();
		if (list2.Count == 0)
		{
			Facepunch.Pool.FreeList<global::Item>(ref list);
			return false;
		}
		int num3 = list2.Sum((global::Item x) => x.amount);
		int num4 = sellOrder.currencyAmountPerItem * numberOfTransactions;
		if (num3 < num4)
		{
			Facepunch.Pool.FreeList<global::Item>(ref list);
			return false;
		}
		this.transactionActive = true;
		int num5 = 0;
		foreach (global::Item item in list2)
		{
			int num6 = Mathf.Min(num4 - num5, item.amount);
			global::Item item2;
			if (item.amount <= num6)
			{
				item2 = item;
			}
			else
			{
				item2 = item.SplitItem(num6);
			}
			this.TakeCurrencyItem(item2);
			if (onCurrencyRemoved != null)
			{
				onCurrencyRemoved(buyer, item2);
			}
			num5 += num6;
			if (num5 >= num4)
			{
				break;
			}
		}
		int num7 = 0;
		foreach (global::Item item3 in list)
		{
			int num8 = num - num7;
			global::Item item4;
			if (item3.amount <= num8)
			{
				item4 = item3;
			}
			else
			{
				item4 = item3.SplitItem(num8);
			}
			if (item4 == null)
			{
				Debug.LogError("Vending machine error, contact developers!");
			}
			else
			{
				num7 += item4.amount;
				this.RecordSaleAnalytics(item4);
				if (targetContainer == null)
				{
					this.GiveSoldItem(item4, buyer);
				}
				else if (!item4.MoveToContainer(targetContainer, -1, true, false, null, true))
				{
					item4.Drop(targetContainer.dropPosition, targetContainer.dropVelocity, default(Quaternion));
				}
				if (onItemPurchased != null)
				{
					onItemPurchased(buyer, item4);
				}
			}
			if (num7 >= num)
			{
				break;
			}
		}
		Facepunch.Pool.FreeList<global::Item>(ref list);
		this.UpdateEmptyFlag();
		this.transactionActive = false;
		return true;
	}

	// Token: 0x060013EE RID: 5102 RVA: 0x0009D9F0 File Offset: 0x0009BBF0
	protected virtual void RecordSaleAnalytics(global::Item itemSold)
	{
		Analytics.Server.VendingMachineTransaction(null, itemSold.info, itemSold.amount);
	}

	// Token: 0x060013EF RID: 5103 RVA: 0x0009DA04 File Offset: 0x0009BC04
	public virtual void TakeCurrencyItem(global::Item takenCurrencyItem)
	{
		if (!takenCurrencyItem.MoveToContainer(base.inventory, -1, true, false, null, true))
		{
			takenCurrencyItem.Drop(base.inventory.dropPosition, Vector3.zero, default(Quaternion));
		}
	}

	// Token: 0x060013F0 RID: 5104 RVA: 0x0009DA44 File Offset: 0x0009BC44
	public virtual void GiveSoldItem(global::Item soldItem, global::BasePlayer buyer)
	{
		buyer.GiveItem(soldItem, global::BaseEntity.GiveItemReason.PickedUp);
	}

	// Token: 0x060013F1 RID: 5105 RVA: 0x0009DA4E File Offset: 0x0009BC4E
	public void SendSellOrders(global::BasePlayer player = null)
	{
		if (player)
		{
			base.ClientRPCPlayer<ProtoBuf.VendingMachine.SellOrderContainer>(null, player, "CLIENT_ReceiveSellOrders", this.sellOrders);
			return;
		}
		base.ClientRPC<ProtoBuf.VendingMachine.SellOrderContainer>(null, "CLIENT_ReceiveSellOrders", this.sellOrders);
	}

	// Token: 0x060013F2 RID: 5106 RVA: 0x0009DA80 File Offset: 0x0009BC80
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Broadcast(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		bool b = msg.read.Bit();
		if (this.CanPlayerAdmin(player))
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved4, b, false, true);
			this.UpdateMapMarker();
		}
	}

	// Token: 0x060013F3 RID: 5107 RVA: 0x0009DAC0 File Offset: 0x0009BCC0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_UpdateShopName(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		string text = msg.read.String(32);
		if (this.CanPlayerAdmin(player))
		{
			this.shopName = text;
			this.UpdateMapMarker();
		}
	}

	// Token: 0x060013F4 RID: 5108 RVA: 0x0009DAF8 File Offset: 0x0009BCF8
	public void UpdateMapMarker()
	{
		if (!this.IsBroadcasting())
		{
			if (this.myMarker)
			{
				this.myMarker.Kill(global::BaseNetworkable.DestroyMode.None);
				this.myMarker = null;
			}
			return;
		}
		bool flag = false;
		if (this.myMarker == null)
		{
			this.myMarker = (GameManager.server.CreateEntity(this.mapMarkerPrefab.resourcePath, base.transform.position, Quaternion.identity, true) as VendingMachineMapMarker);
			flag = true;
		}
		this.myMarker.SetFlag(global::BaseEntity.Flags.Busy, this.OutOfStock(), false, true);
		this.myMarker.markerShopName = this.shopName;
		this.myMarker.server_vendingMachine = this;
		if (flag)
		{
			this.myMarker.Spawn();
			return;
		}
		this.myMarker.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060013F5 RID: 5109 RVA: 0x0009DBC4 File Offset: 0x0009BDC4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_OpenShop(global::BaseEntity.RPCMessage msg)
	{
		if (!base.OccupiedCheck(msg.player))
		{
			return;
		}
		this.SendSellOrders(msg.player);
		this.PlayerOpenLoot(msg.player, this.customerPanel, true);
	}

	// Token: 0x060013F6 RID: 5110 RVA: 0x0009DBF8 File Offset: 0x0009BDF8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_OpenAdmin(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.CanPlayerAdmin(player))
		{
			return;
		}
		this.SendSellOrders(player);
		this.PlayerOpenLoot(player, "", true);
		base.ClientRPCPlayer(null, player, "CLIENT_OpenAdminMenu");
	}

	// Token: 0x060013F7 RID: 5111 RVA: 0x0009DC38 File Offset: 0x0009BE38
	public void OnIndustrialItemTransferBegins()
	{
		this.industrialItemIncoming = true;
	}

	// Token: 0x060013F8 RID: 5112 RVA: 0x0009DC41 File Offset: 0x0009BE41
	public void OnIndustrialItemTransferEnds()
	{
		this.industrialItemIncoming = false;
	}

	// Token: 0x060013F9 RID: 5113 RVA: 0x0009DC4C File Offset: 0x0009BE4C
	public bool CanAcceptItem(global::Item item, int targetSlot)
	{
		global::BasePlayer ownerPlayer = item.GetOwnerPlayer();
		return this.transactionActive || this.industrialItemIncoming || item.parent == null || base.inventory.itemList.Contains(item) || (!(ownerPlayer == null) && this.CanPlayerAdmin(ownerPlayer));
	}

	// Token: 0x060013FA RID: 5114 RVA: 0x0009DCA3 File Offset: 0x0009BEA3
	public override bool CanMoveFrom(global::BasePlayer player, global::Item item)
	{
		return this.CanPlayerAdmin(player);
	}

	// Token: 0x060013FB RID: 5115 RVA: 0x0009DCAC File Offset: 0x0009BEAC
	public override bool CanOpenLootPanel(global::BasePlayer player, string panelName)
	{
		return panelName == this.customerPanel || (base.CanOpenLootPanel(player, panelName) && this.CanPlayerAdmin(player));
	}

	// Token: 0x060013FC RID: 5116 RVA: 0x0009DCD4 File Offset: 0x0009BED4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_DeleteSellOrder(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.CanPlayerAdmin(player))
		{
			return;
		}
		int num = msg.read.Int32();
		if (num >= 0 && num < this.sellOrders.sellOrders.Count)
		{
			this.sellOrders.sellOrders.RemoveAt(num);
		}
		this.RefreshSellOrderStockLevel(null);
		this.UpdateMapMarker();
		this.SendSellOrders(player);
	}

	// Token: 0x060013FD RID: 5117 RVA: 0x0009DD3C File Offset: 0x0009BF3C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_RotateVM(global::BaseEntity.RPCMessage msg)
	{
		if (!this.CanRotate())
		{
			return;
		}
		this.UpdateEmptyFlag();
		if (msg.player.CanBuild() && this.IsInventoryEmpty())
		{
			base.transform.rotation = Quaternion.LookRotation(-base.transform.forward, base.transform.up);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x060013FE RID: 5118 RVA: 0x0009DDA0 File Offset: 0x0009BFA0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_AddSellOrder(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.CanPlayerAdmin(player))
		{
			return;
		}
		if (this.sellOrders.sellOrders.Count >= 7)
		{
			player.ChatMessage("Too many sell orders - remove some");
			return;
		}
		int itemToSellID = msg.read.Int32();
		int itemToSellAmount = msg.read.Int32();
		int currencyToUseID = msg.read.Int32();
		int currencyAmount = msg.read.Int32();
		byte bpState = msg.read.UInt8();
		this.AddSellOrder(itemToSellID, itemToSellAmount, currencyToUseID, currencyAmount, bpState);
	}

	// Token: 0x060013FF RID: 5119 RVA: 0x0009DE28 File Offset: 0x0009C028
	public void AddSellOrder(int itemToSellID, int itemToSellAmount, int currencyToUseID, int currencyAmount, byte bpState)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemToSellID);
		ItemDefinition x = ItemManager.FindItemDefinition(currencyToUseID);
		if (itemDefinition == null || x == null)
		{
			return;
		}
		currencyAmount = Mathf.Clamp(currencyAmount, 1, 10000);
		itemToSellAmount = Mathf.Clamp(itemToSellAmount, 1, itemDefinition.stackable);
		ProtoBuf.VendingMachine.SellOrder sellOrder = new ProtoBuf.VendingMachine.SellOrder();
		sellOrder.ShouldPool = false;
		sellOrder.itemToSellID = itemToSellID;
		sellOrder.itemToSellAmount = itemToSellAmount;
		sellOrder.currencyID = currencyToUseID;
		sellOrder.currencyAmountPerItem = currencyAmount;
		sellOrder.currencyIsBP = (bpState == 3 || bpState == 2);
		sellOrder.itemToSellIsBP = (bpState == 3 || bpState == 1);
		this.sellOrders.sellOrders.Add(sellOrder);
		this.RefreshSellOrderStockLevel(itemDefinition);
		this.UpdateMapMarker();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001400 RID: 5120 RVA: 0x0009DEE9 File Offset: 0x0009C0E9
	public void RefreshAndSendNetworkUpdate()
	{
		this.RefreshSellOrderStockLevel(null);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001401 RID: 5121 RVA: 0x0009DEFC File Offset: 0x0009C0FC
	public void UpdateOrCreateSalesSheet()
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition("note");
		List<global::Item> list = base.inventory.FindItemsByItemID(itemDefinition.itemid);
		global::Item item = null;
		foreach (global::Item item2 in list)
		{
			if (item2.text.Length == 0)
			{
				item = item2;
				break;
			}
		}
		if (item == null)
		{
			ItemDefinition itemDefinition2 = ItemManager.FindItemDefinition("paper");
			global::Item item3 = base.inventory.FindItemByItemID(itemDefinition2.itemid);
			if (item3 != null)
			{
				item = ItemManager.CreateByItemID(itemDefinition.itemid, 1, 0UL);
				if (!item.MoveToContainer(base.inventory, -1, true, false, null, true))
				{
					item.Drop(this.GetDropPosition(), this.GetDropVelocity(), default(Quaternion));
				}
				item3.UseItem(1);
			}
		}
		if (item != null)
		{
			foreach (ProtoBuf.VendingMachine.SellOrder sellOrder in this.sellOrders.sellOrders)
			{
				ItemDefinition itemDefinition3 = ItemManager.FindItemDefinition(sellOrder.itemToSellID);
				global::Item item4 = item;
				item4.text = item4.text + itemDefinition3.displayName.translated + "\n";
			}
			item.MarkDirty();
		}
	}

	// Token: 0x06001402 RID: 5122 RVA: 0x0009E058 File Offset: 0x0009C258
	protected virtual bool CanRotate()
	{
		return !base.HasAttachedStorageAdaptor();
	}

	// Token: 0x06001403 RID: 5123 RVA: 0x00020F08 File Offset: 0x0001F108
	public bool IsBroadcasting()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved4);
	}

	// Token: 0x06001404 RID: 5124 RVA: 0x00020A80 File Offset: 0x0001EC80
	public bool IsInventoryEmpty()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06001405 RID: 5125 RVA: 0x00004C84 File Offset: 0x00002E84
	public bool IsVending()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved2);
	}

	// Token: 0x06001406 RID: 5126 RVA: 0x0009E064 File Offset: 0x0009C264
	public bool PlayerBehind(global::BasePlayer player)
	{
		return Vector3.Dot(base.transform.forward, (player.transform.position - base.transform.position).normalized) <= -0.7f;
	}

	// Token: 0x06001407 RID: 5127 RVA: 0x0009E0B0 File Offset: 0x0009C2B0
	public bool PlayerInfront(global::BasePlayer player)
	{
		return Vector3.Dot(base.transform.forward, (player.transform.position - base.transform.position).normalized) >= 0.7f;
	}

	// Token: 0x06001408 RID: 5128 RVA: 0x0009E0FA File Offset: 0x0009C2FA
	public virtual bool CanPlayerAdmin(global::BasePlayer player)
	{
		return this.PlayerBehind(player) && base.OccupiedCheck(player);
	}

	// Token: 0x06001409 RID: 5129 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x04000C7D RID: 3197
	[Header("VendingMachine")]
	public static readonly Translate.Phrase WaitForVendingMessage = new Translate.Phrase("vendingmachine.wait", "Please wait...");

	// Token: 0x04000C7E RID: 3198
	public GameObjectRef adminMenuPrefab;

	// Token: 0x04000C7F RID: 3199
	public string customerPanel = "";

	// Token: 0x04000C80 RID: 3200
	public ProtoBuf.VendingMachine.SellOrderContainer sellOrders;

	// Token: 0x04000C81 RID: 3201
	public SoundPlayer buySound;

	// Token: 0x04000C82 RID: 3202
	public string shopName = "A Shop";

	// Token: 0x04000C83 RID: 3203
	public GameObjectRef mapMarkerPrefab;

	// Token: 0x04000C84 RID: 3204
	public ItemDefinition blueprintBaseDef;

	// Token: 0x04000C85 RID: 3205
	private Action fullUpdateCached;

	// Token: 0x04000C86 RID: 3206
	protected global::BasePlayer vend_Player;

	// Token: 0x04000C87 RID: 3207
	private int vend_sellOrderID;

	// Token: 0x04000C88 RID: 3208
	private int vend_numberOfTransactions;

	// Token: 0x04000C89 RID: 3209
	protected bool transactionActive;

	// Token: 0x04000C8A RID: 3210
	private VendingMachineMapMarker myMarker;

	// Token: 0x04000C8B RID: 3211
	private bool industrialItemIncoming;

	// Token: 0x02000BC8 RID: 3016
	public static class VendingMachineFlags
	{
		// Token: 0x04003F97 RID: 16279
		public const global::BaseEntity.Flags EmptyInv = global::BaseEntity.Flags.Reserved1;

		// Token: 0x04003F98 RID: 16280
		public const global::BaseEntity.Flags IsVending = global::BaseEntity.Flags.Reserved2;

		// Token: 0x04003F99 RID: 16281
		public const global::BaseEntity.Flags Broadcasting = global::BaseEntity.Flags.Reserved4;

		// Token: 0x04003F9A RID: 16282
		public const global::BaseEntity.Flags OutOfStock = global::BaseEntity.Flags.Reserved5;

		// Token: 0x04003F9B RID: 16283
		public const global::BaseEntity.Flags NoDirectAccess = global::BaseEntity.Flags.Reserved6;
	}
}
