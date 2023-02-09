using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200004D RID: 77
public class CameraTool : HeldEntity
{
	// Token: 0x060008C3 RID: 2243 RVA: 0x00053730 File Offset: 0x00051930
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CameraTool.OnRpcMessage", 0))
		{
			if (rpc == 3167878597U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SVNoteScreenshot ");
				}
				using (TimeWarning.New("SVNoteScreenshot", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(3167878597U, "SVNoteScreenshot", this, player))
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
							this.SVNoteScreenshot(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SVNoteScreenshot");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060008C4 RID: 2244 RVA: 0x000059DD File Offset: 0x00003BDD
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.FromOwner]
	private void SVNoteScreenshot(BaseEntity.RPCMessage msg)
	{
	}

	// Token: 0x040005D9 RID: 1497
	public GameObjectRef screenshotEffect;
}
