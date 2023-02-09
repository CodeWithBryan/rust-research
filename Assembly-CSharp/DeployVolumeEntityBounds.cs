using System;
using UnityEngine;

// Token: 0x020004CD RID: 1229
public class DeployVolumeEntityBounds : DeployVolume
{
	// Token: 0x06002781 RID: 10113 RVA: 0x000F2CEC File Offset: 0x000F0EEC
	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		position += rotation * this.bounds.center;
		return DeployVolume.CheckOBB(new OBB(position, this.bounds.size, rotation), this.layers & mask, this);
	}

	// Token: 0x06002782 RID: 10114 RVA: 0x00007074 File Offset: 0x00005274
	protected override bool Check(Vector3 position, Quaternion rotation, OBB obb, int mask = -1)
	{
		return false;
	}

	// Token: 0x06002783 RID: 10115 RVA: 0x000F2D3C File Offset: 0x000F0F3C
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.bounds = rootObj.GetComponent<BaseEntity>().bounds;
	}

	// Token: 0x04001FBC RID: 8124
	private Bounds bounds = new Bounds(Vector3.zero, Vector3.one);
}
