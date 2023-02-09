using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000827 RID: 2087
public class UIInventory : SingletonComponent<UIInventory>
{
	// Token: 0x04002E46 RID: 11846
	public TextMeshProUGUI PlayerName;

	// Token: 0x04002E47 RID: 11847
	public static bool isOpen;

	// Token: 0x04002E48 RID: 11848
	public static float LastOpened;

	// Token: 0x04002E49 RID: 11849
	public VerticalLayoutGroup rightContents;

	// Token: 0x04002E4A RID: 11850
	public GameObject QuickCraft;

	// Token: 0x04002E4B RID: 11851
	public Transform InventoryIconContainer;

	// Token: 0x04002E4C RID: 11852
	public ChangelogPanel ChangelogPanel;

	// Token: 0x04002E4D RID: 11853
	public ContactsPanel contactsPanel;
}
