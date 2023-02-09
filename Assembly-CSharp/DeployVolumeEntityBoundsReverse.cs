using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020004CE RID: 1230
public class DeployVolumeEntityBoundsReverse : DeployVolume
{
	// Token: 0x06002785 RID: 10117 RVA: 0x000F2D6C File Offset: 0x000F0F6C
	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		position += rotation * this.bounds.center;
		OBB test = new OBB(position, this.bounds.size, rotation);
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(position, test.extents.magnitude, list, this.layers & mask, QueryTriggerInteraction.Collide);
		foreach (BaseEntity baseEntity in list)
		{
			DeployVolume[] volumes = PrefabAttribute.server.FindAll<DeployVolume>(baseEntity.prefabID);
			if (DeployVolume.Check(baseEntity.transform.position, baseEntity.transform.rotation, volumes, test, 1 << this.layer))
			{
				Pool.FreeList<BaseEntity>(ref list);
				return true;
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		return false;
	}

	// Token: 0x06002786 RID: 10118 RVA: 0x00007074 File Offset: 0x00005274
	protected override bool Check(Vector3 position, Quaternion rotation, OBB test, int mask = -1)
	{
		return false;
	}

	// Token: 0x06002787 RID: 10119 RVA: 0x000F2E5C File Offset: 0x000F105C
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.bounds = rootObj.GetComponent<BaseEntity>().bounds;
		this.layer = rootObj.layer;
	}

	// Token: 0x04001FBD RID: 8125
	private Bounds bounds = new Bounds(Vector3.zero, Vector3.one);

	// Token: 0x04001FBE RID: 8126
	private int layer;
}
