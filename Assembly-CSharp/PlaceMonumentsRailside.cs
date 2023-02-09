using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020006B1 RID: 1713
public class PlaceMonumentsRailside : ProceduralComponent
{
	// Token: 0x0600304E RID: 12366 RVA: 0x0012841C File Offset: 0x0012661C
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
		PlaceMonumentsRailside.SpawnInfoGroup[] array5 = new PlaceMonumentsRailside.SpawnInfoGroup[array4.Length];
		for (int j = 0; j < array4.Length; j++)
		{
			Prefab<MonumentInfo> prefab = array4[j];
			PlaceMonumentsRailside.SpawnInfoGroup spawnInfoGroup = null;
			for (int k = 0; k < j; k++)
			{
				PlaceMonumentsRailside.SpawnInfoGroup spawnInfoGroup2 = array5[k];
				Prefab<MonumentInfo> prefab2 = spawnInfoGroup2.prefab;
				if (prefab == prefab2)
				{
					spawnInfoGroup = spawnInfoGroup2;
					break;
				}
			}
			if (spawnInfoGroup == null)
			{
				spawnInfoGroup = new PlaceMonumentsRailside.SpawnInfoGroup();
				spawnInfoGroup.prefab = array4[j];
				spawnInfoGroup.candidates = new List<PlaceMonumentsRailside.SpawnInfo>();
			}
			array5[j] = spawnInfoGroup;
		}
		foreach (PlaceMonumentsRailside.SpawnInfoGroup spawnInfoGroup3 in array5)
		{
			if (!spawnInfoGroup3.processed)
			{
				Prefab<MonumentInfo> prefab3 = spawnInfoGroup3.prefab;
				MonumentInfo component = prefab3.Component;
				if (!(component == null) && (ulong)World.Size >= (ulong)((long)component.MinWorldSize))
				{
					int num = 0;
					Vector3 vector = Vector3.zero;
					Vector3 b = Vector3.zero;
					Vector3 a = Vector3.zero;
					foreach (TerrainPathConnect terrainPathConnect in prefab3.Object.GetComponentsInChildren<TerrainPathConnect>(true))
					{
						if (terrainPathConnect.Type == InfrastructureType.Rail)
						{
							if (num == 0)
							{
								b = terrainPathConnect.transform.position;
							}
							else if (num == 1)
							{
								a = terrainPathConnect.transform.position;
							}
							vector += terrainPathConnect.transform.position;
							num++;
						}
					}
					Vector3 normalized = (a - b).normalized;
					Vector3 a2 = PlaceMonumentsRailside.rot90 * normalized;
					if (num > 1)
					{
						vector /= (float)num;
					}
					if (this.PositionOffset > 0)
					{
						vector += a2 * (float)this.PositionOffset;
					}
					foreach (PathList pathList in TerrainMeta.Path.Rails)
					{
						PathInterpolator path = pathList.Path;
						float num2 = (float)(this.TangentInterval / 2);
						float num3 = 5f;
						float num4 = 5f;
						float num5 = path.StartOffset + num4;
						float num6 = path.Length - path.EndOffset - num4;
						for (float num7 = num5; num7 <= num6; num7 += num3)
						{
							Vector3 vector2 = pathList.Spline ? path.GetPointCubicHermite(num7) : path.GetPoint(num7);
							Vector3 normalized2 = (path.GetPoint(num7 + num2) - path.GetPoint(num7 - num2)).normalized;
							for (int m = -1; m <= 1; m += 2)
							{
								Quaternion quaternion = Quaternion.LookRotation((float)m * normalized2.XZ3D());
								Vector3 vector3 = vector2;
								Quaternion quaternion2 = quaternion;
								Vector3 localScale = prefab3.Object.transform.localScale;
								quaternion2 *= Quaternion.LookRotation(normalized);
								vector3 -= quaternion2 * vector;
								PlaceMonumentsRailside.SpawnInfo item = default(PlaceMonumentsRailside.SpawnInfo);
								item.prefab = prefab3;
								item.position = vector3;
								item.rotation = quaternion2;
								item.scale = localScale;
								spawnInfoGroup3.candidates.Add(item);
							}
						}
					}
					spawnInfoGroup3.processed = true;
				}
			}
		}
		List<PlaceMonumentsRailside.SpawnInfo> list2 = new List<PlaceMonumentsRailside.SpawnInfo>();
		int num8 = 0;
		List<PlaceMonumentsRailside.SpawnInfo> list3 = new List<PlaceMonumentsRailside.SpawnInfo>();
		for (int n = 0; n < 8; n++)
		{
			int num9 = 0;
			list2.Clear();
			array5.Shuffle(ref seed);
			foreach (PlaceMonumentsRailside.SpawnInfoGroup spawnInfoGroup4 in array5)
			{
				Prefab<MonumentInfo> prefab4 = spawnInfoGroup4.prefab;
				MonumentInfo component2 = prefab4.Component;
				if (!(component2 == null) && (ulong)World.Size >= (ulong)((long)component2.MinWorldSize))
				{
					DungeonGridInfo dungeonEntrance = component2.DungeonEntrance;
					int num10 = (int)(prefab4.Parameters ? (prefab4.Parameters.Priority + 1) : PrefabPriority.Low);
					int num11 = 100000 * num10 * num10 * num10 * num10;
					int num12 = 0;
					int num13 = 0;
					PlaceMonumentsRailside.SpawnInfo item2 = default(PlaceMonumentsRailside.SpawnInfo);
					spawnInfoGroup4.candidates.Shuffle(ref seed);
					for (int num14 = 0; num14 < spawnInfoGroup4.candidates.Count; num14++)
					{
						PlaceMonumentsRailside.SpawnInfo spawnInfo = spawnInfoGroup4.candidates[num14];
						PlaceMonumentsRailside.DistanceInfo distanceInfo = this.GetDistanceInfo(list2, prefab4, spawnInfo.position, spawnInfo.rotation, spawnInfo.scale);
						if (distanceInfo.minDistanceSameType >= (float)this.MinDistanceSameType && distanceInfo.minDistanceDifferentType >= (float)this.MinDistanceDifferentType)
						{
							int num15 = num11;
							if (distanceInfo.minDistanceSameType != 3.4028235E+38f)
							{
								if (this.DistanceSameType == PlaceMonumentsRailside.DistanceMode.Min)
								{
									num15 -= Mathf.RoundToInt(distanceInfo.minDistanceSameType * distanceInfo.minDistanceSameType * 2f);
								}
								else if (this.DistanceSameType == PlaceMonumentsRailside.DistanceMode.Max)
								{
									num15 += Mathf.RoundToInt(distanceInfo.minDistanceSameType * distanceInfo.minDistanceSameType * 2f);
								}
							}
							if (distanceInfo.minDistanceDifferentType != 3.4028235E+38f)
							{
								if (this.DistanceDifferentType == PlaceMonumentsRailside.DistanceMode.Min)
								{
									num15 -= Mathf.RoundToInt(distanceInfo.minDistanceDifferentType * distanceInfo.minDistanceDifferentType);
								}
								else if (this.DistanceDifferentType == PlaceMonumentsRailside.DistanceMode.Max)
								{
									num15 += Mathf.RoundToInt(distanceInfo.minDistanceDifferentType * distanceInfo.minDistanceDifferentType);
								}
							}
							if (num15 > num13 && prefab4.ApplyTerrainAnchors(ref spawnInfo.position, spawnInfo.rotation, spawnInfo.scale, this.Filter) && component2.CheckPlacement(spawnInfo.position, spawnInfo.rotation, spawnInfo.scale))
							{
								if (dungeonEntrance)
								{
									Vector3 vector4 = spawnInfo.position + spawnInfo.rotation * Vector3.Scale(spawnInfo.scale, dungeonEntrance.transform.position);
									Vector3 vector5 = dungeonEntrance.SnapPosition(vector4);
									spawnInfo.position += vector5 - vector4;
									if (!dungeonEntrance.IsValidSpawnPosition(vector5))
									{
										goto IL_736;
									}
								}
								if (prefab4.ApplyTerrainChecks(spawnInfo.position, spawnInfo.rotation, spawnInfo.scale, this.Filter) && prefab4.ApplyTerrainFilters(spawnInfo.position, spawnInfo.rotation, spawnInfo.scale, null) && prefab4.ApplyWaterChecks(spawnInfo.position, spawnInfo.rotation, spawnInfo.scale) && !prefab4.CheckEnvironmentVolumes(spawnInfo.position, spawnInfo.rotation, spawnInfo.scale, EnvironmentType.Underground | EnvironmentType.TrainTunnels))
								{
									num13 = num15;
									item2 = spawnInfo;
									num12++;
									if (num12 >= 8 || this.DistanceDifferentType == PlaceMonumentsRailside.DistanceMode.Any)
									{
										break;
									}
								}
							}
						}
						IL_736:;
					}
					if (num13 > 0)
					{
						list2.Add(item2);
						num9 += num13;
					}
					if (this.TargetCount > 0 && list2.Count >= this.TargetCount)
					{
						break;
					}
				}
			}
			if (num9 > num8)
			{
				num8 = num9;
				GenericsUtil.Swap<List<PlaceMonumentsRailside.SpawnInfo>>(ref list2, ref list3);
			}
		}
		foreach (PlaceMonumentsRailside.SpawnInfo spawnInfo2 in list3)
		{
			World.AddPrefab("Monument", spawnInfo2.prefab, spawnInfo2.position, spawnInfo2.rotation, spawnInfo2.scale);
		}
	}

	// Token: 0x0600304F RID: 12367 RVA: 0x00128C60 File Offset: 0x00126E60
	private PlaceMonumentsRailside.DistanceInfo GetDistanceInfo(List<PlaceMonumentsRailside.SpawnInfo> spawns, Prefab<MonumentInfo> prefab, Vector3 monumentPos, Quaternion monumentRot, Vector3 monumentScale)
	{
		PlaceMonumentsRailside.DistanceInfo distanceInfo = default(PlaceMonumentsRailside.DistanceInfo);
		distanceInfo.minDistanceDifferentType = float.MaxValue;
		distanceInfo.maxDistanceDifferentType = float.MinValue;
		distanceInfo.minDistanceSameType = float.MaxValue;
		distanceInfo.maxDistanceSameType = float.MinValue;
		OBB obb = new OBB(monumentPos, monumentScale, monumentRot, prefab.Component.Bounds);
		if (TerrainMeta.Path != null)
		{
			foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
			{
				if (!prefab.Component.HasDungeonLink || (!monumentInfo.HasDungeonLink && monumentInfo.WantsDungeonLink))
				{
					float num = monumentInfo.SqrDistance(obb);
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
			if (distanceInfo.minDistanceDifferentType != 3.4028235E+38f)
			{
				distanceInfo.minDistanceDifferentType = Mathf.Sqrt(distanceInfo.minDistanceDifferentType);
			}
			if (distanceInfo.maxDistanceDifferentType != -3.4028235E+38f)
			{
				distanceInfo.maxDistanceDifferentType = Mathf.Sqrt(distanceInfo.maxDistanceDifferentType);
			}
		}
		if (spawns != null)
		{
			foreach (PlaceMonumentsRailside.SpawnInfo spawnInfo in spawns)
			{
				float num2 = new OBB(spawnInfo.position, spawnInfo.scale, spawnInfo.rotation, spawnInfo.prefab.Component.Bounds).SqrDistance(obb);
				if (num2 < distanceInfo.minDistanceSameType)
				{
					distanceInfo.minDistanceSameType = num2;
				}
				if (num2 > distanceInfo.maxDistanceSameType)
				{
					distanceInfo.maxDistanceSameType = num2;
				}
			}
			if (prefab.Component.HasDungeonLink)
			{
				foreach (MonumentInfo monumentInfo2 in TerrainMeta.Path.Monuments)
				{
					if (monumentInfo2.HasDungeonLink || !monumentInfo2.WantsDungeonLink)
					{
						float num3 = monumentInfo2.SqrDistance(obb);
						if (num3 < distanceInfo.minDistanceSameType)
						{
							distanceInfo.minDistanceSameType = num3;
						}
						if (num3 > distanceInfo.maxDistanceSameType)
						{
							distanceInfo.maxDistanceSameType = num3;
						}
					}
				}
				foreach (DungeonGridInfo dungeonGridInfo in TerrainMeta.Path.DungeonGridEntrances)
				{
					float num4 = dungeonGridInfo.SqrDistance(monumentPos);
					if (num4 < distanceInfo.minDistanceSameType)
					{
						distanceInfo.minDistanceSameType = num4;
					}
					if (num4 > distanceInfo.maxDistanceSameType)
					{
						distanceInfo.maxDistanceSameType = num4;
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
		}
		return distanceInfo;
	}

	// Token: 0x04002740 RID: 10048
	public SpawnFilter Filter;

	// Token: 0x04002741 RID: 10049
	public string ResourceFolder = string.Empty;

	// Token: 0x04002742 RID: 10050
	public int TargetCount;

	// Token: 0x04002743 RID: 10051
	public int PositionOffset = 100;

	// Token: 0x04002744 RID: 10052
	public int TangentInterval = 100;

	// Token: 0x04002745 RID: 10053
	[FormerlySerializedAs("MinDistance")]
	public int MinDistanceSameType = 500;

	// Token: 0x04002746 RID: 10054
	public int MinDistanceDifferentType;

	// Token: 0x04002747 RID: 10055
	[FormerlySerializedAs("MinSize")]
	public int MinWorldSize;

	// Token: 0x04002748 RID: 10056
	[Tooltip("Distance to monuments of the same type")]
	public PlaceMonumentsRailside.DistanceMode DistanceSameType = PlaceMonumentsRailside.DistanceMode.Max;

	// Token: 0x04002749 RID: 10057
	[Tooltip("Distance to monuments of a different type")]
	public PlaceMonumentsRailside.DistanceMode DistanceDifferentType;

	// Token: 0x0400274A RID: 10058
	private const int GroupCandidates = 8;

	// Token: 0x0400274B RID: 10059
	private const int IndividualCandidates = 8;

	// Token: 0x0400274C RID: 10060
	private static Quaternion rot90 = Quaternion.Euler(0f, 90f, 0f);

	// Token: 0x02000DBA RID: 3514
	private struct SpawnInfo
	{
		// Token: 0x04004761 RID: 18273
		public Prefab<MonumentInfo> prefab;

		// Token: 0x04004762 RID: 18274
		public Vector3 position;

		// Token: 0x04004763 RID: 18275
		public Quaternion rotation;

		// Token: 0x04004764 RID: 18276
		public Vector3 scale;
	}

	// Token: 0x02000DBB RID: 3515
	private class SpawnInfoGroup
	{
		// Token: 0x04004765 RID: 18277
		public bool processed;

		// Token: 0x04004766 RID: 18278
		public Prefab<MonumentInfo> prefab;

		// Token: 0x04004767 RID: 18279
		public List<PlaceMonumentsRailside.SpawnInfo> candidates;
	}

	// Token: 0x02000DBC RID: 3516
	private struct DistanceInfo
	{
		// Token: 0x04004768 RID: 18280
		public float minDistanceSameType;

		// Token: 0x04004769 RID: 18281
		public float maxDistanceSameType;

		// Token: 0x0400476A RID: 18282
		public float minDistanceDifferentType;

		// Token: 0x0400476B RID: 18283
		public float maxDistanceDifferentType;
	}

	// Token: 0x02000DBD RID: 3517
	public enum DistanceMode
	{
		// Token: 0x0400476D RID: 18285
		Any,
		// Token: 0x0400476E RID: 18286
		Min,
		// Token: 0x0400476F RID: 18287
		Max
	}
}
