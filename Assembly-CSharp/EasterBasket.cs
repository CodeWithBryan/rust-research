using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000069 RID: 105
public class EasterBasket : AttackEntity
{
	// Token: 0x06000A5A RID: 2650 RVA: 0x0005DCAC File Offset: 0x0005BEAC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("EasterBasket.OnRpcMessage", 0))
		{
			if (rpc == 3763591455U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ThrowEgg ");
				}
				using (TimeWarning.New("ThrowEgg", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(3763591455U, "ThrowEgg", this, player))
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
							this.ThrowEgg(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ThrowEgg");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A5B RID: 2651 RVA: 0x000451D9 File Offset: 0x000433D9
	public override Vector3 GetInheritedVelocity(BasePlayer player, Vector3 direction)
	{
		return player.GetInheritedProjectileVelocity(direction);
	}

	// Token: 0x06000A5C RID: 2652 RVA: 0x0005DE10 File Offset: 0x0005C010
	public Item GetAmmo()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return null;
		}
		Item item = ownerPlayer.inventory.containerMain.FindItemByItemID(this.ammoType.itemid);
		if (item == null)
		{
			item = ownerPlayer.inventory.containerBelt.FindItemByItemID(this.ammoType.itemid);
		}
		return item;
	}

	// Token: 0x06000A5D RID: 2653 RVA: 0x0005DE6A File Offset: 0x0005C06A
	public bool HasAmmo()
	{
		return this.GetAmmo() != null;
	}

	// Token: 0x06000A5E RID: 2654 RVA: 0x0005DE78 File Offset: 0x0005C078
	public void UseAmmo()
	{
		Item ammo = this.GetAmmo();
		if (ammo != null)
		{
			ammo.UseItem(1);
		}
	}

	// Token: 0x06000A5F RID: 2655 RVA: 0x0005DE98 File Offset: 0x0005C098
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	public void ThrowEgg(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!this.VerifyClientAttack(player))
		{
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			return;
		}
		if (!this.HasAmmo())
		{
			return;
		}
		this.UseAmmo();
		Vector3 vector = msg.read.Vector3();
		Vector3 vector2 = msg.read.Vector3().normalized;
		bool flag = msg.read.Bit();
		BaseEntity baseEntity = player.GetParentEntity();
		if (baseEntity == null)
		{
			baseEntity = player.GetMounted();
		}
		if (flag)
		{
			if (baseEntity != null)
			{
				vector = baseEntity.transform.TransformPoint(vector);
				vector2 = baseEntity.transform.TransformDirection(vector2);
			}
			else
			{
				vector = player.eyes.position;
				vector2 = player.eyes.BodyForward();
			}
		}
		if (!base.ValidateEyePos(player, vector))
		{
			return;
		}
		float num = 2f;
		if (num > 0f)
		{
			vector2 = AimConeUtil.GetModifiedAimConeDirection(num, vector2, true);
		}
		float num2 = 1f;
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(vector, vector2, out raycastHit, num2, 1236478737))
		{
			num2 = raycastHit.distance - 0.1f;
		}
		BaseEntity baseEntity2 = GameManager.server.CreateEntity(this.eggProjectile.resourcePath, vector + vector2 * num2, default(Quaternion), true);
		if (baseEntity2 == null)
		{
			return;
		}
		baseEntity2.creatorEntity = player;
		ServerProjectile component = baseEntity2.GetComponent<ServerProjectile>();
		if (component)
		{
			component.InitializeVelocity(this.GetInheritedVelocity(player, vector2) + vector2 * component.speed);
		}
		baseEntity2.Spawn();
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			return;
		}
		ownerItem.LoseCondition(UnityEngine.Random.Range(1f, 2f));
	}

	// Token: 0x040006AC RID: 1708
	public GameObjectRef eggProjectile;

	// Token: 0x040006AD RID: 1709
	public ItemDefinition ammoType;
}
