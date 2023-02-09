using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x02000688 RID: 1672
public class GenerateBiome : ProceduralComponent
{
	// Token: 0x06002FCC RID: 12236
	[DllImport("RustNative", EntryPoint = "generate_biome")]
	public static extern void Native_GenerateBiome(byte[] map, int res, Vector3 pos, Vector3 size, uint seed, float lootAngle, float biomeAngle, short[] heightmap, int heightres);

	// Token: 0x06002FCD RID: 12237 RVA: 0x0011D564 File Offset: 0x0011B764
	public override void Process(uint seed)
	{
		byte[] dst = TerrainMeta.BiomeMap.dst;
		int res = TerrainMeta.BiomeMap.res;
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float lootAxisAngle = TerrainMeta.LootAxisAngle;
		float biomeAxisAngle = TerrainMeta.BiomeAxisAngle;
		short[] src = TerrainMeta.HeightMap.src;
		int res2 = TerrainMeta.HeightMap.res;
		GenerateBiome.Native_GenerateBiome(dst, res, position, size, seed, lootAxisAngle, biomeAxisAngle, src, res2);
	}
}
