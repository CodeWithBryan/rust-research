using System;
using UnityEngine;

// Token: 0x02000228 RID: 552
public class SoundFade : MonoBehaviour, IClientComponent
{
	// Token: 0x040013DA RID: 5082
	public SoundFadeHQAudioFilter hqFadeFilter;

	// Token: 0x040013DB RID: 5083
	public float currentGain = 1f;

	// Token: 0x040013DC RID: 5084
	public float startingGain;

	// Token: 0x040013DD RID: 5085
	public float finalGain = 1f;

	// Token: 0x040013DE RID: 5086
	public int sampleRate = 44100;

	// Token: 0x040013DF RID: 5087
	public bool highQualityFadeCompleted;

	// Token: 0x040013E0 RID: 5088
	public float length;

	// Token: 0x040013E1 RID: 5089
	public SoundFade.Direction currentDirection;

	// Token: 0x02000C2C RID: 3116
	public enum Direction
	{
		// Token: 0x0400411B RID: 16667
		In,
		// Token: 0x0400411C RID: 16668
		Out
	}
}
