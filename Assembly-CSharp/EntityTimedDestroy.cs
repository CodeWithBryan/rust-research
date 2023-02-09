using System;
using UnityEngine;

// Token: 0x020003AA RID: 938
public class EntityTimedDestroy : EntityComponent<BaseEntity>
{
	// Token: 0x06002058 RID: 8280 RVA: 0x000D32E3 File Offset: 0x000D14E3
	private void OnEnable()
	{
		base.Invoke(new Action(this.TimedDestroy), this.secondsTillDestroy);
	}

	// Token: 0x06002059 RID: 8281 RVA: 0x000D32FD File Offset: 0x000D14FD
	private void TimedDestroy()
	{
		if (base.baseEntity != null)
		{
			base.baseEntity.Kill(BaseNetworkable.DestroyMode.None);
			return;
		}
		Debug.LogWarning("EntityTimedDestroy failed, baseEntity was already null!");
	}

	// Token: 0x04001938 RID: 6456
	public float secondsTillDestroy = 1f;
}
