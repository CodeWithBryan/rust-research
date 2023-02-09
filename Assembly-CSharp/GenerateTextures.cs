using System;

// Token: 0x020006A7 RID: 1703
public class GenerateTextures : ProceduralComponent
{
	// Token: 0x06003031 RID: 12337 RVA: 0x00126328 File Offset: 0x00124528
	public override void Process(uint seed)
	{
		if (!World.Cached)
		{
			World.AddMap("height", TerrainMeta.HeightMap.ToByteArray());
			World.AddMap("splat", TerrainMeta.SplatMap.ToByteArray());
			World.AddMap("biome", TerrainMeta.BiomeMap.ToByteArray());
			World.AddMap("topology", TerrainMeta.TopologyMap.ToByteArray());
			World.AddMap("alpha", TerrainMeta.AlphaMap.ToByteArray());
			World.AddMap("water", TerrainMeta.WaterMap.ToByteArray());
			return;
		}
		TerrainMeta.HeightMap.FromByteArray(World.GetMap("height"));
	}

	// Token: 0x170003B7 RID: 951
	// (get) Token: 0x06003032 RID: 12338 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}
}
