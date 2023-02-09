using System;

// Token: 0x020003C7 RID: 967
public class Upkeep : PrefabAttribute
{
	// Token: 0x06002106 RID: 8454 RVA: 0x000D5C52 File Offset: 0x000D3E52
	protected override Type GetIndexedType()
	{
		return typeof(Upkeep);
	}

	// Token: 0x04001981 RID: 6529
	public float upkeepMultiplier = 1f;
}
