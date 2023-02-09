using System;
using UnityEngine;

// Token: 0x0200040F RID: 1039
public class ExcavatorEffects : MonoBehaviour
{
	// Token: 0x04001B43 RID: 6979
	public static ExcavatorEffects instance;

	// Token: 0x04001B44 RID: 6980
	public ParticleSystemContainer[] miningParticles;

	// Token: 0x04001B45 RID: 6981
	public SoundPlayer[] miningSounds;

	// Token: 0x04001B46 RID: 6982
	public SoundFollowCollider[] beltSounds;

	// Token: 0x04001B47 RID: 6983
	public SoundPlayer[] miningStartSounds;

	// Token: 0x04001B48 RID: 6984
	public GameObject[] ambientMetalRattles;

	// Token: 0x04001B49 RID: 6985
	public bool wasMining;
}
