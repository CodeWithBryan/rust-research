using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.Modular;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000E3 RID: 227
public class VehicleModuleStorage : VehicleModuleSeating
{
	// Token: 0x060013C1 RID: 5057 RVA: 0x0009B96C File Offset: 0x00099B6C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("VehicleModuleStorage.OnRpcMessage", 0))
		{
			if (rpc == 4254195175U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Open ");
				}
				using (TimeWarning.New("RPC_Open", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(4254195175U, "RPC_Open", this, player, 3f))
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
							this.RPC_Open(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Open");
					}
				}
				return true;
			}
			if (rpc == 425471188U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_TryOpenWithKeycode ");
				}
				using (TimeWarning.New("RPC_TryOpenWithKeycode", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(425471188U, "RPC_TryOpenWithKeycode", this, player, 3f))
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
							this.RPC_TryOpenWithKeycode(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_TryOpenWithKeycode");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060013C2 RID: 5058 RVA: 0x0009BC6C File Offset: 0x00099E6C
	public IItemContainerEntity GetContainer()
	{
		global::BaseEntity baseEntity = this.storageUnitInstance.Get(base.isServer);
		if (baseEntity != null && baseEntity.IsValid())
		{
			return baseEntity as IItemContainerEntity;
		}
		return null;
	}

	// Token: 0x060013C3 RID: 5059 RVA: 0x0009BCA4 File Offset: 0x00099EA4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		this.storageUnitInstance.uid = info.msg.simpleUID.uid;
	}

	// Token: 0x060013C4 RID: 5060 RVA: 0x0009BCC8 File Offset: 0x00099EC8
	public override void Spawn()
	{
		base.Spawn();
		if (!Rust.Application.isLoadingSave && this.storage.storageUnitPoint.gameObject.activeSelf)
		{
			this.CreateStorageEntity();
		}
	}

	// Token: 0x060013C5 RID: 5061 RVA: 0x0009BCF4 File Offset: 0x00099EF4
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		IItemContainerEntity container = this.GetContainer();
		if (!container.IsUnityNull<IItemContainerEntity>())
		{
			global::ItemContainer inventory = container.inventory;
			inventory.onItemAddedRemoved = (Action<global::Item, bool>)Delegate.Combine(inventory.onItemAddedRemoved, new Action<global::Item, bool>(this.OnItemAddedRemoved));
		}
	}

	// Token: 0x060013C6 RID: 5062 RVA: 0x0009ACA6 File Offset: 0x00098EA6
	private void OnItemAddedRemoved(global::Item item, bool add)
	{
		global::Item associatedItemInstance = this.AssociatedItemInstance;
		if (associatedItemInstance == null)
		{
			return;
		}
		associatedItemInstance.LockUnlock(!this.CanBeMovedNowOnVehicle());
	}

	// Token: 0x060013C7 RID: 5063 RVA: 0x0009BD40 File Offset: 0x00099F40
	public override void NonUserSpawn()
	{
		Rust.Modular.EngineStorage engineStorage = this.GetContainer() as Rust.Modular.EngineStorage;
		if (engineStorage != null)
		{
			engineStorage.NonUserSpawn();
		}
	}

	// Token: 0x060013C8 RID: 5064 RVA: 0x0009BD68 File Offset: 0x00099F68
	internal override void DoServerDestroy()
	{
		if (vehicle.vehiclesdroploot)
		{
			IItemContainerEntity container = this.GetContainer();
			if (!container.IsUnityNull<IItemContainerEntity>())
			{
				container.DropItems(null);
			}
		}
		base.DoServerDestroy();
	}

	// Token: 0x060013C9 RID: 5065 RVA: 0x0009BD98 File Offset: 0x00099F98
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.simpleUID = Facepunch.Pool.Get<SimpleUID>();
		info.msg.simpleUID.uid = this.storageUnitInstance.uid;
	}

	// Token: 0x060013CA RID: 5066 RVA: 0x0009BDCC File Offset: 0x00099FCC
	public void CreateStorageEntity()
	{
		if (!base.IsFullySpawned())
		{
			return;
		}
		if (!base.isServer)
		{
			return;
		}
		if (!this.storageUnitInstance.IsValid(base.isServer))
		{
			global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.storage.storageUnitPrefab.resourcePath, this.storage.storageUnitPoint.localPosition, this.storage.storageUnitPoint.localRotation, true);
			this.storageUnitInstance.Set(baseEntity);
			baseEntity.SetParent(this, false, false);
			baseEntity.Spawn();
			global::ItemContainer inventory = this.GetContainer().inventory;
			inventory.onItemAddedRemoved = (Action<global::Item, bool>)Delegate.Combine(inventory.onItemAddedRemoved, new Action<global::Item, bool>(this.OnItemAddedRemoved));
		}
	}

	// Token: 0x060013CB RID: 5067 RVA: 0x0009BE84 File Offset: 0x0009A084
	public void DestroyStorageEntity()
	{
		if (!base.IsFullySpawned())
		{
			return;
		}
		if (!base.isServer)
		{
			return;
		}
		global::BaseEntity baseEntity = this.storageUnitInstance.Get(base.isServer);
		if (baseEntity.IsValid())
		{
			BaseCombatEntity baseCombatEntity;
			if ((baseCombatEntity = (baseEntity as BaseCombatEntity)) != null)
			{
				baseCombatEntity.Die(null);
				return;
			}
			baseEntity.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x060013CC RID: 5068 RVA: 0x0009BED6 File Offset: 0x0009A0D6
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_Open(global::BaseEntity.RPCMessage msg)
	{
		this.TryOpen(msg.player);
	}

	// Token: 0x060013CD RID: 5069 RVA: 0x0009BEE8 File Offset: 0x0009A0E8
	private bool TryOpen(global::BasePlayer player)
	{
		if (!player.IsValid() || !this.CanBeLooted(player))
		{
			return false;
		}
		IItemContainerEntity container = this.GetContainer();
		if (!container.IsUnityNull<IItemContainerEntity>())
		{
			container.PlayerOpenLoot(player, "", true);
		}
		else
		{
			Debug.LogError(base.GetType().Name + ": No container component found.");
		}
		return true;
	}

	// Token: 0x060013CE RID: 5070 RVA: 0x0009BF44 File Offset: 0x0009A144
	protected override bool CanBeMovedNowOnVehicle()
	{
		IItemContainerEntity container = this.GetContainer();
		return container.IsUnityNull<IItemContainerEntity>() || container.inventory.IsEmpty();
	}

	// Token: 0x060013CF RID: 5071 RVA: 0x0009BF70 File Offset: 0x0009A170
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_TryOpenWithKeycode(global::BaseEntity.RPCMessage msg)
	{
		if (!base.IsOnACar)
		{
			return;
		}
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		string codeEntered = msg.read.String(256);
		if (base.Car.CarLock.TryOpenWithCode(player, codeEntered))
		{
			this.TryOpen(player);
			return;
		}
		base.Car.ClientRPC(null, "CodeEntryFailed");
	}

	// Token: 0x04000C78 RID: 3192
	[SerializeField]
	private VehicleModuleStorage.Storage storage;

	// Token: 0x04000C79 RID: 3193
	private EntityRef storageUnitInstance;

	// Token: 0x02000BC7 RID: 3015
	[Serializable]
	public class Storage
	{
		// Token: 0x04003F95 RID: 16277
		public GameObjectRef storageUnitPrefab;

		// Token: 0x04003F96 RID: 16278
		public Transform storageUnitPoint;
	}
}
