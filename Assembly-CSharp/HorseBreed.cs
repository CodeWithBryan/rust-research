using System;
using UnityEngine;

// Token: 0x020001F9 RID: 505
[CreateAssetMenu(menuName = "Scriptable Object/Horse Breed", fileName = "newbreed.asset")]
public class HorseBreed : ScriptableObject
{
	// Token: 0x04001291 RID: 4753
	public Translate.Phrase breedName;

	// Token: 0x04001292 RID: 4754
	public Translate.Phrase breedDesc;

	// Token: 0x04001293 RID: 4755
	public Material[] materialOverrides;

	// Token: 0x04001294 RID: 4756
	public float maxHealth = 1f;

	// Token: 0x04001295 RID: 4757
	public float maxSpeed = 1f;

	// Token: 0x04001296 RID: 4758
	public float staminaDrain = 1f;

	// Token: 0x04001297 RID: 4759
	public float maxStamina = 1f;
}
