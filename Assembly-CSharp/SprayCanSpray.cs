using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000CF RID: 207
public class SprayCanSpray : global::DecayEntity, ISplashable
{
	// Token: 0x06001225 RID: 4645 RVA: 0x000920F4 File Offset: 0x000902F4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SprayCanSpray.OnRpcMessage", 0))
		{
			if (rpc == 2774110739U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_RequestWaterClear ");
				}
				using (TimeWarning.New("Server_RequestWaterClear", 0))
				{
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
							this.Server_RequestWaterClear(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_RequestWaterClear");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001226 RID: 4646 RVA: 0x00092218 File Offset: 0x00090418
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.spray == null)
		{
			info.msg.spray = Facepunch.Pool.Get<Spray>();
		}
		info.msg.spray.sprayedBy = this.sprayedByPlayer;
		info.msg.spray.timestamp = this.sprayTimestamp.ToBinary();
	}

	// Token: 0x06001227 RID: 4647 RVA: 0x0009227C File Offset: 0x0009047C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.spray != null)
		{
			this.sprayedByPlayer = info.msg.spray.sprayedBy;
			this.sprayTimestamp = DateTime.FromBinary(info.msg.spray.timestamp);
		}
	}

	// Token: 0x06001228 RID: 4648 RVA: 0x000922D0 File Offset: 0x000904D0
	public override void OnDeployed(global::BaseEntity parent, global::BasePlayer deployedBy, global::Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		this.sprayTimestamp = DateTime.Now;
		this.sprayedByPlayer = deployedBy.userID;
		if (ConVar.Global.MaxSpraysPerPlayer > 0 && this.sprayedByPlayer != 0UL)
		{
			int num = -1;
			DateTime now = DateTime.Now;
			int num2 = 0;
			for (int i = 0; i < SprayCanSpray.AllSprays.Count; i++)
			{
				if (SprayCanSpray.AllSprays[i].sprayedByPlayer == this.sprayedByPlayer)
				{
					num2++;
					if (num == -1 || SprayCanSpray.AllSprays[i].sprayTimestamp < now)
					{
						num = i;
						now = SprayCanSpray.AllSprays[i].sprayTimestamp;
					}
				}
			}
			if (num2 >= ConVar.Global.MaxSpraysPerPlayer && num != -1)
			{
				SprayCanSpray.AllSprays[num].Kill(global::BaseNetworkable.DestroyMode.None);
			}
		}
		if (deployedBy == null || !deployedBy.IsBuildingAuthed())
		{
			base.Invoke(new Action(this.ApplyOutOfAuthConditionPenalty), 1f);
		}
	}

	// Token: 0x06001229 RID: 4649 RVA: 0x000923C4 File Offset: 0x000905C4
	private void ApplyOutOfAuthConditionPenalty()
	{
		if (!base.IsFullySpawned())
		{
			base.Invoke(new Action(this.ApplyOutOfAuthConditionPenalty), 1f);
			return;
		}
		float amount = this.MaxHealth() * (1f - ConVar.Global.SprayOutOfAuthMultiplier);
		base.Hurt(amount, DamageType.Decay, null, true);
	}

	// Token: 0x0600122A RID: 4650 RVA: 0x00092410 File Offset: 0x00090610
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.RainCheck), 60f, 180f, 30f);
		if (!SprayCanSpray.AllSprays.Contains(this))
		{
			SprayCanSpray.AllSprays.Add(this);
		}
	}

	// Token: 0x0600122B RID: 4651 RVA: 0x0009245C File Offset: 0x0009065C
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (SprayCanSpray.AllSprays.Contains(this))
		{
			SprayCanSpray.AllSprays.Remove(this);
		}
	}

	// Token: 0x0600122C RID: 4652 RVA: 0x0009247D File Offset: 0x0009067D
	private void RainCheck()
	{
		if (Climate.GetRain(base.transform.position) > 0f && this.IsOutside())
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x0600122D RID: 4653 RVA: 0x000924A5 File Offset: 0x000906A5
	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		return amount > 0;
	}

	// Token: 0x0600122E RID: 4654 RVA: 0x000924AB File Offset: 0x000906AB
	public int DoSplash(ItemDefinition splashType, int amount)
	{
		if (!base.IsDestroyed)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
		return 1;
	}

	// Token: 0x0600122F RID: 4655 RVA: 0x000924C0 File Offset: 0x000906C0
	[global::BaseEntity.RPC_Server]
	private void Server_RequestWaterClear(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (!this.Menu_WaterClear_ShowIf(player))
		{
			return;
		}
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x17000184 RID: 388
	// (get) Token: 0x06001230 RID: 4656 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool BypassInsideDecayMultiplier
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001231 RID: 4657 RVA: 0x000924F0 File Offset: 0x000906F0
	private bool Menu_WaterClear_ShowIf(global::BasePlayer player)
	{
		BaseLiquidVessel baseLiquidVessel;
		return player.GetHeldEntity() != null && (baseLiquidVessel = (player.GetHeldEntity() as BaseLiquidVessel)) != null && baseLiquidVessel.AmountHeld() > 0;
	}

	// Token: 0x04000B6C RID: 2924
	public DateTime sprayTimestamp;

	// Token: 0x04000B6D RID: 2925
	public ulong sprayedByPlayer;

	// Token: 0x04000B6E RID: 2926
	public static ListHashSet<SprayCanSpray> AllSprays = new ListHashSet<SprayCanSpray>(8);
}
