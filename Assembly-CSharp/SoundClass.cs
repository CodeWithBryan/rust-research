using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000225 RID: 549
[CreateAssetMenu(menuName = "Rust/Sound Class")]
public class SoundClass : ScriptableObject
{
	// Token: 0x040013B2 RID: 5042
	[Header("Mixer Settings")]
	public AudioMixerGroup output;

	// Token: 0x040013B3 RID: 5043
	public AudioMixerGroup firstPersonOutput;

	// Token: 0x040013B4 RID: 5044
	[Header("Occlusion Settings")]
	public bool enableOcclusion;

	// Token: 0x040013B5 RID: 5045
	public bool playIfOccluded = true;

	// Token: 0x040013B6 RID: 5046
	public float occlusionGain = 1f;

	// Token: 0x040013B7 RID: 5047
	[Tooltip("Use this mixer group when the sound is occluded to save DSP CPU usage. Only works for non-looping sounds.")]
	public AudioMixerGroup occludedOutput;

	// Token: 0x040013B8 RID: 5048
	[Header("Voice Limiting")]
	public int globalVoiceMaxCount = 100;

	// Token: 0x040013B9 RID: 5049
	public int priority = 128;

	// Token: 0x040013BA RID: 5050
	public List<SoundDefinition> definitions = new List<SoundDefinition>();
}
