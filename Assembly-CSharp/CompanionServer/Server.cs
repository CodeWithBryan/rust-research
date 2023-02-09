using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CompanionServer.Handlers;
using ConVar;
using Facepunch;
using Newtonsoft.Json;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer
{
	// Token: 0x020009B7 RID: 2487
	public static class Server
	{
		// Token: 0x1700048E RID: 1166
		// (get) Token: 0x06003AEA RID: 15082 RVA: 0x0015978A File Offset: 0x0015798A
		// (set) Token: 0x06003AEB RID: 15083 RVA: 0x00159791 File Offset: 0x00157991
		public static Listener Listener { get; private set; }

		// Token: 0x1700048F RID: 1167
		// (get) Token: 0x06003AEC RID: 15084 RVA: 0x00159799 File Offset: 0x00157999
		public static bool IsEnabled
		{
			get
			{
				return App.port >= 0 && !string.IsNullOrWhiteSpace(App.serverid) && Server.Listener != null;
			}
		}

		// Token: 0x06003AED RID: 15085 RVA: 0x001597BC File Offset: 0x001579BC
		public static void Initialize()
		{
			if (App.port < 0)
			{
				return;
			}
			if (Server.IsEnabled)
			{
				UnityEngine.Debug.LogWarning("Rust+ is already started up! Skipping second startup");
				return;
			}
			BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
			if (activeGameMode != null && !activeGameMode.rustPlus)
			{
				return;
			}
			Map.PopulateCache();
			if (App.port == 0)
			{
				App.port = Math.Max(Server.port, RCon.Port) + 67;
			}
			try
			{
				Server.Listener = new Listener(App.GetListenIP(), App.port);
			}
			catch (Exception arg)
			{
				UnityEngine.Debug.LogError(string.Format("Companion server failed to start: {0}", arg));
			}
			Server.PostInitializeServer();
		}

		// Token: 0x06003AEE RID: 15086 RVA: 0x00159860 File Offset: 0x00157A60
		public static void Shutdown()
		{
			Server.SetServerId(null);
			Listener listener = Server.Listener;
			if (listener != null)
			{
				listener.Dispose();
			}
			Server.Listener = null;
		}

		// Token: 0x06003AEF RID: 15087 RVA: 0x0015987E File Offset: 0x00157A7E
		public static void Update()
		{
			Listener listener = Server.Listener;
			if (listener == null)
			{
				return;
			}
			listener.Update();
		}

		// Token: 0x06003AF0 RID: 15088 RVA: 0x0015988F File Offset: 0x00157A8F
		public static void Broadcast(PlayerTarget target, AppBroadcast broadcast)
		{
			Listener listener = Server.Listener;
			if (listener == null)
			{
				return;
			}
			SubscriberList<PlayerTarget, Connection, AppBroadcast> playerSubscribers = listener.PlayerSubscribers;
			if (playerSubscribers == null)
			{
				return;
			}
			playerSubscribers.Send(target, broadcast);
		}

		// Token: 0x06003AF1 RID: 15089 RVA: 0x001598AC File Offset: 0x00157AAC
		public static void Broadcast(EntityTarget target, AppBroadcast broadcast)
		{
			Listener listener = Server.Listener;
			if (listener == null)
			{
				return;
			}
			SubscriberList<EntityTarget, Connection, AppBroadcast> entitySubscribers = listener.EntitySubscribers;
			if (entitySubscribers == null)
			{
				return;
			}
			entitySubscribers.Send(target, broadcast);
		}

		// Token: 0x06003AF2 RID: 15090 RVA: 0x001598C9 File Offset: 0x00157AC9
		public static void ClearSubscribers(EntityTarget target)
		{
			Listener listener = Server.Listener;
			if (listener == null)
			{
				return;
			}
			SubscriberList<EntityTarget, Connection, AppBroadcast> entitySubscribers = listener.EntitySubscribers;
			if (entitySubscribers == null)
			{
				return;
			}
			entitySubscribers.Clear(target);
		}

		// Token: 0x06003AF3 RID: 15091 RVA: 0x001598E5 File Offset: 0x00157AE5
		public static bool CanSendPairingNotification(ulong playerId)
		{
			Listener listener = Server.Listener;
			return listener != null && listener.CanSendPairingNotification(playerId);
		}

		// Token: 0x06003AF4 RID: 15092 RVA: 0x001598F8 File Offset: 0x00157AF8
		private static async void PostInitializeServer()
		{
			await Server.SetupServerRegistration();
			await Server.CheckConnectivity();
		}

		// Token: 0x06003AF5 RID: 15093 RVA: 0x0015992C File Offset: 0x00157B2C
		private static async Task SetupServerRegistration()
		{
			try
			{
				string text;
				string content;
				if (Server.TryLoadServerRegistration(out text, out content))
				{
					StringContent content2 = new StringContent(content, Encoding.UTF8, "text/plain");
					HttpResponseMessage httpResponseMessage = await Server.Http.PostAsync("https://companion-rust.facepunch.com/api/server/refresh", content2);
					if (httpResponseMessage.IsSuccessStatusCode)
					{
						Server.SetServerRegistration(await httpResponseMessage.Content.ReadAsStringAsync());
						return;
					}
					UnityEngine.Debug.LogWarning("Failed to refresh server ID - registering a new one");
				}
				Server.SetServerRegistration(await Server.Http.GetStringAsync("https://companion-rust.facepunch.com/api/server/register"));
			}
			catch (Exception arg)
			{
				UnityEngine.Debug.LogError(string.Format("Failed to setup companion server registration: {0}", arg));
			}
		}

		// Token: 0x06003AF6 RID: 15094 RVA: 0x0015996C File Offset: 0x00157B6C
		private static bool TryLoadServerRegistration(out string serverId, out string serverToken)
		{
			serverId = null;
			serverToken = null;
			string serverIdPath = Server.GetServerIdPath();
			if (!File.Exists(serverIdPath))
			{
				return false;
			}
			bool result;
			try
			{
				Server.RegisterResponse registerResponse = JsonConvert.DeserializeObject<Server.RegisterResponse>(File.ReadAllText(serverIdPath));
				serverId = registerResponse.ServerId;
				serverToken = registerResponse.ServerToken;
				result = true;
			}
			catch (Exception arg)
			{
				UnityEngine.Debug.LogError(string.Format("Failed to load companion server registration: {0}", arg));
				result = false;
			}
			return result;
		}

		// Token: 0x06003AF7 RID: 15095 RVA: 0x001599D8 File Offset: 0x00157BD8
		private static void SetServerRegistration(string responseJson)
		{
			Server.RegisterResponse registerResponse = null;
			try
			{
				registerResponse = JsonConvert.DeserializeObject<Server.RegisterResponse>(responseJson);
			}
			catch (Exception arg)
			{
				UnityEngine.Debug.LogError(string.Format("Failed to parse registration response JSON: {0}\n\n{1}", responseJson, arg));
			}
			Server.SetServerId((registerResponse != null) ? registerResponse.ServerId : null);
			Server.Token = ((registerResponse != null) ? registerResponse.ServerToken : null);
			if (registerResponse == null)
			{
				return;
			}
			try
			{
				File.WriteAllText(Server.GetServerIdPath(), responseJson);
			}
			catch (Exception arg2)
			{
				UnityEngine.Debug.LogError(string.Format("Unable to save companion app server registration - server ID may be different after restart: {0}", arg2));
			}
		}

		// Token: 0x06003AF8 RID: 15096 RVA: 0x00159A68 File Offset: 0x00157C68
		private static async Task CheckConnectivity()
		{
			if (!Server.IsEnabled)
			{
				Server.SetServerId(null);
			}
			else
			{
				try
				{
					string arg = await Server.GetPublicIPAsync();
					StringContent content = new StringContent("", Encoding.UTF8, "text/plain");
					HttpResponseMessage testResponse = await Server.Http.PostAsync("https://companion-rust.facepunch.com/api/server" + string.Format("/test_connection?address={0}&port={1}", arg, App.port), content);
					string text = await testResponse.Content.ReadAsStringAsync();
					Server.TestConnectionResponse testConnectionResponse = null;
					try
					{
						testConnectionResponse = JsonConvert.DeserializeObject<Server.TestConnectionResponse>(text);
					}
					catch (Exception arg2)
					{
						UnityEngine.Debug.LogError(string.Format("Failed to parse connectivity test response JSON: {0}\n\n{1}", text, arg2));
					}
					if (testConnectionResponse != null)
					{
						string text2 = string.Join("\n", testConnectionResponse.Messages ?? Enumerable.Empty<string>());
						if (testResponse.StatusCode == (HttpStatusCode)555)
						{
							UnityEngine.Debug.LogError("Rust+ companion server connectivity test failed! Disabling Rust+ features.\n\n" + text2);
							Server.SetServerId(null);
						}
						else
						{
							testResponse.EnsureSuccessStatusCode();
							if (!string.IsNullOrWhiteSpace(text2))
							{
								UnityEngine.Debug.LogWarning("Rust+ companion server connectivity test has warnings:\n" + text2);
							}
						}
					}
					testResponse = null;
				}
				catch (Exception arg3)
				{
					UnityEngine.Debug.LogError(string.Format("Failed to check connectivity to the companion server: {0}", arg3));
				}
			}
		}

		// Token: 0x06003AF9 RID: 15097 RVA: 0x00159AA8 File Offset: 0x00157CA8
		private static async Task<string> GetPublicIPAsync()
		{
			Stopwatch timer = Stopwatch.StartNew();
			string publicIP;
			for (;;)
			{
				bool flag = timer.Elapsed.TotalMinutes > 2.0;
				publicIP = App.GetPublicIP();
				if (flag || (!string.IsNullOrWhiteSpace(publicIP) && publicIP != "0.0.0.0"))
				{
					break;
				}
				await Task.Delay(10000);
			}
			return publicIP;
		}

		// Token: 0x06003AFA RID: 15098 RVA: 0x00159AE5 File Offset: 0x00157CE5
		private static void SetServerId(string serverId)
		{
			ConsoleSystem.Command command = ConsoleSystem.Index.Server.Find("app.serverid");
			if (command == null)
			{
				return;
			}
			command.Set(serverId ?? "");
		}

		// Token: 0x06003AFB RID: 15099 RVA: 0x00159B05 File Offset: 0x00157D05
		private static string GetServerIdPath()
		{
			return Path.Combine(Server.rootFolder, "companion.id");
		}

		// Token: 0x04003510 RID: 13584
		private const string ApiEndpoint = "https://companion-rust.facepunch.com/api/server";

		// Token: 0x04003511 RID: 13585
		private static readonly HttpClient Http = new HttpClient();

		// Token: 0x04003512 RID: 13586
		internal static readonly ChatLog TeamChat = new ChatLog();

		// Token: 0x04003513 RID: 13587
		internal static string Token;

		// Token: 0x02000E92 RID: 3730
		private class RegisterResponse
		{
			// Token: 0x04004B2D RID: 19245
			public string ServerId;

			// Token: 0x04004B2E RID: 19246
			public string ServerToken;
		}

		// Token: 0x02000E93 RID: 3731
		private class TestConnectionResponse
		{
			// Token: 0x04004B2F RID: 19247
			public List<string> Messages;
		}
	}
}
