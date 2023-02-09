using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000095 RID: 149
public class MedicalTool : AttackEntity
{
	// Token: 0x06000D90 RID: 3472 RVA: 0x00071F4C File Offset: 0x0007014C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("MedicalTool.OnRpcMessage", 0))
		{
			if (rpc == 789049461U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - UseOther ");
				}
				using (TimeWarning.New("UseOther", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(789049461U, "UseOther", this, player))
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
							this.UseOther(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in UseOther");
					}
				}
				return true;
			}
			if (rpc == 2918424470U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - UseSelf ");
				}
				using (TimeWarning.New("UseSelf", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(2918424470U, "UseSelf", this, player))
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
							this.UseSelf(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in UseSelf");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000D91 RID: 3473 RVA: 0x00072244 File Offset: 0x00070444
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void UseOther(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!this.VerifyClientAttack(player))
		{
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			return;
		}
		if (!player.CanInteract())
		{
			return;
		}
		if (!base.HasItemAmount() || !this.canUseOnOther)
		{
			return;
		}
		BasePlayer basePlayer = BaseNetworkable.serverEntities.Find(msg.read.UInt32()) as BasePlayer;
		if (basePlayer != null && Vector3.Distance(basePlayer.transform.position, player.transform.position) < 4f)
		{
			base.ClientRPCPlayer(null, player, "Reset");
			this.GiveEffectsTo(basePlayer);
			base.UseItemAmount(1);
			base.StartAttackCooldown(this.repeatDelay);
		}
	}

	// Token: 0x06000D92 RID: 3474 RVA: 0x000722F4 File Offset: 0x000704F4
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void UseSelf(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!this.VerifyClientAttack(player))
		{
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			return;
		}
		if (!player.CanInteract())
		{
			return;
		}
		if (!base.HasItemAmount())
		{
			return;
		}
		base.ClientRPCPlayer(null, player, "Reset");
		this.GiveEffectsTo(player);
		base.UseItemAmount(1);
		base.StartAttackCooldown(this.repeatDelay);
	}

	// Token: 0x06000D93 RID: 3475 RVA: 0x00072354 File Offset: 0x00070554
	public override void ServerUse()
	{
		if (base.isClient)
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		if (!ownerPlayer.CanInteract())
		{
			return;
		}
		if (!base.HasItemAmount())
		{
			return;
		}
		this.GiveEffectsTo(ownerPlayer);
		base.UseItemAmount(1);
		base.StartAttackCooldown(this.repeatDelay);
		base.SignalBroadcast(BaseEntity.Signal.Attack, string.Empty, null);
		if (ownerPlayer.IsNpc)
		{
			ownerPlayer.SignalBroadcast(BaseEntity.Signal.Attack, null);
		}
	}

	// Token: 0x06000D94 RID: 3476 RVA: 0x000723C8 File Offset: 0x000705C8
	private void GiveEffectsTo(BasePlayer player)
	{
		if (!player)
		{
			return;
		}
		ItemModConsumable component = base.GetOwnerItemDefinition().GetComponent<ItemModConsumable>();
		if (!component)
		{
			Debug.LogWarning("No consumable for medicaltool :" + base.name);
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (player != ownerPlayer && player.IsWounded() && this.canRevive)
		{
			player.StopWounded(ownerPlayer);
		}
		foreach (ItemModConsumable.ConsumableEffect consumableEffect in component.effects)
		{
			if (consumableEffect.type == MetabolismAttribute.Type.Health)
			{
				player.health += consumableEffect.amount;
			}
			else
			{
				player.metabolism.ApplyChange(consumableEffect.type, consumableEffect.amount, consumableEffect.time);
			}
		}
		if (player is BasePet)
		{
			player.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x040008BF RID: 2239
	public float healDurationSelf = 4f;

	// Token: 0x040008C0 RID: 2240
	public float healDurationOther = 4f;

	// Token: 0x040008C1 RID: 2241
	public float healDurationOtherWounded = 7f;

	// Token: 0x040008C2 RID: 2242
	public float maxDistanceOther = 2f;

	// Token: 0x040008C3 RID: 2243
	public bool canUseOnOther = true;

	// Token: 0x040008C4 RID: 2244
	public bool canRevive = true;
}
