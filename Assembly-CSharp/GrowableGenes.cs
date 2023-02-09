using System;
using System.Linq;
using System.Text;
using UnityEngine;

// Token: 0x020003D3 RID: 979
public class GrowableGenes
{
	// Token: 0x06002164 RID: 8548 RVA: 0x000D6FE9 File Offset: 0x000D51E9
	public GrowableGenes()
	{
		this.Clear();
	}

	// Token: 0x06002165 RID: 8549 RVA: 0x000D6FF8 File Offset: 0x000D51F8
	private void Clear()
	{
		this.Genes = new GrowableGene[6];
		for (int i = 0; i < 6; i++)
		{
			this.Genes[i] = new GrowableGene();
		}
	}

	// Token: 0x06002166 RID: 8550 RVA: 0x000D702C File Offset: 0x000D522C
	public void GenerateRandom(GrowableEntity growable)
	{
		if (growable == null)
		{
			return;
		}
		if (growable.Properties.Genes == null)
		{
			return;
		}
		this.CalculateBaseWeights(growable.Properties.Genes);
		for (int i = 0; i < 6; i++)
		{
			this.CalculateSlotWeights(growable.Properties.Genes, i);
			this.Genes[i].Set(this.PickWeightedGeneType(), true);
		}
	}

	// Token: 0x06002167 RID: 8551 RVA: 0x000D709C File Offset: 0x000D529C
	private void CalculateBaseWeights(GrowableGeneProperties properties)
	{
		int num = 0;
		foreach (GrowableGeneProperties.GeneWeight geneWeight in properties.Weights)
		{
			GrowableGenes.baseWeights[num].GeneType = (GrowableGenes.slotWeights[num].GeneType = (GrowableGenetics.GeneType)num);
			GrowableGenes.baseWeights[num].Weighting = geneWeight.BaseWeight;
			num++;
		}
	}

	// Token: 0x06002168 RID: 8552 RVA: 0x000D7108 File Offset: 0x000D5308
	private void CalculateSlotWeights(GrowableGeneProperties properties, int slot)
	{
		int num = 0;
		foreach (GrowableGeneProperties.GeneWeight geneWeight in properties.Weights)
		{
			GrowableGenes.slotWeights[num].Weighting = GrowableGenes.baseWeights[num].Weighting + geneWeight.SlotWeights[slot];
			num++;
		}
	}

	// Token: 0x06002169 RID: 8553 RVA: 0x000D7164 File Offset: 0x000D5364
	private GrowableGenetics.GeneType PickWeightedGeneType()
	{
		IOrderedEnumerable<GrowableGenetics.GeneWeighting> orderedEnumerable = from w in GrowableGenes.slotWeights
		orderby w.Weighting
		select w;
		float num = 0f;
		foreach (GrowableGenetics.GeneWeighting geneWeighting in orderedEnumerable)
		{
			num += geneWeighting.Weighting;
		}
		GrowableGenetics.GeneType result = GrowableGenetics.GeneType.Empty;
		float num2 = UnityEngine.Random.Range(0f, num);
		float num3 = 0f;
		foreach (GrowableGenetics.GeneWeighting geneWeighting2 in orderedEnumerable)
		{
			num3 += geneWeighting2.Weighting;
			if (num2 < num3)
			{
				result = geneWeighting2.GeneType;
				break;
			}
		}
		return result;
	}

	// Token: 0x0600216A RID: 8554 RVA: 0x000D724C File Offset: 0x000D544C
	public int GetGeneTypeCount(GrowableGenetics.GeneType geneType)
	{
		int num = 0;
		GrowableGene[] genes = this.Genes;
		for (int i = 0; i < genes.Length; i++)
		{
			if (genes[i].Type == geneType)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x0600216B RID: 8555 RVA: 0x000D7280 File Offset: 0x000D5480
	public int GetPositiveGeneCount()
	{
		int num = 0;
		GrowableGene[] genes = this.Genes;
		for (int i = 0; i < genes.Length; i++)
		{
			if (genes[i].IsPositive())
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x0600216C RID: 8556 RVA: 0x000D72B4 File Offset: 0x000D54B4
	public int GetNegativeGeneCount()
	{
		int num = 0;
		GrowableGene[] genes = this.Genes;
		for (int i = 0; i < genes.Length; i++)
		{
			if (!genes[i].IsPositive())
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x0600216D RID: 8557 RVA: 0x000D72E7 File Offset: 0x000D54E7
	public void Save(BaseNetworkable.SaveInfo info)
	{
		info.msg.growableEntity.genes = GrowableGeneEncoding.EncodeGenesToInt(this);
		info.msg.growableEntity.previousGenes = GrowableGeneEncoding.EncodePreviousGenesToInt(this);
	}

	// Token: 0x0600216E RID: 8558 RVA: 0x000D7315 File Offset: 0x000D5515
	public void Load(BaseNetworkable.LoadInfo info)
	{
		if (info.msg.growableEntity == null)
		{
			return;
		}
		GrowableGeneEncoding.DecodeIntToGenes(info.msg.growableEntity.genes, this);
		GrowableGeneEncoding.DecodeIntToPreviousGenes(info.msg.growableEntity.previousGenes, this);
	}

	// Token: 0x0600216F RID: 8559 RVA: 0x000D7351 File Offset: 0x000D5551
	public void DebugPrint()
	{
		Debug.Log(this.GetDisplayString(false));
	}

	// Token: 0x06002170 RID: 8560 RVA: 0x000D7360 File Offset: 0x000D5560
	private string GetDisplayString(bool previousGenes)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < 6; i++)
		{
			stringBuilder.Append(GrowableGene.GetDisplayCharacter(previousGenes ? this.Genes[i].PreviousType : this.Genes[i].Type));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x040019AF RID: 6575
	public GrowableGene[] Genes;

	// Token: 0x040019B0 RID: 6576
	private static GrowableGenetics.GeneWeighting[] baseWeights = new GrowableGenetics.GeneWeighting[6];

	// Token: 0x040019B1 RID: 6577
	private static GrowableGenetics.GeneWeighting[] slotWeights = new GrowableGenetics.GeneWeighting[6];
}
