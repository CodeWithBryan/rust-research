using System;
using System.Linq;

// Token: 0x020006A4 RID: 1700
public class GenerateRoadTopology : ProceduralComponent
{
	// Token: 0x06003028 RID: 12328 RVA: 0x00126188 File Offset: 0x00124388
	public override void Process(uint seed)
	{
		foreach (PathList pathList in TerrainMeta.Path.Roads.AsEnumerable<PathList>().Reverse<PathList>())
		{
			pathList.AdjustTerrainTopology();
		}
		this.MarkRoadside();
		TerrainMeta.PlacementMap.Reset();
	}

	// Token: 0x06003029 RID: 12329 RVA: 0x001261F0 File Offset: 0x001243F0
	private void MarkRoadside()
	{
		TerrainHeightMap heightmap = TerrainMeta.HeightMap;
		TerrainTopologyMap topomap = TerrainMeta.TopologyMap;
		int[] map = topomap.dst;
		int res = topomap.res;
		ImageProcessing.Dilate2D(map, res, res, 6144, 6, delegate(int x, int y)
		{
			if ((map[x * res + y] & 49) != 0)
			{
				map[x * res + y] |= 4096;
			}
			float normX = topomap.Coordinate(x);
			float normZ = topomap.Coordinate(y);
			if (heightmap.GetSlope(normX, normZ) > 40f)
			{
				map[x * res + y] |= 2;
			}
		});
	}
}
