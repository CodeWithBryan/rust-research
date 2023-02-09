using System;
using UnityEngine;

// Token: 0x02000153 RID: 339
public class RandomRendererEnable : MonoBehaviour
{
	// Token: 0x170001BC RID: 444
	// (get) Token: 0x0600164D RID: 5709 RVA: 0x000A9D8F File Offset: 0x000A7F8F
	// (set) Token: 0x0600164E RID: 5710 RVA: 0x000A9D97 File Offset: 0x000A7F97
	public int EnabledIndex { get; private set; }

	// Token: 0x0600164F RID: 5711 RVA: 0x000A9DA0 File Offset: 0x000A7FA0
	public void OnEnable()
	{
		int num = UnityEngine.Random.Range(0, this.randoms.Length);
		this.EnabledIndex = num;
		this.randoms[num].enabled = true;
	}

	// Token: 0x04000F33 RID: 3891
	public Renderer[] randoms;
}
