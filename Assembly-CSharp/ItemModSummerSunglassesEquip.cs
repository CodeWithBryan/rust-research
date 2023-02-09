using System;
using ConVar;

// Token: 0x020005D2 RID: 1490
public class ItemModSummerSunglassesEquip : ItemMod
{
	// Token: 0x06002BF1 RID: 11249 RVA: 0x00107F14 File Offset: 0x00106114
	public override void DoAction(Item item, BasePlayer player)
	{
		base.DoAction(item, player);
		if (player != null && !string.IsNullOrEmpty(this.AchivementName) && player.inventory.containerWear.FindItemByUID(item.uid) != null)
		{
			float time = Env.time;
			if (time < this.SunriseTime || time > this.SunsetTime)
			{
				player.GiveAchievement(this.AchivementName);
			}
		}
	}

	// Token: 0x040023C9 RID: 9161
	public float SunsetTime;

	// Token: 0x040023CA RID: 9162
	public float SunriseTime;

	// Token: 0x040023CB RID: 9163
	public string AchivementName;
}
