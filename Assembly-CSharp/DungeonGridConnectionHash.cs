using System;

// Token: 0x02000645 RID: 1605
public struct DungeonGridConnectionHash
{
	// Token: 0x1700038E RID: 910
	// (get) Token: 0x06002DFB RID: 11771 RVA: 0x001141A6 File Offset: 0x001123A6
	public int Value
	{
		get
		{
			return (this.North ? 1 : 0) | (this.South ? 2 : 0) | (this.West ? 4 : 0) | (this.East ? 8 : 0);
		}
	}

	// Token: 0x0400259C RID: 9628
	public bool North;

	// Token: 0x0400259D RID: 9629
	public bool South;

	// Token: 0x0400259E RID: 9630
	public bool West;

	// Token: 0x0400259F RID: 9631
	public bool East;
}
