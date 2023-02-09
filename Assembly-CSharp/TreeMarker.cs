using System;
using Network;
using UnityEngine;

// Token: 0x0200000F RID: 15
public class TreeMarker : BaseEntity
{
	// Token: 0x06000025 RID: 37 RVA: 0x00002850 File Offset: 0x00000A50
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("TreeMarker.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0400002C RID: 44
	public GameObjectRef hitEffect;

	// Token: 0x0400002D RID: 45
	public SoundDefinition hitEffectSound;

	// Token: 0x0400002E RID: 46
	public GameObjectRef spawnEffect;

	// Token: 0x0400002F RID: 47
	private Vector3 initialPosition;

	// Token: 0x04000030 RID: 48
	public bool SpherecastOnInit = true;
}
