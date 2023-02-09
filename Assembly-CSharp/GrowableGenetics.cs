using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020003D4 RID: 980
public static class GrowableGenetics
{
	// Token: 0x06002172 RID: 8562 RVA: 0x000D73C8 File Offset: 0x000D55C8
	public static void CrossBreed(GrowableEntity growable)
	{
		List<GrowableEntity> list = Pool.GetList<GrowableEntity>();
		Vis.Entities<GrowableEntity>(growable.transform.position, 1.5f, list, 512, QueryTriggerInteraction.Collide);
		bool flag = false;
		for (int i = 0; i < 6; i++)
		{
			GrowableGene growableGene = growable.Genes.Genes[i];
			GrowableGenetics.GeneWeighting dominantGeneWeighting = GrowableGenetics.GetDominantGeneWeighting(growable, list, i);
			if (dominantGeneWeighting.Weighting > growable.Properties.Genes.Weights[(int)growableGene.Type].CrossBreedingWeight)
			{
				flag = true;
				growableGene.Set(dominantGeneWeighting.GeneType, false);
			}
		}
		if (flag)
		{
			growable.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06002173 RID: 8563 RVA: 0x000D7460 File Offset: 0x000D5660
	private static GrowableGenetics.GeneWeighting GetDominantGeneWeighting(GrowableEntity crossBreedingGrowable, List<GrowableEntity> neighbours, int slot)
	{
		PlanterBox planter = crossBreedingGrowable.GetPlanter();
		if (planter == null)
		{
			GrowableGenetics.dominant.Weighting = -1f;
			return GrowableGenetics.dominant;
		}
		for (int i = 0; i < GrowableGenetics.neighbourWeights.Length; i++)
		{
			GrowableGenetics.neighbourWeights[i].Weighting = 0f;
			GrowableGenetics.neighbourWeights[i].GeneType = (GrowableGenetics.GeneType)i;
		}
		GrowableGenetics.dominant.Weighting = 0f;
		foreach (GrowableEntity growableEntity in neighbours)
		{
			if (growableEntity.isServer)
			{
				PlanterBox planter2 = growableEntity.GetPlanter();
				if (!(planter2 == null) && !(planter2 != planter) && !(growableEntity == crossBreedingGrowable) && growableEntity.prefabID == crossBreedingGrowable.prefabID && !growableEntity.IsDead())
				{
					GrowableGenetics.GeneType type = growableEntity.Genes.Genes[slot].Type;
					float crossBreedingWeight = growableEntity.Properties.Genes.Weights[(int)type].CrossBreedingWeight;
					GrowableGenetics.GeneWeighting[] array = GrowableGenetics.neighbourWeights;
					GrowableGenetics.GeneType geneType = type;
					float num = array[(int)geneType].Weighting = array[(int)geneType].Weighting + crossBreedingWeight;
					if (num > GrowableGenetics.dominant.Weighting)
					{
						GrowableGenetics.dominant.Weighting = num;
						GrowableGenetics.dominant.GeneType = type;
					}
				}
			}
		}
		return GrowableGenetics.dominant;
	}

	// Token: 0x040019B2 RID: 6578
	public const int GeneSlotCount = 6;

	// Token: 0x040019B3 RID: 6579
	public const float CrossBreedingRadius = 1.5f;

	// Token: 0x040019B4 RID: 6580
	private static GrowableGenetics.GeneWeighting[] neighbourWeights = new GrowableGenetics.GeneWeighting[Enum.GetValues(typeof(GrowableGenetics.GeneType)).Length];

	// Token: 0x040019B5 RID: 6581
	private static GrowableGenetics.GeneWeighting dominant = default(GrowableGenetics.GeneWeighting);

	// Token: 0x02000C75 RID: 3189
	public enum GeneType
	{
		// Token: 0x04004263 RID: 16995
		Empty,
		// Token: 0x04004264 RID: 16996
		WaterRequirement,
		// Token: 0x04004265 RID: 16997
		GrowthSpeed,
		// Token: 0x04004266 RID: 16998
		Yield,
		// Token: 0x04004267 RID: 16999
		Hardiness
	}

	// Token: 0x02000C76 RID: 3190
	public struct GeneWeighting
	{
		// Token: 0x04004268 RID: 17000
		public float Weighting;

		// Token: 0x04004269 RID: 17001
		public GrowableGenetics.GeneType GeneType;
	}
}
