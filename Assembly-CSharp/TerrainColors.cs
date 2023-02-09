using System;
using UnityEngine;

// Token: 0x0200066A RID: 1642
public class TerrainColors : TerrainExtension
{
	// Token: 0x06002E53 RID: 11859 RVA: 0x00115CCC File Offset: 0x00113ECC
	public override void Setup()
	{
		this.splatMap = this.terrain.GetComponent<TerrainSplatMap>();
		this.biomeMap = this.terrain.GetComponent<TerrainBiomeMap>();
	}

	// Token: 0x06002E54 RID: 11860 RVA: 0x00115CF0 File Offset: 0x00113EF0
	public Color GetColor(Vector3 worldPos, int mask = -1)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetColor(normX, normZ, mask);
	}

	// Token: 0x06002E55 RID: 11861 RVA: 0x00115D20 File Offset: 0x00113F20
	public Color GetColor(float normX, float normZ, int mask = -1)
	{
		float biome = this.biomeMap.GetBiome(normX, normZ, 1);
		float biome2 = this.biomeMap.GetBiome(normX, normZ, 2);
		float biome3 = this.biomeMap.GetBiome(normX, normZ, 4);
		float biome4 = this.biomeMap.GetBiome(normX, normZ, 8);
		int num = TerrainSplat.TypeToIndex(this.splatMap.GetSplatMaxType(normX, normZ, mask));
		TerrainConfig.SplatType splatType = this.config.Splats[num];
		return biome * splatType.AridColor + biome2 * splatType.TemperateColor + biome3 * splatType.TundraColor + biome4 * splatType.ArcticColor;
	}

	// Token: 0x0400261E RID: 9758
	private TerrainSplatMap splatMap;

	// Token: 0x0400261F RID: 9759
	private TerrainBiomeMap biomeMap;
}
