using System;
using UnityEngine;

// Token: 0x02000261 RID: 609
public class SocketMod_SphereCheck : SocketMod
{
	// Token: 0x06001B97 RID: 7063 RVA: 0x000C0184 File Offset: 0x000BE384
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = (this.wantsCollide ? new Color(0f, 1f, 0f, 0.7f) : new Color(1f, 0f, 0f, 0.7f));
		Gizmos.DrawSphere(Vector3.zero, this.sphereRadius);
	}

	// Token: 0x06001B98 RID: 7064 RVA: 0x000C01F4 File Offset: 0x000BE3F4
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 position = place.position + place.rotation * this.worldPosition;
		if (this.wantsCollide == GamePhysics.CheckSphere(position, this.sphereRadius, this.layerMask.value, QueryTriggerInteraction.UseGlobal))
		{
			return true;
		}
		bool flag = false;
		Construction.lastPlacementError = "Failed Check: Sphere Test (" + this.hierachyName + ")";
		if (this.layerMask == 2097152 && this.wantsCollide)
		{
			Construction.lastPlacementError = SocketMod_SphereCheck.Error_WantsCollideConstruction.translated;
			if (flag)
			{
				Construction.lastPlacementError = Construction.lastPlacementError + " (" + this.hierachyName + ")";
			}
		}
		else if (!this.wantsCollide && (this.layerMask & 2097152) == 2097152)
		{
			Construction.lastPlacementError = SocketMod_SphereCheck.Error_DoesNotWantCollideConstruction.translated;
			if (flag)
			{
				Construction.lastPlacementError = Construction.lastPlacementError + " (" + this.hierachyName + ")";
			}
		}
		else
		{
			Construction.lastPlacementError = "Failed Check: Sphere Test (" + this.hierachyName + ")";
		}
		return false;
	}

	// Token: 0x040014A2 RID: 5282
	public float sphereRadius = 1f;

	// Token: 0x040014A3 RID: 5283
	public LayerMask layerMask;

	// Token: 0x040014A4 RID: 5284
	public bool wantsCollide;

	// Token: 0x040014A5 RID: 5285
	public static Translate.Phrase Error_WantsCollideConstruction = new Translate.Phrase("error_wantsconstruction", "Must be placed on construction");

	// Token: 0x040014A6 RID: 5286
	public static Translate.Phrase Error_DoesNotWantCollideConstruction = new Translate.Phrase("error_doesnotwantconstruction", "Cannot be placed on construction");
}
