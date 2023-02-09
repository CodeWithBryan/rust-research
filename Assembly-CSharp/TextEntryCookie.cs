using System;
using Rust;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200088C RID: 2188
public class TextEntryCookie : MonoBehaviour
{
	// Token: 0x1700040A RID: 1034
	// (get) Token: 0x0600358F RID: 13711 RVA: 0x00141F14 File Offset: 0x00140114
	public InputField control
	{
		get
		{
			return base.GetComponent<InputField>();
		}
	}

	// Token: 0x06003590 RID: 13712 RVA: 0x00141F1C File Offset: 0x0014011C
	private void OnEnable()
	{
		string @string = PlayerPrefs.GetString("TextEntryCookie_" + base.name);
		if (!string.IsNullOrEmpty(@string))
		{
			this.control.text = @string;
		}
		this.control.onValueChanged.Invoke(this.control.text);
	}

	// Token: 0x06003591 RID: 13713 RVA: 0x00141F6E File Offset: 0x0014016E
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		PlayerPrefs.SetString("TextEntryCookie_" + base.name, this.control.text);
	}
}
