using System;
using UnityEngine;

// Token: 0x02000940 RID: 2368
public class SwapKeycard : MonoBehaviour
{
	// Token: 0x06003835 RID: 14389 RVA: 0x0014C5E4 File Offset: 0x0014A7E4
	public void UpdateAccessLevel(int level)
	{
		GameObject[] array = this.accessLevels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		this.accessLevels[level - 1].SetActive(true);
	}

	// Token: 0x06003836 RID: 14390 RVA: 0x0014C620 File Offset: 0x0014A820
	public void SetRootActive(int index)
	{
		for (int i = 0; i < this.accessLevels.Length; i++)
		{
			this.accessLevels[i].SetActive(i == index);
		}
	}

	// Token: 0x04003257 RID: 12887
	public GameObject[] accessLevels;
}
