using System;
using Facepunch;
using Network;
using ProtoBuf;

// Token: 0x0200049F RID: 1183
public class StorageMonitor : AppIOEntity
{
	// Token: 0x1700030D RID: 781
	// (get) Token: 0x06002652 RID: 9810 RVA: 0x00002E0E File Offset: 0x0000100E
	public override AppEntityType Type
	{
		get
		{
			return AppEntityType.StorageMonitor;
		}
	}

	// Token: 0x1700030E RID: 782
	// (get) Token: 0x06002653 RID: 9811 RVA: 0x00006C79 File Offset: 0x00004E79
	// (set) Token: 0x06002654 RID: 9812 RVA: 0x000059DD File Offset: 0x00003BDD
	public override bool Value
	{
		get
		{
			return base.IsOn();
		}
		set
		{
		}
	}

	// Token: 0x06002655 RID: 9813 RVA: 0x000EEB31 File Offset: 0x000ECD31
	public StorageMonitor()
	{
		this._onContainerChangedHandler = new Action<global::Item, bool>(this.OnContainerChanged);
		this._resetSwitchHandler = new Action(this.ResetSwitch);
	}

	// Token: 0x06002656 RID: 9814 RVA: 0x000EEB60 File Offset: 0x000ECD60
	internal override void FillEntityPayload(AppEntityPayload payload)
	{
		base.FillEntityPayload(payload);
		StorageContainer storageContainer = this.GetStorageContainer();
		if (storageContainer == null || !base.HasFlag(global::BaseEntity.Flags.Reserved8))
		{
			return;
		}
		payload.items = Pool.GetList<AppEntityPayload.Item>();
		foreach (global::Item item in storageContainer.inventory.itemList)
		{
			AppEntityPayload.Item item2 = Pool.Get<AppEntityPayload.Item>();
			item2.itemId = (item.IsBlueprint() ? item.blueprintTargetDef.itemid : item.info.itemid);
			item2.quantity = item.amount;
			item2.itemIsBlueprint = item.IsBlueprint();
			payload.items.Add(item2);
		}
		payload.capacity = storageContainer.inventory.capacity;
		BuildingPrivlidge buildingPrivlidge;
		if ((buildingPrivlidge = (storageContainer as BuildingPrivlidge)) != null)
		{
			payload.hasProtection = true;
			float protectedMinutes = buildingPrivlidge.GetProtectedMinutes(false);
			if (protectedMinutes > 0f)
			{
				payload.protectionExpiry = (uint)DateTimeOffset.UtcNow.AddMinutes((double)protectedMinutes).ToUnixTimeSeconds();
			}
		}
	}

	// Token: 0x06002657 RID: 9815 RVA: 0x000EEC8C File Offset: 0x000ECE8C
	public override void Init()
	{
		base.Init();
		StorageContainer storageContainer = this.GetStorageContainer();
		if (storageContainer != null && storageContainer.inventory != null)
		{
			global::ItemContainer inventory = storageContainer.inventory;
			inventory.onItemAddedRemoved = (Action<global::Item, bool>)Delegate.Combine(inventory.onItemAddedRemoved, this._onContainerChangedHandler);
		}
	}

	// Token: 0x06002658 RID: 9816 RVA: 0x000EECD8 File Offset: 0x000ECED8
	public override void DestroyShared()
	{
		base.DestroyShared();
		StorageContainer storageContainer = this.GetStorageContainer();
		if (storageContainer != null && storageContainer.inventory != null)
		{
			global::ItemContainer inventory = storageContainer.inventory;
			inventory.onItemAddedRemoved = (Action<global::Item, bool>)Delegate.Remove(inventory.onItemAddedRemoved, this._onContainerChangedHandler);
		}
	}

	// Token: 0x06002659 RID: 9817 RVA: 0x000EED24 File Offset: 0x000ECF24
	private StorageContainer GetStorageContainer()
	{
		return base.GetParentEntity() as StorageContainer;
	}

	// Token: 0x0600265A RID: 9818 RVA: 0x0005E459 File Offset: 0x0005C659
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.GetCurrentEnergy();
	}

	// Token: 0x0600265B RID: 9819 RVA: 0x000EED34 File Offset: 0x000ECF34
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		bool flag = base.HasFlag(global::BaseEntity.Flags.Reserved8);
		base.UpdateHasPower(inputAmount, inputSlot);
		if (inputSlot == 0)
		{
			bool flag2 = inputAmount >= this.ConsumptionAmount();
			double realtimeSinceStartup = TimeEx.realtimeSinceStartup;
			if (flag2 && !flag && this._lastPowerOnUpdate < realtimeSinceStartup - 1.0)
			{
				this._lastPowerOnUpdate = realtimeSinceStartup;
				base.BroadcastValueChange();
			}
		}
	}

	// Token: 0x0600265C RID: 9820 RVA: 0x000EED90 File Offset: 0x000ECF90
	private void OnContainerChanged(global::Item item, bool added)
	{
		if (!base.HasFlag(global::BaseEntity.Flags.Reserved8))
		{
			return;
		}
		base.Invoke(this._resetSwitchHandler, 0.5f);
		if (base.IsOn())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
		base.BroadcastValueChange();
	}

	// Token: 0x0600265D RID: 9821 RVA: 0x000EEDE2 File Offset: 0x000ECFE2
	private void ResetSwitch()
	{
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
		base.BroadcastValueChange();
	}

	// Token: 0x04001F16 RID: 7958
	private readonly Action<global::Item, bool> _onContainerChangedHandler;

	// Token: 0x04001F17 RID: 7959
	private readonly Action _resetSwitchHandler;

	// Token: 0x04001F18 RID: 7960
	private double _lastPowerOnUpdate;
}
