using System;
using UnityEngine;

// Token: 0x020001CC RID: 460
public class AITraversalWaitPoint : MonoBehaviour
{
	// Token: 0x0600186A RID: 6250 RVA: 0x000B403E File Offset: 0x000B223E
	public bool Occupied()
	{
		return Time.time > this.nextFreeTime;
	}

	// Token: 0x0600186B RID: 6251 RVA: 0x000B404D File Offset: 0x000B224D
	public void Occupy(float dur = 1f)
	{
		this.nextFreeTime = Time.time + dur;
	}

	// Token: 0x04001185 RID: 4485
	public float nextFreeTime;
}
