using System;
using UnityEngine;

// Token: 0x0200093B RID: 2363
public class IronsightAimPoint : MonoBehaviour
{
	// Token: 0x0600381F RID: 14367 RVA: 0x0014BFAC File Offset: 0x0014A1AC
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Vector3 normalized = (this.targetPoint.position - base.transform.position).normalized;
		Gizmos.color = Color.red;
		this.DrawArrow(base.transform.position, base.transform.position + normalized * 0.1f, 0.1f);
		Gizmos.color = Color.cyan;
		this.DrawArrow(base.transform.position, this.targetPoint.position, 0.02f);
		Gizmos.color = Color.yellow;
		this.DrawArrow(this.targetPoint.position, this.targetPoint.position + normalized * 3f, 0.02f);
	}

	// Token: 0x06003820 RID: 14368 RVA: 0x0014C088 File Offset: 0x0014A288
	private void DrawArrow(Vector3 start, Vector3 end, float arrowLength)
	{
		Vector3 normalized = (end - start).normalized;
		Vector3 up = Camera.current.transform.up;
		Gizmos.DrawLine(start, end);
		Gizmos.DrawLine(end, end + up * arrowLength - normalized * arrowLength);
		Gizmos.DrawLine(end, end - up * arrowLength - normalized * arrowLength);
		Gizmos.DrawLine(end + up * arrowLength - normalized * arrowLength, end - up * arrowLength - normalized * arrowLength);
	}

	// Token: 0x04003247 RID: 12871
	public Transform targetPoint;
}
