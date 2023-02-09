using System;
using System.Linq;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000A9A RID: 2714
	[ConsoleSystem.Factory("stability")]
	public class Stability : ConsoleSystem
	{
		// Token: 0x060040E3 RID: 16611 RVA: 0x0017E23C File Offset: 0x0017C43C
		[ServerVar]
		public static void refresh_stability(ConsoleSystem.Arg args)
		{
			StabilityEntity[] array = BaseNetworkable.serverEntities.OfType<StabilityEntity>().ToArray<StabilityEntity>();
			Debug.Log("Refreshing stability on " + array.Length + " entities...");
			for (int i = 0; i < array.Length; i++)
			{
				array[i].UpdateStability();
			}
		}

		// Token: 0x04003A11 RID: 14865
		[ServerVar]
		public static int verbose = 0;

		// Token: 0x04003A12 RID: 14866
		[ServerVar]
		public static int strikes = 10;

		// Token: 0x04003A13 RID: 14867
		[ServerVar]
		public static float collapse = 0.05f;

		// Token: 0x04003A14 RID: 14868
		[ServerVar]
		public static float accuracy = 0.001f;

		// Token: 0x04003A15 RID: 14869
		[ServerVar]
		public static float stabilityqueue = 9f;

		// Token: 0x04003A16 RID: 14870
		[ServerVar]
		public static float surroundingsqueue = 3f;
	}
}
