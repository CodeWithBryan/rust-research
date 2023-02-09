using System;
using System.Text;
using ProtoBuf;

// Token: 0x020003D1 RID: 977
public static class GrowableGeneEncoding
{
	// Token: 0x0600215A RID: 8538 RVA: 0x000D6E42 File Offset: 0x000D5042
	public static void EncodeGenesToItem(global::GrowableEntity sourceGrowable, global::Item targetItem)
	{
		if (sourceGrowable == null || sourceGrowable.Genes == null)
		{
			return;
		}
		GrowableGeneEncoding.EncodeGenesToItem(GrowableGeneEncoding.EncodeGenesToInt(sourceGrowable.Genes), targetItem);
	}

	// Token: 0x0600215B RID: 8539 RVA: 0x000D6E67 File Offset: 0x000D5067
	public static void EncodeGenesToItem(int genes, global::Item targetItem)
	{
		if (targetItem == null)
		{
			return;
		}
		targetItem.instanceData = new ProtoBuf.Item.InstanceData
		{
			ShouldPool = false,
			dataInt = genes
		};
	}

	// Token: 0x0600215C RID: 8540 RVA: 0x000D6E88 File Offset: 0x000D5088
	public static int EncodeGenesToInt(GrowableGenes genes)
	{
		int num = 0;
		for (int i = 0; i < genes.Genes.Length; i++)
		{
			num = GrowableGeneEncoding.Set(num, i, (int)genes.Genes[i].Type);
		}
		return num;
	}

	// Token: 0x0600215D RID: 8541 RVA: 0x000D6EC0 File Offset: 0x000D50C0
	public static int EncodePreviousGenesToInt(GrowableGenes genes)
	{
		int num = 0;
		for (int i = 0; i < genes.Genes.Length; i++)
		{
			num = GrowableGeneEncoding.Set(num, i, (int)genes.Genes[i].PreviousType);
		}
		return num;
	}

	// Token: 0x0600215E RID: 8542 RVA: 0x000D6EF8 File Offset: 0x000D50F8
	public static void DecodeIntToGenes(int data, GrowableGenes genes)
	{
		for (int i = 0; i < 6; i++)
		{
			genes.Genes[i].Set((GrowableGenetics.GeneType)GrowableGeneEncoding.Get(data, i), false);
		}
	}

	// Token: 0x0600215F RID: 8543 RVA: 0x000D6F28 File Offset: 0x000D5128
	public static void DecodeIntToPreviousGenes(int data, GrowableGenes genes)
	{
		for (int i = 0; i < 6; i++)
		{
			genes.Genes[i].SetPrevious((GrowableGenetics.GeneType)GrowableGeneEncoding.Get(data, i));
		}
	}

	// Token: 0x06002160 RID: 8544 RVA: 0x000D6F58 File Offset: 0x000D5158
	public static string DecodeIntToGeneString(int data)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < 6; i++)
		{
			stringBuilder.Append(GrowableGene.GetColourCodedDisplayCharacter((GrowableGenetics.GeneType)GrowableGeneEncoding.Get(data, i)));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06002161 RID: 8545 RVA: 0x000D6F90 File Offset: 0x000D5190
	private static int Set(int storage, int slot, int value)
	{
		int num = slot * 5;
		int num2 = 31 << num;
		return (storage & ~num2) | value << num;
	}

	// Token: 0x06002162 RID: 8546 RVA: 0x000D6FB4 File Offset: 0x000D51B4
	private static int Get(int storage, int slot)
	{
		int num = slot * 5;
		int num2 = 31 << num;
		return (storage & num2) >> num;
	}
}
