using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200025A RID: 602
public class SocketMod_BuildingBlock : SocketMod
{
	// Token: 0x06001B82 RID: 7042 RVA: 0x000BFA2C File Offset: 0x000BDC2C
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = (this.wantsCollide ? new Color(0f, 1f, 0f, 0.7f) : new Color(1f, 0f, 0f, 0.7f));
		Gizmos.DrawSphere(Vector3.zero, this.sphereRadius);
	}

	// Token: 0x06001B83 RID: 7043 RVA: 0x000BFA9C File Offset: 0x000BDC9C
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 position = place.position + place.rotation * this.worldPosition;
		List<BuildingBlock> list = Pool.GetList<BuildingBlock>();
		Vis.Entities<BuildingBlock>(position, this.sphereRadius, list, this.layerMask.value, this.queryTriggers);
		bool flag = list.Count > 0;
		if (flag && this.wantsCollide)
		{
			Pool.FreeList<BuildingBlock>(ref list);
			return true;
		}
		if (flag && !this.wantsCollide)
		{
			Pool.FreeList<BuildingBlock>(ref list);
			return false;
		}
		Pool.FreeList<BuildingBlock>(ref list);
		return !this.wantsCollide;
	}

	// Token: 0x0400148A RID: 5258
	public float sphereRadius = 1f;

	// Token: 0x0400148B RID: 5259
	public LayerMask layerMask;

	// Token: 0x0400148C RID: 5260
	public QueryTriggerInteraction queryTriggers;

	// Token: 0x0400148D RID: 5261
	public bool wantsCollide;
}
