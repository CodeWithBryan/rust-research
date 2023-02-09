using System;
using UnityEngine;

// Token: 0x02000628 RID: 1576
[Serializable]
public class SpawnFilter
{
	// Token: 0x06002D58 RID: 11608 RVA: 0x00111807 File Offset: 0x0010FA07
	public bool Test(Vector3 worldPos)
	{
		return this.GetFactor(worldPos, true) > 0.5f;
	}

	// Token: 0x06002D59 RID: 11609 RVA: 0x00111818 File Offset: 0x0010FA18
	public bool Test(float normX, float normZ)
	{
		return this.GetFactor(normX, normZ, true) > 0.5f;
	}

	// Token: 0x06002D5A RID: 11610 RVA: 0x0011182C File Offset: 0x0010FA2C
	public float GetFactor(Vector3 worldPos, bool checkPlacementMap = true)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetFactor(normX, normZ, checkPlacementMap);
	}

	// Token: 0x06002D5B RID: 11611 RVA: 0x0011185C File Offset: 0x0010FA5C
	public float GetFactor(float normX, float normZ, bool checkPlacementMap = true)
	{
		if (TerrainMeta.TopologyMap == null)
		{
			return 0f;
		}
		if (checkPlacementMap && TerrainMeta.PlacementMap != null && TerrainMeta.PlacementMap.GetBlocked(normX, normZ))
		{
			return 0f;
		}
		int splatType = (int)this.SplatType;
		int biomeType = (int)this.BiomeType;
		int topologyAny = (int)this.TopologyAny;
		int topologyAll = (int)this.TopologyAll;
		int topologyNot = (int)this.TopologyNot;
		if (topologyAny == 0)
		{
			Debug.LogError("Empty topology filter is invalid.");
		}
		else if (topologyAny != -1 || topologyAll != 0 || topologyNot != 0)
		{
			int topology = TerrainMeta.TopologyMap.GetTopology(normX, normZ);
			if (topologyAny != -1 && (topology & topologyAny) == 0)
			{
				return 0f;
			}
			if (topologyNot != 0 && (topology & topologyNot) != 0)
			{
				return 0f;
			}
			if (topologyAll != 0 && (topology & topologyAll) != topologyAll)
			{
				return 0f;
			}
		}
		if (biomeType == 0)
		{
			Debug.LogError("Empty biome filter is invalid.");
		}
		else if (biomeType != -1 && (TerrainMeta.BiomeMap.GetBiomeMaxType(normX, normZ, -1) & biomeType) == 0)
		{
			return 0f;
		}
		if (splatType == 0)
		{
			Debug.LogError("Empty splat filter is invalid.");
		}
		else if (splatType != -1)
		{
			return TerrainMeta.SplatMap.GetSplat(normX, normZ, splatType);
		}
		return 1f;
	}

	// Token: 0x04002521 RID: 9505
	[InspectorFlags]
	public TerrainSplat.Enum SplatType = (TerrainSplat.Enum)(-1);

	// Token: 0x04002522 RID: 9506
	[InspectorFlags]
	public TerrainBiome.Enum BiomeType = (TerrainBiome.Enum)(-1);

	// Token: 0x04002523 RID: 9507
	[InspectorFlags]
	public TerrainTopology.Enum TopologyAny = (TerrainTopology.Enum)(-1);

	// Token: 0x04002524 RID: 9508
	[InspectorFlags]
	public TerrainTopology.Enum TopologyAll;

	// Token: 0x04002525 RID: 9509
	[InspectorFlags]
	public TerrainTopology.Enum TopologyNot;
}
