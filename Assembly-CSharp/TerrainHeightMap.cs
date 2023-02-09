using System;
using UnityEngine;

// Token: 0x02000670 RID: 1648
public class TerrainHeightMap : TerrainMap<short>
{
	// Token: 0x06002E96 RID: 11926 RVA: 0x0011718C File Offset: 0x0011538C
	public override void Setup()
	{
		this.res = this.terrain.terrainData.heightmapResolution;
		this.src = (this.dst = new short[this.res * this.res]);
		this.normY = TerrainMeta.Size.x / TerrainMeta.Size.y / (float)this.res;
		if (this.HeightTexture != null)
		{
			if (this.HeightTexture.width == this.HeightTexture.height && this.HeightTexture.width == this.res)
			{
				Color32[] pixels = this.HeightTexture.GetPixels32();
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
			Debug.LogError("Invalid height texture: " + this.HeightTexture.name);
		}
	}

	// Token: 0x06002E97 RID: 11927 RVA: 0x001172A4 File Offset: 0x001154A4
	public void ApplyToTerrain()
	{
		float[,] heights = this.terrain.terrainData.GetHeights(0, 0, this.res, this.res);
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				heights[z, i] = this.GetHeight01(i, z);
			}
		});
		this.terrain.terrainData.SetHeights(0, 0, heights);
		TerrainCollider component = this.terrain.GetComponent<TerrainCollider>();
		if (component)
		{
			component.enabled = false;
			component.enabled = true;
		}
	}

	// Token: 0x06002E98 RID: 11928 RVA: 0x00117334 File Offset: 0x00115534
	public void GenerateTextures(bool heightTexture = true, bool normalTexture = true)
	{
		if (heightTexture)
		{
			Color32[] heights = new Color32[this.res * this.res];
			Parallel.For(0, this.res, delegate(int z)
			{
				for (int i = 0; i < this.res; i++)
				{
					heights[z * this.res + i] = BitUtility.EncodeShort(this.src[z * this.res + i]);
				}
			});
			this.HeightTexture = new Texture2D(this.res, this.res, TextureFormat.RGBA32, true, true);
			this.HeightTexture.name = "HeightTexture";
			this.HeightTexture.wrapMode = TextureWrapMode.Clamp;
			this.HeightTexture.SetPixels32(heights);
		}
		if (normalTexture)
		{
			int normalres = (this.res - 1) / 2;
			Color32[] normals = new Color32[normalres * normalres];
			Parallel.For(0, normalres, delegate(int z)
			{
				float normZ = ((float)z + 0.5f) / (float)normalres;
				for (int i = 0; i < normalres; i++)
				{
					float normX = ((float)i + 0.5f) / (float)normalres;
					Vector3 vector = this.GetNormal(normX, normZ);
					float value = Vector3.Angle(Vector3.up, vector);
					float t = Mathf.InverseLerp(50f, 70f, value);
					vector = Vector3.Slerp(vector, Vector3.up, t);
					normals[z * normalres + i] = BitUtility.EncodeNormal(vector);
				}
			});
			this.NormalTexture = new Texture2D(normalres, normalres, TextureFormat.RGBA32, false, true);
			this.NormalTexture.name = "NormalTexture";
			this.NormalTexture.wrapMode = TextureWrapMode.Clamp;
			this.NormalTexture.SetPixels32(normals);
		}
	}

	// Token: 0x06002E99 RID: 11929 RVA: 0x00117468 File Offset: 0x00115668
	public void ApplyTextures()
	{
		this.HeightTexture.Apply(true, false);
		this.NormalTexture.Apply(true, false);
		this.NormalTexture.Compress(false);
		this.HeightTexture.Apply(false, true);
		this.NormalTexture.Apply(false, true);
	}

	// Token: 0x06002E9A RID: 11930 RVA: 0x001174B5 File Offset: 0x001156B5
	public float GetHeight(Vector3 worldPos)
	{
		return TerrainMeta.Position.y + this.GetHeight01(worldPos) * TerrainMeta.Size.y;
	}

	// Token: 0x06002E9B RID: 11931 RVA: 0x001174D4 File Offset: 0x001156D4
	public float GetHeight(float normX, float normZ)
	{
		return TerrainMeta.Position.y + this.GetHeight01(normX, normZ) * TerrainMeta.Size.y;
	}

	// Token: 0x06002E9C RID: 11932 RVA: 0x001174F4 File Offset: 0x001156F4
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
		return TerrainMeta.Position.y + num19 * TerrainMeta.Size.y;
	}

	// Token: 0x06002E9D RID: 11933 RVA: 0x0011760D File Offset: 0x0011580D
	public float GetHeight(int x, int z)
	{
		return TerrainMeta.Position.y + this.GetHeight01(x, z) * TerrainMeta.Size.y;
	}

	// Token: 0x06002E9E RID: 11934 RVA: 0x00117630 File Offset: 0x00115830
	public float GetHeight01(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetHeight01(normX, normZ);
	}

	// Token: 0x06002E9F RID: 11935 RVA: 0x00117660 File Offset: 0x00115860
	public float GetHeight01(float normX, float normZ)
	{
		int num = this.res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int x = Mathf.Min(num4 + 1, num);
		int z = Mathf.Min(num5 + 1, num);
		float height = this.GetHeight01(num4, num5);
		float height2 = this.GetHeight01(x, num5);
		float height3 = this.GetHeight01(num4, z);
		float height4 = this.GetHeight01(x, z);
		float t = num2 - (float)num4;
		float t2 = num3 - (float)num5;
		float a = Mathf.Lerp(height, height2, t);
		float b = Mathf.Lerp(height3, height4, t);
		return Mathf.Lerp(a, b, t2);
	}

	// Token: 0x06002EA0 RID: 11936 RVA: 0x00117704 File Offset: 0x00115904
	public float GetTriangulatedHeight01(float normX, float normZ)
	{
		int num = this.res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int x = Mathf.Min(num4 + 1, num);
		int z = Mathf.Min(num5 + 1, num);
		float num6 = num2 - (float)num4;
		float num7 = num3 - (float)num5;
		float height = this.GetHeight01(num4, num5);
		float height2 = this.GetHeight01(x, z);
		if (num6 > num7)
		{
			float height3 = this.GetHeight01(x, num5);
			return height + (height3 - height) * num6 + (height2 - height3) * num7;
		}
		float height4 = this.GetHeight01(num4, z);
		return height + (height2 - height4) * num6 + (height4 - height) * num7;
	}

	// Token: 0x06002EA1 RID: 11937 RVA: 0x001177B3 File Offset: 0x001159B3
	public float GetHeight01(int x, int z)
	{
		return BitUtility.Short2Float((int)this.src[z * this.res + x]);
	}

	// Token: 0x06002EA2 RID: 11938 RVA: 0x001177B3 File Offset: 0x001159B3
	private float GetSrcHeight01(int x, int z)
	{
		return BitUtility.Short2Float((int)this.src[z * this.res + x]);
	}

	// Token: 0x06002EA3 RID: 11939 RVA: 0x001177CB File Offset: 0x001159CB
	private float GetDstHeight01(int x, int z)
	{
		return BitUtility.Short2Float((int)this.dst[z * this.res + x]);
	}

	// Token: 0x06002EA4 RID: 11940 RVA: 0x001177E4 File Offset: 0x001159E4
	public Vector3 GetNormal(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetNormal(normX, normZ);
	}

	// Token: 0x06002EA5 RID: 11941 RVA: 0x00117814 File Offset: 0x00115A14
	public Vector3 GetNormal(float normX, float normZ)
	{
		int num = this.res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int x = Mathf.Min(num4 + 1, num);
		int z = Mathf.Min(num5 + 1, num);
		Vector3 normal = this.GetNormal(num4, num5);
		Vector3 normal2 = this.GetNormal(x, num5);
		Vector3 normal3 = this.GetNormal(num4, z);
		Vector3 normal4 = this.GetNormal(x, z);
		float t = num2 - (float)num4;
		float t2 = num3 - (float)num5;
		Vector3 a = Vector3.Slerp(normal, normal2, t);
		Vector3 b = Vector3.Slerp(normal3, normal4, t);
		return Vector3.Slerp(a, b, t2).normalized;
	}

	// Token: 0x06002EA6 RID: 11942 RVA: 0x001178C4 File Offset: 0x00115AC4
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

	// Token: 0x06002EA7 RID: 11943 RVA: 0x00117950 File Offset: 0x00115B50
	private Vector3 GetNormalSobel(int x, int z)
	{
		int num = this.res - 1;
		Vector3 vector = new Vector3(TerrainMeta.Size.x / (float)num, TerrainMeta.Size.y, TerrainMeta.Size.z / (float)num);
		int x2 = Mathf.Clamp(x - 1, 0, num);
		int z2 = Mathf.Clamp(z - 1, 0, num);
		int x3 = Mathf.Clamp(x + 1, 0, num);
		int z3 = Mathf.Clamp(z + 1, 0, num);
		float num2 = this.GetHeight01(x2, z2) * -1f;
		num2 += this.GetHeight01(x2, z) * -2f;
		num2 += this.GetHeight01(x2, z3) * -1f;
		num2 += this.GetHeight01(x3, z2) * 1f;
		num2 += this.GetHeight01(x3, z) * 2f;
		num2 += this.GetHeight01(x3, z3) * 1f;
		num2 *= vector.y;
		num2 /= vector.x;
		float num3 = this.GetHeight01(x2, z2) * -1f;
		num3 += this.GetHeight01(x, z2) * -2f;
		num3 += this.GetHeight01(x3, z2) * -1f;
		num3 += this.GetHeight01(x2, z3) * 1f;
		num3 += this.GetHeight01(x, z3) * 2f;
		num3 += this.GetHeight01(x3, z3) * 1f;
		num3 *= vector.y;
		num3 /= vector.z;
		Vector3 vector2 = new Vector3(-num2, 8f, -num3);
		return vector2.normalized;
	}

	// Token: 0x06002EA8 RID: 11944 RVA: 0x00117AFC File Offset: 0x00115CFC
	public float GetSlope(Vector3 worldPos)
	{
		return Vector3.Angle(Vector3.up, this.GetNormal(worldPos));
	}

	// Token: 0x06002EA9 RID: 11945 RVA: 0x00117B0F File Offset: 0x00115D0F
	public float GetSlope(float normX, float normZ)
	{
		return Vector3.Angle(Vector3.up, this.GetNormal(normX, normZ));
	}

	// Token: 0x06002EAA RID: 11946 RVA: 0x00117B23 File Offset: 0x00115D23
	public float GetSlope(int x, int z)
	{
		return Vector3.Angle(Vector3.up, this.GetNormal(x, z));
	}

	// Token: 0x06002EAB RID: 11947 RVA: 0x00117B37 File Offset: 0x00115D37
	public float GetSlope01(Vector3 worldPos)
	{
		return this.GetSlope(worldPos) * 0.011111111f;
	}

	// Token: 0x06002EAC RID: 11948 RVA: 0x00117B46 File Offset: 0x00115D46
	public float GetSlope01(float normX, float normZ)
	{
		return this.GetSlope(normX, normZ) * 0.011111111f;
	}

	// Token: 0x06002EAD RID: 11949 RVA: 0x00117B56 File Offset: 0x00115D56
	public float GetSlope01(int x, int z)
	{
		return this.GetSlope(x, z) * 0.011111111f;
	}

	// Token: 0x06002EAE RID: 11950 RVA: 0x00117B68 File Offset: 0x00115D68
	public void SetHeight(Vector3 worldPos, float height)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetHeight(normX, normZ, height);
	}

	// Token: 0x06002EAF RID: 11951 RVA: 0x00117B98 File Offset: 0x00115D98
	public void SetHeight(float normX, float normZ, float height)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetHeight(x, z, height);
	}

	// Token: 0x06002EB0 RID: 11952 RVA: 0x00117BBE File Offset: 0x00115DBE
	public void SetHeight(int x, int z, float height)
	{
		this.dst[z * this.res + x] = BitUtility.Float2Short(height);
	}

	// Token: 0x06002EB1 RID: 11953 RVA: 0x00117BD8 File Offset: 0x00115DD8
	public void SetHeight(Vector3 worldPos, float height, float opacity)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetHeight(normX, normZ, height, opacity);
	}

	// Token: 0x06002EB2 RID: 11954 RVA: 0x00117C08 File Offset: 0x00115E08
	public void SetHeight(float normX, float normZ, float height, float opacity)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetHeight(x, z, height, opacity);
	}

	// Token: 0x06002EB3 RID: 11955 RVA: 0x00117C30 File Offset: 0x00115E30
	public void SetHeight(int x, int z, float height, float opacity)
	{
		float height2 = Mathf.SmoothStep(this.GetSrcHeight01(x, z), height, opacity);
		this.SetHeight(x, z, height2);
	}

	// Token: 0x06002EB4 RID: 11956 RVA: 0x00117C58 File Offset: 0x00115E58
	public void AddHeight(Vector3 worldPos, float delta)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.AddHeight(normX, normZ, delta);
	}

	// Token: 0x06002EB5 RID: 11957 RVA: 0x00117C88 File Offset: 0x00115E88
	public void AddHeight(float normX, float normZ, float delta)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.AddHeight(x, z, delta);
	}

	// Token: 0x06002EB6 RID: 11958 RVA: 0x00117CB0 File Offset: 0x00115EB0
	public void AddHeight(int x, int z, float delta)
	{
		float height = Mathf.Clamp01(this.GetDstHeight01(x, z) + delta);
		this.SetHeight(x, z, height);
	}

	// Token: 0x06002EB7 RID: 11959 RVA: 0x00117CD8 File Offset: 0x00115ED8
	public void LowerHeight(Vector3 worldPos, float height, float opacity)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.LowerHeight(normX, normZ, height, opacity);
	}

	// Token: 0x06002EB8 RID: 11960 RVA: 0x00117D08 File Offset: 0x00115F08
	public void LowerHeight(float normX, float normZ, float height, float opacity)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.LowerHeight(x, z, height, opacity);
	}

	// Token: 0x06002EB9 RID: 11961 RVA: 0x00117D30 File Offset: 0x00115F30
	public void LowerHeight(int x, int z, float height, float opacity)
	{
		float height2 = Mathf.Min(this.GetDstHeight01(x, z), Mathf.SmoothStep(this.GetSrcHeight01(x, z), height, opacity));
		this.SetHeight(x, z, height2);
	}

	// Token: 0x06002EBA RID: 11962 RVA: 0x00117D64 File Offset: 0x00115F64
	public void RaiseHeight(Vector3 worldPos, float height, float opacity)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.RaiseHeight(normX, normZ, height, opacity);
	}

	// Token: 0x06002EBB RID: 11963 RVA: 0x00117D94 File Offset: 0x00115F94
	public void RaiseHeight(float normX, float normZ, float height, float opacity)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.RaiseHeight(x, z, height, opacity);
	}

	// Token: 0x06002EBC RID: 11964 RVA: 0x00117DBC File Offset: 0x00115FBC
	public void RaiseHeight(int x, int z, float height, float opacity)
	{
		float height2 = Mathf.Max(this.GetDstHeight01(x, z), Mathf.SmoothStep(this.GetSrcHeight01(x, z), height, opacity));
		this.SetHeight(x, z, height2);
	}

	// Token: 0x06002EBD RID: 11965 RVA: 0x00117DF0 File Offset: 0x00115FF0
	public void SetHeight(Vector3 worldPos, float opacity, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		float height = TerrainMeta.NormalizeY(worldPos.y);
		this.SetHeight(normX, normZ, height, opacity, radius, fade);
	}

	// Token: 0x06002EBE RID: 11966 RVA: 0x00117E30 File Offset: 0x00116030
	public void SetHeight(float normX, float normZ, float height, float opacity, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				this.SetHeight(x, z, height, lerp * opacity);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x06002EBF RID: 11967 RVA: 0x00117E74 File Offset: 0x00116074
	public void LowerHeight(Vector3 worldPos, float opacity, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		float height = TerrainMeta.NormalizeY(worldPos.y);
		this.LowerHeight(normX, normZ, height, opacity, radius, fade);
	}

	// Token: 0x06002EC0 RID: 11968 RVA: 0x00117EB4 File Offset: 0x001160B4
	public void LowerHeight(float normX, float normZ, float height, float opacity, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				this.LowerHeight(x, z, height, lerp * opacity);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x06002EC1 RID: 11969 RVA: 0x00117EF8 File Offset: 0x001160F8
	public void RaiseHeight(Vector3 worldPos, float opacity, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		float height = TerrainMeta.NormalizeY(worldPos.y);
		this.RaiseHeight(normX, normZ, height, opacity, radius, fade);
	}

	// Token: 0x06002EC2 RID: 11970 RVA: 0x00117F38 File Offset: 0x00116138
	public void RaiseHeight(float normX, float normZ, float height, float opacity, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				this.RaiseHeight(x, z, height, lerp * opacity);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x06002EC3 RID: 11971 RVA: 0x00117F7C File Offset: 0x0011617C
	public void AddHeight(Vector3 worldPos, float delta, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.AddHeight(normX, normZ, delta, radius, fade);
	}

	// Token: 0x06002EC4 RID: 11972 RVA: 0x00117FB0 File Offset: 0x001161B0
	public void AddHeight(float normX, float normZ, float delta, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				this.AddHeight(x, z, lerp * delta);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x04002628 RID: 9768
	public Texture2D HeightTexture;

	// Token: 0x04002629 RID: 9769
	public Texture2D NormalTexture;

	// Token: 0x0400262A RID: 9770
	private float normY;
}
