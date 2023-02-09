using System;
using UnityEngine;

// Token: 0x020006EB RID: 1771
public class CoreEnvBrdfLut
{
	// Token: 0x0600316D RID: 12653 RVA: 0x0012FC5E File Offset: 0x0012DE5E
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void OnRuntimeLoad()
	{
		CoreEnvBrdfLut.PrepareTextureForRuntime();
		CoreEnvBrdfLut.UpdateReflProbe();
	}

	// Token: 0x0600316E RID: 12654 RVA: 0x0012FC6A File Offset: 0x0012DE6A
	private static void PrepareTextureForRuntime()
	{
		if (CoreEnvBrdfLut.runtimeEnvBrdfLut == null)
		{
			CoreEnvBrdfLut.runtimeEnvBrdfLut = CoreEnvBrdfLut.Generate(false);
		}
		Shader.SetGlobalTexture("_EnvBrdfLut", CoreEnvBrdfLut.runtimeEnvBrdfLut);
	}

	// Token: 0x0600316F RID: 12655 RVA: 0x0012FC94 File Offset: 0x0012DE94
	private static void UpdateReflProbe()
	{
		int num = (int)Mathf.Log((float)RenderSettings.defaultReflectionResolution, 2f) - 1;
		if (Shader.GetGlobalFloat("_ReflProbeMaxMip") != (float)num)
		{
			Shader.SetGlobalFloat("_ReflProbeMaxMip", (float)num);
		}
	}

	// Token: 0x06003170 RID: 12656 RVA: 0x0012FCD0 File Offset: 0x0012DED0
	public static Texture2D Generate(bool asset = false)
	{
		TextureFormat textureFormat = asset ? TextureFormat.RGBAHalf : TextureFormat.RGHalf;
		textureFormat = (SystemInfo.SupportsTextureFormat(textureFormat) ? textureFormat : TextureFormat.ARGB32);
		int num = 128;
		int num2 = 32;
		float num3 = 1f / (float)num;
		float num4 = 1f / (float)num2;
		Texture2D texture2D = new Texture2D(num, num2, textureFormat, false, true);
		texture2D.name = "_EnvBrdfLut";
		texture2D.wrapMode = TextureWrapMode.Clamp;
		texture2D.filterMode = FilterMode.Bilinear;
		Color[] array = new Color[num * num2];
		float num5 = 0.0078125f;
		for (int i = 0; i < num2; i++)
		{
			float num6 = ((float)i + 0.5f) * num4;
			float num7 = num6 * num6;
			float num8 = num7 * num7;
			int j = 0;
			int num9 = i * num;
			while (j < num)
			{
				float num10 = ((float)j + 0.5f) * num3;
				Vector3 vector = new Vector3(Mathf.Sqrt(1f - num10 * num10), 0f, num10);
				float num11 = 0f;
				float num12 = 0f;
				for (uint num13 = 0U; num13 < 128U; num13 += 1U)
				{
					float num14 = num13 * num5;
					float num15 = (float)(CoreEnvBrdfLut.ReverseBits(num13) / 4294967296.0);
					float f = 6.2831855f * num14;
					float num16 = Mathf.Sqrt((1f - num15) / (1f + (num8 - 1f) * num15));
					float num17 = Mathf.Sqrt(1f - num16 * num16);
					Vector3 vector2 = new Vector3(num17 * Mathf.Cos(f), num17 * Mathf.Sin(f), num16);
					float num18 = Mathf.Max((2f * Vector3.Dot(vector, vector2) * vector2 - vector).z, 0f);
					float num19 = Mathf.Max(vector2.z, 0f);
					float num20 = Mathf.Max(Vector3.Dot(vector, vector2), 0f);
					if (num18 > 0f)
					{
						float num21 = num18 * (num10 * (1f - num7) + num7);
						float num22 = num10 * (num18 * (1f - num7) + num7);
						float num23 = 0.5f / (num21 + num22);
						float num24 = num18 * num23 * (4f * num20 / num19);
						float num25 = 1f - num20;
						num25 *= num25 * num25 * (num25 * num25);
						num11 += num24 * (1f - num25);
						num12 += num24 * num25;
					}
				}
				num11 = Mathf.Clamp(num11 * num5, 0f, 1f);
				num12 = Mathf.Clamp(num12 * num5, 0f, 1f);
				array[num9++] = new Color(num11, num12, 0f, 0f);
				j++;
			}
		}
		texture2D.SetPixels(array);
		texture2D.Apply(false, !asset);
		return texture2D;
	}

	// Token: 0x06003171 RID: 12657 RVA: 0x0012FFA0 File Offset: 0x0012E1A0
	private static uint ReverseBits(uint Bits)
	{
		Bits = (Bits << 16 | Bits >> 16);
		Bits = ((Bits & 16711935U) << 8 | (Bits & 4278255360U) >> 8);
		Bits = ((Bits & 252645135U) << 4 | (Bits & 4042322160U) >> 4);
		Bits = ((Bits & 858993459U) << 2 | (Bits & 3435973836U) >> 2);
		Bits = ((Bits & 1431655765U) << 1 | (Bits & 2863311530U) >> 1);
		return Bits;
	}

	// Token: 0x04002818 RID: 10264
	private static Texture2D runtimeEnvBrdfLut;
}
