using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CircularBuffer;
using CompanionServer;
using Facepunch;
using Facepunch.Math;
using Network;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000A67 RID: 2663
	[ConsoleSystem.Factory("chat")]
	public class Chat : ConsoleSystem
	{
		// Token: 0x06003F5F RID: 16223 RVA: 0x00175DAC File Offset: 0x00173FAC
		public static void Broadcast(string message, string username = "SERVER", string color = "#eee", ulong userid = 0UL)
		{
			string text = username.EscapeRichText();
			ConsoleNetwork.BroadcastToAllClients("chat.add", new object[]
			{
				2,
				0,
				string.Concat(new string[]
				{
					"<color=",
					color,
					">",
					text,
					"</color> ",
					message
				})
			});
			Chat.Record(new Chat.ChatEntry
			{
				Channel = Chat.ChatChannel.Server,
				Message = message,
				UserId = userid.ToString(),
				Username = username,
				Color = color,
				Time = Epoch.Current
			});
		}

		// Token: 0x06003F60 RID: 16224 RVA: 0x00175E5C File Offset: 0x0017405C
		[ServerUserVar]
		public static void say(ConsoleSystem.Arg arg)
		{
			if (Chat.globalchat)
			{
				Chat.sayImpl(Chat.ChatChannel.Global, arg);
			}
		}

		// Token: 0x06003F61 RID: 16225 RVA: 0x00175E6C File Offset: 0x0017406C
		[ServerUserVar]
		public static void localsay(ConsoleSystem.Arg arg)
		{
			if (Chat.localchat)
			{
				Chat.sayImpl(Chat.ChatChannel.Local, arg);
			}
		}

		// Token: 0x06003F62 RID: 16226 RVA: 0x00175E7C File Offset: 0x0017407C
		[ServerUserVar]
		public static void teamsay(ConsoleSystem.Arg arg)
		{
			Chat.sayImpl(Chat.ChatChannel.Team, arg);
		}

		// Token: 0x06003F63 RID: 16227 RVA: 0x00175E85 File Offset: 0x00174085
		[ServerUserVar]
		public static void cardgamesay(ConsoleSystem.Arg arg)
		{
			Chat.sayImpl(Chat.ChatChannel.Cards, arg);
		}

		// Token: 0x06003F64 RID: 16228 RVA: 0x00175E90 File Offset: 0x00174090
		private static void sayImpl(Chat.ChatChannel targetChannel, ConsoleSystem.Arg arg)
		{
			if (!Chat.enabled)
			{
				arg.ReplyWith("Chat is disabled.");
				return;
			}
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.HasPlayerFlag(BasePlayer.PlayerFlags.ChatMute))
			{
				return;
			}
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper)
			{
				if (basePlayer.NextChatTime == 0f)
				{
					basePlayer.NextChatTime = Time.realtimeSinceStartup - 30f;
				}
				if (basePlayer.NextChatTime > Time.realtimeSinceStartup)
				{
					basePlayer.NextChatTime += 2f;
					float num = basePlayer.NextChatTime - Time.realtimeSinceStartup;
					ConsoleNetwork.SendClientCommand(basePlayer.net.connection, "chat.add", new object[]
					{
						2,
						0,
						"You're chatting too fast - try again in " + (num + 0.5f).ToString("0") + " seconds"
					});
					if (num > 120f)
					{
						basePlayer.Kick("Chatting too fast");
					}
					return;
				}
			}
			string @string = arg.GetString(0, "text");
			if (Chat.sayAs(targetChannel, basePlayer.userID, basePlayer.displayName, @string, basePlayer))
			{
				basePlayer.NextChatTime = Time.realtimeSinceStartup + 1.5f;
			}
		}

		// Token: 0x06003F65 RID: 16229 RVA: 0x00175FCC File Offset: 0x001741CC
		internal static bool sayAs(Chat.ChatChannel targetChannel, ulong userId, string username, string message, BasePlayer player = null)
		{
			if (!player)
			{
				player = null;
			}
			if (!Chat.enabled)
			{
				return false;
			}
			if (player != null && player.HasPlayerFlag(BasePlayer.PlayerFlags.ChatMute))
			{
				return false;
			}
			ServerUsers.User user = ServerUsers.Get(userId);
			ServerUsers.UserGroup userGroup = (user != null) ? user.group : ServerUsers.UserGroup.None;
			if (userGroup == ServerUsers.UserGroup.Banned)
			{
				return false;
			}
			string text = message.Replace("\n", "").Replace("\r", "").Trim();
			if (text.Length > 128)
			{
				text = text.Substring(0, 128);
			}
			if (text.Length <= 0)
			{
				return false;
			}
			if (text.StartsWith("/") || text.StartsWith("\\"))
			{
				return false;
			}
			text = text.EscapeRichText();
			if (Chat.serverlog)
			{
				ServerConsole.PrintColoured(new object[]
				{
					ConsoleColor.DarkYellow,
					string.Concat(new object[]
					{
						"[",
						targetChannel,
						"] ",
						username,
						": "
					}),
					ConsoleColor.DarkGreen,
					text
				});
				string str = ((player != null) ? player.ToString() : null) ?? string.Format("{0}[{1}]", username, userId);
				if (targetChannel == Chat.ChatChannel.Team)
				{
					DebugEx.Log("[TEAM CHAT] " + str + " : " + text, StackTraceLogType.None);
				}
				else if (targetChannel == Chat.ChatChannel.Cards)
				{
					DebugEx.Log("[CARDS CHAT] " + str + " : " + text, StackTraceLogType.None);
				}
				else
				{
					DebugEx.Log("[CHAT] " + str + " : " + text, StackTraceLogType.None);
				}
			}
			bool flag = userGroup == ServerUsers.UserGroup.Owner || userGroup == ServerUsers.UserGroup.Moderator;
			bool flag2 = (player != null) ? player.IsDeveloper : DeveloperList.Contains(userId);
			string text2 = "#5af";
			if (flag)
			{
				text2 = "#af5";
			}
			if (flag2)
			{
				text2 = "#fa5";
			}
			string text3 = username.EscapeRichText();
			Chat.Record(new Chat.ChatEntry
			{
				Channel = targetChannel,
				Message = text,
				UserId = ((player != null) ? player.UserIDString : userId.ToString()),
				Username = username,
				Color = text2,
				Time = Epoch.Current
			});
			switch (targetChannel)
			{
			case Chat.ChatChannel.Global:
				ConsoleNetwork.BroadcastToAllClients("chat.add2", new object[]
				{
					0,
					userId,
					text,
					text3,
					text2,
					1f
				});
				return true;
			case Chat.ChatChannel.Team:
			{
				RelationshipManager.PlayerTeam playerTeam = RelationshipManager.ServerInstance.FindPlayersTeam(userId);
				if (playerTeam == null)
				{
					return false;
				}
				List<Network.Connection> onlineMemberConnections = playerTeam.GetOnlineMemberConnections();
				if (onlineMemberConnections != null)
				{
					ConsoleNetwork.SendClientCommand(onlineMemberConnections, "chat.add2", new object[]
					{
						1,
						userId,
						text,
						text3,
						text2,
						1f
					});
				}
				playerTeam.BroadcastTeamChat(userId, text3, text, text2);
				return true;
			}
			case Chat.ChatChannel.Cards:
			{
				if (player == null)
				{
					return false;
				}
				if (!player.isMounted)
				{
					return false;
				}
				BaseCardGameEntity baseCardGameEntity = player.GetMountedVehicle() as BaseCardGameEntity;
				if (baseCardGameEntity == null || !baseCardGameEntity.GameController.IsAtTable(player))
				{
					return false;
				}
				List<Network.Connection> list = Pool.GetList<Network.Connection>();
				baseCardGameEntity.GameController.GetConnectionsInGame(list);
				if (list.Count > 0)
				{
					ConsoleNetwork.SendClientCommand(list, "chat.add2", new object[]
					{
						3,
						userId,
						text,
						text3,
						text2,
						1f
					});
				}
				Pool.FreeList<Network.Connection>(ref list);
				return true;
			}
			case Chat.ChatChannel.Local:
				if (player != null)
				{
					float num = Chat.localChatRange * Chat.localChatRange;
					foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
					{
						float sqrMagnitude = (basePlayer.transform.position - player.transform.position).sqrMagnitude;
						if (sqrMagnitude <= num)
						{
							ConsoleNetwork.SendClientCommand(basePlayer.net.connection, "chat.add2", new object[]
							{
								4,
								userId,
								text,
								text3,
								text2,
								Mathf.Clamp01(sqrMagnitude / num + 0.2f)
							});
						}
					}
					return true;
				}
				break;
			}
			return false;
		}

		// Token: 0x06003F66 RID: 16230 RVA: 0x0017645C File Offset: 0x0017465C
		[ServerVar]
		[Help("Return the last x lines of the console. Default is 200")]
		public static IEnumerable<Chat.ChatEntry> tail(ConsoleSystem.Arg arg)
		{
			int @int = arg.GetInt(0, 200);
			int num = Chat.History.Size - @int;
			if (num < 0)
			{
				num = 0;
			}
			return Chat.History.Skip(num);
		}

		// Token: 0x06003F67 RID: 16231 RVA: 0x00176494 File Offset: 0x00174694
		[ServerVar]
		[Help("Search the console for a particular string")]
		public static IEnumerable<Chat.ChatEntry> search(ConsoleSystem.Arg arg)
		{
			string search = arg.GetString(0, null);
			if (search == null)
			{
				return Enumerable.Empty<Chat.ChatEntry>();
			}
			return from x in Chat.History
			where x.Message.Length < 4096 && x.Message.Contains(search, CompareOptions.IgnoreCase)
			select x;
		}

		// Token: 0x06003F68 RID: 16232 RVA: 0x001764DC File Offset: 0x001746DC
		private static void Record(Chat.ChatEntry ce)
		{
			int num = Mathf.Max(Chat.historysize, 10);
			if (Chat.History.Capacity != num)
			{
				CircularBuffer<Chat.ChatEntry> circularBuffer = new CircularBuffer<Chat.ChatEntry>(num);
				foreach (Chat.ChatEntry item in Chat.History)
				{
					circularBuffer.PushBack(item);
				}
				Chat.History = circularBuffer;
			}
			Chat.History.PushBack(ce);
			RCon.Broadcast(RCon.LogType.Chat, ce);
		}

		// Token: 0x040038FA RID: 14586
		[ServerVar]
		public static float localChatRange = 100f;

		// Token: 0x040038FB RID: 14587
		[ReplicatedVar]
		public static bool globalchat = true;

		// Token: 0x040038FC RID: 14588
		[ReplicatedVar]
		public static bool localchat = false;

		// Token: 0x040038FD RID: 14589
		private const float textVolumeBoost = 0.2f;

		// Token: 0x040038FE RID: 14590
		[ServerVar]
		[ClientVar]
		public static bool enabled = true;

		// Token: 0x040038FF RID: 14591
		[ServerVar(Help = "Number of messages to keep in memory for chat history")]
		public static int historysize = 1000;

		// Token: 0x04003900 RID: 14592
		private static CircularBuffer<Chat.ChatEntry> History = new CircularBuffer<Chat.ChatEntry>(Chat.historysize);

		// Token: 0x04003901 RID: 14593
		[ServerVar]
		public static bool serverlog = true;

		// Token: 0x02000EE6 RID: 3814
		public enum ChatChannel
		{
			// Token: 0x04004C9F RID: 19615
			Global,
			// Token: 0x04004CA0 RID: 19616
			Team,
			// Token: 0x04004CA1 RID: 19617
			Server,
			// Token: 0x04004CA2 RID: 19618
			Cards,
			// Token: 0x04004CA3 RID: 19619
			Local
		}

		// Token: 0x02000EE7 RID: 3815
		public struct ChatEntry
		{
			// Token: 0x170006B4 RID: 1716
			// (get) Token: 0x06005174 RID: 20852 RVA: 0x001A4CA2 File Offset: 0x001A2EA2
			// (set) Token: 0x06005175 RID: 20853 RVA: 0x001A4CAA File Offset: 0x001A2EAA
			public Chat.ChatChannel Channel { get; set; }

			// Token: 0x170006B5 RID: 1717
			// (get) Token: 0x06005176 RID: 20854 RVA: 0x001A4CB3 File Offset: 0x001A2EB3
			// (set) Token: 0x06005177 RID: 20855 RVA: 0x001A4CBB File Offset: 0x001A2EBB
			public string Message { get; set; }

			// Token: 0x170006B6 RID: 1718
			// (get) Token: 0x06005178 RID: 20856 RVA: 0x001A4CC4 File Offset: 0x001A2EC4
			// (set) Token: 0x06005179 RID: 20857 RVA: 0x001A4CCC File Offset: 0x001A2ECC
			public string UserId { get; set; }

			// Token: 0x170006B7 RID: 1719
			// (get) Token: 0x0600517A RID: 20858 RVA: 0x001A4CD5 File Offset: 0x001A2ED5
			// (set) Token: 0x0600517B RID: 20859 RVA: 0x001A4CDD File Offset: 0x001A2EDD
			public string Username { get; set; }

			// Token: 0x170006B8 RID: 1720
			// (get) Token: 0x0600517C RID: 20860 RVA: 0x001A4CE6 File Offset: 0x001A2EE6
			// (set) Token: 0x0600517D RID: 20861 RVA: 0x001A4CEE File Offset: 0x001A2EEE
			public string Color { get; set; }

			// Token: 0x170006B9 RID: 1721
			// (get) Token: 0x0600517E RID: 20862 RVA: 0x001A4CF7 File Offset: 0x001A2EF7
			// (set) Token: 0x0600517F RID: 20863 RVA: 0x001A4CFF File Offset: 0x001A2EFF
			public int Time { get; set; }
		}
	}
}
