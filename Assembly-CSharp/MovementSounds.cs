using System;
using UnityEngine;

// Token: 0x020005F3 RID: 1523
public class MovementSounds : MonoBehaviour
{
	// Token: 0x0400245E RID: 9310
	public SoundDefinition waterMovementDef;

	// Token: 0x0400245F RID: 9311
	public float waterMovementFadeInSpeed = 1f;

	// Token: 0x04002460 RID: 9312
	public float waterMovementFadeOutSpeed = 1f;

	// Token: 0x04002461 RID: 9313
	public SoundDefinition enterWaterSmall;

	// Token: 0x04002462 RID: 9314
	public SoundDefinition enterWaterMedium;

	// Token: 0x04002463 RID: 9315
	public SoundDefinition enterWaterLarge;

	// Token: 0x04002464 RID: 9316
	private Sound waterMovement;

	// Token: 0x04002465 RID: 9317
	private SoundModulation.Modulator waterGainMod;

	// Token: 0x04002466 RID: 9318
	public bool inWater;

	// Token: 0x04002467 RID: 9319
	public float waterLevel;

	// Token: 0x04002468 RID: 9320
	public bool mute;
}
