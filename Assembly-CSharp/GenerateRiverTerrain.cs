using System;
using System.Linq;
using UnityEngine;

// Token: 0x0200069C RID: 1692
public class GenerateRiverTerrain : ProceduralComponent
{
	// Token: 0x06003015 RID: 12309 RVA: 0x00124860 File Offset: 0x00122A60
	public override void Process(uint seed)
	{
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		for (int i = 0; i < 1; i++)
		{
			foreach (PathList pathList in TerrainMeta.Path.Rivers.AsEnumerable<PathList>().Reverse<PathList>())
			{
				if (!World.Networked)
				{
					PathInterpolator path = pathList.Path;
					path.Smoothen(8, Vector3.up, null);
					path.RecalculateTangents();
				}
				heightMap.Push();
				float intensity = 1f;
				float fade = 1f / (1f + (float)i / 3f);
				pathList.AdjustTerrainHeight(intensity, fade);
				heightMap.Pop();
			}
		}
	}

	// Token: 0x040026E9 RID: 9961
	public const int SmoothenLoops = 1;

	// Token: 0x040026EA RID: 9962
	public const int SmoothenIterations = 8;

	// Token: 0x040026EB RID: 9963
	public const int SmoothenY = 8;

	// Token: 0x040026EC RID: 9964
	public const int SmoothenXZ = 4;
}
