using System;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
	// Token: 0x020009D2 RID: 2514
	public sealed class MinAttribute : PropertyAttribute
	{
		// Token: 0x06003B5F RID: 15199 RVA: 0x0015B667 File Offset: 0x00159867
		public MinAttribute(float min)
		{
			this.min = min;
		}

		// Token: 0x0400353B RID: 13627
		public readonly float min;
	}
}
