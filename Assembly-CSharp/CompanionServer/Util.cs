using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using CompanionServer.Handlers;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Math;
using Network;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer
{
	// Token: 0x020009BE RID: 2494
	public static class Util
	{
		// Token: 0x06003B14 RID: 15124 RVA: 0x0015A32C File Offset: 0x0015852C
		public static Vector2 WorldToMap(Vector3 worldPos)
		{
			return new Vector2(worldPos.x - TerrainMeta.Position.x, worldPos.z - TerrainMeta.Position.z);
		}

		// Token: 0x06003B15 RID: 15125 RVA: 0x0015A358 File Offset: 0x00158558
		public static void SendSignedInNotification(global::BasePlayer player)
		{
			if (player == null || player.currentTeam == 0UL)
			{
				return;
			}
			global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindTeam(player.currentTeam);
			Dictionary<string, string> serverPairingData = Util.GetServerPairingData();
			serverPairingData.Add("type", "login");
			serverPairingData.Add("targetId", player.UserIDString);
			serverPairingData.Add("targetName", player.displayName.Truncate(128, null));
			if (playerTeam == null)
			{
				return;
			}
			playerTeam.SendNotification(NotificationChannel.PlayerLoggedIn, player.displayName + " is now online", ConVar.Server.hostname, serverPairingData, player.userID);
		}

		// Token: 0x06003B16 RID: 15126 RVA: 0x0015A3F8 File Offset: 0x001585F8
		public static void SendDeathNotification(global::BasePlayer player, global::BaseEntity killer)
		{
			global::BasePlayer basePlayer;
			string value;
			string text;
			if ((basePlayer = (killer as global::BasePlayer)) != null && basePlayer.GetType() == typeof(global::BasePlayer))
			{
				value = basePlayer.UserIDString;
				text = basePlayer.displayName;
			}
			else
			{
				value = "";
				text = killer.ShortPrefabName;
			}
			if (player == null || string.IsNullOrEmpty(text))
			{
				return;
			}
			Dictionary<string, string> serverPairingData = Util.GetServerPairingData();
			serverPairingData.Add("type", "death");
			serverPairingData.Add("targetId", value);
			serverPairingData.Add("targetName", text.Truncate(128, null));
			NotificationList.SendNotificationTo(player.userID, NotificationChannel.PlayerDied, "You were killed by " + text, ConVar.Server.hostname, serverPairingData);
		}

		// Token: 0x06003B17 RID: 15127 RVA: 0x0015A4B0 File Offset: 0x001586B0
		public static Task<NotificationSendResult> SendPairNotification(string type, global::BasePlayer player, string title, string message, Dictionary<string, string> data)
		{
			if (!CompanionServer.Server.IsEnabled)
			{
				return Task.FromResult<NotificationSendResult>(NotificationSendResult.Disabled);
			}
			if (!CompanionServer.Server.CanSendPairingNotification(player.userID))
			{
				return Task.FromResult<NotificationSendResult>(NotificationSendResult.RateLimited);
			}
			if (data == null)
			{
				data = Util.GetPlayerPairingData(player);
			}
			data.Add("type", type);
			return NotificationList.SendNotificationTo(player.userID, NotificationChannel.Pairing, title, message, data);
		}

		// Token: 0x06003B18 RID: 15128 RVA: 0x0015A50C File Offset: 0x0015870C
		public static Dictionary<string, string> GetServerPairingData()
		{
			Dictionary<string, string> dictionary = Facepunch.Pool.Get<Dictionary<string, string>>();
			dictionary.Clear();
			dictionary.Add("id", App.serverid);
			dictionary.Add("name", ConVar.Server.hostname.Truncate(128, null));
			dictionary.Add("desc", ConVar.Server.description.Truncate(512, null));
			dictionary.Add("img", ConVar.Server.headerimage.Truncate(128, null));
			dictionary.Add("logo", ConVar.Server.logoimage.Truncate(128, null));
			dictionary.Add("url", ConVar.Server.url.Truncate(128, null));
			dictionary.Add("ip", App.GetPublicIP());
			dictionary.Add("port", App.port.ToString("G", CultureInfo.InvariantCulture));
			return dictionary;
		}

		// Token: 0x06003B19 RID: 15129 RVA: 0x0015A5EC File Offset: 0x001587EC
		public static Dictionary<string, string> GetPlayerPairingData(global::BasePlayer player)
		{
			bool flag;
			int orGenerateAppToken = SingletonComponent<ServerMgr>.Instance.persistance.GetOrGenerateAppToken(player.userID, out flag);
			Dictionary<string, string> serverPairingData = Util.GetServerPairingData();
			serverPairingData.Add("playerId", player.UserIDString);
			serverPairingData.Add("playerToken", orGenerateAppToken.ToString("G", CultureInfo.InvariantCulture));
			return serverPairingData;
		}

		// Token: 0x06003B1A RID: 15130 RVA: 0x0015A644 File Offset: 0x00158844
		public static void BroadcastAppTeamRemoval(this global::BasePlayer player)
		{
			AppBroadcast appBroadcast = Facepunch.Pool.Get<AppBroadcast>();
			appBroadcast.teamChanged = Facepunch.Pool.Get<AppTeamChanged>();
			appBroadcast.teamChanged.playerId = player.userID;
			appBroadcast.teamChanged.teamInfo = player.GetAppTeamInfo(player.userID);
			CompanionServer.Server.Broadcast(new PlayerTarget(player.userID), appBroadcast);
		}

		// Token: 0x06003B1B RID: 15131 RVA: 0x0015A69C File Offset: 0x0015889C
		public static void BroadcastAppTeamUpdate(this global::RelationshipManager.PlayerTeam team)
		{
			AppBroadcast appBroadcast = Facepunch.Pool.Get<AppBroadcast>();
			appBroadcast.teamChanged = Facepunch.Pool.Get<AppTeamChanged>();
			appBroadcast.ShouldPool = false;
			foreach (ulong num in team.members)
			{
				appBroadcast.teamChanged.playerId = num;
				appBroadcast.teamChanged.teamInfo = team.GetAppTeamInfo(num);
				CompanionServer.Server.Broadcast(new PlayerTarget(num), appBroadcast);
			}
			appBroadcast.ShouldPool = true;
			appBroadcast.Dispose();
		}

		// Token: 0x06003B1C RID: 15132 RVA: 0x0015A738 File Offset: 0x00158938
		public static void BroadcastTeamChat(this global::RelationshipManager.PlayerTeam team, ulong steamId, string name, string message, string color)
		{
			uint time = (uint)Epoch.Current;
			CompanionServer.Server.TeamChat.Record(team.teamID, steamId, name, message, color, time);
			AppBroadcast appBroadcast = Facepunch.Pool.Get<AppBroadcast>();
			appBroadcast.teamMessage = Facepunch.Pool.Get<AppTeamMessage>();
			appBroadcast.teamMessage.message = Facepunch.Pool.Get<AppChatMessage>();
			appBroadcast.ShouldPool = false;
			AppChatMessage message2 = appBroadcast.teamMessage.message;
			message2.steamId = steamId;
			message2.name = name;
			message2.message = message;
			message2.color = color;
			message2.time = time;
			foreach (ulong steamId2 in team.members)
			{
				CompanionServer.Server.Broadcast(new PlayerTarget(steamId2), appBroadcast);
			}
			appBroadcast.ShouldPool = true;
			appBroadcast.Dispose();
		}

		// Token: 0x06003B1D RID: 15133 RVA: 0x0015A810 File Offset: 0x00158A10
		public static void SendNotification(this global::RelationshipManager.PlayerTeam team, NotificationChannel channel, string title, string body, Dictionary<string, string> data, ulong ignorePlayer = 0UL)
		{
			List<ulong> list = Facepunch.Pool.GetList<ulong>();
			foreach (ulong num in team.members)
			{
				if (num != ignorePlayer)
				{
					global::BasePlayer basePlayer = global::RelationshipManager.FindByID(num);
					if (!(basePlayer == null))
					{
						Networkable net = basePlayer.net;
						if (((net != null) ? net.connection : null) != null)
						{
							continue;
						}
					}
					list.Add(num);
				}
			}
			NotificationList.SendNotificationTo(list, channel, title, body, data);
			Facepunch.Pool.FreeList<ulong>(ref list);
		}

		// Token: 0x06003B1E RID: 15134 RVA: 0x0015A8A8 File Offset: 0x00158AA8
		public static string ToErrorCode(this ValidationResult result)
		{
			switch (result)
			{
			case ValidationResult.NotFound:
				return "not_found";
			case ValidationResult.RateLimit:
				return "rate_limit";
			case ValidationResult.Banned:
				return "banned";
			default:
				return "unknown";
			}
		}

		// Token: 0x06003B1F RID: 15135 RVA: 0x0015A8D8 File Offset: 0x00158AD8
		public static string ToErrorMessage(this NotificationSendResult result)
		{
			switch (result)
			{
			case NotificationSendResult.Sent:
				return null;
			case NotificationSendResult.Empty:
				return Util.NotificationEmpty.translated;
			case NotificationSendResult.Disabled:
				return Util.NotificationDisabled.translated;
			case NotificationSendResult.RateLimited:
				return Util.NotificationRateLimit.translated;
			case NotificationSendResult.ServerError:
				return Util.NotificationServerError.translated;
			case NotificationSendResult.NoTargetsFound:
				return Util.NotificationNoTargets.translated;
			case NotificationSendResult.TooManySubscribers:
				return Util.NotificationTooManySubscribers.translated;
			default:
				return Util.NotificationUnknown.translated;
			}
		}

		// Token: 0x0400351E RID: 13598
		public const int OceanMargin = 500;

		// Token: 0x0400351F RID: 13599
		public static readonly Translate.Phrase NotificationEmpty = new Translate.Phrase("app.error.empty", "Notification was not sent because it was missing some content.");

		// Token: 0x04003520 RID: 13600
		public static readonly Translate.Phrase NotificationDisabled = new Translate.Phrase("app.error.disabled", "Rust+ features are disabled on this server.");

		// Token: 0x04003521 RID: 13601
		public static readonly Translate.Phrase NotificationRateLimit = new Translate.Phrase("app.error.ratelimit", "You are sending too many notifications at a time. Please wait and then try again.");

		// Token: 0x04003522 RID: 13602
		public static readonly Translate.Phrase NotificationServerError = new Translate.Phrase("app.error.servererror", "The companion server failed to send the notification.");

		// Token: 0x04003523 RID: 13603
		public static readonly Translate.Phrase NotificationNoTargets = new Translate.Phrase("app.error.notargets", "Open the Rust+ menu in-game to pair your phone with this server.");

		// Token: 0x04003524 RID: 13604
		public static readonly Translate.Phrase NotificationTooManySubscribers = new Translate.Phrase("app.error.toomanysubs", "There are too many players subscribed to these notifications.");

		// Token: 0x04003525 RID: 13605
		public static readonly Translate.Phrase NotificationUnknown = new Translate.Phrase("app.error.unknown", "An unknown error occurred sending the notification.");
	}
}
