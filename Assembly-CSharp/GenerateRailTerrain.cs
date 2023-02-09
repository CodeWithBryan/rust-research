using System;
using System.Linq;
using UnityEngine;

// Token: 0x02000697 RID: 1687
public class GenerateRailTerrain : ProceduralComponent
{
	// Token: 0x06003009 RID: 12297 RVA: 0x00123D78 File Offset: 0x00121F78
	public override void Process(uint seed)
	{
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Func<int, float> func = (int i) => Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0f, 8f, (float)i));
		for (int l = 0; l < 8; l++)
		{
			foreach (PathList pathList in TerrainMeta.Path.Rails.AsEnumerable<PathList>().Reverse<PathList>())
			{
				PathInterpolator path = pathList.Path;
				Vector3[] points = path.Points;
				for (int j = 0; j < points.Length; j++)
				{
					Vector3 vector = points[j];
					float t = pathList.Start ? func(j) : 1f;
					vector.y = Mathf.SmoothStep(vector.y, heightMap.GetHeight(vector), t);
					points[j] = vector;
				}
				path.Smoothen(8, Vector3.up, pathList.Start ? func : null);
				path.RecalculateTangents();
				heightMap.Push();
				float intensity = 1f;
				float fade = Mathf.InverseLerp(8f, 0f, (float)l);
				pathList.AdjustTerrainHeight(intensity, fade);
				heightMap.Pop();
			}
		}
		foreach (PathList pathList2 in TerrainMeta.Path.Rails)
		{
			PathInterpolator path2 = pathList2.Path;
			Vector3[] points2 = path2.Points;
			for (int k = 0; k < points2.Length; k++)
			{
				Vector3 vector2 = points2[k];
				float t2 = pathList2.Start ? func(k) : 1f;
				vector2.y = Mathf.SmoothStep(vector2.y, heightMap.GetHeight(vector2), t2);
				points2[k] = vector2;
			}
			path2.RecalculateTangents();
		}
	}

	// Token: 0x040026D6 RID: 9942
	public const int SmoothenLoops = 8;

	// Token: 0x040026D7 RID: 9943
	public const int SmoothenIterations = 8;

	// Token: 0x040026D8 RID: 9944
	public const int SmoothenY = 64;

	// Token: 0x040026D9 RID: 9945
	public const int SmoothenXZ = 32;

	// Token: 0x040026DA RID: 9946
	public const int TransitionSteps = 8;
}
