using System;
using System.Collections.Generic;
using System.Linq;
using Rust.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002ED RID: 749
public class ItemListTools : MonoBehaviour
{
	// Token: 0x06001D5D RID: 7517 RVA: 0x000C8E37 File Offset: 0x000C7037
	public void OnPanelOpened()
	{
		this.CacheAllItems();
		this.Refresh();
		this.searchInputText.InputField.ActivateInputField();
	}

	// Token: 0x06001D5E RID: 7518 RVA: 0x000C8E55 File Offset: 0x000C7055
	private void OnOpenDevTools()
	{
		this.searchInputText.InputField.ActivateInputField();
	}

	// Token: 0x06001D5F RID: 7519 RVA: 0x000C8E67 File Offset: 0x000C7067
	private void CacheAllItems()
	{
		if (this.allItems != null)
		{
			return;
		}
		this.allItems = from x in ItemManager.GetItemDefinitions()
		orderby x.displayName.translated
		select x;
	}

	// Token: 0x06001D60 RID: 7520 RVA: 0x000C8EA1 File Offset: 0x000C70A1
	public void Refresh()
	{
		this.RebuildCategories();
	}

	// Token: 0x06001D61 RID: 7521 RVA: 0x000C8EAC File Offset: 0x000C70AC
	private void RebuildCategories()
	{
		for (int i = 0; i < this.categoryButton.transform.parent.childCount; i++)
		{
			Transform child = this.categoryButton.transform.parent.GetChild(i);
			if (!(child == this.categoryButton.transform))
			{
				GameManager.Destroy(child.gameObject, 0f);
			}
		}
		this.categoryButton.SetActive(true);
		foreach (IGrouping<ItemCategory, ItemDefinition> source in from x in ItemManager.GetItemDefinitions()
		group x by x.category into x
		orderby x.First<ItemDefinition>().category
		select x)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.categoryButton);
			gameObject.transform.SetParent(this.categoryButton.transform.parent, false);
			gameObject.GetComponentInChildren<TextMeshProUGUI>().text = source.First<ItemDefinition>().category.ToString();
			Button btn = gameObject.GetComponentInChildren<Button>();
			ItemDefinition[] itemArray = source.ToArray<ItemDefinition>();
			btn.onClick.AddListener(delegate()
			{
				if (this.lastCategory)
				{
					this.lastCategory.interactable = true;
				}
				this.lastCategory = btn;
				this.lastCategory.interactable = false;
				this.SwitchItemCategory(itemArray);
			});
			if (this.lastCategory == null)
			{
				this.lastCategory = btn;
				this.lastCategory.interactable = false;
				this.SwitchItemCategory(itemArray);
			}
		}
		this.categoryButton.SetActive(false);
	}

	// Token: 0x06001D62 RID: 7522 RVA: 0x000C907C File Offset: 0x000C727C
	private void SwitchItemCategory(ItemDefinition[] defs)
	{
		this.currentItems = from x in defs
		orderby x.displayName.translated
		select x;
		this.searchInputText.Text = "";
		this.FilterItems(null);
	}

	// Token: 0x06001D63 RID: 7523 RVA: 0x000C90CC File Offset: 0x000C72CC
	public void FilterItems(string searchText)
	{
		if (this.itemButton == null)
		{
			return;
		}
		for (int i = 0; i < this.itemButton.transform.parent.childCount; i++)
		{
			Transform child = this.itemButton.transform.parent.GetChild(i);
			if (!(child == this.itemButton.transform))
			{
				GameManager.Destroy(child.gameObject, 0f);
			}
		}
		this.itemButton.SetActive(true);
		bool flag = !string.IsNullOrEmpty(searchText);
		string value = flag ? searchText.ToLower() : null;
		IEnumerable<ItemDefinition> enumerable = flag ? this.allItems : this.currentItems;
		int num = 0;
		foreach (ItemDefinition itemDefinition in enumerable)
		{
			if (!itemDefinition.hidden && (!flag || itemDefinition.displayName.translated.ToLower().Contains(value)))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.itemButton);
				gameObject.transform.SetParent(this.itemButton.transform.parent, false);
				gameObject.GetComponentInChildren<TextMeshProUGUI>().text = itemDefinition.displayName.translated;
				gameObject.GetComponentInChildren<ItemButtonTools>().itemDef = itemDefinition;
				gameObject.GetComponentInChildren<ItemButtonTools>().image.sprite = itemDefinition.iconSprite;
				num++;
				if (num >= 160)
				{
					break;
				}
			}
		}
		this.itemButton.SetActive(false);
	}

	// Token: 0x040016C0 RID: 5824
	public GameObject categoryButton;

	// Token: 0x040016C1 RID: 5825
	public GameObject itemButton;

	// Token: 0x040016C2 RID: 5826
	public RustInput searchInputText;

	// Token: 0x040016C3 RID: 5827
	internal Button lastCategory;

	// Token: 0x040016C4 RID: 5828
	private IOrderedEnumerable<ItemDefinition> currentItems;

	// Token: 0x040016C5 RID: 5829
	private IOrderedEnumerable<ItemDefinition> allItems;
}
