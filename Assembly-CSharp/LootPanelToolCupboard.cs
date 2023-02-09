using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200011B RID: 283
public class LootPanelToolCupboard : LootPanel
{
	// Token: 0x04000E01 RID: 3585
	public List<VirtualItemIcon> costIcons;

	// Token: 0x04000E02 RID: 3586
	public Text costPerTimeText;

	// Token: 0x04000E03 RID: 3587
	public Text protectedText;

	// Token: 0x04000E04 RID: 3588
	public GameObject baseNotProtectedObj;

	// Token: 0x04000E05 RID: 3589
	public GameObject baseProtectedObj;

	// Token: 0x04000E06 RID: 3590
	public Translate.Phrase protectedPrefix;

	// Token: 0x04000E07 RID: 3591
	public Tooltip costToolTip;

	// Token: 0x04000E08 RID: 3592
	public Translate.Phrase blocksPhrase;
}
