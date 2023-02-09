using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200080A RID: 2058
public class LootPanelMixingTable : LootPanel, IInventoryChanged
{
	// Token: 0x04002D8C RID: 11660
	public GameObject controlsOn;

	// Token: 0x04002D8D RID: 11661
	public GameObject controlsOff;

	// Token: 0x04002D8E RID: 11662
	public Button StartMixingButton;

	// Token: 0x04002D8F RID: 11663
	public InfoBar ProgressBar;

	// Token: 0x04002D90 RID: 11664
	public GameObject recipeItemPrefab;

	// Token: 0x04002D91 RID: 11665
	public RectTransform recipeContentRect;
}
