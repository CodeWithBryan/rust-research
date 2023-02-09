using System;
using UnityEngine;

// Token: 0x020002E9 RID: 745
public class DrawArrow : MonoBehaviour
{
	// Token: 0x06001D51 RID: 7505 RVA: 0x000C8BF8 File Offset: 0x000C6DF8
	private void OnDrawGizmos()
	{
		Vector3 forward = base.transform.forward;
		Vector3 up = Camera.current.transform.up;
		Vector3 position = base.transform.position;
		Vector3 vector = base.transform.position + forward * this.length;
		Gizmos.color = this.color;
		Gizmos.DrawLine(position, vector);
		Gizmos.DrawLine(vector, vector + up * this.arrowLength - forward * this.arrowLength);
		Gizmos.DrawLine(vector, vector - up * this.arrowLength - forward * this.arrowLength);
		Gizmos.DrawLine(vector + up * this.arrowLength - forward * this.arrowLength, vector - up * this.arrowLength - forward * this.arrowLength);
	}

	// Token: 0x040016B9 RID: 5817
	public Color color = new Color(1f, 1f, 1f, 1f);

	// Token: 0x040016BA RID: 5818
	public float length = 0.2f;

	// Token: 0x040016BB RID: 5819
	public float arrowLength = 0.02f;
}
