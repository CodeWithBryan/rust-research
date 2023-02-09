using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000C6 RID: 198
public class SleepingBagCamper : global::SleepingBag
{
	// Token: 0x0600118A RID: 4490 RVA: 0x0008DD50 File Offset: 0x0008BF50
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SleepingBagCamper.OnRpcMessage", 0))
		{
			if (rpc == 2177887503U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerClearBed ");
				}
				using (TimeWarning.New("ServerClearBed", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2177887503U, "ServerClearBed", this, player, 3f))
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
							this.ServerClearBed(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ServerClearBed");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600118B RID: 4491 RVA: 0x0008DEB8 File Offset: 0x0008C0B8
	public override void ServerInit()
	{
		base.ServerInit();
		base.SetFlag(global::BaseEntity.Flags.Reserved3, true, false, true);
	}

	// Token: 0x0600118C RID: 4492 RVA: 0x0008DED0 File Offset: 0x0008C0D0
	protected override void PostPlayerSpawn(global::BasePlayer p)
	{
		base.PostPlayerSpawn(p);
		BaseVehicleSeat baseVehicleSeat = this.AssociatedSeat.Get(base.isServer);
		if (baseVehicleSeat != null)
		{
			p.EndSleeping();
			baseVehicleSeat.MountPlayer(p);
		}
	}

	// Token: 0x0600118D RID: 4493 RVA: 0x0008DF0C File Offset: 0x0008C10C
	public void SetSeat(BaseVehicleSeat seat, bool sendNetworkUpdate = false)
	{
		this.AssociatedSeat.Set(seat);
		if (sendNetworkUpdate)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x0600118E RID: 4494 RVA: 0x0008DF24 File Offset: 0x0008C124
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.sleepingBagCamper = Facepunch.Pool.Get<ProtoBuf.SleepingBagCamper>();
			info.msg.sleepingBagCamper.seatID = this.AssociatedSeat.uid;
		}
	}

	// Token: 0x0600118F RID: 4495 RVA: 0x0008DF60 File Offset: 0x0008C160
	public override bool IsOccupied()
	{
		return (this.AssociatedSeat.IsValid(base.isServer) && this.AssociatedSeat.Get(base.isServer).AnyMounted()) || WaterLevel.Test(base.transform.position, true, null);
	}

	// Token: 0x06001190 RID: 4496 RVA: 0x0008DFAC File Offset: 0x0008C1AC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void ServerClearBed(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null || !this.AssociatedSeat.IsValid(base.isServer) || this.AssociatedSeat.Get(base.isServer).GetMounted() != player)
		{
			return;
		}
		this.deployerUserID = 0UL;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x04000AEA RID: 2794
	public EntityRef<BaseVehicleSeat> AssociatedSeat;
}
