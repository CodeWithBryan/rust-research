using System;
using UnityEngine;

// Token: 0x020003D2 RID: 978
[CreateAssetMenu(menuName = "Rust/Growable Gene Properties")]
public class GrowableGeneProperties : ScriptableObject
{
	// Token: 0x040019AE RID: 6574
	[ArrayIndexIsEnum(enumType = typeof(GrowableGenetics.GeneType))]
	public GrowableGeneProperties.GeneWeight[] Weights = new GrowableGeneProperties.GeneWeight[5];

	// Token: 0x02000C73 RID: 3187
	[Serializable]
	public struct GeneWeight
	{
		// Token: 0x0400425D RID: 16989
		public float BaseWeight;

		// Token: 0x0400425E RID: 16990
		public float[] SlotWeights;

		// Token: 0x0400425F RID: 16991
		public float CrossBreedingWeight;
	}
}
