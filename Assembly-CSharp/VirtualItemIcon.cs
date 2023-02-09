using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200082E RID: 2094
public class VirtualItemIcon : MonoBehaviour
{
	// Token: 0x04002E80 RID: 11904
	public ItemDefinition itemDef;

	// Token: 0x04002E81 RID: 11905
	public int itemAmount;

	// Token: 0x04002E82 RID: 11906
	public bool asBlueprint;

	// Token: 0x04002E83 RID: 11907
	public Image iconImage;

	// Token: 0x04002E84 RID: 11908
	public Image bpUnderlay;

	// Token: 0x04002E85 RID: 11909
	public Text amountText;

	// Token: 0x04002E86 RID: 11910
	public Text hoverText;

	// Token: 0x04002E87 RID: 11911
	public CanvasGroup iconContents;

	// Token: 0x04002E88 RID: 11912
	public Tooltip ToolTip;

	// Token: 0x04002E89 RID: 11913
	public CanvasGroup conditionObject;

	// Token: 0x04002E8A RID: 11914
	public Image conditionFill;

	// Token: 0x04002E8B RID: 11915
	public Image maxConditionFill;

	// Token: 0x04002E8C RID: 11916
	public Image cornerIcon;
}
