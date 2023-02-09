using System;
using Network;
using UnityEngine;

// Token: 0x02000072 RID: 114
public class Flashbang : TimedExplosive
{
	// Token: 0x06000AD2 RID: 2770 RVA: 0x00060CEC File Offset: 0x0005EEEC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Flashbang.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000AD3 RID: 2771 RVA: 0x00060D2C File Offset: 0x0005EF2C
	public override void Explode()
	{
		base.ClientRPC<Vector3>(null, "Client_DoFlash", base.transform.position);
		base.Explode();
	}

	// Token: 0x06000AD4 RID: 2772 RVA: 0x00026D90 File Offset: 0x00024F90
	public void DelayedDestroy()
	{
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x04000712 RID: 1810
	public SoundDefinition deafLoopDef;

	// Token: 0x04000713 RID: 1811
	public float flashReductionPerSecond = 1f;

	// Token: 0x04000714 RID: 1812
	public float flashToAdd = 3f;

	// Token: 0x04000715 RID: 1813
	public float flashMinRange = 5f;

	// Token: 0x04000716 RID: 1814
	public float flashMaxRange = 10f;
}
