using System;
using UnityEngine;

// Token: 0x02000012 RID: 18
public class FireworkShell : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x0400003B RID: 59
	public float fuseLengthMin;

	// Token: 0x0400003C RID: 60
	public float fuseLengthMax;

	// Token: 0x0400003D RID: 61
	public float speedMin;

	// Token: 0x0400003E RID: 62
	public float speedMax;

	// Token: 0x0400003F RID: 63
	public ParticleSystem explodePFX;

	// Token: 0x04000040 RID: 64
	public SoundPlayer explodeSound;

	// Token: 0x04000041 RID: 65
	public float inaccuracyDegrees;

	// Token: 0x04000042 RID: 66
	public LightEx explosionLight;

	// Token: 0x04000043 RID: 67
	public float lifetime = 8f;
}
