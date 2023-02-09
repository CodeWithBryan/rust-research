using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x020009FF RID: 2559
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class MinAttribute : Attribute
	{
		// Token: 0x06003D3B RID: 15675 RVA: 0x00165B93 File Offset: 0x00163D93
		public MinAttribute(float min)
		{
			this.min = min;
		}

		// Token: 0x04003643 RID: 13891
		public readonly float min;
	}
}
