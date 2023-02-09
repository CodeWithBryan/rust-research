using System;
using UnityEngine;

// Token: 0x02000830 RID: 2096
public class InventoryUI : MonoBehaviour
{
	// Token: 0x06003490 RID: 13456 RVA: 0x0013E3CC File Offset: 0x0013C5CC
	private void Update()
	{
		if (this.ContactsButton != null && this.ContactsButton.activeSelf && !RelationshipManager.contacts)
		{
			this.ContactsButton.SetActive(false);
		}
	}

	// Token: 0x04002E96 RID: 11926
	public GameObject ContactsButton;
}
