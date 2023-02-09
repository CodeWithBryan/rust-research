using System;
using ConVar;

// Token: 0x0200042F RID: 1071
public class PlayerStatistics
{
	// Token: 0x0600235C RID: 9052 RVA: 0x000E08C1 File Offset: 0x000DEAC1
	public PlayerStatistics(BasePlayer player)
	{
		this.steam = new SteamStatistics(player);
		this.server = new ServerStatistics(player);
		this.combat = new CombatLog(player);
		this.forPlayer = player;
	}

	// Token: 0x0600235D RID: 9053 RVA: 0x000E08F4 File Offset: 0x000DEAF4
	public void Init()
	{
		this.steam.Init();
		this.server.Init();
		this.combat.Init();
	}

	// Token: 0x0600235E RID: 9054 RVA: 0x000E0918 File Offset: 0x000DEB18
	public void Save(bool forceSteamSave = false)
	{
		if (Server.official && (forceSteamSave || this.lastSteamSave > 60f))
		{
			this.lastSteamSave = 0f;
			this.steam.Save();
		}
		this.server.Save();
		this.combat.Save();
	}

	// Token: 0x0600235F RID: 9055 RVA: 0x000E0972 File Offset: 0x000DEB72
	public void Add(string name, int val, Stats stats = Stats.Steam)
	{
		if ((stats & Stats.Steam) != (Stats)0)
		{
			this.steam.Add(name, val);
		}
		if ((stats & Stats.Server) != (Stats)0)
		{
			this.server.Add(name, val);
		}
		if ((stats & Stats.Life) != (Stats)0)
		{
			this.forPlayer.LifeStoryGenericStat(name, val);
		}
	}

	// Token: 0x04001C1B RID: 7195
	public SteamStatistics steam;

	// Token: 0x04001C1C RID: 7196
	public ServerStatistics server;

	// Token: 0x04001C1D RID: 7197
	public CombatLog combat;

	// Token: 0x04001C1E RID: 7198
	private BasePlayer forPlayer;

	// Token: 0x04001C1F RID: 7199
	private TimeSince lastSteamSave;
}
