using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000A9C RID: 2716
	[ConsoleSystem.Factory("supply")]
	public class Supply : ConsoleSystem
	{
		// Token: 0x060040E7 RID: 16615 RVA: 0x0017E2C4 File Offset: 0x0017C4C4
		[ServerVar]
		public static void drop(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			Debug.Log("Supply Drop Inbound");
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/cargo plane/cargo_plane.prefab", default(Vector3), default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.GetComponent<CargoPlane>().InitDropPosition(basePlayer.transform.position + new Vector3(0f, 10f, 0f));
				baseEntity.Spawn();
			}
		}

		// Token: 0x060040E8 RID: 16616 RVA: 0x0017E34C File Offset: 0x0017C54C
		[ServerVar]
		public static void call(ConsoleSystem.Arg arg)
		{
			if (!arg.Player())
			{
				return;
			}
			Debug.Log("Supply Drop Inbound");
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/cargo plane/cargo_plane.prefab", default(Vector3), default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.Spawn();
			}
		}

		// Token: 0x04003A17 RID: 14871
		private const string path = "assets/prefabs/npc/cargo plane/cargo_plane.prefab";
	}
}
