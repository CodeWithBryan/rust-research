using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000805 RID: 2053
public class LootPanelEngine : LootPanel
{
	// Token: 0x04002D77 RID: 11639
	[SerializeField]
	private Image engineImage;

	// Token: 0x04002D78 RID: 11640
	[SerializeField]
	private ItemIcon[] icons;

	// Token: 0x04002D79 RID: 11641
	[SerializeField]
	private GameObject warning;

	// Token: 0x04002D7A RID: 11642
	[SerializeField]
	private RustText hp;

	// Token: 0x04002D7B RID: 11643
	[SerializeField]
	private RustText power;

	// Token: 0x04002D7C RID: 11644
	[SerializeField]
	private RustText acceleration;

	// Token: 0x04002D7D RID: 11645
	[SerializeField]
	private RustText topSpeed;

	// Token: 0x04002D7E RID: 11646
	[SerializeField]
	private RustText fuelEconomy;
}
