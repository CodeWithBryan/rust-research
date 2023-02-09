using System;
using System.Collections.Generic;
using Rust.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200084D RID: 2125
public class NewsParagraph : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x060034D1 RID: 13521 RVA: 0x0013EADC File Offset: 0x0013CCDC
	public void OnPointerClick(PointerEventData eventData)
	{
		if (this.Text == null || this.Links == null || eventData.button != PointerEventData.InputButton.Left)
		{
			return;
		}
		int num = TMP_TextUtilities.FindIntersectingLink(this.Text, eventData.position, eventData.pressEventCamera);
		if (num < 0 || num >= this.Text.textInfo.linkCount)
		{
			return;
		}
		TMP_LinkInfo tmp_LinkInfo = this.Text.textInfo.linkInfo[num];
		int num2;
		if (!int.TryParse(tmp_LinkInfo.GetLinkID(), out num2) || num2 < 0 || num2 >= this.Links.Count)
		{
			return;
		}
		string text = this.Links[num2];
		if (text.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
		{
			Application.OpenURL(text);
		}
	}

	// Token: 0x04002F51 RID: 12113
	public RustText Text;

	// Token: 0x04002F52 RID: 12114
	public List<string> Links;
}
