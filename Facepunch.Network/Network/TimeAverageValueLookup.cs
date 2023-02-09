using System;
using System.Collections.Generic;

namespace Network
{
	// Token: 0x0200000D RID: 13
	public class TimeAverageValueLookup<T>
	{
		// Token: 0x06000063 RID: 99 RVA: 0x00002D8C File Offset: 0x00000F8C
		public bool TryIncrement(T id, ulong limit)
		{
			TimeAverageValue timeAverageValue;
			if (!this.dict.TryGetValue(id, out timeAverageValue))
			{
				timeAverageValue = new TimeAverageValue();
				this.dict.Add(id, timeAverageValue);
				timeAverageValue.Increment();
				return true;
			}
			if (timeAverageValue.Calculate() >= limit)
			{
				return false;
			}
			timeAverageValue.Increment();
			return true;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00002DD8 File Offset: 0x00000FD8
		public void Increment(T id)
		{
			TimeAverageValue timeAverageValue;
			if (!this.dict.TryGetValue(id, out timeAverageValue))
			{
				timeAverageValue = new TimeAverageValue();
				this.dict.Add(id, timeAverageValue);
			}
			timeAverageValue.Increment();
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00002E10 File Offset: 0x00001010
		public ulong Calculate(T id)
		{
			TimeAverageValue timeAverageValue;
			if (!this.dict.TryGetValue(id, out timeAverageValue))
			{
				return 0UL;
			}
			return timeAverageValue.Calculate();
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00002E36 File Offset: 0x00001036
		public void Clear()
		{
			this.dict.Clear();
		}

		// Token: 0x04000044 RID: 68
		public Dictionary<T, TimeAverageValue> dict = new Dictionary<T, TimeAverageValue>();
	}
}
