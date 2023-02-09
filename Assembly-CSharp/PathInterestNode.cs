using System;
using UnityEngine;

// Token: 0x0200019C RID: 412
public class PathInterestNode : MonoBehaviour
{
	// Token: 0x170001CB RID: 459
	// (get) Token: 0x060017A8 RID: 6056 RVA: 0x000B05D8 File Offset: 0x000AE7D8
	// (set) Token: 0x060017A9 RID: 6057 RVA: 0x000B05E0 File Offset: 0x000AE7E0
	public float NextVisitTime { get; set; }

	// Token: 0x060017AA RID: 6058 RVA: 0x000B05E9 File Offset: 0x000AE7E9
	public void OnDrawGizmos()
	{
		Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
		Gizmos.DrawSphere(base.transform.position, 0.5f);
	}
}
