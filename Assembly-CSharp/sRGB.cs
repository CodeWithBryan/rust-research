using System;
using UnityEngine;

// Token: 0x020008FD RID: 2301
public class sRGB
{
	// Token: 0x060036CF RID: 14031 RVA: 0x0014630C File Offset: 0x0014450C
	static sRGB()
	{
		sRGB.to_linear = new byte[256];
		sRGB.to_srgb = new byte[256];
		for (int i = 0; i < 256; i++)
		{
			sRGB.to_linear[i] = (byte)(sRGB.srgb_to_linear((float)i * 0.003921569f) * 255f + 0.5f);
		}
		for (int j = 0; j < 256; j++)
		{
			sRGB.to_srgb[j] = (byte)(sRGB.linear_to_srgb((float)j * 0.003921569f) * 255f + 0.5f);
		}
	}

	// Token: 0x060036D0 RID: 14032 RVA: 0x001463B8 File Offset: 0x001445B8
	public static float linear_to_srgb(float linear)
	{
		if (float.IsNaN(linear))
		{
			return 0f;
		}
		if (linear > 1f)
		{
			return 1f;
		}
		if (linear < 0f)
		{
			return 0f;
		}
		if (linear < 0.0031308f)
		{
			return 12.92f * linear;
		}
		return 1.055f * Mathf.Pow(linear, 0.41666f) - 0.055f;
	}

	// Token: 0x060036D1 RID: 14033 RVA: 0x00146416 File Offset: 0x00144616
	public static float srgb_to_linear(float srgb)
	{
		if (srgb <= 0.04045f)
		{
			return srgb / 12.92f;
		}
		return Mathf.Pow((srgb + 0.055f) / 1.055f, 2.4f);
	}

	// Token: 0x04003195 RID: 12693
	public static byte[] to_linear = new byte[256];

	// Token: 0x04003196 RID: 12694
	public static byte[] to_srgb = new byte[256];
}
