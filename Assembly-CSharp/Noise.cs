using System;

// Token: 0x02000625 RID: 1573
public static class Noise
{
	// Token: 0x06002D32 RID: 11570 RVA: 0x0010FA12 File Offset: 0x0010DC12
	public static float Simplex1D(float x)
	{
		return NativeNoise.Simplex1D(x);
	}

	// Token: 0x06002D33 RID: 11571 RVA: 0x0010FA1A File Offset: 0x0010DC1A
	public static float Simplex2D(float x, float y)
	{
		return NativeNoise.Simplex2D(x, y);
	}

	// Token: 0x06002D34 RID: 11572 RVA: 0x0010FA23 File Offset: 0x0010DC23
	public static float Turbulence(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.Turbulence(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06002D35 RID: 11573 RVA: 0x0010FA34 File Offset: 0x0010DC34
	public static float Billow(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.Billow(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06002D36 RID: 11574 RVA: 0x0010FA45 File Offset: 0x0010DC45
	public static float Ridge(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.Ridge(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06002D37 RID: 11575 RVA: 0x0010FA56 File Offset: 0x0010DC56
	public static float Sharp(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.Sharp(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06002D38 RID: 11576 RVA: 0x0010FA67 File Offset: 0x0010DC67
	public static float TurbulenceIQ(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.TurbulenceIQ(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06002D39 RID: 11577 RVA: 0x0010FA78 File Offset: 0x0010DC78
	public static float BillowIQ(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.BillowIQ(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06002D3A RID: 11578 RVA: 0x0010FA89 File Offset: 0x0010DC89
	public static float RidgeIQ(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.RidgeIQ(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06002D3B RID: 11579 RVA: 0x0010FA9A File Offset: 0x0010DC9A
	public static float SharpIQ(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.SharpIQ(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06002D3C RID: 11580 RVA: 0x0010FAAB File Offset: 0x0010DCAB
	public static float TurbulenceWarp(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f, float warp = 0.25f)
	{
		return NativeNoise.TurbulenceWarp(x, y, octaves, frequency, amplitude, lacunarity, gain, warp);
	}

	// Token: 0x06002D3D RID: 11581 RVA: 0x0010FABE File Offset: 0x0010DCBE
	public static float BillowWarp(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f, float warp = 0.25f)
	{
		return NativeNoise.BillowWarp(x, y, octaves, frequency, amplitude, lacunarity, gain, warp);
	}

	// Token: 0x06002D3E RID: 11582 RVA: 0x0010FAD1 File Offset: 0x0010DCD1
	public static float RidgeWarp(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f, float warp = 0.25f)
	{
		return NativeNoise.RidgeWarp(x, y, octaves, frequency, amplitude, lacunarity, gain, warp);
	}

	// Token: 0x06002D3F RID: 11583 RVA: 0x0010FAE4 File Offset: 0x0010DCE4
	public static float SharpWarp(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f, float warp = 0.25f)
	{
		return NativeNoise.SharpWarp(x, y, octaves, frequency, amplitude, lacunarity, gain, warp);
	}

	// Token: 0x06002D40 RID: 11584 RVA: 0x0010FAF8 File Offset: 0x0010DCF8
	public static float Jordan(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f, float warp = 1f, float damp = 1f, float damp_scale = 1f)
	{
		return NativeNoise.Jordan(x, y, octaves, frequency, amplitude, lacunarity, gain, warp, damp, damp_scale);
	}

	// Token: 0x04002504 RID: 9476
	public const float MIN = -1000000f;

	// Token: 0x04002505 RID: 9477
	public const float MAX = 1000000f;
}
