using System;
using UnityEngine;

// Token: 0x02000455 RID: 1109
public abstract class GroundVehicleAudio : MonoBehaviour, IClientComponent
{
	// Token: 0x04001CFD RID: 7421
	[SerializeField]
	protected GroundVehicle groundVehicle;

	// Token: 0x04001CFE RID: 7422
	[Header("Engine")]
	[SerializeField]
	private SoundDefinition engineStartSound;

	// Token: 0x04001CFF RID: 7423
	[SerializeField]
	private SoundDefinition engineStopSound;

	// Token: 0x04001D00 RID: 7424
	[SerializeField]
	private SoundDefinition engineStartFailSound;

	// Token: 0x04001D01 RID: 7425
	[SerializeField]
	private BlendedLoopEngineSound blendedEngineLoops;

	// Token: 0x04001D02 RID: 7426
	[SerializeField]
	private float wheelRatioMultiplier = 600f;

	// Token: 0x04001D03 RID: 7427
	[Header("Water")]
	[SerializeField]
	private SoundDefinition waterSplashSoundDef;

	// Token: 0x04001D04 RID: 7428
	[SerializeField]
	private BlendedSoundLoops waterLoops;

	// Token: 0x04001D05 RID: 7429
	[SerializeField]
	private float waterSoundsMaxSpeed = 10f;

	// Token: 0x04001D06 RID: 7430
	[Header("Brakes")]
	[SerializeField]
	private SoundDefinition brakeSoundDef;

	// Token: 0x04001D07 RID: 7431
	[Header("Lights")]
	[SerializeField]
	private SoundDefinition lightsToggleSound;
}
