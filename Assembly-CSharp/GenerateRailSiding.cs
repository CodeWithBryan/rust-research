using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000696 RID: 1686
public class GenerateRailSiding : ProceduralComponent
{
	// Token: 0x06003004 RID: 12292 RVA: 0x001236E4 File Offset: 0x001218E4
	private PathList CreateSegment(int number, Vector3[] points)
	{
		return new PathList("Rail " + number, points)
		{
			Spline = true,
			Width = 4f,
			InnerPadding = 1f,
			OuterPadding = 1f,
			InnerFade = 1f,
			OuterFade = 32f,
			RandomScale = 1f,
			MeshOffset = 0f,
			TerrainOffset = -0.125f,
			Topology = 524288,
			Splat = 128,
			Hierarchy = 2
		};
	}

	// Token: 0x06003005 RID: 12293 RVA: 0x00123784 File Offset: 0x00121984
	public override void Process(uint seed)
	{
		if (World.Networked)
		{
			TerrainMeta.Path.Rails.Clear();
			TerrainMeta.Path.Rails.AddRange(World.GetPaths("Rail"));
			return;
		}
		int min = Mathf.RoundToInt(40f);
		int max = Mathf.RoundToInt(53.333332f);
		int min2 = Mathf.RoundToInt(13.333333f);
		int max2 = Mathf.RoundToInt(20f);
		float num = 16f;
		float num2 = num * num;
		List<PathList> list = new List<PathList>();
		int[,] array = TerrainPath.CreateRailCostmap(ref seed);
		new PathFinder(array, true, true);
		array.GetLength(0);
		List<Vector3> list2 = new List<Vector3>();
		List<Vector3> list3 = new List<Vector3>();
		HashSet<Vector3> hashSet = new HashSet<Vector3>();
		foreach (PathList pathList in TerrainMeta.Path.Rails)
		{
			foreach (PathList pathList2 in TerrainMeta.Path.Rails)
			{
				if (pathList != pathList2)
				{
					foreach (Vector3 vector in pathList.Path.Points)
					{
						foreach (Vector3 b in pathList2.Path.Points)
						{
							if ((vector - b).sqrMagnitude < num2)
							{
								hashSet.Add(vector);
								break;
							}
						}
					}
				}
			}
		}
		foreach (PathList pathList3 in TerrainMeta.Path.Rails)
		{
			PathInterpolator path = pathList3.Path;
			Vector3[] points3 = path.Points;
			Vector3[] tangents = path.Tangents;
			int num3 = path.MinIndex + 1 + 16;
			int num4 = path.MaxIndex - 1 - 16;
			for (int k = num3; k <= num4; k++)
			{
				list2.Clear();
				list3.Clear();
				int num5 = SeedRandom.Range(ref seed, min2, max2);
				int num6 = SeedRandom.Range(ref seed, min, max);
				int num7 = k;
				int num8 = k + num5;
				if (num8 < num4)
				{
					Vector3 from = tangents[num7];
					Vector3 to = tangents[num8];
					if (Vector3.Angle(from, to) <= 30f)
					{
						Vector3 to2 = tangents[num7];
						Vector3 to3 = tangents[num8];
						Vector3 from2 = Vector3.Normalize(points3[num7 + 8] - points3[num7]);
						Vector3 from3 = Vector3.Normalize(points3[num8] - points3[num8 - 8]);
						float num9 = Vector3.SignedAngle(from2, to2, Vector3.up);
						float f = -Vector3.SignedAngle(from3, to3, Vector3.up);
						if (Mathf.Sign(num9) == Mathf.Sign(f) && Mathf.Abs(num9) <= 60f && Mathf.Abs(f) <= 60f)
						{
							float num10 = 5f;
							Quaternion rotation = (num9 > 0f) ? GenerateRailSiding.rotRight : GenerateRailSiding.rotLeft;
							for (int l = num7 - 8; l <= num8 + 8; l++)
							{
								Vector3 vector2 = points3[l];
								if (hashSet.Contains(vector2))
								{
									list2.Clear();
									list3.Clear();
									break;
								}
								Vector3 vector3 = tangents[l];
								Vector3 a = rotation * vector3;
								if (l < num7 + 8)
								{
									float t = Mathf.InverseLerp((float)(num7 - 8), (float)num7, (float)l);
									float d = Mathf.SmoothStep(0f, 1f, t) * num10;
									vector2 += a * d;
								}
								else if (l > num8 - 8)
								{
									float t2 = Mathf.InverseLerp((float)(num8 + 8), (float)num8, (float)l);
									float d2 = Mathf.SmoothStep(0f, 1f, t2) * num10;
									vector2 += a * d2;
								}
								else
								{
									vector2 += a * num10;
								}
								list2.Add(vector2);
								list3.Add(vector3);
							}
							if (list2.Count >= 2)
							{
								int number = TerrainMeta.Path.Rails.Count + list.Count;
								PathList pathList4 = this.CreateSegment(number, list2.ToArray());
								pathList4.Start = false;
								pathList4.End = false;
								list.Add(pathList4);
								k += num5;
							}
							k += num6;
						}
					}
				}
			}
		}
		foreach (PathList pathList5 in list)
		{
			pathList5.Path.Resample(7.5f);
			pathList5.Path.RecalculateTangents();
			pathList5.AdjustPlacementMap(20f);
		}
		TerrainMeta.Path.Rails.AddRange(list);
	}

	// Token: 0x06003006 RID: 12294 RVA: 0x00123CEC File Offset: 0x00121EEC
	public PathFinder.Point GetPathFinderPoint(Vector3 worldPos, int res)
	{
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		return new PathFinder.Point
		{
			x = Mathf.Clamp((int)(num * (float)res), 0, res - 1),
			y = Mathf.Clamp((int)(num2 * (float)res), 0, res - 1)
		};
	}

	// Token: 0x040026CB RID: 9931
	public const float Width = 4f;

	// Token: 0x040026CC RID: 9932
	public const float InnerPadding = 1f;

	// Token: 0x040026CD RID: 9933
	public const float OuterPadding = 1f;

	// Token: 0x040026CE RID: 9934
	public const float InnerFade = 1f;

	// Token: 0x040026CF RID: 9935
	public const float OuterFade = 32f;

	// Token: 0x040026D0 RID: 9936
	public const float RandomScale = 1f;

	// Token: 0x040026D1 RID: 9937
	public const float MeshOffset = 0f;

	// Token: 0x040026D2 RID: 9938
	public const float TerrainOffset = -0.125f;

	// Token: 0x040026D3 RID: 9939
	private static Quaternion rotRight = Quaternion.Euler(0f, 90f, 0f);

	// Token: 0x040026D4 RID: 9940
	private static Quaternion rotLeft = Quaternion.Euler(0f, -90f, 0f);

	// Token: 0x040026D5 RID: 9941
	private const int MaxDepth = 250000;
}
