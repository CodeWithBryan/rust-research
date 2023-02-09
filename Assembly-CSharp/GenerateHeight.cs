using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x0200068F RID: 1679
public class GenerateHeight : ProceduralComponent
{
	// Token: 0x06002FEE RID: 12270
	[DllImport("RustNative", EntryPoint = "generate_height")]
	public static extern void Native_GenerateHeight(short[] map, int res, Vector3 pos, Vector3 size, uint seed, float lootAngle, float biomeAngle);

	// Token: 0x06002FEF RID: 12271 RVA: 0x00121454 File Offset: 0x0011F654
	public override void Process(uint seed)
	{
		short[] dst = TerrainMeta.HeightMap.dst;
		int res = TerrainMeta.HeightMap.res;
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float lootAxisAngle = TerrainMeta.LootAxisAngle;
		float biomeAxisAngle = TerrainMeta.BiomeAxisAngle;
		GenerateHeight.Native_GenerateHeight(dst, res, position, size, seed, lootAxisAngle, biomeAxisAngle);
	}
}
