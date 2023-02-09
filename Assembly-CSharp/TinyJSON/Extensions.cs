using System;
using System.Collections.Generic;

namespace TinyJSON
{
	// Token: 0x02000996 RID: 2454
	public static class Extensions
	{
		// Token: 0x06003A2E RID: 14894 RVA: 0x001570B4 File Offset: 0x001552B4
		public static bool AnyOfType<TSource>(this IEnumerable<TSource> source, Type expectedType)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (expectedType == null)
			{
				throw new ArgumentNullException("expectedType");
			}
			foreach (TSource tsource in source)
			{
				if (expectedType.IsInstanceOfType(tsource))
				{
					return true;
				}
			}
			return false;
		}
	}
}
