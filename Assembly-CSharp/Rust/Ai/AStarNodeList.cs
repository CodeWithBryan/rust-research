using System;
using System.Collections.Generic;

namespace Rust.AI
{
	// Token: 0x02000B05 RID: 2821
	public class AStarNodeList : List<AStarNode>
	{
		// Token: 0x060043C5 RID: 17349 RVA: 0x0018912C File Offset: 0x0018732C
		public bool Contains(BasePathNode n)
		{
			for (int i = 0; i < base.Count; i++)
			{
				AStarNode astarNode = base[i];
				if (astarNode != null && astarNode.Node.Equals(n))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060043C6 RID: 17350 RVA: 0x00189168 File Offset: 0x00187368
		public AStarNode GetAStarNodeOf(BasePathNode n)
		{
			for (int i = 0; i < base.Count; i++)
			{
				AStarNode astarNode = base[i];
				if (astarNode != null && astarNode.Node.Equals(n))
				{
					return astarNode;
				}
			}
			return null;
		}

		// Token: 0x060043C7 RID: 17351 RVA: 0x001891A2 File Offset: 0x001873A2
		public void AStarNodeSort()
		{
			base.Sort(this.comparer);
		}

		// Token: 0x04003C51 RID: 15441
		private readonly AStarNodeList.AStarNodeComparer comparer = new AStarNodeList.AStarNodeComparer();

		// Token: 0x02000F3B RID: 3899
		private class AStarNodeComparer : IComparer<AStarNode>
		{
			// Token: 0x0600522B RID: 21035 RVA: 0x001A6A0F File Offset: 0x001A4C0F
			int IComparer<AStarNode>.Compare(AStarNode lhs, AStarNode rhs)
			{
				if (lhs < rhs)
				{
					return -1;
				}
				if (lhs > rhs)
				{
					return 1;
				}
				return 0;
			}
		}
	}
}
