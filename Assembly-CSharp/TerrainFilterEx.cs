using System;
using UnityEngine;

// Token: 0x02000684 RID: 1668
public static class TerrainFilterEx
{
	// Token: 0x06002FBE RID: 12222 RVA: 0x0011CE04 File Offset: 0x0011B004
	public static bool ApplyTerrainFilters(this Transform transform, TerrainFilter[] filters, Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter globalFilter = null)
	{
		if (filters.Length == 0)
		{
			return true;
		}
		foreach (TerrainFilter terrainFilter in filters)
		{
			Vector3 vector = Vector3.Scale(terrainFilter.worldPosition, scale);
			vector = rot * vector;
			Vector3 vector2 = pos + vector;
			if (TerrainMeta.OutOfBounds(vector2))
			{
				return false;
			}
			if (globalFilter != null && globalFilter.GetFactor(vector2, true) == 0f)
			{
				return false;
			}
			if (!terrainFilter.Check(vector2))
			{
				return false;
			}
		}
		return true;
	}
}
