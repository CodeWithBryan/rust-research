using System;

namespace TinyJSON
{
	// Token: 0x0200099C RID: 2460
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
	public class DecodeAlias : Attribute
	{
		// Token: 0x1700047F RID: 1151
		// (get) Token: 0x06003A34 RID: 14900 RVA: 0x0015712C File Offset: 0x0015532C
		// (set) Token: 0x06003A35 RID: 14901 RVA: 0x00157134 File Offset: 0x00155334
		public string[] Names { get; private set; }

		// Token: 0x06003A36 RID: 14902 RVA: 0x0015713D File Offset: 0x0015533D
		public DecodeAlias(params string[] names)
		{
			this.Names = names;
		}

		// Token: 0x06003A37 RID: 14903 RVA: 0x0015714C File Offset: 0x0015534C
		public bool Contains(string name)
		{
			return Array.IndexOf<string>(this.Names, name) > -1;
		}
	}
}
