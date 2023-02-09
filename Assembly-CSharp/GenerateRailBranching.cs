using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000692 RID: 1682
public class GenerateRailBranching : ProceduralComponent
{
	// Token: 0x06002FF5 RID: 12277 RVA: 0x001219E0 File Offset: 0x0011FBE0
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
			Hierarchy = 1
		};
	}

	// Token: 0x06002FF6 RID: 12278 RVA: 0x00121A80 File Offset: 0x0011FC80
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
		int min2 = Mathf.RoundToInt(40f);
		int max2 = Mathf.RoundToInt(120f);
		float num = 120f;
		float num2 = num * num;
		List<PathList> list = new List<PathList>();
		int[,] array = TerrainPath.CreateRailCostmap(ref seed);
		PathFinder pathFinder = new PathFinder(array, true, true);
		int length = array.GetLength(0);
		List<Vector3> list2 = new List<Vector3>();
		List<Vector3> list3 = new List<Vector3>();
		List<Vector3> list4 = new List<Vector3>();
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
			int num3 = path.MinIndex + 1 + 8;
			int num4 = path.MaxIndex - 1 - 8;
			for (int k = num3; k <= num4; k++)
			{
				list2.Clear();
				list3.Clear();
				list4.Clear();
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
						Vector3 vector2 = points3[num7];
						Vector3 vector3 = points3[num8];
						if (!hashSet.Contains(vector2) && !hashSet.Contains(vector3))
						{
							PathFinder.Point pathFinderPoint = this.GetPathFinderPoint(vector2, length);
							PathFinder.Point pathFinderPoint2 = this.GetPathFinderPoint(vector3, length);
							k += num6;
							PathFinder.Node node = pathFinder.FindPath(pathFinderPoint, pathFinderPoint2, 250000);
							if (node != null)
							{
								PathFinder.Node node2 = null;
								PathFinder.Node node3 = null;
								PathFinder.Node node4 = node;
								while (node4 != null && node4.next != null)
								{
									if (node4 == node.next)
									{
										node2 = node4;
									}
									if (node4.next.next == null)
									{
										node3 = node4;
									}
									node4 = node4.next;
								}
								if (node2 != null && node3 != null)
								{
									node3.next = null;
									for (int l = 0; l < 8; l++)
									{
										int num9 = num7 + (l + 1 - 8);
										int num10 = num8 + l;
										list2.Add(points3[num9]);
										list3.Add(points3[num10]);
									}
									list4.AddRange(list2);
									for (PathFinder.Node node5 = node2; node5 != null; node5 = node5.next)
									{
										float normX = ((float)node5.point.x + 0.5f) / (float)length;
										float normZ = ((float)node5.point.y + 0.5f) / (float)length;
										float x = TerrainMeta.DenormalizeX(normX);
										float z = TerrainMeta.DenormalizeZ(normZ);
										float y = Mathf.Max(TerrainMeta.HeightMap.GetHeight(normX, normZ), 1f);
										list4.Add(new Vector3(x, y, z));
									}
									list4.AddRange(list3);
									int num11 = 8;
									int num12 = list4.Count - 1 - 8;
									Vector3 to2 = Vector3.Normalize(list4[num11 + 16] - list4[num11]);
									Vector3 to3 = Vector3.Normalize(list4[num12] - list4[num12 - 16]);
									Vector3 vector4 = Vector3.Normalize(points3[num7 + 16] - points3[num7]);
									Vector3 vector5 = Vector3.Normalize(points3[num8] - points3[(num8 - 16 + points3.Length) % points3.Length]);
									float num13 = Vector3.SignedAngle(vector4, to2, Vector3.up);
									float num14 = -Vector3.SignedAngle(vector5, to3, Vector3.up);
									if (Mathf.Sign(num13) == Mathf.Sign(num14) && Mathf.Abs(num13) <= 60f && Mathf.Abs(num14) <= 60f)
									{
										Vector3 a = GenerateRailBranching.rot90 * vector4;
										Vector3 a2 = GenerateRailBranching.rot90 * vector5;
										if (num13 < 0f)
										{
											a = -a;
										}
										if (num14 < 0f)
										{
											a2 = -a2;
										}
										for (int m = 0; m < 16; m++)
										{
											int num15 = m;
											int num16 = list4.Count - m - 1;
											float t = Mathf.InverseLerp(0f, 8f, (float)m);
											float d = Mathf.SmoothStep(0f, 2f, t) * 4f;
											List<Vector3> list5 = list4;
											int n = num15;
											list5[n] += a * d;
											list5 = list4;
											n = num16;
											list5[n] += a2 * d;
										}
										bool flag = false;
										bool flag2 = false;
										foreach (Vector3 worldPos in list4)
										{
											bool blocked = TerrainMeta.PlacementMap.GetBlocked(worldPos);
											if (!flag2 && !flag && !blocked)
											{
												flag = true;
											}
											if (flag && !flag2 && blocked)
											{
												flag2 = true;
											}
											if (flag && flag2 && !blocked)
											{
												list4.Clear();
												break;
											}
										}
										if (list4.Count != 0)
										{
											if (list4.Count >= 2)
											{
												int number = TerrainMeta.Path.Rails.Count + list.Count;
												PathList pathList4 = this.CreateSegment(number, list4.ToArray());
												pathList4.Start = false;
												pathList4.End = false;
												pathList4.ProcgenStartNode = node2;
												pathList4.ProcgenEndNode = node3;
												list.Add(pathList4);
											}
											k += num5;
										}
									}
								}
							}
						}
					}
				}
			}
		}
		using (List<PathList>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				PathList rail = enumerator.Current;
				Func<int, float> filter = delegate(int i)
				{
					float a3 = Mathf.InverseLerp(0f, 8f, (float)i);
					float b2 = Mathf.InverseLerp((float)rail.Path.DefaultMaxIndex, (float)(rail.Path.DefaultMaxIndex - 8), (float)i);
					return Mathf.SmoothStep(0f, 1f, Mathf.Min(a3, b2));
				};
				rail.Path.Smoothen(32, new Vector3(1f, 0f, 1f), filter);
				rail.Path.Smoothen(64, new Vector3(0f, 1f, 0f), filter);
				rail.Path.Resample(7.5f);
				rail.Path.RecalculateTangents();
				rail.AdjustPlacementMap(20f);
			}
		}
		TerrainMeta.Path.Rails.AddRange(list);
	}

	// Token: 0x06002FF7 RID: 12279 RVA: 0x001222C0 File Offset: 0x001204C0
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

	// Token: 0x040026A7 RID: 9895
	public const float Width = 4f;

	// Token: 0x040026A8 RID: 9896
	public const float InnerPadding = 1f;

	// Token: 0x040026A9 RID: 9897
	public const float OuterPadding = 1f;

	// Token: 0x040026AA RID: 9898
	public const float InnerFade = 1f;

	// Token: 0x040026AB RID: 9899
	public const float OuterFade = 32f;

	// Token: 0x040026AC RID: 9900
	public const float RandomScale = 1f;

	// Token: 0x040026AD RID: 9901
	public const float MeshOffset = 0f;

	// Token: 0x040026AE RID: 9902
	public const float TerrainOffset = -0.125f;

	// Token: 0x040026AF RID: 9903
	private static Quaternion rot90 = Quaternion.Euler(0f, 90f, 0f);

	// Token: 0x040026B0 RID: 9904
	private const int MaxDepth = 250000;
}
