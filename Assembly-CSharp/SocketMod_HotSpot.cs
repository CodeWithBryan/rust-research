using System;
using UnityEngine;

// Token: 0x0200025D RID: 605
public class SocketMod_HotSpot : SocketMod
{
	// Token: 0x06001B8B RID: 7051 RVA: 0x000BFE0F File Offset: 0x000BE00F
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
		Gizmos.DrawSphere(Vector3.zero, this.spotSize);
	}

	// Token: 0x06001B8C RID: 7052 RVA: 0x000BFE50 File Offset: 0x000BE050
	public override void ModifyPlacement(Construction.Placement place)
	{
		Vector3 position = place.position + place.rotation * this.worldPosition;
		place.position = position;
	}

	// Token: 0x04001498 RID: 5272
	public float spotSize = 0.1f;
}
