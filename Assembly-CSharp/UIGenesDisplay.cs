using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200089C RID: 2204
public class UIGenesDisplay : MonoBehaviour
{
	// Token: 0x060035BC RID: 13756 RVA: 0x00142688 File Offset: 0x00140888
	public void Init(GrowableGenes genes)
	{
		int num = 0;
		foreach (GrowableGene gene in genes.Genes)
		{
			this.GeneUI[num].Init(gene);
			num++;
			if (num < genes.Genes.Length)
			{
				this.TextLinks[num - 1].color = (genes.Genes[num].IsPositive() ? this.GeneUI[num - 1].PositiveColour : this.GeneUI[num - 1].NegativeColour);
			}
		}
	}

	// Token: 0x060035BD RID: 13757 RVA: 0x0014270B File Offset: 0x0014090B
	public void InitDualRow(GrowableGenes genes, bool firstRow)
	{
		if (firstRow)
		{
			this.InitFirstRow(genes);
			return;
		}
		this.InitSecondRow(genes);
	}

	// Token: 0x060035BE RID: 13758 RVA: 0x00142720 File Offset: 0x00140920
	private void InitFirstRow(GrowableGenes genes)
	{
		int num = 0;
		foreach (GrowableGene growableGene in genes.Genes)
		{
			if (growableGene.Type != growableGene.PreviousType)
			{
				this.GeneUI[num].InitPrevious(growableGene);
			}
			else
			{
				this.GeneUI[num].Init(growableGene);
			}
			num++;
			if (num >= genes.Genes.Length)
			{
				return;
			}
			if (growableGene.Type != growableGene.PreviousType || genes.Genes[num].Type != genes.Genes[num].PreviousType)
			{
				this.TextLinks[num - 1].enabled = false;
			}
			else
			{
				this.TextLinks[num - 1].enabled = true;
				this.TextLinks[num - 1].color = (genes.Genes[num].IsPositive() ? this.GeneUI[num - 1].PositiveColour : this.GeneUI[num - 1].NegativeColour);
			}
		}
	}

	// Token: 0x060035BF RID: 13759 RVA: 0x00142814 File Offset: 0x00140A14
	private void InitSecondRow(GrowableGenes genes)
	{
		int num = 0;
		foreach (GrowableGene growableGene in genes.Genes)
		{
			if (growableGene.Type != growableGene.PreviousType)
			{
				this.GeneUI[num].Init(growableGene);
			}
			else
			{
				this.GeneUI[num].Hide();
			}
			num++;
			if (num >= genes.Genes.Length)
			{
				return;
			}
			this.TextLinks[num - 1].enabled = false;
			GrowableGene growableGene2 = genes.Genes[num];
			this.TextDiagLinks[num - 1].enabled = false;
			if (growableGene.Type != growableGene.PreviousType && growableGene2.Type != growableGene2.PreviousType)
			{
				this.TextLinks[num - 1].enabled = true;
				this.TextLinks[num - 1].color = (growableGene2.IsPositive() ? this.GeneUI[num - 1].PositiveColour : this.GeneUI[num - 1].NegativeColour);
			}
			else if (growableGene.Type == growableGene.PreviousType && growableGene2.Type != growableGene2.PreviousType)
			{
				this.ShowDiagLink(num - 1, -43f, growableGene2);
			}
			else if (growableGene.Type != growableGene.PreviousType && growableGene2.Type == growableGene2.PreviousType)
			{
				this.ShowDiagLink(num - 1, 43f, growableGene2);
			}
		}
	}

	// Token: 0x060035C0 RID: 13760 RVA: 0x00142970 File Offset: 0x00140B70
	private void ShowDiagLink(int index, float rotation, GrowableGene nextGene)
	{
		Vector3 localEulerAngles = this.TextDiagLinks[index].transform.localEulerAngles;
		localEulerAngles.z = rotation;
		this.TextDiagLinks[index].transform.localEulerAngles = localEulerAngles;
		this.TextDiagLinks[index].enabled = true;
		this.TextDiagLinks[index].color = (nextGene.IsPositive() ? this.GeneUI[index].PositiveColour : this.GeneUI[index].NegativeColour);
	}

	// Token: 0x040030CE RID: 12494
	public UIGene[] GeneUI;

	// Token: 0x040030CF RID: 12495
	public Text[] TextLinks;

	// Token: 0x040030D0 RID: 12496
	public Text[] TextDiagLinks;
}
