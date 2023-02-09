using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008B6 RID: 2230
public class VitalInfo : MonoBehaviour, IClientComponent, IVitalNotice
{
	// Token: 0x040030F3 RID: 12531
	public HudElement Element;

	// Token: 0x040030F4 RID: 12532
	public Image InfoImage;

	// Token: 0x040030F5 RID: 12533
	public VitalInfo.Vital VitalType;

	// Token: 0x040030F6 RID: 12534
	public TextMeshProUGUI text;

	// Token: 0x02000E4E RID: 3662
	public enum Vital
	{
		// Token: 0x040049F4 RID: 18932
		BuildingBlocked,
		// Token: 0x040049F5 RID: 18933
		CanBuild,
		// Token: 0x040049F6 RID: 18934
		Crafting,
		// Token: 0x040049F7 RID: 18935
		CraftLevel1,
		// Token: 0x040049F8 RID: 18936
		CraftLevel2,
		// Token: 0x040049F9 RID: 18937
		CraftLevel3,
		// Token: 0x040049FA RID: 18938
		DecayProtected,
		// Token: 0x040049FB RID: 18939
		Decaying,
		// Token: 0x040049FC RID: 18940
		SafeZone,
		// Token: 0x040049FD RID: 18941
		Buffed,
		// Token: 0x040049FE RID: 18942
		Pet
	}
}
