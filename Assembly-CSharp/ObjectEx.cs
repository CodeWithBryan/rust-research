using System;

// Token: 0x020008F1 RID: 2289
public static class ObjectEx
{
	// Token: 0x060036A1 RID: 13985 RVA: 0x0014528E File Offset: 0x0014348E
	public static bool IsUnityNull<T>(this T obj) where T : class
	{
		return obj == null || obj.Equals(null);
	}
}
