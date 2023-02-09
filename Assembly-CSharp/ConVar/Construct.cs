using System;

namespace ConVar
{
	// Token: 0x02000A6A RID: 2666
	[ConsoleSystem.Factory("construct")]
	public class Construct : ConsoleSystem
	{
		// Token: 0x04003902 RID: 14594
		[ServerVar]
		[Help("How many minutes before a placed frame gets destroyed")]
		public static float frameminutes = 30f;
	}
}
