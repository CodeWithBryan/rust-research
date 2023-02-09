using System;

namespace ConVar
{
	// Token: 0x02000A8C RID: 2700
	[ConsoleSystem.Factory("net")]
	public class Net : ConsoleSystem
	{
		// Token: 0x04003975 RID: 14709
		[ServerVar]
		public static bool visdebug = false;

		// Token: 0x04003976 RID: 14710
		[ClientVar]
		public static bool debug = false;

		// Token: 0x04003977 RID: 14711
		[ServerVar]
		public static int visibilityRadiusFarOverride = -1;

		// Token: 0x04003978 RID: 14712
		[ServerVar]
		public static int visibilityRadiusNearOverride = -1;
	}
}
