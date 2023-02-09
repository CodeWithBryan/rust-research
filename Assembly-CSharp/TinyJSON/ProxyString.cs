using System;

namespace TinyJSON
{
	// Token: 0x020009A5 RID: 2469
	public sealed class ProxyString : Variant
	{
		// Token: 0x06003A71 RID: 14961 RVA: 0x00157FE5 File Offset: 0x001561E5
		public ProxyString(string value)
		{
			this.value = value;
		}

		// Token: 0x06003A72 RID: 14962 RVA: 0x00157FF4 File Offset: 0x001561F4
		public override string ToString(IFormatProvider provider)
		{
			return this.value;
		}

		// Token: 0x040034D2 RID: 13522
		private readonly string value;
	}
}
