using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000695 RID: 1685
public class GenerateRailRing : ProceduralComponent
{
	// Token: 0x06003002 RID: 12290 RVA: 0x00122DA0 File Offset: 0x00120FA0
	public override void Process(uint seed)
	{
		if (World.Networked)
		{
			TerrainMeta.Path.Rails.Clear();
			TerrainMeta.Path.Rails.AddRange(World.GetPaths("Rail"));
			return;
		}
		if ((ulong)World.Size < (ulong)((long)this.MinWorldSize))
		{
			return;
		}
		int[,] array = TerrainPath.CreateRailCostmap(ref seed);
		PathFinder pathFinder = new PathFinder(array, true, true);
		int length = array.GetLength(0);
		int num = length / 4;
		int num2 = 1;
		int stepcount = num / num2;
		int num3 = length / 2;
		int pos_x = num;
		int pos_x2 = length - num;
		int pos_y = num;
		int pos_y2 = length - num;
		int num4 = 0;
		int dir_x = -num2;
		int dir_x2 = num2;
		int dir_y = -num2;
		int dir_y2 = num2;
		List<GenerateRailRing.RingNode> list2;
		if (World.Size < 5000U)
		{
			List<GenerateRailRing.RingNode> list = new List<GenerateRailRing.RingNode>();
			list.Add(new GenerateRailRing.RingNode(pos_x2, pos_y2, dir_x, dir_y, stepcount));
			list.Add(new GenerateRailRing.RingNode(pos_x2, pos_y, dir_x, dir_y2, stepcount));
			list.Add(new GenerateRailRing.RingNode(pos_x, pos_y, dir_x2, dir_y2, stepcount));
			list2 = list;
			list.Add(new GenerateRailRing.RingNode(pos_x, pos_y2, dir_x2, dir_y, stepcount));
		}
		else
		{
			List<GenerateRailRing.RingNode> list3 = new List<GenerateRailRing.RingNode>();
			list3.Add(new GenerateRailRing.RingNode(num3, pos_y2, num4, dir_y, stepcount));
			list3.Add(new GenerateRailRing.RingNode(pos_x2, pos_y2, dir_x, dir_y, stepcount));
			list3.Add(new GenerateRailRing.RingNode(pos_x2, num3, dir_x, num4, stepcount));
			list3.Add(new GenerateRailRing.RingNode(pos_x2, pos_y, dir_x, dir_y2, stepcount));
			list3.Add(new GenerateRailRing.RingNode(num3, pos_y, num4, dir_y2, stepcount));
			list3.Add(new GenerateRailRing.RingNode(pos_x, pos_y, dir_x2, dir_y2, stepcount));
			list3.Add(new GenerateRailRing.RingNode(pos_x, num3, dir_x2, num4, stepcount));
			list2 = list3;
			list3.Add(new GenerateRailRing.RingNode(pos_x, pos_y2, dir_x2, dir_y, stepcount));
		}
		List<GenerateRailRing.RingNode> list4 = list2;
		for (int i = 0; i < list4.Count; i++)
		{
			GenerateRailRing.RingNode ringNode = list4[i];
			GenerateRailRing.RingNode next = list4[(i + 1) % list4.Count];
			GenerateRailRing.RingNode prev = list4[(i - 1 + list4.Count) % list4.Count];
			ringNode.next = next;
			ringNode.prev = prev;
			while (!pathFinder.IsWalkableWithNeighbours(ringNode.position))
			{
				if (ringNode.attempts <= 0)
				{
					return;
				}
				ringNode.position += ringNode.direction;
				ringNode.attempts--;
			}
		}
		foreach (GenerateRailRing.RingNode ringNode2 in list4)
		{
			ringNode2.path = pathFinder.FindPath(ringNode2.position, ringNode2.next.position, 250000);
		}
		bool flag = false;
		IL_43E:
		while (!flag)
		{
			flag = true;
			PathFinder.Point point = new PathFinder.Point(0, 0);
			foreach (GenerateRailRing.RingNode ringNode3 in list4)
			{
				point += ringNode3.position;
			}
			point /= list4.Count;
			float num5 = float.MinValue;
			GenerateRailRing.RingNode ringNode4 = null;
			foreach (GenerateRailRing.RingNode ringNode5 in list4)
			{
				if (ringNode5.path == null)
				{
					float num6 = new Vector2((float)(ringNode5.position.x - point.x), (float)(ringNode5.position.y - point.y)).magnitude;
					if (ringNode5.prev.path == null)
					{
						num6 *= 1.5f;
					}
					if (num6 > num5)
					{
						num5 = num6;
						ringNode4 = ringNode5;
					}
				}
			}
			if (ringNode4 != null)
			{
				while (ringNode4.attempts > 0)
				{
					ringNode4.position += ringNode4.direction;
					ringNode4.attempts--;
					if (pathFinder.IsWalkableWithNeighbours(ringNode4.position))
					{
						ringNode4.path = pathFinder.FindPath(ringNode4.position, ringNode4.next.position, 250000);
						ringNode4.prev.path = pathFinder.FindPath(ringNode4.prev.position, ringNode4.position, 250000);
						flag = false;
						goto IL_43E;
					}
				}
				return;
			}
		}
		if (flag)
		{
			for (int j = 0; j < list4.Count; j++)
			{
				GenerateRailRing.RingNode ringNode6 = list4[j];
				GenerateRailRing.RingNode ringNode7 = list4[(j + 1) % list4.Count];
				PathFinder.Node node = null;
				PathFinder.Node node2 = null;
				for (PathFinder.Node node3 = ringNode6.path; node3 != null; node3 = node3.next)
				{
					for (PathFinder.Node node4 = ringNode7.path; node4 != null; node4 = node4.next)
					{
						int num7 = Mathf.Abs(node3.point.x - node4.point.x);
						int num8 = Mathf.Abs(node3.point.y - node4.point.y);
						if (num7 <= 15 && num8 <= 15)
						{
							if (node == null || node3.cost > node.cost)
							{
								node = node3;
							}
							if (node2 == null || node4.cost < node2.cost)
							{
								node2 = node4;
							}
						}
					}
				}
				if (node != null && node2 != null)
				{
					PathFinder.Node node5 = pathFinder.FindPath(node.point, node2.point, 250000);
					if (node5 != null && node5.next != null)
					{
						node.next = node5.next;
						ringNode7.path = node2;
					}
				}
			}
			for (int k = 0; k < list4.Count; k++)
			{
				GenerateRailRing.RingNode ringNode8 = list4[k];
				GenerateRailRing.RingNode ringNode9 = list4[(k + 1) % list4.Count];
				PathFinder.Node node6 = null;
				PathFinder.Node node7 = null;
				for (PathFinder.Node node8 = ringNode8.path; node8 != null; node8 = node8.next)
				{
					for (PathFinder.Node node9 = ringNode9.path; node9 != null; node9 = node9.next)
					{
						int num9 = Mathf.Abs(node8.point.x - node9.point.x);
						int num10 = Mathf.Abs(node8.point.y - node9.point.y);
						if (num9 <= 1 && num10 <= 1)
						{
							if (node6 == null || node8.cost > node6.cost)
							{
								node6 = node8;
							}
							if (node7 == null || node9.cost < node7.cost)
							{
								node7 = node9;
							}
						}
					}
				}
				if (node6 != null && node7 != null)
				{
					node6.next = null;
					ringNode9.path = node7;
				}
			}
			PathFinder.Node node10 = null;
			PathFinder.Node node11 = null;
			foreach (GenerateRailRing.RingNode ringNode10 in list4)
			{
				if (node10 == null)
				{
					node10 = ringNode10.path;
					node11 = ringNode10.path;
				}
				else
				{
					node11.next = ringNode10.path;
				}
				while (node11.next != null)
				{
					node11 = node11.next;
				}
			}
			node11.next = new PathFinder.Node(node10.point, node10.cost, node10.heuristic, null);
			List<Vector3> list5 = new List<Vector3>();
			for (PathFinder.Node node12 = node10; node12 != null; node12 = node12.next)
			{
				float normX = ((float)node12.point.x + 0.5f) / (float)length;
				float normZ = ((float)node12.point.y + 0.5f) / (float)length;
				float x = TerrainMeta.DenormalizeX(normX);
				float z = TerrainMeta.DenormalizeZ(normZ);
				float y = Mathf.Max(TerrainMeta.HeightMap.GetHeight(normX, normZ), 1f);
				list5.Add(new Vector3(x, y, z));
			}
			if (list5.Count >= 2)
			{
				int count = TerrainMeta.Path.Rails.Count;
				PathList pathList = new PathList("Rail " + count, list5.ToArray());
				pathList.Spline = true;
				pathList.Width = 4f;
				pathList.InnerPadding = 1f;
				pathList.OuterPadding = 1f;
				pathList.InnerFade = 1f;
				pathList.OuterFade = 32f;
				pathList.RandomScale = 1f;
				pathList.MeshOffset = 0f;
				pathList.TerrainOffset = -0.125f;
				pathList.Topology = 524288;
				pathList.Splat = 128;
				pathList.Start = false;
				pathList.End = false;
				pathList.ProcgenStartNode = node10;
				pathList.ProcgenEndNode = node11;
				pathList.Path.Smoothen(32, new Vector3(1f, 0f, 1f), null);
				pathList.Path.Smoothen(64, new Vector3(0f, 1f, 0f), null);
				pathList.Path.Resample(7.5f);
				pathList.Path.RecalculateTangents();
				pathList.AdjustPlacementMap(20f);
				TerrainMeta.Path.Rails.Add(pathList);
			}
		}
	}

	// Token: 0x040026C1 RID: 9921
	public const float Width = 4f;

	// Token: 0x040026C2 RID: 9922
	public const float InnerPadding = 1f;

	// Token: 0x040026C3 RID: 9923
	public const float OuterPadding = 1f;

	// Token: 0x040026C4 RID: 9924
	public const float InnerFade = 1f;

	// Token: 0x040026C5 RID: 9925
	public const float OuterFade = 32f;

	// Token: 0x040026C6 RID: 9926
	public const float RandomScale = 1f;

	// Token: 0x040026C7 RID: 9927
	public const float MeshOffset = 0f;

	// Token: 0x040026C8 RID: 9928
	public const float TerrainOffset = -0.125f;

	// Token: 0x040026C9 RID: 9929
	private const int MaxDepth = 250000;

	// Token: 0x040026CA RID: 9930
	public int MinWorldSize;

	// Token: 0x02000DA0 RID: 3488
	private class RingNode
	{
		// Token: 0x06004F01 RID: 20225 RVA: 0x0019CFF1 File Offset: 0x0019B1F1
		public RingNode(int pos_x, int pos_y, int dir_x, int dir_y, int stepcount)
		{
			this.position = new PathFinder.Point(pos_x, pos_y);
			this.direction = new PathFinder.Point(dir_x, dir_y);
			this.attempts = stepcount;
		}

		// Token: 0x0400470B RID: 18187
		public int attempts;

		// Token: 0x0400470C RID: 18188
		public PathFinder.Point position;

		// Token: 0x0400470D RID: 18189
		public PathFinder.Point direction;

		// Token: 0x0400470E RID: 18190
		public GenerateRailRing.RingNode next;

		// Token: 0x0400470F RID: 18191
		public GenerateRailRing.RingNode prev;

		// Token: 0x04004710 RID: 18192
		public PathFinder.Node path;
	}
}
