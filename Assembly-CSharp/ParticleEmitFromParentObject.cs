using System;
using UnityEngine;

// Token: 0x0200032B RID: 811
public class ParticleEmitFromParentObject : MonoBehaviour
{
	// Token: 0x04001789 RID: 6025
	public string bonename;

	// Token: 0x0400178A RID: 6026
	private Bounds bounds;

	// Token: 0x0400178B RID: 6027
	private Transform bone;

	// Token: 0x0400178C RID: 6028
	private BaseEntity entity;

	// Token: 0x0400178D RID: 6029
	private float lastBoundsUpdate;
}
