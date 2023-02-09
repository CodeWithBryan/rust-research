using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007FB RID: 2043
public class ItemPreviewIcon : BaseMonoBehaviour, IInventoryChanged, IItemAmountChanged, IItemIconChanged
{
	// Token: 0x04002D53 RID: 11603
	public ItemContainerSource containerSource;

	// Token: 0x04002D54 RID: 11604
	[Range(0f, 64f)]
	public int slot;

	// Token: 0x04002D55 RID: 11605
	public bool setSlotFromSiblingIndex = true;

	// Token: 0x04002D56 RID: 11606
	public CanvasGroup iconContents;

	// Token: 0x04002D57 RID: 11607
	public Image iconImage;

	// Token: 0x04002D58 RID: 11608
	public Text amountText;

	// Token: 0x04002D59 RID: 11609
	[NonSerialized]
	public Item item;
}
