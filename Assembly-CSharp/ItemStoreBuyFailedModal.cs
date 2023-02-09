using System;
using UnityEngine;

// Token: 0x02000843 RID: 2115
public class ItemStoreBuyFailedModal : MonoBehaviour
{
	// Token: 0x060034AC RID: 13484 RVA: 0x0013E65B File Offset: 0x0013C85B
	public void Show(ulong orderid)
	{
		base.gameObject.SetActive(true);
		base.GetComponent<CanvasGroup>().alpha = 0f;
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 1f, 0.1f);
	}

	// Token: 0x060034AD RID: 13485 RVA: 0x0013E68F File Offset: 0x0013C88F
	public void Hide()
	{
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 0f, 0.2f).setOnComplete(delegate()
		{
			base.gameObject.SetActive(false);
		});
	}
}
