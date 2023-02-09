using System;
using UnityEngine;

// Token: 0x0200050B RID: 1291
public abstract class LODComponentParticleSystem : LODComponent
{
	// Token: 0x040020A7 RID: 8359
	[Tooltip("Automatically call Play() the particle system when it's shown via LOD")]
	public bool playOnShow = true;
}
