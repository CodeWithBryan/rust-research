using System;
using TMPro;
using UnityEngine;

// Token: 0x02000845 RID: 2117
public class ItemStoreCartItem : MonoBehaviour
{
	// Token: 0x060034B4 RID: 13492 RVA: 0x0013E721 File Offset: 0x0013C921
	public void Init(int index, IPlayerItemDefinition def)
	{
		this.Index = index;
		this.Name.text = def.Name;
		this.Price.text = def.LocalPriceFormatted;
	}

	// Token: 0x04002F14 RID: 12052
	public int Index;

	// Token: 0x04002F15 RID: 12053
	public TextMeshProUGUI Name;

	// Token: 0x04002F16 RID: 12054
	public TextMeshProUGUI Price;
}
