using System;
using System.Collections.Generic;

// Token: 0x020008EC RID: 2284
public static class CollectionEx
{
	// Token: 0x06003695 RID: 13973 RVA: 0x00144BD1 File Offset: 0x00142DD1
	public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
	{
		return collection == null || collection.Count == 0;
	}

	// Token: 0x06003696 RID: 13974 RVA: 0x00144BE1 File Offset: 0x00142DE1
	public static bool IsEmpty<T>(this ICollection<T> collection)
	{
		return collection.Count == 0;
	}
}
