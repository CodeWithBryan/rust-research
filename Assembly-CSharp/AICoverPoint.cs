using System;
using UnityEngine;

// Token: 0x020001C1 RID: 449
public class AICoverPoint : AIPoint
{
	// Token: 0x06001800 RID: 6144 RVA: 0x000B1978 File Offset: 0x000AFB78
	public void OnDrawGizmos()
	{
		Vector3 vector = base.transform.position + Vector3.up * 1f;
		Gizmos.color = Color.white;
		Gizmos.DrawLine(vector, vector + base.transform.forward * 0.5f);
		Gizmos.color = Color.yellow;
		Gizmos.DrawCube(base.transform.position + Vector3.up * 0.125f, new Vector3(0.5f, 0.25f, 0.5f));
		Gizmos.DrawLine(base.transform.position, vector);
		Vector3 normalized = (base.transform.forward + base.transform.right * this.coverDot * 1f).normalized;
		Vector3 normalized2 = (base.transform.forward + -base.transform.right * this.coverDot * 1f).normalized;
		Gizmos.DrawLine(vector, vector + normalized * 1f);
		Gizmos.DrawLine(vector, vector + normalized2 * 1f);
	}

	// Token: 0x04001148 RID: 4424
	public float coverDot = 0.5f;
}
