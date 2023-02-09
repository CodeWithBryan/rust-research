using System;
using UnityEngine;

// Token: 0x020006C8 RID: 1736
public class TerrainTopologyAdd : TerrainModifier
{
	// Token: 0x0600308F RID: 12431 RVA: 0x0012AE41 File Offset: 0x00129041
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.TopologyMap)
		{
			return;
		}
		TerrainMeta.TopologyMap.AddTopology(position, (int)this.TopologyType, radius, fade);
	}

	// Token: 0x0400277E RID: 10110
	[InspectorFlags]
	public TerrainTopology.Enum TopologyType = TerrainTopology.Enum.Decor;
}
