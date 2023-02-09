using System;

namespace ConVar
{
	// Token: 0x02000A65 RID: 2661
	[ConsoleSystem.Factory("batching")]
	public class Batching : ConsoleSystem
	{
		// Token: 0x040038F1 RID: 14577
		[ClientVar]
		public static bool renderers = true;

		// Token: 0x040038F2 RID: 14578
		[ClientVar]
		public static bool renderer_threading = true;

		// Token: 0x040038F3 RID: 14579
		[ClientVar]
		public static int renderer_capacity = 30000;

		// Token: 0x040038F4 RID: 14580
		[ClientVar]
		public static int renderer_vertices = 1000;

		// Token: 0x040038F5 RID: 14581
		[ClientVar]
		public static int renderer_submeshes = 1;

		// Token: 0x040038F6 RID: 14582
		[ServerVar]
		[ClientVar]
		public static int verbose = 0;
	}
}
