using System;

namespace ConVar
{
	// Token: 0x02000A9D RID: 2717
	[ConsoleSystem.Factory("terrain")]
	public class Terrain : ConsoleSystem
	{
		// Token: 0x04003A18 RID: 14872
		[ClientVar(Saved = true)]
		public static float quality = 100f;
	}
}
