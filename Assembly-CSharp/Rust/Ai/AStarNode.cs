using System;

namespace Rust.AI
{
	// Token: 0x02000B04 RID: 2820
	public class AStarNode
	{
		// Token: 0x170005EC RID: 1516
		// (get) Token: 0x060043BF RID: 17343 RVA: 0x001890A8 File Offset: 0x001872A8
		public float F
		{
			get
			{
				return this.G + this.H;
			}
		}

		// Token: 0x060043C0 RID: 17344 RVA: 0x001890B7 File Offset: 0x001872B7
		public AStarNode(float g, float h, AStarNode parent, BasePathNode node)
		{
			this.G = g;
			this.H = h;
			this.Parent = parent;
			this.Node = node;
		}

		// Token: 0x060043C1 RID: 17345 RVA: 0x001890DC File Offset: 0x001872DC
		public void Update(float g, float h, AStarNode parent, BasePathNode node)
		{
			this.G = g;
			this.H = h;
			this.Parent = parent;
			this.Node = node;
		}

		// Token: 0x060043C2 RID: 17346 RVA: 0x001890FB File Offset: 0x001872FB
		public bool Satisfies(BasePathNode node)
		{
			return this.Node == node;
		}

		// Token: 0x060043C3 RID: 17347 RVA: 0x00189109 File Offset: 0x00187309
		public static bool operator <(AStarNode lhs, AStarNode rhs)
		{
			return lhs.F < rhs.F;
		}

		// Token: 0x060043C4 RID: 17348 RVA: 0x00189119 File Offset: 0x00187319
		public static bool operator >(AStarNode lhs, AStarNode rhs)
		{
			return lhs.F > rhs.F;
		}

		// Token: 0x04003C4D RID: 15437
		public AStarNode Parent;

		// Token: 0x04003C4E RID: 15438
		public float G;

		// Token: 0x04003C4F RID: 15439
		public float H;

		// Token: 0x04003C50 RID: 15440
		public BasePathNode Node;
	}
}
