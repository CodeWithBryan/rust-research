using System;

namespace ConVar
{
	// Token: 0x02000A99 RID: 2713
	[ConsoleSystem.Factory("SSS")]
	public class SSS : ConsoleSystem
	{
		// Token: 0x04003A0D RID: 14861
		[ClientVar(Saved = true)]
		public static bool enabled = true;

		// Token: 0x04003A0E RID: 14862
		[ClientVar(Saved = true)]
		public static int quality = 0;

		// Token: 0x04003A0F RID: 14863
		[ClientVar(Saved = true)]
		public static bool halfres = true;

		// Token: 0x04003A10 RID: 14864
		[ClientVar(Saved = true)]
		public static float scale = 1f;
	}
}
