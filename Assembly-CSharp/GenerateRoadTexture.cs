using System;
using System.Linq;

// Token: 0x020006A3 RID: 1699
public class GenerateRoadTexture : ProceduralComponent
{
	// Token: 0x06003026 RID: 12326 RVA: 0x00126130 File Offset: 0x00124330
	public override void Process(uint seed)
	{
		foreach (PathList pathList in TerrainMeta.Path.Roads.AsEnumerable<PathList>().Reverse<PathList>())
		{
			pathList.AdjustTerrainTexture();
		}
	}
}
