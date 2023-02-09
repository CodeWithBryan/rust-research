using System;
using UnityEngine;

// Token: 0x020004D0 RID: 1232
public class DeployVolumeSphere : DeployVolume
{
	// Token: 0x0600278C RID: 10124 RVA: 0x000F2FA4 File Offset: 0x000F11A4
	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		position += rotation * (this.worldRotation * this.center + this.worldPosition);
		return DeployVolume.CheckSphere(position, this.radius, this.layers & mask, this);
	}

	// Token: 0x0600278D RID: 10125 RVA: 0x000F2FFC File Offset: 0x000F11FC
	protected override bool Check(Vector3 position, Quaternion rotation, OBB obb, int mask = -1)
	{
		position += rotation * (this.worldRotation * this.center + this.worldPosition);
		return (this.layers & mask) != 0 && Vector3.Distance(position, obb.ClosestPoint(position)) <= this.radius;
	}

	// Token: 0x04001FC0 RID: 8128
	public Vector3 center = Vector3.zero;

	// Token: 0x04001FC1 RID: 8129
	public float radius = 0.5f;
}
