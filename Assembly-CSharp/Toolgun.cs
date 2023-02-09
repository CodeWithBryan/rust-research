using System;
using Network;
using UnityEngine;

// Token: 0x020000DA RID: 218
public class Toolgun : Hammer
{
	// Token: 0x060012D3 RID: 4819 RVA: 0x00096D70 File Offset: 0x00094F70
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Toolgun.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060012D4 RID: 4820 RVA: 0x00096DB0 File Offset: 0x00094FB0
	public override void DoAttackShared(HitInfo info)
	{
		if (base.isServer)
		{
			base.ClientRPC<Vector3, Vector3>(null, "EffectSpawn", info.HitPositionWorld, info.HitNormalWorld);
		}
		base.DoAttackShared(info);
	}

	// Token: 0x04000BB5 RID: 2997
	public GameObjectRef attackEffect;

	// Token: 0x04000BB6 RID: 2998
	public GameObjectRef beamEffect;

	// Token: 0x04000BB7 RID: 2999
	public GameObjectRef beamImpactEffect;

	// Token: 0x04000BB8 RID: 3000
	public GameObjectRef errorEffect;

	// Token: 0x04000BB9 RID: 3001
	public GameObjectRef beamEffectClassic;

	// Token: 0x04000BBA RID: 3002
	public GameObjectRef beamImpactEffectClassic;

	// Token: 0x04000BBB RID: 3003
	public Transform muzzlePoint;
}
