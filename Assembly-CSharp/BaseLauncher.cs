﻿using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000039 RID: 57
public class BaseLauncher : BaseProjectile
{
	// Token: 0x060003B4 RID: 948 RVA: 0x0002ECA0 File Offset: 0x0002CEA0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseLauncher.OnRpcMessage", 0))
		{
			if (rpc == 853319324U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SV_Launch ");
				}
				using (TimeWarning.New("SV_Launch", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(853319324U, "SV_Launch", this, player))
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
							this.SV_Launch(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SV_Launch");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060003B5 RID: 949 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool ForceSendMagazine(BaseNetworkable.SaveInfo saveInfo)
	{
		return true;
	}

	// Token: 0x060003B6 RID: 950 RVA: 0x0002EE04 File Offset: 0x0002D004
	public override void ServerUse()
	{
		this.ServerUse(1f, null);
	}

	// Token: 0x060003B7 RID: 951 RVA: 0x0002EE14 File Offset: 0x0002D014
	public override void ServerUse(float damageModifier, Transform originOverride = null)
	{
		ItemModProjectile component = this.primaryMagazine.ammoType.GetComponent<ItemModProjectile>();
		if (!component)
		{
			return;
		}
		if (this.primaryMagazine.contents <= 0)
		{
			base.SignalBroadcast(BaseEntity.Signal.DryFire, null);
			base.StartAttackCooldown(1f);
			return;
		}
		if (!component.projectileObject.Get().GetComponent<ServerProjectile>())
		{
			base.ServerUse(damageModifier, originOverride);
			return;
		}
		this.primaryMagazine.contents--;
		if (this.primaryMagazine.contents < 0)
		{
			this.primaryMagazine.contents = 0;
		}
		Vector3 vector = this.MuzzlePoint.transform.forward;
		Vector3 position = this.MuzzlePoint.transform.position;
		float num = this.GetAimCone() + component.projectileSpread;
		if (num > 0f)
		{
			vector = AimConeUtil.GetModifiedAimConeDirection(num, vector, true);
		}
		float num2 = 1f;
		RaycastHit raycastHit;
		if (UnityEngine.Physics.Raycast(position, vector, out raycastHit, num2, 1236478737))
		{
			num2 = raycastHit.distance - 0.1f;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(component.projectileObject.resourcePath, position + vector * num2, default(Quaternion), true);
		if (baseEntity == null)
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		bool flag = ownerPlayer != null && ownerPlayer.IsNpc;
		ServerProjectile component2 = baseEntity.GetComponent<ServerProjectile>();
		if (component2)
		{
			component2.InitializeVelocity(vector * component2.speed);
		}
		baseEntity.SendMessage("SetDamageScale", flag ? this.npcDamageScale : this.turretDamageScale);
		baseEntity.Spawn();
		base.StartAttackCooldown(base.ScaleRepeatDelay(this.repeatDelay));
		base.SignalBroadcast(BaseEntity.Signal.Attack, string.Empty, null);
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			return;
		}
		ownerItem.LoseCondition(UnityEngine.Random.Range(1f, 2f));
	}

	// Token: 0x060003B8 RID: 952 RVA: 0x0002F000 File Offset: 0x0002D200
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void SV_Launch(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!this.VerifyClientAttack(player))
		{
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			return;
		}
		if (this.reloadFinished && base.HasReloadCooldown())
		{
			global::AntiHack.Log(player, AntiHackType.ProjectileHack, "Reloading (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "reload_cooldown");
			return;
		}
		this.reloadStarted = false;
		this.reloadFinished = false;
		if (this.primaryMagazine.contents <= 0)
		{
			global::AntiHack.Log(player, AntiHackType.ProjectileHack, "Magazine empty (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "magazine_empty");
			return;
		}
		this.primaryMagazine.contents--;
		base.SignalBroadcast(BaseEntity.Signal.Attack, string.Empty, player.net.connection);
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
		ItemModProjectile component = this.primaryMagazine.ammoType.GetComponent<ItemModProjectile>();
		if (!component)
		{
			global::AntiHack.Log(player, AntiHackType.ProjectileHack, "Item mod not found (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "mod_missing");
			return;
		}
		float num = this.GetAimCone() + component.projectileSpread;
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
		BaseEntity baseEntity2 = GameManager.server.CreateEntity(component.projectileObject.resourcePath, vector + vector2 * num2, default(Quaternion), true);
		if (baseEntity2 == null)
		{
			return;
		}
		baseEntity2.creatorEntity = player;
		ServerProjectile component2 = baseEntity2.GetComponent<ServerProjectile>();
		if (component2)
		{
			component2.InitializeVelocity(this.GetInheritedVelocity(player, vector2) + vector2 * component2.speed);
		}
		baseEntity2.Spawn();
		base.StartAttackCooldown(base.ScaleRepeatDelay(this.repeatDelay));
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			return;
		}
		ownerItem.LoseCondition(UnityEngine.Random.Range(1f, 2f));
	}
}
