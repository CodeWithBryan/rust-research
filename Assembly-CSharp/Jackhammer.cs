using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000085 RID: 133
public class Jackhammer : BaseMelee
{
	// Token: 0x06000C98 RID: 3224 RVA: 0x0006B7D0 File Offset: 0x000699D0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Jackhammer.OnRpcMessage", 0))
		{
			if (rpc == 1699910227U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_SetEngineStatus ");
				}
				using (TimeWarning.New("Server_SetEngineStatus", 0))
				{
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
							this.Server_SetEngineStatus(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_SetEngineStatus");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000C99 RID: 3225 RVA: 0x00003A54 File Offset: 0x00001C54
	public bool HasAmmo()
	{
		return true;
	}

	// Token: 0x06000C9A RID: 3226 RVA: 0x0006B8F4 File Offset: 0x00069AF4
	[BaseEntity.RPC_Server]
	public void Server_SetEngineStatus(BaseEntity.RPCMessage msg)
	{
		this.SetEngineStatus(msg.read.Bit());
	}

	// Token: 0x06000C9B RID: 3227 RVA: 0x0006B907 File Offset: 0x00069B07
	public void SetEngineStatus(bool on)
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, on, false, true);
	}

	// Token: 0x06000C9C RID: 3228 RVA: 0x0006B917 File Offset: 0x00069B17
	public override void SetHeld(bool bHeld)
	{
		if (!bHeld)
		{
			this.SetEngineStatus(false);
		}
		base.SetHeld(bHeld);
	}

	// Token: 0x04000802 RID: 2050
	public float HotspotBonusScale = 1f;
}
