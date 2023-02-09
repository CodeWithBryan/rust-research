using System;
using UnityEngine;

// Token: 0x0200011E RID: 286
[CreateAssetMenu(menuName = "Rust/NPCVendingOrderManifest")]
public class NPCVendingOrderManifest : ScriptableObject
{
	// Token: 0x060015B1 RID: 5553 RVA: 0x000A7664 File Offset: 0x000A5864
	public int GetIndex(NPCVendingOrder sample)
	{
		if (sample == null)
		{
			return -1;
		}
		for (int i = 0; i < this.orderList.Length; i++)
		{
			NPCVendingOrder y = this.orderList[i];
			if (sample == y)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060015B2 RID: 5554 RVA: 0x000A76A4 File Offset: 0x000A58A4
	public NPCVendingOrder GetFromIndex(int index)
	{
		if (this.orderList == null)
		{
			return null;
		}
		if (index < 0)
		{
			return null;
		}
		if (index >= this.orderList.Length)
		{
			return null;
		}
		return this.orderList[index];
	}

	// Token: 0x04000E0C RID: 3596
	public NPCVendingOrder[] orderList;
}
