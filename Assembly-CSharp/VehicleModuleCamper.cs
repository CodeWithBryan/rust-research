using System;
using System.Collections.Generic;
using System.Text;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.Modular;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000E1 RID: 225
public class VehicleModuleCamper : VehicleModuleSeating
{
	// Token: 0x06001397 RID: 5015 RVA: 0x0009A660 File Offset: 0x00098860
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("VehicleModuleCamper.OnRpcMessage", 0))
		{
			if (rpc == 2501069650U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenLocker ");
				}
				using (TimeWarning.New("RPC_OpenLocker", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2501069650U, "RPC_OpenLocker", this, player, 3f))
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
							this.RPC_OpenLocker(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_OpenLocker");
					}
				}
				return true;
			}
			if (rpc == 4185921214U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenStorage ");
				}
				using (TimeWarning.New("RPC_OpenStorage", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(4185921214U, "RPC_OpenStorage", this, player, 3f))
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
							this.RPC_OpenStorage(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_OpenStorage");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001398 RID: 5016 RVA: 0x0009A960 File Offset: 0x00098B60
	public override void ResetState()
	{
		base.ResetState();
		this.activeBbq.Set(null);
		this.activeLocker.Set(null);
		this.activeStorage.Set(null);
		this.wasLoaded = false;
	}

	// Token: 0x06001399 RID: 5017 RVA: 0x0009A994 File Offset: 0x00098B94
	public override void ModuleAdded(BaseModularVehicle vehicle, int firstSocketIndex)
	{
		base.ModuleAdded(vehicle, firstSocketIndex);
		if (base.isServer)
		{
			if (!Rust.Application.isLoadingSave && !this.wasLoaded)
			{
				for (int i = 0; i < this.SleepingBagPoints.Length; i++)
				{
					global::SleepingBagCamper sleepingBagCamper = base.gameManager.CreateEntity(this.SleepingBagEntity.resourcePath, this.SleepingBagPoints[i].localPosition, this.SleepingBagPoints[i].localRotation, true) as global::SleepingBagCamper;
					if (sleepingBagCamper != null)
					{
						sleepingBagCamper.SetParent(this, false, false);
						sleepingBagCamper.SetSeat(base.GetSeatAtIndex(i), false);
						sleepingBagCamper.Spawn();
					}
				}
				this.PostConditionalRefresh();
				return;
			}
			int num = 0;
			foreach (global::BaseEntity baseEntity in this.children)
			{
				global::SleepingBagCamper sleepingBagCamper2;
				IItemContainerEntity itemContainerEntity;
				if ((sleepingBagCamper2 = (baseEntity as global::SleepingBagCamper)) != null)
				{
					sleepingBagCamper2.SetSeat(base.GetSeatAtIndex(num++), true);
				}
				else if ((itemContainerEntity = (baseEntity as IItemContainerEntity)) != null)
				{
					global::ItemContainer inventory = itemContainerEntity.inventory;
					inventory.onItemAddedRemoved = (Action<global::Item, bool>)Delegate.Combine(inventory.onItemAddedRemoved, new Action<global::Item, bool>(this.OnItemAddedRemoved));
				}
			}
		}
	}

	// Token: 0x0600139A RID: 5018 RVA: 0x0009AAD4 File Offset: 0x00098CD4
	protected override Vector3 ModifySeatPositionLocalSpace(int index, Vector3 desiredPos)
	{
		CamperSeatConfig seatConfig = this.GetSeatConfig();
		if (seatConfig != null && seatConfig.SeatPositions.Length > index)
		{
			return seatConfig.SeatPositions[index].localPosition;
		}
		return base.ModifySeatPositionLocalSpace(index, desiredPos);
	}

	// Token: 0x0600139B RID: 5019 RVA: 0x0009AB12 File Offset: 0x00098D12
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.wasLoaded = true;
	}

	// Token: 0x0600139C RID: 5020 RVA: 0x0009AB24 File Offset: 0x00098D24
	public override void Spawn()
	{
		base.Spawn();
		if (!Rust.Application.isLoadingSave)
		{
			Locker locker = base.gameManager.CreateEntity(this.LockerEntity.resourcePath, this.LockerPoint.localPosition, this.LockerPoint.localRotation, true) as Locker;
			locker.SetParent(this, false, false);
			locker.Spawn();
			global::ItemContainer inventory = locker.inventory;
			inventory.onItemAddedRemoved = (Action<global::Item, bool>)Delegate.Combine(inventory.onItemAddedRemoved, new Action<global::Item, bool>(this.OnItemAddedRemoved));
			this.activeLocker.Set(locker);
			global::BaseOven baseOven = base.gameManager.CreateEntity(this.BbqEntity.resourcePath, this.BbqPoint.localPosition, this.BbqPoint.localRotation, true) as global::BaseOven;
			baseOven.SetParent(this, false, false);
			baseOven.Spawn();
			global::ItemContainer inventory2 = baseOven.inventory;
			inventory2.onItemAddedRemoved = (Action<global::Item, bool>)Delegate.Combine(inventory2.onItemAddedRemoved, new Action<global::Item, bool>(this.OnItemAddedRemoved));
			this.activeBbq.Set(baseOven);
			StorageContainer storageContainer = base.gameManager.CreateEntity(this.StorageEntity.resourcePath, this.StoragePoint.localPosition, this.StoragePoint.localRotation, true) as StorageContainer;
			storageContainer.SetParent(this, false, false);
			storageContainer.Spawn();
			global::ItemContainer inventory3 = storageContainer.inventory;
			inventory3.onItemAddedRemoved = (Action<global::Item, bool>)Delegate.Combine(inventory3.onItemAddedRemoved, new Action<global::Item, bool>(this.OnItemAddedRemoved));
			this.activeStorage.Set(storageContainer);
			this.PostConditionalRefresh();
		}
	}

	// Token: 0x0600139D RID: 5021 RVA: 0x0009ACA6 File Offset: 0x00098EA6
	private void OnItemAddedRemoved(global::Item item, bool add)
	{
		global::Item associatedItemInstance = this.AssociatedItemInstance;
		if (associatedItemInstance == null)
		{
			return;
		}
		associatedItemInstance.LockUnlock(!this.CanBeMovedNowOnVehicle());
	}

	// Token: 0x0600139E RID: 5022 RVA: 0x0009ACC4 File Offset: 0x00098EC4
	protected override bool CanBeMovedNowOnVehicle()
	{
		using (List<global::BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				IItemContainerEntity itemContainerEntity;
				if ((itemContainerEntity = (enumerator.Current as IItemContainerEntity)) != null && !itemContainerEntity.IsUnityNull<IItemContainerEntity>() && !itemContainerEntity.inventory.IsEmpty())
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x0600139F RID: 5023 RVA: 0x0009AD34 File Offset: 0x00098F34
	protected override void PostConditionalRefresh()
	{
		base.PostConditionalRefresh();
		if (base.isClient)
		{
			return;
		}
		CamperSeatConfig seatConfig = this.GetSeatConfig();
		if (seatConfig != null && this.mountPoints != null)
		{
			for (int i = 0; i < this.mountPoints.Count; i++)
			{
				if (this.mountPoints[i].mountable != null)
				{
					this.mountPoints[i].mountable.transform.position = seatConfig.SeatPositions[i].position;
					this.mountPoints[i].mountable.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				}
			}
		}
		if (this.activeBbq.IsValid(base.isServer) && seatConfig != null)
		{
			global::BaseOven baseOven = this.activeBbq.Get(true);
			baseOven.transform.position = seatConfig.StovePosition.position;
			baseOven.transform.rotation = seatConfig.StovePosition.rotation;
			baseOven.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
		if (this.activeStorage.IsValid(base.isServer) && seatConfig != null)
		{
			StorageContainer storageContainer = this.activeStorage.Get(base.isServer);
			storageContainer.transform.position = seatConfig.StoragePosition.position;
			storageContainer.transform.rotation = seatConfig.StoragePosition.rotation;
			storageContainer.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x060013A0 RID: 5024 RVA: 0x0009AE90 File Offset: 0x00099090
	private CamperSeatConfig GetSeatConfig()
	{
		List<ConditionalObject> conditionals = base.GetConditionals();
		CamperSeatConfig result = null;
		foreach (ConditionalObject conditionalObject in conditionals)
		{
			CamperSeatConfig camperSeatConfig;
			if (conditionalObject.gameObject.activeSelf && conditionalObject.gameObject.TryGetComponent<CamperSeatConfig>(out camperSeatConfig))
			{
				result = camperSeatConfig;
			}
		}
		return result;
	}

	// Token: 0x060013A1 RID: 5025 RVA: 0x0009AF00 File Offset: 0x00099100
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.camperModule == null)
		{
			info.msg.camperModule = Facepunch.Pool.Get<CamperModule>();
		}
		info.msg.camperModule.bbqId = this.activeBbq.uid;
		info.msg.camperModule.lockerId = this.activeLocker.uid;
		info.msg.camperModule.storageID = this.activeStorage.uid;
	}

	// Token: 0x060013A2 RID: 5026 RVA: 0x0009AF84 File Offset: 0x00099184
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_OpenLocker(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (!this.CanBeLooted(player))
		{
			return;
		}
		IItemContainerEntity itemContainerEntity = this.activeLocker.Get(base.isServer);
		if (!itemContainerEntity.IsUnityNull<IItemContainerEntity>())
		{
			itemContainerEntity.PlayerOpenLoot(player, "", true);
			return;
		}
		Debug.LogError(base.GetType().Name + ": No container component found.");
	}

	// Token: 0x060013A3 RID: 5027 RVA: 0x0009AFF0 File Offset: 0x000991F0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_OpenStorage(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (!this.CanBeLooted(player))
		{
			return;
		}
		IItemContainerEntity itemContainerEntity = this.activeStorage.Get(base.isServer);
		if (!itemContainerEntity.IsUnityNull<IItemContainerEntity>())
		{
			itemContainerEntity.PlayerOpenLoot(player, "", true);
			return;
		}
		Debug.LogError(base.GetType().Name + ": No container component found.");
	}

	// Token: 0x060013A4 RID: 5028 RVA: 0x0009B05C File Offset: 0x0009925C
	internal override void DoServerDestroy()
	{
		if (vehicle.vehiclesdroploot)
		{
			if (this.activeStorage.IsValid(base.isServer))
			{
				this.activeStorage.Get(base.isServer).DropItems(null);
			}
			if (this.activeBbq.IsValid(base.isServer))
			{
				this.activeBbq.Get(base.isServer).DropItems(null);
			}
			if (this.activeLocker.IsValid(base.isServer))
			{
				this.activeLocker.Get(base.isServer).DropItems(null);
			}
		}
		base.DoServerDestroy();
	}

	// Token: 0x060013A5 RID: 5029 RVA: 0x0009B0F4 File Offset: 0x000992F4
	public IItemContainerEntity GetContainer()
	{
		Locker locker = this.activeLocker.Get(base.isServer);
		if (locker != null && locker.IsValid() && !locker.inventory.IsEmpty())
		{
			return locker;
		}
		global::BaseOven baseOven = this.activeBbq.Get(base.isServer);
		if (baseOven != null && baseOven.IsValid() && !baseOven.inventory.IsEmpty())
		{
			return baseOven;
		}
		StorageContainer storageContainer = this.activeStorage.Get(base.isServer);
		if (storageContainer != null && storageContainer.IsValid() && !storageContainer.inventory.IsEmpty())
		{
			return storageContainer;
		}
		return null;
	}

	// Token: 0x060013A6 RID: 5030 RVA: 0x0009B198 File Offset: 0x00099398
	public override string Admin_Who()
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		using (List<global::BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				global::SleepingBagCamper sleepingBagCamper;
				if ((sleepingBagCamper = (enumerator.Current as global::SleepingBagCamper)) != null)
				{
					stringBuilder.AppendLine(string.Format("Bag {0}:", num++));
					stringBuilder.AppendLine(sleepingBagCamper.Admin_Who());
				}
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060013A7 RID: 5031 RVA: 0x0009B224 File Offset: 0x00099424
	public override bool CanBeLooted(global::BasePlayer player)
	{
		if (base.IsOnAVehicle && base.Vehicle.IsDead())
		{
			return base.CanBeLooted(player);
		}
		return base.CanBeLooted(player) && this.IsOnThisModule(player);
	}

	// Token: 0x060013A8 RID: 5032 RVA: 0x0009B258 File Offset: 0x00099458
	public override bool IsOnThisModule(global::BasePlayer player)
	{
		if (base.IsOnThisModule(player))
		{
			return true;
		}
		if (!player.isMounted)
		{
			return false;
		}
		OBB obb = new OBB(base.transform, this.bounds);
		return obb.Contains(player.CenterPoint());
	}

	// Token: 0x060013A9 RID: 5033 RVA: 0x0009B29C File Offset: 0x0009949C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.camperModule != null)
		{
			this.activeBbq.uid = info.msg.camperModule.bbqId;
			this.activeLocker.uid = info.msg.camperModule.lockerId;
			this.activeStorage.uid = info.msg.camperModule.storageID;
		}
	}

	// Token: 0x04000C57 RID: 3159
	public GameObjectRef SleepingBagEntity;

	// Token: 0x04000C58 RID: 3160
	public Transform[] SleepingBagPoints;

	// Token: 0x04000C59 RID: 3161
	public GameObjectRef LockerEntity;

	// Token: 0x04000C5A RID: 3162
	public Transform LockerPoint;

	// Token: 0x04000C5B RID: 3163
	public GameObjectRef BbqEntity;

	// Token: 0x04000C5C RID: 3164
	public Transform BbqPoint;

	// Token: 0x04000C5D RID: 3165
	public GameObjectRef StorageEntity;

	// Token: 0x04000C5E RID: 3166
	public Transform StoragePoint;

	// Token: 0x04000C5F RID: 3167
	private EntityRef<global::BaseOven> activeBbq;

	// Token: 0x04000C60 RID: 3168
	private EntityRef<Locker> activeLocker;

	// Token: 0x04000C61 RID: 3169
	private EntityRef<StorageContainer> activeStorage;

	// Token: 0x04000C62 RID: 3170
	private bool wasLoaded;
}
