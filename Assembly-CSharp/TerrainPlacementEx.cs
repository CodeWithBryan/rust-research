using System;
using UnityEngine;

// Token: 0x020006CD RID: 1741
public static class TerrainPlacementEx
{
	// Token: 0x060030B2 RID: 12466 RVA: 0x0012BE68 File Offset: 0x0012A068
	public static void ApplyTerrainPlacements(this Transform transform, TerrainPlacement[] placements, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		if (placements.Length == 0)
		{
			return;
		}
		Matrix4x4 localToWorld = Matrix4x4.TRS(pos, rot, scale);
		Matrix4x4 inverse = localToWorld.inverse;
		for (int i = 0; i < placements.Length; i++)
		{
			placements[i].Apply(localToWorld, inverse);
		}
	}

	// Token: 0x060030B3 RID: 12467 RVA: 0x0012BEA4 File Offset: 0x0012A0A4
	public static void ApplyTerrainPlacements(this Transform transform, TerrainPlacement[] placements)
	{
		transform.ApplyTerrainPlacements(placements, transform.position, transform.rotation, transform.lossyScale);
	}
}
