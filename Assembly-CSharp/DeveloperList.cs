using System;
using System.Linq;
using Facepunch;
using Facepunch.Models;

// Token: 0x02000304 RID: 772
public static class DeveloperList
{
	// Token: 0x06001DA4 RID: 7588 RVA: 0x000CA9F4 File Offset: 0x000C8BF4
	public static bool Contains(string steamid)
	{
		return Application.Manifest != null && Application.Manifest.Administrators != null && Application.Manifest.Administrators.Any((Facepunch.Models.Manifest.Administrator x) => x.UserId == steamid);
	}

	// Token: 0x06001DA5 RID: 7589 RVA: 0x000CAA40 File Offset: 0x000C8C40
	public static bool Contains(ulong steamid)
	{
		return DeveloperList.Contains(steamid.ToString());
	}

	// Token: 0x06001DA6 RID: 7590 RVA: 0x000CAA4E File Offset: 0x000C8C4E
	public static bool IsDeveloper(BasePlayer ply)
	{
		return ply != null && DeveloperList.Contains(ply.UserIDString);
	}
}
