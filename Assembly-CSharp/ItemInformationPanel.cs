using System;
using UnityEngine;

// Token: 0x020007F7 RID: 2039
public class ItemInformationPanel : MonoBehaviour
{
	// Token: 0x06003440 RID: 13376 RVA: 0x0013D649 File Offset: 0x0013B849
	public virtual bool EligableForDisplay(ItemDefinition info)
	{
		Debug.LogWarning("ItemInformationPanel.EligableForDisplay");
		return false;
	}

	// Token: 0x06003441 RID: 13377 RVA: 0x0013D656 File Offset: 0x0013B856
	public virtual void SetupForItem(ItemDefinition info, Item item = null)
	{
		Debug.LogWarning("ItemInformationPanel.SetupForItem");
	}
}
