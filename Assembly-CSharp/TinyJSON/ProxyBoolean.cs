using System;

namespace TinyJSON
{
	// Token: 0x020009A2 RID: 2466
	public sealed class ProxyBoolean : Variant
	{
		// Token: 0x06003A52 RID: 14930 RVA: 0x00157D2A File Offset: 0x00155F2A
		public ProxyBoolean(bool value)
		{
			this.value = value;
		}

		// Token: 0x06003A53 RID: 14931 RVA: 0x00157D39 File Offset: 0x00155F39
		public override bool ToBoolean(IFormatProvider provider)
		{
			return this.value;
		}

		// Token: 0x06003A54 RID: 14932 RVA: 0x00157D41 File Offset: 0x00155F41
		public override string ToString(IFormatProvider provider)
		{
			if (!this.value)
			{
				return "false";
			}
			return "true";
		}

		// Token: 0x040034CD RID: 13517
		private readonly bool value;
	}
}
