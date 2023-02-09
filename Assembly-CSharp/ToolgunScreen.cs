using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001BF RID: 447
public class ToolgunScreen : MonoBehaviour
{
	// Token: 0x060017FD RID: 6141 RVA: 0x000B1930 File Offset: 0x000AFB30
	public void SetScreenText(string newText)
	{
		bool flag = string.IsNullOrEmpty(newText);
		this.blockInfoText.gameObject.SetActive(!flag);
		this.noBlockText.gameObject.SetActive(flag);
		this.blockInfoText.text = newText;
	}

	// Token: 0x04001145 RID: 4421
	public Text blockInfoText;

	// Token: 0x04001146 RID: 4422
	public Text noBlockText;
}
