using System;
using UnityEngine;

// Token: 0x0200066F RID: 1647
public class TerrainDistanceMap : TerrainMap<byte>
{
	// Token: 0x06002E8E RID: 11918 RVA: 0x00116E18 File Offset: 0x00115018
	public override void Setup()
	{
		this.res = this.terrain.terrainData.heightmapResolution;
		this.src = (this.dst = new byte[4 * this.res * this.res]);
		if (this.DistanceTexture != null)
		{
			if (this.DistanceTexture.width == this.DistanceTexture.height && this.DistanceTexture.width == this.res)
			{
				Color32[] pixels = this.DistanceTexture.GetPixels32();
				int i = 0;
				int num = 0;
				while (i < this.res)
				{
					int j = 0;
					while (j < this.res)
					{
						this.SetDistance(j, i, BitUtility.DecodeVector2i(pixels[num]));
						j++;
						num++;
					}
					i++;
				}
				return;
			}
			Debug.LogError("Invalid distance texture: " + this.DistanceTexture.name, this.DistanceTexture);
		}
	}

	// Token: 0x06002E8F RID: 11919 RVA: 0x00116F08 File Offset: 0x00115108
	public void GenerateTextures()
	{
		this.DistanceTexture = new Texture2D(this.res, this.res, TextureFormat.RGBA32, true, true);
		this.DistanceTexture.name = "DistanceTexture";
		this.DistanceTexture.wrapMode = TextureWrapMode.Clamp;
		Color32[] cols = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				cols[z * this.res + i] = BitUtility.EncodeVector2i(this.GetDistance(i, z));
			}
		});
		this.DistanceTexture.SetPixels32(cols);
	}

	// Token: 0x06002E90 RID: 11920 RVA: 0x00116F99 File Offset: 0x00115199
	public void ApplyTextures()
	{
		this.DistanceTexture.Apply(true, true);
	}

	// Token: 0x06002E91 RID: 11921 RVA: 0x00116FA8 File Offset: 0x001151A8
	public Vector2i GetDistance(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetDistance(normX, normZ);
	}

	// Token: 0x06002E92 RID: 11922 RVA: 0x00116FD8 File Offset: 0x001151D8
	public Vector2i GetDistance(float normX, float normZ)
	{
		int num = this.res - 1;
		int x = Mathf.Clamp(Mathf.RoundToInt(normX * (float)num), 0, num);
		int z = Mathf.Clamp(Mathf.RoundToInt(normZ * (float)num), 0, num);
		return this.GetDistance(x, z);
	}

	// Token: 0x06002E93 RID: 11923 RVA: 0x00117018 File Offset: 0x00115218
	public Vector2i GetDistance(int x, int z)
	{
		byte[] src = this.src;
		int res = this.res;
		byte b = src[(0 + z) * this.res + x];
		byte b2 = this.src[(this.res + z) * this.res + x];
		byte b3 = this.src[(2 * this.res + z) * this.res + x];
		byte b4 = this.src[(3 * this.res + z) * this.res + x];
		if (b == 255 && b2 == 255 && b3 == 255 && b4 == 255)
		{
			return new Vector2i(256, 256);
		}
		return new Vector2i((int)(b - b2), (int)(b3 - b4));
	}

	// Token: 0x06002E94 RID: 11924 RVA: 0x001170CC File Offset: 0x001152CC
	public void SetDistance(int x, int z, Vector2i v)
	{
		byte[] dst = this.dst;
		int res = this.res;
		dst[(0 + z) * this.res + x] = (byte)Mathf.Clamp(v.x, 0, 255);
		this.dst[(this.res + z) * this.res + x] = (byte)Mathf.Clamp(-v.x, 0, 255);
		this.dst[(2 * this.res + z) * this.res + x] = (byte)Mathf.Clamp(v.y, 0, 255);
		this.dst[(3 * this.res + z) * this.res + x] = (byte)Mathf.Clamp(-v.y, 0, 255);
	}

	// Token: 0x04002627 RID: 9767
	public Texture2D DistanceTexture;
}
