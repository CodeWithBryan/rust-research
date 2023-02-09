using System;

// Token: 0x02000731 RID: 1841
[Serializable]
public class SkinReplacement
{
	// Token: 0x0400295A RID: 10586
	public SkinReplacement.SkinType skinReplacementType;

	// Token: 0x0400295B RID: 10587
	public GameObjectRef targetReplacement;

	// Token: 0x02000E11 RID: 3601
	public enum SkinType
	{
		// Token: 0x040048E3 RID: 18659
		NONE,
		// Token: 0x040048E4 RID: 18660
		Hands,
		// Token: 0x040048E5 RID: 18661
		Head,
		// Token: 0x040048E6 RID: 18662
		Feet,
		// Token: 0x040048E7 RID: 18663
		Torso,
		// Token: 0x040048E8 RID: 18664
		Legs
	}
}
