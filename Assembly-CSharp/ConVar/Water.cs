using System;

namespace ConVar
{
	// Token: 0x02000AA4 RID: 2724
	[ConsoleSystem.Factory("water")]
	public class Water : ConsoleSystem
	{
		// Token: 0x04003A2B RID: 14891
		[ClientVar(Saved = true)]
		public static int quality = 1;

		// Token: 0x04003A2C RID: 14892
		public static int MaxQuality = 2;

		// Token: 0x04003A2D RID: 14893
		public static int MinQuality = 0;

		// Token: 0x04003A2E RID: 14894
		[ClientVar(Saved = true)]
		public static int reflections = 1;

		// Token: 0x04003A2F RID: 14895
		public static int MaxReflections = 2;

		// Token: 0x04003A30 RID: 14896
		public static int MinReflections = 0;
	}
}
