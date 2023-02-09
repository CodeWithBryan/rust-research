using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x020009FE RID: 2558
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class MaxAttribute : Attribute
	{
		// Token: 0x06003D3A RID: 15674 RVA: 0x00165B84 File Offset: 0x00163D84
		public MaxAttribute(float max)
		{
			this.max = max;
		}

		// Token: 0x04003642 RID: 13890
		public readonly float max;
	}
}
