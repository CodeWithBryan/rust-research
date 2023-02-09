using System;
using ConVar;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x020000FC RID: 252
public class Composter : StorageContainer
{
	// Token: 0x170001AC RID: 428
	// (get) Token: 0x060014C9 RID: 5321 RVA: 0x000A414D File Offset: 0x000A234D
	protected float UpdateInterval
	{
		get
		{
			return Server.composterUpdateInterval;
		}
	}

	// Token: 0x060014CA RID: 5322 RVA: 0x000A4154 File Offset: 0x000A2354
	public override void ServerInit()
	{
		base.ServerInit();
		global::ItemContainer inventory = base.inventory;
		inventory.canAcceptItem = (Func<global::Item, int, bool>)Delegate.Combine(inventory.canAcceptItem, new Func<global::Item, int, bool>(this.InventoryItemFilter));
		base.InvokeRandomized(new Action(this.UpdateComposting), this.UpdateInterval, this.UpdateInterval, this.UpdateInterval * 0.1f);
	}

	// Token: 0x060014CB RID: 5323 RVA: 0x000A41B8 File Offset: 0x000A23B8
	public bool InventoryItemFilter(global::Item item, int targetSlot)
	{
		return item != null && (item.info.GetComponent<ItemModCompostable>() != null || this.ItemIsFertilizer(item));
	}

	// Token: 0x060014CC RID: 5324 RVA: 0x000A41DE File Offset: 0x000A23DE
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.composter = Facepunch.Pool.Get<ProtoBuf.Composter>();
		info.msg.composter.fertilizerProductionProgress = this.fertilizerProductionProgress;
	}

	// Token: 0x060014CD RID: 5325 RVA: 0x000A420D File Offset: 0x000A240D
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.composter != null)
		{
			this.fertilizerProductionProgress = info.msg.composter.fertilizerProductionProgress;
		}
	}

	// Token: 0x060014CE RID: 5326 RVA: 0x000A4239 File Offset: 0x000A2439
	private bool ItemIsFertilizer(global::Item item)
	{
		return item.info.shortname == "fertilizer";
	}

	// Token: 0x060014CF RID: 5327 RVA: 0x000A4250 File Offset: 0x000A2450
	public void UpdateComposting()
	{
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			global::Item slot = base.inventory.GetSlot(i);
			if (slot != null)
			{
				this.CompostItem(slot);
			}
		}
	}

	// Token: 0x060014D0 RID: 5328 RVA: 0x000A428C File Offset: 0x000A248C
	private void CompostItem(global::Item item)
	{
		if (this.ItemIsFertilizer(item))
		{
			return;
		}
		ItemModCompostable component = item.info.GetComponent<ItemModCompostable>();
		if (component == null)
		{
			return;
		}
		int num = this.CompostEntireStack ? item.amount : 1;
		item.UseItem(num);
		this.fertilizerProductionProgress += (float)num * component.TotalFertilizerProduced;
		this.ProduceFertilizer(Mathf.FloorToInt(this.fertilizerProductionProgress));
	}

	// Token: 0x060014D1 RID: 5329 RVA: 0x000A42FC File Offset: 0x000A24FC
	private void ProduceFertilizer(int amount)
	{
		if (amount <= 0)
		{
			return;
		}
		global::Item item = ItemManager.Create(this.FertilizerDef, amount, 0UL);
		if (!item.MoveToContainer(base.inventory, -1, true, false, null, true))
		{
			item.Drop(base.inventory.dropPosition, base.inventory.dropVelocity, default(Quaternion));
		}
		this.fertilizerProductionProgress -= (float)amount;
	}

	// Token: 0x04000D5A RID: 3418
	[Header("Composter")]
	public ItemDefinition FertilizerDef;

	// Token: 0x04000D5B RID: 3419
	[Tooltip("If enabled, entire item stacks will be composted each tick, instead of a single item of a stack.")]
	public bool CompostEntireStack;

	// Token: 0x04000D5C RID: 3420
	private float fertilizerProductionProgress;
}
