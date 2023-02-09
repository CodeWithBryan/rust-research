using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000622 RID: 1570
public static class ImageProcessing
{
	// Token: 0x06002D00 RID: 11520 RVA: 0x0010DF18 File Offset: 0x0010C118
	public static void GaussianBlur2D(float[] data, int len1, int len2, int iterations = 1)
	{
		float[] array = data;
		float[] array2 = new float[len1 * len2];
		for (int i = 0; i < iterations; i++)
		{
			for (int j = 0; j < len1; j++)
			{
				int num = Mathf.Max(0, j - 1);
				int num2 = Mathf.Min(len1 - 1, j + 1);
				for (int k = 0; k < len2; k++)
				{
					int num3 = Mathf.Max(0, k - 1);
					int num4 = Mathf.Min(len2 - 1, k + 1);
					float num5 = array[j * len2 + k] * 4f + array[j * len2 + num3] + array[j * len2 + num4] + array[num * len2 + k] + array[num2 * len2 + k];
					array2[j * len2 + k] = num5 * 0.125f;
				}
			}
			GenericsUtil.Swap<float[]>(ref array, ref array2);
		}
		if (array != data)
		{
			Buffer.BlockCopy(array, 0, data, 0, data.Length * 4);
		}
	}

	// Token: 0x06002D01 RID: 11521 RVA: 0x0010DFF8 File Offset: 0x0010C1F8
	public static void GaussianBlur2D(float[] data, int len1, int len2, int len3, int iterations = 1)
	{
		float[] src = data;
		float[] dst = new float[len1 * len2 * len3];
		Action<int> <>9__0;
		for (int i = 0; i < iterations; i++)
		{
			int fromInclusive = 0;
			int len4 = len1;
			Action<int> action;
			if ((action = <>9__0) == null)
			{
				action = (<>9__0 = delegate(int x)
				{
					int num = Mathf.Max(0, x - 1);
					int num2 = Mathf.Min(len1 - 1, x + 1);
					for (int j = 0; j < len2; j++)
					{
						int num3 = Mathf.Max(0, j - 1);
						int num4 = Mathf.Min(len2 - 1, j + 1);
						for (int k = 0; k < len3; k++)
						{
							float num5 = src[(x * len2 + j) * len3 + k] * 4f + src[(x * len2 + num3) * len3 + k] + src[(x * len2 + num4) * len3 + k] + src[(num * len2 + j) * len3 + k] + src[(num2 * len2 + j) * len3 + k];
							dst[(x * len2 + j) * len3 + k] = num5 * 0.125f;
						}
					}
				});
			}
			Parallel.For(fromInclusive, len4, action);
			GenericsUtil.Swap<float[]>(ref src, ref dst);
		}
		if (src != data)
		{
			Buffer.BlockCopy(src, 0, data, 0, data.Length * 4);
		}
	}

	// Token: 0x06002D02 RID: 11522 RVA: 0x0010E0AC File Offset: 0x0010C2AC
	public static void Average2D(float[] data, int len1, int len2, int iterations = 1)
	{
		float[] src = data;
		float[] dst = new float[len1 * len2];
		Action<int> <>9__0;
		for (int i = 0; i < iterations; i++)
		{
			int fromInclusive = 0;
			int len3 = len1;
			Action<int> action;
			if ((action = <>9__0) == null)
			{
				action = (<>9__0 = delegate(int x)
				{
					int num = Mathf.Max(0, x - 1);
					int num2 = Mathf.Min(len1 - 1, x + 1);
					for (int j = 0; j < len2; j++)
					{
						int num3 = Mathf.Max(0, j - 1);
						int num4 = Mathf.Min(len2 - 1, j + 1);
						float num5 = src[x * len2 + j] + src[x * len2 + num3] + src[x * len2 + num4] + src[num * len2 + j] + src[num2 * len2 + j];
						dst[x * len2 + j] = num5 * 0.2f;
					}
				});
			}
			Parallel.For(fromInclusive, len3, action);
			GenericsUtil.Swap<float[]>(ref src, ref dst);
		}
		if (src != data)
		{
			Buffer.BlockCopy(src, 0, data, 0, data.Length * 4);
		}
	}

	// Token: 0x06002D03 RID: 11523 RVA: 0x0010E150 File Offset: 0x0010C350
	public static void Average2D(float[] data, int len1, int len2, int len3, int iterations = 1)
	{
		float[] src = data;
		float[] dst = new float[len1 * len2 * len3];
		Action<int> <>9__0;
		for (int i = 0; i < iterations; i++)
		{
			int fromInclusive = 0;
			int len4 = len1;
			Action<int> action;
			if ((action = <>9__0) == null)
			{
				action = (<>9__0 = delegate(int x)
				{
					int num = Mathf.Max(0, x - 1);
					int num2 = Mathf.Min(len1 - 1, x + 1);
					for (int j = 0; j < len2; j++)
					{
						int num3 = Mathf.Max(0, j - 1);
						int num4 = Mathf.Min(len2 - 1, j + 1);
						for (int k = 0; k < len3; k++)
						{
							float num5 = src[(x * len2 + j) * len3 + k] + src[(x * len2 + num3) * len3 + k] + src[(x * len2 + num4) * len3 + k] + src[(num * len2 + j) * len3 + k] + src[(num2 * len2 + j) * len3 + k];
							dst[(x * len2 + j) * len3 + k] = num5 * 0.2f;
						}
					}
				});
			}
			Parallel.For(fromInclusive, len4, action);
			GenericsUtil.Swap<float[]>(ref src, ref dst);
		}
		if (src != data)
		{
			Buffer.BlockCopy(src, 0, data, 0, data.Length * 4);
		}
	}

	// Token: 0x06002D04 RID: 11524 RVA: 0x0010E204 File Offset: 0x0010C404
	public static void Upsample2D(float[] src, int srclen1, int srclen2, float[] dst, int dstlen1, int dstlen2)
	{
		if (2 * srclen1 != dstlen1 || 2 * srclen2 != dstlen2)
		{
			return;
		}
		Parallel.For(0, srclen1, delegate(int x)
		{
			int num = Mathf.Max(0, x - 1);
			int num2 = Mathf.Min(srclen1 - 1, x + 1);
			for (int i = 0; i < srclen2; i++)
			{
				int num3 = Mathf.Max(0, i - 1);
				int num4 = Mathf.Min(srclen2 - 1, i + 1);
				float num5 = src[x * srclen2 + i] * 6f;
				float num6 = num5 + src[num * srclen2 + i] + src[x * srclen2 + num3];
				dst[2 * x * dstlen2 + 2 * i] = num6 * 0.125f;
				float num7 = num5 + src[num2 * srclen2 + i] + src[x * srclen2 + num3];
				dst[(2 * x + 1) * dstlen2 + 2 * i] = num7 * 0.125f;
				float num8 = num5 + src[num * srclen2 + i] + src[x * srclen2 + num4];
				dst[2 * x * dstlen2 + (2 * i + 1)] = num8 * 0.125f;
				float num9 = num5 + src[num2 * srclen2 + i] + src[x * srclen2 + num4];
				dst[(2 * x + 1) * dstlen2 + (2 * i + 1)] = num9 * 0.125f;
			}
		});
	}

	// Token: 0x06002D05 RID: 11525 RVA: 0x0010E270 File Offset: 0x0010C470
	public static void Upsample2D(float[] src, int srclen1, int srclen2, int srclen3, float[] dst, int dstlen1, int dstlen2, int dstlen3)
	{
		if (2 * srclen1 != dstlen1 || 2 * srclen2 != dstlen2 || srclen3 != dstlen3)
		{
			return;
		}
		Parallel.For(0, srclen1, delegate(int x)
		{
			int num = Mathf.Max(0, x - 1);
			int num2 = Mathf.Min(srclen1 - 1, x + 1);
			for (int i = 0; i < srclen2; i++)
			{
				int num3 = Mathf.Max(0, i - 1);
				int num4 = Mathf.Min(srclen2 - 1, i + 1);
				for (int j = 0; j < srclen3; j++)
				{
					float num5 = src[(x * srclen2 + i) * srclen3 + j] * 6f;
					float num6 = num5 + src[(num * srclen2 + i) * srclen3 + j] + src[(x * srclen2 + num3) * srclen3 + j];
					dst[(2 * x * dstlen2 + 2 * i) * dstlen3 + j] = num6 * 0.125f;
					float num7 = num5 + src[(num2 * srclen2 + i) * srclen3 + j] + src[(x * srclen2 + num3) * srclen3 + j];
					dst[((2 * x + 1) * dstlen2 + 2 * i) * dstlen3 + j] = num7 * 0.125f;
					float num8 = num5 + src[(num * srclen2 + i) * srclen3 + j] + src[(x * srclen2 + num4) * srclen3 + j];
					dst[(2 * x * dstlen2 + (2 * i + 1)) * dstlen3 + j] = num8 * 0.125f;
					float num9 = num5 + src[(num2 * srclen2 + i) * srclen3 + j] + src[(x * srclen2 + num4) * srclen3 + j];
					dst[((2 * x + 1) * dstlen2 + (2 * i + 1)) * dstlen3 + j] = num9 * 0.125f;
				}
			}
		});
	}

	// Token: 0x06002D06 RID: 11526 RVA: 0x0010E2FC File Offset: 0x0010C4FC
	public static void Dilate2D(int[] src, int len1, int len2, int srcmask, int radius, Action<int, int> action)
	{
		Parallel.For(0, len1, delegate(int x)
		{
			MaxQueue maxQueue = new MaxQueue(radius * 2 + 1);
			for (int i = 0; i < radius; i++)
			{
				maxQueue.Push(src[x * len2 + i] & srcmask);
			}
			for (int j = 0; j < len2; j++)
			{
				if (j > radius)
				{
					maxQueue.Pop();
				}
				if (j < len2 - radius)
				{
					maxQueue.Push(src[x * len2 + j + radius] & srcmask);
				}
				if (maxQueue.Max != 0)
				{
					action(x, j);
				}
			}
		});
		Parallel.For(0, len2, delegate(int y)
		{
			MaxQueue maxQueue = new MaxQueue(radius * 2 + 1);
			for (int i = 0; i < radius; i++)
			{
				maxQueue.Push(src[i * len2 + y] & srcmask);
			}
			for (int j = 0; j < len1; j++)
			{
				if (j > radius)
				{
					maxQueue.Pop();
				}
				if (j < len1 - radius)
				{
					maxQueue.Push(src[(j + radius) * len2 + y] & srcmask);
				}
				if (maxQueue.Max != 0)
				{
					action(j, y);
				}
			}
		});
	}

	// Token: 0x06002D07 RID: 11527 RVA: 0x0010E36C File Offset: 0x0010C56C
	public static void FloodFill2D(int x, int y, int[] data, int len1, int len2, int mask_any, int mask_not, Func<int, int> action)
	{
		Stack<KeyValuePair<int, int>> stack = new Stack<KeyValuePair<int, int>>();
		stack.Push(new KeyValuePair<int, int>(x, y));
		while (stack.Count > 0)
		{
			KeyValuePair<int, int> keyValuePair = stack.Pop();
			x = keyValuePair.Key;
			y = keyValuePair.Value;
			int i;
			for (i = y; i >= 0; i--)
			{
				int num = data[x * len2 + i];
				if ((num & mask_any) == 0 || (num & mask_not) != 0)
				{
					break;
				}
			}
			i++;
			bool flag2;
			bool flag = flag2 = false;
			while (i < len2)
			{
				int num2 = data[x * len2 + i];
				if ((num2 & mask_any) == 0 || (num2 & mask_not) != 0)
				{
					break;
				}
				data[x * len2 + i] = action(num2);
				if (x > 0)
				{
					int num3 = data[(x - 1) * len2 + i];
					bool flag3 = (num3 & mask_any) != 0 && (num3 & mask_not) == 0;
					if (!flag2 && flag3)
					{
						stack.Push(new KeyValuePair<int, int>(x - 1, i));
						flag2 = true;
					}
					else if (flag2 && !flag3)
					{
						flag2 = false;
					}
				}
				if (x < len1 - 1)
				{
					int num4 = data[(x + 1) * len2 + i];
					bool flag4 = (num4 & mask_any) != 0 && (num4 & mask_not) == 0;
					if (!flag && flag4)
					{
						stack.Push(new KeyValuePair<int, int>(x + 1, i));
						flag = true;
					}
					else if (flag && !flag4)
					{
						flag = false;
					}
				}
				i++;
			}
		}
	}

	// Token: 0x06002D08 RID: 11528 RVA: 0x0010E4D0 File Offset: 0x0010C6D0
	public static bool IsValidPNG(byte[] data, int maxWidth, int maxHeight)
	{
		if (data == null || data.Length < 29)
		{
			return false;
		}
		if (data.Length > 29 + maxWidth * maxHeight * 4)
		{
			return false;
		}
		for (int i = 0; i < ImageProcessing.signaturePNG.Length; i++)
		{
			if (data[i] != ImageProcessing.signaturePNG[i])
			{
				return false;
			}
		}
		for (int j = 0; j < ImageProcessing.signatureIHDR.Length; j++)
		{
			if (data[8 + j] != ImageProcessing.signatureIHDR[j])
			{
				return false;
			}
		}
		Union32 union = default(Union32);
		union.b4 = data[16];
		union.b3 = data[17];
		union.b2 = data[18];
		union.b1 = data[19];
		if (union.i < 1 || union.i > maxWidth)
		{
			return false;
		}
		Union32 union2 = default(Union32);
		union2.b4 = data[20];
		union2.b3 = data[21];
		union2.b2 = data[22];
		union2.b1 = data[23];
		if (union2.i < 1 || union2.i > maxHeight)
		{
			return false;
		}
		byte b = data[24];
		if (b != 8 && b != 16)
		{
			return false;
		}
		byte b2 = data[25];
		return (b2 == 2 || b2 == 6) && data[26] == 0 && data[27] == 0 && data[28] == 0;
	}

	// Token: 0x06002D09 RID: 11529 RVA: 0x0010E610 File Offset: 0x0010C810
	public static bool IsValidJPG(byte[] data, int maxWidth, int maxHeight)
	{
		if (data.Length < 30)
		{
			return false;
		}
		if (data.Length > 30 + maxWidth * maxHeight)
		{
			return false;
		}
		bool result;
		try
		{
			if (data[0] != 255 || data[1] != 216)
			{
				result = false;
			}
			else if (data[2] != 255 || data[3] != 224)
			{
				result = false;
			}
			else if (data[6] != 74 || data[7] != 70 || data[8] != 73 || data[9] != 70 || data[10] != 0)
			{
				result = false;
			}
			else if (data[13] != 0)
			{
				result = false;
			}
			else if (data[14] != data[16] || data[15] != data[17])
			{
				result = false;
			}
			else
			{
				int i = 4;
				int num = (int)data[i] << 8 | (int)data[i + 1];
				while (i < data.Length)
				{
					i += num;
					if (i >= data.Length)
					{
						return false;
					}
					if (data[i] != 255)
					{
						return false;
					}
					if (data[i + 1] == 192 || data[i + 1] == 193 || data[i + 1] == 194)
					{
						int num2 = (int)data[i + 5] << 8 | (int)data[i + 6];
						return ((int)data[i + 7] << 8 | (int)data[i + 8]) <= maxWidth && num2 <= maxHeight;
					}
					i += 2;
					num = ((int)data[i] << 8 | (int)data[i + 1]);
				}
				result = false;
			}
		}
		catch
		{
			result = false;
		}
		return result;
	}

	// Token: 0x06002D0A RID: 11530 RVA: 0x0010E778 File Offset: 0x0010C978
	public static bool IsClear(Color32[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			if (data[i].a > 5)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x040024F4 RID: 9460
	private static byte[] signaturePNG = new byte[]
	{
		137,
		80,
		78,
		71,
		13,
		10,
		26,
		10
	};

	// Token: 0x040024F5 RID: 9461
	private static byte[] signatureIHDR = new byte[]
	{
		0,
		0,
		0,
		13,
		73,
		72,
		68,
		82
	};
}
