using System;

namespace Network
{
	// Token: 0x0200000C RID: 12
	public class TimeAverageValue
	{
		// Token: 0x0600005F RID: 95 RVA: 0x00002D63 File Offset: 0x00000F63
		public ulong Calculate()
		{
			return this.data.Calculate();
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00002D70 File Offset: 0x00000F70
		public void Increment()
		{
			this.data.Increment();
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00002D7D File Offset: 0x00000F7D
		public void Reset()
		{
			this.data.Reset();
		}

		// Token: 0x04000043 RID: 67
		private TimeAverageValueData data;
	}
}
