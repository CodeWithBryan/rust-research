using System;
using UnityEngine;

// Token: 0x020006C2 RID: 1730
public class TerrainHeightAdd : TerrainModifier
{
	// Token: 0x06003081 RID: 12417 RVA: 0x0012ACE9 File Offset: 0x00128EE9
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.HeightMap)
		{
			return;
		}
		TerrainMeta.HeightMap.AddHeight(position, opacity * this.Delta * TerrainMeta.OneOverSize.y, radius, fade);
	}

	// Token: 0x04002779 RID: 10105
	public float Delta = 1f;
}
