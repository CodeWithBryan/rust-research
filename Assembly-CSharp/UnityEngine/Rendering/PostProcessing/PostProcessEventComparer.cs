using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A4E RID: 2638
	internal struct PostProcessEventComparer : IEqualityComparer<PostProcessEvent>
	{
		// Token: 0x06003E55 RID: 15957 RVA: 0x0016DFE6 File Offset: 0x0016C1E6
		public bool Equals(PostProcessEvent x, PostProcessEvent y)
		{
			return x == y;
		}

		// Token: 0x06003E56 RID: 15958 RVA: 0x0003421C File Offset: 0x0003241C
		public int GetHashCode(PostProcessEvent obj)
		{
			return (int)obj;
		}
	}
}
