using System;
using UnityEngine;

// Token: 0x02000293 RID: 659
public struct MeshInstance
{
	// Token: 0x17000219 RID: 537
	// (get) Token: 0x06001C19 RID: 7193 RVA: 0x000C27F9 File Offset: 0x000C09F9
	// (set) Token: 0x06001C1A RID: 7194 RVA: 0x000C2806 File Offset: 0x000C0A06
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

	// Token: 0x04001559 RID: 5465
	public Vector3 position;

	// Token: 0x0400155A RID: 5466
	public Quaternion rotation;

	// Token: 0x0400155B RID: 5467
	public Vector3 scale;

	// Token: 0x0400155C RID: 5468
	public MeshCache.Data data;
}
