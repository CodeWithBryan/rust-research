using System;

// Token: 0x0200039A RID: 922
public static class BaseNetworkableEx
{
	// Token: 0x0600202C RID: 8236 RVA: 0x000D2C82 File Offset: 0x000D0E82
	public static bool IsValid(this BaseNetworkable ent)
	{
		return !(ent == null) && ent.net != null;
	}

	// Token: 0x0600202D RID: 8237 RVA: 0x000D2C9A File Offset: 0x000D0E9A
	public static bool IsRealNull(this BaseNetworkable ent)
	{
		return ent == null;
	}
}
