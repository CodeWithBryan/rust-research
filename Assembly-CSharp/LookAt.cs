using System;
using UnityEngine;

// Token: 0x020002AD RID: 685
[ExecuteInEditMode]
public class LookAt : MonoBehaviour, IClientComponent
{
	// Token: 0x06001C44 RID: 7236 RVA: 0x000C351A File Offset: 0x000C171A
	private void Update()
	{
		if (this.target == null)
		{
			return;
		}
		base.transform.LookAt(this.target);
	}

	// Token: 0x040015A5 RID: 5541
	public Transform target;
}
