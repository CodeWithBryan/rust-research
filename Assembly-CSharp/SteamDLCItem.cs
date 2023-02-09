using System;
using UnityEngine;

// Token: 0x02000734 RID: 1844
[CreateAssetMenu(menuName = "Rust/Steam DLC Item")]
public class SteamDLCItem : ScriptableObject
{
	// Token: 0x060032F9 RID: 13049 RVA: 0x0013AD5C File Offset: 0x00138F5C
	public bool HasLicense(ulong steamid)
	{
		return this.bypassLicenseCheck || (PlatformService.Instance.IsValid && PlatformService.Instance.PlayerOwnsDownloadableContent(steamid, this.dlcAppID));
	}

	// Token: 0x060032FA RID: 13050 RVA: 0x0013AD87 File Offset: 0x00138F87
	public bool CanUse(BasePlayer player)
	{
		return player.isServer && (this.HasLicense(player.userID) || player.userID < 10000000UL);
	}

	// Token: 0x0400296A RID: 10602
	public int id;

	// Token: 0x0400296B RID: 10603
	public Translate.Phrase dlcName;

	// Token: 0x0400296C RID: 10604
	public int dlcAppID;

	// Token: 0x0400296D RID: 10605
	public bool bypassLicenseCheck;
}
