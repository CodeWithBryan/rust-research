using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using CompanionServer;
using Facepunch.Extend;
using Steamworks;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000A63 RID: 2659
	[ConsoleSystem.Factory("app")]
	public class App : ConsoleSystem
	{
		// Token: 0x06003F4A RID: 16202 RVA: 0x0017595C File Offset: 0x00173B5C
		[ServerUserVar]
		public static async void pair(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!(basePlayer == null))
			{
				Dictionary<string, string> playerPairingData = Util.GetPlayerPairingData(basePlayer);
				NotificationSendResult notificationSendResult = await Util.SendPairNotification("server", basePlayer, Server.hostname.Truncate(128, null), "Tap to pair with this server.", playerPairingData);
				arg.ReplyWith((notificationSendResult == NotificationSendResult.Sent) ? "Sent pairing notification." : notificationSendResult.ToErrorMessage());
			}
		}

		// Token: 0x06003F4B RID: 16203 RVA: 0x00175998 File Offset: 0x00173B98
		[ServerVar]
		public static void info(ConsoleSystem.Arg arg)
		{
			if (!Server.IsEnabled)
			{
				arg.ReplyWith("Companion server is not enabled");
				return;
			}
			Listener listener = Server.Listener;
			arg.ReplyWith(string.Format("Server ID: {0}\nListening on: {1}:{2}\nApp connects to: {3}:{4}", new object[]
			{
				App.serverid,
				listener.Address,
				listener.Port,
				App.GetPublicIP(),
				App.port
			}));
		}

		// Token: 0x06003F4C RID: 16204 RVA: 0x00175A08 File Offset: 0x00173C08
		[ServerVar]
		public static void resetlimiter(ConsoleSystem.Arg arg)
		{
			Listener listener = Server.Listener;
			if (listener == null)
			{
				return;
			}
			ConnectionLimiter limiter = listener.Limiter;
			if (limiter == null)
			{
				return;
			}
			limiter.Clear();
		}

		// Token: 0x06003F4D RID: 16205 RVA: 0x00175A24 File Offset: 0x00173C24
		[ServerVar]
		public static void connections(ConsoleSystem.Arg arg)
		{
			Listener listener = Server.Listener;
			string text;
			if (listener == null)
			{
				text = null;
			}
			else
			{
				ConnectionLimiter limiter = listener.Limiter;
				text = ((limiter != null) ? limiter.ToString() : null);
			}
			string strValue = text ?? "Not available";
			arg.ReplyWith(strValue);
		}

		// Token: 0x06003F4E RID: 16206 RVA: 0x00175A60 File Offset: 0x00173C60
		[ServerVar]
		public static void appban(ConsoleSystem.Arg arg)
		{
			ulong @ulong = arg.GetULong(0, 0UL);
			if (@ulong == 0UL)
			{
				arg.ReplyWith("Usage: app.appban <steamID64>");
				return;
			}
			string strValue = SingletonComponent<ServerMgr>.Instance.persistance.SetAppTokenLocked(@ulong, true) ? string.Format("Banned {0} from using the companion app", @ulong) : string.Format("{0} is already banned from using the companion app", @ulong);
			arg.ReplyWith(strValue);
		}

		// Token: 0x06003F4F RID: 16207 RVA: 0x00175AC4 File Offset: 0x00173CC4
		[ServerVar]
		public static void appunban(ConsoleSystem.Arg arg)
		{
			ulong @ulong = arg.GetULong(0, 0UL);
			if (@ulong == 0UL)
			{
				arg.ReplyWith("Usage: app.appunban <steamID64>");
				return;
			}
			string strValue = SingletonComponent<ServerMgr>.Instance.persistance.SetAppTokenLocked(@ulong, false) ? string.Format("Unbanned {0}, they can use the companion app again", @ulong) : string.Format("{0} is not banned from using the companion app", @ulong);
			arg.ReplyWith(strValue);
		}

		// Token: 0x06003F50 RID: 16208 RVA: 0x00175B28 File Offset: 0x00173D28
		public static IPAddress GetListenIP()
		{
			if (string.IsNullOrWhiteSpace(App.listenip))
			{
				return IPAddress.Any;
			}
			IPAddress ipaddress;
			if (!IPAddress.TryParse(App.listenip, out ipaddress) || ipaddress.AddressFamily != AddressFamily.InterNetwork)
			{
				Debug.LogError("Invalid app.listenip: " + App.listenip);
				return IPAddress.Any;
			}
			return ipaddress;
		}

		// Token: 0x06003F51 RID: 16209 RVA: 0x00175B7C File Offset: 0x00173D7C
		public static string GetPublicIP()
		{
			IPAddress ipaddress;
			if (!string.IsNullOrWhiteSpace(App.publicip) && IPAddress.TryParse(App.publicip, out ipaddress) && ipaddress.AddressFamily == AddressFamily.InterNetwork)
			{
				return App.publicip;
			}
			return SteamServer.PublicIp.ToString();
		}

		// Token: 0x040038D9 RID: 14553
		[ServerVar]
		public static string listenip = "";

		// Token: 0x040038DA RID: 14554
		[ServerVar]
		public static int port;

		// Token: 0x040038DB RID: 14555
		[ServerVar]
		public static string publicip = "";

		// Token: 0x040038DC RID: 14556
		[ServerVar(Help = "Disables updating entirely - emergency use only")]
		public static bool update = true;

		// Token: 0x040038DD RID: 14557
		[ServerVar(Help = "Enables sending push notifications")]
		public static bool notifications = true;

		// Token: 0x040038DE RID: 14558
		[ServerVar(Help = "Max number of queued messages - set to 0 to disable message processing")]
		public static int queuelimit = 100;

		// Token: 0x040038DF RID: 14559
		[ReplicatedVar(Default = "")]
		public static string serverid = "";

		// Token: 0x040038E0 RID: 14560
		[ServerVar(Help = "Cooldown time before alarms can send another notification (in seconds)")]
		public static float alarmcooldown = 30f;

		// Token: 0x040038E1 RID: 14561
		[ServerVar]
		public static int maxconnections = 500;

		// Token: 0x040038E2 RID: 14562
		[ServerVar]
		public static int maxconnectionsperip = 5;
	}
}
