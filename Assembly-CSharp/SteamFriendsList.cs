using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000887 RID: 2183
public class SteamFriendsList : MonoBehaviour
{
	// Token: 0x0400306C RID: 12396
	public RectTransform targetPanel;

	// Token: 0x0400306D RID: 12397
	public SteamUserButton userButton;

	// Token: 0x0400306E RID: 12398
	public bool IncludeFriendsList = true;

	// Token: 0x0400306F RID: 12399
	public bool IncludeRecentlySeen;

	// Token: 0x04003070 RID: 12400
	public bool IncludeLastAttacker;

	// Token: 0x04003071 RID: 12401
	public bool IncludeRecentlyPlayedWith;

	// Token: 0x04003072 RID: 12402
	public bool ShowTeamFirst;

	// Token: 0x04003073 RID: 12403
	public bool HideSteamIdsInStreamerMode;

	// Token: 0x04003074 RID: 12404
	public bool RefreshOnEnable = true;

	// Token: 0x04003075 RID: 12405
	public SteamFriendsList.onFriendSelectedEvent onFriendSelected;

	// Token: 0x04003076 RID: 12406
	public Func<ulong, bool> shouldShowPlayer;

	// Token: 0x02000E46 RID: 3654
	[Serializable]
	public class onFriendSelectedEvent : UnityEvent<ulong, string>
	{
	}
}
