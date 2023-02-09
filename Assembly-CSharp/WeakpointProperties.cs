using System;

// Token: 0x0200026A RID: 618
public class WeakpointProperties : PrefabAttribute
{
	// Token: 0x06001BC0 RID: 7104 RVA: 0x000C0F05 File Offset: 0x000BF105
	protected override Type GetIndexedType()
	{
		return typeof(WeakpointProperties);
	}

	// Token: 0x040014C6 RID: 5318
	public bool BlockWhenRoofAttached;
}
