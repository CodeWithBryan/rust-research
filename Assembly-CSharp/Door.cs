using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

// Token: 0x02000064 RID: 100
public class Door : AnimatedBuildingBlock, INotifyTrigger
{
	// Token: 0x06000A0D RID: 2573 RVA: 0x0005B958 File Offset: 0x00059B58
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Door.OnRpcMessage", 0))
		{
			if (rpc == 3999508679U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_CloseDoor ");
				}
				using (TimeWarning.New("RPC_CloseDoor", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(3999508679U, "RPC_CloseDoor", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpc2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_CloseDoor(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_CloseDoor");
					}
				}
				return true;
			}
			if (rpc == 1487779344U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_KnockDoor ");
				}
				using (TimeWarning.New("RPC_KnockDoor", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(1487779344U, "RPC_KnockDoor", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpc3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_KnockDoor(rpc3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_KnockDoor");
					}
				}
				return true;
			}
			if (rpc == 3314360565U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenDoor ");
				}
				using (TimeWarning.New("RPC_OpenDoor", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(3314360565U, "RPC_OpenDoor", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpc4 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_OpenDoor(rpc4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in RPC_OpenDoor");
					}
				}
				return true;
			}
			if (rpc == 3000490601U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_ToggleHatch ");
				}
				using (TimeWarning.New("RPC_ToggleHatch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(3000490601U, "RPC_ToggleHatch", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpc5 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_ToggleHatch(rpc5);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in RPC_ToggleHatch");
					}
				}
				return true;
			}
			if (rpc == 3672787865U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_NotifyWoundedClose ");
				}
				using (TimeWarning.New("Server_NotifyWoundedClose", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(3672787865U, "Server_NotifyWoundedClose", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_NotifyWoundedClose(msg2);
						}
					}
					catch (Exception exception5)
					{
						Debug.LogException(exception5);
						player.Kick("RPC Error in Server_NotifyWoundedClose");
					}
				}
				return true;
			}
			if (rpc == 3730851545U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_NotifyWoundedOpen ");
				}
				using (TimeWarning.New("Server_NotifyWoundedOpen", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(3730851545U, "Server_NotifyWoundedOpen", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_NotifyWoundedOpen(msg3);
						}
					}
					catch (Exception exception6)
					{
						Debug.LogException(exception6);
						player.Kick("RPC Error in Server_NotifyWoundedOpen");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A0E RID: 2574 RVA: 0x0005C1C4 File Offset: 0x0005A3C4
	public override void ResetState()
	{
		base.ResetState();
		if (base.isServer)
		{
			this.decayResetTimeLast = float.NegativeInfinity;
			if (this.isSecurityDoor && this.NavMeshLink != null)
			{
				this.SetNavMeshLinkEnabled(false);
			}
			this.woundedCloses.Clear();
			this.woundedOpens.Clear();
		}
	}

	// Token: 0x06000A0F RID: 2575 RVA: 0x0005C220 File Offset: 0x0005A420
	public override void ServerInit()
	{
		base.ServerInit();
		if (Door.nonWalkableArea < 0)
		{
			Door.nonWalkableArea = NavMesh.GetAreaFromName("Not Walkable");
		}
		if (Door.animalAgentTypeId < 0)
		{
			Door.animalAgentTypeId = NavMesh.GetSettingsByIndex(1).agentTypeID;
		}
		if (this.NavMeshVolumeAnimals == null)
		{
			this.NavMeshVolumeAnimals = base.gameObject.AddComponent<NavMeshModifierVolume>();
			this.NavMeshVolumeAnimals.area = Door.nonWalkableArea;
			this.NavMeshVolumeAnimals.AddAgentType(Door.animalAgentTypeId);
			this.NavMeshVolumeAnimals.center = Vector3.zero;
			this.NavMeshVolumeAnimals.size = Vector3.one;
		}
		if (this.HasSlot(BaseEntity.Slot.Lock))
		{
			this.canNpcOpen = false;
		}
		if (!this.canNpcOpen)
		{
			if (Door.humanoidAgentTypeId < 0)
			{
				Door.humanoidAgentTypeId = NavMesh.GetSettingsByIndex(0).agentTypeID;
			}
			if (this.NavMeshVolumeHumanoids == null)
			{
				this.NavMeshVolumeHumanoids = base.gameObject.AddComponent<NavMeshModifierVolume>();
				this.NavMeshVolumeHumanoids.area = Door.nonWalkableArea;
				this.NavMeshVolumeHumanoids.AddAgentType(Door.humanoidAgentTypeId);
				this.NavMeshVolumeHumanoids.center = Vector3.zero;
				this.NavMeshVolumeHumanoids.size = Vector3.one + Vector3.up + Vector3.forward;
			}
		}
		else if (this.NpcTriggerBox == null)
		{
			if (this.isSecurityDoor)
			{
				NavMeshObstacle navMeshObstacle = base.gameObject.AddComponent<NavMeshObstacle>();
				navMeshObstacle.carving = true;
				navMeshObstacle.center = Vector3.zero;
				navMeshObstacle.size = Vector3.one;
				navMeshObstacle.shape = NavMeshObstacleShape.Box;
			}
			this.NpcTriggerBox = new GameObject("NpcTriggerBox").AddComponent<NPCDoorTriggerBox>();
			this.NpcTriggerBox.Setup(this);
		}
		AIInformationZone forPoint = AIInformationZone.GetForPoint(base.transform.position, true);
		if (forPoint != null && this.NavMeshLink == null)
		{
			this.NavMeshLink = forPoint.GetClosestNavMeshLink(base.transform.position);
		}
		this.DisableVehiclePhysBox();
	}

	// Token: 0x06000A10 RID: 2576 RVA: 0x0005C41D File Offset: 0x0005A61D
	public override bool HasSlot(BaseEntity.Slot slot)
	{
		return (slot == BaseEntity.Slot.Lock && this.canTakeLock) || slot == BaseEntity.Slot.UpperModifier || (slot == BaseEntity.Slot.CenterDecoration && this.canTakeCloser) || (slot == BaseEntity.Slot.LowerCenterDecoration && this.canTakeKnocker) || base.HasSlot(slot);
	}

	// Token: 0x06000A11 RID: 2577 RVA: 0x0005C458 File Offset: 0x0005A658
	public override bool CanPickup(BasePlayer player)
	{
		return base.IsOpen() && !base.GetSlot(BaseEntity.Slot.Lock) && !base.GetSlot(BaseEntity.Slot.UpperModifier) && !base.GetSlot(BaseEntity.Slot.CenterDecoration) && !base.GetSlot(BaseEntity.Slot.LowerCenterDecoration) && base.CanPickup(player);
	}

	// Token: 0x06000A12 RID: 2578 RVA: 0x0005C4B6 File Offset: 0x0005A6B6
	public void CloseRequest()
	{
		this.SetOpen(false, false);
	}

	// Token: 0x06000A13 RID: 2579 RVA: 0x0005C4C0 File Offset: 0x0005A6C0
	public void SetOpen(bool open, bool suppressBlockageChecks = false)
	{
		base.SetFlag(BaseEntity.Flags.Open, open, false, true);
		base.SendNetworkUpdateImmediate(false);
		if (this.isSecurityDoor && this.NavMeshLink != null)
		{
			this.SetNavMeshLinkEnabled(open);
		}
		if (!suppressBlockageChecks && (!open || this.checkPhysBoxesOnOpen))
		{
			this.StartCheckingForBlockages();
		}
	}

	// Token: 0x06000A14 RID: 2580 RVA: 0x0005C515 File Offset: 0x0005A715
	public void SetLocked(bool locked)
	{
		base.SetFlag(BaseEntity.Flags.Locked, false, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000A15 RID: 2581 RVA: 0x0005C52C File Offset: 0x0005A72C
	public bool GetPlayerLockPermission(BasePlayer player)
	{
		BaseLock baseLock = base.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
		return baseLock == null || baseLock.GetPlayerLockPermission(player);
	}

	// Token: 0x06000A16 RID: 2582 RVA: 0x0005C558 File Offset: 0x0005A758
	public void SetNavMeshLinkEnabled(bool wantsOn)
	{
		if (this.NavMeshLink != null)
		{
			if (wantsOn)
			{
				this.NavMeshLink.gameObject.SetActive(true);
				this.NavMeshLink.enabled = true;
				return;
			}
			this.NavMeshLink.enabled = false;
			this.NavMeshLink.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000A17 RID: 2583 RVA: 0x0005C5B4 File Offset: 0x0005A7B4
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_OpenDoor(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract(true))
		{
			return;
		}
		if (!this.canHandOpen)
		{
			return;
		}
		if (base.IsOpen())
		{
			return;
		}
		if (base.IsBusy())
		{
			return;
		}
		if (base.IsLocked())
		{
			return;
		}
		if (rpc.player.IsWounded())
		{
			if (!this.woundedOpens.ContainsKey(rpc.player) || this.woundedOpens[rpc.player] <= 2.5f)
			{
				return;
			}
			this.woundedOpens.Remove(rpc.player);
		}
		BaseLock baseLock = base.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
		if (baseLock != null)
		{
			if (!baseLock.OnTryToOpen(rpc.player))
			{
				return;
			}
			if (baseLock.IsLocked() && UnityEngine.Time.realtimeSinceStartup - this.decayResetTimeLast > 60f)
			{
				BuildingBlock buildingBlock = base.FindLinkedEntity<BuildingBlock>();
				if (buildingBlock)
				{
					global::Decay.BuildingDecayTouch(buildingBlock);
				}
				else
				{
					global::Decay.RadialDecayTouch(base.transform.position, 40f, 2097408);
				}
				this.decayResetTimeLast = UnityEngine.Time.realtimeSinceStartup;
			}
		}
		base.SetFlag(BaseEntity.Flags.Open, true, false, true);
		base.SendNetworkUpdateImmediate(false);
		if (this.isSecurityDoor && this.NavMeshLink != null)
		{
			this.SetNavMeshLinkEnabled(true);
		}
		if (this.checkPhysBoxesOnOpen)
		{
			this.StartCheckingForBlockages();
		}
	}

	// Token: 0x06000A18 RID: 2584 RVA: 0x0005C6FE File Offset: 0x0005A8FE
	private void StartCheckingForBlockages()
	{
		if (this.HasVehiclePushBoxes)
		{
			base.Invoke(new Action(this.EnableVehiclePhysBoxes), 0.25f);
			base.Invoke(new Action(this.DisableVehiclePhysBox), 4f);
		}
	}

	// Token: 0x06000A19 RID: 2585 RVA: 0x0005C736 File Offset: 0x0005A936
	private void StopCheckingForBlockages()
	{
		if (this.HasVehiclePushBoxes)
		{
			this.ToggleVehiclePushBoxes(false);
			base.CancelInvoke(new Action(this.DisableVehiclePhysBox));
		}
	}

	// Token: 0x06000A1A RID: 2586 RVA: 0x0005C75C File Offset: 0x0005A95C
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_CloseDoor(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract(true))
		{
			return;
		}
		if (!this.canHandOpen)
		{
			return;
		}
		if (!base.IsOpen())
		{
			return;
		}
		if (base.IsBusy())
		{
			return;
		}
		if (base.IsLocked())
		{
			return;
		}
		if (rpc.player.IsWounded())
		{
			if (!this.woundedCloses.ContainsKey(rpc.player) || this.woundedCloses[rpc.player] <= 2.5f)
			{
				return;
			}
			this.woundedCloses.Remove(rpc.player);
		}
		BaseLock baseLock = base.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
		if (baseLock != null && !baseLock.OnTryToClose(rpc.player))
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Open, false, false, true);
		base.SendNetworkUpdateImmediate(false);
		if (this.isSecurityDoor && this.NavMeshLink != null)
		{
			this.SetNavMeshLinkEnabled(false);
		}
		this.StartCheckingForBlockages();
	}

	// Token: 0x06000A1B RID: 2587 RVA: 0x0005C848 File Offset: 0x0005AA48
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_KnockDoor(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract(true))
		{
			return;
		}
		if (!this.knockEffect.isValid)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup < this.nextKnockTime)
		{
			return;
		}
		this.nextKnockTime = UnityEngine.Time.realtimeSinceStartup + 0.5f;
		BaseEntity slot = base.GetSlot(BaseEntity.Slot.LowerCenterDecoration);
		if (slot != null)
		{
			DoorKnocker component = slot.GetComponent<DoorKnocker>();
			if (component)
			{
				component.Knock(rpc.player);
				return;
			}
		}
		Effect.server.Run(this.knockEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
	}

	// Token: 0x06000A1C RID: 2588 RVA: 0x0005C8DC File Offset: 0x0005AADC
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_ToggleHatch(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract(true))
		{
			return;
		}
		if (!this.hasHatch)
		{
			return;
		}
		BaseLock baseLock = base.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
		if (!baseLock || baseLock.OnTryToOpen(rpc.player))
		{
			base.SetFlag(BaseEntity.Flags.Reserved3, !base.HasFlag(BaseEntity.Flags.Reserved3), false, true);
		}
	}

	// Token: 0x06000A1D RID: 2589 RVA: 0x0005C93F File Offset: 0x0005AB3F
	private void EnableVehiclePhysBoxes()
	{
		this.ToggleVehiclePushBoxes(true);
	}

	// Token: 0x06000A1E RID: 2590 RVA: 0x0005C948 File Offset: 0x0005AB48
	private void DisableVehiclePhysBox()
	{
		this.ToggleVehiclePushBoxes(false);
	}

	// Token: 0x170000E6 RID: 230
	// (get) Token: 0x06000A1F RID: 2591 RVA: 0x0005C951 File Offset: 0x0005AB51
	private bool HasVehiclePushBoxes
	{
		get
		{
			return this.vehiclePhysBoxes != null && this.vehiclePhysBoxes.Length != 0;
		}
	}

	// Token: 0x06000A20 RID: 2592 RVA: 0x0005C968 File Offset: 0x0005AB68
	private void ToggleVehiclePushBoxes(bool state)
	{
		if (this.vehiclePhysBoxes == null)
		{
			return;
		}
		foreach (TriggerNotify triggerNotify in this.vehiclePhysBoxes)
		{
			if (triggerNotify != null)
			{
				triggerNotify.gameObject.SetActive(state);
			}
		}
	}

	// Token: 0x06000A21 RID: 2593 RVA: 0x0005C9AC File Offset: 0x0005ABAC
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	private void Server_NotifyWoundedOpen(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!player.IsWounded())
		{
			return;
		}
		if (!this.woundedOpens.ContainsKey(player))
		{
			this.woundedOpens.Add(player, default(TimeSince));
		}
		else
		{
			this.woundedOpens[player] = 0f;
		}
		base.Invoke(delegate()
		{
			this.CheckTimedOutPlayers(this.woundedOpens);
		}, 5f);
	}

	// Token: 0x06000A22 RID: 2594 RVA: 0x0005CA1C File Offset: 0x0005AC1C
	private void CheckTimedOutPlayers(Dictionary<BasePlayer, TimeSince> dictionary)
	{
		List<BasePlayer> list = Facepunch.Pool.GetList<BasePlayer>();
		foreach (KeyValuePair<BasePlayer, TimeSince> keyValuePair in dictionary)
		{
			if (keyValuePair.Value > 5f)
			{
				list.Add(keyValuePair.Key);
			}
		}
		foreach (BasePlayer key in list)
		{
			if (dictionary.ContainsKey(key))
			{
				dictionary.Remove(key);
			}
		}
		Facepunch.Pool.FreeList<BasePlayer>(ref list);
	}

	// Token: 0x06000A23 RID: 2595 RVA: 0x0005CADC File Offset: 0x0005ACDC
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	private void Server_NotifyWoundedClose(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!player.IsWounded())
		{
			return;
		}
		if (!this.woundedCloses.ContainsKey(player))
		{
			this.woundedCloses.Add(player, default(TimeSince));
		}
		else
		{
			this.woundedCloses[player] = 0f;
		}
		base.Invoke(delegate()
		{
			this.CheckTimedOutPlayers(this.woundedCloses);
		}, 5f);
	}

	// Token: 0x06000A24 RID: 2596 RVA: 0x0005CB4C File Offset: 0x0005AD4C
	private void ReverseDoorAnimation(bool wasOpening)
	{
		if (this.model == null || this.model.animator == null)
		{
			return;
		}
		AnimatorStateInfo currentAnimatorStateInfo = this.model.animator.GetCurrentAnimatorStateInfo(0);
		this.model.animator.Play(wasOpening ? Door.closeHash : Door.openHash, 0, 1f - currentAnimatorStateInfo.normalizedTime);
	}

	// Token: 0x06000A25 RID: 2597 RVA: 0x000300D2 File Offset: 0x0002E2D2
	public override float BoundsPadding()
	{
		return 2f;
	}

	// Token: 0x06000A26 RID: 2598 RVA: 0x0005CBBC File Offset: 0x0005ADBC
	public void OnObjects(TriggerNotify trigger)
	{
		if (base.isServer)
		{
			bool flag = false;
			foreach (BaseEntity baseEntity in trigger.entityContents)
			{
				BaseMountable baseMountable;
				if ((baseMountable = (baseEntity as BaseMountable)) != null && baseMountable.BlocksDoors)
				{
					flag = true;
					break;
				}
				BaseVehicleModule baseVehicleModule;
				if ((baseVehicleModule = (baseEntity as BaseVehicleModule)) != null && baseVehicleModule.Vehicle != null && baseVehicleModule.Vehicle.BlocksDoors)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
			bool flag2 = base.HasFlag(BaseEntity.Flags.Open);
			this.SetOpen(!flag2, true);
			this.ReverseDoorAnimation(flag2);
			this.StopCheckingForBlockages();
			base.ClientRPC<int>(null, "OnDoorInterrupted", flag2 ? 1 : 0);
		}
	}

	// Token: 0x06000A27 RID: 2599 RVA: 0x000059DD File Offset: 0x00003BDD
	public void OnEmpty()
	{
	}

	// Token: 0x06000A28 RID: 2600 RVA: 0x0005CC90 File Offset: 0x0005AE90
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer)
		{
			BaseEntity slot = base.GetSlot(BaseEntity.Slot.UpperModifier);
			if (slot)
			{
				slot.SendMessage("Think");
			}
		}
		if (this.ClosedColliderRoots != null)
		{
			bool active = !base.HasFlag(BaseEntity.Flags.Open) || base.HasFlag(BaseEntity.Flags.Busy);
			foreach (GameObject gameObject in this.ClosedColliderRoots)
			{
				if (gameObject != null)
				{
					gameObject.gameObject.SetActive(active);
				}
			}
		}
	}

	// Token: 0x0400067F RID: 1663
	public GameObjectRef knockEffect;

	// Token: 0x04000680 RID: 1664
	public bool canTakeLock = true;

	// Token: 0x04000681 RID: 1665
	public bool hasHatch;

	// Token: 0x04000682 RID: 1666
	public bool canTakeCloser;

	// Token: 0x04000683 RID: 1667
	public bool canTakeKnocker;

	// Token: 0x04000684 RID: 1668
	public bool canNpcOpen = true;

	// Token: 0x04000685 RID: 1669
	public bool canHandOpen = true;

	// Token: 0x04000686 RID: 1670
	public bool isSecurityDoor;

	// Token: 0x04000687 RID: 1671
	public TriggerNotify[] vehiclePhysBoxes;

	// Token: 0x04000688 RID: 1672
	public bool checkPhysBoxesOnOpen;

	// Token: 0x04000689 RID: 1673
	public SoundDefinition vehicleCollisionSfx;

	// Token: 0x0400068A RID: 1674
	public GameObject[] ClosedColliderRoots;

	// Token: 0x0400068B RID: 1675
	private float decayResetTimeLast = float.NegativeInfinity;

	// Token: 0x0400068C RID: 1676
	public NavMeshModifierVolume NavMeshVolumeAnimals;

	// Token: 0x0400068D RID: 1677
	public NavMeshModifierVolume NavMeshVolumeHumanoids;

	// Token: 0x0400068E RID: 1678
	public NavMeshLink NavMeshLink;

	// Token: 0x0400068F RID: 1679
	public NPCDoorTriggerBox NpcTriggerBox;

	// Token: 0x04000690 RID: 1680
	private static int nonWalkableArea = -1;

	// Token: 0x04000691 RID: 1681
	private static int animalAgentTypeId = -1;

	// Token: 0x04000692 RID: 1682
	private static int humanoidAgentTypeId = -1;

	// Token: 0x04000693 RID: 1683
	private Dictionary<BasePlayer, TimeSince> woundedOpens = new Dictionary<BasePlayer, TimeSince>();

	// Token: 0x04000694 RID: 1684
	private Dictionary<BasePlayer, TimeSince> woundedCloses = new Dictionary<BasePlayer, TimeSince>();

	// Token: 0x04000695 RID: 1685
	private float nextKnockTime = float.NegativeInfinity;

	// Token: 0x04000696 RID: 1686
	private static int openHash = Animator.StringToHash("open");

	// Token: 0x04000697 RID: 1687
	private static int closeHash = Animator.StringToHash("close");
}
