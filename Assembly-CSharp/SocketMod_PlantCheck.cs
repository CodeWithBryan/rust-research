using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000260 RID: 608
public class SocketMod_PlantCheck : SocketMod
{
	// Token: 0x06001B94 RID: 7060 RVA: 0x000C002C File Offset: 0x000BE22C
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = (this.wantsCollide ? new Color(0f, 1f, 0f, 0.7f) : new Color(1f, 0f, 0f, 0.7f));
		Gizmos.DrawSphere(Vector3.zero, this.sphereRadius);
	}

	// Token: 0x06001B95 RID: 7061 RVA: 0x000C009C File Offset: 0x000BE29C
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 position = place.position + place.rotation * this.worldPosition;
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(position, this.sphereRadius, list, this.layerMask.value, this.queryTriggers);
		foreach (BaseEntity baseEntity in list)
		{
			GrowableEntity component = baseEntity.GetComponent<GrowableEntity>();
			if (component && this.wantsCollide)
			{
				Pool.FreeList<BaseEntity>(ref list);
				return true;
			}
			if (component && !this.wantsCollide)
			{
				Pool.FreeList<BaseEntity>(ref list);
				return false;
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		return !this.wantsCollide;
	}

	// Token: 0x0400149E RID: 5278
	public float sphereRadius = 1f;

	// Token: 0x0400149F RID: 5279
	public LayerMask layerMask;

	// Token: 0x040014A0 RID: 5280
	public QueryTriggerInteraction queryTriggers;

	// Token: 0x040014A1 RID: 5281
	public bool wantsCollide;
}
