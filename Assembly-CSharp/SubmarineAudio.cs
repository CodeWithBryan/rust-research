using System;
using UnityEngine;

// Token: 0x02000483 RID: 1155
public class SubmarineAudio : MonoBehaviour
{
	// Token: 0x04001E2C RID: 7724
	[Header("Engine")]
	[SerializeField]
	private SoundDefinition engineStartSound;

	// Token: 0x04001E2D RID: 7725
	[SerializeField]
	private SoundDefinition engineStopSound;

	// Token: 0x04001E2E RID: 7726
	[SerializeField]
	private SoundDefinition engineStartFailSound;

	// Token: 0x04001E2F RID: 7727
	[SerializeField]
	private SoundDefinition engineLoopSound;

	// Token: 0x04001E30 RID: 7728
	[SerializeField]
	private AnimationCurve engineLoopPitchCurve;

	// Token: 0x04001E31 RID: 7729
	[Header("Water")]
	[SerializeField]
	private SoundDefinition underwaterLoopDef;

	// Token: 0x04001E32 RID: 7730
	[SerializeField]
	private SoundDefinition underwaterMovementLoopDef;

	// Token: 0x04001E33 RID: 7731
	[SerializeField]
	private BlendedSoundLoops surfaceWaterLoops;

	// Token: 0x04001E34 RID: 7732
	[SerializeField]
	private float surfaceWaterSoundsMaxSpeed = 5f;

	// Token: 0x04001E35 RID: 7733
	[SerializeField]
	private SoundDefinition waterEmergeSoundDef;

	// Token: 0x04001E36 RID: 7734
	[SerializeField]
	private SoundDefinition waterSubmergeSoundDef;

	// Token: 0x04001E37 RID: 7735
	[Header("Interior")]
	[SerializeField]
	private SoundDefinition activeLoopDef;

	// Token: 0x04001E38 RID: 7736
	[SerializeField]
	private SoundDefinition footPedalSoundDef;

	// Token: 0x04001E39 RID: 7737
	[SerializeField]
	private Transform footPedalSoundPos;

	// Token: 0x04001E3A RID: 7738
	[SerializeField]
	private SoundDefinition steeringWheelSoundDef;

	// Token: 0x04001E3B RID: 7739
	[SerializeField]
	private Transform steeringWheelSoundPos;

	// Token: 0x04001E3C RID: 7740
	[SerializeField]
	private SoundDefinition heavyDamageSparksDef;

	// Token: 0x04001E3D RID: 7741
	[SerializeField]
	private Transform heavyDamageSparksPos;

	// Token: 0x04001E3E RID: 7742
	[SerializeField]
	private SoundDefinition flagRaise;

	// Token: 0x04001E3F RID: 7743
	[SerializeField]
	private SoundDefinition flagLower;

	// Token: 0x04001E40 RID: 7744
	[Header("Other")]
	[SerializeField]
	private SoundDefinition climbOrDiveLoopSound;

	// Token: 0x04001E41 RID: 7745
	[SerializeField]
	private SoundDefinition sonarBlipSound;

	// Token: 0x04001E42 RID: 7746
	[SerializeField]
	private SoundDefinition torpedoFailedSound;
}
