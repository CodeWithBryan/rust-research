using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x020009FD RID: 2557
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class DisplayNameAttribute : Attribute
	{
		// Token: 0x06003D39 RID: 15673 RVA: 0x00165B75 File Offset: 0x00163D75
		public DisplayNameAttribute(string displayName)
		{
			this.displayName = displayName;
		}

		// Token: 0x04003641 RID: 13889
		public readonly string displayName;
	}
}
