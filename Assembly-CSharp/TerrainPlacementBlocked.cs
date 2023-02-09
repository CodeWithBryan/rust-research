using System;
using UnityEngine;

// Token: 0x020006C6 RID: 1734
public class TerrainPlacementBlocked : TerrainModifier
{
	// Token: 0x0600308B RID: 12427 RVA: 0x0012AE00 File Offset: 0x00129000
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.PlacementMap)
		{
			return;
		}
		TerrainMeta.PlacementMap.SetBlocked(position, radius, fade);
	}
}
