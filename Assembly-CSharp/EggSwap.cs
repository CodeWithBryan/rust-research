using System;
using UnityEngine;

// Token: 0x0200014E RID: 334
public class EggSwap : MonoBehaviour
{
	// Token: 0x0600162F RID: 5679 RVA: 0x000A9024 File Offset: 0x000A7224
	public void Show(int index)
	{
		this.HideAll();
		this.eggRenderers[index].enabled = true;
	}

	// Token: 0x06001630 RID: 5680 RVA: 0x000A903C File Offset: 0x000A723C
	public void HideAll()
	{
		Renderer[] array = this.eggRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
	}

	// Token: 0x04000F17 RID: 3863
	public Renderer[] eggRenderers;
}
