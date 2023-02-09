using System;

// Token: 0x0200029B RID: 667
public abstract class DecalComponent : PrefabAttribute
{
	// Token: 0x06001C2F RID: 7215 RVA: 0x000C3239 File Offset: 0x000C1439
	protected override Type GetIndexedType()
	{
		return typeof(DecalComponent);
	}
}
