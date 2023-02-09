using System;
using UnityEngine;

// Token: 0x020008D5 RID: 2261
public class MoveForward : MonoBehaviour
{
	// Token: 0x06003652 RID: 13906 RVA: 0x00143F79 File Offset: 0x00142179
	protected void Update()
	{
		base.GetComponent<Rigidbody>().velocity = this.Speed * base.transform.forward;
	}

	// Token: 0x0400314C RID: 12620
	public float Speed = 2f;
}
