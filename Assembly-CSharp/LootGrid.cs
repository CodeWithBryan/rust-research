using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000813 RID: 2067
public class LootGrid : MonoBehaviour
{
	// Token: 0x06003462 RID: 13410 RVA: 0x0013D88C File Offset: 0x0013BA8C
	public void CreateInventory(ItemContainerSource inventory, int? slots = null, int? offset = null)
	{
		foreach (ItemIcon itemIcon in this._icons)
		{
			UnityEngine.Object.Destroy(itemIcon.gameObject);
		}
		this._icons.Clear();
		this.Inventory = inventory;
		this.Count = (slots ?? this.Count);
		this.Offset = (offset ?? this.Offset);
		for (int i = 0; i < this.Count; i++)
		{
			ItemIcon component = UnityEngine.Object.Instantiate<GameObject>(this.ItemIconPrefab, base.transform).GetComponent<ItemIcon>();
			component.slot = this.Offset + i;
			component.emptySlotBackgroundSprite = (this.BackgroundImage ?? component.emptySlotBackgroundSprite);
			component.containerSource = inventory;
			this._icons.Add(component);
		}
	}

	// Token: 0x04002DB2 RID: 11698
	public int Container;

	// Token: 0x04002DB3 RID: 11699
	public int Offset;

	// Token: 0x04002DB4 RID: 11700
	public int Count = 1;

	// Token: 0x04002DB5 RID: 11701
	public GameObject ItemIconPrefab;

	// Token: 0x04002DB6 RID: 11702
	public Sprite BackgroundImage;

	// Token: 0x04002DB7 RID: 11703
	public ItemContainerSource Inventory;

	// Token: 0x04002DB8 RID: 11704
	private List<ItemIcon> _icons = new List<ItemIcon>();
}
