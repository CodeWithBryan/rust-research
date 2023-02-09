using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007E5 RID: 2021
public class BlueprintHeader : MonoBehaviour
{
	// Token: 0x06003428 RID: 13352 RVA: 0x0013D4C4 File Offset: 0x0013B6C4
	public void Setup(ItemCategory name, int unlocked, int total)
	{
		this.categoryName.text = name.ToString().ToUpper();
		this.unlockCount.text = string.Format("UNLOCKED {0}/{1}", unlocked, total);
	}

	// Token: 0x04002CD3 RID: 11475
	public Text categoryName;

	// Token: 0x04002CD4 RID: 11476
	public Text unlockCount;
}
