using System;
using UnityEngine;

// Token: 0x020006C5 RID: 1733
public static class TerrainModifierEx
{
	// Token: 0x06003089 RID: 12425 RVA: 0x0012AD9C File Offset: 0x00128F9C
	public static void ApplyTerrainModifiers(this Transform transform, TerrainModifier[] modifiers, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		foreach (TerrainModifier terrainModifier in modifiers)
		{
			Vector3 point = Vector3.Scale(terrainModifier.worldPosition, scale);
			Vector3 pos2 = pos + rot * point;
			float y = scale.y;
			terrainModifier.Apply(pos2, y);
		}
	}

	// Token: 0x0600308A RID: 12426 RVA: 0x0012ADE5 File Offset: 0x00128FE5
	public static void ApplyTerrainModifiers(this Transform transform, TerrainModifier[] modifiers)
	{
		transform.ApplyTerrainModifiers(modifiers, transform.position, transform.rotation, transform.lossyScale);
	}
}
