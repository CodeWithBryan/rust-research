using System;
using UnityEngine;

namespace Smaa
{
	// Token: 0x0200098B RID: 2443
	public sealed class MinAttribute : PropertyAttribute
	{
		// Token: 0x060039F9 RID: 14841 RVA: 0x00155F16 File Offset: 0x00154116
		public MinAttribute(float min)
		{
			this.min = min;
		}

		// Token: 0x04003480 RID: 13440
		public readonly float min;
	}
}
