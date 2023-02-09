using System;
using System.Runtime.InteropServices;
using System.Security;

// Token: 0x02000624 RID: 1572
[SuppressUnmanagedCodeSecurity]
public static class NativeNoise
{
	// Token: 0x06002D21 RID: 11553
	[DllImport("RustNative", EntryPoint = "snoise1_32")]
	public static extern float Simplex1D(float x);

	// Token: 0x06002D22 RID: 11554
	[DllImport("RustNative", EntryPoint = "sdnoise1_32")]
	public static extern float Simplex1D(float x, out float dx);

	// Token: 0x06002D23 RID: 11555
	[DllImport("RustNative", EntryPoint = "snoise2_32")]
	public static extern float Simplex2D(float x, float y);

	// Token: 0x06002D24 RID: 11556
	[DllImport("RustNative", EntryPoint = "sdnoise2_32")]
	public static extern float Simplex2D(float x, float y, out float dx, out float dy);

	// Token: 0x06002D25 RID: 11557
	[DllImport("RustNative", EntryPoint = "turbulence_32")]
	public static extern float Turbulence(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06002D26 RID: 11558
	[DllImport("RustNative", EntryPoint = "billow_32")]
	public static extern float Billow(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06002D27 RID: 11559
	[DllImport("RustNative", EntryPoint = "ridge_32")]
	public static extern float Ridge(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06002D28 RID: 11560
	[DllImport("RustNative", EntryPoint = "sharp_32")]
	public static extern float Sharp(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06002D29 RID: 11561
	[DllImport("RustNative", EntryPoint = "turbulence_iq_32")]
	public static extern float TurbulenceIQ(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06002D2A RID: 11562
	[DllImport("RustNative", EntryPoint = "billow_iq_32")]
	public static extern float BillowIQ(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06002D2B RID: 11563
	[DllImport("RustNative", EntryPoint = "ridge_iq_32")]
	public static extern float RidgeIQ(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06002D2C RID: 11564
	[DllImport("RustNative", EntryPoint = "sharp_iq_32")]
	public static extern float SharpIQ(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06002D2D RID: 11565
	[DllImport("RustNative", EntryPoint = "turbulence_warp_32")]
	public static extern float TurbulenceWarp(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain, float warp);

	// Token: 0x06002D2E RID: 11566
	[DllImport("RustNative", EntryPoint = "billow_warp_32")]
	public static extern float BillowWarp(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain, float warp);

	// Token: 0x06002D2F RID: 11567
	[DllImport("RustNative", EntryPoint = "ridge_warp_32")]
	public static extern float RidgeWarp(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain, float warp);

	// Token: 0x06002D30 RID: 11568
	[DllImport("RustNative", EntryPoint = "sharp_warp_32")]
	public static extern float SharpWarp(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain, float warp);

	// Token: 0x06002D31 RID: 11569
	[DllImport("RustNative", EntryPoint = "jordan_32")]
	public static extern float Jordan(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain, float warp, float damp, float damp_scale);
}
