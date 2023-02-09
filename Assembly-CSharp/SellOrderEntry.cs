using System;
using UnityEngine;

// Token: 0x02000127 RID: 295
public class SellOrderEntry : MonoBehaviour, IInventoryChanged
{
	// Token: 0x04000E2A RID: 3626
	public VirtualItemIcon MerchandiseIcon;

	// Token: 0x04000E2B RID: 3627
	public VirtualItemIcon CurrencyIcon;

	// Token: 0x04000E2C RID: 3628
	private ItemDefinition merchandiseInfo;

	// Token: 0x04000E2D RID: 3629
	private ItemDefinition currencyInfo;

	// Token: 0x04000E2E RID: 3630
	public GameObject buyButton;

	// Token: 0x04000E2F RID: 3631
	public GameObject cantaffordNotification;

	// Token: 0x04000E30 RID: 3632
	public GameObject outOfStockNotification;

	// Token: 0x04000E31 RID: 3633
	private IVendingMachineInterface vendingPanel;

	// Token: 0x04000E32 RID: 3634
	public UIIntegerEntry intEntry;
}
