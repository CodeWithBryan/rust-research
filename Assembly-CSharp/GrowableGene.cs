using System;

// Token: 0x020003D0 RID: 976
public class GrowableGene
{
	// Token: 0x17000293 RID: 659
	// (get) Token: 0x0600214D RID: 8525 RVA: 0x000D6D2D File Offset: 0x000D4F2D
	// (set) Token: 0x0600214E RID: 8526 RVA: 0x000D6D35 File Offset: 0x000D4F35
	public GrowableGenetics.GeneType Type { get; private set; }

	// Token: 0x17000294 RID: 660
	// (get) Token: 0x0600214F RID: 8527 RVA: 0x000D6D3E File Offset: 0x000D4F3E
	// (set) Token: 0x06002150 RID: 8528 RVA: 0x000D6D46 File Offset: 0x000D4F46
	public GrowableGenetics.GeneType PreviousType { get; private set; }

	// Token: 0x06002151 RID: 8529 RVA: 0x000D6D4F File Offset: 0x000D4F4F
	public void Set(GrowableGenetics.GeneType geneType, bool firstSet = false)
	{
		if (firstSet)
		{
			this.SetPrevious(geneType);
		}
		else
		{
			this.SetPrevious(this.Type);
		}
		this.Type = geneType;
	}

	// Token: 0x06002152 RID: 8530 RVA: 0x000D6D70 File Offset: 0x000D4F70
	public void SetPrevious(GrowableGenetics.GeneType type)
	{
		this.PreviousType = type;
	}

	// Token: 0x06002153 RID: 8531 RVA: 0x000D6D79 File Offset: 0x000D4F79
	public string GetDisplayCharacter()
	{
		return GrowableGene.GetDisplayCharacter(this.Type);
	}

	// Token: 0x06002154 RID: 8532 RVA: 0x000D6D88 File Offset: 0x000D4F88
	public static string GetDisplayCharacter(GrowableGenetics.GeneType type)
	{
		switch (type)
		{
		case GrowableGenetics.GeneType.Empty:
			return "X";
		case GrowableGenetics.GeneType.WaterRequirement:
			return "W";
		case GrowableGenetics.GeneType.GrowthSpeed:
			return "G";
		case GrowableGenetics.GeneType.Yield:
			return "Y";
		case GrowableGenetics.GeneType.Hardiness:
			return "H";
		default:
			return "U";
		}
	}

	// Token: 0x06002155 RID: 8533 RVA: 0x000D6DD4 File Offset: 0x000D4FD4
	public string GetColourCodedDisplayCharacter()
	{
		return GrowableGene.GetColourCodedDisplayCharacter(this.Type);
	}

	// Token: 0x06002156 RID: 8534 RVA: 0x000D6DE1 File Offset: 0x000D4FE1
	public static string GetColourCodedDisplayCharacter(GrowableGenetics.GeneType type)
	{
		return "<color=" + (GrowableGene.IsPositive(type) ? "#60891B>" : "#AA4734>") + GrowableGene.GetDisplayCharacter(type) + "</color>";
	}

	// Token: 0x06002157 RID: 8535 RVA: 0x000D6E0C File Offset: 0x000D500C
	public static bool IsPositive(GrowableGenetics.GeneType type)
	{
		switch (type)
		{
		case GrowableGenetics.GeneType.Empty:
			return false;
		case GrowableGenetics.GeneType.WaterRequirement:
			return false;
		case GrowableGenetics.GeneType.GrowthSpeed:
			return true;
		case GrowableGenetics.GeneType.Yield:
			return true;
		case GrowableGenetics.GeneType.Hardiness:
			return true;
		default:
			return false;
		}
	}

	// Token: 0x06002158 RID: 8536 RVA: 0x000D6E35 File Offset: 0x000D5035
	public bool IsPositive()
	{
		return GrowableGene.IsPositive(this.Type);
	}
}
