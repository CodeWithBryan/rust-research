using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200012A RID: 298
public class VendingMachineScreen : MonoBehaviour
{
	// Token: 0x04000E3A RID: 3642
	public RawImage largeIcon;

	// Token: 0x04000E3B RID: 3643
	public RawImage blueprintIcon;

	// Token: 0x04000E3C RID: 3644
	public Text mainText;

	// Token: 0x04000E3D RID: 3645
	public Text lowerText;

	// Token: 0x04000E3E RID: 3646
	public Text centerText;

	// Token: 0x04000E3F RID: 3647
	public RawImage smallIcon;

	// Token: 0x04000E40 RID: 3648
	public VendingMachine vendingMachine;

	// Token: 0x04000E41 RID: 3649
	public Sprite outOfStockSprite;

	// Token: 0x04000E42 RID: 3650
	public Renderer fadeoutMesh;

	// Token: 0x04000E43 RID: 3651
	public CanvasGroup screenCanvas;

	// Token: 0x04000E44 RID: 3652
	public Renderer light1;

	// Token: 0x04000E45 RID: 3653
	public Renderer light2;

	// Token: 0x04000E46 RID: 3654
	public float nextImageTime;

	// Token: 0x04000E47 RID: 3655
	public int currentImageIndex;

	// Token: 0x02000BD6 RID: 3030
	public enum vmScreenState
	{
		// Token: 0x04003FDD RID: 16349
		ItemScroll,
		// Token: 0x04003FDE RID: 16350
		Vending,
		// Token: 0x04003FDF RID: 16351
		Message,
		// Token: 0x04003FE0 RID: 16352
		ShopName,
		// Token: 0x04003FE1 RID: 16353
		OutOfStock
	}
}
