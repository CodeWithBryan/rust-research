using System;
using Rust.UI;
using UnityEngine;

// Token: 0x0200074D RID: 1869
public class EnumListItemUI : MonoBehaviour
{
	// Token: 0x0600332B RID: 13099 RVA: 0x0013B3C1 File Offset: 0x001395C1
	public void Init(object value, string valueText, EnumListUI list)
	{
		this.Value = value;
		this.list = list;
		this.TextValue.text = valueText;
	}

	// Token: 0x0600332C RID: 13100 RVA: 0x0013B3DD File Offset: 0x001395DD
	public void Clicked()
	{
		this.list.ItemClicked(this.Value);
	}

	// Token: 0x0400299B RID: 10651
	public object Value;

	// Token: 0x0400299C RID: 10652
	public RustText TextValue;

	// Token: 0x0400299D RID: 10653
	private EnumListUI list;
}
