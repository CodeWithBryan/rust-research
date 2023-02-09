using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000627 RID: 1575
public class PathList
{
	// Token: 0x06002D42 RID: 11586 RVA: 0x0010FB39 File Offset: 0x0010DD39
	public PathList(string name, Vector3[] points)
	{
		this.Name = name;
		this.Path = new PathInterpolator(points);
	}

	// Token: 0x06002D43 RID: 11587 RVA: 0x0010FB54 File Offset: 0x0010DD54
	private void SpawnObjectsNeighborAligned(ref uint seed, Prefab[] prefabs, List<Vector3> positions, SpawnFilter filter = null)
	{
		if (positions.Count < 2)
		{
			return;
		}
		List<Prefab> list = Pool.GetList<Prefab>();
		for (int i = 0; i < positions.Count; i++)
		{
			int index = Mathf.Max(i - 1, 0);
			int index2 = Mathf.Min(i + 1, positions.Count - 1);
			Vector3 position = positions[i];
			Quaternion rotation = Quaternion.LookRotation((positions[index2] - positions[index]).XZ3D());
			Prefab prefab;
			this.SpawnObject(ref seed, prefabs, position, rotation, list, out prefab, positions.Count, i, filter);
			if (prefab != null)
			{
				list.Add(prefab);
			}
		}
		Pool.FreeList<Prefab>(ref list);
	}

	// Token: 0x06002D44 RID: 11588 RVA: 0x0010FBF4 File Offset: 0x0010DDF4
	private bool SpawnObject(ref uint seed, Prefab[] prefabs, Vector3 position, Quaternion rotation, SpawnFilter filter = null)
	{
		Prefab random = prefabs.GetRandom(ref seed);
		Vector3 position2 = position;
		Quaternion quaternion = rotation;
		Vector3 localScale = random.Object.transform.localScale;
		random.ApplyDecorComponents(ref position2, ref quaternion, ref localScale);
		if (!random.ApplyTerrainAnchors(ref position2, quaternion, localScale, filter))
		{
			return false;
		}
		World.AddPrefab(this.Name, random, position2, quaternion, localScale);
		return true;
	}

	// Token: 0x06002D45 RID: 11589 RVA: 0x0010FC4C File Offset: 0x0010DE4C
	private bool SpawnObject(ref uint seed, Prefab[] prefabs, Vector3 position, Quaternion rotation, List<Prefab> previousSpawns, out Prefab spawned, int pathLength, int index, SpawnFilter filter = null)
	{
		spawned = null;
		Prefab random = prefabs.GetRandom(ref seed);
		random.ApplySequenceReplacement(previousSpawns, ref random, prefabs, pathLength, index);
		Vector3 position2 = position;
		Quaternion quaternion = rotation;
		Vector3 localScale = random.Object.transform.localScale;
		random.ApplyDecorComponents(ref position2, ref quaternion, ref localScale);
		if (!random.ApplyTerrainAnchors(ref position2, quaternion, localScale, filter))
		{
			return false;
		}
		World.AddPrefab(this.Name, random, position2, quaternion, localScale);
		spawned = random;
		return true;
	}

	// Token: 0x06002D46 RID: 11590 RVA: 0x0010FCBC File Offset: 0x0010DEBC
	private bool CheckObjects(Prefab[] prefabs, Vector3 position, Quaternion rotation, SpawnFilter filter = null)
	{
		foreach (Prefab prefab in prefabs)
		{
			Vector3 vector = position;
			Vector3 localScale = prefab.Object.transform.localScale;
			if (!prefab.ApplyTerrainAnchors(ref vector, rotation, localScale, filter))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002D47 RID: 11591 RVA: 0x0010FD00 File Offset: 0x0010DF00
	private void SpawnObject(ref uint seed, Prefab[] prefabs, Vector3 pos, Vector3 dir, PathList.BasicObject obj)
	{
		if (!obj.AlignToNormal)
		{
			dir = dir.XZ3D().normalized;
		}
		SpawnFilter filter = obj.Filter;
		Vector3 a = (this.Width * 0.5f + obj.Offset) * (PathList.rot90 * dir);
		for (int i = 0; i < PathList.placements.Length; i++)
		{
			if ((obj.Placement != PathList.Placement.Center || i == 0) && (obj.Placement != PathList.Placement.Side || i != 0))
			{
				Vector3 vector = pos + PathList.placements[i] * a;
				if (obj.HeightToTerrain)
				{
					vector.y = TerrainMeta.HeightMap.GetHeight(vector);
				}
				if (filter.Test(vector))
				{
					Quaternion rotation = (i == 2) ? Quaternion.LookRotation(PathList.rot180 * dir) : Quaternion.LookRotation(dir);
					if (this.SpawnObject(ref seed, prefabs, vector, rotation, filter))
					{
						break;
					}
				}
			}
		}
	}

	// Token: 0x06002D48 RID: 11592 RVA: 0x0010FDF0 File Offset: 0x0010DFF0
	private bool CheckObjects(Prefab[] prefabs, Vector3 pos, Vector3 dir, PathList.BasicObject obj)
	{
		if (!obj.AlignToNormal)
		{
			dir = dir.XZ3D().normalized;
		}
		SpawnFilter filter = obj.Filter;
		Vector3 a = (this.Width * 0.5f + obj.Offset) * (PathList.rot90 * dir);
		for (int i = 0; i < PathList.placements.Length; i++)
		{
			if ((obj.Placement != PathList.Placement.Center || i == 0) && (obj.Placement != PathList.Placement.Side || i != 0))
			{
				Vector3 vector = pos + PathList.placements[i] * a;
				if (obj.HeightToTerrain)
				{
					vector.y = TerrainMeta.HeightMap.GetHeight(vector);
				}
				if (filter.Test(vector))
				{
					Quaternion rotation = (i == 2) ? Quaternion.LookRotation(PathList.rot180 * dir) : Quaternion.LookRotation(dir);
					if (this.CheckObjects(prefabs, vector, rotation, filter))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06002D49 RID: 11593 RVA: 0x0010FEE0 File Offset: 0x0010E0E0
	public void SpawnSide(ref uint seed, PathList.SideObject obj)
	{
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		PathList.Side side = obj.Side;
		SpawnFilter filter = obj.Filter;
		float density = obj.Density;
		float distance = obj.Distance;
		float num = this.Width * 0.5f + obj.Offset;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		float[] array2 = new float[]
		{
			-num,
			num
		};
		int num2 = 0;
		Vector3 b = this.Path.GetStartPoint();
		List<Vector3> list = new List<Vector3>();
		float num3 = distance * 0.25f;
		float num4 = distance * 0.5f;
		float num5 = this.Path.StartOffset + num4;
		float num6 = this.Path.Length - this.Path.EndOffset - num4;
		for (float num7 = num5; num7 <= num6; num7 += num3)
		{
			Vector3 vector = this.Spline ? this.Path.GetPointCubicHermite(num7) : this.Path.GetPoint(num7);
			if ((vector - b).magnitude >= distance)
			{
				Vector3 tangent = this.Path.GetTangent(num7);
				Vector3 vector2 = PathList.rot90 * tangent;
				for (int i = 0; i < array2.Length; i++)
				{
					int num8 = (num2 + i) % array2.Length;
					if ((side != PathList.Side.Left || num8 == 0) && (side != PathList.Side.Right || num8 == 1))
					{
						float num9 = array2[num8];
						Vector3 vector3 = vector;
						vector3.x += vector2.x * num9;
						vector3.z += vector2.z * num9;
						float normX = TerrainMeta.NormalizeX(vector3.x);
						float normZ = TerrainMeta.NormalizeZ(vector3.z);
						if (filter.GetFactor(normX, normZ, true) >= SeedRandom.Value(ref seed))
						{
							if (density >= SeedRandom.Value(ref seed))
							{
								vector3.y = heightMap.GetHeight(normX, normZ);
								if (obj.Alignment == PathList.Alignment.None)
								{
									if (!this.SpawnObject(ref seed, array, vector3, Quaternion.LookRotation(Vector3.zero), filter))
									{
										goto IL_284;
									}
								}
								else if (obj.Alignment == PathList.Alignment.Forward)
								{
									if (!this.SpawnObject(ref seed, array, vector3, Quaternion.LookRotation(tangent * num9), filter))
									{
										goto IL_284;
									}
								}
								else if (obj.Alignment == PathList.Alignment.Inward)
								{
									if (!this.SpawnObject(ref seed, array, vector3, Quaternion.LookRotation(tangent * num9) * PathList.rot270, filter))
									{
										goto IL_284;
									}
								}
								else
								{
									list.Add(vector3);
								}
							}
							num2 = num8;
							b = vector;
							if (side == PathList.Side.Any)
							{
								break;
							}
						}
					}
					IL_284:;
				}
			}
		}
		if (list.Count > 0)
		{
			this.SpawnObjectsNeighborAligned(ref seed, array, list, filter);
		}
	}

	// Token: 0x06002D4A RID: 11594 RVA: 0x001101A8 File Offset: 0x0010E3A8
	public void SpawnAlong(ref uint seed, PathList.PathObject obj)
	{
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		SpawnFilter filter = obj.Filter;
		float density = obj.Density;
		float distance = obj.Distance;
		float dithering = obj.Dithering;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Vector3 b = this.Path.GetStartPoint();
		List<Vector3> list = new List<Vector3>();
		float num = distance * 0.25f;
		float num2 = distance * 0.5f;
		float num3 = this.Path.StartOffset + num2;
		float num4 = this.Path.Length - this.Path.EndOffset - num2;
		for (float num5 = num3; num5 <= num4; num5 += num)
		{
			Vector3 vector = this.Spline ? this.Path.GetPointCubicHermite(num5) : this.Path.GetPoint(num5);
			if ((vector - b).magnitude >= distance)
			{
				Vector3 tangent = this.Path.GetTangent(num5);
				Vector3 forward = PathList.rot90 * tangent;
				Vector3 vector2 = vector;
				vector2.x += SeedRandom.Range(ref seed, -dithering, dithering);
				vector2.z += SeedRandom.Range(ref seed, -dithering, dithering);
				float normX = TerrainMeta.NormalizeX(vector2.x);
				float normZ = TerrainMeta.NormalizeZ(vector2.z);
				if (filter.GetFactor(normX, normZ, true) >= SeedRandom.Value(ref seed))
				{
					if (density >= SeedRandom.Value(ref seed))
					{
						vector2.y = heightMap.GetHeight(normX, normZ);
						if (obj.Alignment == PathList.Alignment.None)
						{
							if (!this.SpawnObject(ref seed, array, vector2, Quaternion.identity, filter))
							{
								goto IL_1FE;
							}
						}
						else if (obj.Alignment == PathList.Alignment.Forward)
						{
							if (!this.SpawnObject(ref seed, array, vector2, Quaternion.LookRotation(tangent), filter))
							{
								goto IL_1FE;
							}
						}
						else if (obj.Alignment == PathList.Alignment.Inward)
						{
							if (!this.SpawnObject(ref seed, array, vector2, Quaternion.LookRotation(forward), filter))
							{
								goto IL_1FE;
							}
						}
						else
						{
							list.Add(vector2);
						}
					}
					b = vector;
				}
			}
			IL_1FE:;
		}
		if (list.Count > 0)
		{
			this.SpawnObjectsNeighborAligned(ref seed, array, list, filter);
		}
	}

	// Token: 0x06002D4B RID: 11595 RVA: 0x001103D8 File Offset: 0x0010E5D8
	public void SpawnBridge(ref uint seed, PathList.BridgeObject obj)
	{
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		Vector3 startPoint = this.Path.GetStartPoint();
		Vector3 a = this.Path.GetEndPoint() - startPoint;
		float magnitude = a.magnitude;
		Vector3 vector = a / magnitude;
		float num = magnitude / obj.Distance;
		int num2 = Mathf.RoundToInt(num);
		float num3 = 0.5f * (num - (float)num2);
		Vector3 vector2 = obj.Distance * vector;
		Vector3 vector3 = startPoint + (0.5f + num3) * vector2;
		Quaternion rotation = Quaternion.LookRotation(vector);
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainWaterMap waterMap = TerrainMeta.WaterMap;
		for (int i = 0; i < num2; i++)
		{
			float num4 = Mathf.Max(heightMap.GetHeight(vector3), waterMap.GetHeight(vector3)) - 1f;
			if (vector3.y > num4)
			{
				this.SpawnObject(ref seed, array, vector3, rotation, null);
			}
			vector3 += vector2;
		}
	}

	// Token: 0x06002D4C RID: 11596 RVA: 0x0011050C File Offset: 0x0010E70C
	public void SpawnStart(ref uint seed, PathList.BasicObject obj)
	{
		if (!this.Start)
		{
			return;
		}
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		Vector3 startPoint = this.Path.GetStartPoint();
		Vector3 startTangent = this.Path.GetStartTangent();
		this.SpawnObject(ref seed, array, startPoint, startTangent, obj);
	}

	// Token: 0x06002D4D RID: 11597 RVA: 0x0011058C File Offset: 0x0010E78C
	public void SpawnEnd(ref uint seed, PathList.BasicObject obj)
	{
		if (!this.End)
		{
			return;
		}
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		Vector3 endPoint = this.Path.GetEndPoint();
		Vector3 dir = -this.Path.GetEndTangent();
		this.SpawnObject(ref seed, array, endPoint, dir, obj);
	}

	// Token: 0x06002D4E RID: 11598 RVA: 0x00110610 File Offset: 0x0010E810
	public void TrimStart(PathList.BasicObject obj)
	{
		if (!this.Start)
		{
			return;
		}
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		Vector3[] points = this.Path.Points;
		Vector3[] tangents = this.Path.Tangents;
		int num = points.Length / 4;
		for (int i = 0; i < num; i++)
		{
			Vector3 pos = points[this.Path.MinIndex + i];
			Vector3 dir = tangents[this.Path.MinIndex + i];
			if (this.CheckObjects(array, pos, dir, obj))
			{
				this.Path.MinIndex += i;
				return;
			}
		}
	}

	// Token: 0x06002D4F RID: 11599 RVA: 0x001106E8 File Offset: 0x0010E8E8
	public void TrimEnd(PathList.BasicObject obj)
	{
		if (!this.End)
		{
			return;
		}
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder, null, null, true);
		if (array == null || array.Length == 0)
		{
			Debug.LogError("Empty decor folder: " + obj.Folder);
			return;
		}
		Vector3[] points = this.Path.Points;
		Vector3[] tangents = this.Path.Tangents;
		int num = points.Length / 4;
		for (int i = 0; i < num; i++)
		{
			Vector3 pos = points[this.Path.MaxIndex - i];
			Vector3 dir = -tangents[this.Path.MaxIndex - i];
			if (this.CheckObjects(array, pos, dir, obj))
			{
				this.Path.MaxIndex -= i;
				return;
			}
		}
	}

	// Token: 0x06002D50 RID: 11600 RVA: 0x001107C4 File Offset: 0x0010E9C4
	public void TrimTopology(int topology)
	{
		Vector3[] points = this.Path.Points;
		int num = points.Length / 4;
		for (int i = 0; i < num; i++)
		{
			Vector3 worldPos = points[this.Path.MinIndex + i];
			if (!TerrainMeta.TopologyMap.GetTopology(worldPos, topology))
			{
				this.Path.MinIndex += i;
				break;
			}
		}
		for (int j = 0; j < num; j++)
		{
			Vector3 worldPos2 = points[this.Path.MaxIndex - j];
			if (!TerrainMeta.TopologyMap.GetTopology(worldPos2, topology))
			{
				this.Path.MaxIndex -= j;
				return;
			}
		}
	}

	// Token: 0x06002D51 RID: 11601 RVA: 0x00110870 File Offset: 0x0010EA70
	public void ResetTrims()
	{
		this.Path.MinIndex = this.Path.DefaultMinIndex;
		this.Path.MaxIndex = this.Path.DefaultMaxIndex;
	}

	// Token: 0x06002D52 RID: 11602 RVA: 0x001108A0 File Offset: 0x0010EAA0
	public void AdjustTerrainHeight(float intensity = 1f, float fade = 1f)
	{
		TerrainHeightMap heightmap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		float num = 1f;
		float randomScale = this.RandomScale;
		float outerPadding = this.OuterPadding;
		float innerPadding = this.InnerPadding;
		float outerFade = this.OuterFade * fade;
		float innerFade = this.InnerFade;
		float offset01 = this.TerrainOffset * TerrainMeta.OneOverSize.y;
		float num2 = this.Width * 0.5f;
		Vector3 startPoint = this.Path.GetStartPoint();
		Vector3 endPoint = this.Path.GetEndPoint();
		Vector3 startTangent = this.Path.GetStartTangent();
		Vector3 normalized = startTangent.XZ3D().normalized;
		Vector3 a = PathList.rot90 * normalized;
		Vector3 vector = startPoint;
		Vector3 vector2 = startTangent;
		Line prev_line = new Line(startPoint, startPoint + startTangent * num);
		Vector3 v = startPoint - a * (num2 + outerPadding + outerFade);
		Vector3 v2 = startPoint + a * (num2 + outerPadding + outerFade);
		Vector3 vector3 = vector;
		Vector3 v3 = vector2;
		Line cur_line = prev_line;
		float num3 = this.Path.Length + num;
		for (float num4 = 0f; num4 < num3; num4 += num)
		{
			Vector3 vector4 = this.Spline ? this.Path.GetPointCubicHermite(num4 + num) : this.Path.GetPoint(num4 + num);
			Vector3 tangent = this.Path.GetTangent(num4 + num);
			Line next_line = new Line(vector4, vector4 + tangent * num);
			float opacity = 1f;
			float radius = Mathf.Lerp(num2, num2 * randomScale, Noise.Billow(vector3.x, vector3.z, 2, 0.005f, 1f, 2f, 0.5f));
			if (!this.Path.Circular)
			{
				float a2 = (startPoint - vector3).Magnitude2D();
				float b = (endPoint - vector3).Magnitude2D();
				opacity = Mathf.InverseLerp(0f, num2, Mathf.Min(a2, b));
			}
			normalized = v3.XZ3D().normalized;
			a = PathList.rot90 * normalized;
			Vector3 vector5 = vector3 - a * (radius + outerPadding + outerFade);
			Vector3 vector6 = vector3 + a * (radius + outerPadding + outerFade);
			float yn = TerrainMeta.NormalizeY((vector3.y + vector.y) * 0.5f);
			heightmap.ForEach(v, v2, vector5, vector6, delegate(int x, int z)
			{
				float x2 = heightmap.Coordinate(x);
				float z2 = heightmap.Coordinate(z);
				Vector3 vector7 = TerrainMeta.Denormalize(new Vector3(x2, yn, z2));
				Vector3 vector8 = prev_line.ClosestPoint2D(vector7);
				Vector3 vector9 = cur_line.ClosestPoint2D(vector7);
				Vector3 vector10 = next_line.ClosestPoint2D(vector7);
				float num5 = (vector7 - vector8).Magnitude2D();
				float num6 = (vector7 - vector9).Magnitude2D();
				float num7 = (vector7 - vector10).Magnitude2D();
				float value = num6;
				Vector3 vector11 = vector9;
				if (num6 > num5 || num6 > num7)
				{
					if (num5 <= num7)
					{
						value = num5;
						vector11 = vector8;
					}
					else
					{
						value = num7;
						vector11 = vector10;
					}
				}
				float num8 = Mathf.InverseLerp(radius + outerPadding + outerFade, radius + outerPadding, value);
				float t = Mathf.InverseLerp(radius - innerPadding, radius - innerPadding - innerFade, value);
				float num9 = TerrainMeta.NormalizeY(vector11.y);
				heightmap.SetHeight(x, z, num9 + Mathf.SmoothStep(0f, offset01, t), intensity * opacity * num8);
			});
			vector = vector3;
			v = vector5;
			v2 = vector6;
			prev_line = cur_line;
			vector3 = vector4;
			v3 = tangent;
			cur_line = next_line;
		}
	}

	// Token: 0x06002D53 RID: 11603 RVA: 0x00110C14 File Offset: 0x0010EE14
	public void AdjustTerrainTexture()
	{
		if (this.Splat == 0)
		{
			return;
		}
		TerrainSplatMap splatmap = TerrainMeta.SplatMap;
		float num = 1f;
		float randomScale = this.RandomScale;
		float outerPadding = this.OuterPadding;
		float innerPadding = this.InnerPadding;
		float num2 = this.Width * 0.5f;
		Vector3 startPoint = this.Path.GetStartPoint();
		Vector3 endPoint = this.Path.GetEndPoint();
		Vector3 vector = this.Path.GetStartTangent();
		Vector3 normalized = vector.XZ3D().normalized;
		Vector3 a = PathList.rot90 * normalized;
		Vector3 v = startPoint - a * (num2 + outerPadding);
		Vector3 v2 = startPoint + a * (num2 + outerPadding);
		float num3 = this.Path.Length + num;
		for (float num4 = 0f; num4 < num3; num4 += num)
		{
			Vector3 vector2 = this.Spline ? this.Path.GetPointCubicHermite(num4) : this.Path.GetPoint(num4);
			float opacity = 1f;
			float radius = Mathf.Lerp(num2, num2 * randomScale, Noise.Billow(vector2.x, vector2.z, 2, 0.005f, 1f, 2f, 0.5f));
			if (!this.Path.Circular)
			{
				float a2 = (startPoint - vector2).Magnitude2D();
				float b = (endPoint - vector2).Magnitude2D();
				opacity = Mathf.InverseLerp(0f, num2, Mathf.Min(a2, b));
			}
			vector = this.Path.GetTangent(num4);
			normalized = vector.XZ3D().normalized;
			a = PathList.rot90 * normalized;
			Ray ray = new Ray(vector2, vector);
			Vector3 vector3 = vector2 - a * (radius + outerPadding);
			Vector3 vector4 = vector2 + a * (radius + outerPadding);
			float yn = TerrainMeta.NormalizeY(vector2.y);
			splatmap.ForEach(v, v2, vector3, vector4, delegate(int x, int z)
			{
				float x2 = splatmap.Coordinate(x);
				float z2 = splatmap.Coordinate(z);
				Vector3 vector5 = TerrainMeta.Denormalize(new Vector3(x2, yn, z2));
				Vector3 b2 = ray.ClosestPoint(vector5);
				float value = (vector5 - b2).Magnitude2D();
				float num5 = Mathf.InverseLerp(radius + outerPadding, radius - innerPadding, value);
				splatmap.SetSplat(x, z, this.Splat, num5 * opacity);
			});
			v = vector3;
			v2 = vector4;
		}
	}

	// Token: 0x06002D54 RID: 11604 RVA: 0x00110EA0 File Offset: 0x0010F0A0
	public void AdjustTerrainTopology()
	{
		if (this.Topology == 0)
		{
			return;
		}
		TerrainTopologyMap topomap = TerrainMeta.TopologyMap;
		float num = 1f;
		float randomScale = this.RandomScale;
		float outerPadding = this.OuterPadding;
		float innerPadding = this.InnerPadding;
		float num2 = this.Width * 0.5f;
		Vector3 startPoint = this.Path.GetStartPoint();
		Vector3 endPoint = this.Path.GetEndPoint();
		Vector3 vector = this.Path.GetStartTangent();
		Vector3 normalized = vector.XZ3D().normalized;
		Vector3 a = PathList.rot90 * normalized;
		Vector3 v = startPoint - a * (num2 + outerPadding);
		Vector3 v2 = startPoint + a * (num2 + outerPadding);
		float num3 = this.Path.Length + num;
		for (float num4 = 0f; num4 < num3; num4 += num)
		{
			Vector3 vector2 = this.Spline ? this.Path.GetPointCubicHermite(num4) : this.Path.GetPoint(num4);
			float opacity = 1f;
			float radius = Mathf.Lerp(num2, num2 * randomScale, Noise.Billow(vector2.x, vector2.z, 2, 0.005f, 1f, 2f, 0.5f));
			if (!this.Path.Circular)
			{
				float a2 = (startPoint - vector2).Magnitude2D();
				float b = (endPoint - vector2).Magnitude2D();
				opacity = Mathf.InverseLerp(0f, num2, Mathf.Min(a2, b));
			}
			vector = this.Path.GetTangent(num4);
			normalized = vector.XZ3D().normalized;
			a = PathList.rot90 * normalized;
			Ray ray = new Ray(vector2, vector);
			Vector3 vector3 = vector2 - a * (radius + outerPadding);
			Vector3 vector4 = vector2 + a * (radius + outerPadding);
			float yn = TerrainMeta.NormalizeY(vector2.y);
			topomap.ForEach(v, v2, vector3, vector4, delegate(int x, int z)
			{
				float x2 = topomap.Coordinate(x);
				float z2 = topomap.Coordinate(z);
				Vector3 vector5 = TerrainMeta.Denormalize(new Vector3(x2, yn, z2));
				Vector3 b2 = ray.ClosestPoint(vector5);
				float value = (vector5 - b2).Magnitude2D();
				if (Mathf.InverseLerp(radius + outerPadding, radius - innerPadding, value) * opacity > 0.3f)
				{
					topomap.AddTopology(x, z, this.Topology);
				}
			});
			v = vector3;
			v2 = vector4;
		}
	}

	// Token: 0x06002D55 RID: 11605 RVA: 0x0011112C File Offset: 0x0010F32C
	public void AdjustPlacementMap(float width)
	{
		TerrainPlacementMap placementmap = TerrainMeta.PlacementMap;
		float num = 1f;
		float radius = width * 0.5f;
		Vector3 startPoint = this.Path.GetStartPoint();
		this.Path.GetEndPoint();
		Vector3 vector = this.Path.GetStartTangent();
		Vector3 normalized = vector.XZ3D().normalized;
		Vector3 a = PathList.rot90 * normalized;
		Vector3 v = startPoint - a * radius;
		Vector3 v2 = startPoint + a * radius;
		float num2 = this.Path.Length + num;
		for (float num3 = 0f; num3 < num2; num3 += num)
		{
			Vector3 vector2 = this.Spline ? this.Path.GetPointCubicHermite(num3) : this.Path.GetPoint(num3);
			vector = this.Path.GetTangent(num3);
			normalized = vector.XZ3D().normalized;
			a = PathList.rot90 * normalized;
			Ray ray = new Ray(vector2, vector);
			Vector3 vector3 = vector2 - a * radius;
			Vector3 vector4 = vector2 + a * radius;
			float yn = TerrainMeta.NormalizeY(vector2.y);
			placementmap.ForEach(v, v2, vector3, vector4, delegate(int x, int z)
			{
				float x2 = placementmap.Coordinate(x);
				float z2 = placementmap.Coordinate(z);
				Vector3 vector5 = TerrainMeta.Denormalize(new Vector3(x2, yn, z2));
				Vector3 b = ray.ClosestPoint(vector5);
				if ((vector5 - b).Magnitude2D() <= radius)
				{
					placementmap.SetBlocked(x, z);
				}
			});
			v = vector3;
			v2 = vector4;
		}
	}

	// Token: 0x06002D56 RID: 11606 RVA: 0x001112D8 File Offset: 0x0010F4D8
	public List<PathList.MeshObject> CreateMesh(Mesh[] meshes, float normalSmoothing, bool snapToTerrain, bool snapStartToTerrain, bool snapEndToTerrain)
	{
		MeshCache.Data[] array = new MeshCache.Data[meshes.Length];
		MeshData[] array2 = new MeshData[meshes.Length];
		for (int i = 0; i < meshes.Length; i++)
		{
			array[i] = MeshCache.Get(meshes[i]);
			array2[i] = new MeshData();
		}
		MeshData[] array3 = array2;
		for (int j = 0; j < array3.Length; j++)
		{
			array3[j].AllocMinimal();
		}
		Bounds bounds = meshes[meshes.Length - 1].bounds;
		Vector3 min = bounds.min;
		Vector3 size = bounds.size;
		float num = this.Width / bounds.size.x;
		List<PathList.MeshObject> list = new List<PathList.MeshObject>();
		int num2 = (int)(this.Path.Length / (num * bounds.size.z));
		int num3 = 5;
		float num4 = this.Path.Length / (float)num2;
		float randomScale = this.RandomScale;
		float meshOffset = this.MeshOffset;
		float num5 = this.Width * 0.5f;
		int num6 = array[0].vertices.Length;
		int num7 = array[0].triangles.Length;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		for (int k = 0; k < num2; k += num3)
		{
			float distance = (float)k * num4 + 0.5f * (float)num3 * num4;
			Vector3 vector = this.Spline ? this.Path.GetPointCubicHermite(distance) : this.Path.GetPoint(distance);
			int num8 = 0;
			while (num8 < num3 && k + num8 < num2)
			{
				float num9 = (float)(k + num8) * num4;
				for (int l = 0; l < meshes.Length; l++)
				{
					MeshCache.Data data = array[l];
					MeshData meshData = array2[l];
					int count = meshData.vertices.Count;
					for (int m = 0; m < data.vertices.Length; m++)
					{
						Vector2 item = data.uv[m];
						Vector3 vector2 = data.vertices[m];
						Vector3 vector3 = data.normals[m];
						Vector4 vector4 = data.tangents[m];
						float t = (vector2.x - min.x) / size.x;
						float num10 = vector2.y - min.y;
						float num11 = (vector2.z - min.z) / size.z;
						float num12 = num9 + num11 * num4;
						Vector3 vector5 = this.Spline ? this.Path.GetPointCubicHermite(num12) : this.Path.GetPoint(num12);
						Vector3 tangent = this.Path.GetTangent(num12);
						Vector3 normalized = tangent.XZ3D().normalized;
						Vector3 vector6 = PathList.rot90 * normalized;
						Vector3 vector7 = Vector3.Cross(tangent, vector6);
						Quaternion rotation = Quaternion.LookRotation(normalized, vector7);
						float d = Mathf.Lerp(num5, num5 * randomScale, Noise.Billow(vector5.x, vector5.z, 2, 0.005f, 1f, 2f, 0.5f));
						Vector3 vector8 = vector5 - vector6 * d;
						Vector3 vector9 = vector5 + vector6 * d;
						if (snapToTerrain)
						{
							vector8.y = heightMap.GetHeight(vector8);
							vector9.y = heightMap.GetHeight(vector9);
						}
						vector8 += vector7 * meshOffset;
						vector9 += vector7 * meshOffset;
						vector2 = Vector3.Lerp(vector8, vector9, t);
						if ((snapStartToTerrain && num12 < 0.1f) || (snapEndToTerrain && num12 > this.Path.Length - 0.1f))
						{
							vector2.y = heightMap.GetHeight(vector2);
						}
						else
						{
							vector2.y += num10;
						}
						vector2 -= vector;
						vector3 = rotation * vector3;
						vector4 = rotation * vector4;
						if (normalSmoothing > 0f)
						{
							vector3 = Vector3.Slerp(vector3, Vector3.up, normalSmoothing);
						}
						meshData.vertices.Add(vector2);
						meshData.normals.Add(vector3);
						meshData.tangents.Add(vector4);
						meshData.uv.Add(item);
					}
					for (int n = 0; n < data.triangles.Length; n++)
					{
						int num13 = data.triangles[n];
						meshData.triangles.Add(count + num13);
					}
				}
				num8++;
			}
			list.Add(new PathList.MeshObject(vector, array2));
			array3 = array2;
			for (int j = 0; j < array3.Length; j++)
			{
				array3[j].Clear();
			}
		}
		array3 = array2;
		for (int j = 0; j < array3.Length; j++)
		{
			array3[j].Free();
		}
		return list;
	}

	// Token: 0x0400250A RID: 9482
	private static Quaternion rot90 = Quaternion.Euler(0f, 90f, 0f);

	// Token: 0x0400250B RID: 9483
	private static Quaternion rot180 = Quaternion.Euler(0f, 180f, 0f);

	// Token: 0x0400250C RID: 9484
	private static Quaternion rot270 = Quaternion.Euler(0f, 270f, 0f);

	// Token: 0x0400250D RID: 9485
	public string Name;

	// Token: 0x0400250E RID: 9486
	public PathInterpolator Path;

	// Token: 0x0400250F RID: 9487
	public bool Spline;

	// Token: 0x04002510 RID: 9488
	public bool Start;

	// Token: 0x04002511 RID: 9489
	public bool End;

	// Token: 0x04002512 RID: 9490
	public float Width;

	// Token: 0x04002513 RID: 9491
	public float InnerPadding;

	// Token: 0x04002514 RID: 9492
	public float OuterPadding;

	// Token: 0x04002515 RID: 9493
	public float InnerFade;

	// Token: 0x04002516 RID: 9494
	public float OuterFade;

	// Token: 0x04002517 RID: 9495
	public float RandomScale;

	// Token: 0x04002518 RID: 9496
	public float MeshOffset;

	// Token: 0x04002519 RID: 9497
	public float TerrainOffset;

	// Token: 0x0400251A RID: 9498
	public int Topology;

	// Token: 0x0400251B RID: 9499
	public int Splat;

	// Token: 0x0400251C RID: 9500
	public int Hierarchy;

	// Token: 0x0400251D RID: 9501
	public PathFinder.Node ProcgenStartNode;

	// Token: 0x0400251E RID: 9502
	public PathFinder.Node ProcgenEndNode;

	// Token: 0x0400251F RID: 9503
	public const float StepSize = 1f;

	// Token: 0x04002520 RID: 9504
	private static float[] placements = new float[]
	{
		0f,
		-1f,
		1f
	};

	// Token: 0x02000D45 RID: 3397
	public enum Side
	{
		// Token: 0x040045C7 RID: 17863
		Both,
		// Token: 0x040045C8 RID: 17864
		Left,
		// Token: 0x040045C9 RID: 17865
		Right,
		// Token: 0x040045CA RID: 17866
		Any
	}

	// Token: 0x02000D46 RID: 3398
	public enum Placement
	{
		// Token: 0x040045CC RID: 17868
		Center,
		// Token: 0x040045CD RID: 17869
		Side
	}

	// Token: 0x02000D47 RID: 3399
	public enum Alignment
	{
		// Token: 0x040045CF RID: 17871
		None,
		// Token: 0x040045D0 RID: 17872
		Neighbor,
		// Token: 0x040045D1 RID: 17873
		Forward,
		// Token: 0x040045D2 RID: 17874
		Inward
	}

	// Token: 0x02000D48 RID: 3400
	[Serializable]
	public class BasicObject
	{
		// Token: 0x040045D3 RID: 17875
		public string Folder;

		// Token: 0x040045D4 RID: 17876
		public SpawnFilter Filter;

		// Token: 0x040045D5 RID: 17877
		public PathList.Placement Placement;

		// Token: 0x040045D6 RID: 17878
		public bool AlignToNormal = true;

		// Token: 0x040045D7 RID: 17879
		public bool HeightToTerrain = true;

		// Token: 0x040045D8 RID: 17880
		public float Offset;
	}

	// Token: 0x02000D49 RID: 3401
	[Serializable]
	public class SideObject
	{
		// Token: 0x040045D9 RID: 17881
		public string Folder;

		// Token: 0x040045DA RID: 17882
		public SpawnFilter Filter;

		// Token: 0x040045DB RID: 17883
		public PathList.Side Side;

		// Token: 0x040045DC RID: 17884
		public PathList.Alignment Alignment;

		// Token: 0x040045DD RID: 17885
		public float Density = 1f;

		// Token: 0x040045DE RID: 17886
		public float Distance = 25f;

		// Token: 0x040045DF RID: 17887
		public float Offset = 2f;
	}

	// Token: 0x02000D4A RID: 3402
	[Serializable]
	public class PathObject
	{
		// Token: 0x040045E0 RID: 17888
		public string Folder;

		// Token: 0x040045E1 RID: 17889
		public SpawnFilter Filter;

		// Token: 0x040045E2 RID: 17890
		public PathList.Alignment Alignment;

		// Token: 0x040045E3 RID: 17891
		public float Density = 1f;

		// Token: 0x040045E4 RID: 17892
		public float Distance = 5f;

		// Token: 0x040045E5 RID: 17893
		public float Dithering = 5f;
	}

	// Token: 0x02000D4B RID: 3403
	[Serializable]
	public class BridgeObject
	{
		// Token: 0x040045E6 RID: 17894
		public string Folder;

		// Token: 0x040045E7 RID: 17895
		public float Distance = 10f;
	}

	// Token: 0x02000D4C RID: 3404
	public class MeshObject
	{
		// Token: 0x06004E6C RID: 20076 RVA: 0x0019B6A4 File Offset: 0x001998A4
		public MeshObject(Vector3 meshPivot, MeshData[] meshData)
		{
			this.Position = meshPivot;
			this.Meshes = new Mesh[meshData.Length];
			for (int i = 0; i < this.Meshes.Length; i++)
			{
				MeshData meshData2 = meshData[i];
				Mesh mesh = this.Meshes[i] = new Mesh();
				meshData2.Apply(mesh);
			}
		}

		// Token: 0x040045E8 RID: 17896
		public Vector3 Position;

		// Token: 0x040045E9 RID: 17897
		public Mesh[] Meshes;
	}
}
