using System;
using UnityEngine;

// Token: 0x020002F2 RID: 754
public class RotateObject : MonoBehaviour
{
	// Token: 0x06001D6F RID: 7535 RVA: 0x000C9424 File Offset: 0x000C7624
	private void Awake()
	{
		this.rotateVector = new Vector3(this.rotateSpeed_X, this.rotateSpeed_Y, this.rotateSpeed_Z);
	}

	// Token: 0x06001D70 RID: 7536 RVA: 0x000C9444 File Offset: 0x000C7644
	private void Update()
	{
		if (this.localSpace)
		{
			base.transform.Rotate(this.rotateVector * Time.deltaTime, Space.Self);
			return;
		}
		if (this.rotateSpeed_X != 0f)
		{
			base.transform.Rotate(Vector3.up, Time.deltaTime * this.rotateSpeed_X);
		}
		if (this.rotateSpeed_Y != 0f)
		{
			base.transform.Rotate(base.transform.forward, Time.deltaTime * this.rotateSpeed_Y);
		}
		if (this.rotateSpeed_Z != 0f)
		{
			base.transform.Rotate(base.transform.right, Time.deltaTime * this.rotateSpeed_Z);
		}
	}

	// Token: 0x040016D1 RID: 5841
	public float rotateSpeed_X = 1f;

	// Token: 0x040016D2 RID: 5842
	public float rotateSpeed_Y = 1f;

	// Token: 0x040016D3 RID: 5843
	public float rotateSpeed_Z = 1f;

	// Token: 0x040016D4 RID: 5844
	public bool localSpace;

	// Token: 0x040016D5 RID: 5845
	private Vector3 rotateVector;
}
