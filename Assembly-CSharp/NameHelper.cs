using System;
using System.Collections.Generic;

// Token: 0x0200072A RID: 1834
public static class NameHelper
{
	// Token: 0x060032D5 RID: 13013 RVA: 0x0003421C File Offset: 0x0003241C
	public static string Get(ulong userId, string name)
	{
		return name;
	}

	// Token: 0x060032D6 RID: 13014 RVA: 0x0013A3D5 File Offset: 0x001385D5
	public static string Get(IPlayerInfo playerInfo)
	{
		return NameHelper.Get(playerInfo.UserId, playerInfo.UserName);
	}

	// Token: 0x04002930 RID: 10544
	private static Dictionary<string, string> _cache = new Dictionary<string, string>();
}
