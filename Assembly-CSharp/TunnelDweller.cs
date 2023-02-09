using System;
using Rust;

// Token: 0x020001E1 RID: 481
public class TunnelDweller : HumanNPC
{
	// Token: 0x0600192A RID: 6442 RVA: 0x000B6B3C File Offset: 0x000B4D3C
	protected override string OverrideCorpseName()
	{
		return "Tunnel Dweller";
	}

	// Token: 0x0600192B RID: 6443 RVA: 0x000B6B44 File Offset: 0x000B4D44
	protected override void OnKilledByPlayer(BasePlayer p)
	{
		base.OnKilledByPlayer(p);
		TrainEngine trainEngine;
		if (GameInfo.HasAchievements && p.GetParentEntity() != null && (trainEngine = (p.GetParentEntity() as TrainEngine)) != null && trainEngine.CurThrottleSetting != TrainEngine.EngineSpeeds.Zero && trainEngine.IsMovingOrOn)
		{
			p.stats.Add("dweller_kills_while_moving", 1, Stats.All);
			p.stats.Save(true);
		}
	}

	// Token: 0x040011E8 RID: 4584
	private const string DWELLER_KILL_STAT = "dweller_kills_while_moving";
}
