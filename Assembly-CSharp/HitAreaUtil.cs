using System;

// Token: 0x020004F4 RID: 1268
public static class HitAreaUtil
{
	// Token: 0x06002825 RID: 10277 RVA: 0x000F5C2B File Offset: 0x000F3E2B
	public static string Format(HitArea area)
	{
		if (area == (HitArea)0)
		{
			return "None";
		}
		if (area == (HitArea)(-1))
		{
			return "Generic";
		}
		return area.ToString();
	}
}
