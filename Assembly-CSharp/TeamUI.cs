using System;
using TMPro;
using UnityEngine;

// Token: 0x0200088B RID: 2187
public class TeamUI : MonoBehaviour
{
	// Token: 0x0400308F RID: 12431
	public static Translate.Phrase invitePhrase = new Translate.Phrase("team.invited", "{0} has invited you to join a team");

	// Token: 0x04003090 RID: 12432
	public RectTransform MemberPanel;

	// Token: 0x04003091 RID: 12433
	public GameObject memberEntryPrefab;

	// Token: 0x04003092 RID: 12434
	public TeamMemberElement[] elements;

	// Token: 0x04003093 RID: 12435
	public GameObject NoTeamPanel;

	// Token: 0x04003094 RID: 12436
	public GameObject TeamPanel;

	// Token: 0x04003095 RID: 12437
	public GameObject LeaveTeamButton;

	// Token: 0x04003096 RID: 12438
	public GameObject InviteAcceptPanel;

	// Token: 0x04003097 RID: 12439
	public TextMeshProUGUI inviteText;

	// Token: 0x04003098 RID: 12440
	public static bool dirty = true;

	// Token: 0x04003099 RID: 12441
	[NonSerialized]
	public static ulong pendingTeamID;

	// Token: 0x0400309A RID: 12442
	[NonSerialized]
	public static string pendingTeamLeaderName;
}
