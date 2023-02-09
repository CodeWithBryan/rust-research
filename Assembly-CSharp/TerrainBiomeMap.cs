using System;
using UnityEngine;

// Token: 0x0200066D RID: 1645
public class TerrainBiomeMap : TerrainMap<byte>
{
	// Token: 0x06002E69 RID: 11881 RVA: 0x001161D4 File Offset: 0x001143D4
	public override void Setup()
	{
		this.res = this.terrain.terrainData.alphamapResolution;
		this.num = 4;
		this.src = (this.dst = new byte[this.num * this.res * this.res]);
		if (this.BiomeTexture != null)
		{
			if (this.BiomeTexture.width == this.BiomeTexture.height && this.BiomeTexture.width == this.res)
			{
				Color32[] pixels = this.BiomeTexture.GetPixels32();
				int i = 0;
				int num = 0;
				while (i < this.res)
				{
					int j = 0;
					while (j < this.res)
					{
						Color32 color = pixels[num];
						byte[] dst = this.dst;
						int res = this.res;
						dst[(0 + i) * this.res + j] = color.r;
						this.dst[(this.res + i) * this.res + j] = color.g;
						this.dst[(2 * this.res + i) * this.res + j] = color.b;
						this.dst[(3 * this.res + i) * this.res + j] = color.a;
						j++;
						num++;
					}
					i++;
				}
				return;
			}
			Debug.LogError("Invalid biome texture: " + this.BiomeTexture.name);
		}
	}

	// Token: 0x06002E6A RID: 11882 RVA: 0x00116358 File Offset: 0x00114558
	public void GenerateTextures()
	{
		this.BiomeTexture = new Texture2D(this.res, this.res, TextureFormat.RGBA32, true, true);
		this.BiomeTexture.name = "BiomeTexture";
		this.BiomeTexture.wrapMode = TextureWrapMode.Clamp;
		Color32[] col = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				byte[] src = this.src;
				int res = this.res;
				byte r = src[(0 + z) * this.res + i];
				byte g = this.src[(this.res + z) * this.res + i];
				byte b = this.src[(2 * this.res + z) * this.res + i];
				byte a = this.src[(3 * this.res + z) * this.res + i];
				col[z * this.res + i] = new Color32(r, g, b, a);
			}
		});
		this.BiomeTexture.SetPixels32(col);
	}

	// Token: 0x06002E6B RID: 11883 RVA: 0x001163E9 File Offset: 0x001145E9
	public void ApplyTextures()
	{
		this.BiomeTexture.Apply(true, false);
		this.BiomeTexture.Compress(false);
		this.BiomeTexture.Apply(false, true);
	}

	// Token: 0x06002E6C RID: 11884 RVA: 0x00116414 File Offset: 0x00114614
	public float GetBiomeMax(Vector3 worldPos, int mask = -1)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetBiomeMax(normX, normZ, mask);
	}

	// Token: 0x06002E6D RID: 11885 RVA: 0x00116444 File Offset: 0x00114644
	public float GetBiomeMax(float normX, float normZ, int mask = -1)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		return this.GetBiomeMax(x, z, mask);
	}

	// Token: 0x06002E6E RID: 11886 RVA: 0x0011646C File Offset: 0x0011466C
	public float GetBiomeMax(int x, int z, int mask = -1)
	{
		byte b = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainBiome.IndexToType(i) & mask) != 0)
			{
				byte b2 = this.src[(i * this.res + z) * this.res + x];
				if (b2 >= b)
				{
					b = b2;
				}
			}
		}
		return (float)b;
	}

	// Token: 0x06002E6F RID: 11887 RVA: 0x001164BC File Offset: 0x001146BC
	public int GetBiomeMaxIndex(Vector3 worldPos, int mask = -1)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetBiomeMaxIndex(normX, normZ, mask);
	}

	// Token: 0x06002E70 RID: 11888 RVA: 0x001164EC File Offset: 0x001146EC
	public int GetBiomeMaxIndex(float normX, float normZ, int mask = -1)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		return this.GetBiomeMaxIndex(x, z, mask);
	}

	// Token: 0x06002E71 RID: 11889 RVA: 0x00116514 File Offset: 0x00114714
	public int GetBiomeMaxIndex(int x, int z, int mask = -1)
	{
		byte b = 0;
		int result = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainBiome.IndexToType(i) & mask) != 0)
			{
				byte b2 = this.src[(i * this.res + z) * this.res + x];
				if (b2 >= b)
				{
					b = b2;
					result = i;
				}
			}
		}
		return result;
	}

	// Token: 0x06002E72 RID: 11890 RVA: 0x00116564 File Offset: 0x00114764
	public int GetBiomeMaxType(Vector3 worldPos, int mask = -1)
	{
		return TerrainBiome.IndexToType(this.GetBiomeMaxIndex(worldPos, mask));
	}

	// Token: 0x06002E73 RID: 11891 RVA: 0x00116573 File Offset: 0x00114773
	public int GetBiomeMaxType(float normX, float normZ, int mask = -1)
	{
		return TerrainBiome.IndexToType(this.GetBiomeMaxIndex(normX, normZ, mask));
	}

	// Token: 0x06002E74 RID: 11892 RVA: 0x00116583 File Offset: 0x00114783
	public int GetBiomeMaxType(int x, int z, int mask = -1)
	{
		return TerrainBiome.IndexToType(this.GetBiomeMaxIndex(x, z, mask));
	}

	// Token: 0x06002E75 RID: 11893 RVA: 0x00116594 File Offset: 0x00114794
	public float GetBiome(Vector3 worldPos, int mask)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetBiome(normX, normZ, mask);
	}

	// Token: 0x06002E76 RID: 11894 RVA: 0x001165C4 File Offset: 0x001147C4
	public float GetBiome(float normX, float normZ, int mask)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		return this.GetBiome(x, z, mask);
	}

	// Token: 0x06002E77 RID: 11895 RVA: 0x001165EC File Offset: 0x001147EC
	public float GetBiome(int x, int z, int mask)
	{
		if (Mathf.IsPowerOfTwo(mask))
		{
			return BitUtility.Byte2Float((int)this.src[(TerrainBiome.TypeToIndex(mask) * this.res + z) * this.res + x]);
		}
		int num = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainBiome.IndexToType(i) & mask) != 0)
			{
				num += (int)this.src[(i * this.res + z) * this.res + x];
			}
		}
		return Mathf.Clamp01(BitUtility.Byte2Float(num));
	}

	// Token: 0x06002E78 RID: 11896 RVA: 0x0011666C File Offset: 0x0011486C
	public void SetBiome(Vector3 worldPos, int id)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetBiome(normX, normZ, id);
	}

	// Token: 0x06002E79 RID: 11897 RVA: 0x0011669C File Offset: 0x0011489C
	public void SetBiome(float normX, float normZ, int id)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetBiome(x, z, id);
	}

	// Token: 0x06002E7A RID: 11898 RVA: 0x001166C4 File Offset: 0x001148C4
	public void SetBiome(int x, int z, int id)
	{
		int num = TerrainBiome.TypeToIndex(id);
		for (int i = 0; i < this.num; i++)
		{
			if (i == num)
			{
				this.dst[(i * this.res + z) * this.res + x] = byte.MaxValue;
			}
			else
			{
				this.dst[(i * this.res + z) * this.res + x] = 0;
			}
		}
	}

	// Token: 0x06002E7B RID: 11899 RVA: 0x0011672C File Offset: 0x0011492C
	public void SetBiome(Vector3 worldPos, int id, float v)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetBiome(normX, normZ, id, v);
	}

	// Token: 0x06002E7C RID: 11900 RVA: 0x0011675C File Offset: 0x0011495C
	public void SetBiome(float normX, float normZ, int id, float v)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetBiome(x, z, id, v);
	}

	// Token: 0x06002E7D RID: 11901 RVA: 0x00116784 File Offset: 0x00114984
	public void SetBiome(int x, int z, int id, float v)
	{
		this.SetBiome(x, z, id, this.GetBiome(x, z, id), v);
	}

	// Token: 0x06002E7E RID: 11902 RVA: 0x0011679C File Offset: 0x0011499C
	public void SetBiomeRaw(int x, int z, Vector4 v, float opacity)
	{
		if (opacity == 0f)
		{
			return;
		}
		float num = Mathf.Clamp01(v.x + v.y + v.z + v.w);
		if (num == 0f)
		{
			return;
		}
		float num2 = 1f - opacity * num;
		if (num2 == 0f && opacity == 1f)
		{
			byte[] dst = this.dst;
			int res = this.res;
			dst[(0 + z) * this.res + x] = BitUtility.Float2Byte(v.x);
			this.dst[(this.res + z) * this.res + x] = BitUtility.Float2Byte(v.y);
			this.dst[(2 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v.z);
			this.dst[(3 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v.w);
			return;
		}
		byte[] dst2 = this.dst;
		int res2 = this.res;
		int num3 = (0 + z) * this.res + x;
		byte[] src = this.src;
		int res3 = this.res;
		dst2[num3] = BitUtility.Float2Byte(BitUtility.Byte2Float(src[(0 + z) * this.res + x]) * num2 + v.x * opacity);
		this.dst[(this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(this.res + z) * this.res + x]) * num2 + v.y * opacity);
		this.dst[(2 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(2 * this.res + z) * this.res + x]) * num2 + v.z * opacity);
		this.dst[(3 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(3 * this.res + z) * this.res + x]) * num2 + v.w * opacity);
	}

	// Token: 0x06002E7F RID: 11903 RVA: 0x001169B4 File Offset: 0x00114BB4
	private void SetBiome(int x, int z, int id, float old_val, float new_val)
	{
		int num = TerrainBiome.TypeToIndex(id);
		if (old_val >= 1f)
		{
			return;
		}
		float num2 = (1f - new_val) / (1f - old_val);
		for (int i = 0; i < this.num; i++)
		{
			if (i == num)
			{
				this.dst[(i * this.res + z) * this.res + x] = BitUtility.Float2Byte(new_val);
			}
			else
			{
				this.dst[(i * this.res + z) * this.res + x] = BitUtility.Float2Byte(num2 * BitUtility.Byte2Float((int)this.dst[(i * this.res + z) * this.res + x]));
			}
		}
	}

	// Token: 0x04002624 RID: 9764
	public Texture2D BiomeTexture;

	// Token: 0x04002625 RID: 9765
	internal int num;
}
