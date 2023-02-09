using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000821 RID: 2081
public class SelectedItem : SingletonComponent<SelectedItem>, IInventoryChanged
{
	// Token: 0x04002E29 RID: 11817
	public Image icon;

	// Token: 0x04002E2A RID: 11818
	public Image iconSplitter;

	// Token: 0x04002E2B RID: 11819
	public RustText title;

	// Token: 0x04002E2C RID: 11820
	public RustText description;

	// Token: 0x04002E2D RID: 11821
	public GameObject splitPanel;

	// Token: 0x04002E2E RID: 11822
	public GameObject itemProtection;

	// Token: 0x04002E2F RID: 11823
	public GameObject menuOption;

	// Token: 0x04002E30 RID: 11824
	public GameObject optionsParent;

	// Token: 0x04002E31 RID: 11825
	public GameObject innerPanelContainer;
}
