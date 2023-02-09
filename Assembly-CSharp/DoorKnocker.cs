using System;
using Network;
using UnityEngine;

// Token: 0x02000066 RID: 102
public class DoorKnocker : BaseCombatEntity
{
	// Token: 0x06000A34 RID: 2612 RVA: 0x0005D068 File Offset: 0x0005B268
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("DoorKnocker.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A35 RID: 2613 RVA: 0x0005D0A8 File Offset: 0x0005B2A8
	public void Knock(BasePlayer player)
	{
		base.ClientRPC<Vector3>(null, "ClientKnock", player.transform.position);
	}

	// Token: 0x0400069A RID: 1690
	public Animator knocker1;

	// Token: 0x0400069B RID: 1691
	public Animator knocker2;
}
