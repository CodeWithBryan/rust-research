using System;
using UnityEngine;

// Token: 0x020006E0 RID: 1760
public static class WaterCheckEx
{
	// Token: 0x0600315D RID: 12637 RVA: 0x0012FACC File Offset: 0x0012DCCC
	public static bool ApplyWaterChecks(this Transform transform, WaterCheck[] anchors, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		if (anchors.Length == 0)
		{
			return true;
		}
		foreach (WaterCheck waterCheck in anchors)
		{
			Vector3 vector = Vector3.Scale(waterCheck.worldPosition, scale);
			if (waterCheck.Rotate)
			{
				vector = rot * vector;
			}
			Vector3 pos2 = pos + vector;
			if (!waterCheck.Check(pos2))
			{
				return false;
			}
		}
		return true;
	}
}
