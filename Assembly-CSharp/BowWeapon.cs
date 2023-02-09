using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000049 RID: 73
public class BowWeapon : BaseProjectile
{
	// Token: 0x06000823 RID: 2083 RVA: 0x0004E35C File Offset: 0x0004C55C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BowWeapon.OnRpcMessage", 0))
		{
			if (rpc == 4228048190U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - BowReload ");
				}
				using (TimeWarning.New("BowReload", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(4228048190U, "BowReload", this, player))
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
							this.BowReload(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in BowReload");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000824 RID: 2084 RVA: 0x0004E4C0 File Offset: 0x0004C6C0
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void BowReload(BaseEntity.RPCMessage msg)
	{
		this.ReloadMagazine(-1);
	}

	// Token: 0x06000825 RID: 2085 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool ForceSendMagazine(BaseNetworkable.SaveInfo saveInfo)
	{
		return true;
	}
}
