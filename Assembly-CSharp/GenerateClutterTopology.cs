using System;

// Token: 0x0200068B RID: 1675
public class GenerateClutterTopology : ProceduralComponent
{
	// Token: 0x06002FD6 RID: 12246 RVA: 0x0011D7EC File Offset: 0x0011B9EC
	public override void Process(uint seed)
	{
		int[] map = TerrainMeta.TopologyMap.dst;
		int res = TerrainMeta.TopologyMap.res;
		ImageProcessing.Dilate2D(map, res, res, 16777728, 3, delegate(int x, int y)
		{
			if ((map[x * res + y] & 512) == 0)
			{
				map[x * res + y] |= 16777216;
			}
		});
	}
}
