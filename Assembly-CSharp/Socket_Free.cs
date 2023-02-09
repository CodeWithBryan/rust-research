using System;
using UnityEngine;

// Token: 0x02000265 RID: 613
public class Socket_Free : Socket_Base
{
	// Token: 0x06001BAC RID: 7084 RVA: 0x000C0828 File Offset: 0x000BEA28
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward * 1f);
		GizmosUtil.DrawWireCircleZ(Vector3.forward * 0f, 0.2f);
		Gizmos.DrawIcon(base.transform.position, "light_circle_green.png", false);
	}

	// Token: 0x06001BAD RID: 7085 RVA: 0x000C0897 File Offset: 0x000BEA97
	public override bool TestTarget(Construction.Target target)
	{
		return target.onTerrain;
	}

	// Token: 0x06001BAE RID: 7086 RVA: 0x000C08A0 File Offset: 0x000BEAA0
	public override Construction.Placement DoPlacement(Construction.Target target)
	{
		Quaternion rotation = Quaternion.identity;
		if (this.useTargetNormal)
		{
			if (this.blendAimAngle)
			{
				Vector3 vector = (target.position - target.ray.origin).normalized;
				float t = Mathf.Abs(Vector3.Dot(vector, target.normal));
				vector = Vector3.Lerp(vector, this.idealPlacementNormal, t);
				rotation = Quaternion.LookRotation(target.normal, vector) * Quaternion.Inverse(this.rotation) * Quaternion.Euler(target.rotation);
			}
			else
			{
				rotation = Quaternion.LookRotation(target.normal);
			}
		}
		else
		{
			Vector3 normalized = (target.position - target.ray.origin).normalized;
			normalized.y = 0f;
			rotation = Quaternion.LookRotation(normalized, this.idealPlacementNormal) * Quaternion.Euler(target.rotation);
		}
		Vector3 vector2 = target.position;
		vector2 -= rotation * this.position;
		return new Construction.Placement
		{
			rotation = rotation,
			position = vector2
		};
	}

	// Token: 0x040014BB RID: 5307
	public Vector3 idealPlacementNormal = Vector3.up;

	// Token: 0x040014BC RID: 5308
	public bool useTargetNormal = true;

	// Token: 0x040014BD RID: 5309
	public bool blendAimAngle = true;
}
