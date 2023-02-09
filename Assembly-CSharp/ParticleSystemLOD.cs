using System;
using UnityEngine;

// Token: 0x02000519 RID: 1305
public class ParticleSystemLOD : LODComponentParticleSystem
{
	// Token: 0x040020C4 RID: 8388
	[Horizontal(1, 0)]
	public ParticleSystemLOD.State[] States;

	// Token: 0x02000CE9 RID: 3305
	[Serializable]
	public class State
	{
		// Token: 0x04004443 RID: 17475
		public float distance;

		// Token: 0x04004444 RID: 17476
		[Range(0f, 1f)]
		public float emission;
	}
}
