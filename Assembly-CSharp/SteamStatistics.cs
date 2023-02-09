using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConVar;
using UnityEngine;

// Token: 0x02000431 RID: 1073
public class SteamStatistics
{
	// Token: 0x06002366 RID: 9062 RVA: 0x000E0A25 File Offset: 0x000DEC25
	public SteamStatistics(BasePlayer p)
	{
		this.player = p;
	}

	// Token: 0x06002367 RID: 9063 RVA: 0x000E0A3F File Offset: 0x000DEC3F
	public void Init()
	{
		if (!PlatformService.Instance.IsValid)
		{
			return;
		}
		this.refresh = PlatformService.Instance.LoadPlayerStats(this.player.userID);
		this.intStats.Clear();
	}

	// Token: 0x06002368 RID: 9064 RVA: 0x000E0A74 File Offset: 0x000DEC74
	public void Save()
	{
		if (!PlatformService.Instance.IsValid)
		{
			return;
		}
		PlatformService.Instance.SavePlayerStats(this.player.userID);
	}

	// Token: 0x06002369 RID: 9065 RVA: 0x000E0A9C File Offset: 0x000DEC9C
	public void Add(string name, int var)
	{
		if (!PlatformService.Instance.IsValid)
		{
			return;
		}
		if (this.refresh == null || !this.refresh.IsCompleted)
		{
			return;
		}
		using (TimeWarning.New("PlayerStats.Add", 0))
		{
			int num = 0;
			if (this.intStats.TryGetValue(name, out num))
			{
				Dictionary<string, int> dictionary = this.intStats;
				dictionary[name] += var;
				PlatformService.Instance.SetPlayerStatInt(this.player.userID, name, (long)this.intStats[name]);
			}
			else
			{
				num = (int)PlatformService.Instance.GetPlayerStatInt(this.player.userID, name, 0L);
				if (!PlatformService.Instance.SetPlayerStatInt(this.player.userID, name, (long)(num + var)))
				{
					if (Global.developer > 0)
					{
						Debug.LogWarning("[STEAMWORKS] Couldn't SetUserStat: " + name);
					}
				}
				else
				{
					this.intStats.Add(name, num + var);
				}
			}
		}
	}

	// Token: 0x0600236A RID: 9066 RVA: 0x000E0BA4 File Offset: 0x000DEDA4
	public int Get(string name)
	{
		if (!PlatformService.Instance.IsValid)
		{
			return 0;
		}
		if (this.refresh == null || !this.refresh.IsCompleted)
		{
			return 0;
		}
		int result;
		using (TimeWarning.New("PlayerStats.Get", 0))
		{
			int num;
			if (this.intStats.TryGetValue(name, out num))
			{
				result = num;
			}
			else
			{
				result = (int)PlatformService.Instance.GetPlayerStatInt(this.player.userID, name, 0L);
			}
		}
		return result;
	}

	// Token: 0x04001C23 RID: 7203
	private BasePlayer player;

	// Token: 0x04001C24 RID: 7204
	public Dictionary<string, int> intStats = new Dictionary<string, int>();

	// Token: 0x04001C25 RID: 7205
	private Task refresh;
}
