using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Epic.OnlineServices;
using Epic.OnlineServices.Logging;
using Epic.OnlineServices.Reports;
using Facepunch.Extend;
using Network;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000A97 RID: 2711
	[ConsoleSystem.Factory("server")]
	public class Server : ConsoleSystem
	{
		// Token: 0x1700055B RID: 1371
		// (get) Token: 0x060040A5 RID: 16549 RVA: 0x0017CDC5 File Offset: 0x0017AFC5
		// (set) Token: 0x060040A6 RID: 16550 RVA: 0x0017CDCC File Offset: 0x0017AFCC
		[ServerVar]
		public static int anticheatlog
		{
			get
			{
				return (int)EOS.LogLevel;
			}
			set
			{
				EOS.LogLevel = (LogLevel)value;
			}
		}

		// Token: 0x060040A7 RID: 16551 RVA: 0x0017CDD4 File Offset: 0x0017AFD4
		public static float TickDelta()
		{
			return 1f / (float)Server.tickrate;
		}

		// Token: 0x060040A8 RID: 16552 RVA: 0x0017CDE2 File Offset: 0x0017AFE2
		public static float TickTime(uint tick)
		{
			return (float)((double)Server.TickDelta() * tick);
		}

		// Token: 0x060040A9 RID: 16553 RVA: 0x0017CDF0 File Offset: 0x0017AFF0
		[ServerVar(Help = "Show holstered items on player bodies")]
		public static void setshowholstereditems(ConsoleSystem.Arg arg)
		{
			Server.showHolsteredItems = arg.GetBool(0, Server.showHolsteredItems);
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				basePlayer.inventory.UpdatedVisibleHolsteredItems();
			}
			foreach (BasePlayer basePlayer2 in BasePlayer.sleepingPlayerList)
			{
				basePlayer2.inventory.UpdatedVisibleHolsteredItems();
			}
		}

		// Token: 0x1700055C RID: 1372
		// (get) Token: 0x060040AA RID: 16554 RVA: 0x0017CE98 File Offset: 0x0017B098
		// (set) Token: 0x060040AB RID: 16555 RVA: 0x0017CE9F File Offset: 0x0017B09F
		[ServerVar]
		public static int maxconnectionsperip
		{
			get
			{
				return Server.MaxConnectionsPerIP;
			}
			set
			{
				Server.MaxConnectionsPerIP = Mathf.Clamp(value, 1, 1000);
			}
		}

		// Token: 0x1700055D RID: 1373
		// (get) Token: 0x060040AC RID: 16556 RVA: 0x0017CEB2 File Offset: 0x0017B0B2
		// (set) Token: 0x060040AD RID: 16557 RVA: 0x0017CEB9 File Offset: 0x0017B0B9
		[ServerVar]
		public static float maxreceivetime
		{
			get
			{
				return Server.MaxReceiveTime;
			}
			set
			{
				Server.MaxReceiveTime = Mathf.Clamp(value, 1f, 1000f);
			}
		}

		// Token: 0x1700055E RID: 1374
		// (get) Token: 0x060040AE RID: 16558 RVA: 0x0017CED0 File Offset: 0x0017B0D0
		// (set) Token: 0x060040AF RID: 16559 RVA: 0x0017CED7 File Offset: 0x0017B0D7
		[ServerVar]
		public static int maxreadqueue
		{
			get
			{
				return Server.MaxReadQueue;
			}
			set
			{
				Server.MaxReadQueue = Mathf.Max(value, 1);
			}
		}

		// Token: 0x1700055F RID: 1375
		// (get) Token: 0x060040B0 RID: 16560 RVA: 0x0017CEE5 File Offset: 0x0017B0E5
		// (set) Token: 0x060040B1 RID: 16561 RVA: 0x0017CEEC File Offset: 0x0017B0EC
		[ServerVar]
		public static int maxwritequeue
		{
			get
			{
				return Server.MaxWriteQueue;
			}
			set
			{
				Server.MaxWriteQueue = Mathf.Max(value, 1);
			}
		}

		// Token: 0x17000560 RID: 1376
		// (get) Token: 0x060040B2 RID: 16562 RVA: 0x0017CEFA File Offset: 0x0017B0FA
		// (set) Token: 0x060040B3 RID: 16563 RVA: 0x0017CF01 File Offset: 0x0017B101
		[ServerVar]
		public static int maxdecryptqueue
		{
			get
			{
				return Server.MaxDecryptQueue;
			}
			set
			{
				Server.MaxDecryptQueue = Mathf.Max(value, 1);
			}
		}

		// Token: 0x17000561 RID: 1377
		// (get) Token: 0x060040B4 RID: 16564 RVA: 0x0017CF0F File Offset: 0x0017B10F
		// (set) Token: 0x060040B5 RID: 16565 RVA: 0x0017CF17 File Offset: 0x0017B117
		[ServerVar]
		public static int maxpacketspersecond
		{
			get
			{
				return (int)Server.MaxPacketsPerSecond;
			}
			set
			{
				Server.MaxPacketsPerSecond = (ulong)((long)Mathf.Clamp(value, 1, 1000000));
			}
		}

		// Token: 0x17000562 RID: 1378
		// (get) Token: 0x060040B6 RID: 16566 RVA: 0x0017CF2B File Offset: 0x0017B12B
		// (set) Token: 0x060040B7 RID: 16567 RVA: 0x0017CF32 File Offset: 0x0017B132
		[ServerVar]
		public static int maxpacketsize
		{
			get
			{
				return Server.MaxPacketSize;
			}
			set
			{
				Server.MaxPacketSize = Mathf.Clamp(value, 1, 1000000000);
			}
		}

		// Token: 0x060040B8 RID: 16568 RVA: 0x0017CF48 File Offset: 0x0017B148
		[ServerVar]
		public static string packetlog(ConsoleSystem.Arg arg)
		{
			if (!Server.packetlog_enabled)
			{
				return "Packet log is not enabled.";
			}
			List<Tuple<Message.Type, ulong>> list = new List<Tuple<Message.Type, ulong>>();
			foreach (KeyValuePair<Message.Type, TimeAverageValue> keyValuePair in SingletonComponent<ServerMgr>.Instance.packetHistory.dict)
			{
				list.Add(new Tuple<Message.Type, ulong>(keyValuePair.Key, keyValuePair.Value.Calculate()));
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("type");
			textTable.AddColumn("calls");
			foreach (Tuple<Message.Type, ulong> tuple in from entry in list
			orderby entry.Item2 descending
			select entry)
			{
				if (tuple.Item2 == 0UL)
				{
					break;
				}
				string text = tuple.Item1.ToString();
				string text2 = tuple.Item2.ToString();
				textTable.AddRow(new string[]
				{
					text,
					text2
				});
			}
			if (!arg.HasArg("--json"))
			{
				return textTable.ToString();
			}
			return textTable.ToJson();
		}

		// Token: 0x060040B9 RID: 16569 RVA: 0x0017D0AC File Offset: 0x0017B2AC
		[ServerVar]
		public static string rpclog(ConsoleSystem.Arg arg)
		{
			if (!Server.rpclog_enabled)
			{
				return "RPC log is not enabled.";
			}
			List<Tuple<uint, ulong>> list = new List<Tuple<uint, ulong>>();
			foreach (KeyValuePair<uint, TimeAverageValue> keyValuePair in SingletonComponent<ServerMgr>.Instance.rpcHistory.dict)
			{
				list.Add(new Tuple<uint, ulong>(keyValuePair.Key, keyValuePair.Value.Calculate()));
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("id");
			textTable.AddColumn("name");
			textTable.AddColumn("calls");
			foreach (Tuple<uint, ulong> tuple in from entry in list
			orderby entry.Item2 descending
			select entry)
			{
				if (tuple.Item2 == 0UL)
				{
					break;
				}
				string text = tuple.Item1.ToString();
				string text2 = StringPool.Get(tuple.Item1);
				string text3 = tuple.Item2.ToString();
				textTable.AddRow(new string[]
				{
					text,
					text2,
					text3
				});
			}
			return textTable.ToString();
		}

		// Token: 0x060040BA RID: 16570 RVA: 0x0017D214 File Offset: 0x0017B414
		[ServerVar(Help = "Starts a server")]
		public static void start(ConsoleSystem.Arg arg)
		{
			if (Net.sv.IsConnected())
			{
				arg.ReplyWith("There is already a server running!");
				return;
			}
			string @string = arg.GetString(0, Server.level);
			if (!LevelManager.IsValid(@string))
			{
				arg.ReplyWith("Level '" + @string + "' isn't valid!");
				return;
			}
			if (UnityEngine.Object.FindObjectOfType<ServerMgr>())
			{
				arg.ReplyWith("There is already a server running!");
				return;
			}
			UnityEngine.Object.DontDestroyOnLoad(GameManager.server.CreatePrefab("assets/bundled/prefabs/system/server.prefab", true));
			LevelManager.LoadLevel(@string, true);
		}

		// Token: 0x060040BB RID: 16571 RVA: 0x0017D299 File Offset: 0x0017B499
		[ServerVar(Help = "Stops a server")]
		public static void stop(ConsoleSystem.Arg arg)
		{
			if (!Net.sv.IsConnected())
			{
				arg.ReplyWith("There isn't a server running!");
				return;
			}
			Net.sv.Stop(arg.GetString(0, "Stopping Server"));
		}

		// Token: 0x17000563 RID: 1379
		// (get) Token: 0x060040BC RID: 16572 RVA: 0x0017D2C9 File Offset: 0x0017B4C9
		public static string rootFolder
		{
			get
			{
				return "server/" + Server.identity;
			}
		}

		// Token: 0x17000564 RID: 1380
		// (get) Token: 0x060040BD RID: 16573 RVA: 0x0017D2DA File Offset: 0x0017B4DA
		public static string backupFolder
		{
			get
			{
				return "backup/0/" + Server.identity;
			}
		}

		// Token: 0x17000565 RID: 1381
		// (get) Token: 0x060040BE RID: 16574 RVA: 0x0017D2EB File Offset: 0x0017B4EB
		public static string backupFolder1
		{
			get
			{
				return "backup/1/" + Server.identity;
			}
		}

		// Token: 0x17000566 RID: 1382
		// (get) Token: 0x060040BF RID: 16575 RVA: 0x0017D2FC File Offset: 0x0017B4FC
		public static string backupFolder2
		{
			get
			{
				return "backup/2/" + Server.identity;
			}
		}

		// Token: 0x17000567 RID: 1383
		// (get) Token: 0x060040C0 RID: 16576 RVA: 0x0017D30D File Offset: 0x0017B50D
		public static string backupFolder3
		{
			get
			{
				return "backup/3/" + Server.identity;
			}
		}

		// Token: 0x060040C1 RID: 16577 RVA: 0x0017D31E File Offset: 0x0017B51E
		[ServerVar(Help = "Backup server folder")]
		public static void backup()
		{
			DirectoryEx.Backup(new string[]
			{
				Server.backupFolder,
				Server.backupFolder1,
				Server.backupFolder2,
				Server.backupFolder3
			});
			DirectoryEx.CopyAll(Server.rootFolder, Server.backupFolder);
		}

		// Token: 0x060040C2 RID: 16578 RVA: 0x0017D35C File Offset: 0x0017B55C
		public static string GetServerFolder(string folder)
		{
			string text = Server.rootFolder + "/" + folder;
			if (Directory.Exists(text))
			{
				return text;
			}
			Directory.CreateDirectory(text);
			return text;
		}

		// Token: 0x060040C3 RID: 16579 RVA: 0x0017D38C File Offset: 0x0017B58C
		[ServerVar(Help = "Writes config files")]
		public static void writecfg(ConsoleSystem.Arg arg)
		{
			string contents = ConsoleSystem.SaveToConfigString(true);
			File.WriteAllText(Server.GetServerFolder("cfg") + "/serverauto.cfg", contents);
			ServerUsers.Save();
			arg.ReplyWith("Config Saved");
		}

		// Token: 0x060040C4 RID: 16580 RVA: 0x0017D3CA File Offset: 0x0017B5CA
		[ServerVar]
		public static void fps(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(Performance.report.frameRate.ToString() + " FPS");
		}

		// Token: 0x060040C5 RID: 16581 RVA: 0x0017D3EC File Offset: 0x0017B5EC
		[ServerVar(Help = "Force save the current game")]
		public static void save(ConsoleSystem.Arg arg)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			foreach (BaseEntity baseEntity in BaseEntity.saveList)
			{
				baseEntity.InvalidateNetworkCache();
			}
			UnityEngine.Debug.Log("Invalidate Network Cache took " + stopwatch.Elapsed.TotalSeconds.ToString("0.00") + " seconds");
			SaveRestore.Save(true);
		}

		// Token: 0x060040C6 RID: 16582 RVA: 0x0017D478 File Offset: 0x0017B678
		[ServerVar]
		public static string readcfg(ConsoleSystem.Arg arg)
		{
			string serverFolder = Server.GetServerFolder("cfg");
			if (File.Exists(serverFolder + "/serverauto.cfg"))
			{
				string strFile = File.ReadAllText(serverFolder + "/serverauto.cfg");
				ConsoleSystem.RunFile(ConsoleSystem.Option.Server.Quiet(), strFile);
			}
			if (File.Exists(serverFolder + "/server.cfg"))
			{
				string strFile2 = File.ReadAllText(serverFolder + "/server.cfg");
				ConsoleSystem.RunFile(ConsoleSystem.Option.Server.Quiet(), strFile2);
			}
			return "Server Config Loaded";
		}

		// Token: 0x17000568 RID: 1384
		// (get) Token: 0x060040C7 RID: 16583 RVA: 0x0017D501 File Offset: 0x0017B701
		// (set) Token: 0x060040C8 RID: 16584 RVA: 0x0017D516 File Offset: 0x0017B716
		[ServerVar]
		public static bool compression
		{
			get
			{
				return Net.sv != null && Net.sv.compressionEnabled;
			}
			set
			{
				Net.sv.compressionEnabled = value;
			}
		}

		// Token: 0x17000569 RID: 1385
		// (get) Token: 0x060040C9 RID: 16585 RVA: 0x0017D523 File Offset: 0x0017B723
		// (set) Token: 0x060040CA RID: 16586 RVA: 0x0017D538 File Offset: 0x0017B738
		[ServerVar]
		public static bool netlog
		{
			get
			{
				return Net.sv != null && Net.sv.logging;
			}
			set
			{
				Net.sv.logging = value;
			}
		}

		// Token: 0x060040CB RID: 16587 RVA: 0x0017D545 File Offset: 0x0017B745
		[ServerVar]
		public static string netprotocol(ConsoleSystem.Arg arg)
		{
			if (Net.sv == null)
			{
				return string.Empty;
			}
			return Net.sv.ProtocolId;
		}

		// Token: 0x060040CC RID: 16588 RVA: 0x0017D560 File Offset: 0x0017B760
		[ServerUserVar]
		public static void cheatreport(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			ulong @uint = arg.GetUInt64(0, 0UL);
			string @string = arg.GetString(1, "");
			UnityEngine.Debug.LogWarning(string.Concat(new object[]
			{
				basePlayer,
				" reported ",
				@uint,
				": ",
				@string.ToPrintable(140)
			}));
			if (EACServer.Reports != null)
			{
				SendPlayerBehaviorReportOptions sendPlayerBehaviorReportOptions = new SendPlayerBehaviorReportOptions
				{
					ReportedUserId = ProductUserId.FromString(@uint.ToString()),
					ReporterUserId = ProductUserId.FromString(basePlayer.net.connection.userid.ToString()),
					Category = PlayerReportsCategory.Cheating,
					Message = @string
				};
				EACServer.Reports.SendPlayerBehaviorReport(ref sendPlayerBehaviorReportOptions, null, null);
			}
		}

		// Token: 0x060040CD RID: 16589 RVA: 0x0017D64C File Offset: 0x0017B84C
		[ServerAllVar(Help = "Get the player combat log")]
		public static string combatlog(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (arg.HasArgs(1) && arg.IsAdmin)
			{
				basePlayer = arg.GetPlayerOrSleeper(0);
			}
			if (basePlayer == null || basePlayer.net == null)
			{
				return "invalid player";
			}
			CombatLog combat = basePlayer.stats.combat;
			int count = Server.combatlogsize;
			uint filterByAttacker = 0U;
			bool json = arg.HasArg("--json");
			bool isAdmin = arg.IsAdmin;
			Connection connection = arg.Connection;
			return combat.Get(count, filterByAttacker, json, isAdmin, (connection != null) ? connection.userid : 0UL);
		}

		// Token: 0x060040CE RID: 16590 RVA: 0x0017D6CC File Offset: 0x0017B8CC
		[ServerAllVar(Help = "Get the player combat log, only showing outgoing damage")]
		public static string combatlog_outgoing(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (arg.HasArgs(1) && arg.IsAdmin)
			{
				basePlayer = arg.GetPlayerOrSleeper(0);
			}
			if (basePlayer == null)
			{
				return "invalid player";
			}
			CombatLog combat = basePlayer.stats.combat;
			int count = Server.combatlogsize;
			uint id = basePlayer.net.ID;
			bool json = arg.HasArg("--json");
			bool isAdmin = arg.IsAdmin;
			Connection connection = arg.Connection;
			return combat.Get(count, id, json, isAdmin, (connection != null) ? connection.userid : 0UL);
		}

		// Token: 0x060040CF RID: 16591 RVA: 0x0017D74C File Offset: 0x0017B94C
		[ServerVar(Help = "Print the current player position.")]
		public static string printpos(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (arg.HasArgs(1))
			{
				basePlayer = arg.GetPlayerOrSleeper(0);
			}
			if (!(basePlayer == null))
			{
				return basePlayer.transform.position.ToString();
			}
			return "invalid player";
		}

		// Token: 0x060040D0 RID: 16592 RVA: 0x0017D79C File Offset: 0x0017B99C
		[ServerVar(Help = "Print the current player rotation.")]
		public static string printrot(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (arg.HasArgs(1))
			{
				basePlayer = arg.GetPlayerOrSleeper(0);
			}
			if (!(basePlayer == null))
			{
				return basePlayer.transform.rotation.eulerAngles.ToString();
			}
			return "invalid player";
		}

		// Token: 0x060040D1 RID: 16593 RVA: 0x0017D7F4 File Offset: 0x0017B9F4
		[ServerVar(Help = "Print the current player eyes.")]
		public static string printeyes(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (arg.HasArgs(1))
			{
				basePlayer = arg.GetPlayerOrSleeper(0);
			}
			if (!(basePlayer == null))
			{
				return basePlayer.eyes.rotation.eulerAngles.ToString();
			}
			return "invalid player";
		}

		// Token: 0x060040D2 RID: 16594 RVA: 0x0017D84C File Offset: 0x0017BA4C
		[ServerVar(ServerAdmin = true, Help = "This sends a snapshot of all the entities in the client's pvs. This is mostly redundant, but we request this when the client starts recording a demo.. so they get all the information.")]
		public static void snapshot(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			UnityEngine.Debug.Log("Sending full snapshot to " + basePlayer);
			basePlayer.SendNetworkUpdateImmediate(false);
			basePlayer.SendGlobalSnapshot();
			basePlayer.SendFullSnapshot();
			basePlayer.SendEntityUpdate();
			TreeManager.SendSnapshot(basePlayer);
			ServerMgr.SendReplicatedVars(basePlayer.net.connection);
		}

		// Token: 0x060040D3 RID: 16595 RVA: 0x0017D8AC File Offset: 0x0017BAAC
		[ServerVar(Help = "Send network update for all players")]
		public static void sendnetworkupdate(ConsoleSystem.Arg arg)
		{
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				basePlayer.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			}
		}

		// Token: 0x060040D4 RID: 16596 RVA: 0x0017D8FC File Offset: 0x0017BAFC
		[ServerVar(Help = "Prints the position of all players on the server")]
		public static void playerlistpos(ConsoleSystem.Arg arg)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumns(new string[]
			{
				"SteamID",
				"DisplayName",
				"POS",
				"ROT"
			});
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				textTable.AddRow(new string[]
				{
					basePlayer.userID.ToString(),
					basePlayer.displayName,
					basePlayer.transform.position.ToString(),
					basePlayer.eyes.BodyForward().ToString()
				});
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x060040D5 RID: 16597 RVA: 0x0017D9F8 File Offset: 0x0017BBF8
		[ServerVar(Help = "Prints all the vending machines on the server")]
		public static void listvendingmachines(ConsoleSystem.Arg arg)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumns(new string[]
			{
				"EntityId",
				"Position",
				"Name"
			});
			foreach (VendingMachine vendingMachine in BaseNetworkable.serverEntities.OfType<VendingMachine>())
			{
				textTable.AddRow(new string[]
				{
					vendingMachine.net.ID.ToString(),
					vendingMachine.transform.position.ToString(),
					vendingMachine.shopName.QuoteSafe()
				});
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x060040D6 RID: 16598 RVA: 0x0017DAD8 File Offset: 0x0017BCD8
		[ServerVar(Help = "Prints all the Tool Cupboards on the server")]
		public static void listtoolcupboards(ConsoleSystem.Arg arg)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumns(new string[]
			{
				"EntityId",
				"Position",
				"Authed"
			});
			foreach (BuildingPrivlidge buildingPrivlidge in BaseNetworkable.serverEntities.OfType<BuildingPrivlidge>())
			{
				textTable.AddRow(new string[]
				{
					buildingPrivlidge.net.ID.ToString(),
					buildingPrivlidge.transform.position.ToString(),
					buildingPrivlidge.authorizedPlayers.Count.ToString()
				});
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x04003989 RID: 14729
		[ServerVar]
		public static string ip = "";

		// Token: 0x0400398A RID: 14730
		[ServerVar]
		public static int port = 28015;

		// Token: 0x0400398B RID: 14731
		[ServerVar]
		public static int queryport = 0;

		// Token: 0x0400398C RID: 14732
		[ServerVar(ShowInAdminUI = true)]
		public static int maxplayers = 500;

		// Token: 0x0400398D RID: 14733
		[ServerVar(ShowInAdminUI = true)]
		public static string hostname = "My Untitled Rust Server";

		// Token: 0x0400398E RID: 14734
		[ServerVar]
		public static string identity = "my_server_identity";

		// Token: 0x0400398F RID: 14735
		[ServerVar]
		public static string level = "Procedural Map";

		// Token: 0x04003990 RID: 14736
		[ServerVar]
		public static string levelurl = "";

		// Token: 0x04003991 RID: 14737
		[ServerVar]
		public static bool leveltransfer = true;

		// Token: 0x04003992 RID: 14738
		[ServerVar]
		public static int seed = 1337;

		// Token: 0x04003993 RID: 14739
		[ServerVar]
		public static int salt = 1;

		// Token: 0x04003994 RID: 14740
		[ServerVar]
		public static int worldsize = 4500;

		// Token: 0x04003995 RID: 14741
		[ServerVar]
		public static int saveinterval = 600;

		// Token: 0x04003996 RID: 14742
		[ServerVar]
		public static bool secure = true;

		// Token: 0x04003997 RID: 14743
		[ServerVar]
		public static int encryption = 2;

		// Token: 0x04003998 RID: 14744
		[ServerVar]
		public static string anticheatid = "xyza7891h6UjNfd0eb2HQGtaul0WhfvS";

		// Token: 0x04003999 RID: 14745
		[ServerVar]
		public static string anticheatkey = "OWUDFZmi9VNL/7VhGVSSmCWALKTltKw8ISepa0VXs60";

		// Token: 0x0400399A RID: 14746
		[ServerVar]
		public static int tickrate = 10;

		// Token: 0x0400399B RID: 14747
		[ServerVar]
		public static int entityrate = 16;

		// Token: 0x0400399C RID: 14748
		[ServerVar]
		public static float schematime = 1800f;

		// Token: 0x0400399D RID: 14749
		[ServerVar]
		public static float cycletime = 500f;

		// Token: 0x0400399E RID: 14750
		[ServerVar]
		public static bool official = false;

		// Token: 0x0400399F RID: 14751
		[ServerVar]
		public static bool stats = false;

		// Token: 0x040039A0 RID: 14752
		[ServerVar]
		public static bool stability = true;

		// Token: 0x040039A1 RID: 14753
		[ServerVar(ShowInAdminUI = true)]
		public static bool radiation = true;

		// Token: 0x040039A2 RID: 14754
		[ServerVar]
		public static float itemdespawn = 300f;

		// Token: 0x040039A3 RID: 14755
		[ServerVar]
		public static float itemdespawn_container_scale = 2f;

		// Token: 0x040039A4 RID: 14756
		[ServerVar]
		public static float itemdespawn_quick = 30f;

		// Token: 0x040039A5 RID: 14757
		[ServerVar]
		public static float corpsedespawn = 300f;

		// Token: 0x040039A6 RID: 14758
		[ServerVar]
		public static float debrisdespawn = 30f;

		// Token: 0x040039A7 RID: 14759
		[ServerVar]
		public static bool pve = false;

		// Token: 0x040039A8 RID: 14760
		[ServerVar]
		public static bool cinematic = false;

		// Token: 0x040039A9 RID: 14761
		[ServerVar(ShowInAdminUI = true)]
		public static string description = "No server description has been provided.";

		// Token: 0x040039AA RID: 14762
		[ServerVar(ShowInAdminUI = true)]
		public static string url = "";

		// Token: 0x040039AB RID: 14763
		[ServerVar]
		public static string branch = "";

		// Token: 0x040039AC RID: 14764
		[ServerVar]
		public static int queriesPerSecond = 2000;

		// Token: 0x040039AD RID: 14765
		[ServerVar]
		public static int ipQueriesPerMin = 30;

		// Token: 0x040039AE RID: 14766
		[ServerVar]
		public static bool statBackup = false;

		// Token: 0x040039AF RID: 14767
		[ServerVar(Saved = true, ShowInAdminUI = true)]
		public static string headerimage = "";

		// Token: 0x040039B0 RID: 14768
		[ServerVar(Saved = true, ShowInAdminUI = true)]
		public static string logoimage = "";

		// Token: 0x040039B1 RID: 14769
		[ServerVar(Saved = true, ShowInAdminUI = true)]
		public static int saveBackupCount = 2;

		// Token: 0x040039B2 RID: 14770
		[ReplicatedVar(Saved = true, ShowInAdminUI = true)]
		public static string motd = "";

		// Token: 0x040039B3 RID: 14771
		[ServerVar(Saved = true)]
		public static float meleedamage = 1f;

		// Token: 0x040039B4 RID: 14772
		[ServerVar(Saved = true)]
		public static float arrowdamage = 1f;

		// Token: 0x040039B5 RID: 14773
		[ServerVar(Saved = true)]
		public static float bulletdamage = 1f;

		// Token: 0x040039B6 RID: 14774
		[ServerVar(Saved = true)]
		public static float bleedingdamage = 1f;

		// Token: 0x040039B7 RID: 14775
		[ReplicatedVar(Saved = true)]
		public static float funWaterDamageThreshold = 0.8f;

		// Token: 0x040039B8 RID: 14776
		[ReplicatedVar(Saved = true)]
		public static float funWaterWetnessGain = 0.05f;

		// Token: 0x040039B9 RID: 14777
		[ServerVar(Saved = true)]
		public static float meleearmor = 1f;

		// Token: 0x040039BA RID: 14778
		[ServerVar(Saved = true)]
		public static float arrowarmor = 1f;

		// Token: 0x040039BB RID: 14779
		[ServerVar(Saved = true)]
		public static float bulletarmor = 1f;

		// Token: 0x040039BC RID: 14780
		[ServerVar(Saved = true)]
		public static float bleedingarmor = 1f;

		// Token: 0x040039BD RID: 14781
		[ServerVar]
		public static int updatebatch = 512;

		// Token: 0x040039BE RID: 14782
		[ServerVar]
		public static int updatebatchspawn = 1024;

		// Token: 0x040039BF RID: 14783
		[ServerVar]
		public static int entitybatchsize = 100;

		// Token: 0x040039C0 RID: 14784
		[ServerVar]
		public static float entitybatchtime = 1f;

		// Token: 0x040039C1 RID: 14785
		[ServerVar]
		public static float composterUpdateInterval = 300f;

		// Token: 0x040039C2 RID: 14786
		[ReplicatedVar]
		public static float planttick = 60f;

		// Token: 0x040039C3 RID: 14787
		[ServerVar]
		public static float planttickscale = 1f;

		// Token: 0x040039C4 RID: 14788
		[ServerVar]
		public static bool useMinimumPlantCondition = true;

		// Token: 0x040039C5 RID: 14789
		[ServerVar(Saved = true)]
		public static float nonPlanterDeathChancePerTick = 0.005f;

		// Token: 0x040039C6 RID: 14790
		[ServerVar(Saved = true)]
		public static float ceilingLightGrowableRange = 3f;

		// Token: 0x040039C7 RID: 14791
		[ServerVar(Saved = true)]
		public static float artificialTemperatureGrowableRange = 4f;

		// Token: 0x040039C8 RID: 14792
		[ServerVar(Saved = true)]
		public static float ceilingLightHeightOffset = 3f;

		// Token: 0x040039C9 RID: 14793
		[ServerVar(Saved = true)]
		public static float sprinklerRadius = 3f;

		// Token: 0x040039CA RID: 14794
		[ServerVar(Saved = true)]
		public static float sprinklerEyeHeightOffset = 3f;

		// Token: 0x040039CB RID: 14795
		[ServerVar(Saved = true)]
		public static float optimalPlanterQualitySaturation = 0.6f;

		// Token: 0x040039CC RID: 14796
		[ServerVar]
		public static float metabolismtick = 1f;

		// Token: 0x040039CD RID: 14797
		[ServerVar]
		public static float modifierTickRate = 1f;

		// Token: 0x040039CE RID: 14798
		[ServerVar(Saved = true)]
		public static float rewounddelay = 60f;

		// Token: 0x040039CF RID: 14799
		[ServerVar(Saved = true, Help = "Can players be wounded after recieving fatal damage")]
		public static bool woundingenabled = true;

		// Token: 0x040039D0 RID: 14800
		[ServerVar(Saved = true, Help = "Do players go into the crawling wounded state")]
		public static bool crawlingenabled = true;

		// Token: 0x040039D1 RID: 14801
		[ServerVar(Help = "Base chance of recovery after crawling wounded state", Saved = true)]
		public static float woundedrecoverchance = 0.2f;

		// Token: 0x040039D2 RID: 14802
		[ServerVar(Help = "Base chance of recovery after incapacitated wounded state", Saved = true)]
		public static float incapacitatedrecoverchance = 0.1f;

		// Token: 0x040039D3 RID: 14803
		[ServerVar(Help = "Maximum percent chance added to base wounded/incapacitated recovery chance, based on the player's food and water level", Saved = true)]
		public static float woundedmaxfoodandwaterbonus = 0.25f;

		// Token: 0x040039D4 RID: 14804
		[ServerVar(Help = "Minimum initial health given when a player dies and moves to crawling wounded state", Saved = false)]
		public static int crawlingminimumhealth = 7;

		// Token: 0x040039D5 RID: 14805
		[ServerVar(Help = "Maximum initial health given when a player dies and moves to crawling wounded state", Saved = false)]
		public static int crawlingmaximumhealth = 12;

		// Token: 0x040039D6 RID: 14806
		[ServerVar(Saved = true)]
		public static bool playerserverfall = true;

		// Token: 0x040039D7 RID: 14807
		[ServerVar]
		public static bool plantlightdetection = true;

		// Token: 0x040039D8 RID: 14808
		[ServerVar]
		public static float respawnresetrange = 50f;

		// Token: 0x040039D9 RID: 14809
		[ServerVar]
		public static int maxunack = 4;

		// Token: 0x040039DA RID: 14810
		[ServerVar]
		public static bool netcache = true;

		// Token: 0x040039DB RID: 14811
		[ServerVar]
		public static bool corpses = true;

		// Token: 0x040039DC RID: 14812
		[ServerVar]
		public static bool events = true;

		// Token: 0x040039DD RID: 14813
		[ServerVar]
		public static bool dropitems = true;

		// Token: 0x040039DE RID: 14814
		[ServerVar]
		public static int netcachesize = 0;

		// Token: 0x040039DF RID: 14815
		[ServerVar]
		public static int savecachesize = 0;

		// Token: 0x040039E0 RID: 14816
		[ServerVar]
		public static int combatlogsize = 30;

		// Token: 0x040039E1 RID: 14817
		[ServerVar]
		public static int combatlogdelay = 10;

		// Token: 0x040039E2 RID: 14818
		[ServerVar]
		public static int authtimeout = 60;

		// Token: 0x040039E3 RID: 14819
		[ServerVar]
		public static int playertimeout = 60;

		// Token: 0x040039E4 RID: 14820
		[ServerVar(ShowInAdminUI = true)]
		public static int idlekick = 30;

		// Token: 0x040039E5 RID: 14821
		[ServerVar]
		public static int idlekickmode = 1;

		// Token: 0x040039E6 RID: 14822
		[ServerVar]
		public static int idlekickadmins = 0;

		// Token: 0x040039E7 RID: 14823
		[ServerVar]
		public static string gamemode = "";

		// Token: 0x040039E8 RID: 14824
		[ServerVar(Help = "Comma-separated server browser tag values (see wiki)", Saved = true, ShowInAdminUI = true)]
		public static string tags = "";

		// Token: 0x040039E9 RID: 14825
		[ServerVar(Help = "Censors the Steam player list to make player tracking more difficult")]
		public static bool censorplayerlist = true;

		// Token: 0x040039EA RID: 14826
		[ServerVar(Help = "HTTP API endpoint for centralized banning (see wiki)")]
		public static string bansServerEndpoint = "";

		// Token: 0x040039EB RID: 14827
		[ServerVar(Help = "Failure mode for centralized banning, set to 1 to reject players from joining if it's down (see wiki)")]
		public static int bansServerFailureMode = 0;

		// Token: 0x040039EC RID: 14828
		[ServerVar(Help = "Timeout (in seconds) for centralized banning web server requests")]
		public static int bansServerTimeout = 5;

		// Token: 0x040039ED RID: 14829
		[ServerVar(Help = "HTTP API endpoint for receiving F7 reports", Saved = true)]
		public static string reportsServerEndpoint = "";

		// Token: 0x040039EE RID: 14830
		[ServerVar(Help = "If set, this key will be included with any reports sent via reportsServerEndpoint (for validation)", Saved = true)]
		public static string reportsServerEndpointKey = "";

		// Token: 0x040039EF RID: 14831
		[ServerVar(Help = "Should F7 reports from players be printed to console", Saved = true)]
		public static bool printReportsToConsole = false;

		// Token: 0x040039F0 RID: 14832
		[ServerVar(Help = "If a player presses the respawn button, respawn at their death location (for trailer filming)")]
		public static bool respawnAtDeathPosition = false;

		// Token: 0x040039F1 RID: 14833
		[ServerVar(Help = "When a player respawns give them the loadout assigned to client.RespawnLoadout (created with inventory.saveloadout)")]
		public static bool respawnWithLoadout = false;

		// Token: 0x040039F2 RID: 14834
		[ServerVar(Help = "When transferring water, should containers keep 1 water behind. Enabling this should help performance if water IO is causing performance loss", Saved = true)]
		public static bool waterContainersLeaveWaterBehind = false;

		// Token: 0x040039F3 RID: 14835
		[ServerVar(Help = "How often industrial conveyors attempt to move items. Setting to 0 will disable all movement", Saved = true, ShowInAdminUI = true)]
		public static float conveyorMoveFrequency = 5f;

		// Token: 0x040039F4 RID: 14836
		[ServerVar(Help = "How often industrial crafters attempt to craft items. Setting to 0 will disable all crafting", Saved = true, ShowInAdminUI = true)]
		public static float industrialCrafterFrequency = 5f;

		// Token: 0x040039F5 RID: 14837
		[ReplicatedVar(Help = "How much scrap is required to research default blueprints", Saved = true, ShowInAdminUI = true)]
		public static int defaultBlueprintResearchCost = 10;

		// Token: 0x040039F6 RID: 14838
		[ServerVar(Help = "Whether to check for illegal industrial pipes when changing building block states (roof bunkers)", Saved = true, ShowInAdminUI = true)]
		public static bool enforcePipeChecksOnBuildingBlockChanges = true;

		// Token: 0x040039F7 RID: 14839
		[ServerVar(Help = "How many stacks a single conveyor can move in a single tick", Saved = true, ShowInAdminUI = true)]
		public static int maxItemStacksMovedPerTickIndustrial = 12;

		// Token: 0x040039F8 RID: 14840
		[ServerVar(Saved = true)]
		public static bool showHolsteredItems = true;

		// Token: 0x040039F9 RID: 14841
		[ServerVar]
		public static int maxpacketspersecond_world = 1;

		// Token: 0x040039FA RID: 14842
		[ServerVar]
		public static int maxpacketspersecond_rpc = 200;

		// Token: 0x040039FB RID: 14843
		[ServerVar]
		public static int maxpacketspersecond_rpc_signal = 50;

		// Token: 0x040039FC RID: 14844
		[ServerVar]
		public static int maxpacketspersecond_command = 100;

		// Token: 0x040039FD RID: 14845
		[ServerVar]
		public static int maxpacketsize_command = 100000;

		// Token: 0x040039FE RID: 14846
		[ServerVar]
		public static int maxpacketspersecond_tick = 300;

		// Token: 0x040039FF RID: 14847
		[ServerVar]
		public static int maxpacketspersecond_voice = 100;

		// Token: 0x04003A00 RID: 14848
		[ServerVar]
		public static bool packetlog_enabled = false;

		// Token: 0x04003A01 RID: 14849
		[ServerVar]
		public static bool rpclog_enabled = false;
	}
}
