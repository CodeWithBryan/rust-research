using System;

// Token: 0x020008F6 RID: 2294
public static class TimeSpanEx
{
	// Token: 0x060036AE RID: 13998 RVA: 0x00145439 File Offset: 0x00143639
	public static string ToShortString(this TimeSpan timeSpan)
	{
		return string.Format("{0:00}:{1:00}:{2:00}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
	}

	// Token: 0x060036AF RID: 13999 RVA: 0x0014546A File Offset: 0x0014366A
	public static string ToShortStringNoHours(this TimeSpan timeSpan)
	{
		return string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
	}
}
