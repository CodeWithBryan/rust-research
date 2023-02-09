using System;
using Facepunch.Rust;
using UnityEngine;

// Token: 0x0200011F RID: 287
public class NPCVendingMachine : VendingMachine
{
	// Token: 0x060015B4 RID: 5556 RVA: 0x000A76CC File Offset: 0x000A58CC
	public byte GetBPState(bool sellItemAsBP, bool currencyItemAsBP)
	{
		byte result = 0;
		if (sellItemAsBP)
		{
			result = 1;
		}
		if (currencyItemAsBP)
		{
			result = 2;
		}
		if (sellItemAsBP && currencyItemAsBP)
		{
			result = 3;
		}
		return result;
	}

	// Token: 0x060015B5 RID: 5557 RVA: 0x000A76ED File Offset: 0x000A58ED
	public override void TakeCurrencyItem(Item takenCurrencyItem)
	{
		takenCurrencyItem.MoveToContainer(base.inventory, -1, true, false, null, true);
		takenCurrencyItem.RemoveFromContainer();
		takenCurrencyItem.Remove(0f);
	}

	// Token: 0x060015B6 RID: 5558 RVA: 0x000A7712 File Offset: 0x000A5912
	public override void GiveSoldItem(Item soldItem, BasePlayer buyer)
	{
		base.GiveSoldItem(soldItem, buyer);
	}

	// Token: 0x060015B7 RID: 5559 RVA: 0x000A771C File Offset: 0x000A591C
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.Invoke(new Action(this.InstallFromVendingOrders), 1f);
	}

	// Token: 0x060015B8 RID: 5560 RVA: 0x000A773C File Offset: 0x000A593C
	public override void ServerInit()
	{
		base.ServerInit();
		this.skinID = 861142659UL;
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		base.Invoke(new Action(this.InstallFromVendingOrders), 1f);
		base.InvokeRandomized(new Action(this.Refill), 1f, 1f, 0.1f);
	}

	// Token: 0x060015B9 RID: 5561 RVA: 0x000A779C File Offset: 0x000A599C
	public virtual void InstallFromVendingOrders()
	{
		if (this.vendingOrders == null)
		{
			Debug.LogError("No vending orders!");
			return;
		}
		this.ClearSellOrders();
		base.inventory.Clear();
		ItemManager.DoRemoves();
		foreach (NPCVendingOrder.Entry entry in this.vendingOrders.orders)
		{
			this.AddItemForSale(entry.sellItem.itemid, entry.sellItemAmount, entry.currencyItem.itemid, entry.currencyAmount, this.GetBPState(entry.sellItemAsBP, entry.currencyAsBP));
		}
	}

	// Token: 0x060015BA RID: 5562 RVA: 0x000A7830 File Offset: 0x000A5A30
	public override void InstallDefaultSellOrders()
	{
		base.InstallDefaultSellOrders();
	}

	// Token: 0x060015BB RID: 5563 RVA: 0x000A7838 File Offset: 0x000A5A38
	public void Refill()
	{
		if (this.vendingOrders == null || this.vendingOrders.orders == null)
		{
			return;
		}
		if (base.inventory == null)
		{
			return;
		}
		if (this.refillTimes == null)
		{
			this.refillTimes = new float[this.vendingOrders.orders.Length];
		}
		for (int i = 0; i < this.vendingOrders.orders.Length; i++)
		{
			NPCVendingOrder.Entry entry = this.vendingOrders.orders[i];
			if (Time.realtimeSinceStartup > this.refillTimes[i])
			{
				int num = Mathf.FloorToInt((float)(base.inventory.GetAmount(entry.sellItem.itemid, false) / entry.sellItemAmount));
				int num2 = Mathf.Min(10 - num, entry.refillAmount) * entry.sellItemAmount;
				if (num2 > 0)
				{
					this.transactionActive = true;
					Item item;
					if (entry.sellItemAsBP)
					{
						item = ItemManager.Create(this.blueprintBaseDef, num2, 0UL);
						item.blueprintTarget = entry.sellItem.itemid;
					}
					else
					{
						item = ItemManager.Create(entry.sellItem, num2, 0UL);
					}
					item.MoveToContainer(base.inventory, -1, true, false, null, true);
					this.transactionActive = false;
				}
				this.refillTimes[i] = Time.realtimeSinceStartup + entry.refillDelay;
			}
		}
	}

	// Token: 0x060015BC RID: 5564 RVA: 0x000A797E File Offset: 0x000A5B7E
	public void ClearSellOrders()
	{
		this.sellOrders.sellOrders.Clear();
	}

	// Token: 0x060015BD RID: 5565 RVA: 0x000A7990 File Offset: 0x000A5B90
	public void AddItemForSale(int itemID, int amountToSell, int currencyID, int currencyPerTransaction, byte bpState)
	{
		base.AddSellOrder(itemID, amountToSell, currencyID, currencyPerTransaction, bpState);
		this.transactionActive = true;
		int num = 10;
		if (bpState == 1 || bpState == 3)
		{
			for (int i = 0; i < num; i++)
			{
				Item item = ItemManager.CreateByItemID(this.blueprintBaseDef.itemid, 1, 0UL);
				item.blueprintTarget = itemID;
				base.inventory.Insert(item);
			}
		}
		else
		{
			base.inventory.AddItem(ItemManager.FindItemDefinition(itemID), amountToSell * num, 0UL, ItemContainer.LimitStack.Existing);
		}
		this.transactionActive = false;
		base.RefreshSellOrderStockLevel(null);
	}

	// Token: 0x060015BE RID: 5566 RVA: 0x000059DD File Offset: 0x00003BDD
	public void RefreshStock()
	{
	}

	// Token: 0x060015BF RID: 5567 RVA: 0x000A7A19 File Offset: 0x000A5C19
	protected override void RecordSaleAnalytics(Item itemSold)
	{
		Analytics.Server.VendingMachineTransaction(this.vendingOrders, itemSold.info, itemSold.amount);
	}

	// Token: 0x060015C0 RID: 5568 RVA: 0x00007074 File Offset: 0x00005274
	protected override bool CanRotate()
	{
		return false;
	}

	// Token: 0x060015C1 RID: 5569 RVA: 0x00007074 File Offset: 0x00005274
	public override bool CanPlayerAdmin(BasePlayer player)
	{
		return false;
	}

	// Token: 0x04000E0D RID: 3597
	public NPCVendingOrder vendingOrders;

	// Token: 0x04000E0E RID: 3598
	private float[] refillTimes;
}
