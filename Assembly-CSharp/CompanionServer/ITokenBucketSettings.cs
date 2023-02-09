using System;

namespace CompanionServer
{
	// Token: 0x020009BC RID: 2492
	public interface ITokenBucketSettings
	{
		// Token: 0x17000492 RID: 1170
		// (get) Token: 0x06003B0D RID: 15117
		double MaxTokens { get; }

		// Token: 0x17000493 RID: 1171
		// (get) Token: 0x06003B0E RID: 15118
		double TokensPerSec { get; }
	}
}
