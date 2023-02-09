using System;
using System.Diagnostics;

namespace Network
{
	// Token: 0x0200000E RID: 14
	public static class TimeEx
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000068 RID: 104 RVA: 0x00002E58 File Offset: 0x00001058
		public static double realtimeSinceStartup
		{
			get
			{
				return TimeEx.stopwatch.Elapsed.TotalSeconds;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000069 RID: 105 RVA: 0x00002E78 File Offset: 0x00001078
		public static double currentTimestamp
		{
			get
			{
				return (double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0;
			}
		}

		// Token: 0x04000045 RID: 69
		private static Stopwatch stopwatch = Stopwatch.StartNew();
	}
}
