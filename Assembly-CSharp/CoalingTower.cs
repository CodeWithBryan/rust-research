using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

// Token: 0x0200001B RID: 27
public class CoalingTower : global::IOEntity, INotifyEntityTrigger
{
	// Token: 0x06000071 RID: 113 RVA: 0x00004428 File Offset: 0x00002628
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.coalingTower = Facepunch.Pool.Get<ProtoBuf.CoalingTower>();
		info.msg.coalingTower.lootTypeIndex = this.LootTypeIndex;
		info.msg.coalingTower.oreStorageID = this.oreStorageInstance.uid;
		info.msg.coalingTower.fuelStorageID = this.fuelStorageInstance.uid;
		info.msg.coalingTower.activeUnloadableID = this.activeTrainCarRef.uid;
	}

	// Token: 0x06000072 RID: 114 RVA: 0x000044B8 File Offset: 0x000026B8
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, false);
		base.SetFlag(global::BaseEntity.Flags.Reserved1, false, false, false);
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, false);
		base.SetFlag(global::BaseEntity.Flags.Reserved3, false, false, false);
		base.SetFlag(global::BaseEntity.Flags.Reserved4, false, false, false);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000073 RID: 115 RVA: 0x00004518 File Offset: 0x00002718
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer)
		{
			if (child.prefabID == this.oreStoragePrefab.GetEntity().prefabID)
			{
				this.oreStorageInstance.Set((OreHopper)child);
				return;
			}
			if (child.prefabID == this.fuelStoragePrefab.GetEntity().prefabID)
			{
				this.fuelStorageInstance.Set((PercentFullStorageContainer)child);
			}
		}
	}

	// Token: 0x06000074 RID: 116 RVA: 0x00004587 File Offset: 0x00002787
	public void OnEmpty()
	{
		this.ClearActiveTrainCar();
	}

	// Token: 0x06000075 RID: 117 RVA: 0x00004590 File Offset: 0x00002790
	public void OnEntityEnter(global::BaseEntity ent)
	{
		if (!ent.IsValid())
		{
			return;
		}
		if (ent.isClient)
		{
			return;
		}
		TrainCar trainCar = ent as TrainCar;
		if (trainCar != null)
		{
			this.SetActiveTrainCar(trainCar);
		}
	}

	// Token: 0x06000076 RID: 118 RVA: 0x000045C8 File Offset: 0x000027C8
	public void OnEntityLeave(global::BaseEntity ent)
	{
		if (!ent.IsValid())
		{
			return;
		}
		if (ent.isClient)
		{
			return;
		}
		global::BaseEntity y = ent.parentEntity.Get(base.isServer);
		TrainCar x = this.activeTrainCarRef.Get(true);
		if (x == ent && x != y)
		{
			this.ClearActiveTrainCar();
		}
	}

	// Token: 0x06000077 RID: 119 RVA: 0x00004620 File Offset: 0x00002820
	private void SetActiveTrainCar(TrainCar trainCar)
	{
		if (this.GetActiveTrainCar() == trainCar)
		{
			return;
		}
		this.activeTrainCarRef.Set(trainCar);
		TrainCarUnloadable entity;
		if ((entity = (trainCar as TrainCarUnloadable)) != null)
		{
			this.activeUnloadableRef.Set(entity);
		}
		else
		{
			this.activeUnloadableRef.Set(null);
		}
		bool flag = this.activeUnloadableRef.IsValid(true);
		this.CheckWagonLinedUp(false);
		if (flag)
		{
			base.InvokeRandomized(new Action(this.CheckWagonLinedUp), 0.15f, 0.15f, 0.015f);
		}
		else
		{
			base.CancelInvoke(new Action(this.CheckWagonLinedUp));
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000078 RID: 120 RVA: 0x000046BD File Offset: 0x000028BD
	private void ClearActiveTrainCar()
	{
		this.SetActiveTrainCar(null);
	}

	// Token: 0x06000079 RID: 121 RVA: 0x000046C6 File Offset: 0x000028C6
	private void CheckWagonLinedUp()
	{
		this.CheckWagonLinedUp(true);
	}

	// Token: 0x0600007A RID: 122 RVA: 0x000046D0 File Offset: 0x000028D0
	private void CheckWagonLinedUp(bool networkUpdate)
	{
		bool b = false;
		TrainCarUnloadable activeUnloadable = this.GetActiveUnloadable();
		if (activeUnloadable != null)
		{
			b = activeUnloadable.IsLinedUpToUnload(this.unloadingBounds);
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved2, b, false, networkUpdate);
	}

	// Token: 0x0600007B RID: 123 RVA: 0x0000470C File Offset: 0x0000290C
	private bool TryUnloadActiveWagon(out global::CoalingTower.ActionAttemptStatus attemptStatus)
	{
		TrainCarUnloadable activeUnloadable = this.GetActiveUnloadable();
		if (activeUnloadable == null)
		{
			attemptStatus = global::CoalingTower.ActionAttemptStatus.NoTrainCar;
			return false;
		}
		TrainCarUnloadable.WagonType wagonType = activeUnloadable.wagonType;
		if (!this.CanUnloadNow(out attemptStatus))
		{
			return false;
		}
		base.SetFlag(global::BaseEntity.Flags.Busy, true, false, true);
		base.Invoke(new Action(this.WagonBeginUnloadAnim), this.vacuumStartDelay);
		return true;
	}

	// Token: 0x0600007C RID: 124 RVA: 0x00004768 File Offset: 0x00002968
	private void WagonBeginUnloadAnim()
	{
		TrainCarUnloadable activeUnloadable = this.GetActiveUnloadable();
		if (activeUnloadable == null)
		{
			base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
			return;
		}
		TrainWagonLootData.LootOption lootOption;
		if (!activeUnloadable.TryGetLootType(out lootOption))
		{
			base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
			return;
		}
		int value;
		TrainWagonLootData.instance.TryGetIndexFromLoot(lootOption, out value);
		this.LootTypeIndex.Value = value;
		this.tcUnloadingNow = activeUnloadable;
		this.tcUnloadingNow.BeginUnloadAnimation();
		float repeat = 4f;
		base.InvokeRepeating(new Action(this.EmptyTenPercent), 0f, repeat);
	}

	// Token: 0x0600007D RID: 125 RVA: 0x000047F8 File Offset: 0x000029F8
	private void EmptyTenPercent()
	{
		if (!this.IsPowered())
		{
			this.EndEmptyProcess(global::CoalingTower.ActionAttemptStatus.GenericError);
			return;
		}
		if (!this.HasUnloadableLinedUp)
		{
			this.EndEmptyProcess(global::CoalingTower.ActionAttemptStatus.NoTrainCar);
			return;
		}
		TrainCarUnloadable activeUnloadable = this.GetActiveUnloadable();
		if (this.tcUnloadingNow == null || activeUnloadable != this.tcUnloadingNow)
		{
			this.EndEmptyProcess(global::CoalingTower.ActionAttemptStatus.NoTrainCar);
			return;
		}
		StorageContainer storageContainer = this.tcUnloadingNow.GetStorageContainer();
		TrainWagonLootData.LootOption lootOption;
		if (storageContainer.inventory == null || !TrainWagonLootData.instance.TryGetLootFromIndex(this.LootTypeIndex, out lootOption))
		{
			this.EndEmptyProcess(global::CoalingTower.ActionAttemptStatus.NoTrainCar);
			return;
		}
		bool flag = this.tcUnloadingNow.wagonType != TrainCarUnloadable.WagonType.Fuel;
		global::ItemContainer itemContainer = null;
		PercentFullStorageContainer percentFullStorageContainer = flag ? this.GetOreStorage() : this.GetFuelStorage();
		if (percentFullStorageContainer != null)
		{
			itemContainer = percentFullStorageContainer.inventory;
		}
		if (itemContainer == null)
		{
			this.EndEmptyProcess(global::CoalingTower.ActionAttemptStatus.GenericError);
			return;
		}
		global::ItemContainer inventory = storageContainer.inventory;
		global::ItemContainer newcontainer = itemContainer;
		int iAmount = Mathf.RoundToInt((float)lootOption.maxLootAmount / 10f);
		List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
		int num = inventory.Take(list, lootOption.lootItem.itemid, iAmount);
		bool flag2 = true;
		if (num > 0)
		{
			foreach (global::Item item in list)
			{
				if (this.tcUnloadingNow.wagonType == TrainCarUnloadable.WagonType.Lootboxes)
				{
					item.Remove(0f);
				}
				else
				{
					bool flag3 = item.MoveToContainer(newcontainer, -1, true, false, null, true);
					if (flag2 && !flag3)
					{
						item.MoveToContainer(inventory, -1, true, false, null, true);
						flag2 = false;
						break;
					}
				}
			}
		}
		Facepunch.Pool.FreeList<global::Item>(ref list);
		float orePercent = this.tcUnloadingNow.GetOrePercent();
		if (orePercent == 0f)
		{
			this.EndEmptyProcess(global::CoalingTower.ActionAttemptStatus.NoError);
			return;
		}
		if (!flag2)
		{
			this.EndEmptyProcess(global::CoalingTower.ActionAttemptStatus.OutputIsFull);
			return;
		}
		if (flag)
		{
			this.tcUnloadingNow.SetVisualOreLevel(orePercent);
		}
	}

	// Token: 0x0600007E RID: 126 RVA: 0x000049D8 File Offset: 0x00002BD8
	private void EndEmptyProcess(global::CoalingTower.ActionAttemptStatus status)
	{
		base.CancelInvoke(new Action(this.EmptyTenPercent));
		base.CancelInvoke(new Action(this.WagonBeginUnloadAnim));
		if (this.tcUnloadingNow != null)
		{
			this.tcUnloadingNow.EndEmptyProcess();
			this.tcUnloadingNow = null;
		}
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, false);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		if (status != global::CoalingTower.ActionAttemptStatus.NoError)
		{
			base.ClientRPC<byte, bool>(null, "ActionFailed", (byte)status, false);
		}
	}

	// Token: 0x0600007F RID: 127 RVA: 0x00004A50 File Offset: 0x00002C50
	private bool TryShuntTrain(bool next, out global::CoalingTower.ActionAttemptStatus attemptStatus)
	{
		if (!this.IsPowered() || base.HasFlag(global::BaseEntity.Flags.Reserved3) || base.HasFlag(global::BaseEntity.Flags.Reserved4))
		{
			attemptStatus = global::CoalingTower.ActionAttemptStatus.GenericError;
			return false;
		}
		TrainCar activeTrainCar = this.GetActiveTrainCar();
		if (activeTrainCar == null)
		{
			attemptStatus = global::CoalingTower.ActionAttemptStatus.NoTrainCar;
			return false;
		}
		Vector3 unloadingPos = this.UnloadingPos;
		unloadingPos.y = 0f;
		TrainCar trainCar;
		if (activeTrainCar is TrainCarUnloadable && !this.HasUnloadableLinedUp)
		{
			Vector3 position = activeTrainCar.transform.position;
			Vector3 rhs = unloadingPos - position;
			if (Vector3.Dot(base.transform.forward, rhs) >= 0f == next)
			{
				trainCar = activeTrainCar;
				goto IL_BA;
			}
		}
		if (!activeTrainCar.TryGetTrainCar(next, base.transform.forward, out trainCar))
		{
			attemptStatus = (next ? global::CoalingTower.ActionAttemptStatus.NoNextTrainCar : global::CoalingTower.ActionAttemptStatus.NoPrevTrainCar);
			return false;
		}
		IL_BA:
		Vector3 position2 = trainCar.transform.position;
		position2.y = 0f;
		Vector3 shuntDirection = unloadingPos - position2;
		float magnitude = shuntDirection.magnitude;
		return activeTrainCar.completeTrain.TryShuntCarTo(shuntDirection, magnitude, trainCar, new Action<global::CoalingTower.ActionAttemptStatus>(this.ShuntEnded), out attemptStatus);
	}

	// Token: 0x06000080 RID: 128 RVA: 0x00004B5E File Offset: 0x00002D5E
	private void ShuntEnded(global::CoalingTower.ActionAttemptStatus status)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved3, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved4, false, false, true);
		if (status != global::CoalingTower.ActionAttemptStatus.NoError)
		{
			base.ClientRPC(null, "IssueDuringShunt");
		}
	}

	// Token: 0x06000081 RID: 129 RVA: 0x00004B8C File Offset: 0x00002D8C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Unload(global::BaseEntity.RPCMessage msg)
	{
		global::CoalingTower.ActionAttemptStatus actionAttemptStatus;
		if (!this.TryUnloadActiveWagon(out actionAttemptStatus) && msg.player != null)
		{
			base.ClientRPCPlayer<byte, bool>(null, msg.player, "ActionFailed", (byte)actionAttemptStatus, true);
		}
	}

	// Token: 0x06000082 RID: 130 RVA: 0x00004BC8 File Offset: 0x00002DC8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Next(global::BaseEntity.RPCMessage msg)
	{
		global::CoalingTower.ActionAttemptStatus actionAttemptStatus;
		if (this.TryShuntTrain(true, out actionAttemptStatus))
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved3, true, false, true);
			return;
		}
		if (msg.player != null)
		{
			base.ClientRPCPlayer<byte, bool>(null, msg.player, "ActionFailed", (byte)actionAttemptStatus, true);
		}
	}

	// Token: 0x06000083 RID: 131 RVA: 0x00004C14 File Offset: 0x00002E14
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Prev(global::BaseEntity.RPCMessage msg)
	{
		global::CoalingTower.ActionAttemptStatus actionAttemptStatus;
		if (this.TryShuntTrain(false, out actionAttemptStatus))
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved4, true, false, true);
			return;
		}
		if (msg.player != null)
		{
			base.ClientRPCPlayer<byte, bool>(null, msg.player, "ActionFailed", (byte)actionAttemptStatus, true);
		}
	}

	// Token: 0x17000013 RID: 19
	// (get) Token: 0x06000084 RID: 132 RVA: 0x00004C5E File Offset: 0x00002E5E
	private bool HasTrainCar
	{
		get
		{
			return this.activeTrainCarRef.IsValid(base.isServer);
		}
	}

	// Token: 0x17000014 RID: 20
	// (get) Token: 0x06000085 RID: 133 RVA: 0x00004C71 File Offset: 0x00002E71
	private bool HasUnloadable
	{
		get
		{
			return this.activeUnloadableRef.IsValid(base.isServer);
		}
	}

	// Token: 0x17000015 RID: 21
	// (get) Token: 0x06000086 RID: 134 RVA: 0x00004C84 File Offset: 0x00002E84
	private bool HasUnloadableLinedUp
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved2);
		}
	}

	// Token: 0x17000016 RID: 22
	// (get) Token: 0x06000087 RID: 135 RVA: 0x00004C91 File Offset: 0x00002E91
	// (set) Token: 0x06000088 RID: 136 RVA: 0x00004C99 File Offset: 0x00002E99
	public Vector3 UnloadingPos { get; private set; }

	// Token: 0x06000089 RID: 137 RVA: 0x00004CA4 File Offset: 0x00002EA4
	public override void InitShared()
	{
		base.InitShared();
		this.LootTypeIndex = new NetworkedProperty<int>(this);
		this.UnloadingPos = this.unloadingBounds.transform.position + this.unloadingBounds.transform.rotation * this.unloadingBounds.center;
		global::CoalingTower.unloadersInWorld.Add(this);
	}

	// Token: 0x0600008A RID: 138 RVA: 0x00004D09 File Offset: 0x00002F09
	public override void DestroyShared()
	{
		base.DestroyShared();
		global::CoalingTower.unloadersInWorld.Remove(this);
	}

	// Token: 0x0600008B RID: 139 RVA: 0x00004D20 File Offset: 0x00002F20
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.coalingTower != null)
		{
			this.LootTypeIndex.Value = info.msg.coalingTower.lootTypeIndex;
			this.oreStorageInstance.uid = info.msg.coalingTower.oreStorageID;
			this.fuelStorageInstance.uid = info.msg.coalingTower.fuelStorageID;
		}
	}

	// Token: 0x0600008C RID: 140 RVA: 0x00004D94 File Offset: 0x00002F94
	public static bool IsUnderAnUnloader(TrainCar trainCar, out bool isLinedUp, out Vector3 unloaderPos)
	{
		foreach (global::CoalingTower coalingTower in global::CoalingTower.unloadersInWorld)
		{
			if (coalingTower.TrainCarIsUnder(trainCar, out isLinedUp))
			{
				unloaderPos = coalingTower.UnloadingPos;
				return true;
			}
		}
		isLinedUp = false;
		unloaderPos = Vector3.zero;
		return false;
	}

	// Token: 0x0600008D RID: 141 RVA: 0x00004E0C File Offset: 0x0000300C
	public bool TrainCarIsUnder(TrainCar trainCar, out bool isLinedUp)
	{
		isLinedUp = false;
		if (!trainCar.IsValid())
		{
			return false;
		}
		TrainCarUnloadable activeUnloadable = this.GetActiveUnloadable();
		if (activeUnloadable != null && activeUnloadable.EqualNetID(trainCar))
		{
			isLinedUp = this.HasUnloadableLinedUp;
			return true;
		}
		return false;
	}

	// Token: 0x0600008E RID: 142 RVA: 0x00004E4C File Offset: 0x0000304C
	private OreHopper GetOreStorage()
	{
		OreHopper oreHopper = this.oreStorageInstance.Get(base.isServer);
		if (oreHopper.IsValid())
		{
			return oreHopper;
		}
		return null;
	}

	// Token: 0x0600008F RID: 143 RVA: 0x00004E78 File Offset: 0x00003078
	private PercentFullStorageContainer GetFuelStorage()
	{
		PercentFullStorageContainer percentFullStorageContainer = this.fuelStorageInstance.Get(base.isServer);
		if (percentFullStorageContainer.IsValid())
		{
			return percentFullStorageContainer;
		}
		return null;
	}

	// Token: 0x06000090 RID: 144 RVA: 0x00004EA4 File Offset: 0x000030A4
	private TrainCar GetActiveTrainCar()
	{
		TrainCar trainCar = this.activeTrainCarRef.Get(base.isServer);
		if (trainCar.IsValid())
		{
			return trainCar;
		}
		return null;
	}

	// Token: 0x06000091 RID: 145 RVA: 0x00004ED0 File Offset: 0x000030D0
	private TrainCarUnloadable GetActiveUnloadable()
	{
		TrainCarUnloadable trainCarUnloadable = this.activeUnloadableRef.Get(base.isServer);
		if (trainCarUnloadable.IsValid())
		{
			return trainCarUnloadable;
		}
		return null;
	}

	// Token: 0x06000092 RID: 146 RVA: 0x00004EFC File Offset: 0x000030FC
	private bool OutputBinIsFull()
	{
		TrainCarUnloadable activeUnloadable = this.GetActiveUnloadable();
		if (activeUnloadable == null)
		{
			return false;
		}
		TrainCarUnloadable.WagonType wagonType = activeUnloadable.wagonType;
		if (wagonType == TrainCarUnloadable.WagonType.Lootboxes)
		{
			return false;
		}
		if (wagonType != TrainCarUnloadable.WagonType.Fuel)
		{
			OreHopper oreStorage = this.GetOreStorage();
			return oreStorage != null && oreStorage.IsFull();
		}
		PercentFullStorageContainer fuelStorage = this.GetFuelStorage();
		return fuelStorage != null && fuelStorage.IsFull();
	}

	// Token: 0x06000093 RID: 147 RVA: 0x00004F60 File Offset: 0x00003160
	private bool WagonIsEmpty()
	{
		TrainCarUnloadable activeUnloadable = this.GetActiveUnloadable();
		return !(activeUnloadable != null) || activeUnloadable.GetOrePercent() == 0f;
	}

	// Token: 0x06000094 RID: 148 RVA: 0x00004F8C File Offset: 0x0000318C
	private bool CanUnloadNow(out global::CoalingTower.ActionAttemptStatus attemptStatus)
	{
		if (!this.HasUnloadableLinedUp)
		{
			attemptStatus = global::CoalingTower.ActionAttemptStatus.NoTrainCar;
			return false;
		}
		if (this.OutputBinIsFull())
		{
			attemptStatus = global::CoalingTower.ActionAttemptStatus.OutputIsFull;
			return false;
		}
		attemptStatus = global::CoalingTower.ActionAttemptStatus.NoError;
		return this.IsPowered();
	}

	// Token: 0x06000095 RID: 149 RVA: 0x00004FB4 File Offset: 0x000031B4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CoalingTower.OnRpcMessage", 0))
		{
			if (rpc == 3071873383U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Next ");
				}
				using (TimeWarning.New("RPC_Next", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3071873383U, "RPC_Next", this, player, 3f))
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
							this.RPC_Next(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Next");
					}
				}
				return true;
			}
			if (rpc == 3656312045U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Prev ");
				}
				using (TimeWarning.New("RPC_Prev", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3656312045U, "RPC_Prev", this, player, 3f))
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
							this.RPC_Prev(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_Prev");
					}
				}
				return true;
			}
			if (rpc == 998476828U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Unload ");
				}
				using (TimeWarning.New("RPC_Unload", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(998476828U, "RPC_Unload", this, player, 3f))
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
							this.RPC_Unload(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in RPC_Unload");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x04000070 RID: 112
	private TrainCarUnloadable tcUnloadingNow;

	// Token: 0x04000071 RID: 113
	[Header("Coaling Tower")]
	[SerializeField]
	private BoxCollider unloadingBounds;

	// Token: 0x04000072 RID: 114
	[SerializeField]
	private GameObjectRef oreStoragePrefab;

	// Token: 0x04000073 RID: 115
	[SerializeField]
	private GameObjectRef fuelStoragePrefab;

	// Token: 0x04000074 RID: 116
	[SerializeField]
	private MeshRenderer[] signalLightsExterior;

	// Token: 0x04000075 RID: 117
	[SerializeField]
	private MeshRenderer[] signalLightsInterior;

	// Token: 0x04000076 RID: 118
	[ColorUsage(false, true)]
	public Color greenLightOnColour;

	// Token: 0x04000077 RID: 119
	[ColorUsage(false, true)]
	public Color yellowLightOnColour;

	// Token: 0x04000078 RID: 120
	[SerializeField]
	private Animator vacuumAnimator;

	// Token: 0x04000079 RID: 121
	[SerializeField]
	private float vacuumStartDelay = 2f;

	// Token: 0x0400007A RID: 122
	[FormerlySerializedAs("unloadingFXContainer")]
	[SerializeField]
	private ParticleSystemContainer unloadingFXContainerOre;

	// Token: 0x0400007B RID: 123
	[SerializeField]
	private ParticleSystem[] unloadingFXMain;

	// Token: 0x0400007C RID: 124
	[SerializeField]
	private ParticleSystem[] unloadingFXDust;

	// Token: 0x0400007D RID: 125
	[SerializeField]
	private ParticleSystemContainer unloadingFXContainerFuel;

	// Token: 0x0400007E RID: 126
	[Header("Coaling Tower Text")]
	[SerializeField]
	private TokenisedPhrase noTraincar;

	// Token: 0x0400007F RID: 127
	[SerializeField]
	private TokenisedPhrase noNextTraincar;

	// Token: 0x04000080 RID: 128
	[SerializeField]
	private TokenisedPhrase noPrevTraincar;

	// Token: 0x04000081 RID: 129
	[SerializeField]
	private TokenisedPhrase trainIsMoving;

	// Token: 0x04000082 RID: 130
	[SerializeField]
	private TokenisedPhrase outputIsFull;

	// Token: 0x04000083 RID: 131
	[SerializeField]
	private TokenisedPhrase trainHasThrottle;

	// Token: 0x04000084 RID: 132
	[Header("Coaling Tower Audio")]
	[SerializeField]
	private GameObject buttonSoundPos;

	// Token: 0x04000085 RID: 133
	[SerializeField]
	private SoundDefinition buttonPressSound;

	// Token: 0x04000086 RID: 134
	[SerializeField]
	private SoundDefinition buttonReleaseSound;

	// Token: 0x04000087 RID: 135
	[SerializeField]
	private SoundDefinition failedActionSound;

	// Token: 0x04000088 RID: 136
	[SerializeField]
	private SoundDefinition failedShuntAlarmSound;

	// Token: 0x04000089 RID: 137
	[SerializeField]
	private SoundDefinition armMovementLower;

	// Token: 0x0400008A RID: 138
	[SerializeField]
	private SoundDefinition armMovementRaise;

	// Token: 0x0400008B RID: 139
	[SerializeField]
	private SoundDefinition suctionAirStart;

	// Token: 0x0400008C RID: 140
	[SerializeField]
	private SoundDefinition suctionAirStop;

	// Token: 0x0400008D RID: 141
	[SerializeField]
	private SoundDefinition suctionAirLoop;

	// Token: 0x0400008E RID: 142
	[SerializeField]
	private SoundDefinition suctionOreStart;

	// Token: 0x0400008F RID: 143
	[SerializeField]
	private SoundDefinition suctionOreLoop;

	// Token: 0x04000090 RID: 144
	[SerializeField]
	private SoundDefinition suctionOreStop;

	// Token: 0x04000091 RID: 145
	[SerializeField]
	private SoundDefinition suctionOreInteriorLoop;

	// Token: 0x04000092 RID: 146
	[SerializeField]
	private SoundDefinition oreBinLoop;

	// Token: 0x04000093 RID: 147
	[SerializeField]
	private SoundDefinition suctionFluidStart;

	// Token: 0x04000094 RID: 148
	[SerializeField]
	private SoundDefinition suctionFluidLoop;

	// Token: 0x04000095 RID: 149
	[SerializeField]
	private SoundDefinition suctionFluidStop;

	// Token: 0x04000096 RID: 150
	[SerializeField]
	private SoundDefinition suctionFluidInteriorLoop;

	// Token: 0x04000097 RID: 151
	[SerializeField]
	private SoundDefinition fluidTankLoop;

	// Token: 0x04000098 RID: 152
	[SerializeField]
	private GameObject interiorPipeSoundLocation;

	// Token: 0x04000099 RID: 153
	[SerializeField]
	private GameObject armMovementSoundLocation;

	// Token: 0x0400009A RID: 154
	[SerializeField]
	private GameObject armSuctionSoundLocation;

	// Token: 0x0400009B RID: 155
	[SerializeField]
	private GameObject oreBinSoundLocation;

	// Token: 0x0400009C RID: 156
	[SerializeField]
	private GameObject fluidTankSoundLocation;

	// Token: 0x0400009D RID: 157
	private NetworkedProperty<int> LootTypeIndex;

	// Token: 0x0400009E RID: 158
	private EntityRef<TrainCar> activeTrainCarRef;

	// Token: 0x0400009F RID: 159
	private EntityRef<TrainCarUnloadable> activeUnloadableRef;

	// Token: 0x040000A0 RID: 160
	private const global::BaseEntity.Flags LinedUpFlag = global::BaseEntity.Flags.Reserved2;

	// Token: 0x040000A1 RID: 161
	private const global::BaseEntity.Flags HasUnloadableFlag = global::BaseEntity.Flags.Reserved1;

	// Token: 0x040000A2 RID: 162
	private const global::BaseEntity.Flags UnloadingInProgressFlag = global::BaseEntity.Flags.Busy;

	// Token: 0x040000A3 RID: 163
	private const global::BaseEntity.Flags MoveToNextInProgressFlag = global::BaseEntity.Flags.Reserved3;

	// Token: 0x040000A4 RID: 164
	private const global::BaseEntity.Flags MoveToPrevInProgressFlag = global::BaseEntity.Flags.Reserved4;

	// Token: 0x040000A5 RID: 165
	private EntityRef<OreHopper> oreStorageInstance;

	// Token: 0x040000A6 RID: 166
	private EntityRef<PercentFullStorageContainer> fuelStorageInstance;

	// Token: 0x040000A7 RID: 167
	public const float TIME_TO_EMPTY = 40f;

	// Token: 0x040000A9 RID: 169
	private static List<global::CoalingTower> unloadersInWorld = new List<global::CoalingTower>();

	// Token: 0x040000AA RID: 170
	private Sound armMovementLoopSound;

	// Token: 0x040000AB RID: 171
	private Sound suctionAirLoopSound;

	// Token: 0x040000AC RID: 172
	private Sound suctionMaterialLoopSound;

	// Token: 0x040000AD RID: 173
	private Sound interiorPipeLoopSound;

	// Token: 0x040000AE RID: 174
	private Sound unloadDestinationSound;

	// Token: 0x02000B0A RID: 2826
	public enum ActionAttemptStatus
	{
		// Token: 0x04003C72 RID: 15474
		NoError,
		// Token: 0x04003C73 RID: 15475
		GenericError,
		// Token: 0x04003C74 RID: 15476
		NoTrainCar,
		// Token: 0x04003C75 RID: 15477
		NoNextTrainCar,
		// Token: 0x04003C76 RID: 15478
		NoPrevTrainCar,
		// Token: 0x04003C77 RID: 15479
		TrainIsMoving,
		// Token: 0x04003C78 RID: 15480
		OutputIsFull,
		// Token: 0x04003C79 RID: 15481
		AlreadyShunting,
		// Token: 0x04003C7A RID: 15482
		TrainHasThrottle
	}
}
