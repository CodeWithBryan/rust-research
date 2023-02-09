using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200080D RID: 2061
public class LootPanelReclaim : LootPanel
{
	// Token: 0x04002D94 RID: 11668
	public int oldOverflow = -1;

	// Token: 0x04002D95 RID: 11669
	public Text overflowText;

	// Token: 0x04002D96 RID: 11670
	public GameObject overflowObject;

	// Token: 0x04002D97 RID: 11671
	public static readonly Translate.Phrase MorePhrase = new Translate.Phrase("reclaim.more", "additional items...");
}
