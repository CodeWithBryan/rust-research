using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;

// Token: 0x0200025B RID: 603
public class SocketMod_EntityCheck : SocketMod
{
	// Token: 0x06001B85 RID: 7045 RVA: 0x000BFB40 File Offset: 0x000BDD40
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = (this.wantsCollide ? new Color(0f, 1f, 0f, 0.7f) : new Color(1f, 0f, 0f, 0.7f));
		Gizmos.DrawSphere(Vector3.zero, this.sphereRadius);
	}

	// Token: 0x06001B86 RID: 7046 RVA: 0x000BFBB0 File Offset: 0x000BDDB0
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 position = place.position + place.rotation * this.worldPosition;
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(position, this.sphereRadius, list, this.layerMask.value, this.queryTriggers);
		using (List<BaseEntity>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				BaseEntity ent = enumerator.Current;
				bool flag = this.entityTypes.Any((BaseEntity x) => x.prefabID == ent.prefabID);
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
		}
		Pool.FreeList<BaseEntity>(ref list);
		return !this.wantsCollide;
	}

	// Token: 0x0400148E RID: 5262
	public float sphereRadius = 1f;

	// Token: 0x0400148F RID: 5263
	public LayerMask layerMask;

	// Token: 0x04001490 RID: 5264
	public QueryTriggerInteraction queryTriggers;

	// Token: 0x04001491 RID: 5265
	public BaseEntity[] entityTypes;

	// Token: 0x04001492 RID: 5266
	public bool wantsCollide;
}
