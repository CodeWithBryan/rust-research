using System;
using UnityEngine;

// Token: 0x020004CC RID: 1228
public class DeployVolumeCapsule : DeployVolume
{
	// Token: 0x0600277E RID: 10110 RVA: 0x000F2C08 File Offset: 0x000F0E08
	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		position += rotation * (this.worldRotation * this.center + this.worldPosition);
		Vector3 start = position + rotation * this.worldRotation * Vector3.up * this.height * 0.5f;
		Vector3 end = position + rotation * this.worldRotation * Vector3.down * this.height * 0.5f;
		return DeployVolume.CheckCapsule(start, end, this.radius, this.layers & mask, this);
	}

	// Token: 0x0600277F RID: 10111 RVA: 0x00007074 File Offset: 0x00005274
	protected override bool Check(Vector3 position, Quaternion rotation, OBB obb, int mask = -1)
	{
		return false;
	}

	// Token: 0x04001FB9 RID: 8121
	public Vector3 center = Vector3.zero;

	// Token: 0x04001FBA RID: 8122
	public float radius = 0.5f;

	// Token: 0x04001FBB RID: 8123
	public float height = 1f;
}
