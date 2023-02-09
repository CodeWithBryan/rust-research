using System;
using UnityEngine;

// Token: 0x0200052F RID: 1327
[CreateAssetMenu(menuName = "Rust/Clothing Movement Properties")]
public class ClothingMovementProperties : ScriptableObject
{
	// Token: 0x0400210D RID: 8461
	public float speedReduction;

	// Token: 0x0400210E RID: 8462
	[Tooltip("If this piece of clothing is worn movement speed will be reduced by atleast this much")]
	public float minSpeedReduction;

	// Token: 0x0400210F RID: 8463
	public float waterSpeedBonus;
}
