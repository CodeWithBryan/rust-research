using System;

// Token: 0x020006A9 RID: 1705
public class GenerateWireMeshes : ProceduralComponent
{
	// Token: 0x06003037 RID: 12343 RVA: 0x0012644B File Offset: 0x0012464B
	public override void Process(uint seed)
	{
		TerrainMeta.Path.CreateWires();
	}

	// Token: 0x170003B8 RID: 952
	// (get) Token: 0x06003038 RID: 12344 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}
}
