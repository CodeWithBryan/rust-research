using System;
using UnityEngine;

// Token: 0x020006C3 RID: 1731
public class TerrainHeightSet : TerrainModifier
{
	// Token: 0x06003083 RID: 12419 RVA: 0x0012AD2C File Offset: 0x00128F2C
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.HeightMap)
		{
			return;
		}
		TerrainMeta.HeightMap.SetHeight(position, opacity, radius, fade);
	}
}
