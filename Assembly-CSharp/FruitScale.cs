using System;
using UnityEngine;

// Token: 0x02000417 RID: 1047
public class FruitScale : MonoBehaviour, IClientComponent
{
	// Token: 0x060022FD RID: 8957 RVA: 0x000DEEDA File Offset: 0x000DD0DA
	public void SetProgress(float progress)
	{
		base.transform.localScale = Vector3.one * progress;
	}
}
