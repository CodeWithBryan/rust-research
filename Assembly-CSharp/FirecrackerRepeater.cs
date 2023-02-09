using System;
using UnityEngine;

// Token: 0x02000137 RID: 311
public class FirecrackerRepeater : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x04000E85 RID: 3717
	public GameObjectRef singleExplosionEffect;

	// Token: 0x04000E86 RID: 3718
	public Transform[] parts;

	// Token: 0x04000E87 RID: 3719
	public float partWidth = 0.2f;

	// Token: 0x04000E88 RID: 3720
	public float partLength = 0.1f;

	// Token: 0x04000E89 RID: 3721
	public Quaternion[] targetRotations;

	// Token: 0x04000E8A RID: 3722
	public Quaternion[] initialRotations;

	// Token: 0x04000E8B RID: 3723
	public Renderer[] renderers;

	// Token: 0x04000E8C RID: 3724
	public Material materialSource;

	// Token: 0x04000E8D RID: 3725
	public float explodeRepeatMin = 0.05f;

	// Token: 0x04000E8E RID: 3726
	public float explodeRepeatMax = 0.15f;

	// Token: 0x04000E8F RID: 3727
	public float explodeLerpSpeed = 30f;

	// Token: 0x04000E90 RID: 3728
	public Vector3 twistAmount;

	// Token: 0x04000E91 RID: 3729
	public float fuseLength = 3f;

	// Token: 0x04000E92 RID: 3730
	public float explodeStrength = 10f;

	// Token: 0x04000E93 RID: 3731
	public float explodeDirBlend = 0.5f;

	// Token: 0x04000E94 RID: 3732
	public float duration = 10f;

	// Token: 0x04000E95 RID: 3733
	public ParticleSystemContainer smokeParticle;
}
