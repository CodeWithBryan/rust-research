using System;
using UnityEngine;

// Token: 0x02000296 RID: 662
public struct MeshRendererInstance
{
	// Token: 0x1700021A RID: 538
	// (get) Token: 0x06001C23 RID: 7203 RVA: 0x000C30A4 File Offset: 0x000C12A4
	// (set) Token: 0x06001C24 RID: 7204 RVA: 0x000C30B1 File Offset: 0x000C12B1
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

	// Token: 0x04001565 RID: 5477
	public Renderer renderer;

	// Token: 0x04001566 RID: 5478
	public OBB bounds;

	// Token: 0x04001567 RID: 5479
	public Vector3 position;

	// Token: 0x04001568 RID: 5480
	public Quaternion rotation;

	// Token: 0x04001569 RID: 5481
	public Vector3 scale;

	// Token: 0x0400156A RID: 5482
	public MeshCache.Data data;
}
