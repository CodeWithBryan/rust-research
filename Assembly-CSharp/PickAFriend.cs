using System;
using Rust.UI;
using UnityEngine.UI;

// Token: 0x020007A2 RID: 1954
public class PickAFriend : UIDialog
{
	// Token: 0x170003F8 RID: 1016
	// (set) Token: 0x060033A2 RID: 13218 RVA: 0x0013B8EB File Offset: 0x00139AEB
	public Func<ulong, bool> shouldShowPlayer
	{
		set
		{
			if (this.friendsList != null)
			{
				this.friendsList.shouldShowPlayer = value;
			}
		}
	}

	// Token: 0x04002B50 RID: 11088
	public InputField input;

	// Token: 0x04002B51 RID: 11089
	public RustText headerText;

	// Token: 0x04002B52 RID: 11090
	public bool AutoSelectInputField;

	// Token: 0x04002B53 RID: 11091
	public bool AllowMultiple;

	// Token: 0x04002B54 RID: 11092
	public Action<ulong, string> onSelected;

	// Token: 0x04002B55 RID: 11093
	public Translate.Phrase sleepingBagHeaderPhrase = new Translate.Phrase("assign_to_friend", "Assign To a Friend");

	// Token: 0x04002B56 RID: 11094
	public Translate.Phrase turretHeaderPhrase = new Translate.Phrase("authorize_a_friend", "Authorize a Friend");

	// Token: 0x04002B57 RID: 11095
	public SteamFriendsList friendsList;
}
