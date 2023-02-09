using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007AD RID: 1965
public class TechTreeSelectedNodeUI : MonoBehaviour
{
	// Token: 0x04002B8E RID: 11150
	public RustText selectedTitle;

	// Token: 0x04002B8F RID: 11151
	public RawImage selectedIcon;

	// Token: 0x04002B90 RID: 11152
	public RustText selectedDescription;

	// Token: 0x04002B91 RID: 11153
	public RustText costText;

	// Token: 0x04002B92 RID: 11154
	public RustText craftingCostText;

	// Token: 0x04002B93 RID: 11155
	public GameObject costObject;

	// Token: 0x04002B94 RID: 11156
	public GameObject cantAffordObject;

	// Token: 0x04002B95 RID: 11157
	public GameObject unlockedObject;

	// Token: 0x04002B96 RID: 11158
	public GameObject unlockButton;

	// Token: 0x04002B97 RID: 11159
	public GameObject noPathObject;

	// Token: 0x04002B98 RID: 11160
	public TechTreeDialog dialog;

	// Token: 0x04002B99 RID: 11161
	public Color ColorAfford;

	// Token: 0x04002B9A RID: 11162
	public Color ColorCantAfford;

	// Token: 0x04002B9B RID: 11163
	public GameObject totalRequiredRoot;

	// Token: 0x04002B9C RID: 11164
	public RustText totalRequiredText;

	// Token: 0x04002B9D RID: 11165
	public ItemInformationPanel[] informationPanels;
}
