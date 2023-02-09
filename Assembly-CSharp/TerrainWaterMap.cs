using System;
using UnityEngine;

// Token: 0x02000676 RID: 1654
public class TerrainWaterMap : TerrainMap<short>
{
	// Token: 0x06002F27 RID: 12071 RVA: 0x0011A600 File Offset: 0x00118800
	public override void Setup()
	{
		this.res = this.terrain.terrainData.heightmapResolution;
		this.src = (this.dst = new short[this.res * this.res]);
		this.normY = TerrainMeta.Size.x / TerrainMeta.Size.y / (float)this.res;
		if (this.WaterTexture != null)
		{
			if (this.WaterTexture.width == this.WaterTexture.height && this.WaterTexture.width == this.res)
			{
				Color32[] pixels = this.WaterTexture.GetPixels32();
				int i = 0;
				int num = 0;
				while (i < this.res)
				{
					int j = 0;
					while (j < this.res)
					{
						Color32 c = pixels[num];
						this.dst[i * this.res + j] = BitUtility.DecodeShort(c);
						j++;
						num++;
					}
					i++;
				}
				return;
			}
			Debug.LogError("Invalid water texture: " + this.WaterTexture.name);
		}
	}

	// Token: 0x06002F28 RID: 12072 RVA: 0x0011A718 File Offset: 0x00118918
	public void GenerateTextures()
	{
		Color32[] heights = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				heights[z * this.res + i] = BitUtility.EncodeShort(this.src[z * this.res + i]);
			}
		});
		this.WaterTexture = new Texture2D(this.res, this.res, TextureFormat.RGBA32, true, true);
		this.WaterTexture.name = "WaterTexture";
		this.WaterTexture.wrapMode = TextureWrapMode.Clamp;
		this.WaterTexture.SetPixels32(heights);
	}

	// Token: 0x06002F29 RID: 12073 RVA: 0x0011A7A9 File Offset: 0x001189A9
	public void ApplyTextures()
	{
		this.WaterTexture.Apply(true, true);
	}

	// Token: 0x06002F2A RID: 12074 RVA: 0x0011A7B8 File Offset: 0x001189B8
	public float GetHeight(Vector3 worldPos)
	{
		return Math.Max(TerrainMeta.Position.y + this.GetHeight01(worldPos) * TerrainMeta.Size.y, WaterSystem.OceanLevel);
	}

	// Token: 0x06002F2B RID: 12075 RVA: 0x0011A7E1 File Offset: 0x001189E1
	public float GetHeight(float normX, float normZ)
	{
		return Math.Max(TerrainMeta.Position.y + this.GetHeight01(normX, normZ) * TerrainMeta.Size.y, WaterSystem.OceanLevel);
	}

	// Token: 0x06002F2C RID: 12076 RVA: 0x0011A80C File Offset: 0x00118A0C
	public float GetHeightFast(Vector2 uv)
	{
		int num = this.res - 1;
		float num2 = uv.x * (float)num;
		float num3 = uv.y * (float)num;
		int num4 = (int)num2;
		int num5 = (int)num3;
		float num6 = num2 - (float)num4;
		float num7 = num3 - (float)num5;
		num4 = ((num4 >= 0) ? num4 : 0);
		num5 = ((num5 >= 0) ? num5 : 0);
		num4 = ((num4 <= num) ? num4 : num);
		num5 = ((num5 <= num) ? num5 : num);
		int num8 = (num2 < (float)num) ? 1 : 0;
		int num9 = (num3 < (float)num) ? this.res : 0;
		int num10 = num5 * this.res + num4;
		int num11 = num10 + num8;
		int num12 = num10 + num9;
		int num13 = num12 + num8;
		float num14 = (float)this.src[num10] * 3.051944E-05f;
		float num15 = (float)this.src[num11] * 3.051944E-05f;
		float num16 = (float)this.src[num12] * 3.051944E-05f;
		float num17 = (float)this.src[num13] * 3.051944E-05f;
		float num18 = (num15 - num14) * num6 + num14;
		float num19 = ((num17 - num16) * num6 + num16 - num18) * num7 + num18;
		return Math.Max(TerrainMeta.Position.y + num19 * TerrainMeta.Size.y, WaterSystem.OceanLevel);
	}

	// Token: 0x06002F2D RID: 12077 RVA: 0x0011A92F File Offset: 0x00118B2F
	public float GetHeight(int x, int z)
	{
		return Math.Max(TerrainMeta.Position.y + this.GetHeight01(x, z) * TerrainMeta.Size.y, WaterSystem.OceanLevel);
	}

	// Token: 0x06002F2E RID: 12078 RVA: 0x0011A95C File Offset: 0x00118B5C
	public float GetHeight01(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetHeight01(normX, normZ);
	}

	// Token: 0x06002F2F RID: 12079 RVA: 0x0011A98C File Offset: 0x00118B8C
	public float GetHeight01(float normX, float normZ)
	{
		int num = this.res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int x = Mathf.Min(num4 + 1, num);
		int z = Mathf.Min(num5 + 1, num);
		float a = Mathf.Lerp(this.GetHeight01(num4, num5), this.GetHeight01(x, num5), num2 - (float)num4);
		float b = Mathf.Lerp(this.GetHeight01(num4, z), this.GetHeight01(x, z), num2 - (float)num4);
		return Mathf.Lerp(a, b, num3 - (float)num5);
	}

	// Token: 0x06002F30 RID: 12080 RVA: 0x001177B3 File Offset: 0x001159B3
	public float GetHeight01(int x, int z)
	{
		return BitUtility.Short2Float((int)this.src[z * this.res + x]);
	}

	// Token: 0x06002F31 RID: 12081 RVA: 0x0011AA20 File Offset: 0x00118C20
	public Vector3 GetNormal(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetNormal(normX, normZ);
	}

	// Token: 0x06002F32 RID: 12082 RVA: 0x0011AA50 File Offset: 0x00118C50
	public Vector3 GetNormal(float normX, float normZ)
	{
		int num = this.res - 1;
		int num2 = (int)(normX * (float)num);
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp(num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int x = Mathf.Min(num4 + 1, num);
		int z = Mathf.Min(num5 + 1, num);
		float num6 = this.GetHeight01(x, num5) - this.GetHeight01(num4, num5);
		float num7 = this.GetHeight01(num4, z) - this.GetHeight01(num4, num5);
		return new Vector3(-num6, this.normY, -num7).normalized;
	}

	// Token: 0x06002F33 RID: 12083 RVA: 0x0011AAD8 File Offset: 0x00118CD8
	public Vector3 GetNormalFast(Vector2 uv)
	{
		int num = this.res - 1;
		int num2 = (int)(uv.x * (float)num);
		int num3 = (int)(uv.y * (float)num);
		num2 = ((num2 >= 0) ? num2 : 0);
		num3 = ((num3 >= 0) ? num3 : 0);
		num2 = ((num2 <= num) ? num2 : num);
		num3 = ((num3 <= num) ? num3 : num);
		int num4 = (num2 < num) ? 1 : 0;
		int num5 = (num3 < num) ? this.res : 0;
		int num6 = num3 * this.res + num2;
		int num7 = num6 + num4;
		int num8 = num6 + num5;
		short num9 = this.src[num6];
		float num10 = (float)this.src[num7];
		short num11 = this.src[num8];
		float num12 = (num10 - (float)num9) * 3.051944E-05f;
		float num13 = (float)(num11 - num9) * 3.051944E-05f;
		return new Vector3(-num12, this.normY, -num13);
	}

	// Token: 0x06002F34 RID: 12084 RVA: 0x0011ABA0 File Offset: 0x00118DA0
	public Vector3 GetNormal(int x, int z)
	{
		int max = this.res - 1;
		int x2 = Mathf.Clamp(x - 1, 0, max);
		int z2 = Mathf.Clamp(z - 1, 0, max);
		int x3 = Mathf.Clamp(x + 1, 0, max);
		int z3 = Mathf.Clamp(z + 1, 0, max);
		float num = (this.GetHeight01(x3, z2) - this.GetHeight01(x2, z2)) * 0.5f;
		float num2 = (this.GetHeight01(x2, z3) - this.GetHeight01(x2, z2)) * 0.5f;
		return new Vector3(-num, this.normY, -num2).normalized;
	}

	// Token: 0x06002F35 RID: 12085 RVA: 0x0011AC2C File Offset: 0x00118E2C
	public float GetSlope(Vector3 worldPos)
	{
		return Vector3.Angle(Vector3.up, this.GetNormal(worldPos));
	}

	// Token: 0x06002F36 RID: 12086 RVA: 0x0011AC3F File Offset: 0x00118E3F
	public float GetSlope(float normX, float normZ)
	{
		return Vector3.Angle(Vector3.up, this.GetNormal(normX, normZ));
	}

	// Token: 0x06002F37 RID: 12087 RVA: 0x0011AC53 File Offset: 0x00118E53
	public float GetSlope(int x, int z)
	{
		return Vector3.Angle(Vector3.up, this.GetNormal(x, z));
	}

	// Token: 0x06002F38 RID: 12088 RVA: 0x0011AC67 File Offset: 0x00118E67
	public float GetSlope01(Vector3 worldPos)
	{
		return this.GetSlope(worldPos) * 0.011111111f;
	}

	// Token: 0x06002F39 RID: 12089 RVA: 0x0011AC76 File Offset: 0x00118E76
	public float GetSlope01(float normX, float normZ)
	{
		return this.GetSlope(normX, normZ) * 0.011111111f;
	}

	// Token: 0x06002F3A RID: 12090 RVA: 0x0011AC86 File Offset: 0x00118E86
	public float GetSlope01(int x, int z)
	{
		return this.GetSlope(x, z) * 0.011111111f;
	}

	// Token: 0x06002F3B RID: 12091 RVA: 0x0011AC96 File Offset: 0x00118E96
	public float GetDepth(Vector3 worldPos)
	{
		return this.GetHeight(worldPos) - TerrainMeta.HeightMap.GetHeight(worldPos);
	}

	// Token: 0x06002F3C RID: 12092 RVA: 0x0011ACAB File Offset: 0x00118EAB
	public float GetDepth(float normX, float normZ)
	{
		return this.GetHeight(normX, normZ) - TerrainMeta.HeightMap.GetHeight(normX, normZ);
	}

	// Token: 0x06002F3D RID: 12093 RVA: 0x0011ACC4 File Offset: 0x00118EC4
	public void SetHeight(Vector3 worldPos, float height)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetHeight(normX, normZ, height);
	}

	// Token: 0x06002F3E RID: 12094 RVA: 0x0011ACF4 File Offset: 0x00118EF4
	public void SetHeight(float normX, float normZ, float height)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetHeight(x, z, height);
	}

	// Token: 0x06002F3F RID: 12095 RVA: 0x00117BBE File Offset: 0x00115DBE
	public void SetHeight(int x, int z, float height)
	{
		this.dst[z * this.res + x] = BitUtility.Float2Short(height);
	}

	// Token: 0x04002633 RID: 9779
	public Texture2D WaterTexture;

	// Token: 0x04002634 RID: 9780
	private float normY;
}
