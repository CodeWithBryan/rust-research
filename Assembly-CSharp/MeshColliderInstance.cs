using System;
using UnityEngine;

// Token: 0x0200028F RID: 655
public struct MeshColliderInstance
{
	// Token: 0x17000218 RID: 536
	// (get) Token: 0x06001C0B RID: 7179 RVA: 0x000C20D4 File Offset: 0x000C02D4
	// (set) Token: 0x06001C0C RID: 7180 RVA: 0x000C20E1 File Offset: 0x000C02E1
	public Mesh mesh
	{
		get
		{
			return this.data.mesh;
		}
		set
		{
			this.data = MeshCache.Get(value);
		}
	}

	// Token: 0x04001547 RID: 5447
	public Transform transform;

	// Token: 0x04001548 RID: 5448
	public Rigidbody rigidbody;

	// Token: 0x04001549 RID: 5449
	public Collider collider;

	// Token: 0x0400154A RID: 5450
	public OBB bounds;

	// Token: 0x0400154B RID: 5451
	public Vector3 position;

	// Token: 0x0400154C RID: 5452
	public Quaternion rotation;

	// Token: 0x0400154D RID: 5453
	public Vector3 scale;

	// Token: 0x0400154E RID: 5454
	public MeshCache.Data data;
}
