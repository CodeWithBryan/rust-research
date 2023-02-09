using System;
using UnityEngine;

// Token: 0x02000673 RID: 1651
public class TerrainPlacementMap : TerrainMap<bool>
{
	// Token: 0x06002EE0 RID: 12000 RVA: 0x00118F64 File Offset: 0x00117164
	public override void Setup()
	{
		this.res = this.terrain.terrainData.alphamapResolution;
		this.src = (this.dst = new bool[this.res * this.res]);
		this.Enable();
	}

	// Token: 0x06002EE1 RID: 12001 RVA: 0x00118FAE File Offset: 0x001171AE
	public override void PostSetup()
	{
		this.res = 0;
		this.src = null;
		this.Disable();
	}

	// Token: 0x06002EE2 RID: 12002 RVA: 0x00118FC4 File Offset: 0x001171C4
	public void Enable()
	{
		this.isEnabled = true;
	}

	// Token: 0x06002EE3 RID: 12003 RVA: 0x00118FCD File Offset: 0x001171CD
	public void Disable()
	{
		this.isEnabled = false;
	}

	// Token: 0x06002EE4 RID: 12004 RVA: 0x00118FD8 File Offset: 0x001171D8
	public void Reset()
	{
		for (int i = 0; i < this.res; i++)
		{
			for (int j = 0; j < this.res; j++)
			{
				this.dst[i * this.res + j] = false;
			}
		}
	}

	// Token: 0x06002EE5 RID: 12005 RVA: 0x0011901C File Offset: 0x0011721C
	public bool GetBlocked(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetBlocked(normX, normZ);
	}

	// Token: 0x06002EE6 RID: 12006 RVA: 0x0011904C File Offset: 0x0011724C
	public bool GetBlocked(float normX, float normZ)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		return this.GetBlocked(x, z);
	}

	// Token: 0x06002EE7 RID: 12007 RVA: 0x00119071 File Offset: 0x00117271
	public bool GetBlocked(int x, int z)
	{
		return this.isEnabled && this.res > 0 && this.src[z * this.res + x];
	}

	// Token: 0x06002EE8 RID: 12008 RVA: 0x00119098 File Offset: 0x00117298
	public void SetBlocked(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetBlocked(normX, normZ);
	}

	// Token: 0x06002EE9 RID: 12009 RVA: 0x001190C8 File Offset: 0x001172C8
	public void SetBlocked(float normX, float normZ)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetBlocked(x, z);
	}

	// Token: 0x06002EEA RID: 12010 RVA: 0x001190ED File Offset: 0x001172ED
	public void SetBlocked(int x, int z)
	{
		this.dst[z * this.res + x] = true;
	}

	// Token: 0x06002EEB RID: 12011 RVA: 0x00119104 File Offset: 0x00117304
	public bool GetBlocked(Vector3 worldPos, float radius)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetBlocked(normX, normZ, radius);
	}

	// Token: 0x06002EEC RID: 12012 RVA: 0x00119134 File Offset: 0x00117334
	public bool GetBlocked(float normX, float normZ, float radius)
	{
		float num = TerrainMeta.OneOverSize.x * radius;
		int num2 = base.Index(normX - num);
		int num3 = base.Index(normX + num);
		int num4 = base.Index(normZ - num);
		int num5 = base.Index(normZ + num);
		for (int i = num4; i <= num5; i++)
		{
			for (int j = num2; j <= num3; j++)
			{
				if (this.src[i * this.res + j])
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002EED RID: 12013 RVA: 0x001191AC File Offset: 0x001173AC
	public void SetBlocked(Vector3 worldPos, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetBlocked(normX, normZ, radius, fade);
	}

	// Token: 0x06002EEE RID: 12014 RVA: 0x001191DC File Offset: 0x001173DC
	public void SetBlocked(float normX, float normZ, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if ((double)lerp > 0.5)
			{
				this.dst[z * this.res + x] = true;
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x0400262E RID: 9774
	private bool isEnabled;
}
