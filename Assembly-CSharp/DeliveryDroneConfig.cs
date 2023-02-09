using System;
using UnityEngine;

// Token: 0x02000161 RID: 353
[CreateAssetMenu(menuName = "Rust/Delivery Drone Config")]
public class DeliveryDroneConfig : BaseScriptableObject
{
	// Token: 0x0600167C RID: 5756 RVA: 0x000AAFD4 File Offset: 0x000A91D4
	public void FindDescentPoints(VendingMachine vendingMachine, float currentY, out Vector3 waitPosition, out Vector3 descendPosition)
	{
		float num = this.maxDistanceFromVendingMachine / 4f;
		for (int i = 0; i <= 4; i++)
		{
			Vector3 b = Vector3.forward * (num * (float)i);
			Vector3 vector = vendingMachine.transform.TransformPoint(this.vendingMachineOffset + b);
			Vector3 vector2 = vector + Vector3.up * this.testHeight;
			RaycastHit raycastHit;
			if (!Physics.BoxCast(vector2, this.halfExtents, Vector3.down, out raycastHit, vendingMachine.transform.rotation, this.testHeight, this.layerMask))
			{
				waitPosition = vector;
				descendPosition = vector2.WithY(currentY);
				return;
			}
			if (i == 4)
			{
				waitPosition = vector2 + Vector3.down * (raycastHit.distance - this.halfExtents.y * 2f);
				descendPosition = vector2.WithY(currentY);
				return;
			}
		}
		throw new Exception("Bug: FindDescentPoint didn't return a fallback value");
	}

	// Token: 0x0600167D RID: 5757 RVA: 0x000AB0D8 File Offset: 0x000A92D8
	public bool IsVendingMachineAccessible(VendingMachine vendingMachine, Vector3 offset, out RaycastHit hitInfo)
	{
		Vector3 vector = vendingMachine.transform.TransformPoint(offset);
		return !Physics.BoxCast(vector + Vector3.up * this.testHeight, this.halfExtents, Vector3.down, out hitInfo, vendingMachine.transform.rotation, this.testHeight, this.layerMask) && vendingMachine.IsVisibleAndCanSee(vector, 2f);
	}

	// Token: 0x04000F6A RID: 3946
	public Vector3 vendingMachineOffset = new Vector3(0f, 1f, 1f);

	// Token: 0x04000F6B RID: 3947
	public float maxDistanceFromVendingMachine = 1f;

	// Token: 0x04000F6C RID: 3948
	public Vector3 halfExtents = new Vector3(0.5f, 0.5f, 0.5f);

	// Token: 0x04000F6D RID: 3949
	public float testHeight = 200f;

	// Token: 0x04000F6E RID: 3950
	public LayerMask layerMask = 27328768;
}
