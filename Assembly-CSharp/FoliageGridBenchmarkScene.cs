using System;
using UnityEngine;

// Token: 0x0200000A RID: 10
public class FoliageGridBenchmarkScene : BenchmarkScene
{
	// Token: 0x04000014 RID: 20
	private static TerrainMeta terrainMeta;

	// Token: 0x04000015 RID: 21
	public GameObjectRef foliagePrefab;

	// Token: 0x04000016 RID: 22
	private GameObject foliageInstance;

	// Token: 0x04000017 RID: 23
	public GameObjectRef lodPrefab;

	// Token: 0x04000018 RID: 24
	private GameObject lodInstance;

	// Token: 0x04000019 RID: 25
	public GameObjectRef batchingPrefab;

	// Token: 0x0400001A RID: 26
	private GameObject batchingInstance;

	// Token: 0x0400001B RID: 27
	public Terrain terrain;

	// Token: 0x0400001C RID: 28
	public Transform viewpointA;

	// Token: 0x0400001D RID: 29
	public Transform viewpointB;

	// Token: 0x0400001E RID: 30
	public bool moveVantangePoint = true;
}
