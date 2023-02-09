using System;
using UnityEngine;

// Token: 0x02000494 RID: 1172
[Serializable]
public class VisualCarWheel : CarWheel
{
	// Token: 0x04001EF4 RID: 7924
	public Transform visualWheel;

	// Token: 0x04001EF5 RID: 7925
	public Transform visualWheelSteering;

	// Token: 0x04001EF6 RID: 7926
	public bool visualPowerWheel = true;

	// Token: 0x04001EF7 RID: 7927
	public ParticleSystem snowFX;

	// Token: 0x04001EF8 RID: 7928
	public ParticleSystem sandFX;

	// Token: 0x04001EF9 RID: 7929
	public ParticleSystem dirtFX;

	// Token: 0x04001EFA RID: 7930
	public ParticleSystem asphaltFX;

	// Token: 0x04001EFB RID: 7931
	public ParticleSystem waterFX;

	// Token: 0x04001EFC RID: 7932
	public ParticleSystem snowSpinFX;

	// Token: 0x04001EFD RID: 7933
	public ParticleSystem sandSpinFX;

	// Token: 0x04001EFE RID: 7934
	public ParticleSystem dirtSpinFX;

	// Token: 0x04001EFF RID: 7935
	public ParticleSystem asphaltSpinFX;
}
