using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.Modular;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000046 RID: 70
public class BaseVehicleModule : global::BaseVehicle, IPrefabPreProcess
{
	// Token: 0x060007DA RID: 2010 RVA: 0x0004D2FC File Offset: 0x0004B4FC
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseVehicleModule.OnRpcMessage", 0))
		{
			if (rpc == 2683376664U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Use ");
				}
				using (TimeWarning.New("RPC_Use", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2683376664U, "RPC_Use", this, player, 3f))
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
							this.RPC_Use(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Use");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170000C5 RID: 197
	// (get) Token: 0x060007DB RID: 2011 RVA: 0x0004D464 File Offset: 0x0004B664
	// (set) Token: 0x060007DC RID: 2012 RVA: 0x0004D46C File Offset: 0x0004B66C
	public bool PropagateDamage { get; private set; } = true;

	// Token: 0x060007DD RID: 2013 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void NonUserSpawn()
	{
	}

	// Token: 0x060007DE RID: 2014 RVA: 0x0004D478 File Offset: 0x0004B678
	public override void VehicleFixedUpdate()
	{
		if (!this.isSpawned || !this.IsOnAVehicle)
		{
			return;
		}
		base.VehicleFixedUpdate();
		if (this.Vehicle.IsEditableNow && this.AssociatedItemInstance != null && this.timeSinceItemLockRefresh > 1f)
		{
			this.AssociatedItemInstance.LockUnlock(!this.CanBeMovedNow());
			this.timeSinceItemLockRefresh = 0f;
		}
		for (int i = 0; i < this.slidingComponents.Length; i++)
		{
			this.slidingComponents[i].ServerUpdateTick(this);
		}
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x0004D508 File Offset: 0x0004B708
	public override void Hurt(HitInfo info)
	{
		if (this.IsOnAVehicle)
		{
			this.Vehicle.ModuleHurt(this, info);
		}
		base.Hurt(info);
	}

	// Token: 0x060007E0 RID: 2016 RVA: 0x0004D528 File Offset: 0x0004B728
	public override void OnHealthChanged(float oldValue, float newValue)
	{
		base.OnHealthChanged(oldValue, newValue);
		if (!base.isServer)
		{
			return;
		}
		if (this.IsOnAVehicle)
		{
			if (this.Vehicle.IsDead())
			{
				return;
			}
			if (this.AssociatedItemInstance != null)
			{
				this.AssociatedItemInstance.condition = this.Health();
			}
			if (newValue <= 0f)
			{
				this.Vehicle.ModuleReachedZeroHealth();
			}
		}
		this.RefreshConditionals(true);
	}

	// Token: 0x060007E1 RID: 2017 RVA: 0x0004D58F File Offset: 0x0004B78F
	public bool CanBeMovedNow()
	{
		return !this.IsOnAVehicle || this.CanBeMovedNowOnVehicle();
	}

	// Token: 0x060007E2 RID: 2018 RVA: 0x00003A54 File Offset: 0x00001C54
	protected virtual bool CanBeMovedNowOnVehicle()
	{
		return true;
	}

	// Token: 0x060007E3 RID: 2019 RVA: 0x00026FFC File Offset: 0x000251FC
	public virtual float GetAdjustedDriveForce(float absSpeed, float topSpeed)
	{
		return 0f;
	}

	// Token: 0x060007E4 RID: 2020 RVA: 0x0004D5A1 File Offset: 0x0004B7A1
	public void AcceptPropagatedDamage(float amount, DamageType type, global::BaseEntity attacker = null, bool useProtection = true)
	{
		this.PropagateDamage = false;
		base.Hurt(amount, type, attacker, useProtection);
		this.PropagateDamage = true;
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x000059DD File Offset: 0x00003BDD
	public override void Die(HitInfo info = null)
	{
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x0004D5BC File Offset: 0x0004B7BC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_Use(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.CanBeUsedNowBy(player))
		{
			return;
		}
		string lookingAtColldierName = msg.read.String(256);
		foreach (VehicleModuleSlidingComponent vehicleModuleSlidingComponent in this.slidingComponents)
		{
			if (this.PlayerIsLookingAtUsable(lookingAtColldierName, vehicleModuleSlidingComponent.interactionColliderName))
			{
				vehicleModuleSlidingComponent.Use(this);
				break;
			}
		}
		foreach (VehicleModuleButtonComponent vehicleModuleButtonComponent in this.buttonComponents)
		{
			if (vehicleModuleButtonComponent == null)
			{
				return;
			}
			if (this.PlayerIsLookingAtUsable(lookingAtColldierName, vehicleModuleButtonComponent.interactionColliderName))
			{
				vehicleModuleButtonComponent.ServerUse(player, this);
				return;
			}
		}
	}

	// Token: 0x060007E7 RID: 2023 RVA: 0x0004D661 File Offset: 0x0004B861
	public override void AdminKill()
	{
		if (this.IsOnAVehicle)
		{
			this.Vehicle.AdminKill();
		}
	}

	// Token: 0x060007E8 RID: 2024 RVA: 0x0004D676 File Offset: 0x0004B876
	public override bool AdminFixUp(int tier)
	{
		return (!this.IsOnAVehicle || !this.Vehicle.IsDead()) && base.AdminFixUp(tier);
	}

	// Token: 0x060007E9 RID: 2025 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnPlayerDismountedVehicle(global::BasePlayer player)
	{
	}

	// Token: 0x060007EA RID: 2026 RVA: 0x0004D696 File Offset: 0x0004B896
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.vehicleModule = Facepunch.Pool.Get<VehicleModule>();
		info.msg.vehicleModule.socketIndex = this.FirstSocketIndex;
	}

	// Token: 0x170000C6 RID: 198
	// (get) Token: 0x060007EB RID: 2027 RVA: 0x0004D6C5 File Offset: 0x0004B8C5
	// (set) Token: 0x060007EC RID: 2028 RVA: 0x0004D6CD File Offset: 0x0004B8CD
	public BaseModularVehicle Vehicle { get; private set; }

	// Token: 0x170000C7 RID: 199
	// (get) Token: 0x060007ED RID: 2029 RVA: 0x0004D6D6 File Offset: 0x0004B8D6
	// (set) Token: 0x060007EE RID: 2030 RVA: 0x0004D6DE File Offset: 0x0004B8DE
	public int FirstSocketIndex { get; private set; } = -1;

	// Token: 0x170000C8 RID: 200
	// (get) Token: 0x060007EF RID: 2031 RVA: 0x0004D6E7 File Offset: 0x0004B8E7
	public Vector3 CentreOfMass
	{
		get
		{
			return this.centreOfMassTransform.localPosition;
		}
	}

	// Token: 0x170000C9 RID: 201
	// (get) Token: 0x060007F0 RID: 2032 RVA: 0x0004D6F4 File Offset: 0x0004B8F4
	public float Mass
	{
		get
		{
			return this.mass;
		}
	}

	// Token: 0x170000CA RID: 202
	// (get) Token: 0x060007F1 RID: 2033 RVA: 0x0004D6FC File Offset: 0x0004B8FC
	public uint ID
	{
		get
		{
			return this.net.ID;
		}
	}

	// Token: 0x170000CB RID: 203
	// (get) Token: 0x060007F2 RID: 2034 RVA: 0x0004D709 File Offset: 0x0004B909
	public bool IsOnAVehicle
	{
		get
		{
			return this.Vehicle != null;
		}
	}

	// Token: 0x170000CC RID: 204
	// (get) Token: 0x060007F3 RID: 2035 RVA: 0x0004D717 File Offset: 0x0004B917
	public ItemDefinition AssociatedItemDef
	{
		get
		{
			return this.repair.itemTarget;
		}
	}

	// Token: 0x170000CD RID: 205
	// (get) Token: 0x060007F4 RID: 2036 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool HasSeating
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000CE RID: 206
	// (get) Token: 0x060007F5 RID: 2037 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool HasAnEngine
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060007F6 RID: 2038 RVA: 0x0004D724 File Offset: 0x0004B924
	public override void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(process, rootObj, name, serverside, clientside, bundling);
		this.damageRenderer = base.GetComponent<DamageRenderer>();
		this.RefreshParameters();
		this.lights = base.GetComponentsInChildren<VehicleLight>();
	}

	// Token: 0x060007F7 RID: 2039 RVA: 0x0004D754 File Offset: 0x0004B954
	public void RefreshParameters()
	{
		for (int i = this.conditionals.Count - 1; i >= 0; i--)
		{
			ConditionalObject conditionalObject = this.conditionals[i];
			if (conditionalObject.gameObject == null)
			{
				this.conditionals.RemoveAt(i);
			}
			else if (conditionalObject.restrictOnHealth)
			{
				conditionalObject.healthRestrictionMin = Mathf.Clamp01(conditionalObject.healthRestrictionMin);
				conditionalObject.healthRestrictionMax = Mathf.Clamp01(conditionalObject.healthRestrictionMax);
			}
			if (conditionalObject.gameObject != null)
			{
				Gibbable component = conditionalObject.gameObject.GetComponent<Gibbable>();
				if (component != null)
				{
					component.isConditional = true;
				}
			}
		}
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x0004D7FC File Offset: 0x0004B9FC
	public override global::BaseVehicle VehicleParent()
	{
		return base.GetParentEntity() as global::BaseVehicle;
	}

	// Token: 0x060007F9 RID: 2041 RVA: 0x0004D80C File Offset: 0x0004BA0C
	public virtual void ModuleAdded(BaseModularVehicle vehicle, int firstSocketIndex)
	{
		this.Vehicle = vehicle;
		this.FirstSocketIndex = firstSocketIndex;
		this.TimeSinceAddedToVehicle = 0f;
		if (base.isServer)
		{
			TriggerParent[] array = this.triggerParents;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].associatedMountable = vehicle;
			}
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
		this.RefreshConditionals(false);
	}

	// Token: 0x060007FA RID: 2042 RVA: 0x0004D86C File Offset: 0x0004BA6C
	public virtual void ModuleRemoved()
	{
		this.Vehicle = null;
		this.FirstSocketIndex = -1;
		if (base.isServer)
		{
			TriggerParent[] array = this.triggerParents;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].associatedMountable = null;
			}
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x0004D8B4 File Offset: 0x0004BAB4
	public void OtherVehicleModulesChanged()
	{
		this.RefreshConditionals(false);
	}

	// Token: 0x060007FC RID: 2044 RVA: 0x0004D8BD File Offset: 0x0004BABD
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return this.IsOnAVehicle && this.Vehicle.CanBeLooted(player);
	}

	// Token: 0x060007FD RID: 2045 RVA: 0x0004D8D8 File Offset: 0x0004BAD8
	public bool KeycodeEntryBlocked(global::BasePlayer player)
	{
		global::ModularCar modularCar;
		return this.IsOnAVehicle && (modularCar = (this.Vehicle as global::ModularCar)) != null && modularCar.KeycodeEntryBlocked(player);
	}

	// Token: 0x060007FE RID: 2046 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnEngineStateChanged(VehicleEngineController<GroundVehicle>.EngineState oldState, VehicleEngineController<GroundVehicle>.EngineState newState)
	{
	}

	// Token: 0x060007FF RID: 2047 RVA: 0x0004D905 File Offset: 0x0004BB05
	public override float MaxHealth()
	{
		if (this.AssociatedItemDef != null)
		{
			return this.AssociatedItemDef.condition.max;
		}
		return base.MaxHealth();
	}

	// Token: 0x06000800 RID: 2048 RVA: 0x0004D92C File Offset: 0x0004BB2C
	public override float StartHealth()
	{
		return this.MaxHealth();
	}

	// Token: 0x06000801 RID: 2049 RVA: 0x0004D934 File Offset: 0x0004BB34
	public int GetNumSocketsTaken()
	{
		if (this.AssociatedItemDef == null)
		{
			return 1;
		}
		return this.AssociatedItemDef.GetComponent<ItemModVehicleModule>().socketsTaken;
	}

	// Token: 0x06000802 RID: 2050 RVA: 0x0004D958 File Offset: 0x0004BB58
	public List<ConditionalObject> GetConditionals()
	{
		List<ConditionalObject> list = new List<ConditionalObject>();
		foreach (ConditionalObject conditionalObject in this.conditionals)
		{
			if (conditionalObject.gameObject != null)
			{
				list.Add(conditionalObject);
			}
		}
		return list;
	}

	// Token: 0x06000803 RID: 2051 RVA: 0x00026FFC File Offset: 0x000251FC
	public virtual float GetMaxDriveForce()
	{
		return 0f;
	}

	// Token: 0x06000804 RID: 2052 RVA: 0x0004D9C0 File Offset: 0x0004BBC0
	public void RefreshConditionals(bool canGib)
	{
		if (base.IsDestroyed || !this.IsOnAVehicle || !this.Vehicle.HasInited)
		{
			return;
		}
		foreach (ConditionalObject conditional in this.conditionals)
		{
			this.RefreshConditional(conditional, canGib);
		}
		this.prevRefreshHealth = this.Health();
		this.prevRefreshVehicleIsDead = this.Vehicle.IsDead();
		this.prevRefreshVehicleIsLockable = this.Vehicle.IsLockable;
		this.PostConditionalRefresh();
	}

	// Token: 0x06000805 RID: 2053 RVA: 0x000059DD File Offset: 0x00003BDD
	protected virtual void PostConditionalRefresh()
	{
	}

	// Token: 0x06000806 RID: 2054 RVA: 0x0004DA68 File Offset: 0x0004BC68
	private void RefreshConditional(ConditionalObject conditional, bool canGib)
	{
		if (conditional == null || conditional.gameObject == null)
		{
			return;
		}
		bool flag = true;
		if (conditional.restrictOnHealth)
		{
			if (Mathf.Approximately(conditional.healthRestrictionMin, conditional.healthRestrictionMax))
			{
				flag = Mathf.Approximately(base.healthFraction, conditional.healthRestrictionMin);
			}
			else
			{
				flag = (base.healthFraction > conditional.healthRestrictionMin && base.healthFraction <= conditional.healthRestrictionMax);
			}
			if (canGib)
			{
			}
		}
		if (flag && this.IsOnAVehicle && conditional.restrictOnLockable)
		{
			flag = (this.Vehicle.IsLockable == conditional.lockableRestriction);
		}
		if (flag && conditional.restrictOnAdjacent)
		{
			bool flag2 = false;
			bool flag3 = false;
			BaseVehicleModule moduleEntity;
			if (this.TryGetAdjacentModuleInFront(out moduleEntity))
			{
				flag2 = this.InSameVisualGroupAs(moduleEntity, conditional.adjacentMatch);
			}
			if (this.TryGetAdjacentModuleBehind(out moduleEntity))
			{
				flag3 = this.InSameVisualGroupAs(moduleEntity, conditional.adjacentMatch);
			}
			switch (conditional.adjacentRestriction)
			{
			case ConditionalObject.AdjacentCondition.SameInFront:
				flag = flag2;
				break;
			case ConditionalObject.AdjacentCondition.SameBehind:
				flag = flag3;
				break;
			case ConditionalObject.AdjacentCondition.DifferentInFront:
				flag = !flag2;
				break;
			case ConditionalObject.AdjacentCondition.DifferentBehind:
				flag = !flag3;
				break;
			case ConditionalObject.AdjacentCondition.BothDifferent:
				flag = (!flag2 && !flag3);
				break;
			case ConditionalObject.AdjacentCondition.BothSame:
				flag = (flag2 && flag3);
				break;
			}
		}
		if (flag)
		{
			if (!this.IsOnAVehicle)
			{
				for (int i = 0; i < conditional.socketSettings.Length; i++)
				{
					flag = !conditional.socketSettings[i].HasSocketRestrictions;
					if (!flag)
					{
						break;
					}
				}
			}
			else
			{
				for (int j = 0; j < conditional.socketSettings.Length; j++)
				{
					ModularVehicleSocket socket = this.Vehicle.GetSocket(this.FirstSocketIndex + j);
					if (socket == null)
					{
						Debug.LogWarning(string.Format("{0} module got NULL socket at index {1}. Total vehicle sockets: {2} FirstSocketIndex: {3} Sockets taken: {4}", new object[]
						{
							this.AssociatedItemDef.displayName.translated,
							this.FirstSocketIndex + j,
							this.Vehicle.TotalSockets,
							this.FirstSocketIndex,
							conditional.socketSettings.Length
						}));
					}
					flag = (socket != null && socket.ShouldBeActive(conditional.socketSettings[j]));
					if (!flag)
					{
						break;
					}
				}
			}
		}
		bool activeInHierarchy = conditional.gameObject.activeInHierarchy;
		conditional.SetActive(flag);
	}

	// Token: 0x06000807 RID: 2055 RVA: 0x0004DCB0 File Offset: 0x0004BEB0
	private bool TryGetAdjacentModuleInFront(out BaseVehicleModule result)
	{
		if (!this.IsOnAVehicle)
		{
			result = null;
			return false;
		}
		int socketIndex = this.FirstSocketIndex - 1;
		return this.Vehicle.TryGetModuleAt(socketIndex, out result);
	}

	// Token: 0x06000808 RID: 2056 RVA: 0x0004DCE0 File Offset: 0x0004BEE0
	private bool TryGetAdjacentModuleBehind(out BaseVehicleModule result)
	{
		if (!this.IsOnAVehicle)
		{
			result = null;
			return false;
		}
		int num = this.FirstSocketIndex + this.GetNumSocketsTaken() - 1;
		return this.Vehicle.TryGetModuleAt(num + 1, out result);
	}

	// Token: 0x06000809 RID: 2057 RVA: 0x0004DD1C File Offset: 0x0004BF1C
	private bool InSameVisualGroupAs(BaseVehicleModule moduleEntity, ConditionalObject.AdjacentMatchType matchType)
	{
		if (moduleEntity == null)
		{
			return false;
		}
		if (this.visualGroup == BaseVehicleModule.VisualGroup.None)
		{
			return matchType != ConditionalObject.AdjacentMatchType.GroupNotExact && moduleEntity.prefabID == this.prefabID;
		}
		switch (matchType)
		{
		case ConditionalObject.AdjacentMatchType.GroupOrExact:
			return moduleEntity.prefabID == this.prefabID || moduleEntity.visualGroup == this.visualGroup;
		case ConditionalObject.AdjacentMatchType.ExactOnly:
			return moduleEntity.prefabID == this.prefabID;
		case ConditionalObject.AdjacentMatchType.GroupNotExact:
			return moduleEntity.prefabID != this.prefabID && moduleEntity.visualGroup == this.visualGroup;
		default:
			return false;
		}
	}

	// Token: 0x0600080A RID: 2058 RVA: 0x0004DDB4 File Offset: 0x0004BFB4
	private bool CanBeUsedNowBy(global::BasePlayer player)
	{
		return this.IsOnAVehicle && !(player == null) && !this.Vehicle.IsEditableNow && !this.Vehicle.IsDead() && this.Vehicle.PlayerIsMounted(player) && this.Vehicle.PlayerCanUseThis(player, ModularCarCodeLock.LockType.General);
	}

	// Token: 0x0600080B RID: 2059 RVA: 0x0004DE0B File Offset: 0x0004C00B
	public bool PlayerIsLookingAtUsable(string lookingAtColldierName, string usableColliderName)
	{
		return string.Compare(lookingAtColldierName, usableColliderName, true) == 0;
	}

	// Token: 0x0600080C RID: 2060 RVA: 0x0004DE18 File Offset: 0x0004C018
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}

	// Token: 0x0600080D RID: 2061 RVA: 0x00007074 File Offset: 0x00005274
	public override bool IsVehicleRoot()
	{
		return false;
	}

	// Token: 0x04000545 RID: 1349
	public global::Item AssociatedItemInstance;

	// Token: 0x04000547 RID: 1351
	private TimeSince timeSinceItemLockRefresh;

	// Token: 0x04000548 RID: 1352
	private const float TIME_BETWEEN_LOCK_REFRESH = 1f;

	// Token: 0x04000549 RID: 1353
	[Header("Vehicle Module")]
	[SerializeField]
	private Transform centreOfMassTransform;

	// Token: 0x0400054A RID: 1354
	[SerializeField]
	private float mass = 100f;

	// Token: 0x0400054B RID: 1355
	public BaseVehicleModule.VisualGroup visualGroup;

	// Token: 0x0400054C RID: 1356
	[SerializeField]
	[HideInInspector]
	private VehicleLight[] lights;

	// Token: 0x0400054F RID: 1359
	public BaseVehicleModule.LODLevel[] lodRenderers;

	// Token: 0x04000550 RID: 1360
	[SerializeField]
	[HideInInspector]
	private List<ConditionalObject> conditionals;

	// Token: 0x04000551 RID: 1361
	[Header("Trigger Parent")]
	[SerializeField]
	private TriggerParent[] triggerParents;

	// Token: 0x04000552 RID: 1362
	[Header("Sliding Components")]
	[SerializeField]
	private VehicleModuleSlidingComponent[] slidingComponents;

	// Token: 0x04000553 RID: 1363
	[SerializeField]
	private VehicleModuleButtonComponent[] buttonComponents;

	// Token: 0x04000554 RID: 1364
	private TimeSince TimeSinceAddedToVehicle;

	// Token: 0x04000555 RID: 1365
	private float prevRefreshHealth = -1f;

	// Token: 0x04000556 RID: 1366
	private bool prevRefreshVehicleIsDead;

	// Token: 0x04000557 RID: 1367
	private bool prevRefreshVehicleIsLockable;

	// Token: 0x02000B77 RID: 2935
	public enum VisualGroup
	{
		// Token: 0x04003E7F RID: 15999
		None,
		// Token: 0x04003E80 RID: 16000
		Engine,
		// Token: 0x04003E81 RID: 16001
		Cabin,
		// Token: 0x04003E82 RID: 16002
		Flatbed
	}

	// Token: 0x02000B78 RID: 2936
	[Serializable]
	public class LODLevel
	{
		// Token: 0x04003E83 RID: 16003
		public Renderer[] renderers;
	}
}
