using System;
using UnityEngine;

// Token: 0x0200031E RID: 798
public class EmissionScaledByLight : MonoBehaviour, IClientComponent
{
	// Token: 0x0400174E RID: 5966
	private Color emissionColor;

	// Token: 0x0400174F RID: 5967
	public Renderer[] targetRenderers;

	// Token: 0x04001750 RID: 5968
	public int materialIndex = -1;

	// Token: 0x04001751 RID: 5969
	private static MaterialPropertyBlock block;

	// Token: 0x04001752 RID: 5970
	public Light lightToFollow;

	// Token: 0x04001753 RID: 5971
	public float maxEmissionValue = 3f;
}
