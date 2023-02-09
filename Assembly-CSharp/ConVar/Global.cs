using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Facepunch;
using Facepunch.Extend;
using Network;
using Network.Visibility;
using Rust;
using UnityEngine;
using UnityEngine.Profiling;

namespace ConVar
{
	// Token: 0x02000A7B RID: 2683
	[ConsoleSystem.Factory("global")]
	public class Global : ConsoleSystem
	{
		// Token: 0x17000546 RID: 1350
		// (get) Token: 0x06003FDB RID: 16347 RVA: 0x00178768 File Offset: 0x00176968
		// (set) Token: 0x06003FDA RID: 16346 RVA: 0x00178760 File Offset: 0x00176960
		[ServerVar]
		[ClientVar]
		public static int developer
		{
			get
			{
				return Global._developer;
			}
			set
			{
				Global._developer = value;
			}
		}

		// Token: 0x06003FDC RID: 16348 RVA: 0x00178770 File Offset: 0x00176970
		public static void ApplyAsyncLoadingPreset()
		{
			if (Global.asyncLoadingPreset != 0)
			{
				UnityEngine.Debug.Log(string.Format("Applying async loading preset number {0}", Global.asyncLoadingPreset));
			}
			switch (Global.asyncLoadingPreset)
			{
			case 0:
				break;
			case 1:
				if (Global.warmupConcurrency <= 1)
				{
					Global.warmupConcurrency = 256;
				}
				if (Global.preloadConcurrency <= 1)
				{
					Global.preloadConcurrency = 256;
				}
				Global.asyncWarmup = false;
				return;
			case 2:
				if (Global.warmupConcurrency <= 1)
				{
					Global.warmupConcurrency = 256;
				}
				if (Global.preloadConcurrency <= 1)
				{
					Global.preloadConcurrency = 256;
				}
				Global.asyncWarmup = false;
				return;
			default:
				UnityEngine.Debug.LogWarning(string.Format("There is no asyncLoading preset number {0}", Global.asyncLoadingPreset));
				break;
			}
		}

		// Token: 0x06003FDD RID: 16349 RVA: 0x00178826 File Offset: 0x00176A26
		[ServerVar]
		public static void restart(ConsoleSystem.Arg args)
		{
			ServerMgr.RestartServer(args.GetString(1, string.Empty), args.GetInt(0, 300));
		}

		// Token: 0x06003FDE RID: 16350 RVA: 0x00178845 File Offset: 0x00176A45
		[ClientVar]
		[ServerVar]
		public static void quit(ConsoleSystem.Arg args)
		{
			SingletonComponent<ServerMgr>.Instance.Shutdown();
			Rust.Application.isQuitting = true;
			Net.sv.Stop("quit");
			Process.GetCurrentProcess().Kill();
			UnityEngine.Debug.Log("Quitting");
			Rust.Application.Quit();
		}

		// Token: 0x06003FDF RID: 16351 RVA: 0x0017887F File Offset: 0x00176A7F
		[ServerVar]
		public static void report(ConsoleSystem.Arg args)
		{
			ServerPerformance.DoReport();
		}

		// Token: 0x06003FE0 RID: 16352 RVA: 0x00178888 File Offset: 0x00176A88
		[ServerVar]
		[ClientVar]
		public static void objects(ConsoleSystem.Arg args)
		{
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType<UnityEngine.Object>();
			string text = "";
			Dictionary<Type, int> dictionary = new Dictionary<Type, int>();
			Dictionary<Type, long> dictionary2 = new Dictionary<Type, long>();
			foreach (UnityEngine.Object @object in array)
			{
				int runtimeMemorySize = Profiler.GetRuntimeMemorySize(@object);
				if (dictionary.ContainsKey(@object.GetType()))
				{
					Dictionary<Type, int> dictionary3 = dictionary;
					Type type = @object.GetType();
					int num = dictionary3[type];
					dictionary3[type] = num + 1;
				}
				else
				{
					dictionary.Add(@object.GetType(), 1);
				}
				if (dictionary2.ContainsKey(@object.GetType()))
				{
					Dictionary<Type, long> dictionary4 = dictionary2;
					Type type = @object.GetType();
					dictionary4[type] += (long)runtimeMemorySize;
				}
				else
				{
					dictionary2.Add(@object.GetType(), (long)runtimeMemorySize);
				}
			}
			foreach (KeyValuePair<Type, long> keyValuePair in dictionary2.OrderByDescending(delegate(KeyValuePair<Type, long> x)
			{
				KeyValuePair<Type, long> keyValuePair2 = x;
				return keyValuePair2.Value;
			}))
			{
				text = string.Concat(new object[]
				{
					text,
					dictionary[keyValuePair.Key].ToString().PadLeft(10),
					" ",
					keyValuePair.Value.FormatBytes(false).PadLeft(15),
					"\t",
					keyValuePair.Key,
					"\n"
				});
			}
			args.ReplyWith(text);
		}

		// Token: 0x06003FE1 RID: 16353 RVA: 0x00178A24 File Offset: 0x00176C24
		[ServerVar]
		[ClientVar]
		public static void textures(ConsoleSystem.Arg args)
		{
			Texture[] array = UnityEngine.Object.FindObjectsOfType<Texture>();
			string text = "";
			foreach (Texture texture in array)
			{
				string text2 = Profiler.GetRuntimeMemorySize(texture).FormatBytes(false);
				text = string.Concat(new string[]
				{
					text,
					texture.ToString().PadRight(30),
					texture.name.PadRight(30),
					text2,
					"\n"
				});
			}
			args.ReplyWith(text);
		}

		// Token: 0x06003FE2 RID: 16354 RVA: 0x00178AA4 File Offset: 0x00176CA4
		[ServerVar]
		[ClientVar]
		public static void colliders(ConsoleSystem.Arg args)
		{
			int num = (from x in UnityEngine.Object.FindObjectsOfType<Collider>()
			where x.enabled
			select x).Count<Collider>();
			int num2 = (from x in UnityEngine.Object.FindObjectsOfType<Collider>()
			where !x.enabled
			select x).Count<Collider>();
			string strValue = string.Concat(new object[]
			{
				num,
				" colliders enabled, ",
				num2,
				" disabled"
			});
			args.ReplyWith(strValue);
		}

		// Token: 0x06003FE3 RID: 16355 RVA: 0x00178B44 File Offset: 0x00176D44
		[ServerVar]
		[ClientVar]
		public static void error(ConsoleSystem.Arg args)
		{
			((GameObject)null).transform.position = Vector3.zero;
		}

		// Token: 0x06003FE4 RID: 16356 RVA: 0x00178B58 File Offset: 0x00176D58
		[ServerVar]
		[ClientVar]
		public static void queue(ConsoleSystem.Arg args)
		{
			string text = "";
			text = text + "stabilityCheckQueue:\t\t" + StabilityEntity.stabilityCheckQueue.Info() + "\n";
			text = text + "updateSurroundingsQueue:\t" + StabilityEntity.updateSurroundingsQueue.Info() + "\n";
			args.ReplyWith(text);
		}

		// Token: 0x06003FE5 RID: 16357 RVA: 0x00178BA8 File Offset: 0x00176DA8
		[ServerUserVar]
		public static void setinfo(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			string @string = args.GetString(0, null);
			string string2 = args.GetString(1, null);
			if (@string == null || string2 == null)
			{
				return;
			}
			basePlayer.SetInfo(@string, string2);
		}

		// Token: 0x06003FE6 RID: 16358 RVA: 0x00178BE8 File Offset: 0x00176DE8
		[ServerVar]
		public static void sleep(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.IsSleeping())
			{
				return;
			}
			if (basePlayer.IsSpectating())
			{
				return;
			}
			if (basePlayer.IsDead())
			{
				return;
			}
			basePlayer.StartSleeping();
		}

		// Token: 0x06003FE7 RID: 16359 RVA: 0x00178C28 File Offset: 0x00176E28
		[ServerUserVar]
		public static void kill(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.IsSpectating())
			{
				return;
			}
			if (basePlayer.IsDead())
			{
				return;
			}
			if (basePlayer.CanSuicide())
			{
				basePlayer.MarkSuicide();
				basePlayer.Hurt(1000f, DamageType.Suicide, basePlayer, false);
				return;
			}
			basePlayer.ConsoleMessage("You can't suicide again so quickly, wait a while");
		}

		// Token: 0x06003FE8 RID: 16360 RVA: 0x00178C80 File Offset: 0x00176E80
		[ServerUserVar]
		public static void respawn(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsDead() && !basePlayer.IsSpectating())
			{
				if (Global.developer > 0)
				{
					UnityEngine.Debug.LogWarning(basePlayer + " wanted to respawn but isn't dead or spectating");
				}
				basePlayer.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
				return;
			}
			if (basePlayer.CanRespawn())
			{
				basePlayer.MarkRespawn(5f);
				basePlayer.Respawn();
				return;
			}
			basePlayer.ConsoleMessage("You can't respawn again so quickly, wait a while");
		}

		// Token: 0x06003FE9 RID: 16361 RVA: 0x00178CF2 File Offset: 0x00176EF2
		[ServerVar]
		public static void injure(ConsoleSystem.Arg args)
		{
			Global.InjurePlayer(args.Player());
		}

		// Token: 0x06003FEA RID: 16362 RVA: 0x00178D00 File Offset: 0x00176F00
		public static void InjurePlayer(BasePlayer ply)
		{
			if (ply == null)
			{
				return;
			}
			if (ply.IsDead())
			{
				return;
			}
			if (!ConVar.Server.woundingenabled || ply.IsIncapacitated() || ply.IsSleeping() || ply.isMounted)
			{
				ply.ConsoleMessage("Can't go to wounded state right now.");
				return;
			}
			if (ply.IsCrawling())
			{
				ply.GoToIncapacitated(null);
				return;
			}
			ply.BecomeWounded(null);
		}

		// Token: 0x06003FEB RID: 16363 RVA: 0x00178D68 File Offset: 0x00176F68
		[ServerVar]
		public static void recover(ConsoleSystem.Arg args)
		{
			Global.RecoverPlayer(args.Player());
		}

		// Token: 0x06003FEC RID: 16364 RVA: 0x00178D75 File Offset: 0x00176F75
		public static void RecoverPlayer(BasePlayer ply)
		{
			if (ply == null)
			{
				return;
			}
			if (ply.IsDead())
			{
				return;
			}
			ply.StopWounded(null);
		}

		// Token: 0x06003FED RID: 16365 RVA: 0x00178D94 File Offset: 0x00176F94
		[ServerVar]
		public static void spectate(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsDead())
			{
				basePlayer.DieInstantly();
			}
			string @string = args.GetString(0, "");
			if (basePlayer.IsDead())
			{
				basePlayer.StartSpectating();
				basePlayer.UpdateSpectateTarget(@string);
			}
		}

		// Token: 0x06003FEE RID: 16366 RVA: 0x00178DE4 File Offset: 0x00176FE4
		[ServerVar]
		public static void spectateid(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsDead())
			{
				basePlayer.DieInstantly();
			}
			uint @uint = args.GetUInt(0, 0U);
			if (basePlayer.IsDead())
			{
				basePlayer.StartSpectating();
				basePlayer.UpdateSpectateTarget((ulong)@uint);
			}
		}

		// Token: 0x06003FEF RID: 16367 RVA: 0x00178E30 File Offset: 0x00177030
		[ServerUserVar]
		public static void respawn_sleepingbag(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsDead())
			{
				return;
			}
			uint @uint = args.GetUInt(0, 0U);
			if (@uint == 0U)
			{
				args.ReplyWith("Missing sleeping bag ID");
				return;
			}
			if (!basePlayer.CanRespawn())
			{
				basePlayer.ConsoleMessage("You can't respawn again so quickly, wait a while");
				return;
			}
			if (SleepingBag.SpawnPlayer(basePlayer, @uint))
			{
				basePlayer.MarkRespawn(5f);
				return;
			}
			args.ReplyWith("Couldn't spawn in sleeping bag!");
		}

		// Token: 0x06003FF0 RID: 16368 RVA: 0x00178EA4 File Offset: 0x001770A4
		[ServerUserVar]
		public static void respawn_sleepingbag_remove(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			uint @uint = args.GetUInt(0, 0U);
			if (@uint == 0U)
			{
				args.ReplyWith("Missing sleeping bag ID");
				return;
			}
			SleepingBag.DestroyBag(basePlayer, @uint);
		}

		// Token: 0x06003FF1 RID: 16369 RVA: 0x00178EE4 File Offset: 0x001770E4
		[ServerUserVar]
		public static void status_sv(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			args.ReplyWith(basePlayer.GetDebugStatus());
		}

		// Token: 0x06003FF2 RID: 16370 RVA: 0x000059DD File Offset: 0x00003BDD
		[ClientVar]
		public static void status_cl(ConsoleSystem.Arg args)
		{
		}

		// Token: 0x06003FF3 RID: 16371 RVA: 0x00178F10 File Offset: 0x00177110
		[ServerVar]
		public static void teleport(ConsoleSystem.Arg args)
		{
			if (args.HasArgs(2))
			{
				BasePlayer playerOrSleeperOrBot = args.GetPlayerOrSleeperOrBot(0);
				if (!playerOrSleeperOrBot)
				{
					return;
				}
				if (!playerOrSleeperOrBot.IsAlive())
				{
					return;
				}
				BasePlayer playerOrSleeperOrBot2 = args.GetPlayerOrSleeperOrBot(1);
				if (!playerOrSleeperOrBot2)
				{
					return;
				}
				if (!playerOrSleeperOrBot2.IsAlive())
				{
					return;
				}
				playerOrSleeperOrBot.Teleport(playerOrSleeperOrBot2);
				return;
			}
			else
			{
				BasePlayer basePlayer = args.Player();
				if (!basePlayer)
				{
					return;
				}
				if (!basePlayer.IsAlive())
				{
					return;
				}
				BasePlayer playerOrSleeperOrBot3 = args.GetPlayerOrSleeperOrBot(0);
				if (!playerOrSleeperOrBot3)
				{
					return;
				}
				if (!playerOrSleeperOrBot3.IsAlive())
				{
					return;
				}
				basePlayer.Teleport(playerOrSleeperOrBot3);
				return;
			}
		}

		// Token: 0x06003FF4 RID: 16372 RVA: 0x00178F9C File Offset: 0x0017719C
		[ServerVar]
		public static void teleport2me(ConsoleSystem.Arg args)
		{
			BasePlayer playerOrSleeperOrBot = args.GetPlayerOrSleeperOrBot(0);
			if (playerOrSleeperOrBot == null)
			{
				args.ReplyWith("Player or bot not found");
				return;
			}
			if (!playerOrSleeperOrBot.IsAlive())
			{
				args.ReplyWith("Target is not alive");
				return;
			}
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsAlive())
			{
				return;
			}
			playerOrSleeperOrBot.Teleport(basePlayer);
		}

		// Token: 0x06003FF5 RID: 16373 RVA: 0x00178FFC File Offset: 0x001771FC
		[ServerVar]
		public static void teleportany(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsAlive())
			{
				return;
			}
			basePlayer.Teleport(args.GetString(0, ""), false);
		}

		// Token: 0x06003FF6 RID: 16374 RVA: 0x00179038 File Offset: 0x00177238
		[ServerVar]
		public static void teleportpos(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsAlive())
			{
				return;
			}
			basePlayer.Teleport(args.GetVector3(0, Vector3.zero));
		}

		// Token: 0x06003FF7 RID: 16375 RVA: 0x00179070 File Offset: 0x00177270
		[ServerVar]
		public static void teleportlos(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			if (!basePlayer.IsAlive())
			{
				return;
			}
			Ray ray = basePlayer.eyes.HeadRay();
			int @int = args.GetInt(0, 1000);
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, (float)@int, 1218652417))
			{
				basePlayer.Teleport(raycastHit.point);
				return;
			}
			basePlayer.Teleport(ray.origin + ray.direction * (float)@int);
		}

		// Token: 0x06003FF8 RID: 16376 RVA: 0x001790F0 File Offset: 0x001772F0
		[ServerVar]
		public static void teleport2owneditem(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			ulong userID;
			if (playerOrSleeper != null)
			{
				userID = playerOrSleeper.userID;
			}
			else if (!ulong.TryParse(arg.GetString(0, ""), out userID))
			{
				arg.ReplyWith("No player with that id found");
				return;
			}
			string @string = arg.GetString(1, "");
			BaseEntity[] array = BaseEntity.Util.FindTargetsOwnedBy(userID, @string);
			if (array.Length == 0)
			{
				arg.ReplyWith("No targets found");
				return;
			}
			int num = UnityEngine.Random.Range(0, array.Length);
			arg.ReplyWith(string.Format("Teleporting to {0} at {1}", array[num].ShortPrefabName, array[num].transform.position));
			basePlayer.Teleport(array[num].transform.position);
		}

		// Token: 0x06003FF9 RID: 16377 RVA: 0x001791B8 File Offset: 0x001773B8
		[ServerVar]
		public static void teleport2autheditem(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			ulong userID;
			if (playerOrSleeper != null)
			{
				userID = playerOrSleeper.userID;
			}
			else if (!ulong.TryParse(arg.GetString(0, ""), out userID))
			{
				arg.ReplyWith("No player with that id found");
				return;
			}
			string @string = arg.GetString(1, "");
			BaseEntity[] array = BaseEntity.Util.FindTargetsAuthedTo(userID, @string);
			if (array.Length == 0)
			{
				arg.ReplyWith("No targets found");
				return;
			}
			int num = UnityEngine.Random.Range(0, array.Length);
			arg.ReplyWith(string.Format("Teleporting to {0} at {1}", array[num].ShortPrefabName, array[num].transform.position));
			basePlayer.Teleport(array[num].transform.position);
		}

		// Token: 0x06003FFA RID: 16378 RVA: 0x00179280 File Offset: 0x00177480
		[ServerVar]
		public static void teleport2marker(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer.ServerCurrentMapNote == null)
			{
				arg.ReplyWith("You don't have a marker set");
				return;
			}
			Vector3 worldPosition = basePlayer.ServerCurrentMapNote.worldPosition;
			float height = TerrainMeta.HeightMap.GetHeight(worldPosition);
			float height2 = TerrainMeta.WaterMap.GetHeight(worldPosition);
			worldPosition.y = Mathf.Max(height, height2);
			basePlayer.Teleport(worldPosition);
		}

		// Token: 0x06003FFB RID: 16379 RVA: 0x001792E4 File Offset: 0x001774E4
		[ServerVar]
		public static void teleport2death(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer.ServerCurrentDeathNote == null)
			{
				arg.ReplyWith("You don't have a current death note!");
			}
			Vector3 worldPosition = basePlayer.ServerCurrentDeathNote.worldPosition;
			basePlayer.Teleport(worldPosition);
		}

		// Token: 0x06003FFC RID: 16380 RVA: 0x0017931C File Offset: 0x0017751C
		[ServerVar]
		[ClientVar]
		public static void free(ConsoleSystem.Arg args)
		{
			Pool.clear_prefabs(args);
			Pool.clear_assets(args);
			Pool.clear_memory(args);
			ConVar.GC.collect();
			ConVar.GC.unload();
		}

		// Token: 0x06003FFD RID: 16381 RVA: 0x0017933C File Offset: 0x0017753C
		[ServerVar(ServerUser = true)]
		[ClientVar]
		public static void version(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(string.Format("Protocol: {0}\nBuild Date: {1}\nUnity Version: {2}\nChangeset: {3}\nBranch: {4}", new object[]
			{
				Protocol.printable,
				BuildInfo.Current.BuildDate,
				UnityEngine.Application.unityVersion,
				BuildInfo.Current.Scm.ChangeId,
				BuildInfo.Current.Scm.Branch
			}));
		}

		// Token: 0x06003FFE RID: 16382 RVA: 0x001793A5 File Offset: 0x001775A5
		[ServerVar]
		[ClientVar]
		public static void sysinfo(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(SystemInfoGeneralText.currentInfo);
		}

		// Token: 0x06003FFF RID: 16383 RVA: 0x001793B2 File Offset: 0x001775B2
		[ServerVar]
		[ClientVar]
		public static void sysuid(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(SystemInfo.deviceUniqueIdentifier);
		}

		// Token: 0x06004000 RID: 16384 RVA: 0x001793C0 File Offset: 0x001775C0
		[ServerVar]
		public static void breakitem(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			Item activeItem = basePlayer.GetActiveItem();
			if (activeItem != null)
			{
				activeItem.LoseCondition(activeItem.condition);
			}
		}

		// Token: 0x06004001 RID: 16385 RVA: 0x001793F4 File Offset: 0x001775F4
		[ServerVar]
		public static void breakclothing(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!basePlayer)
			{
				return;
			}
			foreach (Item item in basePlayer.inventory.containerWear.itemList)
			{
				if (item != null)
				{
					item.LoseCondition(item.condition);
				}
			}
		}

		// Token: 0x06004002 RID: 16386 RVA: 0x0017946C File Offset: 0x0017766C
		[ServerVar]
		[ClientVar]
		public static void subscriptions(ConsoleSystem.Arg arg)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumn("realm");
			textTable.AddColumn("group");
			BasePlayer basePlayer = arg.Player();
			if (basePlayer)
			{
				foreach (Group group in basePlayer.net.subscriber.subscribed)
				{
					textTable.AddRow(new string[]
					{
						"sv",
						group.ID.ToString()
					});
				}
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x06004003 RID: 16387 RVA: 0x00179530 File Offset: 0x00177730
		public static uint GingerbreadMaterialID()
		{
			if (Global._gingerbreadMaterialID == 0U)
			{
				Global._gingerbreadMaterialID = StringPool.Get("Gingerbread");
			}
			return Global._gingerbreadMaterialID;
		}

		// Token: 0x06004004 RID: 16388 RVA: 0x00179550 File Offset: 0x00177750
		[ServerVar]
		public static void ClearAllSprays()
		{
			List<SprayCanSpray> list = Pool.GetList<SprayCanSpray>();
			foreach (SprayCanSpray item in SprayCanSpray.AllSprays)
			{
				list.Add(item);
			}
			foreach (SprayCanSpray sprayCanSpray in list)
			{
				sprayCanSpray.Kill(BaseNetworkable.DestroyMode.None);
			}
			Pool.FreeList<SprayCanSpray>(ref list);
		}

		// Token: 0x06004005 RID: 16389 RVA: 0x001795EC File Offset: 0x001777EC
		[ServerVar]
		public static void ClearAllSpraysByPlayer(ConsoleSystem.Arg arg)
		{
			if (!arg.HasArgs(1))
			{
				return;
			}
			ulong @ulong = arg.GetULong(0, 0UL);
			List<SprayCanSpray> list = Pool.GetList<SprayCanSpray>();
			foreach (SprayCanSpray sprayCanSpray in SprayCanSpray.AllSprays)
			{
				if (sprayCanSpray.sprayedByPlayer == @ulong)
				{
					list.Add(sprayCanSpray);
				}
			}
			foreach (SprayCanSpray sprayCanSpray2 in list)
			{
				sprayCanSpray2.Kill(BaseNetworkable.DestroyMode.None);
			}
			int count = list.Count;
			Pool.FreeList<SprayCanSpray>(ref list);
			arg.ReplyWith(string.Format("Deleted {0} sprays by {1}", count, @ulong));
		}

		// Token: 0x06004006 RID: 16390 RVA: 0x001796CC File Offset: 0x001778CC
		[ServerVar]
		public static void ClearSpraysInRadius(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			float @float = arg.GetFloat(0, 16f);
			int num = Global.ClearSpraysInRadius(basePlayer.transform.position, @float);
			arg.ReplyWith(string.Format("Deleted {0} sprays within {1} of {2}", num, @float, basePlayer.displayName));
		}

		// Token: 0x06004007 RID: 16391 RVA: 0x0017972C File Offset: 0x0017792C
		private static int ClearSpraysInRadius(Vector3 position, float radius)
		{
			List<SprayCanSpray> list = Pool.GetList<SprayCanSpray>();
			foreach (SprayCanSpray sprayCanSpray in SprayCanSpray.AllSprays)
			{
				if (sprayCanSpray.Distance(position) <= radius)
				{
					list.Add(sprayCanSpray);
				}
			}
			foreach (SprayCanSpray sprayCanSpray2 in list)
			{
				sprayCanSpray2.Kill(BaseNetworkable.DestroyMode.None);
			}
			int count = list.Count;
			Pool.FreeList<SprayCanSpray>(ref list);
			return count;
		}

		// Token: 0x06004008 RID: 16392 RVA: 0x001797D8 File Offset: 0x001779D8
		[ServerVar]
		public static void ClearSpraysAtPositionInRadius(ConsoleSystem.Arg arg)
		{
			Vector3 vector = arg.GetVector3(0, default(Vector3));
			float @float = arg.GetFloat(1, 0f);
			if (@float == 0f)
			{
				return;
			}
			int num = Global.ClearSpraysInRadius(vector, @float);
			arg.ReplyWith(string.Format("Deleted {0} sprays within {1} of {2}", num, @float, vector));
		}

		// Token: 0x06004009 RID: 16393 RVA: 0x00179838 File Offset: 0x00177A38
		[ServerVar]
		public static void ClearDroppedItems()
		{
			List<DroppedItem> list = Pool.GetList<DroppedItem>();
			using (IEnumerator<BaseNetworkable> enumerator = BaseNetworkable.serverEntities.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DroppedItem item;
					if ((item = (enumerator.Current as DroppedItem)) != null)
					{
						list.Add(item);
					}
				}
			}
			foreach (DroppedItem droppedItem in list)
			{
				droppedItem.Kill(BaseNetworkable.DestroyMode.None);
			}
			Pool.FreeList<DroppedItem>(ref list);
		}

		// Token: 0x04003930 RID: 14640
		private static int _developer;

		// Token: 0x04003931 RID: 14641
		[ServerVar]
		[ClientVar(Help = "WARNING: This causes random crashes!")]
		public static bool skipAssetWarmup_crashes = false;

		// Token: 0x04003932 RID: 14642
		[ServerVar]
		[ClientVar]
		public static int maxthreads = 8;

		// Token: 0x04003933 RID: 14643
		private const int DefaultWarmupConcurrency = 1;

		// Token: 0x04003934 RID: 14644
		private const int DefaultPreloadConcurrency = 1;

		// Token: 0x04003935 RID: 14645
		[ServerVar]
		[ClientVar]
		public static int warmupConcurrency = 1;

		// Token: 0x04003936 RID: 14646
		[ServerVar]
		[ClientVar]
		public static int preloadConcurrency = 1;

		// Token: 0x04003937 RID: 14647
		[ServerVar]
		[ClientVar]
		public static bool forceUnloadBundles = true;

		// Token: 0x04003938 RID: 14648
		private const bool DefaultAsyncWarmupEnabled = false;

		// Token: 0x04003939 RID: 14649
		[ServerVar]
		[ClientVar]
		public static bool asyncWarmup = false;

		// Token: 0x0400393A RID: 14650
		[ClientVar(Saved = true, Help = "Experimental faster loading, requires game restart (0 = off, 1 = partial, 2 = full)")]
		public static int asyncLoadingPreset = 0;

		// Token: 0x0400393B RID: 14651
		[ServerVar(Saved = true)]
		[ClientVar(Saved = true)]
		public static int perf = 0;

		// Token: 0x0400393C RID: 14652
		[ClientVar(ClientInfo = true, Saved = true, Help = "If you're an admin this will enable god mode")]
		public static bool god = false;

		// Token: 0x0400393D RID: 14653
		[ClientVar(ClientInfo = true, Saved = true, Help = "If enabled you will be networked when you're spectating. This means that you will hear audio chat, but also means that cheaters will potentially be able to detect you watching them.")]
		public static bool specnet = false;

		// Token: 0x0400393E RID: 14654
		[ClientVar]
		[ServerVar(ClientAdmin = true, ServerAdmin = true, Help = "When enabled a player wearing a gingerbread suit will gib like the gingerbread NPC's")]
		public static bool cinematicGingerbreadCorpses = false;

		// Token: 0x0400393F RID: 14655
		private static uint _gingerbreadMaterialID = 0U;

		// Token: 0x04003940 RID: 14656
		[ServerVar(Saved = true, ShowInAdminUI = true, Help = "Multiplier applied to SprayDuration if a spray isn't in the sprayers auth (cannot go above 1f)")]
		public static float SprayOutOfAuthMultiplier = 0.5f;

		// Token: 0x04003941 RID: 14657
		[ServerVar(Saved = true, ShowInAdminUI = true, Help = "Base time (in seconds) that sprays last")]
		public static float SprayDuration = 10800f;

		// Token: 0x04003942 RID: 14658
		[ServerVar(Saved = true, ShowInAdminUI = true, Help = "If a player sprays more than this, the oldest spray will be destroyed. 0 will disable")]
		public static int MaxSpraysPerPlayer = 40;

		// Token: 0x04003943 RID: 14659
		[ServerVar(Help = "Disables the backpacks that appear after a corpse times out")]
		public static bool disableBagDropping = false;
	}
}
