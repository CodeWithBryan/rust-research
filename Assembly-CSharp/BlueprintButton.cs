using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007E2 RID: 2018
public class BlueprintButton : MonoBehaviour, IClientComponent, IInventoryChanged
{
	// Token: 0x04002CBE RID: 11454
	public Image image;

	// Token: 0x04002CBF RID: 11455
	public Image imageFavourite;

	// Token: 0x04002CC0 RID: 11456
	public Button button;

	// Token: 0x04002CC1 RID: 11457
	public CanvasGroup group;

	// Token: 0x04002CC2 RID: 11458
	public GameObject newNotification;

	// Token: 0x04002CC3 RID: 11459
	public GameObject lockedOverlay;

	// Token: 0x04002CC4 RID: 11460
	public Tooltip Tip;

	// Token: 0x04002CC5 RID: 11461
	public Image FavouriteIcon;
}
