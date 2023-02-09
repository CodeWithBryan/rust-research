using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000A98 RID: 2712
	[ConsoleSystem.Factory("spawn")]
	public class Spawn : ConsoleSystem
	{
		// Token: 0x060040D9 RID: 16601 RVA: 0x0017DFBA File Offset: 0x0017C1BA
		[ServerVar]
		public static void fill_populations(ConsoleSystem.Arg args)
		{
			if (SingletonComponent<SpawnHandler>.Instance)
			{
				SingletonComponent<SpawnHandler>.Instance.FillPopulations();
			}
		}

		// Token: 0x060040DA RID: 16602 RVA: 0x0017DFD2 File Offset: 0x0017C1D2
		[ServerVar]
		public static void fill_groups(ConsoleSystem.Arg args)
		{
			if (SingletonComponent<SpawnHandler>.Instance)
			{
				SingletonComponent<SpawnHandler>.Instance.FillGroups();
			}
		}

		// Token: 0x060040DB RID: 16603 RVA: 0x0017DFEA File Offset: 0x0017C1EA
		[ServerVar]
		public static void fill_individuals(ConsoleSystem.Arg args)
		{
			if (SingletonComponent<SpawnHandler>.Instance)
			{
				SingletonComponent<SpawnHandler>.Instance.FillIndividuals();
			}
		}

		// Token: 0x060040DC RID: 16604 RVA: 0x0017E002 File Offset: 0x0017C202
		[ServerVar]
		public static void report(ConsoleSystem.Arg args)
		{
			if (SingletonComponent<SpawnHandler>.Instance)
			{
				args.ReplyWith(SingletonComponent<SpawnHandler>.Instance.GetReport(false));
				return;
			}
			args.ReplyWith("No spawn handler found.");
		}

		// Token: 0x060040DD RID: 16605 RVA: 0x0017E030 File Offset: 0x0017C230
		[ServerVar]
		public static void scalars(ConsoleSystem.Arg args)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumn("Type");
			textTable.AddColumn("Value");
			textTable.AddRow(new string[]
			{
				"Player Fraction",
				SpawnHandler.PlayerFraction().ToString()
			});
			textTable.AddRow(new string[]
			{
				"Player Excess",
				SpawnHandler.PlayerExcess().ToString()
			});
			textTable.AddRow(new string[]
			{
				"Population Rate",
				SpawnHandler.PlayerLerp(Spawn.min_rate, Spawn.max_rate).ToString()
			});
			textTable.AddRow(new string[]
			{
				"Population Density",
				SpawnHandler.PlayerLerp(Spawn.min_density, Spawn.max_density).ToString()
			});
			textTable.AddRow(new string[]
			{
				"Group Rate",
				SpawnHandler.PlayerScale(Spawn.player_scale).ToString()
			});
			args.ReplyWith(args.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x060040DE RID: 16606 RVA: 0x0017E148 File Offset: 0x0017C348
		[ServerVar]
		public static void cargoshipevent(ConsoleSystem.Arg args)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/content/vehicles/boats/cargoship/cargoshiptest.prefab", default(Vector3), default(Quaternion), true);
			if (baseEntity != null)
			{
				baseEntity.SendMessage("TriggeredEventSpawn", SendMessageOptions.DontRequireReceiver);
				baseEntity.Spawn();
				args.ReplyWith("Cargo ship event has been started");
				return;
			}
			args.ReplyWith("Couldn't find cargo ship prefab - maybe it has been renamed?");
		}

		// Token: 0x04003A02 RID: 14850
		[ServerVar]
		public static float min_rate = 0.5f;

		// Token: 0x04003A03 RID: 14851
		[ServerVar]
		public static float max_rate = 1f;

		// Token: 0x04003A04 RID: 14852
		[ServerVar]
		public static float min_density = 0.5f;

		// Token: 0x04003A05 RID: 14853
		[ServerVar]
		public static float max_density = 1f;

		// Token: 0x04003A06 RID: 14854
		[ServerVar]
		public static float player_base = 100f;

		// Token: 0x04003A07 RID: 14855
		[ServerVar]
		public static float player_scale = 2f;

		// Token: 0x04003A08 RID: 14856
		[ServerVar]
		public static bool respawn_populations = true;

		// Token: 0x04003A09 RID: 14857
		[ServerVar]
		public static bool respawn_groups = true;

		// Token: 0x04003A0A RID: 14858
		[ServerVar]
		public static bool respawn_individuals = true;

		// Token: 0x04003A0B RID: 14859
		[ServerVar]
		public static float tick_populations = 60f;

		// Token: 0x04003A0C RID: 14860
		[ServerVar]
		public static float tick_individuals = 300f;
	}
}
