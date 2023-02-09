using System;
using System.Linq;

// Token: 0x02000698 RID: 1688
public class GenerateRailTexture : ProceduralComponent
{
	// Token: 0x0600300B RID: 12299 RVA: 0x00123F90 File Offset: 0x00122190
	public override void Process(uint seed)
	{
		foreach (PathList pathList in TerrainMeta.Path.Rails.AsEnumerable<PathList>().Reverse<PathList>())
		{
			pathList.AdjustTerrainTexture();
		}
	}
}
