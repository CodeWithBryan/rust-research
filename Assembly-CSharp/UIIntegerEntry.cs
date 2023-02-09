using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000826 RID: 2086
public class UIIntegerEntry : MonoBehaviour
{
	// Token: 0x14000005 RID: 5
	// (add) Token: 0x0600347E RID: 13438 RVA: 0x0013E188 File Offset: 0x0013C388
	// (remove) Token: 0x0600347F RID: 13439 RVA: 0x0013E1C0 File Offset: 0x0013C3C0
	public event Action textChanged;

	// Token: 0x06003480 RID: 13440 RVA: 0x0013E1F5 File Offset: 0x0013C3F5
	public void OnAmountTextChanged()
	{
		this.textChanged();
	}

	// Token: 0x06003481 RID: 13441 RVA: 0x0013E202 File Offset: 0x0013C402
	public void SetAmount(int amount)
	{
		if (amount == this.GetIntAmount())
		{
			return;
		}
		this.textEntry.text = amount.ToString();
	}

	// Token: 0x06003482 RID: 13442 RVA: 0x0013E220 File Offset: 0x0013C420
	public int GetIntAmount()
	{
		int result = 0;
		int.TryParse(this.textEntry.text, out result);
		return result;
	}

	// Token: 0x06003483 RID: 13443 RVA: 0x0013E243 File Offset: 0x0013C443
	public void PlusMinus(int delta)
	{
		this.SetAmount(this.GetIntAmount() + delta);
	}

	// Token: 0x04002E44 RID: 11844
	public InputField textEntry;
}
