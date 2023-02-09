using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.AI
{
	// Token: 0x02000B03 RID: 2819
	public static class AStarPath
	{
		// Token: 0x060043BD RID: 17341 RVA: 0x00188EE5 File Offset: 0x001870E5
		private static float Heuristic(BasePathNode from, BasePathNode to)
		{
			return Vector3.Distance(from.transform.position, to.transform.position);
		}

		// Token: 0x060043BE RID: 17342 RVA: 0x00188F04 File Offset: 0x00187104
		public static bool FindPath(BasePathNode start, BasePathNode goal, out Stack<BasePathNode> path, out float pathCost)
		{
			path = null;
			pathCost = -1f;
			bool result = false;
			if (start == goal)
			{
				return false;
			}
			AStarNodeList astarNodeList = new AStarNodeList();
			HashSet<BasePathNode> hashSet = new HashSet<BasePathNode>();
			AStarNode item = new AStarNode(0f, AStarPath.Heuristic(start, goal), null, start);
			astarNodeList.Add(item);
			while (astarNodeList.Count > 0)
			{
				AStarNode astarNode = astarNodeList[0];
				astarNodeList.RemoveAt(0);
				hashSet.Add(astarNode.Node);
				if (astarNode.Satisfies(goal))
				{
					path = new Stack<BasePathNode>();
					pathCost = 0f;
					while (astarNode.Parent != null)
					{
						pathCost += astarNode.F;
						path.Push(astarNode.Node);
						astarNode = astarNode.Parent;
					}
					if (astarNode != null)
					{
						path.Push(astarNode.Node);
					}
					result = true;
					break;
				}
				foreach (BasePathNode basePathNode in astarNode.Node.linked)
				{
					if (!hashSet.Contains(basePathNode))
					{
						float num = astarNode.G + AStarPath.Heuristic(astarNode.Node, basePathNode);
						AStarNode astarNode2 = astarNodeList.GetAStarNodeOf(basePathNode);
						if (astarNode2 == null)
						{
							astarNode2 = new AStarNode(num, AStarPath.Heuristic(basePathNode, goal), astarNode, basePathNode);
							astarNodeList.Add(astarNode2);
							astarNodeList.AStarNodeSort();
						}
						else if (num < astarNode2.G)
						{
							astarNode2.Update(num, astarNode2.H, astarNode, basePathNode);
							astarNodeList.AStarNodeSort();
						}
					}
				}
			}
			return result;
		}
	}
}
