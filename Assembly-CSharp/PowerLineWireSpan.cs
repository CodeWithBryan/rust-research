using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200065C RID: 1628
public class PowerLineWireSpan : MonoBehaviour
{
	// Token: 0x06002E30 RID: 11824 RVA: 0x00115178 File Offset: 0x00113378
	public void Init(PowerLineWire wire)
	{
		if (this.start && this.end)
		{
			this.WireLength = Vector3.Distance(this.start.position, this.end.position);
			for (int i = 0; i < this.connections.Count; i++)
			{
				Vector3 a = this.start.TransformPoint(this.connections[i].outOffset);
				Vector3 vector = this.end.TransformPoint(this.connections[i].inOffset);
				this.WireLength = (a - vector).magnitude;
				GameObject gameObject = this.wirePrefab.Instantiate(base.transform);
				gameObject.name = "WIRE";
				gameObject.transform.position = Vector3.Lerp(a, vector, 0.5f);
				gameObject.transform.LookAt(vector);
				gameObject.transform.localScale = new Vector3(1f, 1f, Vector3.Distance(a, vector));
				gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x040025E4 RID: 9700
	public GameObjectRef wirePrefab;

	// Token: 0x040025E5 RID: 9701
	public Transform start;

	// Token: 0x040025E6 RID: 9702
	public Transform end;

	// Token: 0x040025E7 RID: 9703
	public float WireLength;

	// Token: 0x040025E8 RID: 9704
	public List<PowerLineWireConnection> connections = new List<PowerLineWireConnection>();
}
