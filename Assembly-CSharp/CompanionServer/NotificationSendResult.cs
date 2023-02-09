using System;

namespace CompanionServer
{
	// Token: 0x020009B3 RID: 2483
	public enum NotificationSendResult
	{
		// Token: 0x040034FD RID: 13565
		Failed,
		// Token: 0x040034FE RID: 13566
		Sent,
		// Token: 0x040034FF RID: 13567
		Empty,
		// Token: 0x04003500 RID: 13568
		Disabled,
		// Token: 0x04003501 RID: 13569
		RateLimited,
		// Token: 0x04003502 RID: 13570
		ServerError,
		// Token: 0x04003503 RID: 13571
		NoTargetsFound,
		// Token: 0x04003504 RID: 13572
		TooManySubscribers
	}
}
