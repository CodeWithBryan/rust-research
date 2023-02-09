using System;
using UnityEngine;

// Token: 0x0200025F RID: 607
public class SocketMod_PhysicMaterial : SocketMod
{
	// Token: 0x06001B92 RID: 7058 RVA: 0x000BFF80 File Offset: 0x000BE180
	public override bool DoCheck(Construction.Placement place)
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(place.position + place.rotation.eulerAngles.normalized * 0.5f, -place.rotation.eulerAngles.normalized, out raycastHit, 1f, 27328512, QueryTriggerInteraction.Ignore))
		{
			this.foundMaterial = raycastHit.collider.GetMaterialAt(raycastHit.point);
			PhysicMaterial[] validMaterials = this.ValidMaterials;
			for (int i = 0; i < validMaterials.Length; i++)
			{
				if (validMaterials[i] == this.foundMaterial)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0400149C RID: 5276
	public PhysicMaterial[] ValidMaterials;

	// Token: 0x0400149D RID: 5277
	private PhysicMaterial foundMaterial;
}
