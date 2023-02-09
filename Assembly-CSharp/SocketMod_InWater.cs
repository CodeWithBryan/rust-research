using System;
using UnityEngine;

// Token: 0x0200025E RID: 606
public class SocketMod_InWater : SocketMod
{
	// Token: 0x06001B8E RID: 7054 RVA: 0x000BFE94 File Offset: 0x000BE094
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere(Vector3.zero, 0.1f);
	}

	// Token: 0x06001B8F RID: 7055 RVA: 0x000BFEC0 File Offset: 0x000BE0C0
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 vector = place.position + place.rotation * this.worldPosition;
		bool flag = WaterLevel.Test(vector, true, null);
		if (!flag && this.wantsInWater && GamePhysics.CheckSphere(vector, 0.1f, 16, QueryTriggerInteraction.UseGlobal))
		{
			flag = true;
		}
		if (flag == this.wantsInWater)
		{
			return true;
		}
		if (this.wantsInWater)
		{
			Construction.lastPlacementError = SocketMod_InWater.WantsWaterPhrase.translated;
		}
		else
		{
			Construction.lastPlacementError = SocketMod_InWater.NoWaterPhrase.translated;
		}
		return false;
	}

	// Token: 0x04001499 RID: 5273
	public bool wantsInWater = true;

	// Token: 0x0400149A RID: 5274
	public static Translate.Phrase WantsWaterPhrase = new Translate.Phrase("error_inwater_wants", "Must be placed in water");

	// Token: 0x0400149B RID: 5275
	public static Translate.Phrase NoWaterPhrase = new Translate.Phrase("error_inwater", "Can't be placed in water");
}
