using System;
using System.Linq;

// Token: 0x0200069E RID: 1694
public class GenerateRiverTopology : ProceduralComponent
{
	// Token: 0x06003019 RID: 12313 RVA: 0x00124978 File Offset: 0x00122B78
	public override void Process(uint seed)
	{
		foreach (PathList pathList in TerrainMeta.Path.Rivers.AsEnumerable<PathList>().Reverse<PathList>())
		{
			pathList.AdjustTerrainTopology();
		}
		this.MarkRiverside();
	}

	// Token: 0x0600301A RID: 12314 RVA: 0x001249D8 File Offset: 0x00122BD8
	public void MarkRiverside()
	{
		TerrainHeightMap heightmap = TerrainMeta.HeightMap;
		TerrainTopologyMap topomap = TerrainMeta.TopologyMap;
		int[] map = topomap.dst;
		int res = topomap.res;
		ImageProcessing.Dilate2D(map, res, res, 49152, 6, delegate(int x, int y)
		{
			if ((map[x * res + y] & 49) != 0)
			{
				map[x * res + y] |= 32768;
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
