using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200025C RID: 604
public class SocketMod_EntityType : SocketMod
{
	// Token: 0x06001B88 RID: 7048 RVA: 0x000BFCB0 File Offset: 0x000BDEB0
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = (this.wantsCollide ? new Color(0f, 1f, 0f, 0.7f) : new Color(1f, 0f, 0f, 0.7f));
		Gizmos.DrawSphere(Vector3.zero, this.sphereRadius);
	}

	// Token: 0x06001B89 RID: 7049 RVA: 0x000BFD20 File Offset: 0x000BDF20
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 position = place.position + place.rotation * this.worldPosition;
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(position, this.sphereRadius, list, this.layerMask.value, this.queryTriggers);
		foreach (BaseEntity baseEntity in list)
		{
			bool flag = baseEntity.GetType().IsAssignableFrom(this.searchType.GetType());
			if (flag && this.wantsCollide)
			{
				Pool.FreeList<BaseEntity>(ref list);
				return true;
			}
			if (flag && !this.wantsCollide)
			{
				Pool.FreeList<BaseEntity>(ref list);
				return false;
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		return !this.wantsCollide;
	}

	// Token: 0x04001493 RID: 5267
	public float sphereRadius = 1f;

	// Token: 0x04001494 RID: 5268
	public LayerMask layerMask;

	// Token: 0x04001495 RID: 5269
	public QueryTriggerInteraction queryTriggers;

	// Token: 0x04001496 RID: 5270
	public BaseEntity searchType;

	// Token: 0x04001497 RID: 5271
	public bool wantsCollide;
}
