using System;
using UnityEngine;

// Token: 0x02000675 RID: 1653
public class TerrainTopologyMap : TerrainMap<int>
{
	// Token: 0x06002F0D RID: 12045 RVA: 0x0011A03C File Offset: 0x0011823C
	public override void Setup()
	{
		this.res = this.terrain.terrainData.alphamapResolution;
		this.src = (this.dst = new int[this.res * this.res]);
		if (this.TopologyTexture != null)
		{
			if (this.TopologyTexture.width == this.TopologyTexture.height && this.TopologyTexture.width == this.res)
			{
				Color32[] pixels = this.TopologyTexture.GetPixels32();
				int i = 0;
				int num = 0;
				while (i < this.res)
				{
					int j = 0;
					while (j < this.res)
					{
						this.dst[i * this.res + j] = BitUtility.DecodeInt(pixels[num]);
						j++;
						num++;
					}
					i++;
				}
				return;
			}
			Debug.LogError("Invalid topology texture: " + this.TopologyTexture.name);
		}
	}

	// Token: 0x06002F0E RID: 12046 RVA: 0x0011A130 File Offset: 0x00118330
	public void GenerateTextures()
	{
		this.TopologyTexture = new Texture2D(this.res, this.res, TextureFormat.RGBA32, false, true);
		this.TopologyTexture.name = "TopologyTexture";
		this.TopologyTexture.wrapMode = TextureWrapMode.Clamp;
		Color32[] col = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				col[z * this.res + i] = BitUtility.EncodeInt(this.src[z * this.res + i]);
			}
		});
		this.TopologyTexture.SetPixels32(col);
	}

	// Token: 0x06002F0F RID: 12047 RVA: 0x0011A1C1 File Offset: 0x001183C1
	public void ApplyTextures()
	{
		this.TopologyTexture.Apply(false, true);
	}

	// Token: 0x06002F10 RID: 12048 RVA: 0x0011A1D0 File Offset: 0x001183D0
	public bool GetTopology(Vector3 worldPos, int mask)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetTopology(normX, normZ, mask);
	}

	// Token: 0x06002F11 RID: 12049 RVA: 0x0011A200 File Offset: 0x00118400
	public bool GetTopology(float normX, float normZ, int mask)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		return this.GetTopology(x, z, mask);
	}

	// Token: 0x06002F12 RID: 12050 RVA: 0x0011A226 File Offset: 0x00118426
	public bool GetTopology(int x, int z, int mask)
	{
		return (this.src[z * this.res + x] & mask) != 0;
	}

	// Token: 0x06002F13 RID: 12051 RVA: 0x0011A240 File Offset: 0x00118440
	public int GetTopology(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetTopology(normX, normZ);
	}

	// Token: 0x06002F14 RID: 12052 RVA: 0x0011A270 File Offset: 0x00118470
	public int GetTopology(float normX, float normZ)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		return this.GetTopology(x, z);
	}

	// Token: 0x06002F15 RID: 12053 RVA: 0x0011A298 File Offset: 0x00118498
	public int GetTopologyFast(Vector2 uv)
	{
		int num = this.res - 1;
		int num2 = (int)(uv.x * (float)this.res);
		int num3 = (int)(uv.y * (float)this.res);
		num2 = ((num2 >= 0) ? num2 : 0);
		num3 = ((num3 >= 0) ? num3 : 0);
		num2 = ((num2 <= num) ? num2 : num);
		num3 = ((num3 <= num) ? num3 : num);
		return this.src[num3 * this.res + num2];
	}

	// Token: 0x06002F16 RID: 12054 RVA: 0x0011A303 File Offset: 0x00118503
	public int GetTopology(int x, int z)
	{
		return this.src[z * this.res + x];
	}

	// Token: 0x06002F17 RID: 12055 RVA: 0x0011A318 File Offset: 0x00118518
	public void SetTopology(Vector3 worldPos, int mask)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetTopology(normX, normZ, mask);
	}

	// Token: 0x06002F18 RID: 12056 RVA: 0x0011A348 File Offset: 0x00118548
	public void SetTopology(float normX, float normZ, int mask)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetTopology(x, z, mask);
	}

	// Token: 0x06002F19 RID: 12057 RVA: 0x0011A36E File Offset: 0x0011856E
	public void SetTopology(int x, int z, int mask)
	{
		this.dst[z * this.res + x] = mask;
	}

	// Token: 0x06002F1A RID: 12058 RVA: 0x0011A384 File Offset: 0x00118584
	public void AddTopology(Vector3 worldPos, int mask)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.AddTopology(normX, normZ, mask);
	}

	// Token: 0x06002F1B RID: 12059 RVA: 0x0011A3B4 File Offset: 0x001185B4
	public void AddTopology(float normX, float normZ, int mask)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.AddTopology(x, z, mask);
	}

	// Token: 0x06002F1C RID: 12060 RVA: 0x0011A3DA File Offset: 0x001185DA
	public void AddTopology(int x, int z, int mask)
	{
		this.dst[z * this.res + x] |= mask;
	}

	// Token: 0x06002F1D RID: 12061 RVA: 0x0011A3F8 File Offset: 0x001185F8
	public void RemoveTopology(Vector3 worldPos, int mask)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.RemoveTopology(normX, normZ, mask);
	}

	// Token: 0x06002F1E RID: 12062 RVA: 0x0011A428 File Offset: 0x00118628
	public void RemoveTopology(float normX, float normZ, int mask)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.RemoveTopology(x, z, mask);
	}

	// Token: 0x06002F1F RID: 12063 RVA: 0x0011A44E File Offset: 0x0011864E
	public void RemoveTopology(int x, int z, int mask)
	{
		this.dst[z * this.res + x] &= ~mask;
	}

	// Token: 0x06002F20 RID: 12064 RVA: 0x0011A46C File Offset: 0x0011866C
	public int GetTopology(Vector3 worldPos, float radius)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetTopology(normX, normZ, radius);
	}

	// Token: 0x06002F21 RID: 12065 RVA: 0x0011A49C File Offset: 0x0011869C
	public int GetTopology(float normX, float normZ, float radius)
	{
		int num = 0;
		float num2 = TerrainMeta.OneOverSize.x * radius;
		int num3 = base.Index(normX - num2);
		int num4 = base.Index(normX + num2);
		int num5 = base.Index(normZ - num2);
		int num6 = base.Index(normZ + num2);
		for (int i = num5; i <= num6; i++)
		{
			for (int j = num3; j <= num4; j++)
			{
				num |= this.src[i * this.res + j];
			}
		}
		return num;
	}

	// Token: 0x06002F22 RID: 12066 RVA: 0x0011A518 File Offset: 0x00118718
	public void SetTopology(Vector3 worldPos, int mask, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetTopology(normX, normZ, mask, radius, fade);
	}

	// Token: 0x06002F23 RID: 12067 RVA: 0x0011A54C File Offset: 0x0011874C
	public void SetTopology(float normX, float normZ, int mask, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if ((double)lerp > 0.5)
			{
				this.dst[z * this.res + x] = mask;
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x06002F24 RID: 12068 RVA: 0x0011A588 File Offset: 0x00118788
	public void AddTopology(Vector3 worldPos, int mask, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.AddTopology(normX, normZ, mask, radius, fade);
	}

	// Token: 0x06002F25 RID: 12069 RVA: 0x0011A5BC File Offset: 0x001187BC
	public void AddTopology(float normX, float normZ, int mask, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if ((double)lerp > 0.5)
			{
				this.dst[z * this.res + x] |= mask;
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x04002632 RID: 9778
	public Texture2D TopologyTexture;
}
