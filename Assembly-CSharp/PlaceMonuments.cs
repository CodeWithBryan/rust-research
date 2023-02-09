using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020006AF RID: 1711
public class PlaceMonuments : ProceduralComponent
{
	// Token: 0x06003048 RID: 12360 RVA: 0x00127508 File Offset: 0x00125708
	public override void Process(uint seed)
	{
		string[] array = (from folder in this.ResourceFolder.Split(new char[]
		{
			','
		})
		select "assets/bundled/prefabs/autospawn/" + folder + "/").ToArray<string>();
		if (World.Networked)
		{
			World.Spawn("Monument", array);
			return;
		}
		if ((ulong)World.Size < (ulong)((long)this.MinWorldSize))
		{
			return;
		}
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		PathFinder pathFinder = null;
		List<PathFinder.Point> endList = null;
		List<Prefab<MonumentInfo>> list = new List<Prefab<MonumentInfo>>();
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Prefab<MonumentInfo>[] array3 = Prefab.Load<MonumentInfo>(array2[i], null, null, true);
			array3.Shuffle(ref seed);
			list.AddRange(array3);
		}
		Prefab<MonumentInfo>[] array4 = list.ToArray();
		if (array4 == null || array4.Length == 0)
		{
			return;
		}
		array4.BubbleSort<Prefab<MonumentInfo>>();
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float x = position.x;
		float z = position.z;
		float max = position.x + size.x;
		float max2 = position.z + size.z;
		List<PlaceMonuments.SpawnInfo> list2 = new List<PlaceMonuments.SpawnInfo>();
		int num = 0;
		List<PlaceMonuments.SpawnInfo> list3 = new List<PlaceMonuments.SpawnInfo>();
		for (int j = 0; j < 8; j++)
		{
			int num2 = 0;
			list2.Clear();
			foreach (Prefab<MonumentInfo> prefab in array4)
			{
				MonumentInfo component = prefab.Component;
				if (!(component == null) && (ulong)World.Size >= (ulong)((long)component.MinWorldSize))
				{
					DungeonGridInfo dungeonEntrance = component.DungeonEntrance;
					int num3 = (int)(prefab.Parameters ? (prefab.Parameters.Priority + 1) : PrefabPriority.Low);
					int num4 = 100000 * num3 * num3 * num3 * num3;
					int num5 = 0;
					int num6 = 0;
					PlaceMonuments.SpawnInfo item = default(PlaceMonuments.SpawnInfo);
					for (int k = 0; k < 10000; k++)
					{
						float x2 = SeedRandom.Range(ref seed, x, max);
						float z2 = SeedRandom.Range(ref seed, z, max2);
						float normX = TerrainMeta.NormalizeX(x2);
						float normZ = TerrainMeta.NormalizeZ(z2);
						float num7 = SeedRandom.Value(ref seed);
						float factor = this.Filter.GetFactor(normX, normZ, true);
						if (factor * factor >= num7)
						{
							float height = heightMap.GetHeight(normX, normZ);
							Vector3 vector = new Vector3(x2, height, z2);
							Quaternion localRotation = prefab.Object.transform.localRotation;
							Vector3 localScale = prefab.Object.transform.localScale;
							Vector3 vector2 = vector;
							prefab.ApplyDecorComponents(ref vector, ref localRotation, ref localScale);
							PlaceMonuments.DistanceInfo distanceInfo = this.GetDistanceInfo(list2, prefab, vector, localRotation, localScale, vector2);
							if (distanceInfo.minDistanceSameType >= (float)this.MinDistanceSameType && distanceInfo.minDistanceDifferentType >= (float)this.MinDistanceDifferentType && (!dungeonEntrance || distanceInfo.minDistanceDungeonEntrance >= (float)dungeonEntrance.CellSize))
							{
								int num8 = num4;
								if (distanceInfo.minDistanceSameType != 3.4028235E+38f)
								{
									if (this.DistanceSameType == PlaceMonuments.DistanceMode.Min)
									{
										num8 -= Mathf.RoundToInt(distanceInfo.minDistanceSameType * distanceInfo.minDistanceSameType * 2f);
									}
									else if (this.DistanceSameType == PlaceMonuments.DistanceMode.Max)
									{
										num8 += Mathf.RoundToInt(distanceInfo.minDistanceSameType * distanceInfo.minDistanceSameType * 2f);
									}
								}
								if (distanceInfo.minDistanceDifferentType != 3.4028235E+38f)
								{
									if (this.DistanceDifferentType == PlaceMonuments.DistanceMode.Min)
									{
										num8 -= Mathf.RoundToInt(distanceInfo.minDistanceDifferentType * distanceInfo.minDistanceDifferentType);
									}
									else if (this.DistanceDifferentType == PlaceMonuments.DistanceMode.Max)
									{
										num8 += Mathf.RoundToInt(distanceInfo.minDistanceDifferentType * distanceInfo.minDistanceDifferentType);
									}
								}
								if (num8 > num6 && prefab.ApplyTerrainAnchors(ref vector, localRotation, localScale, this.Filter) && component.CheckPlacement(vector, localRotation, localScale))
								{
									if (dungeonEntrance)
									{
										Vector3 vector3 = vector + localRotation * Vector3.Scale(localScale, dungeonEntrance.transform.position);
										Vector3 vector4 = dungeonEntrance.SnapPosition(vector3);
										vector += vector4 - vector3;
										if (!dungeonEntrance.IsValidSpawnPosition(vector4))
										{
											goto IL_5E3;
										}
										vector2 = vector4;
									}
									if (prefab.ApplyTerrainChecks(vector, localRotation, localScale, this.Filter) && prefab.ApplyTerrainFilters(vector, localRotation, localScale, null) && prefab.ApplyWaterChecks(vector, localRotation, localScale) && !prefab.CheckEnvironmentVolumes(vector, localRotation, localScale, EnvironmentType.Underground | EnvironmentType.TrainTunnels))
									{
										bool flag = false;
										foreach (TerrainPathConnect terrainPathConnect in prefab.Object.GetComponentsInChildren<TerrainPathConnect>(true))
										{
											if (terrainPathConnect.Type == InfrastructureType.Boat)
											{
												if (pathFinder == null)
												{
													int[,] array6 = TerrainPath.CreateBoatCostmap(2f);
													int length = array6.GetLength(0);
													pathFinder = new PathFinder(array6, true, true);
													endList = new List<PathFinder.Point>
													{
														new PathFinder.Point(0, 0),
														new PathFinder.Point(0, length / 2),
														new PathFinder.Point(0, length - 1),
														new PathFinder.Point(length / 2, 0),
														new PathFinder.Point(length / 2, length - 1),
														new PathFinder.Point(length - 1, 0),
														new PathFinder.Point(length - 1, length / 2),
														new PathFinder.Point(length - 1, length - 1)
													};
												}
												PathFinder.Point pathFinderPoint = terrainPathConnect.GetPathFinderPoint(pathFinder.GetResolution(0), vector + localRotation * Vector3.Scale(localScale, terrainPathConnect.transform.localPosition));
												if (pathFinder.FindPathUndirected(new List<PathFinder.Point>
												{
													pathFinderPoint
												}, endList, 100000) == null)
												{
													flag = true;
													break;
												}
											}
										}
										if (!flag)
										{
											PlaceMonuments.SpawnInfo spawnInfo = default(PlaceMonuments.SpawnInfo);
											spawnInfo.prefab = prefab;
											spawnInfo.position = vector;
											spawnInfo.rotation = localRotation;
											spawnInfo.scale = localScale;
											if (dungeonEntrance)
											{
												spawnInfo.dungeonEntrance = true;
												spawnInfo.dungeonEntrancePos = vector2;
											}
											num6 = num8;
											item = spawnInfo;
											num5++;
											if (num5 >= 8 || this.DistanceDifferentType == PlaceMonuments.DistanceMode.Any)
											{
												break;
											}
										}
									}
								}
							}
						}
						IL_5E3:;
					}
					if (num6 > 0)
					{
						list2.Add(item);
						num2 += num6;
					}
					if (this.TargetCount > 0 && list2.Count >= this.TargetCount)
					{
						break;
					}
				}
			}
			if (num2 > num)
			{
				num = num2;
				GenericsUtil.Swap<List<PlaceMonuments.SpawnInfo>>(ref list2, ref list3);
			}
		}
		foreach (PlaceMonuments.SpawnInfo spawnInfo2 in list3)
		{
			World.AddPrefab("Monument", spawnInfo2.prefab, spawnInfo2.position, spawnInfo2.rotation, spawnInfo2.scale);
		}
	}

	// Token: 0x06003049 RID: 12361 RVA: 0x00127BCC File Offset: 0x00125DCC
	private PlaceMonuments.DistanceInfo GetDistanceInfo(List<PlaceMonuments.SpawnInfo> spawns, Prefab<MonumentInfo> prefab, Vector3 monumentPos, Quaternion monumentRot, Vector3 monumentScale, Vector3 dungeonPos)
	{
		PlaceMonuments.DistanceInfo distanceInfo = default(PlaceMonuments.DistanceInfo);
		distanceInfo.minDistanceSameType = float.MaxValue;
		distanceInfo.maxDistanceSameType = float.MinValue;
		distanceInfo.minDistanceDifferentType = float.MaxValue;
		distanceInfo.maxDistanceDifferentType = float.MinValue;
		distanceInfo.minDistanceDungeonEntrance = float.MaxValue;
		distanceInfo.maxDistanceDungeonEntrance = float.MinValue;
		OBB obb = new OBB(monumentPos, monumentScale, monumentRot, prefab.Component.Bounds);
		if (spawns != null)
		{
			foreach (PlaceMonuments.SpawnInfo spawnInfo in spawns)
			{
				float num = new OBB(spawnInfo.position, spawnInfo.scale, spawnInfo.rotation, spawnInfo.prefab.Component.Bounds).SqrDistance(obb);
				if (spawnInfo.prefab.Folder == prefab.Folder)
				{
					if (num < distanceInfo.minDistanceSameType)
					{
						distanceInfo.minDistanceSameType = num;
					}
					if (num > distanceInfo.maxDistanceSameType)
					{
						distanceInfo.maxDistanceSameType = num;
					}
				}
				else
				{
					if (num < distanceInfo.minDistanceDifferentType)
					{
						distanceInfo.minDistanceDifferentType = num;
					}
					if (num > distanceInfo.maxDistanceDifferentType)
					{
						distanceInfo.maxDistanceDifferentType = num;
					}
				}
			}
			foreach (PlaceMonuments.SpawnInfo spawnInfo2 in spawns)
			{
				if (spawnInfo2.dungeonEntrance)
				{
					float sqrMagnitude = (spawnInfo2.dungeonEntrancePos - dungeonPos).sqrMagnitude;
					if (sqrMagnitude < distanceInfo.minDistanceDungeonEntrance)
					{
						distanceInfo.minDistanceDungeonEntrance = sqrMagnitude;
					}
					if (sqrMagnitude > distanceInfo.maxDistanceDungeonEntrance)
					{
						distanceInfo.maxDistanceDungeonEntrance = sqrMagnitude;
					}
				}
			}
		}
		if (TerrainMeta.Path != null)
		{
			foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
			{
				float num2 = monumentInfo.SqrDistance(obb);
				if (num2 < distanceInfo.minDistanceDifferentType)
				{
					distanceInfo.minDistanceDifferentType = num2;
				}
				if (num2 > distanceInfo.maxDistanceDifferentType)
				{
					distanceInfo.maxDistanceDifferentType = num2;
				}
			}
			foreach (DungeonGridInfo dungeonGridInfo in TerrainMeta.Path.DungeonGridEntrances)
			{
				float num3 = dungeonGridInfo.SqrDistance(dungeonPos);
				if (num3 < distanceInfo.minDistanceDungeonEntrance)
				{
					distanceInfo.minDistanceDungeonEntrance = num3;
				}
				if (num3 > distanceInfo.maxDistanceDungeonEntrance)
				{
					distanceInfo.maxDistanceDungeonEntrance = num3;
				}
			}
		}
		if (distanceInfo.minDistanceSameType != 3.4028235E+38f)
		{
			distanceInfo.minDistanceSameType = Mathf.Sqrt(distanceInfo.minDistanceSameType);
		}
		if (distanceInfo.maxDistanceSameType != -3.4028235E+38f)
		{
			distanceInfo.maxDistanceSameType = Mathf.Sqrt(distanceInfo.maxDistanceSameType);
		}
		if (distanceInfo.minDistanceDifferentType != 3.4028235E+38f)
		{
			distanceInfo.minDistanceDifferentType = Mathf.Sqrt(distanceInfo.minDistanceDifferentType);
		}
		if (distanceInfo.maxDistanceDifferentType != -3.4028235E+38f)
		{
			distanceInfo.maxDistanceDifferentType = Mathf.Sqrt(distanceInfo.maxDistanceDifferentType);
		}
		if (distanceInfo.minDistanceDungeonEntrance != 3.4028235E+38f)
		{
			distanceInfo.minDistanceDungeonEntrance = Mathf.Sqrt(distanceInfo.minDistanceDungeonEntrance);
		}
		if (distanceInfo.maxDistanceDungeonEntrance != -3.4028235E+38f)
		{
			distanceInfo.maxDistanceDungeonEntrance = Mathf.Sqrt(distanceInfo.maxDistanceDungeonEntrance);
		}
		return distanceInfo;
	}

	// Token: 0x0400272C RID: 10028
	public SpawnFilter Filter;

	// Token: 0x0400272D RID: 10029
	public string ResourceFolder = string.Empty;

	// Token: 0x0400272E RID: 10030
	public int TargetCount;

	// Token: 0x0400272F RID: 10031
	[FormerlySerializedAs("MinDistance")]
	public int MinDistanceSameType = 500;

	// Token: 0x04002730 RID: 10032
	public int MinDistanceDifferentType;

	// Token: 0x04002731 RID: 10033
	[FormerlySerializedAs("MinSize")]
	public int MinWorldSize;

	// Token: 0x04002732 RID: 10034
	[Tooltip("Distance to monuments of the same type")]
	public PlaceMonuments.DistanceMode DistanceSameType = PlaceMonuments.DistanceMode.Max;

	// Token: 0x04002733 RID: 10035
	[Tooltip("Distance to monuments of a different type")]
	public PlaceMonuments.DistanceMode DistanceDifferentType;

	// Token: 0x04002734 RID: 10036
	private const int GroupCandidates = 8;

	// Token: 0x04002735 RID: 10037
	private const int IndividualCandidates = 8;

	// Token: 0x04002736 RID: 10038
	private const int Attempts = 10000;

	// Token: 0x04002737 RID: 10039
	private const int MaxDepth = 100000;

	// Token: 0x02000DB4 RID: 3508
	private struct SpawnInfo
	{
		// Token: 0x04004749 RID: 18249
		public Prefab<MonumentInfo> prefab;

		// Token: 0x0400474A RID: 18250
		public Vector3 position;

		// Token: 0x0400474B RID: 18251
		public Quaternion rotation;

		// Token: 0x0400474C RID: 18252
		public Vector3 scale;

		// Token: 0x0400474D RID: 18253
		public bool dungeonEntrance;

		// Token: 0x0400474E RID: 18254
		public Vector3 dungeonEntrancePos;
	}

	// Token: 0x02000DB5 RID: 3509
	private struct DistanceInfo
	{
		// Token: 0x0400474F RID: 18255
		public float minDistanceSameType;

		// Token: 0x04004750 RID: 18256
		public float maxDistanceSameType;

		// Token: 0x04004751 RID: 18257
		public float minDistanceDifferentType;

		// Token: 0x04004752 RID: 18258
		public float maxDistanceDifferentType;

		// Token: 0x04004753 RID: 18259
		public float minDistanceDungeonEntrance;

		// Token: 0x04004754 RID: 18260
		public float maxDistanceDungeonEntrance;
	}

	// Token: 0x02000DB6 RID: 3510
	public enum DistanceMode
	{
		// Token: 0x04004756 RID: 18262
		Any,
		// Token: 0x04004757 RID: 18263
		Min,
		// Token: 0x04004758 RID: 18264
		Max
	}
}
