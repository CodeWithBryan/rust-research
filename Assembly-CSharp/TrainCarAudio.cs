using System;
using UnityEngine;

// Token: 0x0200048B RID: 1163
public class TrainCarAudio : MonoBehaviour
{
	// Token: 0x04001E8A RID: 7818
	[Header("Train Car Audio")]
	[SerializeField]
	private TrainCar trainCar;

	// Token: 0x04001E8B RID: 7819
	[SerializeField]
	private SoundDefinition movementStartDef;

	// Token: 0x04001E8C RID: 7820
	[SerializeField]
	private SoundDefinition movementStopDef;

	// Token: 0x04001E8D RID: 7821
	[SerializeField]
	private SoundDefinition movementLoopDef;

	// Token: 0x04001E8E RID: 7822
	[SerializeField]
	private AnimationCurve movementLoopGainCurve;

	// Token: 0x04001E8F RID: 7823
	[SerializeField]
	private float movementChangeOneshotDebounce = 1f;

	// Token: 0x04001E90 RID: 7824
	private Sound movementLoop;

	// Token: 0x04001E91 RID: 7825
	private SoundModulation.Modulator movementLoopGain;

	// Token: 0x04001E92 RID: 7826
	[SerializeField]
	private SoundDefinition turnLoopDef;

	// Token: 0x04001E93 RID: 7827
	private Sound turnLoop;

	// Token: 0x04001E94 RID: 7828
	[SerializeField]
	private SoundDefinition trackClatterLoopDef;

	// Token: 0x04001E95 RID: 7829
	[SerializeField]
	private AnimationCurve trackClatterGainCurve;

	// Token: 0x04001E96 RID: 7830
	[SerializeField]
	private AnimationCurve trackClatterPitchCurve;

	// Token: 0x04001E97 RID: 7831
	private Sound trackClatterLoop;

	// Token: 0x04001E98 RID: 7832
	private SoundModulation.Modulator trackClatterGain;

	// Token: 0x04001E99 RID: 7833
	private SoundModulation.Modulator trackClatterPitch;
}
