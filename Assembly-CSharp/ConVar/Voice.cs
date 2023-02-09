using System;

namespace ConVar
{
	// Token: 0x02000AA3 RID: 2723
	[ConsoleSystem.Factory("voice")]
	public class Voice : ConsoleSystem
	{
		// Token: 0x04003A2A RID: 14890
		[ClientVar(Saved = true)]
		public static bool loopback;
	}
}
