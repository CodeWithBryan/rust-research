using System;
using UnityEngine;

// Token: 0x0200026E RID: 622
public class AnimalAnimation : MonoBehaviour, IClientComponent
{
	// Token: 0x040014CC RID: 5324
	public BaseEntity Entity;

	// Token: 0x040014CD RID: 5325
	public BaseNpc Target;

	// Token: 0x040014CE RID: 5326
	public Animator Animator;

	// Token: 0x040014CF RID: 5327
	public MaterialEffect FootstepEffects;

	// Token: 0x040014D0 RID: 5328
	public Transform[] Feet;

	// Token: 0x040014D1 RID: 5329
	public SoundDefinition saddleMovementSoundDef;

	// Token: 0x040014D2 RID: 5330
	public SoundDefinition saddleMovementSoundDefWood;

	// Token: 0x040014D3 RID: 5331
	public SoundDefinition saddleMovementSoundDefRoadsign;

	// Token: 0x040014D4 RID: 5332
	public AnimationCurve saddleMovementGainCurve;

	// Token: 0x040014D5 RID: 5333
	[Tooltip("Ensure there is a float param called idleOffset if this is enabled")]
	public bool hasIdleOffset;

	// Token: 0x040014D6 RID: 5334
	[ReadOnly]
	public string BaseFolder;

	// Token: 0x040014D7 RID: 5335
	public const BaseEntity.Flags Flag_WoodArmor = BaseEntity.Flags.Reserved5;

	// Token: 0x040014D8 RID: 5336
	public const BaseEntity.Flags Flag_RoadsignArmor = BaseEntity.Flags.Reserved6;
}
