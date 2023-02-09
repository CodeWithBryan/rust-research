using System;
using UnityEngine;

// Token: 0x02000663 RID: 1635
public static class TerrainCheckEx
{
	// Token: 0x06002E3E RID: 11838 RVA: 0x0011563C File Offset: 0x0011383C
	public static bool ApplyTerrainChecks(this Transform transform, TerrainCheck[] anchors, Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
	{
		if (anchors.Length == 0)
		{
			return true;
		}
		foreach (TerrainCheck terrainCheck in anchors)
		{
			Vector3 vector = Vector3.Scale(terrainCheck.worldPosition, scale);
			if (terrainCheck.Rotate)
			{
				vector = rot * vector;
			}
			Vector3 vector2 = pos + vector;
			if (TerrainMeta.OutOfBounds(vector2))
			{
				return false;
			}
			if (filter != null && filter.GetFactor(vector2, true) == 0f)
			{
				return false;
			}
			if (!terrainCheck.Check(vector2))
			{
				return false;
			}
		}
		return true;
	}
}
