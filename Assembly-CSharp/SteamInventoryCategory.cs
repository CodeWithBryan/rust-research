using System;
using UnityEngine;

// Token: 0x02000735 RID: 1845
[CreateAssetMenu(menuName = "Rust/Generic Steam Inventory Category")]
public class SteamInventoryCategory : ScriptableObject
{
	// Token: 0x0400296E RID: 10606
	[Header("Steam Inventory")]
	public bool canBeSoldToOtherUsers;

	// Token: 0x0400296F RID: 10607
	public bool canBeTradedWithOtherUsers;

	// Token: 0x04002970 RID: 10608
	public bool isCommodity;

	// Token: 0x04002971 RID: 10609
	public SteamInventoryCategory.Price price;

	// Token: 0x04002972 RID: 10610
	public SteamInventoryCategory.DropChance dropChance;

	// Token: 0x04002973 RID: 10611
	public bool CanBeInCrates = true;

	// Token: 0x02000E12 RID: 3602
	public enum Price
	{
		// Token: 0x040048EA RID: 18666
		CannotBuy,
		// Token: 0x040048EB RID: 18667
		VLV25,
		// Token: 0x040048EC RID: 18668
		VLV50,
		// Token: 0x040048ED RID: 18669
		VLV75,
		// Token: 0x040048EE RID: 18670
		VLV100,
		// Token: 0x040048EF RID: 18671
		VLV150,
		// Token: 0x040048F0 RID: 18672
		VLV200,
		// Token: 0x040048F1 RID: 18673
		VLV250,
		// Token: 0x040048F2 RID: 18674
		VLV300,
		// Token: 0x040048F3 RID: 18675
		VLV350,
		// Token: 0x040048F4 RID: 18676
		VLV400,
		// Token: 0x040048F5 RID: 18677
		VLV450,
		// Token: 0x040048F6 RID: 18678
		VLV500,
		// Token: 0x040048F7 RID: 18679
		VLV550,
		// Token: 0x040048F8 RID: 18680
		VLV600,
		// Token: 0x040048F9 RID: 18681
		VLV650,
		// Token: 0x040048FA RID: 18682
		VLV700,
		// Token: 0x040048FB RID: 18683
		VLV750,
		// Token: 0x040048FC RID: 18684
		VLV800,
		// Token: 0x040048FD RID: 18685
		VLV850,
		// Token: 0x040048FE RID: 18686
		VLV900,
		// Token: 0x040048FF RID: 18687
		VLV950,
		// Token: 0x04004900 RID: 18688
		VLV1000,
		// Token: 0x04004901 RID: 18689
		VLV1100,
		// Token: 0x04004902 RID: 18690
		VLV1200,
		// Token: 0x04004903 RID: 18691
		VLV1300,
		// Token: 0x04004904 RID: 18692
		VLV1400,
		// Token: 0x04004905 RID: 18693
		VLV1500,
		// Token: 0x04004906 RID: 18694
		VLV1600,
		// Token: 0x04004907 RID: 18695
		VLV1700,
		// Token: 0x04004908 RID: 18696
		VLV1800,
		// Token: 0x04004909 RID: 18697
		VLV1900,
		// Token: 0x0400490A RID: 18698
		VLV2000,
		// Token: 0x0400490B RID: 18699
		VLV2500,
		// Token: 0x0400490C RID: 18700
		VLV3000,
		// Token: 0x0400490D RID: 18701
		VLV3500,
		// Token: 0x0400490E RID: 18702
		VLV4000,
		// Token: 0x0400490F RID: 18703
		VLV4500,
		// Token: 0x04004910 RID: 18704
		VLV5000,
		// Token: 0x04004911 RID: 18705
		VLV6000,
		// Token: 0x04004912 RID: 18706
		VLV7000,
		// Token: 0x04004913 RID: 18707
		VLV8000,
		// Token: 0x04004914 RID: 18708
		VLV9000,
		// Token: 0x04004915 RID: 18709
		VLV10000
	}

	// Token: 0x02000E13 RID: 3603
	public enum DropChance
	{
		// Token: 0x04004917 RID: 18711
		NeverDrop,
		// Token: 0x04004918 RID: 18712
		VeryRare,
		// Token: 0x04004919 RID: 18713
		Rare,
		// Token: 0x0400491A RID: 18714
		Common,
		// Token: 0x0400491B RID: 18715
		VeryCommon,
		// Token: 0x0400491C RID: 18716
		ExtremelyRare
	}
}
