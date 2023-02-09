using System;
using UnityEngine;

// Token: 0x02000660 RID: 1632
public static class TerrainAnchorEx
{
	// Token: 0x06002E37 RID: 11831 RVA: 0x0011543E File Offset: 0x0011363E
	public static bool ApplyTerrainAnchors(this Transform transform, TerrainAnchor[] anchors, ref Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
	{
		return transform.ApplyTerrainAnchors(anchors, ref pos, rot, scale, TerrainAnchorMode.MinimizeError, filter);
	}

	// Token: 0x06002E38 RID: 11832 RVA: 0x00115450 File Offset: 0x00113650
	public static bool ApplyTerrainAnchors(this Transform transform, TerrainAnchor[] anchors, ref Vector3 pos, Quaternion rot, Vector3 scale, TerrainAnchorMode mode, SpawnFilter filter = null)
	{
		if (anchors.Length == 0)
		{
			return true;
		}
		float num = 0f;
		float num2 = float.MinValue;
		float num3 = float.MaxValue;
		foreach (TerrainAnchor terrainAnchor in anchors)
		{
			Vector3 vector = Vector3.Scale(terrainAnchor.worldPosition, scale);
			vector = rot * vector;
			Vector3 vector2 = pos + vector;
			if (TerrainMeta.OutOfBounds(vector2))
			{
				return false;
			}
			if (filter != null && filter.GetFactor(vector2, true) == 0f)
			{
				return false;
			}
			float num4;
			float num5;
			float num6;
			terrainAnchor.Apply(out num4, out num5, out num6, vector2, scale);
			num += num4 - vector.y;
			num2 = Mathf.Max(num2, num5 - vector.y);
			num3 = Mathf.Min(num3, num6 - vector.y);
			if (num3 < num2)
			{
				return false;
			}
		}
		if (num3 > 1f && num2 < 1f)
		{
			num2 = 1f;
		}
		if (mode == TerrainAnchorMode.MinimizeError)
		{
			pos.y = Mathf.Clamp(num / (float)anchors.Length, num2, num3);
		}
		else
		{
			pos.y = Mathf.Clamp(pos.y, num2, num3);
		}
		return true;
	}

	// Token: 0x06002E39 RID: 11833 RVA: 0x00115564 File Offset: 0x00113764
	public static void ApplyTerrainAnchors(this Transform transform, TerrainAnchor[] anchors)
	{
		Vector3 position = transform.position;
		transform.ApplyTerrainAnchors(anchors, ref position, transform.rotation, transform.lossyScale, null);
		transform.position = position;
	}
}
