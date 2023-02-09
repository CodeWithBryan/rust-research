using System;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000AE4 RID: 2788
	public class ModularCarAudio : GroundVehicleAudio
	{
		// Token: 0x04003B9B RID: 15259
		public bool showDebug;

		// Token: 0x04003B9C RID: 15260
		[Header("Skid")]
		[SerializeField]
		private SoundDefinition skidSoundLoop;

		// Token: 0x04003B9D RID: 15261
		[SerializeField]
		private SoundDefinition skidSoundDirtLoop;

		// Token: 0x04003B9E RID: 15262
		[SerializeField]
		private SoundDefinition skidSoundSnowLoop;

		// Token: 0x04003B9F RID: 15263
		[SerializeField]
		private float skidMinSlip = 10f;

		// Token: 0x04003BA0 RID: 15264
		[SerializeField]
		private float skidMaxSlip = 25f;

		// Token: 0x04003BA1 RID: 15265
		[Header("Movement & Suspension")]
		[SerializeField]
		private SoundDefinition movementStartOneshot;

		// Token: 0x04003BA2 RID: 15266
		[SerializeField]
		private SoundDefinition movementStopOneshot;

		// Token: 0x04003BA3 RID: 15267
		[SerializeField]
		private float movementStartStopMinTimeBetweenSounds = 0.25f;

		// Token: 0x04003BA4 RID: 15268
		[SerializeField]
		private SoundDefinition movementRattleLoop;

		// Token: 0x04003BA5 RID: 15269
		[SerializeField]
		private float movementRattleMaxSpeed = 10f;

		// Token: 0x04003BA6 RID: 15270
		[SerializeField]
		private float movementRattleMaxAngSpeed = 10f;

		// Token: 0x04003BA7 RID: 15271
		[SerializeField]
		private float movementRattleIdleGain = 0.3f;

		// Token: 0x04003BA8 RID: 15272
		[SerializeField]
		private SoundDefinition suspensionLurchSound;

		// Token: 0x04003BA9 RID: 15273
		[SerializeField]
		private float suspensionLurchMinExtensionDelta = 0.4f;

		// Token: 0x04003BAA RID: 15274
		[SerializeField]
		private float suspensionLurchMinTimeBetweenSounds = 0.25f;

		// Token: 0x04003BAB RID: 15275
		[Header("Wheels")]
		[SerializeField]
		private SoundDefinition tyreRollingSoundDef;

		// Token: 0x04003BAC RID: 15276
		[SerializeField]
		private SoundDefinition tyreRollingWaterSoundDef;

		// Token: 0x04003BAD RID: 15277
		[SerializeField]
		private SoundDefinition tyreRollingGrassSoundDef;

		// Token: 0x04003BAE RID: 15278
		[SerializeField]
		private SoundDefinition tyreRollingSnowSoundDef;

		// Token: 0x04003BAF RID: 15279
		[SerializeField]
		private AnimationCurve tyreRollGainCurve;
	}
}
