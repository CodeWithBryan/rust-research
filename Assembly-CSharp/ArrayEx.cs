using System;
using UnityEngine;

// Token: 0x020008EA RID: 2282
public static class ArrayEx
{
	// Token: 0x06003689 RID: 13961 RVA: 0x0014453F File Offset: 0x0014273F
	public static T[] New<T>(int length)
	{
		if (length == 0)
		{
			return Array.Empty<T>();
		}
		return new T[length];
	}

	// Token: 0x0600368A RID: 13962 RVA: 0x00144550 File Offset: 0x00142750
	public static T GetRandom<T>(this T[] array)
	{
		if (array == null || array.Length == 0)
		{
			return default(T);
		}
		return array[UnityEngine.Random.Range(0, array.Length)];
	}

	// Token: 0x0600368B RID: 13963 RVA: 0x00144580 File Offset: 0x00142780
	public static T GetRandom<T>(this T[] array, uint seed)
	{
		if (array == null || array.Length == 0)
		{
			return default(T);
		}
		return array[SeedRandom.Range(ref seed, 0, array.Length)];
	}

	// Token: 0x0600368C RID: 13964 RVA: 0x001445B0 File Offset: 0x001427B0
	public static T GetRandom<T>(this T[] array, ref uint seed)
	{
		if (array == null || array.Length == 0)
		{
			return default(T);
		}
		return array[SeedRandom.Range(ref seed, 0, array.Length)];
	}

	// Token: 0x0600368D RID: 13965 RVA: 0x001445DE File Offset: 0x001427DE
	public static void Shuffle<T>(this T[] array, uint seed)
	{
		array.Shuffle(ref seed);
	}

	// Token: 0x0600368E RID: 13966 RVA: 0x001445E8 File Offset: 0x001427E8
	public static void Shuffle<T>(this T[] array, ref uint seed)
	{
		for (int i = 0; i < array.Length; i++)
		{
			int num = SeedRandom.Range(ref seed, 0, array.Length);
			int num2 = SeedRandom.Range(ref seed, 0, array.Length);
			T t = array[num];
			array[num] = array[num2];
			array[num2] = t;
		}
	}

	// Token: 0x0600368F RID: 13967 RVA: 0x00144638 File Offset: 0x00142838
	public static void BubbleSort<T>(this T[] array) where T : IComparable<T>
	{
		for (int i = 1; i < array.Length; i++)
		{
			T t = array[i];
			for (int j = i - 1; j >= 0; j--)
			{
				T t2 = array[j];
				if (t.CompareTo(t2) >= 0)
				{
					break;
				}
				array[j + 1] = t2;
				array[j] = t;
			}
		}
	}
}
