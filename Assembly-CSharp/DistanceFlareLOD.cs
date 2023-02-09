using System;

// Token: 0x020008C9 RID: 2249
public class DistanceFlareLOD : FacepunchBehaviour, ILOD, IClientComponent
{
	// Token: 0x04003125 RID: 12581
	public bool isDynamic;

	// Token: 0x04003126 RID: 12582
	public float minEnabledDistance = 100f;

	// Token: 0x04003127 RID: 12583
	public float maxEnabledDistance = 600f;

	// Token: 0x04003128 RID: 12584
	public bool toggleFade;

	// Token: 0x04003129 RID: 12585
	public float toggleFadeDuration = 0.5f;
}
