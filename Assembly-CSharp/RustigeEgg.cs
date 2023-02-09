using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000BF RID: 191
public class RustigeEgg : BaseCombatEntity
{
	// Token: 0x060010F1 RID: 4337 RVA: 0x000899B8 File Offset: 0x00087BB8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RustigeEgg.OnRpcMessage", 0))
		{
			if (rpc == 4254195175U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Open ");
				}
				using (TimeWarning.New("RPC_Open", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(4254195175U, "RPC_Open", this, player, 3f))
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
							this.RPC_Open(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Open");
					}
				}
				return true;
			}
			if (rpc == 1455840454U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Spin ");
				}
				using (TimeWarning.New("RPC_Spin", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1455840454U, "RPC_Spin", this, player, 3f))
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
							this.RPC_Spin(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_Spin");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060010F2 RID: 4338 RVA: 0x00020A80 File Offset: 0x0001EC80
	public bool IsSpinning()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x060010F3 RID: 4339 RVA: 0x000059DD File Offset: 0x00003BDD
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Spin(BaseEntity.RPCMessage msg)
	{
	}

	// Token: 0x060010F4 RID: 4340 RVA: 0x00089CB8 File Offset: 0x00087EB8
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Open(BaseEntity.RPCMessage msg)
	{
		if (msg.player == null)
		{
			return;
		}
		bool flag = msg.read.Bit();
		if (flag == base.IsOpen())
		{
			return;
		}
		if (flag)
		{
			base.ClientRPC<Vector3>(null, "FaceEggPosition", msg.player.eyes.position);
			base.Invoke(new Action(this.CloseEgg), 60f);
		}
		else
		{
			base.CancelInvoke(new Action(this.CloseEgg));
		}
		base.SetFlag(BaseEntity.Flags.Open, flag, false, false);
		if (this.IsSpinning() && flag)
		{
			base.SetFlag(BaseEntity.Flags.Reserved1, false, false, false);
		}
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x060010F5 RID: 4341 RVA: 0x00089D5D File Offset: 0x00087F5D
	public void CloseEgg()
	{
		base.SetFlag(BaseEntity.Flags.Open, false, false, true);
	}

	// Token: 0x04000A88 RID: 2696
	public const BaseEntity.Flags Flag_Spin = BaseEntity.Flags.Reserved1;

	// Token: 0x04000A89 RID: 2697
	public Transform eggRotationTransform;
}
