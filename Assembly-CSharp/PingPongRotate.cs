using System;
using UnityEngine;

// Token: 0x020002F1 RID: 753
public class PingPongRotate : MonoBehaviour
{
	// Token: 0x06001D6C RID: 7532 RVA: 0x000C9354 File Offset: 0x000C7554
	private void Update()
	{
		Quaternion quaternion = Quaternion.identity;
		for (int i = 0; i < 3; i++)
		{
			quaternion *= this.GetRotation(i);
		}
		base.transform.rotation = quaternion;
	}

	// Token: 0x06001D6D RID: 7533 RVA: 0x000C9390 File Offset: 0x000C7590
	public Quaternion GetRotation(int index)
	{
		Vector3 axis = Vector3.zero;
		if (index == 0)
		{
			axis = Vector3.right;
		}
		else if (index == 1)
		{
			axis = Vector3.up;
		}
		else if (index == 2)
		{
			axis = Vector3.forward;
		}
		return Quaternion.AngleAxis(Mathf.Sin((this.offset[index] + Time.time) * this.rotationSpeed[index]) * this.rotationAmount[index], axis);
	}

	// Token: 0x040016CE RID: 5838
	public Vector3 rotationSpeed = Vector3.zero;

	// Token: 0x040016CF RID: 5839
	public Vector3 offset = Vector3.zero;

	// Token: 0x040016D0 RID: 5840
	public Vector3 rotationAmount = Vector3.zero;
}
