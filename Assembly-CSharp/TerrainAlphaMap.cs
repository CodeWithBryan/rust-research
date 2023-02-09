using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200066C RID: 1644
public class TerrainAlphaMap : TerrainMap<byte>
{
	// Token: 0x06002E5C RID: 11868 RVA: 0x00115DFC File Offset: 0x00113FFC
	public override void Setup()
	{
		this.res = this.terrain.terrainData.alphamapResolution;
		this.src = (this.dst = new byte[this.res * this.res]);
		for (int i = 0; i < this.res; i++)
		{
			for (int j = 0; j < this.res; j++)
			{
				this.dst[i * this.res + j] = byte.MaxValue;
			}
		}
		if (this.AlphaTexture != null)
		{
			if (this.AlphaTexture.width == this.AlphaTexture.height && this.AlphaTexture.width == this.res)
			{
				Color32[] pixels = this.AlphaTexture.GetPixels32();
				int k = 0;
				int num = 0;
				while (k < this.res)
				{
					int l = 0;
					while (l < this.res)
					{
						this.dst[k * this.res + l] = pixels[num].a;
						l++;
						num++;
					}
					k++;
				}
				return;
			}
			Debug.LogError("Invalid alpha texture: " + this.AlphaTexture.name);
		}
	}

	// Token: 0x06002E5D RID: 11869 RVA: 0x00115F30 File Offset: 0x00114130
	public void GenerateTextures()
	{
		this.AlphaTexture = new Texture2D(this.res, this.res, TextureFormat.Alpha8, false, true);
		this.AlphaTexture.name = "AlphaTexture";
		this.AlphaTexture.wrapMode = TextureWrapMode.Clamp;
		Color32[] col = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				byte b = this.src[z * this.res + i];
				col[z * this.res + i] = new Color32(b, b, b, b);
			}
		});
		this.AlphaTexture.SetPixels32(col);
	}

	// Token: 0x06002E5E RID: 11870 RVA: 0x00115FC1 File Offset: 0x001141C1
	public void ApplyTextures()
	{
		this.AlphaTexture.Apply(true, false);
		this.AlphaTexture.Compress(false);
		this.AlphaTexture.Apply(false, true);
	}

	// Token: 0x06002E5F RID: 11871 RVA: 0x00115FEC File Offset: 0x001141EC
	public float GetAlpha(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetAlpha(normX, normZ);
	}

	// Token: 0x06002E60 RID: 11872 RVA: 0x0011601C File Offset: 0x0011421C
	public float GetAlpha(float normX, float normZ)
	{
		int num = this.res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int x = Mathf.Min(num4 + 1, num);
		int z = Mathf.Min(num5 + 1, num);
		float a = Mathf.Lerp(this.GetAlpha(num4, num5), this.GetAlpha(x, num5), num2 - (float)num4);
		float b = Mathf.Lerp(this.GetAlpha(num4, z), this.GetAlpha(x, z), num2 - (float)num4);
		return Mathf.Lerp(a, b, num3 - (float)num5);
	}

	// Token: 0x06002E61 RID: 11873 RVA: 0x001160AE File Offset: 0x001142AE
	public float GetAlpha(int x, int z)
	{
		return BitUtility.Byte2Float((int)this.src[z * this.res + x]);
	}

	// Token: 0x06002E62 RID: 11874 RVA: 0x001160C8 File Offset: 0x001142C8
	public void SetAlpha(Vector3 worldPos, float a)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetAlpha(normX, normZ, a);
	}

	// Token: 0x06002E63 RID: 11875 RVA: 0x001160F8 File Offset: 0x001142F8
	public void SetAlpha(float normX, float normZ, float a)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetAlpha(x, z, a);
	}

	// Token: 0x06002E64 RID: 11876 RVA: 0x0011611E File Offset: 0x0011431E
	public void SetAlpha(int x, int z, float a)
	{
		this.dst[z * this.res + x] = BitUtility.Float2Byte(a);
	}

	// Token: 0x06002E65 RID: 11877 RVA: 0x00116137 File Offset: 0x00114337
	public void SetAlpha(int x, int z, float a, float opacity)
	{
		this.SetAlpha(x, z, Mathf.Lerp(this.GetAlpha(x, z), a, opacity));
	}

	// Token: 0x06002E66 RID: 11878 RVA: 0x00116154 File Offset: 0x00114354
	public void SetAlpha(Vector3 worldPos, float a, float opacity, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetAlpha(normX, normZ, a, opacity, radius, fade);
	}

	// Token: 0x06002E67 RID: 11879 RVA: 0x00116188 File Offset: 0x00114388
	public void SetAlpha(float normX, float normZ, float a, float opacity, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			lerp *= opacity;
			if (lerp > 0f)
			{
				this.SetAlpha(x, z, a, lerp);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x04002623 RID: 9763
	[FormerlySerializedAs("ColorTexture")]
	public Texture2D AlphaTexture;
}
