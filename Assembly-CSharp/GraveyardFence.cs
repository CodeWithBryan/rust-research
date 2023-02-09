using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000157 RID: 343
public class GraveyardFence : SimpleBuildingBlock
{
	// Token: 0x06001654 RID: 5716 RVA: 0x000A9DD1 File Offset: 0x000A7FD1
	public override void ServerInit()
	{
		base.ServerInit();
		this.UpdatePillars();
	}

	// Token: 0x06001655 RID: 5717 RVA: 0x000A9DE0 File Offset: 0x000A7FE0
	public override void DestroyShared()
	{
		base.DestroyShared();
		List<GraveyardFence> list = Pool.GetList<GraveyardFence>();
		Vis.Entities<GraveyardFence>(base.transform.position, 5f, list, 2097152, QueryTriggerInteraction.Collide);
		foreach (GraveyardFence graveyardFence in list)
		{
			graveyardFence.UpdatePillars();
		}
		Pool.FreeList<GraveyardFence>(ref list);
	}

	// Token: 0x06001656 RID: 5718 RVA: 0x000A9E5C File Offset: 0x000A805C
	public virtual void UpdatePillars()
	{
		foreach (BoxCollider boxCollider in this.pillars)
		{
			boxCollider.gameObject.SetActive(true);
			foreach (Collider collider in Physics.OverlapBox(boxCollider.transform.TransformPoint(boxCollider.center), boxCollider.size * 0.5f, boxCollider.transform.rotation, 2097152))
			{
				if (collider.CompareTag("Usable Auxiliary"))
				{
					BaseEntity baseEntity = collider.gameObject.ToBaseEntity();
					if (!(baseEntity == null) && !base.EqualNetID(baseEntity) && collider != boxCollider)
					{
						boxCollider.gameObject.SetActive(false);
					}
				}
			}
		}
	}

	// Token: 0x04000F40 RID: 3904
	public BoxCollider[] pillars;
}
