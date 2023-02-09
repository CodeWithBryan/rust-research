using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000916 RID: 2326
public class PathFinder
{
	// Token: 0x0600374B RID: 14155 RVA: 0x001482CC File Offset: 0x001464CC
	public PathFinder(int[,] costmap, bool diagonals = true, bool directional = true)
	{
		this.costmap = costmap;
		this.neighbors = (diagonals ? PathFinder.mooreNeighbors : PathFinder.neumannNeighbors);
		this.directional = directional;
	}

	// Token: 0x0600374C RID: 14156 RVA: 0x001482F7 File Offset: 0x001464F7
	public int GetResolution(int index)
	{
		return this.costmap.GetLength(index);
	}

	// Token: 0x0600374D RID: 14157 RVA: 0x00148305 File Offset: 0x00146505
	public PathFinder.Node FindPath(PathFinder.Point start, PathFinder.Point end, int depth = 2147483647)
	{
		return this.FindPathReversed(end, start, depth);
	}

	// Token: 0x0600374E RID: 14158 RVA: 0x00148310 File Offset: 0x00146510
	private PathFinder.Node FindPathReversed(PathFinder.Point start, PathFinder.Point end, int depth = 2147483647)
	{
		if (this.visited == null)
		{
			this.visited = new int[this.costmap.GetLength(0), this.costmap.GetLength(1)];
		}
		else
		{
			Array.Clear(this.visited, 0, this.visited.Length);
		}
		int num = 0;
		int num2 = this.costmap.GetLength(0) - 1;
		int num3 = 0;
		int num4 = this.costmap.GetLength(1) - 1;
		IntrusiveMinHeap<PathFinder.Node> intrusiveMinHeap = default(IntrusiveMinHeap<PathFinder.Node>);
		int num5 = this.Cost(start);
		int heuristic = this.Heuristic(start, end);
		intrusiveMinHeap.Add(new PathFinder.Node(start, num5, heuristic, null));
		this.visited[start.x, start.y] = num5;
		while (!intrusiveMinHeap.Empty && depth-- > 0)
		{
			PathFinder.Node node = intrusiveMinHeap.Pop();
			if (node.heuristic == 0)
			{
				return node;
			}
			for (int i = 0; i < this.neighbors.Length; i++)
			{
				PathFinder.Point point = node.point + this.neighbors[i];
				if (point.x >= num && point.x <= num2 && point.y >= num3 && point.y <= num4)
				{
					int num6 = this.Cost(point, node);
					if (num6 != 2147483647)
					{
						int num7 = this.visited[point.x, point.y];
						if (num7 == 0 || num6 < num7)
						{
							int cost = node.cost + num6;
							int heuristic2 = this.Heuristic(point, end);
							intrusiveMinHeap.Add(new PathFinder.Node(point, cost, heuristic2, node));
							this.visited[point.x, point.y] = num6;
						}
					}
					else
					{
						this.visited[point.x, point.y] = -1;
					}
				}
			}
		}
		return null;
	}

	// Token: 0x0600374F RID: 14159 RVA: 0x00148501 File Offset: 0x00146701
	public PathFinder.Node FindPathDirected(List<PathFinder.Point> startList, List<PathFinder.Point> endList, int depth = 2147483647)
	{
		if (startList.Count == 0 || endList.Count == 0)
		{
			return null;
		}
		return this.FindPathReversed(endList, startList, depth);
	}

	// Token: 0x06003750 RID: 14160 RVA: 0x0014851E File Offset: 0x0014671E
	public PathFinder.Node FindPathUndirected(List<PathFinder.Point> startList, List<PathFinder.Point> endList, int depth = 2147483647)
	{
		if (startList.Count == 0 || endList.Count == 0)
		{
			return null;
		}
		if (startList.Count > endList.Count)
		{
			return this.FindPathReversed(endList, startList, depth);
		}
		return this.FindPathReversed(startList, endList, depth);
	}

	// Token: 0x06003751 RID: 14161 RVA: 0x00148554 File Offset: 0x00146754
	private PathFinder.Node FindPathReversed(List<PathFinder.Point> startList, List<PathFinder.Point> endList, int depth = 2147483647)
	{
		if (this.visited == null)
		{
			this.visited = new int[this.costmap.GetLength(0), this.costmap.GetLength(1)];
		}
		else
		{
			Array.Clear(this.visited, 0, this.visited.Length);
		}
		int num = 0;
		int num2 = this.costmap.GetLength(0) - 1;
		int num3 = 0;
		int num4 = this.costmap.GetLength(1) - 1;
		IntrusiveMinHeap<PathFinder.Node> intrusiveMinHeap = default(IntrusiveMinHeap<PathFinder.Node>);
		using (List<PathFinder.Point>.Enumerator enumerator = startList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				PathFinder.Point point = enumerator.Current;
				int num5 = this.Cost(point);
				int heuristic = this.Heuristic(point, endList);
				intrusiveMinHeap.Add(new PathFinder.Node(point, num5, heuristic, null));
				this.visited[point.x, point.y] = num5;
			}
			goto IL_1FD;
		}
		IL_E0:
		PathFinder.Node node = intrusiveMinHeap.Pop();
		if (node.heuristic == 0)
		{
			return node;
		}
		for (int i = 0; i < this.neighbors.Length; i++)
		{
			PathFinder.Point point2 = node.point + this.neighbors[i];
			if (point2.x >= num && point2.x <= num2 && point2.y >= num3 && point2.y <= num4)
			{
				int num6 = this.Cost(point2, node);
				if (num6 != 2147483647)
				{
					int num7 = this.visited[point2.x, point2.y];
					if (num7 == 0 || num6 < num7)
					{
						int cost = node.cost + num6;
						int heuristic2 = this.Heuristic(point2, endList);
						intrusiveMinHeap.Add(new PathFinder.Node(point2, cost, heuristic2, node));
						this.visited[point2.x, point2.y] = num6;
					}
				}
				else
				{
					this.visited[point2.x, point2.y] = -1;
				}
			}
		}
		IL_1FD:
		if (intrusiveMinHeap.Empty || depth-- <= 0)
		{
			return null;
		}
		goto IL_E0;
	}

	// Token: 0x06003752 RID: 14162 RVA: 0x00148784 File Offset: 0x00146984
	public PathFinder.Node FindClosestWalkable(PathFinder.Point start, int depth = 2147483647)
	{
		if (this.visited == null)
		{
			this.visited = new int[this.costmap.GetLength(0), this.costmap.GetLength(1)];
		}
		else
		{
			Array.Clear(this.visited, 0, this.visited.Length);
		}
		int num = 0;
		int num2 = this.costmap.GetLength(0) - 1;
		int num3 = 0;
		int num4 = this.costmap.GetLength(1) - 1;
		if (start.x < num)
		{
			return null;
		}
		if (start.x > num2)
		{
			return null;
		}
		if (start.y < num3)
		{
			return null;
		}
		if (start.y > num4)
		{
			return null;
		}
		IntrusiveMinHeap<PathFinder.Node> intrusiveMinHeap = default(IntrusiveMinHeap<PathFinder.Node>);
		int num5 = 1;
		int heuristic = this.Heuristic(start);
		intrusiveMinHeap.Add(new PathFinder.Node(start, num5, heuristic, null));
		this.visited[start.x, start.y] = num5;
		while (!intrusiveMinHeap.Empty && depth-- > 0)
		{
			PathFinder.Node node = intrusiveMinHeap.Pop();
			if (node.heuristic == 0)
			{
				return node;
			}
			for (int i = 0; i < this.neighbors.Length; i++)
			{
				PathFinder.Point point = node.point + this.neighbors[i];
				if (point.x >= num && point.x <= num2 && point.y >= num3 && point.y <= num4)
				{
					int num6 = 1;
					if (this.visited[point.x, point.y] == 0)
					{
						int cost = node.cost + num6;
						int heuristic2 = this.Heuristic(point);
						intrusiveMinHeap.Add(new PathFinder.Node(point, cost, heuristic2, node));
						this.visited[point.x, point.y] = num6;
					}
				}
			}
		}
		return null;
	}

	// Token: 0x06003753 RID: 14163 RVA: 0x00148958 File Offset: 0x00146B58
	public bool IsWalkable(PathFinder.Point point)
	{
		return this.costmap[point.x, point.y] != int.MaxValue;
	}

	// Token: 0x06003754 RID: 14164 RVA: 0x0014897C File Offset: 0x00146B7C
	public bool IsWalkableWithNeighbours(PathFinder.Point point)
	{
		if (this.costmap[point.x, point.y] == 2147483647)
		{
			return false;
		}
		for (int i = 0; i < this.neighbors.Length; i++)
		{
			PathFinder.Point point2 = point + this.neighbors[i];
			if (this.costmap[point2.x, point2.y] == 2147483647)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06003755 RID: 14165 RVA: 0x001489F0 File Offset: 0x00146BF0
	public PathFinder.Node Reverse(PathFinder.Node start)
	{
		PathFinder.Node node = null;
		PathFinder.Node next = null;
		for (PathFinder.Node node2 = start; node2 != null; node2 = node2.next)
		{
			if (node != null)
			{
				node.next = next;
			}
			next = node;
			node = node2;
		}
		if (node != null)
		{
			node.next = next;
		}
		return node;
	}

	// Token: 0x06003756 RID: 14166 RVA: 0x00148A28 File Offset: 0x00146C28
	public PathFinder.Node FindEnd(PathFinder.Node start)
	{
		for (PathFinder.Node node = start; node != null; node = node.next)
		{
			if (node.next == null)
			{
				return node;
			}
		}
		return start;
	}

	// Token: 0x06003757 RID: 14167 RVA: 0x00148A50 File Offset: 0x00146C50
	public int Cost(PathFinder.Point a)
	{
		int num = this.costmap[a.x, a.y];
		int num2 = 0;
		if (num != 2147483647 && this.PushMultiplier > 0)
		{
			int num3 = Mathf.Max(0, this.Heuristic(a, this.PushPoint) - this.PushRadius * this.PushRadius);
			int num4 = Mathf.Max(0, this.PushDistance * this.PushDistance - num3);
			num2 = this.PushMultiplier * num4;
		}
		return num + num2;
	}

	// Token: 0x06003758 RID: 14168 RVA: 0x00148ACC File Offset: 0x00146CCC
	public int Cost(PathFinder.Point a, PathFinder.Node neighbour)
	{
		int num = this.Cost(a);
		int num2 = 0;
		if (num != 2147483647 && this.directional && neighbour != null && neighbour.next != null && this.Heuristic(a, neighbour.next.point) <= 2)
		{
			num2 = 10000;
		}
		return num + num2;
	}

	// Token: 0x06003759 RID: 14169 RVA: 0x00148B1A File Offset: 0x00146D1A
	public int Heuristic(PathFinder.Point a)
	{
		if (this.costmap[a.x, a.y] != 2147483647)
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x0600375A RID: 14170 RVA: 0x00148B40 File Offset: 0x00146D40
	public int Heuristic(PathFinder.Point a, PathFinder.Point b)
	{
		int num = a.x - b.x;
		int num2 = a.y - b.y;
		return num * num + num2 * num2;
	}

	// Token: 0x0600375B RID: 14171 RVA: 0x00148B70 File Offset: 0x00146D70
	public int Heuristic(PathFinder.Point a, List<PathFinder.Point> b)
	{
		int num = int.MaxValue;
		for (int i = 0; i < b.Count; i++)
		{
			num = Mathf.Min(num, this.Heuristic(a, b[i]));
		}
		return num;
	}

	// Token: 0x0600375C RID: 14172 RVA: 0x00148BAC File Offset: 0x00146DAC
	public float Distance(PathFinder.Point a, PathFinder.Point b)
	{
		int num = a.x - b.x;
		int num2 = a.y - b.y;
		return Mathf.Sqrt((float)(num * num + num2 * num2));
	}

	// Token: 0x040031B9 RID: 12729
	private int[,] costmap;

	// Token: 0x040031BA RID: 12730
	private int[,] visited;

	// Token: 0x040031BB RID: 12731
	private PathFinder.Point[] neighbors;

	// Token: 0x040031BC RID: 12732
	private bool directional;

	// Token: 0x040031BD RID: 12733
	public PathFinder.Point PushPoint;

	// Token: 0x040031BE RID: 12734
	public int PushRadius;

	// Token: 0x040031BF RID: 12735
	public int PushDistance;

	// Token: 0x040031C0 RID: 12736
	public int PushMultiplier;

	// Token: 0x040031C1 RID: 12737
	private static PathFinder.Point[] mooreNeighbors = new PathFinder.Point[]
	{
		new PathFinder.Point(0, 1),
		new PathFinder.Point(-1, 0),
		new PathFinder.Point(1, 0),
		new PathFinder.Point(0, -1),
		new PathFinder.Point(-1, 1),
		new PathFinder.Point(1, 1),
		new PathFinder.Point(-1, -1),
		new PathFinder.Point(1, -1)
	};

	// Token: 0x040031C2 RID: 12738
	private static PathFinder.Point[] neumannNeighbors = new PathFinder.Point[]
	{
		new PathFinder.Point(0, 1),
		new PathFinder.Point(-1, 0),
		new PathFinder.Point(1, 0),
		new PathFinder.Point(0, -1)
	};

	// Token: 0x02000E61 RID: 3681
	public struct Point : IEquatable<PathFinder.Point>
	{
		// Token: 0x06005069 RID: 20585 RVA: 0x001A19A0 File Offset: 0x0019FBA0
		public Point(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		// Token: 0x0600506A RID: 20586 RVA: 0x001A19B0 File Offset: 0x0019FBB0
		public static PathFinder.Point operator +(PathFinder.Point a, PathFinder.Point b)
		{
			return new PathFinder.Point(a.x + b.x, a.y + b.y);
		}

		// Token: 0x0600506B RID: 20587 RVA: 0x001A19D1 File Offset: 0x0019FBD1
		public static PathFinder.Point operator -(PathFinder.Point a, PathFinder.Point b)
		{
			return new PathFinder.Point(a.x - b.x, a.y - b.y);
		}

		// Token: 0x0600506C RID: 20588 RVA: 0x001A19F2 File Offset: 0x0019FBF2
		public static PathFinder.Point operator *(PathFinder.Point p, int i)
		{
			return new PathFinder.Point(p.x * i, p.y * i);
		}

		// Token: 0x0600506D RID: 20589 RVA: 0x001A1A09 File Offset: 0x0019FC09
		public static PathFinder.Point operator /(PathFinder.Point p, int i)
		{
			return new PathFinder.Point(p.x / i, p.y / i);
		}

		// Token: 0x0600506E RID: 20590 RVA: 0x001A1A20 File Offset: 0x0019FC20
		public static bool operator ==(PathFinder.Point a, PathFinder.Point b)
		{
			return a.Equals(b);
		}

		// Token: 0x0600506F RID: 20591 RVA: 0x001A1A2A File Offset: 0x0019FC2A
		public static bool operator !=(PathFinder.Point a, PathFinder.Point b)
		{
			return !a.Equals(b);
		}

		// Token: 0x06005070 RID: 20592 RVA: 0x001A1A37 File Offset: 0x0019FC37
		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.y.GetHashCode();
		}

		// Token: 0x06005071 RID: 20593 RVA: 0x001A1A50 File Offset: 0x0019FC50
		public override bool Equals(object other)
		{
			return other is PathFinder.Point && this.Equals((PathFinder.Point)other);
		}

		// Token: 0x06005072 RID: 20594 RVA: 0x001A1A68 File Offset: 0x0019FC68
		public bool Equals(PathFinder.Point other)
		{
			return this.x == other.x && this.y == other.y;
		}

		// Token: 0x04004A3B RID: 19003
		public int x;

		// Token: 0x04004A3C RID: 19004
		public int y;
	}

	// Token: 0x02000E62 RID: 3682
	public class Node : IMinHeapNode<PathFinder.Node>, ILinkedListNode<PathFinder.Node>
	{
		// Token: 0x17000684 RID: 1668
		// (get) Token: 0x06005073 RID: 20595 RVA: 0x001A1A88 File Offset: 0x0019FC88
		// (set) Token: 0x06005074 RID: 20596 RVA: 0x001A1A90 File Offset: 0x0019FC90
		public PathFinder.Node next { get; set; }

		// Token: 0x17000685 RID: 1669
		// (get) Token: 0x06005075 RID: 20597 RVA: 0x001A1A99 File Offset: 0x0019FC99
		// (set) Token: 0x06005076 RID: 20598 RVA: 0x001A1AA1 File Offset: 0x0019FCA1
		public PathFinder.Node child { get; set; }

		// Token: 0x17000686 RID: 1670
		// (get) Token: 0x06005077 RID: 20599 RVA: 0x001A1AAA File Offset: 0x0019FCAA
		public int order
		{
			get
			{
				return this.cost + this.heuristic;
			}
		}

		// Token: 0x06005078 RID: 20600 RVA: 0x001A1AB9 File Offset: 0x0019FCB9
		public Node(PathFinder.Point point, int cost, int heuristic, PathFinder.Node next = null)
		{
			this.point = point;
			this.cost = cost;
			this.heuristic = heuristic;
			this.next = next;
		}

		// Token: 0x04004A3D RID: 19005
		public PathFinder.Point point;

		// Token: 0x04004A3E RID: 19006
		public int cost;

		// Token: 0x04004A3F RID: 19007
		public int heuristic;
	}
}
