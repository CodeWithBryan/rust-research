using System;
using System.Collections.Generic;

// Token: 0x020008EF RID: 2287
public static class LinqEx
{
	// Token: 0x0600369F RID: 13983 RVA: 0x00144F60 File Offset: 0x00143160
	public static int MaxIndex<T>(this IEnumerable<T> sequence) where T : IComparable<T>
	{
		int num = -1;
		T other = default(T);
		int num2 = 0;
		foreach (T t in sequence)
		{
			if (t.CompareTo(other) > 0 || num == -1)
			{
				num = num2;
				other = t;
			}
			num2++;
		}
		return num;
	}
}
