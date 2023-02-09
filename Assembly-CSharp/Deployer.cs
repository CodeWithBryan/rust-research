using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000061 RID: 97
public class Deployer : HeldEntity
{
	// Token: 0x060009E6 RID: 2534 RVA: 0x0005AAE4 File Offset: 0x00058CE4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Deployer.OnRpcMessage", 0))
		{
			if (rpc == 3001117906U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoDeploy ");
				}
				using (TimeWarning.New("DoDeploy", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(3001117906U, "DoDeploy", this, player))
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
							this.DoDeploy(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in DoDeploy");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060009E7 RID: 2535 RVA: 0x0005AC48 File Offset: 0x00058E48
	public ItemModDeployable GetModDeployable()
	{
		ItemDefinition ownerItemDefinition = base.GetOwnerItemDefinition();
		if (ownerItemDefinition == null)
		{
			return null;
		}
		return ownerItemDefinition.GetComponent<ItemModDeployable>();
	}

	// Token: 0x060009E8 RID: 2536 RVA: 0x0005AC70 File Offset: 0x00058E70
	public Deployable GetDeployable()
	{
		ItemModDeployable modDeployable = this.GetModDeployable();
		if (modDeployable == null)
		{
			return null;
		}
		return modDeployable.GetDeployable(this);
	}

	// Token: 0x060009E9 RID: 2537 RVA: 0x0005AC96 File Offset: 0x00058E96
	public Quaternion GetDeployedRotation(Vector3 normal, Vector3 placeDir)
	{
		return Quaternion.LookRotation(normal, placeDir) * Quaternion.Euler(90f, 0f, 0f);
	}

	// Token: 0x060009EA RID: 2538 RVA: 0x0005ACB8 File Offset: 0x00058EB8
	public bool IsPlacementAngleAcceptable(Vector3 pos, Quaternion rot)
	{
		Vector3 lhs = rot * Vector3.up;
		return Mathf.Acos(Vector3.Dot(lhs, Vector3.up)) <= 0.61086524f;
	}

	// Token: 0x060009EB RID: 2539 RVA: 0x0005ACEC File Offset: 0x00058EEC
	public bool CheckPlacement(Deployable deployable, Ray ray, float fDistance)
	{
		using (TimeWarning.New("Deploy.CheckPlacement", 0))
		{
			RaycastHit raycastHit;
			if (!UnityEngine.Physics.Raycast(ray, out raycastHit, fDistance, 1235288065))
			{
				return false;
			}
			DeployVolume[] volumes = PrefabAttribute.server.FindAll<DeployVolume>(deployable.prefabID);
			Vector3 point = raycastHit.point;
			Quaternion deployedRotation = this.GetDeployedRotation(raycastHit.normal, ray.direction);
			if (DeployVolume.Check(point, deployedRotation, volumes, -1))
			{
				return false;
			}
			if (!this.IsPlacementAngleAcceptable(raycastHit.point, deployedRotation))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060009EC RID: 2540 RVA: 0x0005AD8C File Offset: 0x00058F8C
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void DoDeploy(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		Deployable deployable = this.GetDeployable();
		if (deployable == null)
		{
			return;
		}
		Ray ray = msg.read.Ray();
		uint entityID = msg.read.UInt32();
		if (deployable.toSlot)
		{
			this.DoDeploy_Slot(deployable, ray, entityID);
			return;
		}
		this.DoDeploy_Regular(deployable, ray);
	}

	// Token: 0x060009ED RID: 2541 RVA: 0x0005ADEC File Offset: 0x00058FEC
	public void DoDeploy_Slot(Deployable deployable, Ray ray, uint entityID)
	{
		if (!base.HasItemAmount())
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (!ownerPlayer.CanBuild())
		{
			ownerPlayer.ChatMessage("Building is blocked at player position!");
			return;
		}
		BaseEntity baseEntity = BaseNetworkable.serverEntities.Find(entityID) as BaseEntity;
		if (baseEntity == null)
		{
			return;
		}
		if (!baseEntity.HasSlot(deployable.slot))
		{
			return;
		}
		if (baseEntity.GetSlot(deployable.slot) != null)
		{
			return;
		}
		if (ownerPlayer.Distance(baseEntity) > 3f)
		{
			ownerPlayer.ChatMessage("Too far away!");
			return;
		}
		if (!ownerPlayer.CanBuild(baseEntity.WorldSpaceBounds()))
		{
			ownerPlayer.ChatMessage("Building is blocked at placement position!");
			return;
		}
		Item ownerItem = base.GetOwnerItem();
		ItemModDeployable modDeployable = this.GetModDeployable();
		BaseEntity baseEntity2 = GameManager.server.CreateEntity(modDeployable.entityPrefab.resourcePath, default(Vector3), default(Quaternion), true);
		if (baseEntity2 != null)
		{
			baseEntity2.skinID = ownerItem.skin;
			baseEntity2.SetParent(baseEntity, baseEntity.GetSlotAnchorName(deployable.slot), false, false);
			baseEntity2.OwnerID = ownerPlayer.userID;
			baseEntity2.OnDeployed(baseEntity, ownerPlayer, ownerItem);
			baseEntity2.Spawn();
			baseEntity.SetSlot(deployable.slot, baseEntity2);
			if (deployable.placeEffect.isValid)
			{
				Effect.server.Run(deployable.placeEffect.resourcePath, baseEntity.transform.position, Vector3.up, null, false);
			}
		}
		modDeployable.OnDeployed(baseEntity2, ownerPlayer);
		base.UseItemAmount(1);
	}

	// Token: 0x060009EE RID: 2542 RVA: 0x0005AF6C File Offset: 0x0005916C
	public void DoDeploy_Regular(Deployable deployable, Ray ray)
	{
		if (!base.HasItemAmount())
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (!ownerPlayer.CanBuild())
		{
			ownerPlayer.ChatMessage("Building is blocked at player position!");
			return;
		}
		if (ConVar.AntiHack.objectplacement && ownerPlayer.TriggeredAntiHack(1f, float.PositiveInfinity))
		{
			ownerPlayer.ChatMessage("AntiHack!");
			return;
		}
		if (!this.CheckPlacement(deployable, ray, 8f))
		{
			return;
		}
		RaycastHit raycastHit;
		if (!UnityEngine.Physics.Raycast(ray, out raycastHit, 8f, 1235288065))
		{
			return;
		}
		Vector3 point = raycastHit.point;
		Quaternion deployedRotation = this.GetDeployedRotation(raycastHit.normal, ray.direction);
		Item ownerItem = base.GetOwnerItem();
		ItemModDeployable modDeployable = this.GetModDeployable();
		if (ownerPlayer.Distance(point) > 3f)
		{
			ownerPlayer.ChatMessage("Too far away!");
			return;
		}
		if (!ownerPlayer.CanBuild(point, deployedRotation, deployable.bounds))
		{
			ownerPlayer.ChatMessage("Building is blocked at placement position!");
			return;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(modDeployable.entityPrefab.resourcePath, point, deployedRotation, true);
		if (!baseEntity)
		{
			Debug.LogWarning("Couldn't create prefab:" + modDeployable.entityPrefab.resourcePath);
			return;
		}
		baseEntity.skinID = ownerItem.skin;
		baseEntity.SendMessage("SetDeployedBy", ownerPlayer, SendMessageOptions.DontRequireReceiver);
		baseEntity.OwnerID = ownerPlayer.userID;
		baseEntity.Spawn();
		modDeployable.OnDeployed(baseEntity, ownerPlayer);
		base.UseItemAmount(1);
	}
}
