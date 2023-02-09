using System;
using UnityEngine;

// Token: 0x02000529 RID: 1321
public class PrefabInformation : PrefabAttribute
{
	// Token: 0x06002895 RID: 10389 RVA: 0x000F6E8D File Offset: 0x000F508D
	protected override Type GetIndexedType()
	{
		return typeof(PrefabInformation);
	}

	// Token: 0x040020F6 RID: 8438
	public ItemDefinition associatedItemDefinition;

	// Token: 0x040020F7 RID: 8439
	public Translate.Phrase title;

	// Token: 0x040020F8 RID: 8440
	public Translate.Phrase description;

	// Token: 0x040020F9 RID: 8441
	public Sprite sprite;

	// Token: 0x040020FA RID: 8442
	public bool shownOnDeathScreen;
}
