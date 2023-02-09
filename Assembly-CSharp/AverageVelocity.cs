using System;
using UnityEngine;

// Token: 0x02000272 RID: 626
public class AverageVelocity
{
	// Token: 0x06001BD1 RID: 7121 RVA: 0x000C1230 File Offset: 0x000BF430
	public void Record(Vector3 newPos)
	{
		float num = Time.time - this.time;
		if (num < 0.1f)
		{
			return;
		}
		if (this.pos.sqrMagnitude > 0f)
		{
			Vector3 a = newPos - this.pos;
			this.averageVelocity = a * (1f / num);
			this.averageSpeed = this.averageVelocity.magnitude;
		}
		this.time = Time.time;
		this.pos = newPos;
	}

	// Token: 0x17000215 RID: 533
	// (get) Token: 0x06001BD2 RID: 7122 RVA: 0x000C12A8 File Offset: 0x000BF4A8
	public float Speed
	{
		get
		{
			return this.averageSpeed;
		}
	}

	// Token: 0x17000216 RID: 534
	// (get) Token: 0x06001BD3 RID: 7123 RVA: 0x000C12B0 File Offset: 0x000BF4B0
	public Vector3 Average
	{
		get
		{
			return this.averageVelocity;
		}
	}

	// Token: 0x040014E3 RID: 5347
	private Vector3 pos;

	// Token: 0x040014E4 RID: 5348
	private float time;

	// Token: 0x040014E5 RID: 5349
	private float lastEntry;

	// Token: 0x040014E6 RID: 5350
	private float averageSpeed;

	// Token: 0x040014E7 RID: 5351
	private Vector3 averageVelocity;
}
