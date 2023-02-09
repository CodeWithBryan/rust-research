using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Math;
using Network;
using Newtonsoft.Json;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Scripting;

namespace ConVar
{
	// Token: 0x02000A60 RID: 2656
	[ConsoleSystem.Factory("global")]
	public class Admin : ConsoleSystem
	{
		// Token: 0x06003EFD RID: 16125 RVA: 0x00171EF0 File Offset: 0x001700F0
		[ServerVar(Help = "Print out currently connected clients")]
		public static void status(ConsoleSystem.Arg arg)
		{
			string @string = arg.GetString(0, "");
			if (@string == "--json")
			{
				@string = arg.GetString(1, "");
			}
			bool flag = arg.HasArg("--json");
			string str = string.Empty;
			if (!flag && @string.Length == 0)
			{
				str = str + "hostname: " + ConVar.Server.hostname + "\n";
				str = str + "version : " + 2370.ToString() + " secure (secure mode enabled, connected to Steam3)\n";
				str = str + "map     : " + ConVar.Server.level + "\n";
				str += string.Format("players : {0} ({1} max) ({2} queued) ({3} joining)\n\n", new object[]
				{
					global::BasePlayer.activePlayerList.Count<global::BasePlayer>(),
					ConVar.Server.maxplayers,
					SingletonComponent<ServerMgr>.Instance.connectionQueue.Queued,
					SingletonComponent<ServerMgr>.Instance.connectionQueue.Joining
				});
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("id");
			textTable.AddColumn("name");
			textTable.AddColumn("ping");
			textTable.AddColumn("connected");
			textTable.AddColumn("addr");
			textTable.AddColumn("owner");
			textTable.AddColumn("violation");
			textTable.AddColumn("kicks");
			foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
			{
				try
				{
					if (basePlayer.IsValid())
					{
						string userIDString = basePlayer.UserIDString;
						if (basePlayer.net.connection == null)
						{
							textTable.AddRow(new string[]
							{
								userIDString,
								"NO CONNECTION"
							});
						}
						else
						{
							string text = basePlayer.net.connection.ownerid.ToString();
							string text2 = basePlayer.displayName.QuoteSafe();
							string text3 = Net.sv.GetAveragePing(basePlayer.net.connection).ToString();
							string text4 = basePlayer.net.connection.ipaddress;
							string text5 = basePlayer.violationLevel.ToString("0.0");
							string text6 = basePlayer.GetAntiHackKicks().ToString();
							if (!arg.IsAdmin && !arg.IsRcon)
							{
								text4 = "xx.xxx.xx.xxx";
							}
							string text7 = basePlayer.net.connection.GetSecondsConnected().ToString() + "s";
							if (@string.Length <= 0 || text2.Contains(@string, CompareOptions.IgnoreCase) || userIDString.Contains(@string) || text.Contains(@string) || text4.Contains(@string))
							{
								textTable.AddRow(new string[]
								{
									userIDString,
									text2,
									text3,
									text7,
									text4,
									(text == userIDString) ? string.Empty : text,
									text5,
									text6
								});
							}
						}
					}
				}
				catch (Exception ex)
				{
					textTable.AddRow(new string[]
					{
						basePlayer.UserIDString,
						ex.Message.QuoteSafe()
					});
				}
			}
			if (flag)
			{
				arg.ReplyWith(textTable.ToJson());
				return;
			}
			arg.ReplyWith(str + textTable.ToString());
		}

		// Token: 0x06003EFE RID: 16126 RVA: 0x00172284 File Offset: 0x00170484
		[ServerVar(Help = "Print out stats of currently connected clients")]
		public static void stats(ConsoleSystem.Arg arg)
		{
			TextTable table = new TextTable();
			table.AddColumn("id");
			table.AddColumn("name");
			table.AddColumn("time");
			table.AddColumn("kills");
			table.AddColumn("deaths");
			table.AddColumn("suicides");
			table.AddColumn("player");
			table.AddColumn("building");
			table.AddColumn("entity");
			Action<ulong, string> action = delegate(ulong id, string name)
			{
				ServerStatistics.Storage storage = ServerStatistics.Get(id);
				string text2 = TimeSpan.FromSeconds((double)storage.Get("time")).ToShortString();
				string text3 = storage.Get("kill_player").ToString();
				string text4 = (storage.Get("deaths") - storage.Get("death_suicide")).ToString();
				string text5 = storage.Get("death_suicide").ToString();
				string str = storage.Get("hit_player_direct_los").ToString();
				string str2 = storage.Get("hit_player_indirect_los").ToString();
				string str3 = storage.Get("hit_building_direct_los").ToString();
				string str4 = storage.Get("hit_building_indirect_los").ToString();
				string str5 = storage.Get("hit_entity_direct_los").ToString();
				string str6 = storage.Get("hit_entity_indirect_los").ToString();
				table.AddRow(new string[]
				{
					id.ToString(),
					name,
					text2,
					text3,
					text4,
					text5,
					str + " / " + str2,
					str3 + " / " + str4,
					str5 + " / " + str6
				});
			};
			ulong @uint = arg.GetUInt64(0, 0UL);
			if (@uint == 0UL)
			{
				string @string = arg.GetString(0, "");
				using (ListHashSet<global::BasePlayer>.Enumerator enumerator = global::BasePlayer.activePlayerList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						global::BasePlayer basePlayer = enumerator.Current;
						try
						{
							if (basePlayer.IsValid())
							{
								string text = basePlayer.displayName.QuoteSafe();
								if (@string.Length <= 0 || text.Contains(@string, CompareOptions.IgnoreCase))
								{
									action(basePlayer.userID, text);
								}
							}
						}
						catch (Exception ex)
						{
							table.AddRow(new string[]
							{
								basePlayer.UserIDString,
								ex.Message.QuoteSafe()
							});
						}
					}
					goto IL_198;
				}
			}
			string arg2 = "N/A";
			global::BasePlayer basePlayer2 = global::BasePlayer.FindByID(@uint);
			if (basePlayer2)
			{
				arg2 = basePlayer2.displayName.QuoteSafe();
			}
			action(@uint, arg2);
			IL_198:
			arg.ReplyWith(arg.HasArg("--json") ? table.ToJson() : table.ToString());
		}

		// Token: 0x06003EFF RID: 16127 RVA: 0x00172470 File Offset: 0x00170670
		[ServerVar]
		public static void killplayer(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.GetPlayerOrSleeper(0);
			if (!basePlayer)
			{
				basePlayer = global::BasePlayer.FindBotClosestMatch(arg.GetString(0, ""));
			}
			if (!basePlayer)
			{
				arg.ReplyWith("Player not found");
				return;
			}
			basePlayer.Hurt(1000f, DamageType.Suicide, basePlayer, false);
		}

		// Token: 0x06003F00 RID: 16128 RVA: 0x001724C4 File Offset: 0x001706C4
		[ServerVar]
		public static void injureplayer(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.GetPlayerOrSleeper(0);
			if (!basePlayer)
			{
				basePlayer = global::BasePlayer.FindBotClosestMatch(arg.GetString(0, ""));
			}
			if (!basePlayer)
			{
				arg.ReplyWith("Player not found");
				return;
			}
			Global.InjurePlayer(basePlayer);
		}

		// Token: 0x06003F01 RID: 16129 RVA: 0x00172510 File Offset: 0x00170710
		[ServerVar]
		public static void recoverplayer(ConsoleSystem.Arg arg)
		{
			global::BasePlayer basePlayer = arg.GetPlayerOrSleeper(0);
			if (!basePlayer)
			{
				basePlayer = global::BasePlayer.FindBotClosestMatch(arg.GetString(0, ""));
			}
			if (!basePlayer)
			{
				arg.ReplyWith("Player not found");
				return;
			}
			Global.RecoverPlayer(basePlayer);
		}

		// Token: 0x06003F02 RID: 16130 RVA: 0x0017255C File Offset: 0x0017075C
		[ServerVar]
		public static void kick(ConsoleSystem.Arg arg)
		{
			global::BasePlayer player = arg.GetPlayer(0);
			if (!player || player.net == null || player.net.connection == null)
			{
				arg.ReplyWith("Player not found");
				return;
			}
			string @string = arg.GetString(1, "no reason given");
			arg.ReplyWith("Kicked: " + player.displayName);
			Chat.Broadcast(string.Concat(new string[]
			{
				"Kicking ",
				player.displayName,
				" (",
				@string,
				")"
			}), "SERVER", "#eee", 0UL);
			player.Kick("Kicked: " + arg.GetString(1, "No Reason Given"));
		}

		// Token: 0x06003F03 RID: 16131 RVA: 0x0017261C File Offset: 0x0017081C
		[ServerVar]
		public static void kickall(ConsoleSystem.Arg arg)
		{
			global::BasePlayer[] array = global::BasePlayer.activePlayerList.ToArray<global::BasePlayer>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Kick("Kicked: " + arg.GetString(1, "No Reason Given"));
			}
		}

		// Token: 0x06003F04 RID: 16132 RVA: 0x00172660 File Offset: 0x00170860
		[ServerVar(Help = "ban <player> <reason> [optional duration]")]
		public static void ban(ConsoleSystem.Arg arg)
		{
			global::BasePlayer player = arg.GetPlayer(0);
			if (!player || player.net == null || player.net.connection == null)
			{
				arg.ReplyWith("Player not found");
				return;
			}
			ServerUsers.User user = ServerUsers.Get(player.userID);
			if (user != null && user.group == ServerUsers.UserGroup.Banned)
			{
				arg.ReplyWith(string.Format("User {0} is already banned", player.userID));
				return;
			}
			string @string = arg.GetString(1, "No Reason Given");
			long expiry;
			string text;
			if (!Admin.TryGetBanExpiry(arg, 2, out expiry, out text))
			{
				return;
			}
			ServerUsers.Set(player.userID, ServerUsers.UserGroup.Banned, player.displayName, @string, expiry);
			string text2 = "";
			if (player.IsConnected && player.net.connection.ownerid != 0UL && player.net.connection.ownerid != player.net.connection.userid)
			{
				text2 += string.Format(" and also banned ownerid {0}", player.net.connection.ownerid);
				ServerUsers.Set(player.net.connection.ownerid, ServerUsers.UserGroup.Banned, player.displayName, arg.GetString(1, string.Format("Family share owner of {0}", player.net.connection.userid)), -1L);
			}
			ServerUsers.Save();
			arg.ReplyWith(string.Format("Kickbanned User{0}: {1} - {2}{3}", new object[]
			{
				text,
				player.userID,
				player.displayName,
				text2
			}));
			Chat.Broadcast(string.Concat(new string[]
			{
				"Kickbanning ",
				player.displayName,
				text,
				" (",
				@string,
				")"
			}), "SERVER", "#eee", 0UL);
			Net.sv.Kick(player.net.connection, "Banned" + text + ": " + @string, false);
		}

		// Token: 0x06003F05 RID: 16133 RVA: 0x00172860 File Offset: 0x00170A60
		[ServerVar]
		public static void moderatorid(ConsoleSystem.Arg arg)
		{
			ulong @uint = arg.GetUInt64(0, 0UL);
			string @string = arg.GetString(1, "unnamed");
			string string2 = arg.GetString(2, "no reason");
			if (@uint < 70000000000000000UL)
			{
				arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + @uint);
				return;
			}
			ServerUsers.User user = ServerUsers.Get(@uint);
			if (user != null && user.group == ServerUsers.UserGroup.Moderator)
			{
				arg.ReplyWith("User " + @uint + " is already a Moderator");
				return;
			}
			ServerUsers.Set(@uint, ServerUsers.UserGroup.Moderator, @string, string2, -1L);
			global::BasePlayer basePlayer = global::BasePlayer.FindByID(@uint);
			if (basePlayer != null)
			{
				basePlayer.SetPlayerFlag(global::BasePlayer.PlayerFlags.IsAdmin, true);
				basePlayer.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
			arg.ReplyWith(string.Concat(new object[]
			{
				"Added moderator ",
				@string,
				", steamid ",
				@uint
			}));
		}

		// Token: 0x06003F06 RID: 16134 RVA: 0x00172940 File Offset: 0x00170B40
		[ServerVar]
		public static void ownerid(ConsoleSystem.Arg arg)
		{
			ulong @uint = arg.GetUInt64(0, 0UL);
			string @string = arg.GetString(1, "unnamed");
			string string2 = arg.GetString(2, "no reason");
			if (@uint < 70000000000000000UL)
			{
				arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + @uint);
				return;
			}
			if (arg.Connection != null && arg.Connection.authLevel < 2U)
			{
				arg.ReplyWith("Moderators cannot run ownerid");
				return;
			}
			ServerUsers.User user = ServerUsers.Get(@uint);
			if (user != null && user.group == ServerUsers.UserGroup.Owner)
			{
				arg.ReplyWith("User " + @uint + " is already an Owner");
				return;
			}
			ServerUsers.Set(@uint, ServerUsers.UserGroup.Owner, @string, string2, -1L);
			global::BasePlayer basePlayer = global::BasePlayer.FindByID(@uint);
			if (basePlayer != null)
			{
				basePlayer.SetPlayerFlag(global::BasePlayer.PlayerFlags.IsAdmin, true);
				basePlayer.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
			arg.ReplyWith(string.Concat(new object[]
			{
				"Added owner ",
				@string,
				", steamid ",
				@uint
			}));
		}

		// Token: 0x06003F07 RID: 16135 RVA: 0x00172A44 File Offset: 0x00170C44
		[ServerVar]
		public static void removemoderator(ConsoleSystem.Arg arg)
		{
			ulong @uint = arg.GetUInt64(0, 0UL);
			if (@uint < 70000000000000000UL)
			{
				arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + @uint);
				return;
			}
			ServerUsers.User user = ServerUsers.Get(@uint);
			if (user == null || user.group != ServerUsers.UserGroup.Moderator)
			{
				arg.ReplyWith("User " + @uint + " isn't a moderator");
				return;
			}
			ServerUsers.Remove(@uint);
			global::BasePlayer basePlayer = global::BasePlayer.FindByID(@uint);
			if (basePlayer != null)
			{
				basePlayer.SetPlayerFlag(global::BasePlayer.PlayerFlags.IsAdmin, false);
				basePlayer.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
			arg.ReplyWith("Removed Moderator: " + @uint);
		}

		// Token: 0x06003F08 RID: 16136 RVA: 0x00172AE8 File Offset: 0x00170CE8
		[ServerVar]
		public static void removeowner(ConsoleSystem.Arg arg)
		{
			ulong @uint = arg.GetUInt64(0, 0UL);
			if (@uint < 70000000000000000UL)
			{
				arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + @uint);
				return;
			}
			ServerUsers.User user = ServerUsers.Get(@uint);
			if (user == null || user.group != ServerUsers.UserGroup.Owner)
			{
				arg.ReplyWith("User " + @uint + " isn't an owner");
				return;
			}
			ServerUsers.Remove(@uint);
			global::BasePlayer basePlayer = global::BasePlayer.FindByID(@uint);
			if (basePlayer != null)
			{
				basePlayer.SetPlayerFlag(global::BasePlayer.PlayerFlags.IsAdmin, false);
				basePlayer.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
			arg.ReplyWith("Removed Owner: " + @uint);
		}

		// Token: 0x06003F09 RID: 16137 RVA: 0x00172B8C File Offset: 0x00170D8C
		[ServerVar(Help = "banid <steamid> <username> <reason> [optional duration]")]
		public static void banid(ConsoleSystem.Arg arg)
		{
			ulong @uint = arg.GetUInt64(0, 0UL);
			string text = arg.GetString(1, "unnamed");
			string @string = arg.GetString(2, "no reason");
			if (@uint < 70000000000000000UL)
			{
				arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + @uint);
				return;
			}
			ServerUsers.User user = ServerUsers.Get(@uint);
			if (user != null && user.group == ServerUsers.UserGroup.Banned)
			{
				arg.ReplyWith("User " + @uint + " is already banned");
				return;
			}
			long expiry;
			string text2;
			if (!Admin.TryGetBanExpiry(arg, 3, out expiry, out text2))
			{
				return;
			}
			string text3 = "";
			global::BasePlayer basePlayer = global::BasePlayer.FindByID(@uint);
			if (basePlayer != null && basePlayer.IsConnected)
			{
				text = basePlayer.displayName;
				if (basePlayer.IsConnected && basePlayer.net.connection.ownerid != 0UL && basePlayer.net.connection.ownerid != basePlayer.net.connection.userid)
				{
					text3 += string.Format(" and also banned ownerid {0}", basePlayer.net.connection.ownerid);
					ServerUsers.Set(basePlayer.net.connection.ownerid, ServerUsers.UserGroup.Banned, basePlayer.displayName, arg.GetString(1, string.Format("Family share owner of {0}", basePlayer.net.connection.userid)), expiry);
				}
				Chat.Broadcast(string.Concat(new string[]
				{
					"Kickbanning ",
					basePlayer.displayName,
					text2,
					" (",
					@string,
					")"
				}), "SERVER", "#eee", 0UL);
				Net.sv.Kick(basePlayer.net.connection, "Banned" + text2 + ": " + @string, false);
			}
			ServerUsers.Set(@uint, ServerUsers.UserGroup.Banned, text, @string, expiry);
			arg.ReplyWith(string.Format("Banned User{0}: {1} - \"{2}\" for \"{3}\"{4}", new object[]
			{
				text2,
				@uint,
				text,
				@string,
				text3
			}));
		}

		// Token: 0x06003F0A RID: 16138 RVA: 0x00172DB0 File Offset: 0x00170FB0
		private static bool TryGetBanExpiry(ConsoleSystem.Arg arg, int n, out long expiry, out string durationSuffix)
		{
			expiry = arg.GetTimestamp(n, -1L);
			durationSuffix = null;
			int num = Epoch.Current;
			if (expiry > 0L && expiry <= (long)num)
			{
				arg.ReplyWith("Expiry time is in the past");
				return false;
			}
			durationSuffix = ((expiry > 0L) ? (" for " + (expiry - (long)num).FormatSecondsLong()) : "");
			return true;
		}

		// Token: 0x06003F0B RID: 16139 RVA: 0x00172E10 File Offset: 0x00171010
		[ServerVar]
		public static void unban(ConsoleSystem.Arg arg)
		{
			ulong @uint = arg.GetUInt64(0, 0UL);
			if (@uint < 70000000000000000UL)
			{
				arg.ReplyWith(string.Format("This doesn't appear to be a 64bit steamid: {0}", @uint));
				return;
			}
			ServerUsers.User user = ServerUsers.Get(@uint);
			if (user == null || user.group != ServerUsers.UserGroup.Banned)
			{
				arg.ReplyWith(string.Format("User {0} isn't banned", @uint));
				return;
			}
			ServerUsers.Remove(@uint);
			arg.ReplyWith("Unbanned User: " + @uint);
		}

		// Token: 0x06003F0C RID: 16140 RVA: 0x00172E90 File Offset: 0x00171090
		[ServerVar]
		public static void skipqueue(ConsoleSystem.Arg arg)
		{
			ulong @uint = arg.GetUInt64(0, 0UL);
			if (@uint < 70000000000000000UL)
			{
				arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + @uint);
				return;
			}
			SingletonComponent<ServerMgr>.Instance.connectionQueue.SkipQueue(@uint);
		}

		// Token: 0x06003F0D RID: 16141 RVA: 0x00172EDC File Offset: 0x001710DC
		[ServerVar(Help = "Adds skip queue permissions to a SteamID")]
		public static void skipqueueid(ConsoleSystem.Arg arg)
		{
			ulong @uint = arg.GetUInt64(0, 0UL);
			string @string = arg.GetString(1, "unnamed");
			string string2 = arg.GetString(2, "no reason");
			if (@uint < 70000000000000000UL)
			{
				arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + @uint);
				return;
			}
			ServerUsers.User user = ServerUsers.Get(@uint);
			if (user != null && (user.group == ServerUsers.UserGroup.Owner || user.group == ServerUsers.UserGroup.Moderator || user.group == ServerUsers.UserGroup.SkipQueue))
			{
				arg.ReplyWith(string.Format("User {0} will already skip the queue ({1})", @uint, user.group));
				return;
			}
			if (user != null && user.group == ServerUsers.UserGroup.Banned)
			{
				arg.ReplyWith(string.Format("User {0} is banned", @uint));
				return;
			}
			ServerUsers.Set(@uint, ServerUsers.UserGroup.SkipQueue, @string, string2, -1L);
			arg.ReplyWith(string.Format("Added skip queue permission for {0} ({1})", @string, @uint));
		}

		// Token: 0x06003F0E RID: 16142 RVA: 0x00172FBC File Offset: 0x001711BC
		[ServerVar(Help = "Removes skip queue permission from a SteamID")]
		public static void removeskipqueue(ConsoleSystem.Arg arg)
		{
			ulong @uint = arg.GetUInt64(0, 0UL);
			if (@uint < 70000000000000000UL)
			{
				arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + @uint);
				return;
			}
			ServerUsers.User user = ServerUsers.Get(@uint);
			if (user != null && (user.group == ServerUsers.UserGroup.Owner || user.group == ServerUsers.UserGroup.Moderator))
			{
				arg.ReplyWith(string.Format("User is a {0}, cannot remove skip queue permission with this command", user.group));
				return;
			}
			if (user == null || user.group != ServerUsers.UserGroup.SkipQueue)
			{
				arg.ReplyWith("User does not have skip queue permission");
				return;
			}
			ServerUsers.Remove(@uint);
			arg.ReplyWith("Removed skip queue permission: " + @uint);
		}

		// Token: 0x06003F0F RID: 16143 RVA: 0x00173064 File Offset: 0x00171264
		[ServerVar(Help = "Print out currently connected clients etc")]
		public static void players(ConsoleSystem.Arg arg)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumn("id");
			textTable.AddColumn("name");
			textTable.AddColumn("ping");
			textTable.AddColumn("snap");
			textTable.AddColumn("updt");
			textTable.AddColumn("posi");
			textTable.AddColumn("dist");
			foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
			{
				string userIDString = basePlayer.UserIDString;
				string text = basePlayer.displayName.ToString();
				if (text.Length >= 14)
				{
					text = text.Substring(0, 14) + "..";
				}
				string text2 = text;
				string text3 = Net.sv.GetAveragePing(basePlayer.net.connection).ToString();
				string text4 = basePlayer.GetQueuedUpdateCount(global::BasePlayer.NetworkQueue.Update).ToString();
				string text5 = basePlayer.GetQueuedUpdateCount(global::BasePlayer.NetworkQueue.UpdateDistance).ToString();
				textTable.AddRow(new string[]
				{
					userIDString,
					text2,
					text3,
					string.Empty,
					text4,
					string.Empty,
					text5
				});
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x06003F10 RID: 16144 RVA: 0x001731DC File Offset: 0x001713DC
		[ServerVar(Help = "Sends a message in chat")]
		public static void say(ConsoleSystem.Arg arg)
		{
			Chat.Broadcast(arg.FullString, "SERVER", "#eee", 0UL);
		}

		// Token: 0x06003F11 RID: 16145 RVA: 0x001731F8 File Offset: 0x001713F8
		[ServerVar(Help = "Show user info for players on server.")]
		public static void users(ConsoleSystem.Arg arg)
		{
			string text = "<slot:userid:\"name\">\n";
			int num = 0;
			foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
			{
				text = string.Concat(new object[]
				{
					text,
					basePlayer.userID,
					":\"",
					basePlayer.displayName,
					"\"\n"
				});
				num++;
			}
			text = text + num.ToString() + "users\n";
			arg.ReplyWith(text);
		}

		// Token: 0x06003F12 RID: 16146 RVA: 0x001732A0 File Offset: 0x001714A0
		[ServerVar(Help = "Show user info for players on server.")]
		public static void sleepingusers(ConsoleSystem.Arg arg)
		{
			string text = "<slot:userid:\"name\">\n";
			int num = 0;
			foreach (global::BasePlayer basePlayer in global::BasePlayer.sleepingPlayerList)
			{
				text += string.Format("{0}:{1}\n", basePlayer.userID, basePlayer.displayName);
				num++;
			}
			text += string.Format("{0} sleeping users\n", num);
			arg.ReplyWith(text);
		}

		// Token: 0x06003F13 RID: 16147 RVA: 0x00173338 File Offset: 0x00171538
		[ServerVar(Help = "Show user info for sleeping players on server in range of the player.")]
		public static void sleepingusersinrange(ConsoleSystem.Arg arg)
		{
			global::BasePlayer fromPlayer = arg.Player();
			if (fromPlayer == null)
			{
				return;
			}
			float range = arg.GetFloat(0, 0f);
			string text = "<slot:userid:\"name\">\n";
			int num = 0;
			List<global::BasePlayer> list = Pool.GetList<global::BasePlayer>();
			foreach (global::BasePlayer item in global::BasePlayer.sleepingPlayerList)
			{
				list.Add(item);
			}
			list.RemoveAll((global::BasePlayer p) => p.Distance2D(fromPlayer) > range);
			list.Sort(delegate(global::BasePlayer player, global::BasePlayer basePlayer)
			{
				if (player.Distance2D(fromPlayer) >= basePlayer.Distance2D(fromPlayer))
				{
					return 1;
				}
				return -1;
			});
			foreach (global::BasePlayer basePlayer2 in list)
			{
				text += string.Format("{0}:{1}:{2}m\n", basePlayer2.userID, basePlayer2.displayName, basePlayer2.Distance2D(fromPlayer));
				num++;
			}
			Pool.FreeList<global::BasePlayer>(ref list);
			text += string.Format("{0} sleeping users within {1}m\n", num, range);
			arg.ReplyWith(text);
		}

		// Token: 0x06003F14 RID: 16148 RVA: 0x00173498 File Offset: 0x00171698
		[ServerVar(Help = "Show user info for players on server in range of the player.")]
		public static void usersinrange(ConsoleSystem.Arg arg)
		{
			global::BasePlayer fromPlayer = arg.Player();
			if (fromPlayer == null)
			{
				return;
			}
			float range = arg.GetFloat(0, 0f);
			string text = "<slot:userid:\"name\">\n";
			int num = 0;
			List<global::BasePlayer> list = Pool.GetList<global::BasePlayer>();
			foreach (global::BasePlayer item in global::BasePlayer.activePlayerList)
			{
				list.Add(item);
			}
			list.RemoveAll((global::BasePlayer p) => p.Distance2D(fromPlayer) > range);
			list.Sort(delegate(global::BasePlayer player, global::BasePlayer basePlayer)
			{
				if (player.Distance2D(fromPlayer) >= basePlayer.Distance2D(fromPlayer))
				{
					return 1;
				}
				return -1;
			});
			foreach (global::BasePlayer basePlayer2 in list)
			{
				text += string.Format("{0}:{1}:{2}m\n", basePlayer2.userID, basePlayer2.displayName, basePlayer2.Distance2D(fromPlayer));
				num++;
			}
			Pool.FreeList<global::BasePlayer>(ref list);
			text += string.Format("{0} users within {1}m\n", num, range);
			arg.ReplyWith(text);
		}

		// Token: 0x06003F15 RID: 16149 RVA: 0x001735F8 File Offset: 0x001717F8
		[ServerVar(Help = "Show user info for players on server in range of the supplied player (eg. Jim 50)")]
		public static void usersinrangeofplayer(ConsoleSystem.Arg arg)
		{
			global::BasePlayer targetPlayer = arg.GetPlayerOrSleeper(0);
			if (targetPlayer == null)
			{
				return;
			}
			float range = arg.GetFloat(1, 0f);
			string text = "<slot:userid:\"name\">\n";
			int num = 0;
			List<global::BasePlayer> list = Pool.GetList<global::BasePlayer>();
			foreach (global::BasePlayer item in global::BasePlayer.activePlayerList)
			{
				list.Add(item);
			}
			list.RemoveAll((global::BasePlayer p) => p.Distance2D(targetPlayer) > range);
			list.Sort(delegate(global::BasePlayer player, global::BasePlayer basePlayer)
			{
				if (player.Distance2D(targetPlayer) >= basePlayer.Distance2D(targetPlayer))
				{
					return 1;
				}
				return -1;
			});
			foreach (global::BasePlayer basePlayer2 in list)
			{
				text += string.Format("{0}:{1}:{2}m\n", basePlayer2.userID, basePlayer2.displayName, basePlayer2.Distance2D(targetPlayer));
				num++;
			}
			Pool.FreeList<global::BasePlayer>(ref list);
			text += string.Format("{0} users within {1}m of {2}\n", num, range, targetPlayer.displayName);
			arg.ReplyWith(text);
		}

		// Token: 0x06003F16 RID: 16150 RVA: 0x00173764 File Offset: 0x00171964
		[ServerVar(Help = "List of banned users (sourceds compat)")]
		public static void banlist(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(ServerUsers.BanListString(false));
		}

		// Token: 0x06003F17 RID: 16151 RVA: 0x00173772 File Offset: 0x00171972
		[ServerVar(Help = "List of banned users - shows reasons and usernames")]
		public static void banlistex(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(ServerUsers.BanListStringEx());
		}

		// Token: 0x06003F18 RID: 16152 RVA: 0x0017377F File Offset: 0x0017197F
		[ServerVar(Help = "List of banned users, by ID (sourceds compat)")]
		public static void listid(ConsoleSystem.Arg arg)
		{
			arg.ReplyWith(ServerUsers.BanListString(true));
		}

		// Token: 0x06003F19 RID: 16153 RVA: 0x00173790 File Offset: 0x00171990
		[ServerVar]
		public static void mute(ConsoleSystem.Arg arg)
		{
			global::BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			if (!playerOrSleeper || playerOrSleeper.net == null || playerOrSleeper.net.connection == null)
			{
				arg.ReplyWith("Player not found");
				return;
			}
			playerOrSleeper.SetPlayerFlag(global::BasePlayer.PlayerFlags.ChatMute, true);
		}

		// Token: 0x06003F1A RID: 16154 RVA: 0x001737DC File Offset: 0x001719DC
		[ServerVar]
		public static void unmute(ConsoleSystem.Arg arg)
		{
			global::BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			if (!playerOrSleeper || playerOrSleeper.net == null || playerOrSleeper.net.connection == null)
			{
				arg.ReplyWith("Player not found");
				return;
			}
			playerOrSleeper.SetPlayerFlag(global::BasePlayer.PlayerFlags.ChatMute, false);
		}

		// Token: 0x06003F1B RID: 16155 RVA: 0x00173828 File Offset: 0x00171A28
		[ServerVar(Help = "Print a list of currently muted players")]
		public static void mutelist(ConsoleSystem.Arg arg)
		{
			var obj = from x in global::BasePlayer.allPlayerList
			where x.HasPlayerFlag(global::BasePlayer.PlayerFlags.ChatMute)
			select new
			{
				SteamId = x.UserIDString,
				Name = x.displayName
			};
			arg.ReplyWith(obj);
		}

		// Token: 0x06003F1C RID: 16156 RVA: 0x0017388C File Offset: 0x00171A8C
		[ServerVar]
		public static void clientperf(ConsoleSystem.Arg arg)
		{
			string @string = arg.GetString(0, "legacy");
			int @int = arg.GetInt(1, UnityEngine.Random.Range(int.MinValue, int.MaxValue));
			foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
			{
				basePlayer.ClientRPCPlayer<string, int>(null, basePlayer, "GetPerformanceReport", @string, @int);
			}
		}

		// Token: 0x06003F1D RID: 16157 RVA: 0x0017390C File Offset: 0x00171B0C
		[ServerVar]
		public static void clientperf_frametime(ConsoleSystem.Arg arg)
		{
			ClientFrametimeRequest value = new ClientFrametimeRequest
			{
				request_id = arg.GetInt(0, UnityEngine.Random.Range(int.MinValue, int.MaxValue)),
				start_frame = arg.GetInt(1, 0),
				max_frames = arg.GetInt(2, 1000)
			};
			foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
			{
				basePlayer.ClientRPCPlayer<string>(null, basePlayer, "GetPerformanceReport_Frametime", JsonConvert.SerializeObject(value));
			}
		}

		// Token: 0x06003F1E RID: 16158 RVA: 0x001739AC File Offset: 0x00171BAC
		[ServerVar(Help = "Get information about all the cars in the world")]
		public static void carstats(ConsoleSystem.Arg arg)
		{
			HashSet<global::ModularCar> allCarsList = global::ModularCar.allCarsList;
			TextTable textTable = new TextTable();
			textTable.AddColumn("id");
			textTable.AddColumn("sockets");
			textTable.AddColumn("modules");
			textTable.AddColumn("complete");
			textTable.AddColumn("engine");
			textTable.AddColumn("health");
			textTable.AddColumn("location");
			int count = allCarsList.Count;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (global::ModularCar modularCar in allCarsList)
			{
				string text = modularCar.net.ID.ToString();
				string text2 = modularCar.TotalSockets.ToString();
				string text3 = modularCar.NumAttachedModules.ToString();
				string text4;
				if (modularCar.IsComplete())
				{
					text4 = "Complete";
					num++;
				}
				else
				{
					text4 = "Partial";
				}
				string text5;
				if (modularCar.HasAnyWorkingEngines())
				{
					text5 = "Working";
					num2++;
				}
				else
				{
					text5 = "Broken";
				}
				string text6;
				if (modularCar.TotalMaxHealth() == 0f)
				{
					text6 = "0";
				}
				else
				{
					text6 = string.Format("{0:0%}", modularCar.TotalHealth() / modularCar.TotalMaxHealth());
				}
				string text7;
				if (modularCar.IsOutside())
				{
					text7 = "Outside";
				}
				else
				{
					text7 = "Inside";
					num3++;
				}
				textTable.AddRow(new string[]
				{
					text,
					text2,
					text3,
					text4,
					text5,
					text6,
					text7
				});
			}
			string text8 = "";
			if (count == 1)
			{
				text8 += "\nThe world contains 1 modular car.";
			}
			else
			{
				text8 += string.Format("\nThe world contains {0} modular cars.", count);
			}
			if (num == 1)
			{
				text8 += string.Format("\n1 ({0:0%}) is in a completed state.", 1f / (float)count);
			}
			else
			{
				text8 += string.Format("\n{0} ({1:0%}) are in a completed state.", num, (float)num / (float)count);
			}
			if (num2 == 1)
			{
				text8 += string.Format("\n1 ({0:0%}) is driveable.", 1f / (float)count);
			}
			else
			{
				text8 += string.Format("\n{0} ({1:0%}) are driveable.", num2, (float)num2 / (float)count);
			}
			if (num3 == 1)
			{
				text8 += string.Format("\n1 ({0:0%}) is sheltered indoors.", 1f / (float)count);
			}
			else
			{
				text8 += string.Format("\n{0} ({1:0%}) are sheltered indoors.", num3, (float)num3 / (float)count);
			}
			arg.ReplyWith(textTable.ToString() + text8);
		}

		// Token: 0x06003F1F RID: 16159 RVA: 0x00173C88 File Offset: 0x00171E88
		[ServerVar]
		public static string teaminfo(ConsoleSystem.Arg arg)
		{
			ulong num = arg.GetUInt64(0, 0UL);
			if (num == 0UL)
			{
				global::BasePlayer player = arg.GetPlayer(0);
				if (player == null)
				{
					return "Player not found";
				}
				num = player.userID;
			}
			global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindPlayersTeam(num);
			if (playerTeam == null)
			{
				return "Player is not in a team";
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("steamID");
			textTable.AddColumn("username");
			textTable.AddColumn("online");
			textTable.AddColumn("leader");
			using (List<ulong>.Enumerator enumerator = playerTeam.members.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ulong memberId = enumerator.Current;
					bool flag = Net.sv.connections.FirstOrDefault((Connection c) => c.connected && c.userid == memberId) != null;
					textTable.AddRow(new string[]
					{
						memberId.ToString(),
						Admin.GetPlayerName(memberId),
						flag ? "x" : "",
						(memberId == playerTeam.teamLeader) ? "x" : ""
					});
				}
			}
			if (!arg.HasArg("--json"))
			{
				return textTable.ToString();
			}
			return textTable.ToJson();
		}

		// Token: 0x06003F20 RID: 16160 RVA: 0x00173DF0 File Offset: 0x00171FF0
		[ServerVar]
		public static void entid(ConsoleSystem.Arg arg)
		{
			global::BaseEntity baseEntity = global::BaseNetworkable.serverEntities.Find(arg.GetUInt(1, 0U)) as global::BaseEntity;
			if (baseEntity == null)
			{
				return;
			}
			if (baseEntity is global::BasePlayer)
			{
				return;
			}
			string @string = arg.GetString(0, "");
			if (arg.Player() != null)
			{
				Debug.Log(string.Concat(new object[]
				{
					"[ENTCMD] ",
					arg.Player().displayName,
					"/",
					arg.Player().userID,
					" used *",
					@string,
					"* on ent: ",
					baseEntity.name
				}));
			}
			uint num = <PrivateImplementationDetails>.ComputeStringHash(@string);
			if (num <= 2152183181U)
			{
				if (num <= 720644751U)
				{
					if (num != 693242804U)
					{
						if (num == 720644751U)
						{
							if (@string == "who")
							{
								arg.ReplyWith(baseEntity.Admin_Who());
								return;
							}
						}
					}
					else if (@string == "repair")
					{
						Admin.RunInRadius<BaseCombatEntity>(arg.GetFloat(2, 0f), baseEntity, delegate(BaseCombatEntity entity)
						{
							if (entity.repair.enabled)
							{
								entity.SetHealth(entity.MaxHealth());
							}
						}, null);
					}
				}
				else if (num != 1449533269U)
				{
					if (num != 1483009432U)
					{
						if (num == 2152183181U)
						{
							if (@string == "undebug")
							{
								baseEntity.SetFlag(global::BaseEntity.Flags.Debugging, false, false, true);
								return;
							}
						}
					}
					else if (@string == "debug")
					{
						baseEntity.SetFlag(global::BaseEntity.Flags.Debugging, true, false, true);
						return;
					}
				}
				else if (@string == "unlock")
				{
					baseEntity.SetFlag(global::BaseEntity.Flags.Locked, false, false, true);
					return;
				}
			}
			else if (num <= 3306112409U)
			{
				if (num != 2382367150U)
				{
					if (num != 2503977039U)
					{
						if (num == 3306112409U)
						{
							if (@string == "kill")
							{
								baseEntity.AdminKill();
								return;
							}
						}
					}
					else if (@string == "auth")
					{
						arg.ReplyWith(Admin.AuthList(baseEntity));
						return;
					}
				}
				else if (@string == "setgrade")
				{
					arg.ReplyWith(Admin.ChangeGrade(baseEntity, 0, 0, (BuildingGrade.Enum)arg.GetInt(2, 0), arg.GetFloat(3, 0f)));
					return;
				}
			}
			else if (num != 3700935799U)
			{
				if (num != 3846680516U)
				{
					if (num == 4010637378U)
					{
						if (@string == "lock")
						{
							baseEntity.SetFlag(global::BaseEntity.Flags.Locked, true, false, true);
							return;
						}
					}
				}
				else if (@string == "downgrade")
				{
					arg.ReplyWith(Admin.ChangeGrade(baseEntity, 0, arg.GetInt(2, 1), BuildingGrade.Enum.None, arg.GetFloat(3, 0f)));
					return;
				}
			}
			else if (@string == "upgrade")
			{
				arg.ReplyWith(Admin.ChangeGrade(baseEntity, arg.GetInt(2, 1), 0, BuildingGrade.Enum.None, arg.GetFloat(3, 0f)));
				return;
			}
			arg.ReplyWith("Unknown command");
		}

		// Token: 0x06003F21 RID: 16161 RVA: 0x00174128 File Offset: 0x00172328
		private static string AuthList(global::BaseEntity ent)
		{
			if (ent != null)
			{
				BuildingPrivlidge buildingPrivlidge;
				List<PlayerNameID> authorizedPlayers;
				if ((buildingPrivlidge = (ent as BuildingPrivlidge)) == null)
				{
					global::AutoTurret autoTurret;
					if ((autoTurret = (ent as global::AutoTurret)) == null)
					{
						global::CodeLock codeLock;
						if ((codeLock = (ent as global::CodeLock)) != null)
						{
							return Admin.CodeLockAuthList(codeLock);
						}
						BaseVehicleModule vehicleModule;
						if ((vehicleModule = (ent as BaseVehicleModule)) != null)
						{
							return Admin.CodeLockAuthList(vehicleModule);
						}
						goto IL_55;
					}
					else
					{
						authorizedPlayers = autoTurret.authorizedPlayers;
					}
				}
				else
				{
					authorizedPlayers = buildingPrivlidge.authorizedPlayers;
				}
				if (authorizedPlayers == null || authorizedPlayers.Count == 0)
				{
					return "Nobody is authed to this entity";
				}
				TextTable textTable = new TextTable();
				textTable.AddColumn("steamID");
				textTable.AddColumn("username");
				foreach (PlayerNameID playerNameID in authorizedPlayers)
				{
					textTable.AddRow(new string[]
					{
						playerNameID.userid.ToString(),
						Admin.GetPlayerName(playerNameID.userid)
					});
				}
				return textTable.ToString();
			}
			IL_55:
			return "Entity has no auth list";
		}

		// Token: 0x06003F22 RID: 16162 RVA: 0x0017422C File Offset: 0x0017242C
		private static string CodeLockAuthList(global::CodeLock codeLock)
		{
			if (codeLock.whitelistPlayers.Count == 0 && codeLock.guestPlayers.Count == 0)
			{
				return "Nobody is authed to this entity";
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("steamID");
			textTable.AddColumn("username");
			textTable.AddColumn("isGuest");
			foreach (ulong steamId in codeLock.whitelistPlayers)
			{
				textTable.AddRow(new string[]
				{
					steamId.ToString(),
					Admin.GetPlayerName(steamId),
					""
				});
			}
			foreach (ulong steamId2 in codeLock.guestPlayers)
			{
				textTable.AddRow(new string[]
				{
					steamId2.ToString(),
					Admin.GetPlayerName(steamId2),
					"x"
				});
			}
			return textTable.ToString();
		}

		// Token: 0x06003F23 RID: 16163 RVA: 0x00174350 File Offset: 0x00172550
		private static string CodeLockAuthList(BaseVehicleModule vehicleModule)
		{
			if (!vehicleModule.IsOnAVehicle)
			{
				return "Nobody is authed to this entity";
			}
			global::ModularCar modularCar = vehicleModule.Vehicle as global::ModularCar;
			if (modularCar == null || !modularCar.IsLockable || modularCar.CarLock.WhitelistPlayers.Count == 0)
			{
				return "Nobody is authed to this entity";
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("steamID");
			textTable.AddColumn("username");
			foreach (ulong steamId in modularCar.CarLock.WhitelistPlayers)
			{
				textTable.AddRow(new string[]
				{
					steamId.ToString(),
					Admin.GetPlayerName(steamId)
				});
			}
			return textTable.ToString();
		}

		// Token: 0x06003F24 RID: 16164 RVA: 0x00174428 File Offset: 0x00172628
		public static string GetPlayerName(ulong steamId)
		{
			global::BasePlayer basePlayer = global::BasePlayer.allPlayerList.FirstOrDefault((global::BasePlayer p) => p.userID == steamId);
			string result;
			if (!(basePlayer != null))
			{
				if ((result = SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(steamId)) == null)
				{
					return "[unknown]";
				}
			}
			else
			{
				result = basePlayer.displayName;
			}
			return result;
		}

		// Token: 0x06003F25 RID: 16165 RVA: 0x00174488 File Offset: 0x00172688
		public static string ChangeGrade(global::BaseEntity entity, int increaseBy = 0, int decreaseBy = 0, BuildingGrade.Enum targetGrade = BuildingGrade.Enum.None, float radius = 0f)
		{
			if (entity as global::BuildingBlock == null)
			{
				return string.Format("'{0}' is not a building block", entity);
			}
			Admin.RunInRadius<global::BuildingBlock>(radius, entity, delegate(global::BuildingBlock block)
			{
				BuildingGrade.Enum @enum = block.grade;
				if (targetGrade > BuildingGrade.Enum.None && targetGrade < BuildingGrade.Enum.Count)
				{
					@enum = targetGrade;
				}
				else
				{
					@enum = (BuildingGrade.Enum)Mathf.Min((int)(@enum + increaseBy), 4);
					@enum = (BuildingGrade.Enum)Mathf.Max(@enum - (BuildingGrade.Enum)decreaseBy, 0);
				}
				if (@enum != block.grade)
				{
					block.ChangeGrade(@enum, false);
				}
			}, null);
			int count = Pool.GetList<global::BuildingBlock>().Count;
			return string.Format("Upgraded/downgraded '{0}' building block(s)", count);
		}

		// Token: 0x06003F26 RID: 16166 RVA: 0x001744FC File Offset: 0x001726FC
		private static bool RunInRadius<T>(float radius, global::BaseEntity initial, Action<T> callback, Func<T, bool> filter = null) where T : global::BaseEntity
		{
			List<T> list = Pool.GetList<T>();
			radius = Mathf.Clamp(radius, 0f, 200f);
			T item;
			if (radius > 0f)
			{
				Vis.Entities<T>(initial.transform.position, radius, list, 2097152, QueryTriggerInteraction.Collide);
			}
			else if ((item = (initial as T)) != null)
			{
				list.Add(item);
			}
			foreach (T obj in list)
			{
				try
				{
					callback(obj);
				}
				catch (Exception arg)
				{
					Debug.LogError(string.Format("Exception while running callback in radius: {0}", arg));
					return false;
				}
			}
			return true;
		}

		// Token: 0x06003F27 RID: 16167 RVA: 0x001745CC File Offset: 0x001727CC
		[ServerVar(Help = "Get a list of players")]
		public static Admin.PlayerInfo[] playerlist()
		{
			return (from x in global::BasePlayer.activePlayerList
			select new Admin.PlayerInfo
			{
				SteamID = x.UserIDString,
				OwnerSteamID = x.OwnerID.ToString(),
				DisplayName = x.displayName,
				Ping = Net.sv.GetAveragePing(x.net.connection),
				Address = x.net.connection.ipaddress,
				ConnectedSeconds = (int)x.net.connection.GetSecondsConnected(),
				VoiationLevel = x.violationLevel,
				Health = x.Health()
			}).ToArray<Admin.PlayerInfo>();
		}

		// Token: 0x06003F28 RID: 16168 RVA: 0x001745FC File Offset: 0x001727FC
		[ServerVar(Help = "List of banned users")]
		public static ServerUsers.User[] Bans()
		{
			return ServerUsers.GetAll(ServerUsers.UserGroup.Banned).ToArray<ServerUsers.User>();
		}

		// Token: 0x06003F29 RID: 16169 RVA: 0x0017460C File Offset: 0x0017280C
		[ServerVar(Help = "Get a list of information about the server")]
		public static Admin.ServerInfoOutput ServerInfo()
		{
			return new Admin.ServerInfoOutput
			{
				Hostname = ConVar.Server.hostname,
				MaxPlayers = ConVar.Server.maxplayers,
				Players = global::BasePlayer.activePlayerList.Count,
				Queued = SingletonComponent<ServerMgr>.Instance.connectionQueue.Queued,
				Joining = SingletonComponent<ServerMgr>.Instance.connectionQueue.Joining,
				EntityCount = global::BaseNetworkable.serverEntities.Count,
				GameTime = ((TOD_Sky.Instance != null) ? TOD_Sky.Instance.Cycle.DateTime.ToString() : DateTime.UtcNow.ToString()),
				Uptime = (int)Time.realtimeSinceStartup,
				Map = ConVar.Server.level,
				Framerate = (float)global::Performance.report.frameRate,
				Memory = (int)global::Performance.report.memoryAllocations,
				Collections = (int)global::Performance.report.memoryCollections,
				NetworkIn = ((Net.sv == null) ? 0 : ((int)Net.sv.GetStat(null, BaseNetwork.StatTypeLong.BytesReceived_LastSecond))),
				NetworkOut = ((Net.sv == null) ? 0 : ((int)Net.sv.GetStat(null, BaseNetwork.StatTypeLong.BytesSent_LastSecond))),
				Restarting = SingletonComponent<ServerMgr>.Instance.Restarting,
				SaveCreatedTime = SaveRestore.SaveCreatedTime.ToString(),
				Version = 2370,
				Protocol = Protocol.printable
			};
		}

		// Token: 0x06003F2A RID: 16170 RVA: 0x00174787 File Offset: 0x00172987
		[ServerVar(Help = "Get information about this build")]
		public static BuildInfo BuildInfo()
		{
			return Facepunch.BuildInfo.Current;
		}

		// Token: 0x06003F2B RID: 16171 RVA: 0x0017478E File Offset: 0x0017298E
		[ServerVar]
		public static void AdminUI_FullRefresh(ConsoleSystem.Arg arg)
		{
			Admin.AdminUI_RequestPlayerList(arg);
			Admin.AdminUI_RequestServerInfo(arg);
			Admin.AdminUI_RequestServerConvars(arg);
			Admin.AdminUI_RequestUGCList(arg);
		}

		// Token: 0x06003F2C RID: 16172 RVA: 0x001747A8 File Offset: 0x001729A8
		[ServerVar]
		public static void AdminUI_RequestPlayerList(ConsoleSystem.Arg arg)
		{
			if (!Admin.allowAdminUI)
			{
				return;
			}
			ConsoleNetwork.SendClientCommand(arg.Connection, "AdminUI_ReceivePlayerList", new object[]
			{
				JsonConvert.SerializeObject(Admin.playerlist())
			});
		}

		// Token: 0x06003F2D RID: 16173 RVA: 0x001747D5 File Offset: 0x001729D5
		[ServerVar]
		public static void AdminUI_RequestServerInfo(ConsoleSystem.Arg arg)
		{
			if (!Admin.allowAdminUI)
			{
				return;
			}
			ConsoleNetwork.SendClientCommand(arg.Connection, "AdminUI_ReceiveServerInfo", new object[]
			{
				JsonConvert.SerializeObject(Admin.ServerInfo())
			});
		}

		// Token: 0x06003F2E RID: 16174 RVA: 0x00174808 File Offset: 0x00172A08
		[ServerVar]
		public static void AdminUI_RequestServerConvars(ConsoleSystem.Arg arg)
		{
			if (!Admin.allowAdminUI)
			{
				return;
			}
			List<Admin.ServerConvarInfo> list = Pool.GetList<Admin.ServerConvarInfo>();
			foreach (ConsoleSystem.Command command in ConsoleSystem.Index.All)
			{
				if (command.Server && command.Variable && command.ServerAdmin && command.ShowInAdminUI)
				{
					List<Admin.ServerConvarInfo> list2 = list;
					Admin.ServerConvarInfo item = default(Admin.ServerConvarInfo);
					item.FullName = command.FullName;
					Func<string> getOveride = command.GetOveride;
					item.Value = ((getOveride != null) ? getOveride() : null);
					item.Help = command.Description;
					list2.Add(item);
				}
			}
			ConsoleNetwork.SendClientCommand(arg.Connection, "AdminUI_ReceiveCommands", new object[]
			{
				JsonConvert.SerializeObject(list)
			});
			Pool.FreeList<Admin.ServerConvarInfo>(ref list);
		}

		// Token: 0x06003F2F RID: 16175 RVA: 0x001748C4 File Offset: 0x00172AC4
		[ServerVar]
		public static void AdminUI_RequestUGCList(ConsoleSystem.Arg arg)
		{
			if (!Admin.allowAdminUI)
			{
				return;
			}
			List<Admin.ServerUGCInfo> list = Pool.GetList<Admin.ServerUGCInfo>();
			foreach (global::BaseNetworkable baseNetworkable in global::BaseNetworkable.serverEntities)
			{
				uint[] array = null;
				ulong[] playerIds = null;
				UGCType ugctype = UGCType.ImageJpg;
				IUGCBrowserEntity iugcbrowserEntity;
				if (baseNetworkable.TryGetComponent<IUGCBrowserEntity>(out iugcbrowserEntity))
				{
					array = iugcbrowserEntity.GetContentCRCs;
					playerIds = iugcbrowserEntity.EditingHistory.ToArray();
					ugctype = iugcbrowserEntity.ContentType;
				}
				if (array != null && array.Length != 0)
				{
					bool flag = false;
					uint[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						if (array2[i] != 0U)
						{
							flag = true;
							break;
						}
					}
					if (ugctype == UGCType.PatternBoomer)
					{
						flag = true;
					}
					if (flag)
					{
						list.Add(new Admin.ServerUGCInfo
						{
							entityId = baseNetworkable.net.ID,
							crcs = array,
							contentType = ugctype,
							entityPrefabID = baseNetworkable.prefabID,
							shortPrefabName = baseNetworkable.ShortPrefabName,
							playerIds = playerIds
						});
					}
				}
			}
			ConsoleNetwork.SendClientCommand(arg.Connection, "AdminUI_ReceiveUGCList", new object[]
			{
				JsonConvert.SerializeObject(list)
			});
			Pool.FreeList<Admin.ServerUGCInfo>(ref list);
		}

		// Token: 0x06003F30 RID: 16176 RVA: 0x00174A10 File Offset: 0x00172C10
		[ServerVar]
		public static void AdminUI_RequestUGCContent(ConsoleSystem.Arg arg)
		{
			if (!Admin.allowAdminUI || arg.Player() == null)
			{
				return;
			}
			uint @uint = arg.GetUInt(0, 0U);
			uint uint2 = arg.GetUInt(1, 0U);
			FileStorage.Type @int = (FileStorage.Type)arg.GetInt(2, 0);
			uint uint3 = arg.GetUInt(3, 0U);
			byte[] array = FileStorage.server.Get(@uint, @int, uint2, uint3);
			if (array == null)
			{
				return;
			}
			SendInfo sendInfo = new SendInfo(arg.Connection)
			{
				channel = 2,
				method = SendMethod.Reliable
			};
			arg.Player().ClientRPCEx<uint, uint, byte[], uint, byte>(sendInfo, null, "AdminReceivedUGC", @uint, (uint)array.Length, array, uint3, (byte)@int);
		}

		// Token: 0x06003F31 RID: 16177 RVA: 0x00174AAC File Offset: 0x00172CAC
		[ServerVar]
		public static void AdminUI_DeleteUGCContent(ConsoleSystem.Arg arg)
		{
			if (!Admin.allowAdminUI)
			{
				return;
			}
			uint @uint = arg.GetUInt(0, 0U);
			global::BaseNetworkable baseNetworkable = global::BaseNetworkable.serverEntities.Find(@uint);
			if (baseNetworkable != null)
			{
				FileStorage.server.RemoveAllByEntity(@uint);
				IUGCBrowserEntity iugcbrowserEntity;
				if (baseNetworkable.TryGetComponent<IUGCBrowserEntity>(out iugcbrowserEntity))
				{
					iugcbrowserEntity.ClearContent();
				}
			}
		}

		// Token: 0x06003F32 RID: 16178 RVA: 0x00174AFC File Offset: 0x00172CFC
		[ServerVar]
		public static void AdminUI_RequestFireworkPattern(ConsoleSystem.Arg arg)
		{
			if (!Admin.allowAdminUI)
			{
				return;
			}
			uint @uint = arg.GetUInt(0, 0U);
			global::BaseNetworkable baseNetworkable = global::BaseNetworkable.serverEntities.Find(@uint);
			global::PatternFirework patternFirework;
			if (baseNetworkable != null && (patternFirework = (baseNetworkable as global::PatternFirework)) != null)
			{
				SendInfo sendInfo = new SendInfo(arg.Connection)
				{
					channel = 2,
					method = SendMethod.Reliable
				};
				arg.Player().ClientRPCEx<uint, byte[]>(sendInfo, null, "AdminReceivedPatternFirework", @uint, patternFirework.Design.ToProtoBytes());
			}
		}

		// Token: 0x06003F33 RID: 16179 RVA: 0x00174B78 File Offset: 0x00172D78
		[ServerVar]
		public static void clearugcentity(ConsoleSystem.Arg arg)
		{
			uint @uint = arg.GetUInt(0, 0U);
			global::BaseNetworkable baseNetworkable = global::BaseNetworkable.serverEntities.Find(@uint);
			IUGCBrowserEntity iugcbrowserEntity;
			if (baseNetworkable != null && baseNetworkable.TryGetComponent<IUGCBrowserEntity>(out iugcbrowserEntity))
			{
				iugcbrowserEntity.ClearContent();
				arg.ReplyWith(string.Format("Cleared content on {0}/{1}", baseNetworkable.ShortPrefabName, @uint));
				return;
			}
			arg.ReplyWith(string.Format("Could not find UGC entity with id {0}", @uint));
		}

		// Token: 0x06003F34 RID: 16180 RVA: 0x00174BE8 File Offset: 0x00172DE8
		[ServerVar]
		public static void clearugcentitiesinrange(ConsoleSystem.Arg arg)
		{
			Vector3 vector = arg.GetVector3(0, default(Vector3));
			float @float = arg.GetFloat(1, 0f);
			int num = 0;
			foreach (global::BaseNetworkable baseNetworkable in global::BaseNetworkable.serverEntities)
			{
				IUGCBrowserEntity iugcbrowserEntity;
				if (baseNetworkable.TryGetComponent<IUGCBrowserEntity>(out iugcbrowserEntity) && Vector3.Distance(baseNetworkable.transform.position, vector) <= @float)
				{
					iugcbrowserEntity.ClearContent();
					num++;
				}
			}
			arg.ReplyWith(string.Format("Cleared {0} UGC entities within {1}m of {2}", num, @float, vector));
		}

		// Token: 0x06003F35 RID: 16181 RVA: 0x00174CA0 File Offset: 0x00172EA0
		[ServerVar]
		public static void getugcinfo(ConsoleSystem.Arg arg)
		{
			uint @uint = arg.GetUInt(0, 0U);
			global::BaseNetworkable baseNetworkable = global::BaseNetworkable.serverEntities.Find(@uint);
			IUGCBrowserEntity fromEntity;
			if (baseNetworkable != null && baseNetworkable.TryGetComponent<IUGCBrowserEntity>(out fromEntity))
			{
				Admin.ServerUGCInfo serverUGCInfo = new Admin.ServerUGCInfo(fromEntity);
				arg.ReplyWith(JsonConvert.SerializeObject(serverUGCInfo));
				return;
			}
			arg.ReplyWith(string.Format("Invalid entity id: {0}", @uint));
		}

		// Token: 0x06003F36 RID: 16182 RVA: 0x00174D08 File Offset: 0x00172F08
		[ServerVar(Help = "Returns all entities that the provided player is authed to (TC's, locks, etc), supports --json")]
		public static void authcount(ConsoleSystem.Arg arg)
		{
			global::BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			if (playerOrSleeper == null)
			{
				arg.ReplyWith("Please provide a valid player, unable to find '" + arg.GetString(0, "") + "'");
				return;
			}
			string text = arg.GetString(1, "");
			if (text == "--json")
			{
				text = string.Empty;
			}
			List<global::BaseEntity> list = Pool.GetList<global::BaseEntity>();
			Admin.FindEntityAssociationsForPlayer(playerOrSleeper, false, true, text, list);
			TextTable textTable = new TextTable();
			textTable.AddColumns(new string[]
			{
				"Prefab name",
				"Position",
				"ID"
			});
			foreach (global::BaseEntity baseEntity in list)
			{
				textTable.AddRow(new string[]
				{
					baseEntity.ShortPrefabName,
					baseEntity.transform.position.ToString(),
					baseEntity.net.ID.ToString()
				});
			}
			Pool.FreeList<global::BaseEntity>(ref list);
			if (arg.HasArg("--json"))
			{
				arg.ReplyWith(textTable.ToJson());
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Found entities " + playerOrSleeper.displayName + " is authed to");
			stringBuilder.AppendLine(textTable.ToString());
			arg.ReplyWith(stringBuilder.ToString());
		}

		// Token: 0x06003F37 RID: 16183 RVA: 0x00174E88 File Offset: 0x00173088
		[ServerVar(Help = "Returns all entities that the provided player has placed, supports --json")]
		public static void entcount(ConsoleSystem.Arg arg)
		{
			global::BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			if (playerOrSleeper == null)
			{
				arg.ReplyWith("Please provide a valid player, unable to find '" + arg.GetString(0, "") + "'");
				return;
			}
			string text = arg.GetString(1, "");
			if (text == "--json")
			{
				text = string.Empty;
			}
			List<global::BaseEntity> list = Pool.GetList<global::BaseEntity>();
			Admin.FindEntityAssociationsForPlayer(playerOrSleeper, true, false, text, list);
			TextTable textTable = new TextTable();
			textTable.AddColumns(new string[]
			{
				"Prefab name",
				"Position",
				"ID"
			});
			foreach (global::BaseEntity baseEntity in list)
			{
				textTable.AddRow(new string[]
				{
					baseEntity.ShortPrefabName,
					baseEntity.transform.position.ToString(),
					baseEntity.net.ID.ToString()
				});
			}
			Pool.FreeList<global::BaseEntity>(ref list);
			if (arg.HasArg("--json"))
			{
				arg.ReplyWith(textTable.ToJson());
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Found entities associated with " + playerOrSleeper.displayName);
			stringBuilder.AppendLine(textTable.ToString());
			arg.ReplyWith(stringBuilder.ToString());
		}

		// Token: 0x06003F38 RID: 16184 RVA: 0x00175004 File Offset: 0x00173204
		private static void FindEntityAssociationsForPlayer(global::BasePlayer ply, bool useOwnerId, bool useAuth, string filter, List<global::BaseEntity> results)
		{
			results.Clear();
			foreach (global::BaseNetworkable baseNetworkable in global::BaseNetworkable.serverEntities)
			{
				global::BaseEntity baseEntity;
				if ((baseEntity = (baseNetworkable as global::BaseEntity)) != null)
				{
					bool flag = false;
					if (useOwnerId && baseEntity.OwnerID == ply.userID)
					{
						flag = true;
					}
					if (useAuth)
					{
						BuildingPrivlidge buildingPrivlidge;
						if (!flag && (buildingPrivlidge = (baseEntity as BuildingPrivlidge)) != null && buildingPrivlidge.IsAuthed(ply.userID))
						{
							flag = true;
						}
						global::KeyLock keyLock;
						global::CodeLock codeLock;
						if (!flag && (keyLock = (baseEntity as global::KeyLock)) != null && keyLock.HasLockPermission(ply))
						{
							flag = true;
						}
						else if ((codeLock = (baseEntity as global::CodeLock)) != null && codeLock.whitelistPlayers.Contains(ply.userID))
						{
							flag = true;
						}
						global::ModularCar modularCar;
						if (!flag && (modularCar = (baseEntity as global::ModularCar)) != null && modularCar.IsLockable && modularCar.CarLock.HasLockPermission(ply))
						{
							flag = true;
						}
					}
					if (flag && !string.IsNullOrEmpty(filter) && !baseNetworkable.ShortPrefabName.Contains(filter, CompareOptions.IgnoreCase))
					{
						flag = false;
					}
					if (flag)
					{
						results.Add(baseEntity);
					}
				}
			}
		}

		// Token: 0x04003844 RID: 14404
		[ReplicatedVar(Help = "Controls whether the in-game admin UI is displayed to admins")]
		public static bool allowAdminUI = true;

		// Token: 0x02000ED8 RID: 3800
		private enum ChangeGradeMode
		{
			// Token: 0x04004C62 RID: 19554
			Upgrade,
			// Token: 0x04004C63 RID: 19555
			Downgrade
		}

		// Token: 0x02000ED9 RID: 3801
		[Preserve]
		public struct PlayerInfo
		{
			// Token: 0x04004C64 RID: 19556
			public string SteamID;

			// Token: 0x04004C65 RID: 19557
			public string OwnerSteamID;

			// Token: 0x04004C66 RID: 19558
			public string DisplayName;

			// Token: 0x04004C67 RID: 19559
			public int Ping;

			// Token: 0x04004C68 RID: 19560
			public string Address;

			// Token: 0x04004C69 RID: 19561
			public int ConnectedSeconds;

			// Token: 0x04004C6A RID: 19562
			public float VoiationLevel;

			// Token: 0x04004C6B RID: 19563
			public float CurrentLevel;

			// Token: 0x04004C6C RID: 19564
			public float UnspentXp;

			// Token: 0x04004C6D RID: 19565
			public float Health;
		}

		// Token: 0x02000EDA RID: 3802
		[Preserve]
		public struct ServerInfoOutput
		{
			// Token: 0x04004C6E RID: 19566
			public string Hostname;

			// Token: 0x04004C6F RID: 19567
			public int MaxPlayers;

			// Token: 0x04004C70 RID: 19568
			public int Players;

			// Token: 0x04004C71 RID: 19569
			public int Queued;

			// Token: 0x04004C72 RID: 19570
			public int Joining;

			// Token: 0x04004C73 RID: 19571
			public int EntityCount;

			// Token: 0x04004C74 RID: 19572
			public string GameTime;

			// Token: 0x04004C75 RID: 19573
			public int Uptime;

			// Token: 0x04004C76 RID: 19574
			public string Map;

			// Token: 0x04004C77 RID: 19575
			public float Framerate;

			// Token: 0x04004C78 RID: 19576
			public int Memory;

			// Token: 0x04004C79 RID: 19577
			public int Collections;

			// Token: 0x04004C7A RID: 19578
			public int NetworkIn;

			// Token: 0x04004C7B RID: 19579
			public int NetworkOut;

			// Token: 0x04004C7C RID: 19580
			public bool Restarting;

			// Token: 0x04004C7D RID: 19581
			public string SaveCreatedTime;

			// Token: 0x04004C7E RID: 19582
			public int Version;

			// Token: 0x04004C7F RID: 19583
			public string Protocol;
		}

		// Token: 0x02000EDB RID: 3803
		[Preserve]
		public struct ServerConvarInfo
		{
			// Token: 0x04004C80 RID: 19584
			public string FullName;

			// Token: 0x04004C81 RID: 19585
			public string Value;

			// Token: 0x04004C82 RID: 19586
			public string Help;
		}

		// Token: 0x02000EDC RID: 3804
		[Preserve]
		public struct ServerUGCInfo
		{
			// Token: 0x0600515A RID: 20826 RVA: 0x001A479C File Offset: 0x001A299C
			public ServerUGCInfo(IUGCBrowserEntity fromEntity)
			{
				this.entityId = fromEntity.UgcEntity.net.ID;
				this.crcs = fromEntity.GetContentCRCs;
				this.contentType = fromEntity.ContentType;
				this.entityPrefabID = fromEntity.UgcEntity.prefabID;
				this.shortPrefabName = fromEntity.UgcEntity.ShortPrefabName;
				this.playerIds = fromEntity.EditingHistory.ToArray();
			}

			// Token: 0x04004C83 RID: 19587
			public uint entityId;

			// Token: 0x04004C84 RID: 19588
			public uint[] crcs;

			// Token: 0x04004C85 RID: 19589
			public UGCType contentType;

			// Token: 0x04004C86 RID: 19590
			public uint entityPrefabID;

			// Token: 0x04004C87 RID: 19591
			public string shortPrefabName;

			// Token: 0x04004C88 RID: 19592
			public ulong[] playerIds;
		}
	}
}
