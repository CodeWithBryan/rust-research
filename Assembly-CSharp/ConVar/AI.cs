using System;
using Rust.AI;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000A61 RID: 2657
	[ConsoleSystem.Factory("ai")]
	public class AI : ConsoleSystem
	{
		// Token: 0x06003F3B RID: 16187 RVA: 0x00175134 File Offset: 0x00173334
		[ServerVar]
		public static void sleepwakestats(ConsoleSystem.Arg args)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (AIInformationZone aiinformationZone in AIInformationZone.zones)
			{
				if (!(aiinformationZone == null) && aiinformationZone.ShouldSleepAI)
				{
					num++;
					if (aiinformationZone.Sleeping)
					{
						num2++;
						num3 += aiinformationZone.SleepingCount;
					}
				}
			}
			args.ReplyWith(string.Concat(new object[]
			{
				"Sleeping AIZs: ",
				num2,
				" / ",
				num,
				". Total sleeping ents: ",
				num3
			}));
		}

		// Token: 0x06003F3C RID: 16188 RVA: 0x001751F8 File Offset: 0x001733F8
		[ServerVar]
		public static void wakesleepingai(ConsoleSystem.Arg args)
		{
			int num = 0;
			int num2 = 0;
			foreach (AIInformationZone aiinformationZone in AIInformationZone.zones)
			{
				if (!(aiinformationZone == null) && aiinformationZone.ShouldSleepAI && aiinformationZone.Sleeping)
				{
					num++;
					num2 += aiinformationZone.SleepingCount;
					aiinformationZone.WakeAI();
				}
			}
			args.ReplyWith(string.Concat(new object[]
			{
				"Woke ",
				num,
				" sleeping AIZs containing ",
				num2,
				" sleeping entities."
			}));
		}

		// Token: 0x06003F3D RID: 16189 RVA: 0x001752B0 File Offset: 0x001734B0
		[ServerVar]
		public static void brainstats(ConsoleSystem.Arg args)
		{
			args.ReplyWith(string.Concat(new object[]
			{
				"Animal: ",
				AnimalBrain.Count,
				". Scientist: ",
				ScientistBrain.Count,
				". Pet: ",
				PetBrain.Count,
				". Total: ",
				AnimalBrain.Count + ScientistBrain.Count + PetBrain.Count
			}));
		}

		// Token: 0x06003F3E RID: 16190 RVA: 0x00175330 File Offset: 0x00173530
		[ServerVar]
		public static void killscientists(ConsoleSystem.Arg args)
		{
			ScientistNPC[] array = BaseEntity.Util.FindAll<ScientistNPC>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Kill(BaseNetworkable.DestroyMode.None);
			}
		}

		// Token: 0x06003F3F RID: 16191 RVA: 0x0017535C File Offset: 0x0017355C
		[ServerVar]
		public static void killanimals(ConsoleSystem.Arg args)
		{
			BaseAnimalNPC[] array = BaseEntity.Util.FindAll<BaseAnimalNPC>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Kill(BaseNetworkable.DestroyMode.None);
			}
		}

		// Token: 0x06003F40 RID: 16192 RVA: 0x00175388 File Offset: 0x00173588
		[ServerVar(Help = "Add a player (or command user if no player is specified) to the AIs ignore list.")]
		public static void addignoreplayer(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer;
			if (!args.HasArgs(1))
			{
				basePlayer = args.Player();
			}
			else
			{
				basePlayer = args.GetPlayerOrSleeper(0);
			}
			if (basePlayer == null || basePlayer.net == null || basePlayer.net.connection == null)
			{
				args.ReplyWith("Player not found.");
				return;
			}
			SimpleAIMemory.AddIgnorePlayer(basePlayer);
		}

		// Token: 0x06003F41 RID: 16193 RVA: 0x001753E4 File Offset: 0x001735E4
		[ServerVar(Help = "Remove a player (or command user if no player is specified) from the AIs ignore list.")]
		public static void removeignoreplayer(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer;
			if (!args.HasArgs(1))
			{
				basePlayer = args.Player();
			}
			else
			{
				basePlayer = args.GetPlayerOrSleeper(0);
			}
			if (basePlayer == null || basePlayer.net == null || basePlayer.net.connection == null)
			{
				args.ReplyWith("Player not found.");
				return;
			}
			SimpleAIMemory.RemoveIgnorePlayer(basePlayer);
		}

		// Token: 0x06003F42 RID: 16194 RVA: 0x0017543D File Offset: 0x0017363D
		[ServerVar(Help = "Remove all players from the AIs ignore list.")]
		public static void clearignoredplayers(ConsoleSystem.Arg args)
		{
			SimpleAIMemory.ClearIgnoredPlayers();
		}

		// Token: 0x06003F43 RID: 16195 RVA: 0x00175444 File Offset: 0x00173644
		[ServerVar(Help = "Print a lost of all the players in the AI ignore list.")]
		public static void printignoredplayers(ConsoleSystem.Arg args)
		{
			args.ReplyWith(SimpleAIMemory.GetIgnoredPlayers());
		}

		// Token: 0x06003F44 RID: 16196 RVA: 0x00175451 File Offset: 0x00173651
		public static float TickDelta()
		{
			return 1f / AI.tickrate;
		}

		// Token: 0x06003F45 RID: 16197 RVA: 0x000059DD File Offset: 0x00003BDD
		[ServerVar]
		public static void selectNPCLookatServer(ConsoleSystem.Arg args)
		{
		}

		// Token: 0x04003845 RID: 14405
		[ReplicatedVar(Saved = true)]
		public static bool allowdesigning = true;

		// Token: 0x04003846 RID: 14406
		[ServerVar]
		public static bool think = true;

		// Token: 0x04003847 RID: 14407
		[ServerVar]
		public static bool navthink = true;

		// Token: 0x04003848 RID: 14408
		[ServerVar]
		public static bool ignoreplayers = false;

		// Token: 0x04003849 RID: 14409
		[ServerVar]
		public static bool groups = true;

		// Token: 0x0400384A RID: 14410
		[ServerVar]
		public static bool spliceupdates = true;

		// Token: 0x0400384B RID: 14411
		[ServerVar]
		public static bool setdestinationsamplenavmesh = true;

		// Token: 0x0400384C RID: 14412
		[ServerVar]
		public static bool usecalculatepath = true;

		// Token: 0x0400384D RID: 14413
		[ServerVar]
		public static bool usesetdestinationfallback = true;

		// Token: 0x0400384E RID: 14414
		[ServerVar]
		public static bool npcswimming = true;

		// Token: 0x0400384F RID: 14415
		[ServerVar]
		public static bool accuratevisiondistance = true;

		// Token: 0x04003850 RID: 14416
		[ServerVar]
		public static bool move = true;

		// Token: 0x04003851 RID: 14417
		[ServerVar]
		public static bool usegrid = true;

		// Token: 0x04003852 RID: 14418
		[ServerVar]
		public static bool sleepwake = true;

		// Token: 0x04003853 RID: 14419
		[ServerVar]
		public static float sensetime = 1f;

		// Token: 0x04003854 RID: 14420
		[ServerVar]
		public static float frametime = 5f;

		// Token: 0x04003855 RID: 14421
		[ServerVar]
		public static int ocean_patrol_path_iterations = 100000;

		// Token: 0x04003856 RID: 14422
		[ServerVar(Help = "If npc_enable is set to false then npcs won't spawn. (default: true)")]
		public static bool npc_enable = true;

		// Token: 0x04003857 RID: 14423
		[ServerVar(Help = "npc_max_population_military_tunnels defines the size of the npc population at military tunnels. (default: 3)")]
		public static int npc_max_population_military_tunnels = 3;

		// Token: 0x04003858 RID: 14424
		[ServerVar(Help = "npc_spawn_per_tick_max_military_tunnels defines how many can maximum spawn at once at military tunnels. (default: 1)")]
		public static int npc_spawn_per_tick_max_military_tunnels = 1;

		// Token: 0x04003859 RID: 14425
		[ServerVar(Help = "npc_spawn_per_tick_min_military_tunnels defineshow many will minimum spawn at once at military tunnels. (default: 1)")]
		public static int npc_spawn_per_tick_min_military_tunnels = 1;

		// Token: 0x0400385A RID: 14426
		[ServerVar(Help = "npc_respawn_delay_max_military_tunnels defines the maximum delay between spawn ticks at military tunnels. (default: 1920)")]
		public static float npc_respawn_delay_max_military_tunnels = 1920f;

		// Token: 0x0400385B RID: 14427
		[ServerVar(Help = "npc_respawn_delay_min_military_tunnels defines the minimum delay between spawn ticks at military tunnels. (default: 480)")]
		public static float npc_respawn_delay_min_military_tunnels = 480f;

		// Token: 0x0400385C RID: 14428
		[ServerVar(Help = "npc_valid_aim_cone defines how close their aim needs to be on target in order to fire. (default: 0.8)")]
		public static float npc_valid_aim_cone = 0.8f;

		// Token: 0x0400385D RID: 14429
		[ServerVar(Help = "npc_valid_mounted_aim_cone defines how close their aim needs to be on target in order to fire while mounted. (default: 0.92)")]
		public static float npc_valid_mounted_aim_cone = 0.92f;

		// Token: 0x0400385E RID: 14430
		[ServerVar(Help = "npc_cover_compromised_cooldown defines how long a cover point is marked as compromised before it's cleared again for selection. (default: 10)")]
		public static float npc_cover_compromised_cooldown = 10f;

		// Token: 0x0400385F RID: 14431
		[ServerVar(Help = "If npc_cover_use_path_distance is set to true then npcs will look at the distance between the cover point and their target using the path between the two, rather than the straight-line distance.")]
		public static bool npc_cover_use_path_distance = true;

		// Token: 0x04003860 RID: 14432
		[ServerVar(Help = "npc_cover_path_vs_straight_dist_max_diff defines what the maximum difference between straight-line distance and path distance can be when evaluating cover points. (default: 2)")]
		public static float npc_cover_path_vs_straight_dist_max_diff = 2f;

		// Token: 0x04003861 RID: 14433
		[ServerVar(Help = "npc_door_trigger_size defines the size of the trigger box on doors that opens the door as npcs walk close to it (default: 1.5)")]
		public static float npc_door_trigger_size = 1.5f;

		// Token: 0x04003862 RID: 14434
		[ServerVar(Help = "npc_patrol_point_cooldown defines the cooldown time on a patrol point until it's available again (default: 5)")]
		public static float npc_patrol_point_cooldown = 5f;

		// Token: 0x04003863 RID: 14435
		[ServerVar(Help = "npc_speed_walk define the speed of an npc when in the walk state, and should be a number between 0 and 1. (Default: 0.18)")]
		public static float npc_speed_walk = 0.18f;

		// Token: 0x04003864 RID: 14436
		[ServerVar(Help = "npc_speed_walk define the speed of an npc when in the run state, and should be a number between 0 and 1. (Default: 0.4)")]
		public static float npc_speed_run = 0.4f;

		// Token: 0x04003865 RID: 14437
		[ServerVar(Help = "npc_speed_walk define the speed of an npc when in the sprint state, and should be a number between 0 and 1. (Default: 1.0)")]
		public static float npc_speed_sprint = 1f;

		// Token: 0x04003866 RID: 14438
		[ServerVar(Help = "npc_speed_walk define the speed of an npc when in the crouched walk state, and should be a number between 0 and 1. (Default: 0.1)")]
		public static float npc_speed_crouch_walk = 0.1f;

		// Token: 0x04003867 RID: 14439
		[ServerVar(Help = "npc_speed_crouch_run define the speed of an npc when in the crouched run state, and should be a number between 0 and 1. (Default: 0.25)")]
		public static float npc_speed_crouch_run = 0.25f;

		// Token: 0x04003868 RID: 14440
		[ServerVar(Help = "npc_alertness_drain_rate define the rate at which we drain the alertness level of an NPC when there are no enemies in sight. (Default: 0.01)")]
		public static float npc_alertness_drain_rate = 0.01f;

		// Token: 0x04003869 RID: 14441
		[ServerVar(Help = "npc_alertness_zero_detection_mod define the threshold of visibility required to detect an enemy when alertness is zero. (Default: 0.5)")]
		public static float npc_alertness_zero_detection_mod = 0.5f;

		// Token: 0x0400386A RID: 14442
		[ServerVar(Help = "defines the chance for scientists to spawn at NPC junkpiles. (Default: 0.1)")]
		public static float npc_junkpilespawn_chance = 0.07f;

		// Token: 0x0400386B RID: 14443
		[ServerVar(Help = "npc_junkpile_a_spawn_chance define the chance for scientists to spawn at junkpile a. (Default: 0.1)")]
		public static float npc_junkpile_a_spawn_chance = 0.1f;

		// Token: 0x0400386C RID: 14444
		[ServerVar(Help = "npc_junkpile_g_spawn_chance define the chance for scientists to spawn at junkpile g. (Default: 0.1)")]
		public static float npc_junkpile_g_spawn_chance = 0.1f;

		// Token: 0x0400386D RID: 14445
		[ServerVar(Help = "npc_junkpile_dist_aggro_gate define at what range (or closer) a junkpile scientist will get aggressive. (Default: 8)")]
		public static float npc_junkpile_dist_aggro_gate = 8f;

		// Token: 0x0400386E RID: 14446
		[ServerVar(Help = "npc_max_junkpile_count define how many npcs can spawn into the world at junkpiles at the same time (does not include monuments) (Default: 30)")]
		public static int npc_max_junkpile_count = 30;

		// Token: 0x0400386F RID: 14447
		[ServerVar(Help = "If npc_families_no_hurt is true, npcs of the same family won't be able to hurt each other. (default: true)")]
		public static bool npc_families_no_hurt = true;

		// Token: 0x04003870 RID: 14448
		[ServerVar(Help = "If npc_ignore_chairs is true, npcs won't care about seeking out and sitting in chairs. (default: true)")]
		public static bool npc_ignore_chairs = true;

		// Token: 0x04003871 RID: 14449
		[ServerVar(Help = "The rate at which we tick the sensory system. Minimum value is 1, as it multiplies with the tick-rate of the fixed AI tick rate of 0.1 (Default: 5)")]
		public static float npc_sensory_system_tick_rate_multiplier = 5f;

		// Token: 0x04003872 RID: 14450
		[ServerVar(Help = "The rate at which we gather information about available cover points. Minimum value is 1, as it multiplies with the tick-rate of the fixed AI tick rate of 0.1 (Default: 20)")]
		public static float npc_cover_info_tick_rate_multiplier = 20f;

		// Token: 0x04003873 RID: 14451
		[ServerVar(Help = "The rate at which we tick the reasoning system. Minimum value is 1, as it multiplies with the tick-rate of the fixed AI tick rate of 0.1 (Default: 1)")]
		public static float npc_reasoning_system_tick_rate_multiplier = 1f;

		// Token: 0x04003874 RID: 14452
		[ServerVar(Help = "If animal_ignore_food is true, animals will not sense food sources or interact with them (server optimization). (default: true)")]
		public static bool animal_ignore_food = true;

		// Token: 0x04003875 RID: 14453
		[ServerVar(Help = "The modifier by which a silencer reduce the noise that a gun makes when shot. (Default: 0.15)")]
		public static float npc_gun_noise_silencer_modifier = 0.15f;

		// Token: 0x04003876 RID: 14454
		[ServerVar(Help = "If nav_carve_use_building_optimization is true, we attempt to reduce the amount of navmesh carves for a building. (default: false)")]
		public static bool nav_carve_use_building_optimization = false;

		// Token: 0x04003877 RID: 14455
		[ServerVar(Help = "The minimum number of building blocks a building needs to consist of for this optimization to be applied. (default: 25)")]
		public static int nav_carve_min_building_blocks_to_apply_optimization = 25;

		// Token: 0x04003878 RID: 14456
		[ServerVar(Help = "The minimum size we allow a carving volume to be. (default: 2)")]
		public static float nav_carve_min_base_size = 2f;

		// Token: 0x04003879 RID: 14457
		[ServerVar(Help = "The size multiplier applied to the size of the carve volume. The smaller the value, the tighter the skirt around foundation edges, but too small and animals can attack through walls. (default: 4)")]
		public static float nav_carve_size_multiplier = 4f;

		// Token: 0x0400387A RID: 14458
		[ServerVar(Help = "The height of the carve volume. (default: 2)")]
		public static float nav_carve_height = 2f;

		// Token: 0x0400387B RID: 14459
		[ServerVar(Help = "If npc_only_hurt_active_target_in_safezone is true, npcs won't any player other than their actively targeted player when in a safe zone. (default: true)")]
		public static bool npc_only_hurt_active_target_in_safezone = true;

		// Token: 0x0400387C RID: 14460
		[ServerVar(Help = "If npc_use_new_aim_system is true, npcs will miss on purpose on occasion, where the old system would randomize aim cone. (default: true)")]
		public static bool npc_use_new_aim_system = true;

		// Token: 0x0400387D RID: 14461
		[ServerVar(Help = "If npc_use_thrown_weapons is true, npcs will throw grenades, etc. This is an experimental feature. (default: true)")]
		public static bool npc_use_thrown_weapons = true;

		// Token: 0x0400387E RID: 14462
		[ServerVar(Help = "This is multiplied with the max roam range stat of an NPC to determine how far from its spawn point the NPC is allowed to roam. (default: 3)")]
		public static float npc_max_roam_multiplier = 3f;

		// Token: 0x0400387F RID: 14463
		[ServerVar(Help = "This is multiplied with the current alertness (0-10) to decide how long it will take for the NPC to deliberately miss again. (default: 0.33)")]
		public static float npc_alertness_to_aim_modifier = 0.5f;

		// Token: 0x04003880 RID: 14464
		[ServerVar(Help = "The time it takes for the NPC to deliberately miss to the time the NPC tries to hit its target. (default: 1.5)")]
		public static float npc_deliberate_miss_to_hit_alignment_time = 1.5f;

		// Token: 0x04003881 RID: 14465
		[ServerVar(Help = "The offset with which the NPC will maximum miss the target. (default: 1.25)")]
		public static float npc_deliberate_miss_offset_multiplier = 1.25f;

		// Token: 0x04003882 RID: 14466
		[ServerVar(Help = "The percentage away from a maximum miss the randomizer is allowed to travel when shooting to deliberately hit the target (we don't want perfect hits with every shot). (default: 0.85f)")]
		public static float npc_deliberate_hit_randomizer = 0.85f;

		// Token: 0x04003883 RID: 14467
		[ServerVar(Help = "Baseline damage modifier for the new HTN Player NPCs to nerf their damage compared to the old NPCs. (default: 1.15f)")]
		public static float npc_htn_player_base_damage_modifier = 1.15f;

		// Token: 0x04003884 RID: 14468
		[ServerVar(Help = "Spawn NPCs on the Cargo Ship. (default: true)")]
		public static bool npc_spawn_on_cargo_ship = true;

		// Token: 0x04003885 RID: 14469
		[ServerVar(Help = "npc_htn_player_frustration_threshold defines where the frustration threshold for NPCs go, where they have the opportunity to change to a more aggressive tactic. (default: 3)")]
		public static int npc_htn_player_frustration_threshold = 3;

		// Token: 0x04003886 RID: 14470
		[ServerVar]
		public static float tickrate = 5f;
	}
}
