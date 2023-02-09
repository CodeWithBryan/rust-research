using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000A7 RID: 167
public class Planner : global::HeldEntity
{
	// Token: 0x06000F34 RID: 3892 RVA: 0x0007DDA8 File Offset: 0x0007BFA8
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Planner.OnRpcMessage", 0))
		{
			if (rpc == 1872774636U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoPlace ");
				}
				using (TimeWarning.New("DoPlace", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(1872774636U, "DoPlace", this, player))
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
							this.DoPlace(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in DoPlace");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000F35 RID: 3893 RVA: 0x0007DF0C File Offset: 0x0007C10C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	private void DoPlace(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		using (CreateBuilding createBuilding = CreateBuilding.Deserialize(msg.read))
		{
			this.DoBuild(createBuilding);
		}
	}

	// Token: 0x06000F36 RID: 3894 RVA: 0x0007DF58 File Offset: 0x0007C158
	public Socket_Base FindSocket(string name, uint prefabIDToFind)
	{
		return PrefabAttribute.server.FindAll<Socket_Base>(prefabIDToFind).FirstOrDefault((Socket_Base s) => s.socketName == name);
	}

	// Token: 0x06000F37 RID: 3895 RVA: 0x0007DF90 File Offset: 0x0007C190
	public void DoBuild(CreateBuilding msg)
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (ConVar.AntiHack.objectplacement && ownerPlayer.TriggeredAntiHack(1f, float.PositiveInfinity))
		{
			ownerPlayer.ChatMessage("AntiHack!");
			return;
		}
		Construction construction = PrefabAttribute.server.Find<Construction>(msg.blockID);
		if (construction == null)
		{
			ownerPlayer.ChatMessage("Couldn't find Construction " + msg.blockID);
			return;
		}
		if (!this.CanAffordToPlace(construction))
		{
			ownerPlayer.ChatMessage("Can't afford to place!");
			return;
		}
		if (!ownerPlayer.CanBuild() && !construction.canBypassBuildingPermission)
		{
			ownerPlayer.ChatMessage("Building is blocked!");
			return;
		}
		Deployable deployable = this.GetDeployable();
		if (construction.deployable != deployable)
		{
			ownerPlayer.ChatMessage("Deployable mismatch!");
			global::AntiHack.NoteAdminHack(ownerPlayer);
			return;
		}
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		BaseGameMode.CanBuildResult? canBuildResult = (activeGameMode != null) ? activeGameMode.CanBuildEntity(ownerPlayer, construction) : null;
		if (canBuildResult != null)
		{
			if (canBuildResult.Value.Phrase != null)
			{
				ownerPlayer.ShowToast(canBuildResult.Value.Result ? GameTip.Styles.Blue_Long : GameTip.Styles.Red_Normal, canBuildResult.Value.Phrase, canBuildResult.Value.Arguments);
			}
			if (!canBuildResult.Value.Result)
			{
				return;
			}
		}
		Construction.Target target = default(Construction.Target);
		if (msg.entity > 0U)
		{
			target.entity = (global::BaseNetworkable.serverEntities.Find(msg.entity) as global::BaseEntity);
			if (target.entity == null)
			{
				ownerPlayer.ChatMessage("Couldn't find entity " + msg.entity);
				return;
			}
			msg.position = target.entity.transform.TransformPoint(msg.position);
			msg.normal = target.entity.transform.TransformDirection(msg.normal);
			msg.rotation = target.entity.transform.rotation * msg.rotation;
			if (msg.socket > 0U)
			{
				string text = StringPool.Get(msg.socket);
				if (text != "")
				{
					target.socket = this.FindSocket(text, target.entity.prefabID);
				}
				if (target.socket == null)
				{
					ownerPlayer.ChatMessage("Couldn't find socket " + msg.socket);
					return;
				}
			}
			else if (target.entity is Door)
			{
				ownerPlayer.ChatMessage("Can't deploy on door");
				return;
			}
		}
		target.ray = msg.ray;
		target.onTerrain = msg.onterrain;
		target.position = msg.position;
		target.normal = msg.normal;
		target.rotation = msg.rotation;
		target.player = ownerPlayer;
		target.valid = true;
		if (target.entity != null && deployable != null && deployable.setSocketParent)
		{
			Vector3 position = (target.socket != null) ? target.GetWorldPosition() : target.position;
			float num = target.entity.Distance(position);
			if (num > 1f)
			{
				ownerPlayer.ChatMessage("Parent too far away: " + num);
				return;
			}
		}
		this.DoBuild(target, construction);
	}

	// Token: 0x06000F38 RID: 3896 RVA: 0x0007E2DC File Offset: 0x0007C4DC
	public void DoBuild(Construction.Target target, Construction component)
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (target.ray.IsNaNOrInfinity())
		{
			return;
		}
		if (target.position.IsNaNOrInfinity())
		{
			return;
		}
		if (target.normal.IsNaNOrInfinity())
		{
			return;
		}
		if (target.socket != null)
		{
			if (!target.socket.female)
			{
				ownerPlayer.ChatMessage("Target socket is not female. (" + target.socket.socketName + ")");
				return;
			}
			if (target.entity != null && target.entity.IsOccupied(target.socket))
			{
				ownerPlayer.ChatMessage("Target socket is occupied. (" + target.socket.socketName + ")");
				return;
			}
			if (target.onTerrain)
			{
				ownerPlayer.ChatMessage("Target on terrain is not allowed when attaching to socket. (" + target.socket.socketName + ")");
				return;
			}
		}
		Vector3 vector = (target.entity != null && target.socket != null) ? target.GetWorldPosition() : target.position;
		if (global::AntiHack.TestIsBuildingInsideSomething(target, vector))
		{
			ownerPlayer.ChatMessage("Can't deploy inside objects");
			return;
		}
		if (ConVar.AntiHack.eye_protection >= 2)
		{
			Vector3 center = ownerPlayer.eyes.center;
			Vector3 position = ownerPlayer.eyes.position;
			Vector3 origin = target.ray.origin;
			Vector3 vector2 = vector;
			int num = 2097152;
			int num2 = ConVar.AntiHack.build_terraincheck ? 10551296 : 2162688;
			float num3 = ConVar.AntiHack.build_losradius;
			float padding = ConVar.AntiHack.build_losradius + 0.01f;
			int layerMask = num2;
			if (target.socket != null)
			{
				num3 = 0f;
				padding = 0.5f;
				layerMask = num;
			}
			if (component.isSleepingBag)
			{
				num3 = ConVar.AntiHack.build_losradius_sleepingbag;
				padding = ConVar.AntiHack.build_losradius_sleepingbag + 0.01f;
				layerMask = num2;
			}
			if (num3 > 0f)
			{
				vector2 += target.normal.normalized * num3;
			}
			if (target.entity != null)
			{
				DeployShell deployShell = PrefabAttribute.server.Find<DeployShell>(target.entity.prefabID);
				if (deployShell != null)
				{
					vector2 += target.normal.normalized * deployShell.LineOfSightPadding();
				}
			}
			if (!GamePhysics.LineOfSightRadius(center, position, layerMask, num3, null) || !GamePhysics.LineOfSightRadius(position, origin, layerMask, num3, null) || !GamePhysics.LineOfSightRadius(origin, vector2, layerMask, num3, 0f, padding, null))
			{
				ownerPlayer.ChatMessage("Line of sight blocked.");
				return;
			}
		}
		Construction.lastPlacementError = "No Error";
		GameObject gameObject = this.DoPlacement(target, component);
		if (gameObject == null)
		{
			ownerPlayer.ChatMessage("Can't place: " + Construction.lastPlacementError);
		}
		if (gameObject != null)
		{
			Deployable deployable = this.GetDeployable();
			if (deployable != null)
			{
				global::BaseEntity baseEntity = gameObject.ToBaseEntity();
				if (deployable.setSocketParent && target.entity != null && target.entity.SupportsChildDeployables() && baseEntity)
				{
					baseEntity.SetParent(target.entity, true, false);
				}
				if (deployable.wantsInstanceData && base.GetOwnerItem().instanceData != null)
				{
					(baseEntity as IInstanceDataReceiver).ReceiveInstanceData(base.GetOwnerItem().instanceData);
				}
				if (deployable.copyInventoryFromItem)
				{
					StorageContainer component2 = baseEntity.GetComponent<StorageContainer>();
					if (component2)
					{
						component2.ReceiveInventoryFromItem(base.GetOwnerItem());
					}
				}
				ItemModDeployable modDeployable = this.GetModDeployable();
				if (modDeployable != null)
				{
					modDeployable.OnDeployed(baseEntity, ownerPlayer);
				}
				baseEntity.OnDeployed(baseEntity.GetParentEntity(), ownerPlayer, base.GetOwnerItem());
				if (deployable.placeEffect.isValid)
				{
					if (target.entity && target.socket != null)
					{
						Effect.server.Run(deployable.placeEffect.resourcePath, target.entity.transform.TransformPoint(target.socket.worldPosition), target.entity.transform.up, null, false);
					}
					else
					{
						Effect.server.Run(deployable.placeEffect.resourcePath, target.position, target.normal, null, false);
					}
				}
			}
			this.PayForPlacement(ownerPlayer, component);
		}
	}

	// Token: 0x06000F39 RID: 3897 RVA: 0x0007E718 File Offset: 0x0007C918
	public GameObject DoPlacement(Construction.Target placement, Construction component)
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return null;
		}
		global::BaseEntity baseEntity = component.CreateConstruction(placement, true);
		if (!baseEntity)
		{
			return null;
		}
		float num = 1f;
		global::Item ownerItem = base.GetOwnerItem();
		if (ownerItem != null)
		{
			baseEntity.skinID = ownerItem.skin;
			if (ownerItem.hasCondition)
			{
				num = ownerItem.conditionNormalized;
			}
		}
		baseEntity.gameObject.AwakeFromInstantiate();
		global::BuildingBlock buildingBlock = baseEntity as global::BuildingBlock;
		if (buildingBlock)
		{
			buildingBlock.blockDefinition = PrefabAttribute.server.Find<Construction>(buildingBlock.prefabID);
			if (!buildingBlock.blockDefinition)
			{
				Debug.LogError("Placing a building block that has no block definition!");
				return null;
			}
			buildingBlock.SetGrade(buildingBlock.blockDefinition.defaultGrade.gradeBase.type);
			float num2 = buildingBlock.currentGrade.maxHealth;
		}
		BaseCombatEntity baseCombatEntity = baseEntity as BaseCombatEntity;
		if (baseCombatEntity)
		{
			float num2 = (buildingBlock != null) ? buildingBlock.currentGrade.maxHealth : baseCombatEntity.startHealth;
			baseCombatEntity.ResetLifeStateOnSpawn = false;
			baseCombatEntity.InitializeHealth(num2 * num, num2);
		}
		baseEntity.gameObject.SendMessage("SetDeployedBy", ownerPlayer, SendMessageOptions.DontRequireReceiver);
		baseEntity.OwnerID = ownerPlayer.userID;
		baseEntity.Spawn();
		if (buildingBlock)
		{
			Effect.server.Run("assets/bundled/prefabs/fx/build/frame_place.prefab", baseEntity, 0U, Vector3.zero, Vector3.zero, null, false);
		}
		global::StabilityEntity stabilityEntity = baseEntity as global::StabilityEntity;
		if (stabilityEntity)
		{
			stabilityEntity.UpdateSurroundingEntities();
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06000F3A RID: 3898 RVA: 0x0007E8A0 File Offset: 0x0007CAA0
	public void PayForPlacement(global::BasePlayer player, Construction component)
	{
		if (this.isTypeDeployable)
		{
			this.GetItem().UseItem(1);
			return;
		}
		List<global::Item> list = new List<global::Item>();
		foreach (ItemAmount itemAmount in component.defaultGrade.costToBuild)
		{
			player.inventory.Take(list, itemAmount.itemDef.itemid, (int)itemAmount.amount);
			player.Command("note.inv", new object[]
			{
				itemAmount.itemDef.itemid,
				itemAmount.amount * -1f
			});
		}
		foreach (global::Item item in list)
		{
			item.Remove(0f);
		}
	}

	// Token: 0x06000F3B RID: 3899 RVA: 0x0007E9A4 File Offset: 0x0007CBA4
	public bool CanAffordToPlace(Construction component)
	{
		if (this.isTypeDeployable)
		{
			return true;
		}
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return false;
		}
		foreach (ItemAmount itemAmount in component.defaultGrade.costToBuild)
		{
			if ((float)ownerPlayer.inventory.GetAmount(itemAmount.itemDef.itemid) < itemAmount.amount)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000F3C RID: 3900 RVA: 0x0007EA38 File Offset: 0x0007CC38
	public ItemModDeployable GetModDeployable()
	{
		ItemDefinition ownerItemDefinition = base.GetOwnerItemDefinition();
		if (ownerItemDefinition == null)
		{
			return null;
		}
		return ownerItemDefinition.GetComponent<ItemModDeployable>();
	}

	// Token: 0x06000F3D RID: 3901 RVA: 0x0007EA60 File Offset: 0x0007CC60
	public Deployable GetDeployable()
	{
		ItemModDeployable modDeployable = this.GetModDeployable();
		if (modDeployable == null)
		{
			return null;
		}
		return modDeployable.GetDeployable(this);
	}

	// Token: 0x17000155 RID: 341
	// (get) Token: 0x06000F3E RID: 3902 RVA: 0x0007EA86 File Offset: 0x0007CC86
	public bool isTypeDeployable
	{
		get
		{
			return this.GetModDeployable() != null;
		}
	}

	// Token: 0x040009DB RID: 2523
	public global::BaseEntity[] buildableList;
}
