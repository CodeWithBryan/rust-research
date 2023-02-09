using System;
using UnityEngine;

// Token: 0x020008CB RID: 2251
public class IgnoreCollision : MonoBehaviour
{
	// Token: 0x06003632 RID: 13874 RVA: 0x001439A7 File Offset: 0x00141BA7
	protected void OnTriggerEnter(Collider other)
	{
		Debug.Log("IgnoreCollision: " + this.collider.gameObject.name + " + " + other.gameObject.name);
		Physics.IgnoreCollision(other, this.collider, true);
	}

	// Token: 0x0400312A RID: 12586
	public Collider collider;
}
