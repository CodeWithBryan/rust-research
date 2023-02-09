using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000808 RID: 2056
public class LootPanelIndustrialCrafter : LootPanel
{
	// Token: 0x04002D86 RID: 11654
	public GameObject CraftingRoot;

	// Token: 0x04002D87 RID: 11655
	public RustSlider ProgressSlider;

	// Token: 0x04002D88 RID: 11656
	public Transform Spinner;

	// Token: 0x04002D89 RID: 11657
	public float SpinSpeed = 90f;

	// Token: 0x04002D8A RID: 11658
	public GameObject WorkbenchLevelRoot;
}
