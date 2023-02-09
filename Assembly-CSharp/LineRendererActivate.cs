using System;
using UnityEngine;

// Token: 0x020001B5 RID: 437
public class LineRendererActivate : MonoBehaviour, IClientComponent
{
	// Token: 0x060017E0 RID: 6112 RVA: 0x000B119E File Offset: 0x000AF39E
	private void OnEnable()
	{
		base.GetComponent<LineRenderer>().enabled = true;
	}
}
