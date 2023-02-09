using System;

// Token: 0x02000517 RID: 1303
public class ParticleCollisionLOD : LODComponentParticleSystem
{
	// Token: 0x040020C2 RID: 8386
	[Horizontal(1, 0)]
	public ParticleCollisionLOD.State[] States;

	// Token: 0x02000CE7 RID: 3303
	public enum QualityLevel
	{
		// Token: 0x0400443D RID: 17469
		Disabled = -1,
		// Token: 0x0400443E RID: 17470
		HighQuality,
		// Token: 0x0400443F RID: 17471
		MediumQuality,
		// Token: 0x04004440 RID: 17472
		LowQuality
	}

	// Token: 0x02000CE8 RID: 3304
	[Serializable]
	public class State
	{
		// Token: 0x04004441 RID: 17473
		public float distance;

		// Token: 0x04004442 RID: 17474
		public ParticleCollisionLOD.QualityLevel quality = ParticleCollisionLOD.QualityLevel.Disabled;
	}
}
