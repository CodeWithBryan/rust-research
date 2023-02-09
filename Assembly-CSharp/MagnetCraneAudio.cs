using System;
using UnityEngine;

// Token: 0x0200046A RID: 1130
public class MagnetCraneAudio : MonoBehaviour
{
	// Token: 0x04001D88 RID: 7560
	public MagnetCrane crane;

	// Token: 0x04001D89 RID: 7561
	[Header("Sound defs")]
	public SoundDefinition engineStartSoundDef;

	// Token: 0x04001D8A RID: 7562
	public SoundDefinition engineStopSoundDef;

	// Token: 0x04001D8B RID: 7563
	public BlendedLoopEngineSound engineLoops;

	// Token: 0x04001D8C RID: 7564
	public SoundDefinition cabinRotationStartDef;

	// Token: 0x04001D8D RID: 7565
	public SoundDefinition cabinRotationStopDef;

	// Token: 0x04001D8E RID: 7566
	public SoundDefinition cabinRotationLoopDef;

	// Token: 0x04001D8F RID: 7567
	private Sound cabinRotationLoop;

	// Token: 0x04001D90 RID: 7568
	public SoundDefinition turningLoopDef;

	// Token: 0x04001D91 RID: 7569
	private Sound turningLoop;

	// Token: 0x04001D92 RID: 7570
	public SoundDefinition trackMovementLoopDef;

	// Token: 0x04001D93 RID: 7571
	private Sound trackMovementLoop;

	// Token: 0x04001D94 RID: 7572
	private SoundModulation.Modulator trackGainMod;

	// Token: 0x04001D95 RID: 7573
	private SoundModulation.Modulator trackPitchMod;

	// Token: 0x04001D96 RID: 7574
	public SoundDefinition armMovementLoopDef;

	// Token: 0x04001D97 RID: 7575
	public SoundDefinition armMovementStartDef;

	// Token: 0x04001D98 RID: 7576
	public SoundDefinition armMovementStopDef;

	// Token: 0x04001D99 RID: 7577
	private Sound armMovementLoop01;

	// Token: 0x04001D9A RID: 7578
	private SoundModulation.Modulator armMovementLoop01PitchMod;

	// Token: 0x04001D9B RID: 7579
	public GameObject arm01SoundPosition;

	// Token: 0x04001D9C RID: 7580
	public GameObject arm02SoundPosition;

	// Token: 0x04001D9D RID: 7581
	private Sound armMovementLoop02;

	// Token: 0x04001D9E RID: 7582
	private SoundModulation.Modulator armMovementLoop02PitchMod;
}
