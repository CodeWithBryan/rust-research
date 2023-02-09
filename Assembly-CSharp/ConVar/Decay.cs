using System;

namespace ConVar
{
	// Token: 0x02000A70 RID: 2672
	[ConsoleSystem.Factory("decay")]
	public class Decay : ConsoleSystem
	{
		// Token: 0x04003909 RID: 14601
		[ServerVar(Help = "Maximum distance to test to see if a structure is outside, higher values are slower but accurate for huge buildings")]
		public static float outside_test_range = 50f;

		// Token: 0x0400390A RID: 14602
		[ServerVar]
		public static float tick = 600f;

		// Token: 0x0400390B RID: 14603
		[ServerVar]
		public static float scale = 1f;

		// Token: 0x0400390C RID: 14604
		[ServerVar]
		public static bool debug = false;

		// Token: 0x0400390D RID: 14605
		[ServerVar(Help = "Is upkeep enabled")]
		public static bool upkeep = true;

		// Token: 0x0400390E RID: 14606
		[ServerVar(Help = "How many minutes does the upkeep cost last? default : 1440 (24 hours)")]
		public static float upkeep_period_minutes = 1440f;

		// Token: 0x0400390F RID: 14607
		[ServerVar(Help = "How many minutes can the upkeep cost last after the cupboard was destroyed? default : 1440 (24 hours)")]
		public static float upkeep_grief_protection = 1440f;

		// Token: 0x04003910 RID: 14608
		[ServerVar(Help = "Scale at which objects heal when upkeep conditions are met, default of 1 is same rate at which they decay")]
		public static float upkeep_heal_scale = 1f;

		// Token: 0x04003911 RID: 14609
		[ServerVar(Help = "Scale at which objects decay when they are inside, default of 0.1")]
		public static float upkeep_inside_decay_scale = 0.1f;

		// Token: 0x04003912 RID: 14610
		[ServerVar(Help = "When set to a value above 0 everything will decay with this delay")]
		public static float delay_override = 0f;

		// Token: 0x04003913 RID: 14611
		[ServerVar(Help = "How long should this building grade decay be delayed when not protected by upkeep, in hours")]
		public static float delay_twig = 0f;

		// Token: 0x04003914 RID: 14612
		[ServerVar(Help = "How long should this building grade decay be delayed when not protected by upkeep, in hours")]
		public static float delay_wood = 0f;

		// Token: 0x04003915 RID: 14613
		[ServerVar(Help = "How long should this building grade decay be delayed when not protected by upkeep, in hours")]
		public static float delay_stone = 0f;

		// Token: 0x04003916 RID: 14614
		[ServerVar(Help = "How long should this building grade decay be delayed when not protected by upkeep, in hours")]
		public static float delay_metal = 0f;

		// Token: 0x04003917 RID: 14615
		[ServerVar(Help = "How long should this building grade decay be delayed when not protected by upkeep, in hours")]
		public static float delay_toptier = 0f;

		// Token: 0x04003918 RID: 14616
		[ServerVar(Help = "When set to a value above 0 everything will decay with this duration")]
		public static float duration_override = 0f;

		// Token: 0x04003919 RID: 14617
		[ServerVar(Help = "How long should this building grade take to decay when not protected by upkeep, in hours")]
		public static float duration_twig = 1f;

		// Token: 0x0400391A RID: 14618
		[ServerVar(Help = "How long should this building grade take to decay when not protected by upkeep, in hours")]
		public static float duration_wood = 3f;

		// Token: 0x0400391B RID: 14619
		[ServerVar(Help = "How long should this building grade take to decay when not protected by upkeep, in hours")]
		public static float duration_stone = 5f;

		// Token: 0x0400391C RID: 14620
		[ServerVar(Help = "How long should this building grade take to decay when not protected by upkeep, in hours")]
		public static float duration_metal = 8f;

		// Token: 0x0400391D RID: 14621
		[ServerVar(Help = "How long should this building grade take to decay when not protected by upkeep, in hours")]
		public static float duration_toptier = 12f;

		// Token: 0x0400391E RID: 14622
		[ServerVar(Help = "Between 0 and this value are considered bracket 0 and will cost bracket_0_costfraction per upkeep period to maintain")]
		public static int bracket_0_blockcount = 15;

		// Token: 0x0400391F RID: 14623
		[ServerVar(Help = "blocks within bracket 0 will cost this fraction per upkeep period to maintain")]
		public static float bracket_0_costfraction = 0.1f;

		// Token: 0x04003920 RID: 14624
		[ServerVar(Help = "Between bracket_0_blockcount and this value are considered bracket 1 and will cost bracket_1_costfraction per upkeep period to maintain")]
		public static int bracket_1_blockcount = 50;

		// Token: 0x04003921 RID: 14625
		[ServerVar(Help = "blocks within bracket 1 will cost this fraction per upkeep period to maintain")]
		public static float bracket_1_costfraction = 0.15f;

		// Token: 0x04003922 RID: 14626
		[ServerVar(Help = "Between bracket_1_blockcount and this value are considered bracket 2 and will cost bracket_2_costfraction per upkeep period to maintain")]
		public static int bracket_2_blockcount = 125;

		// Token: 0x04003923 RID: 14627
		[ServerVar(Help = "blocks within bracket 2 will cost this fraction per upkeep period to maintain")]
		public static float bracket_2_costfraction = 0.2f;

		// Token: 0x04003924 RID: 14628
		[ServerVar(Help = "Between bracket_2_blockcount and this value (and beyond) are considered bracket 3 and will cost bracket_3_costfraction per upkeep period to maintain")]
		public static int bracket_3_blockcount = 200;

		// Token: 0x04003925 RID: 14629
		[ServerVar(Help = "blocks within bracket 3 will cost this fraction per upkeep period to maintain")]
		public static float bracket_3_costfraction = 0.333f;
	}
}
