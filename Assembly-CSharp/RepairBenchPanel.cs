using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200081E RID: 2078
public class RepairBenchPanel : LootPanel
{
	// Token: 0x04002E08 RID: 11784
	public Text infoText;

	// Token: 0x04002E09 RID: 11785
	public Button repairButton;

	// Token: 0x04002E0A RID: 11786
	public Color gotColor;

	// Token: 0x04002E0B RID: 11787
	public Color notGotColor;

	// Token: 0x04002E0C RID: 11788
	public Translate.Phrase phraseEmpty;

	// Token: 0x04002E0D RID: 11789
	public Translate.Phrase phraseNotRepairable;

	// Token: 0x04002E0E RID: 11790
	public Translate.Phrase phraseRepairNotNeeded;

	// Token: 0x04002E0F RID: 11791
	public Translate.Phrase phraseNoBlueprint;

	// Token: 0x04002E10 RID: 11792
	public GameObject skinsPanel;

	// Token: 0x04002E11 RID: 11793
	public GameObject changeSkinDialog;

	// Token: 0x04002E12 RID: 11794
	public IconSkinPicker picker;

	// Token: 0x04002E13 RID: 11795
	public GameObject attachmentSkinBlocker;
}
