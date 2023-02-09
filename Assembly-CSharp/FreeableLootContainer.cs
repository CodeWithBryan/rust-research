using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000075 RID: 117
public class FreeableLootContainer : LootContainer
{
	// Token: 0x06000B0A RID: 2826 RVA: 0x00061C48 File Offset: 0x0005FE48
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("FreeableLootContainer.OnRpcMessage", 0))
		{
			if (rpc == 2202685945U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_FreeCrate ");
				}
				using (TimeWarning.New("RPC_FreeCrate", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(2202685945U, "RPC_FreeCrate", this, player, 3f))
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
							this.RPC_FreeCrate(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_FreeCrate");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000B0B RID: 2827 RVA: 0x00061DB0 File Offset: 0x0005FFB0
	public Rigidbody GetRB()
	{
		if (this.rb == null)
		{
			this.rb = base.GetComponent<Rigidbody>();
		}
		return this.rb;
	}

	// Token: 0x06000B0C RID: 2828 RVA: 0x000028C8 File Offset: 0x00000AC8
	public bool IsTiedDown()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved8);
	}

	// Token: 0x06000B0D RID: 2829 RVA: 0x00061DD4 File Offset: 0x0005FFD4
	public override void ServerInit()
	{
		this.GetRB().isKinematic = true;
		this.buoyancy.buoyancyScale = 0f;
		this.buoyancy.enabled = false;
		base.ServerInit();
		if (this.skinOverride != 0U)
		{
			this.skinID = (ulong)this.skinOverride;
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000B0E RID: 2830 RVA: 0x00061E2C File Offset: 0x0006002C
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_FreeCrate(BaseEntity.RPCMessage msg)
	{
		if (!this.IsTiedDown())
		{
			return;
		}
		this.GetRB().isKinematic = false;
		this.buoyancy.enabled = true;
		this.buoyancy.buoyancyScale = 1f;
		base.SetFlag(BaseEntity.Flags.Reserved8, false, false, true);
		if (this.freedEffect.isValid)
		{
			Effect.server.Run(this.freedEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		}
		BasePlayer player = msg.player;
		if (player)
		{
			player.ProcessMissionEvent(BaseMission.MissionEventType.FREE_CRATE, "", 1f);
			Analytics.Server.FreeUnderwaterCrate();
		}
	}

	// Token: 0x04000724 RID: 1828
	private const BaseEntity.Flags tiedDown = BaseEntity.Flags.Reserved8;

	// Token: 0x04000725 RID: 1829
	public Buoyancy buoyancy;

	// Token: 0x04000726 RID: 1830
	public GameObjectRef freedEffect;

	// Token: 0x04000727 RID: 1831
	private Rigidbody rb;

	// Token: 0x04000728 RID: 1832
	public uint skinOverride;
}
