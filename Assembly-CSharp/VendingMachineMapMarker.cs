using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;

// Token: 0x02000405 RID: 1029
public class VendingMachineMapMarker : MapMarker
{
	// Token: 0x06002299 RID: 8857 RVA: 0x000DD524 File Offset: 0x000DB724
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.vendingMachine = new ProtoBuf.VendingMachine();
		info.msg.vendingMachine.shopName = this.markerShopName;
		if (this.server_vendingMachine != null)
		{
			info.msg.vendingMachine.networkID = this.server_vendingMachine.net.ID;
			info.msg.vendingMachine.sellOrderContainer = new ProtoBuf.VendingMachine.SellOrderContainer();
			info.msg.vendingMachine.sellOrderContainer.ShouldPool = false;
			info.msg.vendingMachine.sellOrderContainer.sellOrders = new List<ProtoBuf.VendingMachine.SellOrder>();
			foreach (ProtoBuf.VendingMachine.SellOrder sellOrder in this.server_vendingMachine.sellOrders.sellOrders)
			{
				ProtoBuf.VendingMachine.SellOrder sellOrder2 = new ProtoBuf.VendingMachine.SellOrder();
				sellOrder2.ShouldPool = false;
				sellOrder.CopyTo(sellOrder2);
				info.msg.vendingMachine.sellOrderContainer.sellOrders.Add(sellOrder2);
			}
		}
	}

	// Token: 0x0600229A RID: 8858 RVA: 0x000DD64C File Offset: 0x000DB84C
	public override AppMarker GetAppMarkerData()
	{
		AppMarker appMarkerData = base.GetAppMarkerData();
		appMarkerData.name = (this.markerShopName ?? "");
		appMarkerData.outOfStock = !base.HasFlag(global::BaseEntity.Flags.Busy);
		if (this.server_vendingMachine != null)
		{
			appMarkerData.sellOrders = Pool.GetList<AppMarker.SellOrder>();
			foreach (ProtoBuf.VendingMachine.SellOrder sellOrder in this.server_vendingMachine.sellOrders.sellOrders)
			{
				AppMarker.SellOrder sellOrder2 = Pool.Get<AppMarker.SellOrder>();
				sellOrder2.itemId = sellOrder.itemToSellID;
				sellOrder2.quantity = sellOrder.itemToSellAmount;
				sellOrder2.currencyId = sellOrder.currencyID;
				sellOrder2.costPerItem = sellOrder.currencyAmountPerItem;
				sellOrder2.amountInStock = sellOrder.inStock;
				sellOrder2.itemIsBlueprint = sellOrder.itemToSellIsBP;
				sellOrder2.currencyIsBlueprint = sellOrder.currencyIsBP;
				sellOrder2.itemCondition = sellOrder.itemCondition;
				sellOrder2.itemConditionMax = sellOrder.itemConditionMax;
				appMarkerData.sellOrders.Add(sellOrder2);
			}
		}
		return appMarkerData;
	}

	// Token: 0x04001B0F RID: 6927
	public string markerShopName;

	// Token: 0x04001B10 RID: 6928
	public global::VendingMachine server_vendingMachine;

	// Token: 0x04001B11 RID: 6929
	public ProtoBuf.VendingMachine client_vendingMachine;

	// Token: 0x04001B12 RID: 6930
	[NonSerialized]
	public uint client_vendingMachineNetworkID;

	// Token: 0x04001B13 RID: 6931
	public GameObjectRef clusterMarkerObj;
}
