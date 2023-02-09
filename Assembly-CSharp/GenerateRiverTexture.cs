using System;
using System.Linq;

// Token: 0x0200069D RID: 1693
public class GenerateRiverTexture : ProceduralComponent
{
	// Token: 0x06003017 RID: 12311 RVA: 0x00124920 File Offset: 0x00122B20
	public override void Process(uint seed)
	{
		foreach (PathList pathList in TerrainMeta.Path.Rivers.AsEnumerable<PathList>().Reverse<PathList>())
		{
			pathList.AdjustTerrainTexture();
		}
	}
}
