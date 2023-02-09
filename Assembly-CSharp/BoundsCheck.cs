using System;

// Token: 0x0200061D RID: 1565
public class BoundsCheck : PrefabAttribute
{
	// Token: 0x06002CF1 RID: 11505 RVA: 0x0010D9B3 File Offset: 0x0010BBB3
	protected override Type GetIndexedType()
	{
		return typeof(BoundsCheck);
	}

	// Token: 0x040024EB RID: 9451
	public BoundsCheck.BlockType IsType;

	// Token: 0x02000D3D RID: 3389
	public enum BlockType
	{
		// Token: 0x0400459E RID: 17822
		Tree
	}
}
