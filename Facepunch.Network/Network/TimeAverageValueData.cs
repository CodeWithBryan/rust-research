using System;

namespace Network
{
	// Token: 0x0200000B RID: 11
	public struct TimeAverageValueData
	{
		// Token: 0x0600005C RID: 92 RVA: 0x00002CC8 File Offset: 0x00000EC8
		public ulong Calculate()
		{
			double realtimeSinceStartup = TimeEx.realtimeSinceStartup;
			double num = realtimeSinceStartup - this.refreshTime;
			if (num >= 1.0)
			{
				this.counterPrev = (ulong)(this.counterNext / num + 0.5);
				this.counterNext = 0UL;
				this.refreshTime = realtimeSinceStartup;
				num = 0.0;
			}
			return (ulong)(this.counterPrev * (1.0 - num)) + this.counterNext;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00002D40 File Offset: 0x00000F40
		public void Increment()
		{
			this.counterNext += 1UL;
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00002D51 File Offset: 0x00000F51
		public void Reset()
		{
			this.counterPrev = 0UL;
			this.counterNext = 0UL;
		}

		// Token: 0x04000040 RID: 64
		private double refreshTime;

		// Token: 0x04000041 RID: 65
		private ulong counterPrev;

		// Token: 0x04000042 RID: 66
		private ulong counterNext;
	}
}
