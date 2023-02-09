using System;
using UnityEngine;

// Token: 0x020002C2 RID: 706
public class QueryVis : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x04001638 RID: 5688
	public Collider checkCollider;

	// Token: 0x04001639 RID: 5689
	private CoverageQueries.Query query;

	// Token: 0x0400163A RID: 5690
	public CoverageQueries.RadiusSpace coverageRadiusSpace = CoverageQueries.RadiusSpace.World;

	// Token: 0x0400163B RID: 5691
	public float coverageRadius = 0.2f;
}
