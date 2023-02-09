using System;
using UnityEngine;

// Token: 0x02000938 RID: 2360
public class FishingRodViewmodel : MonoBehaviour
{
	// Token: 0x0400322C RID: 12844
	public Transform PitchTransform;

	// Token: 0x0400322D RID: 12845
	public Transform YawTransform;

	// Token: 0x0400322E RID: 12846
	public float YawLerpSpeed = 1f;

	// Token: 0x0400322F RID: 12847
	public float PitchLerpSpeed = 1f;

	// Token: 0x04003230 RID: 12848
	public Transform LineRendererStartPos;

	// Token: 0x04003231 RID: 12849
	public ParticleSystem[] StrainParticles;

	// Token: 0x04003232 RID: 12850
	public bool ApplyTransformRotation = true;

	// Token: 0x04003233 RID: 12851
	public GameObject CatchRoot;

	// Token: 0x04003234 RID: 12852
	public Transform CatchLinePoint;

	// Token: 0x04003235 RID: 12853
	public FishingRodViewmodel.FishViewmodel[] FishViewmodels;

	// Token: 0x04003236 RID: 12854
	public float ShakeMaxScale = 0.1f;

	// Token: 0x02000E69 RID: 3689
	[Serializable]
	public struct FishViewmodel
	{
		// Token: 0x04004A56 RID: 19030
		public ItemDefinition Item;

		// Token: 0x04004A57 RID: 19031
		public GameObject Root;
	}
}
