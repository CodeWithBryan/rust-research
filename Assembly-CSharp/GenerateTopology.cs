using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x020006A8 RID: 1704
public class GenerateTopology : ProceduralComponent
{
	// Token: 0x06003034 RID: 12340
	[DllImport("RustNative", EntryPoint = "generate_topology")]
	public static extern void Native_GenerateTopology(int[] map, int res, Vector3 pos, Vector3 size, uint seed, float lootAngle, float biomeAngle, short[] heightmap, int heightres, byte[] biomemap, int biomeres);

	// Token: 0x06003035 RID: 12341 RVA: 0x001263CC File Offset: 0x001245CC
	public override void Process(uint seed)
	{
		int[] dst = TerrainMeta.TopologyMap.dst;
		int res = TerrainMeta.TopologyMap.res;
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float lootAxisAngle = TerrainMeta.LootAxisAngle;
		float biomeAxisAngle = TerrainMeta.BiomeAxisAngle;
		short[] src = TerrainMeta.HeightMap.src;
		int res2 = TerrainMeta.HeightMap.res;
		byte[] src2 = TerrainMeta.BiomeMap.src;
		int res3 = TerrainMeta.BiomeMap.res;
		GenerateTopology.Native_GenerateTopology(dst, res, position, size, seed, lootAxisAngle, biomeAxisAngle, src, res2, src2, res3);
	}
}
