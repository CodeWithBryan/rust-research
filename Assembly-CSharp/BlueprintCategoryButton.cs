using System;
using TMPro;
using UnityEngine;

// Token: 0x020007E3 RID: 2019
public class BlueprintCategoryButton : MonoBehaviour, IInventoryChanged
{
	// Token: 0x04002CC6 RID: 11462
	public TextMeshProUGUI amountLabel;

	// Token: 0x04002CC7 RID: 11463
	public ItemCategory Category;

	// Token: 0x04002CC8 RID: 11464
	public bool AlwaysShow;

	// Token: 0x04002CC9 RID: 11465
	public bool ShowItemCount = true;

	// Token: 0x04002CCA RID: 11466
	public GameObject BackgroundHighlight;

	// Token: 0x04002CCB RID: 11467
	public SoundDefinition clickSound;

	// Token: 0x04002CCC RID: 11468
	public SoundDefinition hoverSound;
}
