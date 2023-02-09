using System;

namespace CompanionServer.Handlers
{
	// Token: 0x020009C3 RID: 2499
	public enum ValidationResult
	{
		// Token: 0x0400352E RID: 13614
		Success,
		// Token: 0x0400352F RID: 13615
		NotFound,
		// Token: 0x04003530 RID: 13616
		RateLimit,
		// Token: 0x04003531 RID: 13617
		Banned,
		// Token: 0x04003532 RID: 13618
		Rejected
	}
}
