using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200067A RID: 1658
public class TerrainPath : TerrainExtension
{
	// Token: 0x06002F8E RID: 12174 RVA: 0x0011BC20 File Offset: 0x00119E20
	public override void PostSetup()
	{
		foreach (PathList pathList in this.Roads)
		{
			pathList.ProcgenStartNode = null;
			pathList.ProcgenEndNode = null;
		}
		foreach (PathList pathList2 in this.Rails)
		{
			pathList2.ProcgenStartNode = null;
			pathList2.ProcgenEndNode = null;
		}
		foreach (PathList pathList3 in this.Rivers)
		{
			pathList3.ProcgenStartNode = null;
			pathList3.ProcgenEndNode = null;
		}
		foreach (PathList pathList4 in this.Powerlines)
		{
			pathList4.ProcgenStartNode = null;
			pathList4.ProcgenEndNode = null;
		}
	}

	// Token: 0x06002F8F RID: 12175 RVA: 0x0011BD50 File Offset: 0x00119F50
	public void Clear()
	{
		this.Roads.Clear();
		this.Rails.Clear();
		this.Rivers.Clear();
		this.Powerlines.Clear();
	}

	// Token: 0x06002F90 RID: 12176 RVA: 0x0011BD80 File Offset: 0x00119F80
	public T FindClosest<T>(List<T> list, Vector3 pos) where T : MonoBehaviour
	{
		T result = default(T);
		float num = float.MaxValue;
		foreach (T t in list)
		{
			float num2 = Vector3Ex.Distance2D(t.transform.position, pos);
			if (num2 < num)
			{
				result = t;
				num = num2;
			}
		}
		return result;
	}

	// Token: 0x06002F91 RID: 12177 RVA: 0x0011BDF8 File Offset: 0x00119FF8
	public static int[,] CreatePowerlineCostmap(ref uint seed)
	{
		float radius = 5f;
		int num = (int)(World.Size / 7.5f);
		TerrainPlacementMap placementMap = TerrainMeta.PlacementMap;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		int[,] array = new int[num, num];
		for (int i = 0; i < num; i++)
		{
			float normZ = ((float)i + 0.5f) / (float)num;
			for (int j = 0; j < num; j++)
			{
				float normX = ((float)j + 0.5f) / (float)num;
				float slope = heightMap.GetSlope(normX, normZ);
				int topology = topologyMap.GetTopology(normX, normZ, radius);
				int num2 = 2295174;
				int num3 = 1628160;
				int num4 = 512;
				if ((topology & num2) != 0)
				{
					array[j, i] = int.MaxValue;
				}
				else if ((topology & num3) != 0 || placementMap.GetBlocked(normX, normZ, radius))
				{
					array[j, i] = 2500;
				}
				else if ((topology & num4) != 0)
				{
					array[j, i] = 1000;
				}
				else
				{
					array[j, i] = 1 + (int)(slope * slope * 10f);
				}
			}
		}
		return array;
	}

	// Token: 0x06002F92 RID: 12178 RVA: 0x0011BF20 File Offset: 0x0011A120
	public static int[,] CreateRoadCostmap(ref uint seed)
	{
		float radius = 5f;
		int num = (int)(World.Size / 7.5f);
		TerrainPlacementMap placementMap = TerrainMeta.PlacementMap;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		int[,] array = new int[num, num];
		for (int i = 0; i < num; i++)
		{
			float normZ = ((float)i + 0.5f) / (float)num;
			for (int j = 0; j < num; j++)
			{
				float normX = ((float)j + 0.5f) / (float)num;
				int num2 = SeedRandom.Range(ref seed, 100, 200);
				float slope = heightMap.GetSlope(normX, normZ);
				int topology = topologyMap.GetTopology(normX, normZ, radius);
				int num3 = 2295686;
				int num4 = 49152;
				if (slope > 20f || (topology & num3) != 0)
				{
					array[j, i] = int.MaxValue;
				}
				else if ((topology & num4) != 0 || placementMap.GetBlocked(normX, normZ, radius))
				{
					array[j, i] = 5000;
				}
				else
				{
					array[j, i] = 1 + (int)(slope * slope * 10f) + num2;
				}
			}
		}
		return array;
	}

	// Token: 0x06002F93 RID: 12179 RVA: 0x0011C044 File Offset: 0x0011A244
	public static int[,] CreateRailCostmap(ref uint seed)
	{
		float radius = 5f;
		int num = (int)(World.Size / 7.5f);
		TerrainPlacementMap placementMap = TerrainMeta.PlacementMap;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		int[,] array = new int[num, num];
		for (int i = 0; i < num; i++)
		{
			float normZ = ((float)i + 0.5f) / (float)num;
			for (int j = 0; j < num; j++)
			{
				float normX = ((float)j + 0.5f) / (float)num;
				float slope = heightMap.GetSlope(normX, normZ);
				int topology = topologyMap.GetTopology(normX, normZ, radius);
				int num2 = 2295686;
				int num3 = 49152;
				if (slope > 20f || (topology & num2) != 0)
				{
					array[j, i] = int.MaxValue;
				}
				else if ((topology & num3) != 0 || placementMap.GetBlocked(normX, normZ, radius))
				{
					array[j, i] = 5000;
				}
				else if (slope > 10f)
				{
					array[j, i] = 1500;
				}
				else
				{
					array[j, i] = 1000;
				}
			}
		}
		return array;
	}

	// Token: 0x06002F94 RID: 12180 RVA: 0x0011C168 File Offset: 0x0011A368
	public static int[,] CreateBoatCostmap(float depth)
	{
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainWaterMap waterMap = TerrainMeta.WaterMap;
		int res = heightMap.res;
		int[,] array = new int[res, res];
		for (int i = 0; i < res; i++)
		{
			float normZ = ((float)i + 0.5f) / (float)res;
			for (int j = 0; j < res; j++)
			{
				float normX = ((float)j + 0.5f) / (float)res;
				float height = heightMap.GetHeight(normX, normZ);
				if (waterMap.GetHeight(normX, normZ) - height < depth)
				{
					array[j, i] = int.MaxValue;
				}
				else
				{
					array[j, i] = 1;
				}
			}
		}
		return array;
	}

	// Token: 0x06002F95 RID: 12181 RVA: 0x0011C208 File Offset: 0x0011A408
	public void AddWire(PowerlineNode node)
	{
		string name = node.transform.root.name;
		if (!this.wires.ContainsKey(name))
		{
			this.wires.Add(name, new List<PowerlineNode>());
		}
		this.wires[name].Add(node);
	}

	// Token: 0x06002F96 RID: 12182 RVA: 0x0011C258 File Offset: 0x0011A458
	public void CreateWires()
	{
		List<GameObject> list = new List<GameObject>();
		int num = 0;
		GameObjectRef gameObjectRef = null;
		foreach (KeyValuePair<string, List<PowerlineNode>> keyValuePair in this.wires)
		{
			foreach (PowerlineNode powerlineNode in keyValuePair.Value)
			{
				PowerLineWireConnectionHelper component = powerlineNode.GetComponent<PowerLineWireConnectionHelper>();
				if (component)
				{
					if (list.Count == 0)
					{
						gameObjectRef = powerlineNode.WirePrefab;
						num = component.connections.Count;
					}
					else
					{
						GameObject gameObject = list[list.Count - 1];
						if (powerlineNode.WirePrefab.guid != ((gameObjectRef != null) ? gameObjectRef.guid : null) || component.connections.Count != num || (gameObject.transform.position - powerlineNode.transform.position).sqrMagnitude > powerlineNode.MaxDistance * powerlineNode.MaxDistance)
						{
							this.CreateWire(keyValuePair.Key, list, gameObjectRef);
							list.Clear();
						}
					}
					list.Add(powerlineNode.gameObject);
				}
			}
			this.CreateWire(keyValuePair.Key, list, gameObjectRef);
			list.Clear();
		}
	}

	// Token: 0x06002F97 RID: 12183 RVA: 0x0011C3F4 File Offset: 0x0011A5F4
	private void CreateWire(string name, List<GameObject> objects, GameObjectRef wirePrefab)
	{
		if (objects.Count < 3 || wirePrefab == null || !wirePrefab.isValid)
		{
			return;
		}
		PowerLineWire powerLineWire = PowerLineWire.Create(null, objects, wirePrefab, "Powerline Wires", null, 1f, 0.1f);
		if (powerLineWire)
		{
			powerLineWire.enabled = false;
			powerLineWire.gameObject.SetHierarchyGroup(name, true, false);
		}
	}

	// Token: 0x06002F98 RID: 12184 RVA: 0x0011C44C File Offset: 0x0011A64C
	public MonumentInfo FindMonumentWithBoundsOverlap(Vector3 position)
	{
		if (TerrainMeta.Path.Monuments == null)
		{
			return null;
		}
		foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
		{
			if (monumentInfo != null && monumentInfo.IsInBounds(position))
			{
				return monumentInfo;
			}
		}
		return null;
	}

	// Token: 0x04002655 RID: 9813
	internal List<PathList> Roads = new List<PathList>();

	// Token: 0x04002656 RID: 9814
	internal List<PathList> Rails = new List<PathList>();

	// Token: 0x04002657 RID: 9815
	internal List<PathList> Rivers = new List<PathList>();

	// Token: 0x04002658 RID: 9816
	internal List<PathList> Powerlines = new List<PathList>();

	// Token: 0x04002659 RID: 9817
	internal List<LandmarkInfo> Landmarks = new List<LandmarkInfo>();

	// Token: 0x0400265A RID: 9818
	internal List<MonumentInfo> Monuments = new List<MonumentInfo>();

	// Token: 0x0400265B RID: 9819
	internal List<RiverInfo> RiverObjs = new List<RiverInfo>();

	// Token: 0x0400265C RID: 9820
	internal List<LakeInfo> LakeObjs = new List<LakeInfo>();

	// Token: 0x0400265D RID: 9821
	internal GameObject DungeonGridRoot;

	// Token: 0x0400265E RID: 9822
	internal List<DungeonGridInfo> DungeonGridEntrances = new List<DungeonGridInfo>();

	// Token: 0x0400265F RID: 9823
	internal List<DungeonGridCell> DungeonGridCells = new List<DungeonGridCell>();

	// Token: 0x04002660 RID: 9824
	internal GameObject DungeonBaseRoot;

	// Token: 0x04002661 RID: 9825
	internal List<DungeonBaseInfo> DungeonBaseEntrances = new List<DungeonBaseInfo>();

	// Token: 0x04002662 RID: 9826
	internal List<Vector3> OceanPatrolClose = new List<Vector3>();

	// Token: 0x04002663 RID: 9827
	internal List<Vector3> OceanPatrolFar = new List<Vector3>();

	// Token: 0x04002664 RID: 9828
	private Dictionary<string, List<PowerlineNode>> wires = new Dictionary<string, List<PowerlineNode>>();
}
