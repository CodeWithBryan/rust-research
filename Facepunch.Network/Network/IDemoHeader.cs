using System;
using System.IO;

namespace Network
{
	// Token: 0x02000007 RID: 7
	public interface IDemoHeader
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000025 RID: 37
		// (set) Token: 0x06000026 RID: 38
		long Length { get; set; }

		// Token: 0x06000027 RID: 39
		void Write(BinaryWriter writer);
	}
}
