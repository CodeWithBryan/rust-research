using System;
using System.Collections.Generic;
using System.Text;
using Facepunch;
using ProtoBuf;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000A91 RID: 2705
	[ConsoleSystem.Factory("player")]
	public class Player : ConsoleSystem
	{
		// Token: 0x0600407E RID: 16510 RVA: 0x0017BAA8 File Offset: 0x00179CA8
		[ServerUserVar]
		[ClientVar(AllowRunFromServer = true)]
		public static void cinematic_play(ConsoleSystem.Arg arg)
		{
			if (!arg.HasArgs(1))
			{
				return;
			}
			if (arg.IsServerside)
			{
				global::BasePlayer basePlayer = arg.Player();
				if (basePlayer == null)
				{
					return;
				}
				string strCommand = string.Empty;
				if (basePlayer.IsAdmin || basePlayer.IsDeveloper)
				{
					strCommand = string.Concat(new string[]
					{
						arg.cmd.FullName,
						" ",
						arg.FullString,
						" ",
						basePlayer.UserIDString
					});
				}
				else if (Server.cinematic)
				{
					strCommand = string.Concat(new string[]
					{
						arg.cmd.FullName,
						" ",
						arg.GetString(0, ""),
						" ",
						basePlayer.UserIDString
					});
				}
				if (Server.cinematic)
				{
					ConsoleNetwork.BroadcastToAllClients(strCommand, Array.Empty<object>());
					return;
				}
				if (basePlayer.IsAdmin || basePlayer.IsDeveloper)
				{
					ConsoleNetwork.SendClientCommand(arg.Connection, strCommand, Array.Empty<object>());
				}
			}
		}

		// Token: 0x0600407F RID: 16511 RVA: 0x0017BBAC File Offset: 0x00179DAC
		[ServerUserVar]
		[ClientVar(AllowRunFromServer = true)]
		public static void cinematic_stop(ConsoleSystem.Arg arg)
		{
			if (arg.IsServerside)
			{
				global::BasePlayer basePlayer = arg.Player();
				if (basePlayer == null)
				{
					return;
				}
				string strCommand = string.Empty;
				if (basePlayer.IsAdmin || basePlayer.IsDeveloper)
				{
					strCommand = string.Concat(new string[]
					{
						arg.cmd.FullName,
						" ",
						arg.FullString,
						" ",
						basePlayer.UserIDString
					});
				}
				else if (Server.cinematic)
				{
					strCommand = arg.cmd.FullName + " " + basePlayer.UserIDString;
				}
				if (Server.cinematic)
				{
					ConsoleNetwork.BroadcastToAllClients(strCommand, Array.Empty<object>());
					return;
				}
				if (basePlayer.IsAdmin || basePlayer.IsDeveloper)
				{
					ConsoleNetwork.SendClientCommand(arg.Connection, strCommand, Array.Empty<object>());
				}
			}
		}

		// Token: 0x06004080 RID: 16512 RVA: 0x0017BC80 File Offset: 0x00179E80
		[ServerUserVar]
		public static void cinematic_gesture(ConsoleSystem.Arg arg)
		{
			if (!Server.cinematic)
			{
				return;
			}
			string @string = arg.GetString(0, "");
			global::BasePlayer basePlayer = arg.GetPlayer(1);
			if (basePlayer == null)
			{
				basePlayer = arg.Player();
			}
			basePlayer.UpdateActiveItem(0U);
			basePlayer.SignalBroadcast(global::BaseEntity.Signal.Gesture, @string, null);
		}

		// Token: 0x06004081 RID: 16513 RVA: 0x0017BCCC File Offset: 0x00179ECC
		[ServerUserVar]
		public static void copyrotation(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			uint @uint = arg.GetUInt(0, 0U);
			global::BasePlayer basePlayer2 = global::BasePlayer.FindByID((ulong)@uint);
			if (basePlayer2 == null)
			{
				basePlayer2 = global::BasePlayer.FindBot((ulong)@uint);
			}
			if (basePlayer2 != null)
			{
				basePlayer2.CopyRotation(basePlayer);
				Debug.Log("Copied rotation of " + basePlayer2.UserIDString);
			}
		}

		// Token: 0x06004082 RID: 16514 RVA: 0x0017BD40 File Offset: 0x00179F40
		[ServerUserVar]
		public static void abandonmission(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (basePlayer.HasActiveMission())
			{
				basePlayer.AbandonActiveMission();
			}
		}

		// Token: 0x06004083 RID: 16515 RVA: 0x0017BD64 File Offset: 0x00179F64
		[ServerUserVar]
		public static void mount(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			uint @uint = arg.GetUInt(0, 0U);
			global::BasePlayer basePlayer2 = global::BasePlayer.FindByID((ulong)@uint);
			if (basePlayer2 == null)
			{
				basePlayer2 = global::BasePlayer.FindBot((ulong)@uint);
			}
			if (!basePlayer2)
			{
				return;
			}
			RaycastHit hit;
			if (Physics.Raycast(basePlayer.eyes.position, basePlayer.eyes.HeadForward(), out hit, 5f, 10496, QueryTriggerInteraction.Ignore))
			{
				global::BaseEntity entity = hit.GetEntity();
				if (entity)
				{
					BaseMountable baseMountable = entity.GetComponent<BaseMountable>();
					if (!baseMountable)
					{
						global::BaseVehicle baseVehicle = entity.GetComponentInParent<global::BaseVehicle>();
						if (baseVehicle)
						{
							if (!baseVehicle.isServer)
							{
								baseVehicle = (global::BaseNetworkable.serverEntities.Find(baseVehicle.net.ID) as global::BaseVehicle);
							}
							baseVehicle.AttemptMount(basePlayer2, true);
							return;
						}
					}
					if (baseMountable && !baseMountable.isServer)
					{
						baseMountable = (global::BaseNetworkable.serverEntities.Find(baseMountable.net.ID) as BaseMountable);
					}
					if (baseMountable)
					{
						baseMountable.AttemptMount(basePlayer2, true);
					}
				}
			}
		}

		// Token: 0x06004084 RID: 16516 RVA: 0x0017BE94 File Offset: 0x0017A094
		[ServerVar]
		public static void gotosleep(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			uint @uint = arg.GetUInt(0, 0U);
			global::BasePlayer basePlayer2 = global::BasePlayer.FindSleeping(@uint.ToString());
			if (!basePlayer2)
			{
				basePlayer2 = global::BasePlayer.FindBotClosestMatch(@uint.ToString());
				if (basePlayer2.IsSleeping())
				{
					basePlayer2 = null;
				}
			}
			if (!basePlayer2)
			{
				return;
			}
			basePlayer2.StartSleeping();
		}

		// Token: 0x06004085 RID: 16517 RVA: 0x0017BF04 File Offset: 0x0017A104
		[ServerVar]
		public static void dismount(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			uint @uint = arg.GetUInt(0, 0U);
			global::BasePlayer basePlayer2 = global::BasePlayer.FindByID((ulong)@uint);
			if (basePlayer2 == null)
			{
				basePlayer2 = global::BasePlayer.FindBot((ulong)@uint);
			}
			if (!basePlayer2)
			{
				return;
			}
			if (basePlayer2 && basePlayer2.isMounted)
			{
				basePlayer2.GetMounted().DismountPlayer(basePlayer2, false);
			}
		}

		// Token: 0x06004086 RID: 16518 RVA: 0x0017BF78 File Offset: 0x0017A178
		[ServerVar]
		public static void swapseat(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			uint @uint = arg.GetUInt(0, 0U);
			global::BasePlayer basePlayer2 = global::BasePlayer.FindByID((ulong)@uint);
			if (basePlayer2 == null)
			{
				basePlayer2 = global::BasePlayer.FindBot((ulong)@uint);
			}
			if (!basePlayer2)
			{
				return;
			}
			int @int = arg.GetInt(1, 0);
			if (basePlayer2 && basePlayer2.isMounted && basePlayer2.GetMounted().VehicleParent())
			{
				basePlayer2.GetMounted().VehicleParent().SwapSeats(basePlayer2, @int);
			}
		}

		// Token: 0x06004087 RID: 16519 RVA: 0x0017C00C File Offset: 0x0017A20C
		[ServerVar]
		public static void wakeup(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			global::BasePlayer basePlayer2 = global::BasePlayer.FindSleeping(arg.GetUInt(0, 0U).ToString());
			if (!basePlayer2)
			{
				return;
			}
			basePlayer2.EndSleeping();
		}

		// Token: 0x06004088 RID: 16520 RVA: 0x0017C060 File Offset: 0x0017A260
		[ServerVar]
		public static void wakeupall(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			List<global::BasePlayer> list = Pool.GetList<global::BasePlayer>();
			foreach (global::BasePlayer item in global::BasePlayer.sleepingPlayerList)
			{
				list.Add(item);
			}
			foreach (global::BasePlayer basePlayer2 in list)
			{
				basePlayer2.EndSleeping();
			}
			Pool.FreeList<global::BasePlayer>(ref list);
		}

		// Token: 0x06004089 RID: 16521 RVA: 0x0017C11C File Offset: 0x0017A31C
		[ServerVar]
		public static void printstats(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("{0:F1}s alive", basePlayer.lifeStory.secondsAlive));
			stringBuilder.AppendLine(string.Format("{0:F1}s sleeping", basePlayer.lifeStory.secondsSleeping));
			stringBuilder.AppendLine(string.Format("{0:F1}s swimming", basePlayer.lifeStory.secondsSwimming));
			stringBuilder.AppendLine(string.Format("{0:F1}s in base", basePlayer.lifeStory.secondsInBase));
			stringBuilder.AppendLine(string.Format("{0:F1}s in wilderness", basePlayer.lifeStory.secondsWilderness));
			stringBuilder.AppendLine(string.Format("{0:F1}s in monuments", basePlayer.lifeStory.secondsInMonument));
			stringBuilder.AppendLine(string.Format("{0:F1}s flying", basePlayer.lifeStory.secondsFlying));
			stringBuilder.AppendLine(string.Format("{0:F1}s boating", basePlayer.lifeStory.secondsBoating));
			stringBuilder.AppendLine(string.Format("{0:F1}s driving", basePlayer.lifeStory.secondsDriving));
			stringBuilder.AppendLine(string.Format("{0:F1}m run", basePlayer.lifeStory.metersRun));
			stringBuilder.AppendLine(string.Format("{0:F1}m walked", basePlayer.lifeStory.metersWalked));
			stringBuilder.AppendLine(string.Format("{0:F1} damage taken", basePlayer.lifeStory.totalDamageTaken));
			stringBuilder.AppendLine(string.Format("{0:F1} damage healed", basePlayer.lifeStory.totalHealing));
			stringBuilder.AppendLine("===");
			stringBuilder.AppendLine(string.Format("{0} other players killed", basePlayer.lifeStory.killedPlayers));
			stringBuilder.AppendLine(string.Format("{0} scientists killed", basePlayer.lifeStory.killedScientists));
			stringBuilder.AppendLine(string.Format("{0} animals killed", basePlayer.lifeStory.killedAnimals));
			stringBuilder.AppendLine("===");
			stringBuilder.AppendLine("Weapon stats:");
			if (basePlayer.lifeStory.weaponStats != null)
			{
				foreach (PlayerLifeStory.WeaponStats weaponStats in basePlayer.lifeStory.weaponStats)
				{
					float num = weaponStats.shotsHit / weaponStats.shotsFired;
					num *= 100f;
					stringBuilder.AppendLine(string.Format("{0} - shots fired: {1} shots hit: {2} accuracy: {3:F1}%", new object[]
					{
						weaponStats.weaponName,
						weaponStats.shotsFired,
						weaponStats.shotsHit,
						num
					}));
				}
			}
			stringBuilder.AppendLine("===");
			stringBuilder.AppendLine("Misc stats:");
			if (basePlayer.lifeStory.genericStats != null)
			{
				foreach (PlayerLifeStory.GenericStat genericStat in basePlayer.lifeStory.genericStats)
				{
					stringBuilder.AppendLine(string.Format("{0} = {1}", genericStat.key, genericStat.value));
				}
			}
			arg.ReplyWith(stringBuilder.ToString());
		}

		// Token: 0x0600408A RID: 16522 RVA: 0x0017C4C4 File Offset: 0x0017A6C4
		[ServerVar]
		public static void printpresence(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			bool flag = (basePlayer.currentTimeCategory & 1) != 0;
			bool flag2 = (basePlayer.currentTimeCategory & 4) != 0;
			bool flag3 = (basePlayer.currentTimeCategory & 2) != 0;
			bool flag4 = (basePlayer.currentTimeCategory & 32) != 0;
			bool flag5 = (basePlayer.currentTimeCategory & 16) != 0;
			bool flag6 = (basePlayer.currentTimeCategory & 8) != 0;
			arg.ReplyWith(string.Format("Wilderness:{0} Base:{1} Monument:{2} Swimming: {3} Boating: {4} Flying: {5}", new object[]
			{
				flag,
				flag2,
				flag3,
				flag4,
				flag5,
				flag6
			}));
		}

		// Token: 0x0600408B RID: 16523 RVA: 0x0017C570 File Offset: 0x0017A770
		[ServerVar(Help = "Resets the PlayerState of the given player")]
		public static void resetstate(ConsoleSystem.Arg args)
		{
			global::BasePlayer playerOrSleeper = args.GetPlayerOrSleeper(0);
			if (playerOrSleeper == null)
			{
				args.ReplyWith("Player not found");
				return;
			}
			playerOrSleeper.ResetPlayerState();
			args.ReplyWith("Player state reset");
		}

		// Token: 0x0600408C RID: 16524 RVA: 0x0017C5AC File Offset: 0x0017A7AC
		[ServerVar(ServerAdmin = true)]
		public static void fillwater(ConsoleSystem.Arg arg)
		{
			bool flag = arg.GetString(0, "").ToLower() == "salt";
			global::BasePlayer basePlayer = arg.Player();
			ItemDefinition liquidType = ItemManager.FindItemDefinition(flag ? "water.salt" : "water");
			for (int i = 0; i < PlayerBelt.MaxBeltSlots; i++)
			{
				global::Item itemInSlot = basePlayer.Belt.GetItemInSlot(i);
				BaseLiquidVessel baseLiquidVessel;
				if (itemInSlot != null && (baseLiquidVessel = (itemInSlot.GetHeldEntity() as BaseLiquidVessel)) != null && baseLiquidVessel.hasLid)
				{
					int amount = 999;
					ItemModContainer itemModContainer;
					if (baseLiquidVessel.GetItem().info.TryGetComponent<ItemModContainer>(out itemModContainer))
					{
						amount = itemModContainer.maxStackSize;
					}
					baseLiquidVessel.AddLiquid(liquidType, amount);
				}
			}
		}

		// Token: 0x0600408D RID: 16525 RVA: 0x0017C658 File Offset: 0x0017A858
		[ServerVar]
		public static void createskull(ConsoleSystem.Arg arg)
		{
			string text = arg.GetString(0, "");
			global::BasePlayer basePlayer = arg.Player();
			if (string.IsNullOrEmpty(text))
			{
				text = RandomUsernames.Get(UnityEngine.Random.Range(0, 1000));
			}
			global::Item item = ItemManager.Create(ItemManager.FindItemDefinition("skull.human"), 1, 0UL);
			item.name = HumanBodyResourceDispenser.CreateSkullName(text);
			basePlayer.inventory.GiveItem(item, null);
		}

		// Token: 0x0600408E RID: 16526 RVA: 0x0017C6C0 File Offset: 0x0017A8C0
		[ServerVar]
		public static void gesture_radius(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (basePlayer == null || !basePlayer.IsAdmin)
			{
				return;
			}
			float @float = arg.GetFloat(0, 0f);
			List<string> list = Pool.GetList<string>();
			for (int i = 0; i < 5; i++)
			{
				if (!string.IsNullOrEmpty(arg.GetString(i + 1, "")))
				{
					list.Add(arg.GetString(i + 1, ""));
				}
			}
			if (list.Count == 0)
			{
				arg.ReplyWith("No gestures provided. eg. player.gesture_radius 10f cabbagepatch raiseroof");
				return;
			}
			List<global::BasePlayer> list2 = Pool.GetList<global::BasePlayer>();
			Vis.Entities<global::BasePlayer>(basePlayer.transform.position, @float, list2, 131072, QueryTriggerInteraction.Collide);
			foreach (global::BasePlayer basePlayer2 in list2)
			{
				GestureConfig toPlay = basePlayer.gestureList.StringToGesture(list[UnityEngine.Random.Range(0, list.Count)]);
				basePlayer2.Server_StartGesture(toPlay);
			}
			Pool.FreeList<global::BasePlayer>(ref list2);
		}

		// Token: 0x0600408F RID: 16527 RVA: 0x0017C7D0 File Offset: 0x0017A9D0
		[ServerVar]
		public static void stopgesture_radius(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (basePlayer == null || !basePlayer.IsAdmin)
			{
				return;
			}
			float @float = arg.GetFloat(0, 0f);
			List<global::BasePlayer> list = Pool.GetList<global::BasePlayer>();
			Vis.Entities<global::BasePlayer>(basePlayer.transform.position, @float, list, 131072, QueryTriggerInteraction.Collide);
			foreach (global::BasePlayer basePlayer2 in list)
			{
				basePlayer2.Server_CancelGesture();
			}
			Pool.FreeList<global::BasePlayer>(ref list);
		}

		// Token: 0x06004090 RID: 16528 RVA: 0x0017C868 File Offset: 0x0017AA68
		[ServerVar]
		public static void markhostile(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.Player();
			if (basePlayer != null)
			{
				basePlayer.MarkHostileFor(60f);
			}
		}

		// Token: 0x04003980 RID: 14720
		[ServerVar]
		public static int tickrate_cl = 20;

		// Token: 0x04003981 RID: 14721
		[ServerVar]
		public static int tickrate_sv = 16;

		// Token: 0x04003982 RID: 14722
		[ServerVar(Saved = true, ShowInAdminUI = true, Help = "Whether the crawling state expires")]
		public static bool woundforever = false;
	}
}
