using System;

// Token: 0x020006A6 RID: 1702
public class GenerateTerrainMesh : ProceduralComponent
{
	// Token: 0x0600302E RID: 12334 RVA: 0x001262FF File Offset: 0x001244FF
	public override void Process(uint seed)
	{
		if (!World.Cached)
		{
			World.AddMap("terrain", TerrainMeta.HeightMap.ToByteArray());
		}
		TerrainMeta.HeightMap.ApplyToTerrain();
	}

	// Token: 0x170003B6 RID: 950
	// (get) Token: 0x0600302F RID: 12335 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}
}
