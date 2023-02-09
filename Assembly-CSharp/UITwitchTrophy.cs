using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000175 RID: 373
public class UITwitchTrophy : UIDialog
{
	// Token: 0x04000FC8 RID: 4040
	public HttpImage EventImage;

	// Token: 0x04000FC9 RID: 4041
	public RustText EventName;

	// Token: 0x04000FCA RID: 4042
	public RustText WinningTeamName;

	// Token: 0x04000FCB RID: 4043
	public RectTransform TeamMembersRoot;

	// Token: 0x04000FCC RID: 4044
	public GameObject TeamMemberNamePrefab;

	// Token: 0x04000FCD RID: 4045
	public GameObject MissingDataOverlay;
}
