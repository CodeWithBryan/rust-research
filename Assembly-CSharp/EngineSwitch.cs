using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200006D RID: 109
public class EngineSwitch : BaseEntity
{
	// Token: 0x06000A81 RID: 2689 RVA: 0x0005E818 File Offset: 0x0005CA18
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("EngineSwitch.OnRpcMessage", 0))
		{
			if (rpc == 1249530220U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - StartEngine ");
				}
				using (TimeWarning.New("StartEngine", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(1249530220U, "StartEngine", this, player, 3f))
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
							this.StartEngine(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in StartEngine");
					}
				}
				return true;
			}
			if (rpc == 1739656243U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - StopEngine ");
				}
				using (TimeWarning.New("StopEngine", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(1739656243U, "StopEngine", this, player, 3f))
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
							this.StopEngine(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in StopEngine");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A82 RID: 2690 RVA: 0x0005EB18 File Offset: 0x0005CD18
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	public void StopEngine(BaseEntity.RPCMessage msg)
	{
		MiningQuarry miningQuarry = base.GetParentEntity() as MiningQuarry;
		if (miningQuarry)
		{
			miningQuarry.EngineSwitch(false);
		}
	}

	// Token: 0x06000A83 RID: 2691 RVA: 0x0005EB40 File Offset: 0x0005CD40
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	public void StartEngine(BaseEntity.RPCMessage msg)
	{
		MiningQuarry miningQuarry = base.GetParentEntity() as MiningQuarry;
		if (miningQuarry)
		{
			miningQuarry.EngineSwitch(true);
		}
	}
}
