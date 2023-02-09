using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000D3 RID: 211
public class StaticInstrument : BaseMountable
{
	// Token: 0x0600125E RID: 4702 RVA: 0x0009360C File Offset: 0x0009180C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("StaticInstrument.OnRpcMessage", 0))
		{
			if (rpc == 1625188589U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_PlayNote ");
				}
				using (TimeWarning.New("Server_PlayNote", 0))
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
							this.Server_PlayNote(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_PlayNote");
					}
				}
				return true;
			}
			if (rpc == 705843933U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_StopNote ");
				}
				using (TimeWarning.New("Server_StopNote", 0))
				{
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
							this.Server_StopNote(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in Server_StopNote");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600125F RID: 4703 RVA: 0x0009386C File Offset: 0x00091A6C
	[BaseEntity.RPC_Server]
	private void Server_PlayNote(BaseEntity.RPCMessage msg)
	{
		int arg = msg.read.Int32();
		int arg2 = msg.read.Int32();
		int arg3 = msg.read.Int32();
		float arg4 = msg.read.Float();
		this.KeyController.ProcessServerPlayedNote(base.GetMounted());
		base.ClientRPC<int, int, int, float>(null, "Client_PlayNote", arg, arg2, arg3, arg4);
	}

	// Token: 0x06001260 RID: 4704 RVA: 0x000938CC File Offset: 0x00091ACC
	[BaseEntity.RPC_Server]
	private void Server_StopNote(BaseEntity.RPCMessage msg)
	{
		int arg = msg.read.Int32();
		int arg2 = msg.read.Int32();
		int arg3 = msg.read.Int32();
		base.ClientRPC<int, int, int>(null, "Client_StopNote", arg, arg2, arg3);
	}

	// Token: 0x06001261 RID: 4705 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool IsInstrument()
	{
		return true;
	}

	// Token: 0x04000B82 RID: 2946
	public AnimatorOverrideController AnimatorOverride;

	// Token: 0x04000B83 RID: 2947
	public bool ShowDeployAnimation;

	// Token: 0x04000B84 RID: 2948
	public InstrumentKeyController KeyController;

	// Token: 0x04000B85 RID: 2949
	public bool ShouldSuppressHandsAnimationLayer;
}
