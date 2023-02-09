using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200081F RID: 2079
public class ResearchTablePanel : LootPanel
{
	// Token: 0x04002E14 RID: 11796
	public Button researchButton;

	// Token: 0x04002E15 RID: 11797
	public Text timerText;

	// Token: 0x04002E16 RID: 11798
	public GameObject itemDescNoItem;

	// Token: 0x04002E17 RID: 11799
	public GameObject itemDescTooBroken;

	// Token: 0x04002E18 RID: 11800
	public GameObject itemDescNotResearchable;

	// Token: 0x04002E19 RID: 11801
	public GameObject itemDescTooMany;

	// Token: 0x04002E1A RID: 11802
	public GameObject itemTakeBlueprint;

	// Token: 0x04002E1B RID: 11803
	public GameObject itemDescAlreadyResearched;

	// Token: 0x04002E1C RID: 11804
	public GameObject itemDescDefaultBlueprint;

	// Token: 0x04002E1D RID: 11805
	public Text successChanceText;

	// Token: 0x04002E1E RID: 11806
	public ItemIcon scrapIcon;

	// Token: 0x04002E1F RID: 11807
	[NonSerialized]
	public bool wasResearching;

	// Token: 0x04002E20 RID: 11808
	public GameObject[] workbenchReqs;
}
