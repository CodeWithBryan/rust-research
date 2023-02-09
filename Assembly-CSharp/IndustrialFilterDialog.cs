using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020004B0 RID: 1200
public class IndustrialFilterDialog : UIDialog
{
	// Token: 0x04001F31 RID: 7985
	public GameObjectRef ItemPrefab;

	// Token: 0x04001F32 RID: 7986
	public Transform ItemParent;

	// Token: 0x04001F33 RID: 7987
	public GameObject ItemSearchParent;

	// Token: 0x04001F34 RID: 7988
	public ItemSearchEntry ItemSearchEntryPrefab;

	// Token: 0x04001F35 RID: 7989
	public VirtualItemIcon TargetItemIcon;

	// Token: 0x04001F36 RID: 7990
	public GameObject TargetCategoryRoot;

	// Token: 0x04001F37 RID: 7991
	public RustText TargetCategoryText;

	// Token: 0x04001F38 RID: 7992
	public Image TargetCategoryImage;

	// Token: 0x04001F39 RID: 7993
	public GameObject NoItemsPrompt;

	// Token: 0x04001F3A RID: 7994
	public RustButton PasteButton;
}
