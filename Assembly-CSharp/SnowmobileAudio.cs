using System;
using UnityEngine;

// Token: 0x02000480 RID: 1152
public class SnowmobileAudio : GroundVehicleAudio
{
	// Token: 0x04001E0A RID: 7690
	[Header("Engine")]
	[SerializeField]
	private EngineAudioSet engineAudioSet;

	// Token: 0x04001E0B RID: 7691
	[Header("Skis")]
	[SerializeField]
	private AnimationCurve skiGainCurve;

	// Token: 0x04001E0C RID: 7692
	[SerializeField]
	private SoundDefinition skiSlideSoundDef;

	// Token: 0x04001E0D RID: 7693
	[SerializeField]
	private SoundDefinition skiSlideSnowSoundDef;

	// Token: 0x04001E0E RID: 7694
	[SerializeField]
	private SoundDefinition skiSlideSandSoundDef;

	// Token: 0x04001E0F RID: 7695
	[SerializeField]
	private SoundDefinition skiSlideGrassSoundDef;

	// Token: 0x04001E10 RID: 7696
	[SerializeField]
	private SoundDefinition skiSlideWaterSoundDef;

	// Token: 0x04001E11 RID: 7697
	[Header("Movement")]
	[SerializeField]
	private AnimationCurve movementGainCurve;

	// Token: 0x04001E12 RID: 7698
	[SerializeField]
	private SoundDefinition movementLoopDef;

	// Token: 0x04001E13 RID: 7699
	[SerializeField]
	private SoundDefinition suspensionLurchSoundDef;

	// Token: 0x04001E14 RID: 7700
	[SerializeField]
	private float suspensionLurchMinExtensionDelta = 0.4f;

	// Token: 0x04001E15 RID: 7701
	[SerializeField]
	private float suspensionLurchMinTimeBetweenSounds = 0.25f;
}
