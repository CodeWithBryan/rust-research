using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200068E RID: 1678
public class GenerateDungeonGrid : ProceduralComponent
{
	// Token: 0x170003B2 RID: 946
	// (get) Token: 0x06002FEB RID: 12267 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002FEC RID: 12268 RVA: 0x0011F044 File Offset: 0x0011D244
	public override void Process(uint seed)
	{
		if (World.Cached)
		{
			TerrainMeta.Path.DungeonGridRoot = HierarchyUtil.GetRoot("Dungeon", true, false);
			return;
		}
		if (World.Networked)
		{
			World.Spawn("Dungeon", null);
			TerrainMeta.Path.DungeonGridRoot = HierarchyUtil.GetRoot("Dungeon", true, false);
			return;
		}
		Prefab<DungeonGridCell>[] array = Prefab.Load<DungeonGridCell>("assets/bundled/prefabs/autospawn/" + this.TunnelFolder, null, null, true);
		if (array == null || array.Length == 0)
		{
			return;
		}
		Prefab<DungeonGridCell>[] array2 = Prefab.Load<DungeonGridCell>("assets/bundled/prefabs/autospawn/" + this.StationFolder, null, null, true);
		if (array2 == null || array2.Length == 0)
		{
			return;
		}
		Prefab<DungeonGridCell>[] array3 = Prefab.Load<DungeonGridCell>("assets/bundled/prefabs/autospawn/" + this.TransitionFolder, null, null, true);
		if (array3 == null)
		{
			return;
		}
		Prefab<DungeonGridLink>[] array4 = Prefab.Load<DungeonGridLink>("assets/bundled/prefabs/autospawn/" + this.LinkFolder, null, null, true);
		if (array4 == null)
		{
			return;
		}
		array4 = (from x in array4
		orderby x.Component.Priority descending
		select x).ToArray<Prefab<DungeonGridLink>>();
		List<DungeonGridInfo> list = TerrainMeta.Path ? TerrainMeta.Path.DungeonGridEntrances : null;
		WorldSpaceGrid<Prefab<DungeonGridCell>> worldSpaceGrid = new WorldSpaceGrid<Prefab<DungeonGridCell>>(TerrainMeta.Size.x * 2f, (float)this.CellSize);
		int[,] array5 = new int[worldSpaceGrid.CellCount, worldSpaceGrid.CellCount];
		GenerateDungeonGrid.<>c__DisplayClass17_0 CS$<>8__locals1;
		CS$<>8__locals1.hashmap = new DungeonGridConnectionHash[worldSpaceGrid.CellCount, worldSpaceGrid.CellCount];
		CS$<>8__locals1.pathFinder = new PathFinder(array5, false, true);
		int cellCount = worldSpaceGrid.CellCount;
		int num = 0;
		int num2 = worldSpaceGrid.CellCount - 1;
		for (int i = 0; i < cellCount; i++)
		{
			for (int j = 0; j < cellCount; j++)
			{
				array5[j, i] = 1;
			}
		}
		List<GenerateDungeonGrid.PathSegment> list2 = new List<GenerateDungeonGrid.PathSegment>();
		List<GenerateDungeonGrid.PathLink> list3 = new List<GenerateDungeonGrid.PathLink>();
		List<GenerateDungeonGrid.PathNode> list4 = new List<GenerateDungeonGrid.PathNode>();
		CS$<>8__locals1.unconnectedNodeList = new List<GenerateDungeonGrid.PathNode>();
		CS$<>8__locals1.secondaryNodeList = new List<GenerateDungeonGrid.PathNode>();
		List<PathFinder.Point> list5 = new List<PathFinder.Point>();
		List<PathFinder.Point> list6 = new List<PathFinder.Point>();
		List<PathFinder.Point> list7 = new List<PathFinder.Point>();
		using (List<DungeonGridInfo>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				DungeonGridInfo entrance = enumerator.Current;
				GenerateDungeonGrid.<>c__DisplayClass17_1 CS$<>8__locals2;
				CS$<>8__locals2.entrance = entrance;
				foreach (TerrainPathConnect terrainPathConnect in CS$<>8__locals2.entrance.GetComponentsInChildren<TerrainPathConnect>(true))
				{
					GenerateDungeonGrid.<>c__DisplayClass17_2 CS$<>8__locals3 = new GenerateDungeonGrid.<>c__DisplayClass17_2();
					CS$<>8__locals3.<>4__this = this;
					if (terrainPathConnect.Type == this.ConnectionType)
					{
						Vector2i vector2i = worldSpaceGrid.WorldToGridCoords(terrainPathConnect.transform.position);
						if (array5[vector2i.x, vector2i.y] != 2147483647)
						{
							CS$<>8__locals3.stationNode = CS$<>8__locals1.pathFinder.FindClosestWalkable(new PathFinder.Point(vector2i.x, vector2i.y), 1);
							if (CS$<>8__locals3.stationNode != null)
							{
								Prefab<DungeonGridCell> prefab = (vector2i.x > num) ? worldSpaceGrid[vector2i.x - 1, vector2i.y] : null;
								Prefab<DungeonGridCell> prefab2 = (vector2i.x < num2) ? worldSpaceGrid[vector2i.x + 1, vector2i.y] : null;
								Prefab<DungeonGridCell> prefab3 = (vector2i.y > num) ? worldSpaceGrid[vector2i.x, vector2i.y - 1] : null;
								Prefab<DungeonGridCell> prefab4 = (vector2i.y < num2) ? worldSpaceGrid[vector2i.x, vector2i.y + 1] : null;
								Prefab<DungeonGridCell> prefab5 = null;
								float num3 = float.MaxValue;
								array2.Shuffle(ref seed);
								foreach (Prefab<DungeonGridCell> prefab6 in array2)
								{
									if ((prefab == null || prefab6.Component.West == prefab.Component.East) && (prefab2 == null || prefab6.Component.East == prefab2.Component.West) && (prefab3 == null || prefab6.Component.South == prefab3.Component.North) && (prefab4 == null || prefab6.Component.North == prefab4.Component.South))
									{
										DungeonVolume componentInChildren = prefab6.Object.GetComponentInChildren<DungeonVolume>();
										DungeonVolume componentInChildren2 = CS$<>8__locals2.entrance.GetComponentInChildren<DungeonVolume>();
										OBB bounds = componentInChildren.GetBounds(worldSpaceGrid.GridToWorldCoords(vector2i), Quaternion.identity);
										OBB bounds2 = componentInChildren2.GetBounds(CS$<>8__locals2.entrance.transform.position, Quaternion.identity);
										if (!bounds.Intersects2D(bounds2))
										{
											DungeonGridLink componentInChildren3 = prefab6.Object.GetComponentInChildren<DungeonGridLink>();
											Vector3 b = worldSpaceGrid.GridToWorldCoords(new Vector2i(vector2i.x, vector2i.y)) + componentInChildren3.UpSocket.localPosition;
											float num4 = (terrainPathConnect.transform.position - b).Magnitude2D();
											if (num3 >= num4)
											{
												prefab5 = prefab6;
												num3 = num4;
											}
										}
									}
								}
								if (prefab5 != null)
								{
									worldSpaceGrid[vector2i.x, vector2i.y] = prefab5;
									array5[vector2i.x, vector2i.y] = int.MaxValue;
									GenerateDungeonGrid.<>c__DisplayClass17_3 CS$<>8__locals4;
									CS$<>8__locals4.isStartPoint = (CS$<>8__locals1.secondaryNodeList.Count == 0);
									CS$<>8__locals1.secondaryNodeList.RemoveAll((GenerateDungeonGrid.PathNode x) => x.node.point == CS$<>8__locals3.stationNode.point);
									CS$<>8__locals1.unconnectedNodeList.RemoveAll((GenerateDungeonGrid.PathNode x) => x.node.point == CS$<>8__locals3.stationNode.point);
									if (prefab5.Component.West != DungeonGridConnectionType.None)
									{
										CS$<>8__locals3.<Process>g__AddNode|1(vector2i.x - 1, vector2i.y, ref CS$<>8__locals1, ref CS$<>8__locals2, ref CS$<>8__locals4);
									}
									if (prefab5.Component.East != DungeonGridConnectionType.None)
									{
										CS$<>8__locals3.<Process>g__AddNode|1(vector2i.x + 1, vector2i.y, ref CS$<>8__locals1, ref CS$<>8__locals2, ref CS$<>8__locals4);
									}
									if (prefab5.Component.South != DungeonGridConnectionType.None)
									{
										CS$<>8__locals3.<Process>g__AddNode|1(vector2i.x, vector2i.y - 1, ref CS$<>8__locals1, ref CS$<>8__locals2, ref CS$<>8__locals4);
									}
									if (prefab5.Component.North != DungeonGridConnectionType.None)
									{
										CS$<>8__locals3.<Process>g__AddNode|1(vector2i.x, vector2i.y + 1, ref CS$<>8__locals1, ref CS$<>8__locals2, ref CS$<>8__locals4);
									}
									GenerateDungeonGrid.PathLink pathLink = new GenerateDungeonGrid.PathLink();
									DungeonGridLink componentInChildren4 = CS$<>8__locals2.entrance.gameObject.GetComponentInChildren<DungeonGridLink>();
									Vector3 position = CS$<>8__locals2.entrance.transform.position;
									Vector3 eulerAngles = CS$<>8__locals2.entrance.transform.rotation.eulerAngles;
									DungeonGridLink componentInChildren5 = prefab5.Object.GetComponentInChildren<DungeonGridLink>();
									Vector3 position2 = worldSpaceGrid.GridToWorldCoords(new Vector2i(vector2i.x, vector2i.y));
									Vector3 zero = Vector3.zero;
									pathLink.downwards = new GenerateDungeonGrid.PathLinkSide();
									pathLink.downwards.origin = new GenerateDungeonGrid.PathLinkSegment();
									pathLink.downwards.origin.position = position;
									pathLink.downwards.origin.rotation = Quaternion.Euler(eulerAngles);
									pathLink.downwards.origin.scale = Vector3.one;
									pathLink.downwards.origin.link = componentInChildren4;
									pathLink.downwards.segments = new List<GenerateDungeonGrid.PathLinkSegment>();
									pathLink.upwards = new GenerateDungeonGrid.PathLinkSide();
									pathLink.upwards.origin = new GenerateDungeonGrid.PathLinkSegment();
									pathLink.upwards.origin.position = position2;
									pathLink.upwards.origin.rotation = Quaternion.Euler(zero);
									pathLink.upwards.origin.scale = Vector3.one;
									pathLink.upwards.origin.link = componentInChildren5;
									pathLink.upwards.segments = new List<GenerateDungeonGrid.PathLinkSegment>();
									list3.Add(pathLink);
								}
							}
						}
					}
				}
			}
			goto IL_AEF;
		}
		IL_7A5:
		if (CS$<>8__locals1.unconnectedNodeList.Count == 0)
		{
			GenerateDungeonGrid.PathNode node = CS$<>8__locals1.secondaryNodeList[0];
			CS$<>8__locals1.unconnectedNodeList.AddRange(from x in CS$<>8__locals1.secondaryNodeList
			where x.monument == node.monument
			select x);
			CS$<>8__locals1.secondaryNodeList.RemoveAll((GenerateDungeonGrid.PathNode x) => x.monument == node.monument);
			Vector2i vector2i2 = worldSpaceGrid.WorldToGridCoords(node.monument.transform.position);
			CS$<>8__locals1.pathFinder.PushPoint = new PathFinder.Point(vector2i2.x, vector2i2.y);
			CS$<>8__locals1.pathFinder.PushRadius = 2;
			CS$<>8__locals1.pathFinder.PushDistance = 2;
			CS$<>8__locals1.pathFinder.PushMultiplier = 4;
		}
		list7.Clear();
		list7.AddRange(from x in CS$<>8__locals1.unconnectedNodeList
		select x.node.point);
		list6.Clear();
		list6.AddRange(from x in list4
		select x.node.point);
		list6.AddRange(from x in CS$<>8__locals1.secondaryNodeList
		select x.node.point);
		list6.AddRange(list5);
		PathFinder.Node node5 = CS$<>8__locals1.pathFinder.FindPathUndirected(list6, list7, 100000);
		if (node5 == null)
		{
			GenerateDungeonGrid.PathNode node = CS$<>8__locals1.unconnectedNodeList[0];
			CS$<>8__locals1.secondaryNodeList.AddRange(from x in CS$<>8__locals1.unconnectedNodeList
			where x.monument == node.monument
			select x);
			CS$<>8__locals1.unconnectedNodeList.RemoveAll((GenerateDungeonGrid.PathNode x) => x.monument == node.monument);
			CS$<>8__locals1.secondaryNodeList.Remove(node);
			list4.Add(node);
		}
		else
		{
			GenerateDungeonGrid.PathSegment segment = new GenerateDungeonGrid.PathSegment();
			for (PathFinder.Node node2 = node5; node2 != null; node2 = node2.next)
			{
				if (node2 == node5)
				{
					segment.start = node2;
				}
				if (node2.next == null)
				{
					segment.end = node2;
				}
			}
			list2.Add(segment);
			GenerateDungeonGrid.PathNode node = CS$<>8__locals1.unconnectedNodeList.Find((GenerateDungeonGrid.PathNode x) => x.node.point == segment.start.point || x.node.point == segment.end.point);
			CS$<>8__locals1.secondaryNodeList.AddRange(from x in CS$<>8__locals1.unconnectedNodeList
			where x.monument == node.monument
			select x);
			CS$<>8__locals1.unconnectedNodeList.RemoveAll((GenerateDungeonGrid.PathNode x) => x.monument == node.monument);
			CS$<>8__locals1.secondaryNodeList.Remove(node);
			list4.Add(node);
			GenerateDungeonGrid.PathNode pathNode = CS$<>8__locals1.secondaryNodeList.Find((GenerateDungeonGrid.PathNode x) => x.node.point == segment.start.point || x.node.point == segment.end.point);
			if (pathNode != null)
			{
				CS$<>8__locals1.secondaryNodeList.Remove(pathNode);
				list4.Add(pathNode);
			}
			for (PathFinder.Node node3 = node5; node3 != null; node3 = node3.next)
			{
				if (node3 != node5 && node3.next != null)
				{
					list5.Add(node3.point);
				}
			}
		}
		IL_AEF:
		if (CS$<>8__locals1.unconnectedNodeList.Count == 0 && CS$<>8__locals1.secondaryNodeList.Count == 0)
		{
			foreach (GenerateDungeonGrid.PathSegment pathSegment in list2)
			{
				PathFinder.Node node4 = pathSegment.start;
				while (node4 != null && node4.next != null)
				{
					DungeonGridConnectionHash dungeonGridConnectionHash = CS$<>8__locals1.hashmap[node4.point.x, node4.point.y];
					DungeonGridConnectionHash dungeonGridConnectionHash2 = CS$<>8__locals1.hashmap[node4.next.point.x, node4.next.point.y];
					if (node4.point.x > node4.next.point.x)
					{
						dungeonGridConnectionHash.West = true;
						dungeonGridConnectionHash2.East = true;
					}
					if (node4.point.x < node4.next.point.x)
					{
						dungeonGridConnectionHash.East = true;
						dungeonGridConnectionHash2.West = true;
					}
					if (node4.point.y > node4.next.point.y)
					{
						dungeonGridConnectionHash.South = true;
						dungeonGridConnectionHash2.North = true;
					}
					if (node4.point.y < node4.next.point.y)
					{
						dungeonGridConnectionHash.North = true;
						dungeonGridConnectionHash2.South = true;
					}
					CS$<>8__locals1.hashmap[node4.point.x, node4.point.y] = dungeonGridConnectionHash;
					CS$<>8__locals1.hashmap[node4.next.point.x, node4.next.point.y] = dungeonGridConnectionHash2;
					node4 = node4.next;
				}
			}
			for (int m = 0; m < worldSpaceGrid.CellCount; m++)
			{
				for (int n = 0; n < worldSpaceGrid.CellCount; n++)
				{
					if (array5[m, n] != 2147483647)
					{
						DungeonGridConnectionHash dungeonGridConnectionHash3 = CS$<>8__locals1.hashmap[m, n];
						if (dungeonGridConnectionHash3.Value != 0)
						{
							array.Shuffle(ref seed);
							foreach (Prefab<DungeonGridCell> prefab7 in array)
							{
								Prefab<DungeonGridCell> prefab8 = (m > num) ? worldSpaceGrid[m - 1, n] : null;
								if ((prefab8 != null) ? (prefab7.Component.West == prefab8.Component.East) : (dungeonGridConnectionHash3.West ? (prefab7.Component.West > DungeonGridConnectionType.None) : (prefab7.Component.West == DungeonGridConnectionType.None)))
								{
									Prefab<DungeonGridCell> prefab9 = (m < num2) ? worldSpaceGrid[m + 1, n] : null;
									if ((prefab9 != null) ? (prefab7.Component.East == prefab9.Component.West) : (dungeonGridConnectionHash3.East ? (prefab7.Component.East > DungeonGridConnectionType.None) : (prefab7.Component.East == DungeonGridConnectionType.None)))
									{
										Prefab<DungeonGridCell> prefab10 = (n > num) ? worldSpaceGrid[m, n - 1] : null;
										if ((prefab10 != null) ? (prefab7.Component.South == prefab10.Component.North) : (dungeonGridConnectionHash3.South ? (prefab7.Component.South > DungeonGridConnectionType.None) : (prefab7.Component.South == DungeonGridConnectionType.None)))
										{
											Prefab<DungeonGridCell> prefab11 = (n < num2) ? worldSpaceGrid[m, n + 1] : null;
											if (((prefab11 != null) ? (prefab7.Component.North == prefab11.Component.South) : (dungeonGridConnectionHash3.North ? (prefab7.Component.North > DungeonGridConnectionType.None) : (prefab7.Component.North == DungeonGridConnectionType.None))) && (prefab7.Component.West == DungeonGridConnectionType.None || prefab8 == null || !prefab7.Component.ShouldAvoid(prefab8.ID)) && (prefab7.Component.East == DungeonGridConnectionType.None || prefab9 == null || !prefab7.Component.ShouldAvoid(prefab9.ID)) && (prefab7.Component.South == DungeonGridConnectionType.None || prefab10 == null || !prefab7.Component.ShouldAvoid(prefab10.ID)) && (prefab7.Component.North == DungeonGridConnectionType.None || prefab11 == null || !prefab7.Component.ShouldAvoid(prefab11.ID)))
											{
												worldSpaceGrid[m, n] = prefab7;
												bool flag = prefab8 == null || prefab7.Component.WestVariant == prefab8.Component.EastVariant;
												bool flag2 = prefab10 == null || prefab7.Component.SouthVariant == prefab10.Component.NorthVariant;
												if (flag && flag2)
												{
													break;
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			Vector3 zero2 = Vector3.zero;
			Vector3 rhs = Vector3.zero;
			Vector3 b2 = Vector3.up * 10f;
			Vector3 b3 = Vector3.up * (this.LinkTransition + 1f);
			do
			{
				rhs = zero2;
				for (int num5 = 0; num5 < worldSpaceGrid.CellCount; num5++)
				{
					for (int num6 = 0; num6 < worldSpaceGrid.CellCount; num6++)
					{
						Prefab<DungeonGridCell> prefab12 = worldSpaceGrid[num5, num6];
						if (prefab12 != null)
						{
							Vector2i cellPos = new Vector2i(num5, num6);
							Vector3 b4 = worldSpaceGrid.GridToWorldCoords(cellPos);
							while (!prefab12.CheckEnvironmentVolumesInsideTerrain(zero2 + b4 + b2, Quaternion.identity, Vector3.one, EnvironmentType.Underground, 0f) || prefab12.CheckEnvironmentVolumes(zero2 + b4 + b3, Quaternion.identity, Vector3.one, EnvironmentType.Underground | EnvironmentType.Building) || prefab12.CheckEnvironmentVolumes(zero2 + b4, Quaternion.identity, Vector3.one, EnvironmentType.Underground | EnvironmentType.Building))
							{
								zero2.y -= 9f;
							}
						}
					}
				}
			}
			while (zero2 != rhs);
			foreach (GenerateDungeonGrid.PathLink pathLink2 in list3)
			{
				pathLink2.upwards.origin.position += zero2;
			}
			for (int num7 = 0; num7 < worldSpaceGrid.CellCount; num7++)
			{
				for (int num8 = 0; num8 < worldSpaceGrid.CellCount; num8++)
				{
					Prefab<DungeonGridCell> prefab13 = worldSpaceGrid[num7, num8];
					if (prefab13 != null)
					{
						Vector2i cellPos2 = new Vector2i(num7, num8);
						Vector3 b5 = worldSpaceGrid.GridToWorldCoords(cellPos2);
						World.AddPrefab("Dungeon", prefab13, zero2 + b5, Quaternion.identity, Vector3.one);
					}
				}
			}
			for (int num9 = 0; num9 < worldSpaceGrid.CellCount - 1; num9++)
			{
				for (int num10 = 0; num10 < worldSpaceGrid.CellCount - 1; num10++)
				{
					Prefab<DungeonGridCell> prefab14 = worldSpaceGrid[num9, num10];
					Prefab<DungeonGridCell> prefab15 = worldSpaceGrid[num9 + 1, num10];
					Prefab<DungeonGridCell> prefab16 = worldSpaceGrid[num9, num10 + 1];
					if (prefab14 != null && prefab15 != null && prefab14.Component.EastVariant != prefab15.Component.WestVariant)
					{
						array3.Shuffle(ref seed);
						foreach (Prefab<DungeonGridCell> prefab17 in array3)
						{
							if (prefab17.Component.West == prefab14.Component.East && prefab17.Component.East == prefab15.Component.West && prefab17.Component.WestVariant == prefab14.Component.EastVariant && prefab17.Component.EastVariant == prefab15.Component.WestVariant)
							{
								Vector2i cellPos3 = new Vector2i(num9, num10);
								Vector3 b6 = worldSpaceGrid.GridToWorldCoords(cellPos3) + new Vector3(worldSpaceGrid.CellSizeHalf, 0f, 0f);
								World.AddPrefab("Dungeon", prefab17, zero2 + b6, Quaternion.identity, Vector3.one);
								break;
							}
						}
					}
					if (prefab14 != null && prefab16 != null && prefab14.Component.NorthVariant != prefab16.Component.SouthVariant)
					{
						array3.Shuffle(ref seed);
						foreach (Prefab<DungeonGridCell> prefab18 in array3)
						{
							if (prefab18.Component.South == prefab14.Component.North && prefab18.Component.North == prefab16.Component.South && prefab18.Component.SouthVariant == prefab14.Component.NorthVariant && prefab18.Component.NorthVariant == prefab16.Component.SouthVariant)
							{
								Vector2i cellPos4 = new Vector2i(num9, num10);
								Vector3 b7 = worldSpaceGrid.GridToWorldCoords(cellPos4) + new Vector3(0f, 0f, worldSpaceGrid.CellSizeHalf);
								World.AddPrefab("Dungeon", prefab18, zero2 + b7, Quaternion.identity, Vector3.one);
								break;
							}
						}
					}
				}
			}
			foreach (GenerateDungeonGrid.PathLink pathLink3 in list3)
			{
				Vector3 b8 = pathLink3.upwards.origin.position + pathLink3.upwards.origin.rotation * Vector3.Scale(pathLink3.upwards.origin.upSocket.localPosition, pathLink3.upwards.origin.scale);
				Vector3 a = pathLink3.downwards.origin.position + pathLink3.downwards.origin.rotation * Vector3.Scale(pathLink3.downwards.origin.downSocket.localPosition, pathLink3.downwards.origin.scale) - b8;
				foreach (Vector3 vector in new Vector3[]
				{
					new Vector3(0f, 1f, 0f),
					new Vector3(1f, 1f, 1f)
				})
				{
					int num11 = 0;
					int num12 = 0;
					while (a.magnitude > 1f && (num11 < 8 || num12 < 8))
					{
						bool flag3 = num11 > 2 && num12 > 2;
						bool flag4 = num11 > 4 && num12 > 4;
						Prefab<DungeonGridLink> prefab19 = null;
						Vector3 vector2 = Vector3.zero;
						int num13 = int.MinValue;
						Vector3 position3 = Vector3.zero;
						Quaternion rotation = Quaternion.identity;
						GenerateDungeonGrid.PathLinkSegment prevSegment = pathLink3.downwards.prevSegment;
						Vector3 a2 = prevSegment.position + prevSegment.rotation * Vector3.Scale(prevSegment.scale, prevSegment.downSocket.localPosition);
						Quaternion lhs = prevSegment.rotation * prevSegment.downSocket.localRotation;
						foreach (Prefab<DungeonGridLink> prefab20 in array4)
						{
							float num14 = SeedRandom.Value(ref seed);
							DungeonGridLink component = prefab20.Component;
							if (prevSegment.downType == component.UpType)
							{
								switch (component.DownType)
								{
								case DungeonGridLinkType.Elevator:
									if (flag3 || vector.x != 0f)
									{
										goto IL_1B05;
									}
									if (vector.z != 0f)
									{
										goto IL_1B05;
									}
									break;
								case DungeonGridLinkType.Transition:
									if (vector.x != 0f || vector.z != 0f)
									{
										goto IL_1B05;
									}
									break;
								}
								int num15 = flag3 ? 0 : component.Priority;
								if (num13 <= num15)
								{
									Quaternion quaternion = lhs * Quaternion.Inverse(component.UpSocket.localRotation);
									Quaternion quaternion2 = quaternion * component.DownSocket.localRotation;
									GenerateDungeonGrid.PathLinkSegment prevSegment2 = pathLink3.upwards.prevSegment;
									Quaternion quaternion3 = prevSegment2.rotation * prevSegment2.upSocket.localRotation;
									if (component.Rotation > 0)
									{
										if (Quaternion.Angle(quaternion3, quaternion2) > (float)component.Rotation)
										{
											goto IL_1B05;
										}
										Quaternion rhs2 = quaternion3 * Quaternion.Inverse(quaternion2);
										quaternion *= rhs2;
										quaternion2 *= rhs2;
									}
									Vector3 vector3 = a2 - quaternion * component.UpSocket.localPosition;
									Vector3 vector4 = quaternion * (component.DownSocket.localPosition - component.UpSocket.localPosition);
									Vector3 vector5 = a + vector2;
									Vector3 vector6 = a + vector4;
									float magnitude = vector5.magnitude;
									float magnitude2 = vector6.magnitude;
									Vector3 vector7 = Vector3.Scale(vector5, vector);
									Vector3 vector8 = Vector3.Scale(vector6, vector);
									float magnitude3 = vector7.magnitude;
									float magnitude4 = vector8.magnitude;
									if (vector2 != Vector3.zero)
									{
										if (magnitude3 < magnitude4 || (magnitude3 == magnitude4 && magnitude < magnitude2))
										{
											goto IL_1B05;
										}
										if (magnitude3 == magnitude4 && magnitude == magnitude2 && num14 < 0.5f)
										{
											goto IL_1B05;
										}
									}
									else if (magnitude3 <= magnitude4)
									{
										goto IL_1B05;
									}
									if (Mathf.Abs(vector8.x) - Mathf.Abs(vector7.x) <= 0.01f && (Mathf.Abs(vector8.x) <= 0.01f || vector5.x * vector6.x >= 0f) && Mathf.Abs(vector8.y) - Mathf.Abs(vector7.y) <= 0.01f && (Mathf.Abs(vector8.y) <= 0.01f || vector5.y * vector6.y >= 0f) && Mathf.Abs(vector8.z) - Mathf.Abs(vector7.z) <= 0.01f && (Mathf.Abs(vector8.z) <= 0.01f || vector5.z * vector6.z >= 0f) && (!flag3 || vector.x != 0f || vector.z != 0f || component.DownType != DungeonGridLinkType.Default || ((Mathf.Abs(vector6.x) <= 0.01f || Mathf.Abs(vector6.x) >= this.LinkRadius * 2f - 0.1f) && (Mathf.Abs(vector6.z) <= 0.01f || Mathf.Abs(vector6.z) >= this.LinkRadius * 2f - 0.1f))))
									{
										num13 = num15;
										if (vector.x == 0f && vector.z == 0f)
										{
											if (!flag3 && Mathf.Abs(vector6.y) < this.LinkTransition - 0.1f)
											{
												goto IL_1B05;
											}
										}
										else if ((!flag3 && magnitude4 > 0.01f && (Mathf.Abs(vector6.x) < this.LinkRadius * 2f - 0.1f || Mathf.Abs(vector6.z) < this.LinkRadius * 2f - 0.1f)) || (!flag4 && magnitude4 > 0.01f && (Mathf.Abs(vector6.x) < this.LinkRadius * 1f - 0.1f || Mathf.Abs(vector6.z) < this.LinkRadius * 1f - 0.1f)))
										{
											goto IL_1B05;
										}
										if (!flag3 || magnitude4 >= 0.01f || magnitude2 >= 0.01f || Quaternion.Angle(quaternion3, quaternion2) <= 10f)
										{
											prefab19 = prefab20;
											vector2 = vector4;
											num13 = num15;
											position3 = vector3;
											rotation = quaternion;
										}
									}
								}
							}
							IL_1B05:;
						}
						if (vector2 != Vector3.zero)
						{
							GenerateDungeonGrid.PathLinkSegment pathLinkSegment = new GenerateDungeonGrid.PathLinkSegment();
							pathLinkSegment.position = position3;
							pathLinkSegment.rotation = rotation;
							pathLinkSegment.scale = Vector3.one;
							pathLinkSegment.prefab = prefab19;
							pathLinkSegment.link = prefab19.Component;
							pathLink3.downwards.segments.Add(pathLinkSegment);
							a += vector2;
						}
						else
						{
							num12++;
						}
						if (vector.x > 0f || vector.z > 0f)
						{
							Prefab<DungeonGridLink> prefab21 = null;
							Vector3 vector9 = Vector3.zero;
							int num16 = int.MinValue;
							Vector3 position4 = Vector3.zero;
							Quaternion rotation2 = Quaternion.identity;
							GenerateDungeonGrid.PathLinkSegment prevSegment3 = pathLink3.upwards.prevSegment;
							Vector3 a3 = prevSegment3.position + prevSegment3.rotation * Vector3.Scale(prevSegment3.scale, prevSegment3.upSocket.localPosition);
							Quaternion lhs2 = prevSegment3.rotation * prevSegment3.upSocket.localRotation;
							foreach (Prefab<DungeonGridLink> prefab22 in array4)
							{
								float num17 = SeedRandom.Value(ref seed);
								DungeonGridLink component2 = prefab22.Component;
								if (prevSegment3.upType == component2.DownType)
								{
									switch (component2.DownType)
									{
									case DungeonGridLinkType.Elevator:
										if (flag3 || vector.x != 0f)
										{
											goto IL_20FD;
										}
										if (vector.z != 0f)
										{
											goto IL_20FD;
										}
										break;
									case DungeonGridLinkType.Transition:
										if (vector.x != 0f || vector.z != 0f)
										{
											goto IL_20FD;
										}
										break;
									}
									int num18 = flag3 ? 0 : component2.Priority;
									if (num16 <= num18)
									{
										Quaternion quaternion4 = lhs2 * Quaternion.Inverse(component2.DownSocket.localRotation);
										Quaternion quaternion5 = quaternion4 * component2.UpSocket.localRotation;
										GenerateDungeonGrid.PathLinkSegment prevSegment4 = pathLink3.downwards.prevSegment;
										Quaternion quaternion6 = prevSegment4.rotation * prevSegment4.downSocket.localRotation;
										if (component2.Rotation > 0)
										{
											if (Quaternion.Angle(quaternion6, quaternion5) > (float)component2.Rotation)
											{
												goto IL_20FD;
											}
											Quaternion rhs3 = quaternion6 * Quaternion.Inverse(quaternion5);
											quaternion4 *= rhs3;
											quaternion5 *= rhs3;
										}
										Vector3 vector10 = a3 - quaternion4 * component2.DownSocket.localPosition;
										Vector3 vector11 = quaternion4 * (component2.UpSocket.localPosition - component2.DownSocket.localPosition);
										Vector3 vector12 = a - vector9;
										Vector3 vector13 = a - vector11;
										float magnitude5 = vector12.magnitude;
										float magnitude6 = vector13.magnitude;
										Vector3 vector14 = Vector3.Scale(vector12, vector);
										Vector3 vector15 = Vector3.Scale(vector13, vector);
										float magnitude7 = vector14.magnitude;
										float magnitude8 = vector15.magnitude;
										if (vector9 != Vector3.zero)
										{
											if (magnitude7 < magnitude8 || (magnitude7 == magnitude8 && magnitude5 < magnitude6))
											{
												goto IL_20FD;
											}
											if (magnitude7 == magnitude8 && magnitude5 == magnitude6 && num17 < 0.5f)
											{
												goto IL_20FD;
											}
										}
										else if (magnitude7 <= magnitude8)
										{
											goto IL_20FD;
										}
										if (Mathf.Abs(vector15.x) - Mathf.Abs(vector14.x) <= 0.01f && (Mathf.Abs(vector15.x) <= 0.01f || vector12.x * vector13.x >= 0f) && Mathf.Abs(vector15.y) - Mathf.Abs(vector14.y) <= 0.01f && (Mathf.Abs(vector15.y) <= 0.01f || vector12.y * vector13.y >= 0f) && Mathf.Abs(vector15.z) - Mathf.Abs(vector14.z) <= 0.01f && (Mathf.Abs(vector15.z) <= 0.01f || vector12.z * vector13.z >= 0f) && (!flag3 || vector.x != 0f || vector.z != 0f || component2.UpType != DungeonGridLinkType.Default || ((Mathf.Abs(vector13.x) <= 0.01f || Mathf.Abs(vector13.x) >= this.LinkRadius * 2f - 0.1f) && (Mathf.Abs(vector13.z) <= 0.01f || Mathf.Abs(vector13.z) >= this.LinkRadius * 2f - 0.1f))))
										{
											num16 = num18;
											if (vector.x == 0f && vector.z == 0f)
											{
												if (!flag3 && Mathf.Abs(vector13.y) < this.LinkTransition - 0.1f)
												{
													goto IL_20FD;
												}
											}
											else if ((!flag3 && magnitude8 > 0.01f && (Mathf.Abs(vector13.x) < this.LinkRadius * 2f - 0.1f || Mathf.Abs(vector13.z) < this.LinkRadius * 2f - 0.1f)) || (!flag4 && magnitude8 > 0.01f && (Mathf.Abs(vector13.x) < this.LinkRadius * 1f - 0.1f || Mathf.Abs(vector13.z) < this.LinkRadius * 1f - 0.1f)))
											{
												goto IL_20FD;
											}
											if (!flag3 || magnitude8 >= 0.01f || magnitude6 >= 0.01f || Quaternion.Angle(quaternion6, quaternion5) <= 10f)
											{
												prefab21 = prefab22;
												vector9 = vector11;
												num16 = num18;
												position4 = vector10;
												rotation2 = quaternion4;
											}
										}
									}
								}
								IL_20FD:;
							}
							if (vector9 != Vector3.zero)
							{
								GenerateDungeonGrid.PathLinkSegment pathLinkSegment2 = new GenerateDungeonGrid.PathLinkSegment();
								pathLinkSegment2.position = position4;
								pathLinkSegment2.rotation = rotation2;
								pathLinkSegment2.scale = Vector3.one;
								pathLinkSegment2.prefab = prefab21;
								pathLinkSegment2.link = prefab21.Component;
								pathLink3.upwards.segments.Add(pathLinkSegment2);
								a -= vector9;
							}
							else
							{
								num11++;
							}
						}
						else
						{
							num11++;
						}
					}
				}
			}
			foreach (GenerateDungeonGrid.PathLink pathLink4 in list3)
			{
				foreach (GenerateDungeonGrid.PathLinkSegment pathLinkSegment3 in pathLink4.downwards.segments)
				{
					World.AddPrefab("Dungeon", pathLinkSegment3.prefab, pathLinkSegment3.position, pathLinkSegment3.rotation, pathLinkSegment3.scale);
				}
				foreach (GenerateDungeonGrid.PathLinkSegment pathLinkSegment4 in pathLink4.upwards.segments)
				{
					World.AddPrefab("Dungeon", pathLinkSegment4.prefab, pathLinkSegment4.position, pathLinkSegment4.rotation, pathLinkSegment4.scale);
				}
			}
			if (TerrainMeta.Path)
			{
				TerrainMeta.Path.DungeonGridRoot = HierarchyUtil.GetRoot("Dungeon", true, false);
			}
			return;
		}
		goto IL_7A5;
	}

	// Token: 0x0400269C RID: 9884
	public string TunnelFolder = string.Empty;

	// Token: 0x0400269D RID: 9885
	public string StationFolder = string.Empty;

	// Token: 0x0400269E RID: 9886
	public string TransitionFolder = string.Empty;

	// Token: 0x0400269F RID: 9887
	public string LinkFolder = string.Empty;

	// Token: 0x040026A0 RID: 9888
	public InfrastructureType ConnectionType = InfrastructureType.Tunnel;

	// Token: 0x040026A1 RID: 9889
	public int CellSize = 216;

	// Token: 0x040026A2 RID: 9890
	public float LinkHeight = 1.5f;

	// Token: 0x040026A3 RID: 9891
	public float LinkRadius = 3f;

	// Token: 0x040026A4 RID: 9892
	public float LinkTransition = 9f;

	// Token: 0x040026A5 RID: 9893
	private const int MaxDepth = 100000;

	// Token: 0x02000D88 RID: 3464
	private class PathNode
	{
		// Token: 0x040046DA RID: 18138
		public MonumentInfo monument;

		// Token: 0x040046DB RID: 18139
		public PathFinder.Node node;
	}

	// Token: 0x02000D89 RID: 3465
	private class PathSegment
	{
		// Token: 0x040046DC RID: 18140
		public PathFinder.Node start;

		// Token: 0x040046DD RID: 18141
		public PathFinder.Node end;
	}

	// Token: 0x02000D8A RID: 3466
	private class PathLink
	{
		// Token: 0x040046DE RID: 18142
		public GenerateDungeonGrid.PathLinkSide downwards;

		// Token: 0x040046DF RID: 18143
		public GenerateDungeonGrid.PathLinkSide upwards;
	}

	// Token: 0x02000D8B RID: 3467
	private class PathLinkSide
	{
		// Token: 0x1700065F RID: 1631
		// (get) Token: 0x06004ED0 RID: 20176 RVA: 0x0019CB0C File Offset: 0x0019AD0C
		public GenerateDungeonGrid.PathLinkSegment prevSegment
		{
			get
			{
				if (this.segments.Count <= 0)
				{
					return this.origin;
				}
				return this.segments[this.segments.Count - 1];
			}
		}

		// Token: 0x040046E0 RID: 18144
		public GenerateDungeonGrid.PathLinkSegment origin;

		// Token: 0x040046E1 RID: 18145
		public List<GenerateDungeonGrid.PathLinkSegment> segments;
	}

	// Token: 0x02000D8C RID: 3468
	private class PathLinkSegment
	{
		// Token: 0x17000660 RID: 1632
		// (get) Token: 0x06004ED2 RID: 20178 RVA: 0x0019CB3B File Offset: 0x0019AD3B
		public Transform downSocket
		{
			get
			{
				return this.link.DownSocket;
			}
		}

		// Token: 0x17000661 RID: 1633
		// (get) Token: 0x06004ED3 RID: 20179 RVA: 0x0019CB48 File Offset: 0x0019AD48
		public Transform upSocket
		{
			get
			{
				return this.link.UpSocket;
			}
		}

		// Token: 0x17000662 RID: 1634
		// (get) Token: 0x06004ED4 RID: 20180 RVA: 0x0019CB55 File Offset: 0x0019AD55
		public DungeonGridLinkType downType
		{
			get
			{
				return this.link.DownType;
			}
		}

		// Token: 0x17000663 RID: 1635
		// (get) Token: 0x06004ED5 RID: 20181 RVA: 0x0019CB62 File Offset: 0x0019AD62
		public DungeonGridLinkType upType
		{
			get
			{
				return this.link.UpType;
			}
		}

		// Token: 0x040046E2 RID: 18146
		public Vector3 position;

		// Token: 0x040046E3 RID: 18147
		public Quaternion rotation;

		// Token: 0x040046E4 RID: 18148
		public Vector3 scale;

		// Token: 0x040046E5 RID: 18149
		public Prefab<DungeonGridLink> prefab;

		// Token: 0x040046E6 RID: 18150
		public DungeonGridLink link;
	}
}
