using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000844 RID: 2116
public class ItemStoreBuySuccessModal : MonoBehaviour
{
	// Token: 0x060034B0 RID: 13488 RVA: 0x0013E6B8 File Offset: 0x0013C8B8
	public void Show(ulong orderId)
	{
		base.gameObject.SetActive(true);
		base.GetComponent<CanvasGroup>().alpha = 0f;
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 1f, 0.1f);
		SingletonComponent<SteamInventoryManager>.Instance != null;
	}

	// Token: 0x060034B1 RID: 13489 RVA: 0x0013E6F8 File Offset: 0x0013C8F8
	public void Hide()
	{
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 0f, 0.2f).setOnComplete(delegate()
		{
			base.gameObject.SetActive(false);
		});
	}
}
