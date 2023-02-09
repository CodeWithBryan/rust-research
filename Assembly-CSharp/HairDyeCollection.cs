using System;
using UnityEngine;

// Token: 0x02000722 RID: 1826
[CreateAssetMenu(menuName = "Rust/Hair Dye Collection")]
public class HairDyeCollection : ScriptableObject
{
	// Token: 0x060032BC RID: 12988 RVA: 0x00139A98 File Offset: 0x00137C98
	public HairDye Get(float seed)
	{
		if (this.Variations.Length != 0)
		{
			return this.Variations[Mathf.Clamp(Mathf.FloorToInt(seed * (float)this.Variations.Length), 0, this.Variations.Length - 1)];
		}
		return null;
	}

	// Token: 0x04002907 RID: 10503
	public Texture capMask;

	// Token: 0x04002908 RID: 10504
	public bool applyCap;

	// Token: 0x04002909 RID: 10505
	public HairDye[] Variations;
}
