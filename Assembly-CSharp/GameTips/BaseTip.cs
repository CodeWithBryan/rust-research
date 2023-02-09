using System;

namespace GameTips
{
	// Token: 0x020009AA RID: 2474
	public abstract class BaseTip
	{
		// Token: 0x06003AA0 RID: 15008
		public abstract Translate.Phrase GetPhrase();

		// Token: 0x17000489 RID: 1161
		// (get) Token: 0x06003AA1 RID: 15009
		public abstract bool ShouldShow { get; }

		// Token: 0x1700048A RID: 1162
		// (get) Token: 0x06003AA2 RID: 15010 RVA: 0x001582FC File Offset: 0x001564FC
		public string Type
		{
			get
			{
				return base.GetType().Name;
			}
		}
	}
}
