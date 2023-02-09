using System;
using UnityEngine;

// Token: 0x02000205 RID: 517
public class AimConeUtil
{
	// Token: 0x06001A83 RID: 6787 RVA: 0x000BB628 File Offset: 0x000B9828
	public static Vector3 GetModifiedAimConeDirection(float aimCone, Vector3 inputVec, bool anywhereInside = true)
	{
		Quaternion lhs = Quaternion.LookRotation(inputVec);
		Vector2 vector = anywhereInside ? UnityEngine.Random.insideUnitCircle : UnityEngine.Random.insideUnitCircle.normalized;
		return lhs * Quaternion.Euler(vector.x * aimCone * 0.5f, vector.y * aimCone * 0.5f, 0f) * Vector3.forward;
	}

	// Token: 0x06001A84 RID: 6788 RVA: 0x000BB688 File Offset: 0x000B9888
	public static Quaternion GetAimConeQuat(float aimCone)
	{
		Vector3 insideUnitSphere = UnityEngine.Random.insideUnitSphere;
		return Quaternion.Euler(insideUnitSphere.x * aimCone * 0.5f, insideUnitSphere.y * aimCone * 0.5f, 0f);
	}
}
