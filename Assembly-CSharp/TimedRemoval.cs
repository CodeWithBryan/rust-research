using System;
using UnityEngine;

// Token: 0x020002CF RID: 719
public class TimedRemoval : MonoBehaviour
{
	// Token: 0x06001CC9 RID: 7369 RVA: 0x000C5299 File Offset: 0x000C3499
	private void OnEnable()
	{
		UnityEngine.Object.Destroy(this.objectToDestroy, this.removeDelay);
	}

	// Token: 0x0400167A RID: 5754
	public UnityEngine.Object objectToDestroy;

	// Token: 0x0400167B RID: 5755
	public float removeDelay = 1f;
}
