using System;
using System.Collections;
using System.Collections.Generic;

namespace TinyJSON
{
	// Token: 0x020009A1 RID: 2465
	public sealed class ProxyArray : Variant, IEnumerable<Variant>, IEnumerable
	{
		// Token: 0x06003A49 RID: 14921 RVA: 0x00157C3B File Offset: 0x00155E3B
		public ProxyArray()
		{
			this.list = new List<Variant>();
		}

		// Token: 0x06003A4A RID: 14922 RVA: 0x00157C4E File Offset: 0x00155E4E
		IEnumerator<Variant> IEnumerable<Variant>.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		// Token: 0x06003A4B RID: 14923 RVA: 0x00157C4E File Offset: 0x00155E4E
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		// Token: 0x06003A4C RID: 14924 RVA: 0x00157C60 File Offset: 0x00155E60
		public void Add(Variant item)
		{
			this.list.Add(item);
		}

		// Token: 0x17000480 RID: 1152
		public override Variant this[int index]
		{
			get
			{
				return this.list[index];
			}
			set
			{
				this.list[index] = value;
			}
		}

		// Token: 0x17000481 RID: 1153
		// (get) Token: 0x06003A4F RID: 14927 RVA: 0x00157C8B File Offset: 0x00155E8B
		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		// Token: 0x06003A50 RID: 14928 RVA: 0x00157C98 File Offset: 0x00155E98
		internal bool CanBeMultiRankArray(int[] rankLengths)
		{
			return this.CanBeMultiRankArray(0, rankLengths);
		}

		// Token: 0x06003A51 RID: 14929 RVA: 0x00157CA4 File Offset: 0x00155EA4
		private bool CanBeMultiRankArray(int rank, int[] rankLengths)
		{
			int count = this.list.Count;
			rankLengths[rank] = count;
			if (rank == rankLengths.Length - 1)
			{
				return true;
			}
			ProxyArray proxyArray = this.list[0] as ProxyArray;
			if (proxyArray == null)
			{
				return false;
			}
			int count2 = proxyArray.Count;
			for (int i = 1; i < count; i++)
			{
				ProxyArray proxyArray2 = this.list[i] as ProxyArray;
				if (proxyArray2 == null)
				{
					return false;
				}
				if (proxyArray2.Count != count2)
				{
					return false;
				}
				if (!proxyArray2.CanBeMultiRankArray(rank + 1, rankLengths))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x040034CC RID: 13516
		private readonly List<Variant> list;
	}
}
