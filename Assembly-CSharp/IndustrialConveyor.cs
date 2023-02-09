using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

// Token: 0x0200007F RID: 127
public class IndustrialConveyor : IndustrialEntity
{
	// Token: 0x06000BF7 RID: 3063 RVA: 0x00066FBC File Offset: 0x000651BC
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("IndustrialConveyor.OnRpcMessage", 0))
		{
			if (rpc == 617569194U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_ChangeFilters ");
				}
				using (TimeWarning.New("RPC_ChangeFilters", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(617569194U, "RPC_ChangeFilters", this, player, 1UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(617569194U, "RPC_ChangeFilters", this, player, 3f))
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
							this.RPC_ChangeFilters(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_ChangeFilters");
					}
				}
				return true;
			}
			if (rpc == 4167839872U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SvSwitch ");
				}
				using (TimeWarning.New("SvSwitch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(4167839872U, "SvSwitch", this, player, 2UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(4167839872U, "SvSwitch", this, player, 3f))
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
							this.SvSwitch(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in SvSwitch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000BF8 RID: 3064 RVA: 0x000672F4 File Offset: 0x000654F4
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		bool flag = next.HasFlag(global::BaseEntity.Flags.On);
		if (old.HasFlag(global::BaseEntity.Flags.On) != flag && base.isServer)
		{
			float conveyorMoveFrequency = ConVar.Server.conveyorMoveFrequency;
			if (flag && conveyorMoveFrequency > 0f)
			{
				base.InvokeRandomized(new Action(this.ScheduleMove), conveyorMoveFrequency, conveyorMoveFrequency, conveyorMoveFrequency * 0.5f);
				return;
			}
			base.CancelInvoke(new Action(this.ScheduleMove));
		}
	}

	// Token: 0x06000BF9 RID: 3065 RVA: 0x00067376 File Offset: 0x00065576
	private void ScheduleMove()
	{
		IndustrialEntity.Queue.Add(this);
	}

	// Token: 0x06000BFA RID: 3066 RVA: 0x00067384 File Offset: 0x00065584
	private global::Item GetItemToMove(IIndustrialStorage storage, out global::IndustrialConveyor.ItemFilter associatedFilter, int slot, global::ItemContainer targetContainer = null)
	{
		associatedFilter = default(global::IndustrialConveyor.ItemFilter);
		ValueTuple<global::IndustrialConveyor.ItemFilter, int> valueTuple = default(ValueTuple<global::IndustrialConveyor.ItemFilter, int>);
		if (storage == null || storage.Container == null)
		{
			return null;
		}
		if (storage.Container.IsEmpty())
		{
			return null;
		}
		Vector2i vector2i = storage.OutputSlotRange(slot);
		for (int i = vector2i.x; i <= vector2i.y; i++)
		{
			global::Item slot2 = storage.Container.GetSlot(i);
			valueTuple = default(ValueTuple<global::IndustrialConveyor.ItemFilter, int>);
			if (slot2 != null && (this.filterItems.Count == 0 || this.FilterHasItem(slot2, out valueTuple)))
			{
				associatedFilter = valueTuple.Item1;
				if (targetContainer == null || !(associatedFilter.TargetItem != null) || associatedFilter.MaxAmountInOutput <= 0 || targetContainer.GetTotalItemAmount(associatedFilter.TargetItem, vector2i.x, vector2i.y) < associatedFilter.MaxAmountInOutput)
				{
					return slot2;
				}
			}
		}
		return null;
	}

	// Token: 0x06000BFB RID: 3067 RVA: 0x00067458 File Offset: 0x00065658
	private bool FilterHasItem(global::Item item, [TupleElementNames(new string[]
	{
		"filter",
		"index"
	})] out ValueTuple<global::IndustrialConveyor.ItemFilter, int> filter)
	{
		filter = default(ValueTuple<global::IndustrialConveyor.ItemFilter, int>);
		for (int i = 0; i < this.filterItems.Count; i++)
		{
			global::IndustrialConveyor.ItemFilter itemFilter = this.filterItems[i];
			if (this.FilterMatches(itemFilter, item))
			{
				filter = new ValueTuple<global::IndustrialConveyor.ItemFilter, int>(itemFilter, i);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000BFC RID: 3068 RVA: 0x000674AC File Offset: 0x000656AC
	private bool FilterMatches(global::IndustrialConveyor.ItemFilter filter, global::Item item)
	{
		if (item.IsBlueprint() && filter.IsBlueprint && item.blueprintTargetDef == filter.TargetItem)
		{
			return true;
		}
		if (filter.TargetItem == item.info && !filter.IsBlueprint)
		{
			return true;
		}
		if (filter.TargetItem != null && item.info.isRedirectOf == filter.TargetItem)
		{
			return true;
		}
		if (filter.TargetCategory != null)
		{
			ItemCategory category = item.info.category;
			ItemCategory? targetCategory = filter.TargetCategory;
			if (category == targetCategory.GetValueOrDefault() & targetCategory != null)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000BFD RID: 3069 RVA: 0x0006755C File Offset: 0x0006575C
	private bool FilterContainerInput(IIndustrialStorage storage, int slot)
	{
		IIndustrialStorage industrialStorage = this.workerOutput;
		global::IndustrialConveyor.ItemFilter itemFilter;
		return this.GetItemToMove(storage, out itemFilter, slot, (industrialStorage != null) ? industrialStorage.Container : null) != null;
	}

	// Token: 0x06000BFE RID: 3070 RVA: 0x00067588 File Offset: 0x00065788
	protected override void RunJob()
	{
		base.RunJob();
		if (ConVar.Server.conveyorMoveFrequency <= 0f)
		{
			return;
		}
		if (this.filterFunc == null)
		{
			this.filterFunc = new Func<IIndustrialStorage, int, bool>(this.FilterContainerInput);
		}
		this.splitInputs.Clear();
		base.FindContainerSource(this.splitInputs, global::IOEntity.backtracking * 2, true, null);
		bool flag = this.CheckIfAnyInputPassesFilters(this.splitInputs);
		if (this.lastFilterState != null)
		{
			bool flag2 = flag;
			bool? flag3 = this.lastFilterState;
			if (flag2 == flag3.GetValueOrDefault() & flag3 != null)
			{
				goto IL_BA;
			}
		}
		this.lastFilterState = new bool?(flag);
		base.SetFlag(global::BaseEntity.Flags.Reserved9, flag, false, false);
		base.SetFlag(global::BaseEntity.Flags.Reserved10, !flag, false, true);
		this.ensureOutputsUpdated = true;
		this.MarkDirty();
		IL_BA:
		if (!flag)
		{
			return;
		}
		global::IndustrialConveyor.<>c__DisplayClass23_0 CS$<>8__locals1;
		CS$<>8__locals1.transfer = Facepunch.Pool.Get<IndustrialConveyorTransfer>();
		try
		{
			CS$<>8__locals1.transfer.ItemTransfers = Facepunch.Pool.GetList<IndustrialConveyorTransfer.ItemTransfer>();
			CS$<>8__locals1.transfer.inputEntities = Facepunch.Pool.GetList<uint>();
			CS$<>8__locals1.transfer.outputEntities = Facepunch.Pool.GetList<uint>();
			this.splitOutputs.Clear();
			base.FindContainerSource(this.splitOutputs, global::IOEntity.backtracking * 2, false, null);
			foreach (global::IOEntity.ContainerInputOutput containerInputOutput in this.splitOutputs)
			{
				this.workerOutput = containerInputOutput.Storage;
				foreach (global::IOEntity.ContainerInputOutput containerInputOutput2 in this.splitInputs)
				{
					int num = 0;
					IIndustrialStorage storage = containerInputOutput2.Storage;
					if (storage != null && containerInputOutput.Storage != null && !(containerInputOutput2.Storage.IndustrialEntity == containerInputOutput.Storage.IndustrialEntity))
					{
						bool container = storage.Container != null;
						global::ItemContainer container2 = containerInputOutput.Storage.Container;
						if (container && container2 != null && storage.Container != null && !storage.Container.IsEmpty())
						{
							ValueTuple<global::IndustrialConveyor.ItemFilter, int> valueTuple = default(ValueTuple<global::IndustrialConveyor.ItemFilter, int>);
							Vector2i vector2i = storage.OutputSlotRange(containerInputOutput2.SlotIndex);
							for (int i = vector2i.x; i <= vector2i.y; i++)
							{
								Vector2i vector2i2 = containerInputOutput.Storage.InputSlotRange(containerInputOutput.SlotIndex);
								global::Item slot = storage.Container.GetSlot(i);
								if (slot != null && (this.filterItems.Count == 0 || this.FilterHasItem(slot, out valueTuple)) && (!(valueTuple.Item1.TargetItem != null) || valueTuple.Item1.MaxAmountInOutput <= 0 || containerInputOutput.Storage.Container.GetTotalItemAmount(valueTuple.Item1.TargetItem, vector2i2.x, vector2i2.y) < valueTuple.Item1.MaxAmountInOutput))
								{
									int num2 = Mathf.Min(this.MaxStackSizePerMove, slot.info.stackable) / this.splitOutputs.Count;
									if (slot.amount == 1 || (num2 <= 0 && slot.amount > 0))
									{
										num2 = 1;
									}
									if (valueTuple.Item1.MinAmountToTransfer > 0)
									{
										num2 = Mathf.Min(num2, valueTuple.Item1.MinTransferRemaining);
									}
									if (valueTuple.Item1.MaxAmountInOutput > 0)
									{
										if (valueTuple.Item1.TargetItem == slot.info && valueTuple.Item1.TargetItem != null)
										{
											num2 = Mathf.Min(num2, valueTuple.Item1.MaxAmountInOutput - container2.GetTotalItemAmount(slot.info, vector2i2.x, vector2i2.y));
										}
										else if (valueTuple.Item1.TargetCategory != null)
										{
											num2 = Mathf.Min(num2, valueTuple.Item1.MaxAmountInOutput - container2.GetTotalCategoryAmount(valueTuple.Item1.TargetCategory.Value, vector2i2.x, vector2i2.y));
										}
									}
									if (num2 > 0)
									{
										global::Item item = null;
										int amount = slot.amount;
										if (slot.amount > num2)
										{
											item = slot.SplitItem(num2);
											amount = item.amount;
										}
										containerInputOutput.Storage.OnStorageItemTransferBegin();
										bool flag4 = false;
										for (int j = vector2i2.x; j <= vector2i2.y; j++)
										{
											global::Item slot2 = container2.GetSlot(j);
											if (slot2 == null || slot2.info == slot.info)
											{
												if (item != null)
												{
													if (item.MoveToContainer(container2, j, true, false, null, false))
													{
														flag4 = true;
														break;
													}
												}
												else if (slot.MoveToContainer(container2, j, true, false, null, false))
												{
													flag4 = true;
													break;
												}
											}
										}
										if (valueTuple.Item1.MinTransferRemaining > 0)
										{
											global::IndustrialConveyor.ItemFilter item2 = valueTuple.Item1;
											item2.MinTransferRemaining -= amount;
											this.filterItems[valueTuple.Item2] = item2;
										}
										if (!flag4 && item != null)
										{
											slot.amount += item.amount;
											slot.MarkDirty();
											item.Remove(0f);
											item = null;
										}
										if (flag4)
										{
											num++;
											if (item != null)
											{
												global::IndustrialConveyor.<RunJob>g__AddTransfer|23_0(item.info.itemid, amount, containerInputOutput2.Storage.IndustrialEntity, containerInputOutput.Storage.IndustrialEntity, ref CS$<>8__locals1);
											}
											else
											{
												global::IndustrialConveyor.<RunJob>g__AddTransfer|23_0(slot.info.itemid, amount, containerInputOutput2.Storage.IndustrialEntity, containerInputOutput.Storage.IndustrialEntity, ref CS$<>8__locals1);
											}
										}
										containerInputOutput.Storage.OnStorageItemTransferEnd();
										if (num >= ConVar.Server.maxItemStacksMovedPerTickIndustrial)
										{
											break;
										}
									}
								}
							}
						}
					}
				}
			}
			if (CS$<>8__locals1.transfer.ItemTransfers.Count > 0)
			{
				base.ClientRPCEx<IndustrialConveyorTransfer>(new SendInfo(global::BaseNetworkable.GetConnectionsWithin(base.transform.position, 30f)), null, "ReceiveItemTransferDetails", CS$<>8__locals1.transfer);
			}
		}
		finally
		{
			if (CS$<>8__locals1.transfer != null)
			{
				((IDisposable)CS$<>8__locals1.transfer).Dispose();
			}
		}
	}

	// Token: 0x06000BFF RID: 3071 RVA: 0x00067BF0 File Offset: 0x00065DF0
	private bool CheckIfAnyInputPassesFilters(List<global::IOEntity.ContainerInputOutput> inputs)
	{
		if (this.filterItems.Count == 0)
		{
			using (List<global::IOEntity.ContainerInputOutput>.Enumerator enumerator = inputs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::IOEntity.ContainerInputOutput containerInputOutput = enumerator.Current;
					global::IndustrialConveyor.ItemFilter itemFilter;
					if (this.GetItemToMove(containerInputOutput.Storage, out itemFilter, containerInputOutput.SlotIndex, null) != null)
					{
						return true;
					}
				}
				return false;
			}
		}
		for (int i = 0; i < this.filterItems.Count; i++)
		{
			global::IndustrialConveyor.ItemFilter itemFilter2 = this.filterItems[i];
			int num = 0;
			foreach (global::IOEntity.ContainerInputOutput containerInputOutput2 in inputs)
			{
				Vector2i vector2i = containerInputOutput2.Storage.OutputSlotRange(containerInputOutput2.SlotIndex);
				for (int j = vector2i.x; j <= vector2i.y; j++)
				{
					global::Item slot = containerInputOutput2.Storage.Container.GetSlot(j);
					if (slot != null && this.FilterMatches(itemFilter2, slot))
					{
						if (itemFilter2.MinAmountToTransfer <= 0)
						{
							return true;
						}
						if (itemFilter2.MinTransferRemaining > 0)
						{
							return true;
						}
						num += slot.amount;
						if (num >= itemFilter2.MinAmountToTransfer)
						{
							itemFilter2.MinTransferRemaining = itemFilter2.MinAmountToTransfer;
							this.filterItems[i] = itemFilter2;
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06000C00 RID: 3072 RVA: 0x00067D8C File Offset: 0x00065F8C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (this.filterItems.Count == 0)
		{
			return;
		}
		info.msg.industrialConveyor = Facepunch.Pool.Get<ProtoBuf.IndustrialConveyor>();
		info.msg.industrialConveyor.filters = Facepunch.Pool.GetList<ProtoBuf.IndustrialConveyor.ItemFilter>();
		foreach (global::IndustrialConveyor.ItemFilter itemFilter in this.filterItems)
		{
			ProtoBuf.IndustrialConveyor.ItemFilter itemFilter2 = Facepunch.Pool.Get<ProtoBuf.IndustrialConveyor.ItemFilter>();
			itemFilter.CopyTo(itemFilter2);
			info.msg.industrialConveyor.filters.Add(itemFilter2);
		}
	}

	// Token: 0x06000C01 RID: 3073 RVA: 0x00067E38 File Offset: 0x00066038
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	private void RPC_ChangeFilters(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == null || !msg.player.CanBuild())
		{
			return;
		}
		this.filterItems.Clear();
		ProtoBuf.IndustrialConveyor.ItemFilterList itemFilterList = ProtoBuf.IndustrialConveyor.ItemFilterList.Deserialize(msg.read);
		if (itemFilterList.filters == null)
		{
			return;
		}
		int num = Mathf.Min(itemFilterList.filters.Count, 24);
		int num2 = 0;
		while (num2 < num && this.filterItems.Count < 12)
		{
			global::IndustrialConveyor.ItemFilter itemFilter = new global::IndustrialConveyor.ItemFilter(itemFilterList.filters[num2]);
			if (itemFilter.TargetItem != null || itemFilter.TargetCategory != null)
			{
				this.filterItems.Add(itemFilter);
			}
			num2++;
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000C02 RID: 3074 RVA: 0x00067EF1 File Offset: 0x000660F1
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(2UL)]
	private void SvSwitch(global::BaseEntity.RPCMessage msg)
	{
		this.SetSwitch(!base.IsOn());
	}

	// Token: 0x06000C03 RID: 3075 RVA: 0x00067F04 File Offset: 0x00066104
	public virtual void SetSwitch(bool wantsOn)
	{
		if (wantsOn == base.IsOn())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.On, wantsOn, false, true);
		base.SetFlag(global::BaseEntity.Flags.Busy, true, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved10, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved9, false, false, true);
		if (!wantsOn)
		{
			this.lastFilterState = null;
		}
		this.ensureOutputsUpdated = true;
		base.Invoke(new Action(this.Unbusy), 0.5f);
		for (int i = 0; i < this.filterItems.Count; i++)
		{
			global::IndustrialConveyor.ItemFilter itemFilter = this.filterItems[i];
			if (itemFilter.MinTransferRemaining > 0)
			{
				itemFilter.MinTransferRemaining = 0;
				this.filterItems[i] = itemFilter;
			}
		}
		base.SendNetworkUpdateImmediate(false);
		this.MarkDirty();
	}

	// Token: 0x06000C04 RID: 3076 RVA: 0x0005E510 File Offset: 0x0005C710
	public void Unbusy()
	{
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
	}

	// Token: 0x06000C05 RID: 3077 RVA: 0x000059DD File Offset: 0x00003BDD
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
	}

	// Token: 0x06000C06 RID: 3078 RVA: 0x00067FCC File Offset: 0x000661CC
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		if (inputSlot == 1)
		{
			bool flag = inputAmount >= this.ConsumptionAmount() && inputAmount > 0;
			base.SetFlag(global::BaseEntity.Flags.Reserved8, flag, false, true);
			if (!flag)
			{
				base.SetFlag(global::BaseEntity.Flags.Reserved9, false, false, true);
				base.SetFlag(global::BaseEntity.Flags.Reserved10, false, false, true);
			}
			this.currentEnergy = inputAmount;
			this.ensureOutputsUpdated = true;
			this.MarkDirty();
			if (inputAmount <= 0 && base.IsOn())
			{
				this.SetSwitch(false);
			}
		}
		if (inputSlot == 2 && !base.IsOn() && inputAmount > 0 && this.IsPowered())
		{
			this.SetSwitch(true);
		}
		if (inputSlot == 3 && base.IsOn() && inputAmount > 0)
		{
			this.SetSwitch(false);
		}
	}

	// Token: 0x06000C07 RID: 3079 RVA: 0x00068080 File Offset: 0x00066280
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot == 2)
		{
			if (!base.HasFlag(global::BaseEntity.Flags.Reserved10))
			{
				return 0;
			}
			return 1;
		}
		else if (outputSlot == 3)
		{
			if (!base.HasFlag(global::BaseEntity.Flags.Reserved9))
			{
				return 0;
			}
			return 1;
		}
		else
		{
			if (outputSlot == 1)
			{
				return this.GetCurrentEnergy();
			}
			return 0;
		}
	}

	// Token: 0x06000C08 RID: 3080 RVA: 0x00006C79 File Offset: 0x00004E79
	public override bool ShouldDrainBattery(global::IOEntity battery)
	{
		return base.IsOn();
	}

	// Token: 0x06000C09 RID: 3081 RVA: 0x000680B8 File Offset: 0x000662B8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		this.filterItems.Clear();
		ProtoBuf.IndustrialConveyor industrialConveyor = info.msg.industrialConveyor;
		if (((industrialConveyor != null) ? industrialConveyor.filters : null) != null)
		{
			foreach (ProtoBuf.IndustrialConveyor.ItemFilter from in info.msg.industrialConveyor.filters)
			{
				global::IndustrialConveyor.ItemFilter itemFilter = new global::IndustrialConveyor.ItemFilter(from);
				if (itemFilter.TargetItem != null || itemFilter.TargetCategory != null)
				{
					this.filterItems.Add(itemFilter);
				}
			}
		}
	}

	// Token: 0x06000C0B RID: 3083 RVA: 0x000681A0 File Offset: 0x000663A0
	[CompilerGenerated]
	internal static void <RunJob>g__AddTransfer|23_0(int itemId, int amount, global::BaseEntity fromEntity, global::BaseEntity toEntity, ref global::IndustrialConveyor.<>c__DisplayClass23_0 A_4)
	{
		if (A_4.transfer == null || A_4.transfer.ItemTransfers == null)
		{
			return;
		}
		if (fromEntity != null && !A_4.transfer.inputEntities.Contains(fromEntity.net.ID))
		{
			A_4.transfer.inputEntities.Add(fromEntity.net.ID);
		}
		if (toEntity != null && !A_4.transfer.outputEntities.Contains(toEntity.net.ID))
		{
			A_4.transfer.outputEntities.Add(toEntity.net.ID);
		}
		for (int i = 0; i < A_4.transfer.ItemTransfers.Count; i++)
		{
			IndustrialConveyorTransfer.ItemTransfer itemTransfer = A_4.transfer.ItemTransfers[i];
			if (itemTransfer.itemId == itemId)
			{
				itemTransfer.amount += amount;
				A_4.transfer.ItemTransfers[i] = itemTransfer;
				return;
			}
		}
		IndustrialConveyorTransfer.ItemTransfer item = new IndustrialConveyorTransfer.ItemTransfer
		{
			itemId = itemId,
			amount = amount
		};
		A_4.transfer.ItemTransfers.Add(item);
	}

	// Token: 0x040007A9 RID: 1961
	public int MaxStackSizePerMove = 128;

	// Token: 0x040007AA RID: 1962
	public GameObjectRef FilterDialog;

	// Token: 0x040007AB RID: 1963
	private List<global::IndustrialConveyor.ItemFilter> filterItems = new List<global::IndustrialConveyor.ItemFilter>();

	// Token: 0x040007AC RID: 1964
	private const float ScreenUpdateRange = 30f;

	// Token: 0x040007AD RID: 1965
	public const global::BaseEntity.Flags FilterPassFlag = global::BaseEntity.Flags.Reserved9;

	// Token: 0x040007AE RID: 1966
	public const global::BaseEntity.Flags FilterFailFlag = global::BaseEntity.Flags.Reserved10;

	// Token: 0x040007AF RID: 1967
	public SoundDefinition transferItemSoundDef;

	// Token: 0x040007B0 RID: 1968
	public SoundDefinition transferItemStartSoundDef;

	// Token: 0x040007B1 RID: 1969
	public const int MAX_FILTER_SIZE = 12;

	// Token: 0x040007B2 RID: 1970
	public Image IconTransferImage;

	// Token: 0x040007B3 RID: 1971
	private IIndustrialStorage workerOutput;

	// Token: 0x040007B4 RID: 1972
	private Func<IIndustrialStorage, int, bool> filterFunc;

	// Token: 0x040007B5 RID: 1973
	private List<global::IOEntity.ContainerInputOutput> splitOutputs = new List<global::IOEntity.ContainerInputOutput>();

	// Token: 0x040007B6 RID: 1974
	private List<global::IOEntity.ContainerInputOutput> splitInputs = new List<global::IOEntity.ContainerInputOutput>();

	// Token: 0x040007B7 RID: 1975
	private bool? lastFilterState;

	// Token: 0x02000B8C RID: 2956
	public struct ItemFilter
	{
		// Token: 0x06004AEE RID: 19182 RVA: 0x0019112C File Offset: 0x0018F32C
		public void CopyTo(ProtoBuf.IndustrialConveyor.ItemFilter target)
		{
			if (this.TargetItem != null)
			{
				target.itemDef = this.TargetItem.itemid;
			}
			target.maxAmountInDestination = this.MaxAmountInOutput;
			if (this.TargetCategory != null)
			{
				target.itemCategory = (int)this.TargetCategory.Value;
			}
			else
			{
				target.itemCategory = -1;
			}
			target.isBlueprint = (this.IsBlueprint ? 1 : 0);
			target.minAmountForMove = this.MinAmountToTransfer;
		}

		// Token: 0x06004AEF RID: 19183 RVA: 0x001911AC File Offset: 0x0018F3AC
		public ItemFilter(ProtoBuf.IndustrialConveyor.ItemFilter from)
		{
			this = new global::IndustrialConveyor.ItemFilter
			{
				TargetItem = ItemManager.FindItemDefinition(from.itemDef),
				MaxAmountInOutput = from.maxAmountInDestination
			};
			if (from.itemCategory >= 0)
			{
				this.TargetCategory = new ItemCategory?((ItemCategory)from.itemCategory);
			}
			else
			{
				this.TargetCategory = null;
			}
			this.IsBlueprint = (from.isBlueprint == 1);
			this.MinAmountToTransfer = from.minAmountForMove;
		}

		// Token: 0x04003EB0 RID: 16048
		public ItemDefinition TargetItem;

		// Token: 0x04003EB1 RID: 16049
		public ItemCategory? TargetCategory;

		// Token: 0x04003EB2 RID: 16050
		public int MaxAmountInOutput;

		// Token: 0x04003EB3 RID: 16051
		public int MinAmountToTransfer;

		// Token: 0x04003EB4 RID: 16052
		public bool IsBlueprint;

		// Token: 0x04003EB5 RID: 16053
		public int MinTransferRemaining;
	}
}
