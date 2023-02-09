using System;
using UnityEngine;

// Token: 0x02000257 RID: 599
public class SocketMod_AngleCheck : SocketMod
{
	// Token: 0x06001B75 RID: 7029 RVA: 0x000BF4BC File Offset: 0x000BD6BC
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.yellow;
		Gizmos.DrawFrustum(Vector3.zero, this.withinDegrees, 1f, 0f, 1f);
	}

	// Token: 0x06001B76 RID: 7030 RVA: 0x000BF4F7 File Offset: 0x000BD6F7
	public override bool DoCheck(Construction.Placement place)
	{
		if (this.worldNormal.DotDegrees(place.rotation * Vector3.up) < this.withinDegrees)
		{
			return true;
		}
		Construction.lastPlacementError = SocketMod_AngleCheck.ErrorPhrase.translated;
		return false;
	}

	// Token: 0x0400147E RID: 5246
	public bool wantsAngle = true;

	// Token: 0x0400147F RID: 5247
	public Vector3 worldNormal = Vector3.up;

	// Token: 0x04001480 RID: 5248
	public float withinDegrees = 45f;

	// Token: 0x04001481 RID: 5249
	public static Translate.Phrase ErrorPhrase = new Translate.Phrase("error_anglecheck", "Invalid angle");
}
