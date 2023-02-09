using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A5D RID: 2653
	internal class TargetPool
	{
		// Token: 0x06003EEC RID: 16108 RVA: 0x00171412 File Offset: 0x0016F612
		internal TargetPool()
		{
			this.m_Pool = new List<int>();
			this.Get();
		}

		// Token: 0x06003EED RID: 16109 RVA: 0x0017142C File Offset: 0x0016F62C
		internal int Get()
		{
			int result = this.Get(this.m_Current);
			this.m_Current++;
			return result;
		}

		// Token: 0x06003EEE RID: 16110 RVA: 0x00171448 File Offset: 0x0016F648
		private int Get(int i)
		{
			int result;
			if (this.m_Pool.Count > i)
			{
				result = this.m_Pool[i];
			}
			else
			{
				while (this.m_Pool.Count <= i)
				{
					this.m_Pool.Add(Shader.PropertyToID("_TargetPool" + i));
				}
				result = this.m_Pool[i];
			}
			return result;
		}

		// Token: 0x06003EEF RID: 16111 RVA: 0x001714AE File Offset: 0x0016F6AE
		internal void Reset()
		{
			this.m_Current = 0;
		}

		// Token: 0x04003839 RID: 14393
		private readonly List<int> m_Pool;

		// Token: 0x0400383A RID: 14394
		private int m_Current;
	}
}
