using System;
using UnityEngine;

// Token: 0x02000969 RID: 2409
public struct OccludeeSphere
{
	// Token: 0x17000443 RID: 1091
	// (get) Token: 0x060038C9 RID: 14537 RVA: 0x0014EDB4 File Offset: 0x0014CFB4
	public bool IsRegistered
	{
		get
		{
			return this.id >= 0;
		}
	}

	// Token: 0x060038CA RID: 14538 RVA: 0x0014EDC2 File Offset: 0x0014CFC2
	public void Invalidate()
	{
		this.id = -1;
		this.state = null;
		this.sphere = default(OcclusionCulling.Sphere);
	}

	// Token: 0x060038CB RID: 14539 RVA: 0x0014EDDE File Offset: 0x0014CFDE
	public OccludeeSphere(int id)
	{
		this.id = id;
		this.state = ((id < 0) ? null : OcclusionCulling.GetStateById(id));
		this.sphere = new OcclusionCulling.Sphere(Vector3.zero, 0f);
	}

	// Token: 0x060038CC RID: 14540 RVA: 0x0014EE0F File Offset: 0x0014D00F
	public OccludeeSphere(int id, OcclusionCulling.Sphere sphere)
	{
		this.id = id;
		this.state = ((id < 0) ? null : OcclusionCulling.GetStateById(id));
		this.sphere = sphere;
	}

	// Token: 0x04003330 RID: 13104
	public int id;

	// Token: 0x04003331 RID: 13105
	public OccludeeState state;

	// Token: 0x04003332 RID: 13106
	public OcclusionCulling.Sphere sphere;
}
