using System;
using UnityEngine;

// Token: 0x020004CF RID: 1231
public class DeployVolumeOBB : DeployVolume
{
	// Token: 0x06002789 RID: 10121 RVA: 0x000F2E98 File Offset: 0x000F1098
	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		position += rotation * (this.worldRotation * this.bounds.center + this.worldPosition);
		return DeployVolume.CheckOBB(new OBB(position, this.bounds.size, rotation * this.worldRotation), this.layers & mask, this);
	}

	// Token: 0x0600278A RID: 10122 RVA: 0x000F2F0C File Offset: 0x000F110C
	protected override bool Check(Vector3 position, Quaternion rotation, OBB test, int mask = -1)
	{
		position += rotation * (this.worldRotation * this.bounds.center + this.worldPosition);
		OBB obb = new OBB(position, this.bounds.size, rotation * this.worldRotation);
		return (this.layers & mask) != 0 && obb.Intersects(test);
	}

	// Token: 0x04001FBF RID: 8127
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one);
}
