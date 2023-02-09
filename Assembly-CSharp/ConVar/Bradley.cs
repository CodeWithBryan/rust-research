using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000A66 RID: 2662
	[ConsoleSystem.Factory("bradley")]
	public class Bradley : ConsoleSystem
	{
		// Token: 0x06003F5C RID: 16220 RVA: 0x00175D34 File Offset: 0x00173F34
		[ServerVar]
		public static void quickrespawn(ConsoleSystem.Arg arg)
		{
			if (!arg.Player())
			{
				return;
			}
			BradleySpawner singleton = BradleySpawner.singleton;
			if (singleton == null)
			{
				Debug.LogWarning("No Spawner");
				return;
			}
			if (singleton.spawned)
			{
				singleton.spawned.Kill(BaseNetworkable.DestroyMode.None);
			}
			singleton.spawned = null;
			singleton.DoRespawn();
		}

		// Token: 0x040038F7 RID: 14583
		[ServerVar]
		public static float respawnDelayMinutes = 60f;

		// Token: 0x040038F8 RID: 14584
		[ServerVar]
		public static float respawnDelayVariance = 1f;

		// Token: 0x040038F9 RID: 14585
		[ServerVar]
		public static bool enabled = true;
	}
}
