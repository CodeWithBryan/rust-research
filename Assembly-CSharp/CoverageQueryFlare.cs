using System;

// Token: 0x0200016B RID: 363
public class CoverageQueryFlare : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x04000F9A RID: 3994
	public bool isDynamic;

	// Token: 0x04000F9B RID: 3995
	public bool timeShimmer;

	// Token: 0x04000F9C RID: 3996
	public bool positionalShimmer;

	// Token: 0x04000F9D RID: 3997
	public bool rotate;

	// Token: 0x04000F9E RID: 3998
	public float maxVisibleDistance = 30f;

	// Token: 0x04000F9F RID: 3999
	public bool lightScaled;

	// Token: 0x04000FA0 RID: 4000
	public float dotMin = -1f;

	// Token: 0x04000FA1 RID: 4001
	public float dotMax = -1f;

	// Token: 0x04000FA2 RID: 4002
	public CoverageQueries.RadiusSpace coverageRadiusSpace;

	// Token: 0x04000FA3 RID: 4003
	public float coverageRadius = 0.01f;

	// Token: 0x04000FA4 RID: 4004
	public LODDistanceMode DistanceMode;
}
