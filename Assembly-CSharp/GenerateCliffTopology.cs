using System;
using UnityEngine;

// Token: 0x0200068A RID: 1674
public class GenerateCliffTopology : ProceduralComponent
{
	// Token: 0x06002FD2 RID: 12242 RVA: 0x0011D670 File Offset: 0x0011B870
	public static void Process(int x, int z)
	{
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		float normZ = topologyMap.Coordinate(z);
		float normX = topologyMap.Coordinate(x);
		if ((topologyMap.GetTopology(x, z) & 8389632) == 0)
		{
			float slope = TerrainMeta.HeightMap.GetSlope(normX, normZ);
			float splat = TerrainMeta.SplatMap.GetSplat(normX, normZ, 8);
			if (slope > 40f || splat > 0.4f)
			{
				topologyMap.AddTopology(x, z, 2);
				return;
			}
			topologyMap.RemoveTopology(x, z, 2);
		}
	}

	// Token: 0x06002FD3 RID: 12243 RVA: 0x0011D6E0 File Offset: 0x0011B8E0
	private static void Process(int x, int z, bool keepExisting)
	{
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		float normZ = topologyMap.Coordinate(z);
		float normX = topologyMap.Coordinate(x);
		int topology = topologyMap.GetTopology(x, z);
		if (!World.Procedural || (topology & 8389632) == 0)
		{
			float slope = TerrainMeta.HeightMap.GetSlope(normX, normZ);
			float splat = TerrainMeta.SplatMap.GetSplat(normX, normZ, 8);
			if (slope > 40f || splat > 0.4f)
			{
				topologyMap.AddTopology(x, z, 2);
				return;
			}
			if (!keepExisting)
			{
				topologyMap.RemoveTopology(x, z, 2);
			}
		}
	}

	// Token: 0x06002FD4 RID: 12244 RVA: 0x0011D760 File Offset: 0x0011B960
	public override void Process(uint seed)
	{
		int[] map = TerrainMeta.TopologyMap.dst;
		int res = TerrainMeta.TopologyMap.res;
		Parallel.For(0, res, delegate(int z)
		{
			for (int i = 0; i < res; i++)
			{
				GenerateCliffTopology.Process(i, z, this.KeepExisting);
			}
		});
		ImageProcessing.Dilate2D(map, res, res, 4194306, 1, delegate(int x, int y)
		{
			if ((map[x * res + y] & 2) == 0)
			{
				map[x * res + y] |= 4194304;
			}
		});
	}

	// Token: 0x0400268B RID: 9867
	public bool KeepExisting = true;

	// Token: 0x0400268C RID: 9868
	private const int filter = 8389632;
}
