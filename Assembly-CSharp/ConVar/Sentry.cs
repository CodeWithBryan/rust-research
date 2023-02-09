using System;

namespace ConVar
{
	// Token: 0x02000A96 RID: 2710
	[ConsoleSystem.Factory("sentry")]
	public class Sentry : ConsoleSystem
	{
		// Token: 0x04003987 RID: 14727
		[ServerVar(Help = "target everyone regardless of authorization")]
		public static bool targetall = false;

		// Token: 0x04003988 RID: 14728
		[ServerVar(Help = "how long until something is considered hostile after it attacked")]
		public static float hostileduration = 120f;
	}
}
