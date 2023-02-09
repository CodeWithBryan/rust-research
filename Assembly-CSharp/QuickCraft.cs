using System;
using UnityEngine;

// Token: 0x0200081C RID: 2076
public class QuickCraft : SingletonComponent<QuickCraft>, IInventoryChanged
{
	// Token: 0x04002DFD RID: 11773
	public GameObjectRef craftButton;

	// Token: 0x04002DFE RID: 11774
	public GameObject empty;

	// Token: 0x04002DFF RID: 11775
	public Sprite FavouriteOnSprite;

	// Token: 0x04002E00 RID: 11776
	public Sprite FavouriteOffSprite;

	// Token: 0x04002E01 RID: 11777
	public Color FavouriteOnColor;

	// Token: 0x04002E02 RID: 11778
	public Color FavouriteOffColor;
}
