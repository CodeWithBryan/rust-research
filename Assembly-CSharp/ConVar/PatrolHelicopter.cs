using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000A80 RID: 2688
	[ConsoleSystem.Factory("heli")]
	public class PatrolHelicopter : ConsoleSystem
	{
		// Token: 0x06004031 RID: 16433 RVA: 0x00179D24 File Offset: 0x00177F24
		[ServerVar]
		public static void drop(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			Debug.Log("heli called to : " + basePlayer.transform.position);
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab", default(Vector3), default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.GetComponent<PatrolHelicopterAI>().SetInitialDestination(basePlayer.transform.position + new Vector3(0f, 10f, 0f), 0f);
				baseEntity.Spawn();
			}
		}

		// Token: 0x06004032 RID: 16434 RVA: 0x00179DC8 File Offset: 0x00177FC8
		[ServerVar]
		public static void calltome(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			Debug.Log("heli called to : " + basePlayer.transform.position);
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab", default(Vector3), default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.GetComponent<PatrolHelicopterAI>().SetInitialDestination(basePlayer.transform.position + new Vector3(0f, 10f, 0f), 0.25f);
				baseEntity.Spawn();
			}
		}

		// Token: 0x06004033 RID: 16435 RVA: 0x00179E6C File Offset: 0x0017806C
		[ServerVar]
		public static void call(ConsoleSystem.Arg arg)
		{
			if (!arg.Player())
			{
				return;
			}
			Debug.Log("Helicopter inbound");
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab", default(Vector3), default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.Spawn();
			}
		}

		// Token: 0x06004034 RID: 16436 RVA: 0x00179EC4 File Offset: 0x001780C4
		[ServerVar]
		public static void strafe(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			PatrolHelicopterAI heliInstance = PatrolHelicopterAI.heliInstance;
			if (heliInstance == null)
			{
				Debug.Log("no heli instance");
				return;
			}
			RaycastHit raycastHit;
			if (Physics.Raycast(basePlayer.eyes.HeadRay(), out raycastHit, 1000f, 1218652417))
			{
				Debug.Log("strafing :" + raycastHit.point);
				heliInstance.interestZoneOrigin = raycastHit.point;
				heliInstance.ExitCurrentState();
				heliInstance.State_Strafe_Enter(raycastHit.point, false);
				return;
			}
			Debug.Log("strafe ray missed");
		}

		// Token: 0x06004035 RID: 16437 RVA: 0x00179F60 File Offset: 0x00178160
		[ServerVar]
		public static void testpuzzle(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			bool isDeveloper = basePlayer.IsDeveloper;
		}

		// Token: 0x04003969 RID: 14697
		private const string path = "assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab";

		// Token: 0x0400396A RID: 14698
		[ServerVar]
		public static float lifetimeMinutes = 15f;

		// Token: 0x0400396B RID: 14699
		[ServerVar]
		public static int guns = 1;

		// Token: 0x0400396C RID: 14700
		[ServerVar]
		public static float bulletDamageScale = 1f;

		// Token: 0x0400396D RID: 14701
		[ServerVar]
		public static float bulletAccuracy = 2f;
	}
}
