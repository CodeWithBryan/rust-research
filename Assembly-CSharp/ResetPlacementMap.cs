using System;

// Token: 0x020006B8 RID: 1720
public class ResetPlacementMap : ProceduralComponent
{
	// Token: 0x06003061 RID: 12385 RVA: 0x0012A13C File Offset: 0x0012833C
	public override void Process(uint seed)
	{
		TerrainMeta.PlacementMap.Reset();
	}
}
