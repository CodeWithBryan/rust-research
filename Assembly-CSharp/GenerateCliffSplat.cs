using System;
using UnityEngine;

// Token: 0x02000689 RID: 1673
public class GenerateCliffSplat : ProceduralComponent
{
	// Token: 0x06002FCF RID: 12239 RVA: 0x0011D5C8 File Offset: 0x0011B7C8
	public static void Process(int x, int z)
	{
		TerrainSplatMap splatMap = TerrainMeta.SplatMap;
		float normZ = splatMap.Coordinate(z);
		float normX = splatMap.Coordinate(x);
		if ((TerrainMeta.TopologyMap.GetTopology(normX, normZ) & 8389632) == 0)
		{
			float slope = TerrainMeta.HeightMap.GetSlope(normX, normZ);
			if (slope > 30f)
			{
				splatMap.SetSplat(x, z, 8, Mathf.InverseLerp(30f, 50f, slope));
			}
		}
	}

	// Token: 0x06002FD0 RID: 12240 RVA: 0x0011D630 File Offset: 0x0011B830
	public override void Process(uint seed)
	{
		TerrainSplatMap splatMap = TerrainMeta.SplatMap;
		int splatres = splatMap.res;
		Parallel.For(0, splatres, delegate(int z)
		{
			for (int i = 0; i < splatres; i++)
			{
				GenerateCliffSplat.Process(i, z);
			}
		});
	}

	// Token: 0x0400268A RID: 9866
	private const int filter = 8389632;
}
