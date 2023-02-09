using System;

namespace ConVar
{
	// Token: 0x02000AA2 RID: 2722
	[ConsoleSystem.Factory("vis")]
	public class Vis : ConsoleSystem
	{
		// Token: 0x04003A21 RID: 14881
		[ClientVar]
		[Help("Turns on debug display of lerp")]
		public static bool lerp;

		// Token: 0x04003A22 RID: 14882
		[ServerVar]
		[Help("Turns on debug display of damages")]
		public static bool damage;

		// Token: 0x04003A23 RID: 14883
		[ServerVar]
		[ClientVar]
		[Help("Turns on debug display of attacks")]
		public static bool attack;

		// Token: 0x04003A24 RID: 14884
		[ServerVar]
		[ClientVar]
		[Help("Turns on debug display of protection")]
		public static bool protection;

		// Token: 0x04003A25 RID: 14885
		[ServerVar]
		[Help("Turns on debug display of weakspots")]
		public static bool weakspots;

		// Token: 0x04003A26 RID: 14886
		[ServerVar]
		[Help("Show trigger entries")]
		public static bool triggers;

		// Token: 0x04003A27 RID: 14887
		[ServerVar]
		[Help("Turns on debug display of hitboxes")]
		public static bool hitboxes;

		// Token: 0x04003A28 RID: 14888
		[ServerVar]
		[Help("Turns on debug display of line of sight checks")]
		public static bool lineofsight;

		// Token: 0x04003A29 RID: 14889
		[ServerVar]
		[Help("Turns on debug display of senses, which are received by Ai")]
		public static bool sense;
	}
}
