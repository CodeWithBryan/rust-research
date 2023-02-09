using System;
using UnityEngine;

// Token: 0x02000116 RID: 278
public class FlasherLight : IOEntity
{
	// Token: 0x0600158D RID: 5517 RVA: 0x000228A0 File Offset: 0x00020AA0
	public override void ResetState()
	{
		base.ResetState();
	}

	// Token: 0x04000DE3 RID: 3555
	public EmissionToggle toggler;

	// Token: 0x04000DE4 RID: 3556
	public Light myLight;

	// Token: 0x04000DE5 RID: 3557
	public float flashSpacing = 0.2f;

	// Token: 0x04000DE6 RID: 3558
	public float flashBurstSpacing = 0.5f;

	// Token: 0x04000DE7 RID: 3559
	public float flashOnTime = 0.1f;

	// Token: 0x04000DE8 RID: 3560
	public int numFlashesPerBurst = 5;
}
