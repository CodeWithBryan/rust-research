using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200082F RID: 2095
public class WorkbenchPanel : LootPanel, IInventoryChanged
{
	// Token: 0x04002E8D RID: 11917
	public Button experimentButton;

	// Token: 0x04002E8E RID: 11918
	public Text timerText;

	// Token: 0x04002E8F RID: 11919
	public Text costText;

	// Token: 0x04002E90 RID: 11920
	public GameObject expermentCostParent;

	// Token: 0x04002E91 RID: 11921
	public GameObject controlsParent;

	// Token: 0x04002E92 RID: 11922
	public GameObject allUnlockedNotification;

	// Token: 0x04002E93 RID: 11923
	public GameObject informationParent;

	// Token: 0x04002E94 RID: 11924
	public GameObject cycleIcon;

	// Token: 0x04002E95 RID: 11925
	public TechTreeDialog techTreeDialog;
}
