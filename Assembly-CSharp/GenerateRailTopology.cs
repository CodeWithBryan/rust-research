using System;
using System.Linq;

// Token: 0x02000699 RID: 1689
public class GenerateRailTopology : ProceduralComponent
{
	// Token: 0x0600300D RID: 12301 RVA: 0x00123FE8 File Offset: 0x001221E8
	public override void Process(uint seed)
	{
		foreach (PathList pathList in TerrainMeta.Path.Rails.AsEnumerable<PathList>().Reverse<PathList>())
		{
			pathList.AdjustTerrainTopology();
		}
		this.MarkRailside();
		TerrainMeta.PlacementMap.Reset();
	}

	// Token: 0x0600300E RID: 12302 RVA: 0x00124050 File Offset: 0x00122250
	private void MarkRailside()
	{
		TerrainHeightMap heightmap = TerrainMeta.HeightMap;
		TerrainTopologyMap topomap = TerrainMeta.TopologyMap;
		int[] map = topomap.dst;
		int res = topomap.res;
		ImageProcessing.Dilate2D(map, res, res, 1572864, 6, delegate(int x, int y)
		{
			if ((map[x * res + y] & 49) != 0)
			{
				map[x * res + y] |= 1048576;
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
