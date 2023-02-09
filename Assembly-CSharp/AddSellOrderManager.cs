using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000120 RID: 288
public class AddSellOrderManager : MonoBehaviour
{
	// Token: 0x04000E0F RID: 3599
	public VirtualItemIcon sellItemIcon;

	// Token: 0x04000E10 RID: 3600
	public VirtualItemIcon currencyItemIcon;

	// Token: 0x04000E11 RID: 3601
	public GameObject itemSearchParent;

	// Token: 0x04000E12 RID: 3602
	public ItemSearchEntry itemSearchEntryPrefab;

	// Token: 0x04000E13 RID: 3603
	public InputField sellItemInput;

	// Token: 0x04000E14 RID: 3604
	public InputField sellItemAmount;

	// Token: 0x04000E15 RID: 3605
	public InputField currencyItemInput;

	// Token: 0x04000E16 RID: 3606
	public InputField currencyItemAmount;

	// Token: 0x04000E17 RID: 3607
	public VendingPanelAdmin adminPanel;
}
