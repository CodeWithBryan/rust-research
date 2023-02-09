using System;
using UnityEngine;

// Token: 0x0200087E RID: 2174
public class RandomObjectEnableOnEnable : MonoBehaviour
{
	// Token: 0x06003568 RID: 13672 RVA: 0x001418D6 File Offset: 0x0013FAD6
	public void OnEnable()
	{
		this.objects[UnityEngine.Random.Range(0, this.objects.Length)].SetActive(true);
	}

	// Token: 0x0400303E RID: 12350
	public GameObject[] objects;
}
