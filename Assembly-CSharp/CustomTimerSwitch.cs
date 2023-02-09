using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200005E RID: 94
public class CustomTimerSwitch : TimerSwitch
{
	// Token: 0x060009BE RID: 2494 RVA: 0x0005A040 File Offset: 0x00058240
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CustomTimerSwitch.OnRpcMessage", 0))
		{
			if (rpc == 1019813162U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SERVER_SetTime ");
				}
				using (TimeWarning.New("SERVER_SetTime", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1019813162U, "SERVER_SetTime", this, player, 3f))
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
							this.SERVER_SetTime(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SERVER_SetTime");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060009BF RID: 2495 RVA: 0x0005A1A8 File Offset: 0x000583A8
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		base.UpdateFromInput(inputAmount, inputSlot);
		if (inputAmount > 0 && inputSlot == 1)
		{
			base.SwitchPressed();
		}
	}

	// Token: 0x060009C0 RID: 2496 RVA: 0x0005A1C0 File Offset: 0x000583C0
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SERVER_SetTime(BaseEntity.RPCMessage msg)
	{
		if (!this.CanPlayerAdmin(msg.player))
		{
			return;
		}
		float timerLength = msg.read.Float();
		this.timerLength = timerLength;
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060009C1 RID: 2497 RVA: 0x00059F64 File Offset: 0x00058164
	public bool CanPlayerAdmin(BasePlayer player)
	{
		return player != null && player.CanBuild() && !base.IsOn();
	}

	// Token: 0x04000667 RID: 1639
	public GameObjectRef timerPanelPrefab;
}
