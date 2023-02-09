using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AA8 RID: 2728
	[ConsoleSystem.Factory("xmas")]
	public class XMas : ConsoleSystem
	{
		// Token: 0x0600414B RID: 16715 RVA: 0x0017F608 File Offset: 0x0017D808
		[ServerVar]
		public static void refill(ConsoleSystem.Arg arg)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/misc/xmas/xmasrefill.prefab", default(Vector3), default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.Spawn();
			}
		}

		// Token: 0x04003A35 RID: 14901
		private const string path = "assets/prefabs/misc/xmas/xmasrefill.prefab";

		// Token: 0x04003A36 RID: 14902
		[ServerVar]
		public static bool enabled = false;

		// Token: 0x04003A37 RID: 14903
		[ServerVar]
		public static float spawnRange = 40f;

		// Token: 0x04003A38 RID: 14904
		[ServerVar]
		public static int spawnAttempts = 5;

		// Token: 0x04003A39 RID: 14905
		[ServerVar]
		public static int giftsPerPlayer = 2;
	}
}
