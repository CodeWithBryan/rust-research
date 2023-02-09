using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000D6 RID: 214
public class SurveyCrater : BaseCombatEntity
{
	// Token: 0x06001296 RID: 4758 RVA: 0x000944C8 File Offset: 0x000926C8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SurveyCrater.OnRpcMessage", 0))
		{
			if (rpc == 3491246334U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - AnalysisComplete ");
				}
				using (TimeWarning.New("AnalysisComplete", 0))
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
							this.AnalysisComplete(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in AnalysisComplete");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001297 RID: 4759 RVA: 0x000945EC File Offset: 0x000927EC
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.RemoveMe), 1800f);
	}

	// Token: 0x06001298 RID: 4760 RVA: 0x0009460B File Offset: 0x0009280B
	public override void OnAttacked(HitInfo info)
	{
		bool isServer = base.isServer;
		base.OnAttacked(info);
	}

	// Token: 0x06001299 RID: 4761 RVA: 0x000029D4 File Offset: 0x00000BD4
	public void RemoveMe()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x0600129A RID: 4762 RVA: 0x000059DD File Offset: 0x00003BDD
	[BaseEntity.RPC_Server]
	public void AnalysisComplete(BaseEntity.RPCMessage msg)
	{
	}

	// Token: 0x0600129B RID: 4763 RVA: 0x000300D2 File Offset: 0x0002E2D2
	public override float BoundsPadding()
	{
		return 2f;
	}

	// Token: 0x04000B9E RID: 2974
	private ResourceDispenser resourceDispenser;
}
