using System;
using UnityEngine;

// Token: 0x020006C9 RID: 1737
public class TerrainTopologySet : TerrainModifier
{
	// Token: 0x06003091 RID: 12433 RVA: 0x0012AE77 File Offset: 0x00129077
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.TopologyMap)
		{
			return;
		}
		TerrainMeta.TopologyMap.SetTopology(position, (int)this.TopologyType, radius, fade);
	}

	// Token: 0x0400277F RID: 10111
	[InspectorFlags]
	public TerrainTopology.Enum TopologyType = TerrainTopology.Enum.Decor;
}
