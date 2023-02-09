using System;
using Rust.UI;
using TMPro;
using UnityEngine;

// Token: 0x02000846 RID: 2118
public class ItemStoreItem : MonoBehaviour
{
	// Token: 0x060034B6 RID: 13494 RVA: 0x0013E74C File Offset: 0x0013C94C
	internal void Init(IPlayerItemDefinition item, bool inCart)
	{
		this.item = item;
		this.Icon.Load(item.IconUrl);
		this.Name.SetText(item.Name);
		this.Price.text = item.LocalPriceFormatted;
		this.InCartTag.SetActive(inCart);
		if (string.IsNullOrWhiteSpace(item.ItemShortName))
		{
			this.ItemName.SetText("");
			return;
		}
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(item.ItemShortName);
		if (itemDefinition != null && !string.Equals(itemDefinition.displayName.english, item.Name, StringComparison.InvariantCultureIgnoreCase))
		{
			this.ItemName.SetPhrase(itemDefinition.displayName);
			return;
		}
		this.ItemName.SetText("");
	}

	// Token: 0x04002F17 RID: 12055
	public HttpImage Icon;

	// Token: 0x04002F18 RID: 12056
	public RustText Name;

	// Token: 0x04002F19 RID: 12057
	public TextMeshProUGUI Price;

	// Token: 0x04002F1A RID: 12058
	public RustText ItemName;

	// Token: 0x04002F1B RID: 12059
	public GameObject InCartTag;

	// Token: 0x04002F1C RID: 12060
	private IPlayerItemDefinition item;
}
