using System;
using UnityEngine;

// Token: 0x020003B3 RID: 947
public class BoxStorage : StorageContainer
{
	// Token: 0x0600207A RID: 8314 RVA: 0x000D3FF8 File Offset: 0x000D21F8
	public override Vector3 GetDropPosition()
	{
		return base.ClosestPoint(base.GetDropPosition() + base.LastAttackedDir * 10f);
	}

	// Token: 0x0600207B RID: 8315 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x0600207C RID: 8316 RVA: 0x000A0E77 File Offset: 0x0009F077
	public override bool CanPickup(BasePlayer player)
	{
		return this.children.Count == 0 && base.CanPickup(player);
	}
}
