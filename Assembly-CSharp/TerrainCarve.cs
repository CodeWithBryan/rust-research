using System;
using UnityEngine;

// Token: 0x020006C1 RID: 1729
public class TerrainCarve : TerrainModifier
{
	// Token: 0x0600307F RID: 12415 RVA: 0x0012ACBE File Offset: 0x00128EBE
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.AlphaMap)
		{
			return;
		}
		TerrainMeta.AlphaMap.SetAlpha(position, 0f, opacity, radius, fade);
	}
}
