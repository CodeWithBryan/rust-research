using System;
using Rust.UI;
using TMPro;
using UnityEngine;

// Token: 0x02000847 RID: 2119
public class ItemStoreItemInfoModal : MonoBehaviour
{
	// Token: 0x060034B8 RID: 13496 RVA: 0x0013E810 File Offset: 0x0013CA10
	public void Show(IPlayerItemDefinition item)
	{
		this.item = item;
		this.Icon.Load(item.IconUrl);
		this.Name.text = item.Name;
		this.Description.text = item.Description.BBCodeToUnity();
		this.Price.text = item.LocalPriceFormatted;
		base.gameObject.SetActive(true);
		base.GetComponent<CanvasGroup>().alpha = 0f;
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 1f, 0.1f);
	}

	// Token: 0x060034B9 RID: 13497 RVA: 0x0013E8A0 File Offset: 0x0013CAA0
	public void Hide()
	{
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 0f, 0.2f).setOnComplete(delegate()
		{
			base.gameObject.SetActive(false);
		});
	}

	// Token: 0x04002F1D RID: 12061
	public HttpImage Icon;

	// Token: 0x04002F1E RID: 12062
	public TextMeshProUGUI Name;

	// Token: 0x04002F1F RID: 12063
	public TextMeshProUGUI Price;

	// Token: 0x04002F20 RID: 12064
	public TextMeshProUGUI Description;

	// Token: 0x04002F21 RID: 12065
	private IPlayerItemDefinition item;
}
