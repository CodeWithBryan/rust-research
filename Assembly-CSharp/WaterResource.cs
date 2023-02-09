using System;
using UnityEngine;

// Token: 0x02000576 RID: 1398
public class WaterResource
{
	// Token: 0x06002A47 RID: 10823 RVA: 0x000FFAD5 File Offset: 0x000FDCD5
	public static ItemDefinition GetAtPoint(Vector3 pos)
	{
		return ItemManager.FindItemDefinition(WaterResource.IsFreshWater(pos) ? "water" : "water.salt");
	}

	// Token: 0x06002A48 RID: 10824 RVA: 0x000FFAF0 File Offset: 0x000FDCF0
	public static bool IsFreshWater(Vector3 pos)
	{
		return !(TerrainMeta.TopologyMap == null) && TerrainMeta.TopologyMap.GetTopology(pos, 245760);
	}

	// Token: 0x06002A49 RID: 10825 RVA: 0x000FFB14 File Offset: 0x000FDD14
	public static ItemDefinition Merge(ItemDefinition first, ItemDefinition second)
	{
		if (first == second)
		{
			return first;
		}
		if (first.shortname == "water.salt" || second.shortname == "water.salt")
		{
			return ItemManager.FindItemDefinition("water.salt");
		}
		return ItemManager.FindItemDefinition("water");
	}
}
