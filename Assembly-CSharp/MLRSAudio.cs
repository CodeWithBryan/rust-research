using System;
using UnityEngine;

// Token: 0x02000461 RID: 1121
public class MLRSAudio : MonoBehaviour
{
	// Token: 0x04001D4C RID: 7500
	[SerializeField]
	private MLRS mlrs;

	// Token: 0x04001D4D RID: 7501
	[SerializeField]
	private Transform pitchTransform;

	// Token: 0x04001D4E RID: 7502
	[SerializeField]
	private Transform yawTransform;

	// Token: 0x04001D4F RID: 7503
	[SerializeField]
	private float pitchDeltaSmoothRate = 5f;

	// Token: 0x04001D50 RID: 7504
	[SerializeField]
	private float yawDeltaSmoothRate = 5f;

	// Token: 0x04001D51 RID: 7505
	[SerializeField]
	private float pitchDeltaThreshold = 0.5f;

	// Token: 0x04001D52 RID: 7506
	[SerializeField]
	private float yawDeltaThreshold = 0.5f;

	// Token: 0x04001D53 RID: 7507
	private float lastPitch;

	// Token: 0x04001D54 RID: 7508
	private float lastYaw;

	// Token: 0x04001D55 RID: 7509
	private float pitchDelta;

	// Token: 0x04001D56 RID: 7510
	private float yawDelta;

	// Token: 0x04001D57 RID: 7511
	public SoundDefinition turretMovementStartDef;

	// Token: 0x04001D58 RID: 7512
	public SoundDefinition turretMovementLoopDef;

	// Token: 0x04001D59 RID: 7513
	public SoundDefinition turretMovementStopDef;

	// Token: 0x04001D5A RID: 7514
	private Sound turretMovementLoop;
}
