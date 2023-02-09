using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000210 RID: 528
public class BlendedSoundLoops : MonoBehaviour, IClientComponent
{
	// Token: 0x04001302 RID: 4866
	[Range(0f, 1f)]
	public float blend;

	// Token: 0x04001303 RID: 4867
	public float blendSmoothing = 1f;

	// Token: 0x04001304 RID: 4868
	public float loopFadeOutTime = 0.5f;

	// Token: 0x04001305 RID: 4869
	public float loopFadeInTime = 0.5f;

	// Token: 0x04001306 RID: 4870
	public float gainModSmoothing = 1f;

	// Token: 0x04001307 RID: 4871
	public float pitchModSmoothing = 1f;

	// Token: 0x04001308 RID: 4872
	public bool shouldPlay = true;

	// Token: 0x04001309 RID: 4873
	public float gain = 1f;

	// Token: 0x0400130A RID: 4874
	public List<BlendedSoundLoops.Loop> loops = new List<BlendedSoundLoops.Loop>();

	// Token: 0x0400130B RID: 4875
	public float maxDistance;

	// Token: 0x02000C1E RID: 3102
	[Serializable]
	public class Loop
	{
		// Token: 0x040040C4 RID: 16580
		public SoundDefinition soundDef;

		// Token: 0x040040C5 RID: 16581
		public AnimationCurve gainCurve;

		// Token: 0x040040C6 RID: 16582
		public AnimationCurve pitchCurve;

		// Token: 0x040040C7 RID: 16583
		[HideInInspector]
		public Sound sound;

		// Token: 0x040040C8 RID: 16584
		[HideInInspector]
		public SoundModulation.Modulator gainMod;

		// Token: 0x040040C9 RID: 16585
		[HideInInspector]
		public SoundModulation.Modulator pitchMod;
	}
}
