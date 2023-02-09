using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A00 RID: 2560
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class MinMaxAttribute : Attribute
	{
		// Token: 0x06003D3C RID: 15676 RVA: 0x00165BA2 File Offset: 0x00163DA2
		public MinMaxAttribute(float min, float max)
		{
			this.min = min;
			this.max = max;
		}

		// Token: 0x04003644 RID: 13892
		public readonly float min;

		// Token: 0x04003645 RID: 13893
		public readonly float max;
	}
}
