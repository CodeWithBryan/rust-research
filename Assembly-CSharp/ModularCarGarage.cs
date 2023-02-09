using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200009C RID: 156
public class ModularCarGarage : ContainerIOEntity
{
	// Token: 0x06000E55 RID: 3669 RVA: 0x00077E28 File Offset: 0x00076028
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ModularCarGarage.OnRpcMessage", 0))
		{
			if (rpc == 554177909U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_DeselectedLootItem ");
				}
				using (TimeWarning.New("RPC_DeselectedLootItem", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(554177909U, "RPC_DeselectedLootItem", this, player, 3f))
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
							this.RPC_DeselectedLootItem(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_DeselectedLootItem");
					}
				}
				return true;
			}
			if (rpc == 3683966290U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_DiedWithKeypadOpen ");
				}
				using (TimeWarning.New("RPC_DiedWithKeypadOpen", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3683966290U, "RPC_DiedWithKeypadOpen", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3683966290U, "RPC_DiedWithKeypadOpen", this, player, 3f))
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
							this.RPC_DiedWithKeypadOpen(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_DiedWithKeypadOpen");
					}
				}
				return true;
			}
			if (rpc == 3659332720U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenEditing ");
				}
				using (TimeWarning.New("RPC_OpenEditing", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3659332720U, "RPC_OpenEditing", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3659332720U, "RPC_OpenEditing", this, player, 3f))
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
							this.RPC_OpenEditing(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in RPC_OpenEditing");
					}
				}
				return true;
			}
			if (rpc == 1582295101U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_RepairItem ");
				}
				using (TimeWarning.New("RPC_RepairItem", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1582295101U, "RPC_RepairItem", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1582295101U, "RPC_RepairItem", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_RepairItem(msg5);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in RPC_RepairItem");
					}
				}
				return true;
			}
			if (rpc == 3710764312U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_RequestAddLock ");
				}
				using (TimeWarning.New("RPC_RequestAddLock", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3710764312U, "RPC_RequestAddLock", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3710764312U, "RPC_RequestAddLock", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg6 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_RequestAddLock(msg6);
						}
					}
					catch (Exception exception5)
					{
						Debug.LogException(exception5);
						player.Kick("RPC Error in RPC_RequestAddLock");
					}
				}
				return true;
			}
			if (rpc == 3305106830U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_RequestNewCode ");
				}
				using (TimeWarning.New("RPC_RequestNewCode", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3305106830U, "RPC_RequestNewCode", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3305106830U, "RPC_RequestNewCode", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg7 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_RequestNewCode(msg7);
						}
					}
					catch (Exception exception6)
					{
						Debug.LogException(exception6);
						player.Kick("RPC Error in RPC_RequestNewCode");
					}
				}
				return true;
			}
			if (rpc == 1046853419U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_RequestRemoveLock ");
				}
				using (TimeWarning.New("RPC_RequestRemoveLock", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1046853419U, "RPC_RequestRemoveLock", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1046853419U, "RPC_RequestRemoveLock", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg8 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_RequestRemoveLock(msg8);
						}
					}
					catch (Exception exception7)
					{
						Debug.LogException(exception7);
						player.Kick("RPC Error in RPC_RequestRemoveLock");
					}
				}
				return true;
			}
			if (rpc == 4033916654U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_SelectedLootItem ");
				}
				using (TimeWarning.New("RPC_SelectedLootItem", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(4033916654U, "RPC_SelectedLootItem", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg9 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_SelectedLootItem(msg9);
						}
					}
					catch (Exception exception8)
					{
						Debug.LogException(exception8);
						player.Kick("RPC Error in RPC_SelectedLootItem");
					}
				}
				return true;
			}
			if (rpc == 2974124904U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_StartDestroyingChassis ");
				}
				using (TimeWarning.New("RPC_StartDestroyingChassis", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2974124904U, "RPC_StartDestroyingChassis", this, player, 1UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2974124904U, "RPC_StartDestroyingChassis", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2974124904U, "RPC_StartDestroyingChassis", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg10 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_StartDestroyingChassis(msg10);
						}
					}
					catch (Exception exception9)
					{
						Debug.LogException(exception9);
						player.Kick("RPC Error in RPC_StartDestroyingChassis");
					}
				}
				return true;
			}
			if (rpc == 3872977075U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_StartKeycodeEntry ");
				}
				using (TimeWarning.New("RPC_StartKeycodeEntry", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3872977075U, "RPC_StartKeycodeEntry", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg11 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_StartKeycodeEntry(msg11);
						}
					}
					catch (Exception exception10)
					{
						Debug.LogException(exception10);
						player.Kick("RPC Error in RPC_StartKeycodeEntry");
					}
				}
				return true;
			}
			if (rpc == 3830531963U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_StopDestroyingChassis ");
				}
				using (TimeWarning.New("RPC_StopDestroyingChassis", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3830531963U, "RPC_StopDestroyingChassis", this, player, 1UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3830531963U, "RPC_StopDestroyingChassis", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3830531963U, "RPC_StopDestroyingChassis", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg12 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_StopDestroyingChassis(msg12);
						}
					}
					catch (Exception exception11)
					{
						Debug.LogException(exception11);
						player.Kick("RPC Error in RPC_StopDestroyingChassis");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700013C RID: 316
	// (get) Token: 0x06000E56 RID: 3670 RVA: 0x00078E8C File Offset: 0x0007708C
	private global::ModularCar carOccupant
	{
		get
		{
			if (!(this.lockedOccupant != null))
			{
				return this.occupantTrigger.carOccupant;
			}
			return this.lockedOccupant;
		}
	}

	// Token: 0x1700013D RID: 317
	// (get) Token: 0x06000E57 RID: 3671 RVA: 0x00078EAE File Offset: 0x000770AE
	private bool HasOccupant
	{
		get
		{
			return this.carOccupant != null && this.carOccupant.IsFullySpawned();
		}
	}

	// Token: 0x06000E58 RID: 3672 RVA: 0x00078ECC File Offset: 0x000770CC
	protected void FixedUpdate()
	{
		if (!base.isServer)
		{
			return;
		}
		if (this.magnetSnap == null)
		{
			return;
		}
		if (this.playerTrigger != null)
		{
			bool hasAnyContents = this.playerTrigger.HasAnyContents;
			if (this.PlayerObstructingLift != hasAnyContents)
			{
				base.SetFlag(global::BaseEntity.Flags.Reserved8, hasAnyContents, false, true);
			}
		}
		this.UpdateCarOccupant();
		if (this.HasOccupant && this.carOccupant.CouldBeEdited() && this.carOccupant.GetSpeed() <= 1f)
		{
			if (base.IsOn() || !this.carOccupant.IsComplete())
			{
				if (this.lockedOccupant == null)
				{
					this.GrabOccupant(this.occupantTrigger.carOccupant);
				}
				this.magnetSnap.FixedUpdate(this.carOccupant.transform);
			}
			if (this.carOccupant.CarLock.HasALock && !this.carOccupant.CarLock.CanHaveALock())
			{
				this.carOccupant.CarLock.RemoveLock();
			}
		}
		else if (this.HasOccupant && this.carOccupant.rigidBody.isKinematic)
		{
			this.ReleaseOccupant();
		}
		if (this.HasOccupant && this.IsDestroyingChassis && this.carOccupant.HasAnyModules)
		{
			this.StopChassisDestroy();
		}
	}

	// Token: 0x06000E59 RID: 3673 RVA: 0x00079015 File Offset: 0x00077215
	internal override void DoServerDestroy()
	{
		if (this.HasOccupant)
		{
			this.ReleaseOccupant();
			if (!this.HasDriveableOccupant)
			{
				this.carOccupant.Kill(global::BaseNetworkable.DestroyMode.Gib);
			}
		}
		base.DoServerDestroy();
	}

	// Token: 0x06000E5A RID: 3674 RVA: 0x0007903F File Offset: 0x0007723F
	public override void ServerInit()
	{
		base.ServerInit();
		this.magnetSnap = new MagnetSnap(this.vehicleLiftPos);
		this.RefreshOnOffState();
		this.SetOccupantState(false, false, false, ModularCarGarage.OccupantLock.CannotHaveLock, true);
		this.RefreshLiftState(true);
	}

	// Token: 0x06000E5B RID: 3675 RVA: 0x00079070 File Offset: 0x00077270
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.vehicleLift = Facepunch.Pool.Get<VehicleLift>();
		info.msg.vehicleLift.platformIsOccupied = this.PlatformIsOccupied;
		info.msg.vehicleLift.editableOccupant = this.HasEditableOccupant;
		info.msg.vehicleLift.driveableOccupant = this.HasDriveableOccupant;
		info.msg.vehicleLift.occupantLockState = (int)this.OccupantLockState;
	}

	// Token: 0x06000E5C RID: 3676 RVA: 0x00007074 File Offset: 0x00005274
	public override uint GetIdealContainer(global::BasePlayer player, global::Item item)
	{
		return 0U;
	}

	// Token: 0x06000E5D RID: 3677 RVA: 0x000790EC File Offset: 0x000772EC
	public override bool PlayerOpenLoot(global::BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved7, false, false, true);
		if (player == null)
		{
			return false;
		}
		bool flag = base.PlayerOpenLoot(player, panelToOpen, true);
		if (!flag)
		{
			return false;
		}
		if (this.HasEditableOccupant)
		{
			player.inventory.loot.AddContainer(this.carOccupant.Inventory.ModuleContainer);
			player.inventory.loot.AddContainer(this.carOccupant.Inventory.ChassisContainer);
			player.inventory.loot.SendImmediate();
		}
		this.lootingPlayers.Add(player);
		this.RefreshLiftState(false);
		return flag;
	}

	// Token: 0x06000E5E RID: 3678 RVA: 0x0007918E File Offset: 0x0007738E
	public override void PlayerStoppedLooting(global::BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		if (!this.IsEnteringKeycode)
		{
			this.lootingPlayers.Remove(player);
			this.RefreshLiftState(false);
		}
	}

	// Token: 0x06000E5F RID: 3679 RVA: 0x000791B3 File Offset: 0x000773B3
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		this.RefreshOnOffState();
	}

	// Token: 0x06000E60 RID: 3680 RVA: 0x000791C3 File Offset: 0x000773C3
	public bool TryGetModuleForItem(global::Item item, out BaseVehicleModule result)
	{
		if (!this.HasOccupant)
		{
			result = null;
			return false;
		}
		result = this.carOccupant.GetModuleForItem(item);
		return result != null;
	}

	// Token: 0x06000E61 RID: 3681 RVA: 0x000791E8 File Offset: 0x000773E8
	private void RefreshOnOffState()
	{
		bool flag = !this.needsElectricity || this.currentEnergy >= this.ConsumptionAmount();
		if (flag != base.IsOn())
		{
			base.SetFlag(global::BaseEntity.Flags.On, flag, false, true);
		}
	}

	// Token: 0x06000E62 RID: 3682 RVA: 0x00079228 File Offset: 0x00077428
	private void UpdateCarOccupant()
	{
		if (!base.isServer)
		{
			return;
		}
		if (this.HasOccupant)
		{
			bool editableOccupant = Vector3.SqrMagnitude(this.carOccupant.transform.position - this.vehicleLiftPos.position) < 1f && this.carOccupant.CouldBeEdited() && !this.PlayerObstructingLift;
			bool driveableOccupant = this.carOccupant.IsComplete();
			ModularCarGarage.OccupantLock occupantLockState;
			if (this.carOccupant.CarLock.CanHaveALock())
			{
				if (this.carOccupant.CarLock.HasALock)
				{
					occupantLockState = ModularCarGarage.OccupantLock.HasLock;
				}
				else
				{
					occupantLockState = ModularCarGarage.OccupantLock.NoLock;
				}
			}
			else
			{
				occupantLockState = ModularCarGarage.OccupantLock.CannotHaveLock;
			}
			this.SetOccupantState(this.HasOccupant, editableOccupant, driveableOccupant, occupantLockState, false);
			return;
		}
		this.SetOccupantState(false, false, false, ModularCarGarage.OccupantLock.CannotHaveLock, false);
	}

	// Token: 0x06000E63 RID: 3683 RVA: 0x000792E7 File Offset: 0x000774E7
	private void UpdateOccupantMode()
	{
		if (!this.HasOccupant)
		{
			return;
		}
		this.carOccupant.inEditableLocation = (this.HasEditableOccupant && this.LiftIsUp);
		this.carOccupant.immuneToDecay = base.IsOn();
	}

	// Token: 0x06000E64 RID: 3684 RVA: 0x00079320 File Offset: 0x00077520
	private void WakeNearbyRigidbodies()
	{
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		global::Vis.Colliders<Collider>(base.transform.position, 7f, list, 34816, QueryTriggerInteraction.Collide);
		foreach (Collider collider in list)
		{
			Rigidbody attachedRigidbody = collider.attachedRigidbody;
			if (attachedRigidbody != null && attachedRigidbody.IsSleeping())
			{
				attachedRigidbody.WakeUp();
			}
			global::BaseEntity baseEntity = collider.ToBaseEntity();
			BaseRidableAnimal baseRidableAnimal;
			if (baseEntity != null && (baseRidableAnimal = (baseEntity as BaseRidableAnimal)) != null && baseRidableAnimal.isServer)
			{
				baseRidableAnimal.UpdateDropToGroundForDuration(2f);
			}
		}
		Facepunch.Pool.FreeList<Collider>(ref list);
	}

	// Token: 0x06000E65 RID: 3685 RVA: 0x000793DC File Offset: 0x000775DC
	private void EditableOccupantEntered()
	{
		this.RefreshLoot();
	}

	// Token: 0x06000E66 RID: 3686 RVA: 0x000793DC File Offset: 0x000775DC
	private void EditableOccupantLeft()
	{
		this.RefreshLoot();
	}

	// Token: 0x06000E67 RID: 3687 RVA: 0x000793E4 File Offset: 0x000775E4
	private void RefreshLoot()
	{
		List<global::BasePlayer> list = Facepunch.Pool.GetList<global::BasePlayer>();
		list.AddRange(this.lootingPlayers);
		foreach (global::BasePlayer basePlayer in list)
		{
			basePlayer.inventory.loot.Clear();
			this.PlayerOpenLoot(basePlayer, "", true);
		}
		Facepunch.Pool.FreeList<global::BasePlayer>(ref list);
	}

	// Token: 0x06000E68 RID: 3688 RVA: 0x00079464 File Offset: 0x00077664
	private void GrabOccupant(global::ModularCar occupant)
	{
		if (occupant == null)
		{
			return;
		}
		this.lockedOccupant = occupant;
		this.lockedOccupant.DisablePhysics();
	}

	// Token: 0x06000E69 RID: 3689 RVA: 0x00079484 File Offset: 0x00077684
	private void ReleaseOccupant()
	{
		if (!this.HasOccupant)
		{
			return;
		}
		this.carOccupant.inEditableLocation = false;
		this.carOccupant.immuneToDecay = false;
		if (this.lockedOccupant != null)
		{
			this.lockedOccupant.EnablePhysics();
			this.lockedOccupant = null;
		}
	}

	// Token: 0x06000E6A RID: 3690 RVA: 0x000794D2 File Offset: 0x000776D2
	private void StopChassisDestroy()
	{
		if (base.IsInvoking(new Action(this.FinishDestroyingChassis)))
		{
			base.CancelInvoke(new Action(this.FinishDestroyingChassis));
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved6, false, false, true);
	}

	// Token: 0x06000E6B RID: 3691 RVA: 0x00079508 File Offset: 0x00077708
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_RepairItem(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		uint num = msg.read.UInt32();
		if (player == null || !this.HasOccupant)
		{
			return;
		}
		global::Item vehicleItem = this.carOccupant.GetVehicleItem(num);
		if (vehicleItem != null)
		{
			RepairBench.RepairAnItem(vehicleItem, player, this, 0f, false);
			Effect.server.Run(this.repairEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
			return;
		}
		Debug.LogError(base.GetType().Name + ": Couldn't get item to repair, with ID: " + num);
	}

	// Token: 0x06000E6C RID: 3692 RVA: 0x00079598 File Offset: 0x00077798
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_OpenEditing(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null || this.LiftIsMoving)
		{
			return;
		}
		this.PlayerOpenLoot(player, "", true);
	}

	// Token: 0x06000E6D RID: 3693 RVA: 0x000795CC File Offset: 0x000777CC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_DiedWithKeypadOpen(global::BaseEntity.RPCMessage msg)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved7, false, false, true);
		this.lootingPlayers.Clear();
		this.RefreshLiftState(false);
	}

	// Token: 0x06000E6E RID: 3694 RVA: 0x000795F0 File Offset: 0x000777F0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_SelectedLootItem(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		uint itemUID = msg.read.UInt32();
		if (player == null || !player.inventory.loot.IsLooting() || player.inventory.loot.entitySource != this)
		{
			return;
		}
		if (!this.HasOccupant)
		{
			return;
		}
		global::Item vehicleItem = this.carOccupant.GetVehicleItem(itemUID);
		if (vehicleItem != null)
		{
			bool flag = player.inventory.loot.RemoveContainerAt(3);
			BaseVehicleModule baseVehicleModule;
			if (this.TryGetModuleForItem(vehicleItem, out baseVehicleModule))
			{
				VehicleModuleStorage vehicleModuleStorage;
				VehicleModuleCamper vehicleModuleCamper;
				if ((vehicleModuleStorage = (baseVehicleModule as VehicleModuleStorage)) != null)
				{
					IItemContainerEntity container = vehicleModuleStorage.GetContainer();
					if (!container.IsUnityNull<IItemContainerEntity>())
					{
						player.inventory.loot.AddContainer(container.inventory);
						flag = true;
					}
				}
				else if ((vehicleModuleCamper = (baseVehicleModule as VehicleModuleCamper)) != null)
				{
					IItemContainerEntity container2 = vehicleModuleCamper.GetContainer();
					if (!container2.IsUnityNull<IItemContainerEntity>())
					{
						player.inventory.loot.AddContainer(container2.inventory);
						flag = true;
					}
				}
			}
			if (flag)
			{
				player.inventory.loot.SendImmediate();
			}
		}
	}

	// Token: 0x06000E6F RID: 3695 RVA: 0x00079700 File Offset: 0x00077900
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_DeselectedLootItem(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!player.inventory.loot.IsLooting() || player.inventory.loot.entitySource != this)
		{
			return;
		}
		if (player.inventory.loot.RemoveContainerAt(3))
		{
			player.inventory.loot.SendImmediate();
		}
	}

	// Token: 0x06000E70 RID: 3696 RVA: 0x00079762 File Offset: 0x00077962
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_StartKeycodeEntry(global::BaseEntity.RPCMessage msg)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved7, true, false, true);
	}

	// Token: 0x06000E71 RID: 3697 RVA: 0x00079774 File Offset: 0x00077974
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_RequestAddLock(global::BaseEntity.RPCMessage msg)
	{
		if (!this.HasOccupant)
		{
			return;
		}
		if (this.carOccupant.CarLock.HasALock)
		{
			return;
		}
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		string code = msg.read.String(256);
		ItemAmount itemAmount = this.lockResourceCost;
		if ((float)player.inventory.GetAmount(itemAmount.itemDef.itemid) >= itemAmount.amount && this.carOccupant.CarLock.TryAddALock(code, player.userID))
		{
			player.inventory.Take(null, itemAmount.itemDef.itemid, Mathf.CeilToInt(itemAmount.amount));
			Effect.server.Run(this.addRemoveLockEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
		}
	}

	// Token: 0x06000E72 RID: 3698 RVA: 0x00079840 File Offset: 0x00077A40
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_RequestRemoveLock(global::BaseEntity.RPCMessage msg)
	{
		if (!this.HasOccupant)
		{
			return;
		}
		if (!this.carOccupant.CarLock.HasALock)
		{
			return;
		}
		this.carOccupant.CarLock.RemoveLock();
		Effect.server.Run(this.addRemoveLockEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
	}

	// Token: 0x06000E73 RID: 3699 RVA: 0x00079898 File Offset: 0x00077A98
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_RequestNewCode(global::BaseEntity.RPCMessage msg)
	{
		if (!this.HasOccupant)
		{
			return;
		}
		if (!this.carOccupant.CarLock.HasALock)
		{
			return;
		}
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		string newCode = msg.read.String(256);
		if (this.carOccupant.CarLock.TrySetNewCode(newCode, player.userID))
		{
			Effect.server.Run(this.changeLockCodeEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
		}
	}

	// Token: 0x06000E74 RID: 3700 RVA: 0x0007991A File Offset: 0x00077B1A
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void RPC_StartDestroyingChassis(global::BaseEntity.RPCMessage msg)
	{
		if (this.carOccupant.HasAnyModules)
		{
			return;
		}
		base.Invoke(new Action(this.FinishDestroyingChassis), 10f);
		base.SetFlag(global::BaseEntity.Flags.Reserved6, true, false, true);
	}

	// Token: 0x06000E75 RID: 3701 RVA: 0x0007994F File Offset: 0x00077B4F
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void RPC_StopDestroyingChassis(global::BaseEntity.RPCMessage msg)
	{
		this.StopChassisDestroy();
	}

	// Token: 0x06000E76 RID: 3702 RVA: 0x00079957 File Offset: 0x00077B57
	private void FinishDestroyingChassis()
	{
		if (!this.HasOccupant)
		{
			return;
		}
		if (this.carOccupant.HasAnyModules)
		{
			return;
		}
		this.carOccupant.Kill(global::BaseNetworkable.DestroyMode.Gib);
		base.SetFlag(global::BaseEntity.Flags.Reserved6, false, false, true);
	}

	// Token: 0x1700013E RID: 318
	// (get) Token: 0x06000E77 RID: 3703 RVA: 0x0007998A File Offset: 0x00077B8A
	// (set) Token: 0x06000E78 RID: 3704 RVA: 0x00079992 File Offset: 0x00077B92
	public bool PlatformIsOccupied { get; private set; }

	// Token: 0x1700013F RID: 319
	// (get) Token: 0x06000E79 RID: 3705 RVA: 0x0007999B File Offset: 0x00077B9B
	// (set) Token: 0x06000E7A RID: 3706 RVA: 0x000799A3 File Offset: 0x00077BA3
	public bool HasEditableOccupant { get; private set; }

	// Token: 0x17000140 RID: 320
	// (get) Token: 0x06000E7B RID: 3707 RVA: 0x000799AC File Offset: 0x00077BAC
	// (set) Token: 0x06000E7C RID: 3708 RVA: 0x000799B4 File Offset: 0x00077BB4
	public bool HasDriveableOccupant { get; private set; }

	// Token: 0x17000141 RID: 321
	// (get) Token: 0x06000E7D RID: 3709 RVA: 0x000799BD File Offset: 0x00077BBD
	// (set) Token: 0x06000E7E RID: 3710 RVA: 0x000799C5 File Offset: 0x00077BC5
	public ModularCarGarage.OccupantLock OccupantLockState { get; private set; }

	// Token: 0x17000142 RID: 322
	// (get) Token: 0x06000E7F RID: 3711 RVA: 0x000799CE File Offset: 0x00077BCE
	private bool LiftIsUp
	{
		get
		{
			return this.vehicleLiftState == ModularCarGarage.VehicleLiftState.Up;
		}
	}

	// Token: 0x17000143 RID: 323
	// (get) Token: 0x06000E80 RID: 3712 RVA: 0x000799D9 File Offset: 0x00077BD9
	private bool LiftIsMoving
	{
		get
		{
			return this.vehicleLiftAnim.isPlaying;
		}
	}

	// Token: 0x17000144 RID: 324
	// (get) Token: 0x06000E81 RID: 3713 RVA: 0x000799E6 File Offset: 0x00077BE6
	private bool LiftIsDown
	{
		get
		{
			return this.vehicleLiftState == ModularCarGarage.VehicleLiftState.Down;
		}
	}

	// Token: 0x17000145 RID: 325
	// (get) Token: 0x06000E82 RID: 3714 RVA: 0x000035EB File Offset: 0x000017EB
	public bool IsDestroyingChassis
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved6);
		}
	}

	// Token: 0x17000146 RID: 326
	// (get) Token: 0x06000E83 RID: 3715 RVA: 0x00047C77 File Offset: 0x00045E77
	private bool IsEnteringKeycode
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved7);
		}
	}

	// Token: 0x17000147 RID: 327
	// (get) Token: 0x06000E84 RID: 3716 RVA: 0x000028C8 File Offset: 0x00000AC8
	public bool PlayerObstructingLift
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved8);
		}
	}

	// Token: 0x06000E85 RID: 3717 RVA: 0x000799F1 File Offset: 0x00077BF1
	public override void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.downPos = this.vehicleLift.transform.position;
	}

	// Token: 0x06000E86 RID: 3718 RVA: 0x00079A09 File Offset: 0x00077C09
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer)
		{
			this.UpdateOccupantMode();
		}
	}

	// Token: 0x06000E87 RID: 3719 RVA: 0x00006C79 File Offset: 0x00004E79
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return base.IsOn();
	}

	// Token: 0x06000E88 RID: 3720 RVA: 0x0001F1CE File Offset: 0x0001D3CE
	public override int ConsumptionAmount()
	{
		return 5;
	}

	// Token: 0x06000E89 RID: 3721 RVA: 0x00079A24 File Offset: 0x00077C24
	private void SetOccupantState(bool hasOccupant, bool editableOccupant, bool driveableOccupant, ModularCarGarage.OccupantLock occupantLockState, bool forced = false)
	{
		if (this.PlatformIsOccupied == hasOccupant && this.HasEditableOccupant == editableOccupant && this.HasDriveableOccupant == driveableOccupant && this.OccupantLockState == occupantLockState && !forced)
		{
			return;
		}
		bool hasEditableOccupant = this.HasEditableOccupant;
		this.PlatformIsOccupied = hasOccupant;
		this.HasEditableOccupant = editableOccupant;
		this.HasDriveableOccupant = driveableOccupant;
		this.OccupantLockState = occupantLockState;
		if (base.isServer)
		{
			this.UpdateOccupantMode();
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			if (hasEditableOccupant && !editableOccupant)
			{
				this.EditableOccupantLeft();
			}
			else if (editableOccupant && !hasEditableOccupant)
			{
				this.EditableOccupantEntered();
			}
		}
		this.RefreshLiftState(false);
	}

	// Token: 0x06000E8A RID: 3722 RVA: 0x00079AB8 File Offset: 0x00077CB8
	private void RefreshLiftState(bool forced = false)
	{
		ModularCarGarage.VehicleLiftState desiredLiftState = (base.IsOpen() || this.IsEnteringKeycode || (this.HasEditableOccupant && !this.HasDriveableOccupant)) ? ModularCarGarage.VehicleLiftState.Up : ModularCarGarage.VehicleLiftState.Down;
		this.MoveLift(desiredLiftState, 0f, forced);
	}

	// Token: 0x06000E8B RID: 3723 RVA: 0x00079B00 File Offset: 0x00077D00
	private void MoveLift(ModularCarGarage.VehicleLiftState desiredLiftState, float startDelay = 0f, bool forced = false)
	{
		if (this.vehicleLiftState == desiredLiftState && !forced)
		{
			return;
		}
		ModularCarGarage.VehicleLiftState vehicleLiftState = this.vehicleLiftState;
		this.vehicleLiftState = desiredLiftState;
		if (base.isServer)
		{
			this.UpdateOccupantMode();
			this.WakeNearbyRigidbodies();
		}
		if (!base.gameObject.activeSelf)
		{
			this.vehicleLiftAnim[this.animName].time = ((desiredLiftState == ModularCarGarage.VehicleLiftState.Up) ? 1f : 0f);
			this.vehicleLiftAnim.Play();
			return;
		}
		if (desiredLiftState == ModularCarGarage.VehicleLiftState.Up)
		{
			base.Invoke(new Action(this.MoveLiftUp), startDelay);
			return;
		}
		base.Invoke(new Action(this.MoveLiftDown), startDelay);
	}

	// Token: 0x06000E8C RID: 3724 RVA: 0x00079BA7 File Offset: 0x00077DA7
	private void MoveLiftUp()
	{
		this.vehicleLiftAnim[this.animName].length /= this.liftMoveTime;
		this.vehicleLiftAnim.Play();
	}

	// Token: 0x06000E8D RID: 3725 RVA: 0x00079BD8 File Offset: 0x00077DD8
	private void MoveLiftDown()
	{
		AnimationState animationState = this.vehicleLiftAnim[this.animName];
		animationState.speed = animationState.length / this.liftMoveTime;
		if (!this.vehicleLiftAnim.isPlaying && Vector3.Distance(this.vehicleLift.transform.position, this.downPos) > 0.01f)
		{
			animationState.time = 1f;
		}
		animationState.speed *= -1f;
		this.vehicleLiftAnim.Play();
	}

	// Token: 0x04000945 RID: 2373
	private global::ModularCar lockedOccupant;

	// Token: 0x04000946 RID: 2374
	private readonly HashSet<global::BasePlayer> lootingPlayers = new HashSet<global::BasePlayer>();

	// Token: 0x04000947 RID: 2375
	private MagnetSnap magnetSnap;

	// Token: 0x04000948 RID: 2376
	[SerializeField]
	private Transform vehicleLift;

	// Token: 0x04000949 RID: 2377
	[SerializeField]
	private Animation vehicleLiftAnim;

	// Token: 0x0400094A RID: 2378
	[SerializeField]
	private string animName = "LiftUp";

	// Token: 0x0400094B RID: 2379
	[SerializeField]
	private VehicleLiftOccupantTrigger occupantTrigger;

	// Token: 0x0400094C RID: 2380
	[SerializeField]
	private float liftMoveTime = 1f;

	// Token: 0x0400094D RID: 2381
	[SerializeField]
	private EmissionToggle poweredLight;

	// Token: 0x0400094E RID: 2382
	[SerializeField]
	private EmissionToggle inUseLight;

	// Token: 0x0400094F RID: 2383
	[SerializeField]
	private Transform vehicleLiftPos;

	// Token: 0x04000950 RID: 2384
	[SerializeField]
	[Range(0f, 1f)]
	private float recycleEfficiency = 0.5f;

	// Token: 0x04000951 RID: 2385
	[SerializeField]
	private Transform recycleDropPos;

	// Token: 0x04000952 RID: 2386
	[SerializeField]
	private bool needsElectricity;

	// Token: 0x04000953 RID: 2387
	[SerializeField]
	private SoundDefinition liftStartSoundDef;

	// Token: 0x04000954 RID: 2388
	[SerializeField]
	private SoundDefinition liftStopSoundDef;

	// Token: 0x04000955 RID: 2389
	[SerializeField]
	private SoundDefinition liftStopDownSoundDef;

	// Token: 0x04000956 RID: 2390
	[SerializeField]
	private SoundDefinition liftLoopSoundDef;

	// Token: 0x04000957 RID: 2391
	[SerializeField]
	private GameObjectRef addRemoveLockEffect;

	// Token: 0x04000958 RID: 2392
	[SerializeField]
	private GameObjectRef changeLockCodeEffect;

	// Token: 0x04000959 RID: 2393
	[SerializeField]
	private GameObjectRef repairEffect;

	// Token: 0x0400095A RID: 2394
	[SerializeField]
	private TriggerBase playerTrigger;

	// Token: 0x0400095B RID: 2395
	public ModularCarGarage.ChassisBuildOption[] chassisBuildOptions;

	// Token: 0x0400095C RID: 2396
	public ItemAmount lockResourceCost;

	// Token: 0x04000961 RID: 2401
	private ModularCarGarage.VehicleLiftState vehicleLiftState;

	// Token: 0x04000962 RID: 2402
	private Sound liftLoopSound;

	// Token: 0x04000963 RID: 2403
	private Vector3 downPos;

	// Token: 0x04000964 RID: 2404
	public const global::BaseEntity.Flags Flag_DestroyingChassis = global::BaseEntity.Flags.Reserved6;

	// Token: 0x04000965 RID: 2405
	public const float TimeToDestroyChassis = 10f;

	// Token: 0x04000966 RID: 2406
	public const global::BaseEntity.Flags Flag_EnteringKeycode = global::BaseEntity.Flags.Reserved7;

	// Token: 0x04000967 RID: 2407
	public const global::BaseEntity.Flags Flag_PlayerObstructing = global::BaseEntity.Flags.Reserved8;

	// Token: 0x02000B9E RID: 2974
	[Serializable]
	public class ChassisBuildOption
	{
		// Token: 0x04003EF3 RID: 16115
		public GameObjectRef prefab;

		// Token: 0x04003EF4 RID: 16116
		public ItemDefinition itemDef;
	}

	// Token: 0x02000B9F RID: 2975
	public enum OccupantLock
	{
		// Token: 0x04003EF6 RID: 16118
		CannotHaveLock,
		// Token: 0x04003EF7 RID: 16119
		NoLock,
		// Token: 0x04003EF8 RID: 16120
		HasLock
	}

	// Token: 0x02000BA0 RID: 2976
	private enum VehicleLiftState
	{
		// Token: 0x04003EFA RID: 16122
		Down,
		// Token: 0x04003EFB RID: 16123
		Up
	}
}
