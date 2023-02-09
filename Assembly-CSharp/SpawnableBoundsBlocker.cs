using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200061F RID: 1567
public class SpawnableBoundsBlocker : MonoBehaviour
{
	// Token: 0x06002CF4 RID: 11508 RVA: 0x0010DAD0 File Offset: 0x0010BCD0
	[Button("Clear Trees")]
	public void ClearTrees()
	{
		List<TreeEntity> list = Pool.GetList<TreeEntity>();
		if (this.BoxCollider != null)
		{
			GamePhysics.OverlapOBB<TreeEntity>(new OBB(base.transform.TransformPoint(this.BoxCollider.center), this.BoxCollider.size + Vector3.one, base.transform.rotation), list, 1073741824, QueryTriggerInteraction.Collide);
		}
		foreach (TreeEntity treeEntity in list)
		{
			BoundsCheck boundsCheck = PrefabAttribute.server.Find<BoundsCheck>(treeEntity.prefabID);
			if (boundsCheck != null && boundsCheck.IsType == this.BlockType)
			{
				treeEntity.Kill(BaseNetworkable.DestroyMode.None);
			}
		}
		Pool.FreeList<TreeEntity>(ref list);
	}

	// Token: 0x040024EC RID: 9452
	public BoundsCheck.BlockType BlockType;

	// Token: 0x040024ED RID: 9453
	public BoxCollider BoxCollider;
}
