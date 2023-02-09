using System;
using UnityEngine;

// Token: 0x0200066E RID: 1646
public class TerrainBlendMap : TerrainMap<byte>
{
	// Token: 0x06002E81 RID: 11905 RVA: 0x00116A5C File Offset: 0x00114C5C
	public override void Setup()
	{
		if (!(this.BlendTexture != null))
		{
			this.res = this.terrain.terrainData.alphamapResolution;
			this.src = (this.dst = new byte[this.res * this.res]);
			for (int i = 0; i < this.res; i++)
			{
				for (int j = 0; j < this.res; j++)
				{
					this.dst[i * this.res + j] = 0;
				}
			}
			return;
		}
		if (this.BlendTexture.width == this.BlendTexture.height)
		{
			this.res = this.BlendTexture.width;
			this.src = (this.dst = new byte[this.res * this.res]);
			Color32[] pixels = this.BlendTexture.GetPixels32();
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
		Debug.LogError("Invalid alpha texture: " + this.BlendTexture.name);
	}

	// Token: 0x06002E82 RID: 11906 RVA: 0x00116BB0 File Offset: 0x00114DB0
	public void GenerateTextures()
	{
		this.BlendTexture = new Texture2D(this.res, this.res, TextureFormat.Alpha8, true, true);
		this.BlendTexture.name = "BlendTexture";
		this.BlendTexture.wrapMode = TextureWrapMode.Clamp;
		Color32[] col = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				byte b = this.src[z * this.res + i];
				col[z * this.res + i] = new Color32(b, b, b, b);
			}
		});
		this.BlendTexture.SetPixels32(col);
	}

	// Token: 0x06002E83 RID: 11907 RVA: 0x00116C41 File Offset: 0x00114E41
	public void ApplyTextures()
	{
		this.BlendTexture.Apply(true, false);
		this.BlendTexture.Compress(false);
		this.BlendTexture.Apply(false, true);
	}

	// Token: 0x06002E84 RID: 11908 RVA: 0x00116C6C File Offset: 0x00114E6C
	public float GetAlpha(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetAlpha(normX, normZ);
	}

	// Token: 0x06002E85 RID: 11909 RVA: 0x00116C9C File Offset: 0x00114E9C
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

	// Token: 0x06002E86 RID: 11910 RVA: 0x001160AE File Offset: 0x001142AE
	public float GetAlpha(int x, int z)
	{
		return BitUtility.Byte2Float((int)this.src[z * this.res + x]);
	}

	// Token: 0x06002E87 RID: 11911 RVA: 0x00116D30 File Offset: 0x00114F30
	public void SetAlpha(Vector3 worldPos, float a)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetAlpha(normX, normZ, a);
	}

	// Token: 0x06002E88 RID: 11912 RVA: 0x00116D60 File Offset: 0x00114F60
	public void SetAlpha(float normX, float normZ, float a)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetAlpha(x, z, a);
	}

	// Token: 0x06002E89 RID: 11913 RVA: 0x0011611E File Offset: 0x0011431E
	public void SetAlpha(int x, int z, float a)
	{
		this.dst[z * this.res + x] = BitUtility.Float2Byte(a);
	}

	// Token: 0x06002E8A RID: 11914 RVA: 0x00116D86 File Offset: 0x00114F86
	public void SetAlpha(int x, int z, float a, float opacity)
	{
		this.SetAlpha(x, z, Mathf.Lerp(this.GetAlpha(x, z), a, opacity));
	}

	// Token: 0x06002E8B RID: 11915 RVA: 0x00116DA0 File Offset: 0x00114FA0
	public void SetAlpha(Vector3 worldPos, float a, float opacity, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetAlpha(normX, normZ, a, opacity, radius, fade);
	}

	// Token: 0x06002E8C RID: 11916 RVA: 0x00116DD4 File Offset: 0x00114FD4
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

	// Token: 0x04002626 RID: 9766
	public Texture2D BlendTexture;
}
