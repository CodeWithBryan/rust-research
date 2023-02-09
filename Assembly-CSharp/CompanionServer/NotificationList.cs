using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ConVar;
using Facepunch;
using Network;
using Newtonsoft.Json;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer
{
	// Token: 0x020009B4 RID: 2484
	public class NotificationList
	{
		// Token: 0x06003AD4 RID: 15060 RVA: 0x001593A9 File Offset: 0x001575A9
		public bool AddSubscription(ulong steamId)
		{
			return steamId != 0UL && this._subscriptions.Count < 50 && this._subscriptions.Add(steamId);
		}

		// Token: 0x06003AD5 RID: 15061 RVA: 0x001593CD File Offset: 0x001575CD
		public bool RemoveSubscription(ulong steamId)
		{
			return this._subscriptions.Remove(steamId);
		}

		// Token: 0x06003AD6 RID: 15062 RVA: 0x001593DB File Offset: 0x001575DB
		public bool HasSubscription(ulong steamId)
		{
			return this._subscriptions.Contains(steamId);
		}

		// Token: 0x06003AD7 RID: 15063 RVA: 0x001593EC File Offset: 0x001575EC
		public List<ulong> ToList()
		{
			List<ulong> list = Facepunch.Pool.GetList<ulong>();
			foreach (ulong item in this._subscriptions)
			{
				list.Add(item);
			}
			return list;
		}

		// Token: 0x06003AD8 RID: 15064 RVA: 0x00159448 File Offset: 0x00157648
		public void LoadFrom(List<ulong> steamIds)
		{
			this._subscriptions.Clear();
			if (steamIds == null)
			{
				return;
			}
			foreach (ulong item in steamIds)
			{
				this._subscriptions.Add(item);
			}
		}

		// Token: 0x06003AD9 RID: 15065 RVA: 0x001594AC File Offset: 0x001576AC
		public void IntersectWith(List<PlayerNameID> players)
		{
			List<ulong> list = Facepunch.Pool.GetList<ulong>();
			foreach (PlayerNameID playerNameID in players)
			{
				list.Add(playerNameID.userid);
			}
			this._subscriptions.IntersectWith(list);
			Facepunch.Pool.FreeList<ulong>(ref list);
		}

		// Token: 0x06003ADA RID: 15066 RVA: 0x00159518 File Offset: 0x00157718
		public Task<NotificationSendResult> SendNotification(NotificationChannel channel, string title, string body, string type)
		{
			double realtimeSinceStartup = TimeEx.realtimeSinceStartup;
			if (realtimeSinceStartup - this._lastSend < 15.0)
			{
				return Task.FromResult<NotificationSendResult>(NotificationSendResult.RateLimited);
			}
			Dictionary<string, string> serverPairingData = Util.GetServerPairingData();
			if (!string.IsNullOrWhiteSpace(type))
			{
				serverPairingData["type"] = type;
			}
			this._lastSend = realtimeSinceStartup;
			return NotificationList.SendNotificationImpl(this._subscriptions, channel, title, body, serverPairingData);
		}

		// Token: 0x06003ADB RID: 15067 RVA: 0x00159578 File Offset: 0x00157778
		public static async Task<NotificationSendResult> SendNotificationTo(ICollection<ulong> steamIds, NotificationChannel channel, string title, string body, Dictionary<string, string> data)
		{
			NotificationSendResult notificationSendResult = await NotificationList.SendNotificationImpl(steamIds, channel, title, body, data);
			if (notificationSendResult == NotificationSendResult.NoTargetsFound)
			{
				notificationSendResult = NotificationSendResult.Sent;
			}
			return notificationSendResult;
		}

		// Token: 0x06003ADC RID: 15068 RVA: 0x001595E0 File Offset: 0x001577E0
		public static async Task<NotificationSendResult> SendNotificationTo(ulong steamId, NotificationChannel channel, string title, string body, Dictionary<string, string> data)
		{
			HashSet<ulong> set = Facepunch.Pool.Get<HashSet<ulong>>();
			set.Clear();
			set.Add(steamId);
			NotificationSendResult result = await NotificationList.SendNotificationImpl(set, channel, title, body, data);
			set.Clear();
			Facepunch.Pool.Free<HashSet<ulong>>(ref set);
			return result;
		}

		// Token: 0x06003ADD RID: 15069 RVA: 0x00159648 File Offset: 0x00157848
		private static async Task<NotificationSendResult> SendNotificationImpl(ICollection<ulong> steamIds, NotificationChannel channel, string title, string body, Dictionary<string, string> data)
		{
			NotificationSendResult result;
			if (!CompanionServer.Server.IsEnabled || !App.notifications)
			{
				result = NotificationSendResult.Disabled;
			}
			else if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(body))
			{
				result = NotificationSendResult.Empty;
			}
			else if (steamIds.Count == 0)
			{
				result = NotificationSendResult.Sent;
			}
			else
			{
				PushRequest pushRequest = Facepunch.Pool.Get<PushRequest>();
				pushRequest.ServerToken = CompanionServer.Server.Token;
				pushRequest.Channel = channel;
				pushRequest.Title = title;
				pushRequest.Body = body;
				pushRequest.Data = data;
				pushRequest.SteamIds = Facepunch.Pool.GetList<ulong>();
				foreach (ulong item in steamIds)
				{
					pushRequest.SteamIds.Add(item);
				}
				string content = JsonConvert.SerializeObject(pushRequest);
				Facepunch.Pool.Free<PushRequest>(ref pushRequest);
				try
				{
					StringContent content2 = new StringContent(content, Encoding.UTF8, "application/json");
					HttpResponseMessage httpResponseMessage = await NotificationList.Http.PostAsync("https://companion-rust.facepunch.com/api/push/send", content2);
					if (!httpResponseMessage.IsSuccessStatusCode)
					{
						DebugEx.LogWarning(string.Format("Failed to send notification: {0}", httpResponseMessage.StatusCode), StackTraceLogType.None);
						result = NotificationSendResult.ServerError;
					}
					else if (httpResponseMessage.StatusCode == HttpStatusCode.Accepted)
					{
						result = NotificationSendResult.NoTargetsFound;
					}
					else
					{
						result = NotificationSendResult.Sent;
					}
				}
				catch (Exception arg)
				{
					DebugEx.LogWarning(string.Format("Exception thrown when sending notification: {0}", arg), StackTraceLogType.None);
					result = NotificationSendResult.Failed;
				}
			}
			return result;
		}

		// Token: 0x04003505 RID: 13573
		private const string ApiEndpoint = "https://companion-rust.facepunch.com/api/push/send";

		// Token: 0x04003506 RID: 13574
		private static readonly HttpClient Http = new HttpClient();

		// Token: 0x04003507 RID: 13575
		private readonly HashSet<ulong> _subscriptions = new HashSet<ulong>();

		// Token: 0x04003508 RID: 13576
		private double _lastSend;
	}
}
