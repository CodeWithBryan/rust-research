using System;

// Token: 0x02000146 RID: 326
public class MenuButtonArcadeEntity : TextArcadeEntity
{
	// Token: 0x06001625 RID: 5669 RVA: 0x000A8F99 File Offset: 0x000A7199
	public bool IsHighlighted()
	{
		return this.alpha == 1f;
	}

	// Token: 0x04000EFD RID: 3837
	public string titleText = "";

	// Token: 0x04000EFE RID: 3838
	public string selectionSuffix = " - ";

	// Token: 0x04000EFF RID: 3839
	public string clickMessage = "";
}
