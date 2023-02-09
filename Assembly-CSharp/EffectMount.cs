using System;
using UnityEngine;

// Token: 0x020001B2 RID: 434
public class EffectMount : EntityComponent<BaseEntity>, IClientComponent
{
	// Token: 0x040010FF RID: 4351
	public bool firstPerson;

	// Token: 0x04001100 RID: 4352
	public GameObject effectPrefab;

	// Token: 0x04001101 RID: 4353
	public GameObject spawnedEffect;

	// Token: 0x04001102 RID: 4354
	public GameObject mountBone;

	// Token: 0x04001103 RID: 4355
	public SoundDefinition onSoundDef;

	// Token: 0x04001104 RID: 4356
	public SoundDefinition offSoundDef;

	// Token: 0x04001105 RID: 4357
	public bool blockOffSoundWhenGettingDisabled;
}
