using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using Rust.Modular;
using UnityEngine;

// Token: 0x0200046D RID: 1133
public abstract class BaseModularVehicle : GroundVehicle, global::PlayerInventory.ICanMoveFrom, IPrefabPreProcess
{
	// Token: 0x170002CD RID: 717
	// (get) Token: 0x060024EA RID: 9450 RVA: 0x000E8620 File Offset: 0x000E6820
	// (set) Token: 0x060024EB RID: 9451 RVA: 0x000E8628 File Offset: 0x000E6828
	public ModularVehicleInventory Inventory { get; private set; }

	// Token: 0x060024EC RID: 9452 RVA: 0x000E8634 File Offset: 0x000E6834
	public override void ServerInit()
	{
		base.ServerInit();
		if (!this.disablePhysics)
		{
			this.rigidBody.isKinematic = false;
		}
		this.prevEditable = this.IsEditableNow;
		if (this.Inventory == null)
		{
			this.Inventory = new ModularVehicleInventory(this, this.AssociatedItemDef, true);
		}
	}

	// Token: 0x060024ED RID: 9453 RVA: 0x000E8682 File Offset: 0x000E6882
	public override void PreServerLoad()
	{
		base.PreServerLoad();
		if (this.Inventory == null)
		{
			this.Inventory = new ModularVehicleInventory(this, this.AssociatedItemDef, false);
		}
	}

	// Token: 0x060024EE RID: 9454 RVA: 0x000E86A5 File Offset: 0x000E68A5
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.Inventory != null && this.Inventory.UID == 0U)
		{
			this.Inventory.GiveUIDs();
		}
		base.SetFlag(global::BaseEntity.Flags.Open, false, false, true);
	}

	// Token: 0x060024EF RID: 9455 RVA: 0x000E86D7 File Offset: 0x000E68D7
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (this.Inventory != null)
		{
			this.Inventory.Dispose();
		}
	}

	// Token: 0x060024F0 RID: 9456 RVA: 0x0006FAA5 File Offset: 0x0006DCA5
	public override float MaxVelocity()
	{
		return Mathf.Max(this.GetMaxForwardSpeed() * 1.3f, 30f);
	}

	// Token: 0x060024F1 RID: 9457
	public abstract bool IsComplete();

	// Token: 0x060024F2 RID: 9458 RVA: 0x000E86F2 File Offset: 0x000E68F2
	public bool CouldBeEdited()
	{
		return !this.AnyMounted() && !this.IsDead();
	}

	// Token: 0x060024F3 RID: 9459 RVA: 0x000E8707 File Offset: 0x000E6907
	public void DisablePhysics()
	{
		this.disablePhysics = true;
		this.rigidBody.isKinematic = true;
	}

	// Token: 0x060024F4 RID: 9460 RVA: 0x000E871C File Offset: 0x000E691C
	public void EnablePhysics()
	{
		this.disablePhysics = false;
		this.rigidBody.isKinematic = false;
	}

	// Token: 0x060024F5 RID: 9461 RVA: 0x000E8734 File Offset: 0x000E6934
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (this.IsEditableNow != this.prevEditable)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.prevEditable = this.IsEditableNow;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved6, this.rigidBody.isKinematic, false, true);
	}

	// Token: 0x060024F6 RID: 9462 RVA: 0x000E8780 File Offset: 0x000E6980
	public override bool MountEligable(global::BasePlayer player)
	{
		return base.MountEligable(player) && !this.IsDead() && (!base.HasDriver() || base.Velocity.magnitude < 2f);
	}

	// Token: 0x060024F7 RID: 9463 RVA: 0x000E87C2 File Offset: 0x000E69C2
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.modularVehicle = Pool.Get<ModularVehicle>();
		info.msg.modularVehicle.editable = this.IsEditableNow;
	}

	// Token: 0x060024F8 RID: 9464 RVA: 0x000E87F4 File Offset: 0x000E69F4
	public bool CanMoveFrom(global::BasePlayer player, global::Item item)
	{
		BaseVehicleModule moduleForItem = this.GetModuleForItem(item);
		return !(moduleForItem != null) || moduleForItem.CanBeMovedNow();
	}

	// Token: 0x060024F9 RID: 9465
	protected abstract Vector3 GetCOMMultiplier();

	// Token: 0x060024FA RID: 9466
	public abstract void ModuleHurt(BaseVehicleModule hurtModule, HitInfo info);

	// Token: 0x060024FB RID: 9467
	public abstract void ModuleReachedZeroHealth();

	// Token: 0x060024FC RID: 9468 RVA: 0x000E881C File Offset: 0x000E6A1C
	public bool TryAddModule(global::Item moduleItem, int socketIndex)
	{
		string str;
		if (!this.ModuleCanBeAdded(moduleItem, socketIndex, out str))
		{
			Debug.LogError(base.GetType().Name + ": Can't add module: " + str);
			return false;
		}
		bool flag = this.Inventory.TryAddModuleItem(moduleItem, socketIndex);
		if (!flag)
		{
			Debug.LogError(base.GetType().Name + ": Couldn't add new item!");
		}
		return flag;
	}

	// Token: 0x060024FD RID: 9469 RVA: 0x000E887C File Offset: 0x000E6A7C
	public bool TryAddModule(global::Item moduleItem)
	{
		ItemModVehicleModule component = moduleItem.info.GetComponent<ItemModVehicleModule>();
		if (component == null)
		{
			return false;
		}
		int socketsTaken = component.socketsTaken;
		int num = this.Inventory.TryGetFreeSocket(socketsTaken);
		return num >= 0 && this.TryAddModule(moduleItem, num);
	}

	// Token: 0x060024FE RID: 9470 RVA: 0x000E88C4 File Offset: 0x000E6AC4
	public bool ModuleCanBeAdded(global::Item moduleItem, int socketIndex, out string failureReason)
	{
		if (!base.isServer)
		{
			failureReason = "Can only add modules on server";
			return false;
		}
		if (moduleItem == null)
		{
			failureReason = "Module item is null";
			return false;
		}
		if (moduleItem.info.category != ItemCategory.Component)
		{
			failureReason = "Not a component type item";
			return false;
		}
		ItemModVehicleModule component = moduleItem.info.GetComponent<ItemModVehicleModule>();
		if (component == null)
		{
			failureReason = "Not the right item module type";
			return false;
		}
		int socketsTaken = component.socketsTaken;
		if (socketIndex < 0)
		{
			socketIndex = this.Inventory.TryGetFreeSocket(socketsTaken);
		}
		if (!this.Inventory.SocketsAreFree(socketIndex, socketsTaken, moduleItem))
		{
			failureReason = "One or more desired sockets already in use";
			return false;
		}
		failureReason = string.Empty;
		return true;
	}

	// Token: 0x060024FF RID: 9471 RVA: 0x000E8960 File Offset: 0x000E6B60
	public BaseVehicleModule CreatePhysicalModuleEntity(global::Item moduleItem, ItemModVehicleModule itemModModule, int socketIndex)
	{
		Vector3 worldPosition = this.moduleSockets[socketIndex].WorldPosition;
		Quaternion worldRotation = this.moduleSockets[socketIndex].WorldRotation;
		BaseVehicleModule baseVehicleModule = itemModModule.CreateModuleEntity(this, worldPosition, worldRotation);
		baseVehicleModule.AssociatedItemInstance = moduleItem;
		this.SetUpModule(baseVehicleModule, moduleItem);
		return baseVehicleModule;
	}

	// Token: 0x06002500 RID: 9472 RVA: 0x000E89AB File Offset: 0x000E6BAB
	public void SetUpModule(BaseVehicleModule moduleEntity, global::Item moduleItem)
	{
		moduleEntity.InitializeHealth(moduleItem.condition, moduleItem.maxCondition);
		if (moduleItem.condition < moduleItem.maxCondition)
		{
			moduleEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06002501 RID: 9473 RVA: 0x000E89D4 File Offset: 0x000E6BD4
	public global::Item GetVehicleItem(uint itemUID)
	{
		global::Item item = this.Inventory.ChassisContainer.FindItemByUID(itemUID);
		if (item == null)
		{
			item = this.Inventory.ModuleContainer.FindItemByUID(itemUID);
		}
		return item;
	}

	// Token: 0x06002502 RID: 9474 RVA: 0x000E8A0C File Offset: 0x000E6C0C
	public BaseVehicleModule GetModuleForItem(global::Item item)
	{
		if (item == null)
		{
			return null;
		}
		foreach (BaseVehicleModule baseVehicleModule in this.AttachedModuleEntities)
		{
			if (baseVehicleModule.AssociatedItemInstance == item)
			{
				return baseVehicleModule;
			}
		}
		return null;
	}

	// Token: 0x06002503 RID: 9475 RVA: 0x000E8A70 File Offset: 0x000E6C70
	private void SetMass(float mass)
	{
		this.TotalMass = mass;
		this.rigidBody.mass = this.TotalMass;
	}

	// Token: 0x06002504 RID: 9476 RVA: 0x000E8A8A File Offset: 0x000E6C8A
	private void SetCOM(Vector3 com)
	{
		this.realLocalCOM = com;
		this.rigidBody.centerOfMass = Vector3.Scale(this.realLocalCOM, this.GetCOMMultiplier());
	}

	// Token: 0x170002CE RID: 718
	// (get) Token: 0x06002505 RID: 9477 RVA: 0x000E8AAF File Offset: 0x000E6CAF
	public Vector3 CentreOfMass
	{
		get
		{
			return this.centreOfMassTransform.localPosition;
		}
	}

	// Token: 0x170002CF RID: 719
	// (get) Token: 0x06002506 RID: 9478 RVA: 0x000E8ABC File Offset: 0x000E6CBC
	public int NumAttachedModules
	{
		get
		{
			return this.AttachedModuleEntities.Count;
		}
	}

	// Token: 0x170002D0 RID: 720
	// (get) Token: 0x06002507 RID: 9479 RVA: 0x000E8AC9 File Offset: 0x000E6CC9
	public bool HasAnyModules
	{
		get
		{
			return this.AttachedModuleEntities.Count > 0;
		}
	}

	// Token: 0x170002D1 RID: 721
	// (get) Token: 0x06002508 RID: 9480 RVA: 0x000E8AD9 File Offset: 0x000E6CD9
	public List<BaseVehicleModule> AttachedModuleEntities { get; } = new List<BaseVehicleModule>();

	// Token: 0x170002D2 RID: 722
	// (get) Token: 0x06002509 RID: 9481 RVA: 0x000E8AE1 File Offset: 0x000E6CE1
	public int TotalSockets
	{
		get
		{
			return this.moduleSockets.Count;
		}
	}

	// Token: 0x170002D3 RID: 723
	// (get) Token: 0x0600250A RID: 9482 RVA: 0x000E8AF0 File Offset: 0x000E6CF0
	public int NumFreeSockets
	{
		get
		{
			int num = 0;
			for (int i = 0; i < this.NumAttachedModules; i++)
			{
				num += this.AttachedModuleEntities[i].GetNumSocketsTaken();
			}
			return this.TotalSockets - num;
		}
	}

	// Token: 0x170002D4 RID: 724
	// (get) Token: 0x0600250B RID: 9483 RVA: 0x000E8B2C File Offset: 0x000E6D2C
	private float Mass
	{
		get
		{
			if (base.isServer)
			{
				return this.rigidBody.mass;
			}
			return this._mass;
		}
	}

	// Token: 0x170002D5 RID: 725
	// (get) Token: 0x0600250C RID: 9484 RVA: 0x000E8B48 File Offset: 0x000E6D48
	// (set) Token: 0x0600250D RID: 9485 RVA: 0x000E8B50 File Offset: 0x000E6D50
	public float TotalMass { get; private set; }

	// Token: 0x170002D6 RID: 726
	// (get) Token: 0x0600250E RID: 9486 RVA: 0x000035EB File Offset: 0x000017EB
	public bool IsKinematic
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved6);
		}
	}

	// Token: 0x170002D7 RID: 727
	// (get) Token: 0x0600250F RID: 9487 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool IsLockable
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170002D8 RID: 728
	// (get) Token: 0x06002510 RID: 9488 RVA: 0x000E8B59 File Offset: 0x000E6D59
	// (set) Token: 0x06002511 RID: 9489 RVA: 0x000E8B61 File Offset: 0x000E6D61
	public bool HasInited { get; private set; }

	// Token: 0x170002D9 RID: 729
	// (get) Token: 0x06002512 RID: 9490 RVA: 0x0004D717 File Offset: 0x0004B917
	private ItemDefinition AssociatedItemDef
	{
		get
		{
			return this.repair.itemTarget;
		}
	}

	// Token: 0x170002DA RID: 730
	// (get) Token: 0x06002513 RID: 9491 RVA: 0x000E8B6A File Offset: 0x000E6D6A
	public bool IsEditableNow
	{
		get
		{
			return base.isServer && this.inEditableLocation && this.CouldBeEdited();
		}
	}

	// Token: 0x06002514 RID: 9492 RVA: 0x000E8B88 File Offset: 0x000E6D88
	public override void InitShared()
	{
		base.InitShared();
		this.AddMass(this.Mass, this.CentreOfMass, base.transform.position);
		this.HasInited = true;
		foreach (BaseVehicleModule baseVehicleModule in this.AttachedModuleEntities)
		{
			baseVehicleModule.RefreshConditionals(false);
		}
	}

	// Token: 0x06002515 RID: 9493 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool PlayerCanUseThis(global::BasePlayer player, ModularCarCodeLock.LockType lockType)
	{
		return true;
	}

	// Token: 0x06002516 RID: 9494 RVA: 0x000E8C04 File Offset: 0x000E6E04
	public bool TryDeduceSocketIndex(BaseVehicleModule addedModule, out int index)
	{
		if (addedModule.FirstSocketIndex >= 0)
		{
			index = addedModule.FirstSocketIndex;
			return index >= 0;
		}
		index = -1;
		for (int i = 0; i < this.moduleSockets.Count; i++)
		{
			if (Vector3.SqrMagnitude(this.moduleSockets[i].WorldPosition - addedModule.transform.position) < 0.1f)
			{
				index = i;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002517 RID: 9495 RVA: 0x000E8C78 File Offset: 0x000E6E78
	public void AddMass(float moduleMass, Vector3 moduleCOM, Vector3 moduleWorldPos)
	{
		if (base.isServer)
		{
			Vector3 vector = base.transform.InverseTransformPoint(moduleWorldPos) + moduleCOM;
			if (this.TotalMass == 0f)
			{
				this.SetMass(moduleMass);
				this.SetCOM(vector);
				return;
			}
			float num = this.TotalMass + moduleMass;
			Vector3 com = this.realLocalCOM * (this.TotalMass / num) + vector * (moduleMass / num);
			this.SetMass(num);
			this.SetCOM(com);
		}
	}

	// Token: 0x06002518 RID: 9496 RVA: 0x000E8CF8 File Offset: 0x000E6EF8
	public void RemoveMass(float moduleMass, Vector3 moduleCOM, Vector3 moduleWorldPos)
	{
		if (base.isServer)
		{
			float num = this.TotalMass - moduleMass;
			Vector3 a = base.transform.InverseTransformPoint(moduleWorldPos) + moduleCOM;
			Vector3 com = (this.realLocalCOM - a * (moduleMass / this.TotalMass)) / (num / this.TotalMass);
			this.SetMass(num);
			this.SetCOM(com);
		}
	}

	// Token: 0x06002519 RID: 9497 RVA: 0x000E8D60 File Offset: 0x000E6F60
	public bool TryGetModuleAt(int socketIndex, out BaseVehicleModule result)
	{
		if (socketIndex < 0 || socketIndex >= this.moduleSockets.Count)
		{
			result = null;
			return false;
		}
		foreach (BaseVehicleModule baseVehicleModule in this.AttachedModuleEntities)
		{
			int firstSocketIndex = baseVehicleModule.FirstSocketIndex;
			int num = firstSocketIndex + baseVehicleModule.GetNumSocketsTaken() - 1;
			if (firstSocketIndex <= socketIndex && num >= socketIndex)
			{
				result = baseVehicleModule;
				return true;
			}
		}
		result = null;
		return false;
	}

	// Token: 0x0600251A RID: 9498 RVA: 0x000E8DE8 File Offset: 0x000E6FE8
	public ModularVehicleSocket GetSocket(int index)
	{
		if (index < 0 || index >= this.moduleSockets.Count)
		{
			return null;
		}
		return this.moduleSockets[index];
	}

	// Token: 0x0600251B RID: 9499 RVA: 0x000E8E0A File Offset: 0x000E700A
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		ModularVehicle modularVehicle = info.msg.modularVehicle;
	}

	// Token: 0x0600251C RID: 9500 RVA: 0x000E8E1F File Offset: 0x000E701F
	protected override bool CanPushNow(global::BasePlayer pusher)
	{
		return base.CanPushNow(pusher) && !this.IsKinematic && !this.IsEditableNow;
	}

	// Token: 0x0600251D RID: 9501 RVA: 0x000E8E40 File Offset: 0x000E7040
	protected override void OnChildAdded(global::BaseEntity childEntity)
	{
		base.OnChildAdded(childEntity);
		BaseVehicleModule module;
		if ((module = (childEntity as BaseVehicleModule)) != null)
		{
			Action action = delegate()
			{
				this.ModuleEntityAdded(module);
			};
			this.moduleAddActions[module] = action;
			module.Invoke(action, 0f);
		}
	}

	// Token: 0x0600251E RID: 9502 RVA: 0x000E8EA4 File Offset: 0x000E70A4
	protected override void OnChildRemoved(global::BaseEntity childEntity)
	{
		base.OnChildRemoved(childEntity);
		BaseVehicleModule removedModule;
		if ((removedModule = (childEntity as BaseVehicleModule)) != null)
		{
			this.ModuleEntityRemoved(removedModule);
		}
	}

	// Token: 0x0600251F RID: 9503 RVA: 0x000E8ECC File Offset: 0x000E70CC
	protected virtual void ModuleEntityAdded(BaseVehicleModule addedModule)
	{
		if (this.AttachedModuleEntities.Contains(addedModule))
		{
			return;
		}
		if (base.isServer && (this == null || this.IsDead() || base.IsDestroyed))
		{
			if (addedModule != null && !addedModule.IsDestroyed)
			{
				addedModule.Kill(global::BaseNetworkable.DestroyMode.None);
			}
			return;
		}
		int num = -1;
		if (base.isServer && addedModule.AssociatedItemInstance != null)
		{
			num = addedModule.AssociatedItemInstance.position;
		}
		if (num == -1 && !this.TryDeduceSocketIndex(addedModule, out num))
		{
			string text = string.Format("{0}: Couldn't get socket index from position ({1}).", base.GetType().Name, addedModule.transform.position);
			for (int i = 0; i < this.moduleSockets.Count; i++)
			{
				text += string.Format(" Sqr dist to socket {0} at {1} is {2}.", i, this.moduleSockets[i].WorldPosition, Vector3.SqrMagnitude(this.moduleSockets[i].WorldPosition - addedModule.transform.position));
			}
			Debug.LogError(text, addedModule.gameObject);
			return;
		}
		if (this.moduleAddActions.ContainsKey(addedModule))
		{
			this.moduleAddActions.Remove(addedModule);
		}
		this.AttachedModuleEntities.Add(addedModule);
		addedModule.ModuleAdded(this, num);
		this.AddMass(addedModule.Mass, addedModule.CentreOfMass, addedModule.transform.position);
		if (base.isServer && !this.Inventory.TrySyncModuleInventory(addedModule, num))
		{
			Debug.LogError(string.Format("{0}: Unable to add module {1} to socket ({2}). Destroying it.", base.GetType().Name, addedModule.name, num), base.gameObject);
			addedModule.Kill(global::BaseNetworkable.DestroyMode.None);
			this.AttachedModuleEntities.Remove(addedModule);
			return;
		}
		this.RefreshModulesExcept(addedModule);
		if (base.isServer)
		{
			this.UpdateMountFlags();
		}
	}

	// Token: 0x06002520 RID: 9504 RVA: 0x000E90B0 File Offset: 0x000E72B0
	protected virtual void ModuleEntityRemoved(BaseVehicleModule removedModule)
	{
		if (base.IsDestroyed)
		{
			return;
		}
		if (this.moduleAddActions.ContainsKey(removedModule))
		{
			removedModule.CancelInvoke(this.moduleAddActions[removedModule]);
			this.moduleAddActions.Remove(removedModule);
		}
		if (!this.AttachedModuleEntities.Contains(removedModule))
		{
			return;
		}
		this.RemoveMass(removedModule.Mass, removedModule.CentreOfMass, removedModule.transform.position);
		this.AttachedModuleEntities.Remove(removedModule);
		removedModule.ModuleRemoved();
		this.RefreshModulesExcept(removedModule);
		if (base.isServer)
		{
			this.UpdateMountFlags();
		}
	}

	// Token: 0x06002521 RID: 9505 RVA: 0x000E9148 File Offset: 0x000E7348
	private void RefreshModulesExcept(BaseVehicleModule ignoredModule)
	{
		foreach (BaseVehicleModule baseVehicleModule in this.AttachedModuleEntities)
		{
			if (baseVehicleModule != ignoredModule)
			{
				baseVehicleModule.OtherVehicleModulesChanged();
			}
		}
	}

	// Token: 0x04001DA2 RID: 7586
	internal bool inEditableLocation;

	// Token: 0x04001DA3 RID: 7587
	private bool prevEditable;

	// Token: 0x04001DA4 RID: 7588
	internal bool immuneToDecay;

	// Token: 0x04001DA6 RID: 7590
	protected Vector3 realLocalCOM;

	// Token: 0x04001DA7 RID: 7591
	public global::Item AssociatedItemInstance;

	// Token: 0x04001DA8 RID: 7592
	private bool disablePhysics;

	// Token: 0x04001DA9 RID: 7593
	[Header("Modular Vehicle")]
	[SerializeField]
	private List<ModularVehicleSocket> moduleSockets;

	// Token: 0x04001DAA RID: 7594
	[SerializeField]
	private Transform centreOfMassTransform;

	// Token: 0x04001DAB RID: 7595
	[SerializeField]
	protected Transform waterSample;

	// Token: 0x04001DAC RID: 7596
	[SerializeField]
	private LODGroup lodGroup;

	// Token: 0x04001DAD RID: 7597
	public GameObjectRef keyEnterDialog;

	// Token: 0x04001DAF RID: 7599
	private float _mass = -1f;

	// Token: 0x04001DB2 RID: 7602
	public const global::BaseEntity.Flags FLAG_KINEMATIC = global::BaseEntity.Flags.Reserved6;

	// Token: 0x04001DB3 RID: 7603
	private Dictionary<BaseVehicleModule, Action> moduleAddActions = new Dictionary<BaseVehicleModule, Action>();
}
