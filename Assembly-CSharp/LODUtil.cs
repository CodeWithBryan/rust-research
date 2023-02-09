using System;
using UnityEngine;

// Token: 0x0200050F RID: 1295
public static class LODUtil
{
	// Token: 0x06002846 RID: 10310 RVA: 0x000F60EF File Offset: 0x000F42EF
	public static float GetDistance(Transform transform, LODDistanceMode mode = LODDistanceMode.XYZ)
	{
		return LODUtil.GetDistance(transform.position, mode);
	}

	// Token: 0x06002847 RID: 10311 RVA: 0x000F6100 File Offset: 0x000F4300
	public static float GetDistance(Vector3 worldPos, LODDistanceMode mode = LODDistanceMode.XYZ)
	{
		if (MainCamera.isValid)
		{
			switch (mode)
			{
			case LODDistanceMode.XYZ:
				return Vector3.Distance(MainCamera.position, worldPos);
			case LODDistanceMode.XZ:
				return Vector3Ex.Distance2D(MainCamera.position, worldPos);
			case LODDistanceMode.Y:
				return Mathf.Abs(MainCamera.position.y - worldPos.y);
			}
		}
		return 1000f;
	}

	// Token: 0x06002848 RID: 10312 RVA: 0x000F615C File Offset: 0x000F435C
	public static float VerifyDistance(float distance)
	{
		return Mathf.Min(500f, distance);
	}

	// Token: 0x06002849 RID: 10313 RVA: 0x000F6169 File Offset: 0x000F4369
	public static LODEnvironmentMode DetermineEnvironmentMode(Transform transform)
	{
		if (transform.CompareTag("OnlyVisibleUnderground") || transform.root.CompareTag("OnlyVisibleUnderground"))
		{
			return LODEnvironmentMode.Underground;
		}
		return LODEnvironmentMode.Default;
	}

	// Token: 0x040020B4 RID: 8372
	public const float DefaultDistance = 1000f;
}
