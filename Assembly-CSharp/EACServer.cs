using System;
using System.Collections.Concurrent;
using ConVar;
using Epic.OnlineServices;
using Epic.OnlineServices.AntiCheatCommon;
using Epic.OnlineServices.AntiCheatServer;
using Epic.OnlineServices.Reports;
using Network;
using UnityEngine;

// Token: 0x02000716 RID: 1814
public static class EACServer
{
	// Token: 0x170003EC RID: 1004
	// (get) Token: 0x06003221 RID: 12833 RVA: 0x00133D20 File Offset: 0x00131F20
	public static bool CanSendAnalytics
	{
		get
		{
			return ConVar.Server.official && ConVar.Server.stats && EACServer.Interface != null;
		}
	}

	// Token: 0x06003222 RID: 12834 RVA: 0x00133D3D File Offset: 0x00131F3D
	private static IntPtr GenerateCompatibilityClient()
	{
		return (IntPtr)((long)((ulong)(EACServer.clientHandleCounter += 1U)));
	}

	// Token: 0x06003223 RID: 12835 RVA: 0x00133D54 File Offset: 0x00131F54
	public static void Encrypt(Connection connection, ArraySegment<byte> src, ref ArraySegment<byte> dst)
	{
		uint count = (uint)dst.Count;
		dst = new ArraySegment<byte>(dst.Array, dst.Offset, 0);
		if (EACServer.Interface != null)
		{
			IntPtr client = EACServer.GetClient(connection);
			if (client != IntPtr.Zero)
			{
				ProtectMessageOptions protectMessageOptions = new ProtectMessageOptions
				{
					ClientHandle = client,
					Data = src,
					OutBufferSizeBytes = count
				};
				uint count2;
				Result result = EACServer.Interface.ProtectMessage(ref protectMessageOptions, dst, out count2);
				if (result == Result.Success)
				{
					dst = new ArraySegment<byte>(dst.Array, dst.Offset, (int)count2);
					return;
				}
				Debug.LogWarning("[EAC] ProtectMessage failed: " + result);
			}
		}
	}

	// Token: 0x06003224 RID: 12836 RVA: 0x00133E0C File Offset: 0x0013200C
	public static void Decrypt(Connection connection, ArraySegment<byte> src, ref ArraySegment<byte> dst)
	{
		uint count = (uint)dst.Count;
		dst = new ArraySegment<byte>(dst.Array, dst.Offset, 0);
		if (EACServer.Interface != null)
		{
			IntPtr client = EACServer.GetClient(connection);
			if (client != IntPtr.Zero)
			{
				UnprotectMessageOptions unprotectMessageOptions = new UnprotectMessageOptions
				{
					ClientHandle = client,
					Data = src,
					OutBufferSizeBytes = count
				};
				uint count2;
				Result result = EACServer.Interface.UnprotectMessage(ref unprotectMessageOptions, dst, out count2);
				if (result == Result.Success)
				{
					dst = new ArraySegment<byte>(dst.Array, dst.Offset, (int)count2);
					return;
				}
				Debug.LogWarning("[EAC] UnprotectMessage failed: " + result);
			}
		}
	}

	// Token: 0x06003225 RID: 12837 RVA: 0x00133EC4 File Offset: 0x001320C4
	public static IntPtr GetClient(Connection connection)
	{
		uint num;
		EACServer.connection2client.TryGetValue(connection, out num);
		return (IntPtr)((long)((ulong)num));
	}

	// Token: 0x06003226 RID: 12838 RVA: 0x00133EE8 File Offset: 0x001320E8
	public static Connection GetConnection(IntPtr client)
	{
		Connection result;
		EACServer.client2connection.TryGetValue((uint)((int)client), out result);
		return result;
	}

	// Token: 0x06003227 RID: 12839 RVA: 0x00133F0C File Offset: 0x0013210C
	public static bool IsAuthenticated(Connection connection)
	{
		AntiCheatCommonClientAuthStatus antiCheatCommonClientAuthStatus;
		EACServer.connection2status.TryGetValue(connection, out antiCheatCommonClientAuthStatus);
		return antiCheatCommonClientAuthStatus == AntiCheatCommonClientAuthStatus.RemoteAuthComplete;
	}

	// Token: 0x06003228 RID: 12840 RVA: 0x00133F2B File Offset: 0x0013212B
	private static void OnAuthenticatedLocal(Connection connection)
	{
		if (connection.authStatus == string.Empty)
		{
			connection.authStatus = "ok";
		}
		EACServer.connection2status[connection] = AntiCheatCommonClientAuthStatus.LocalAuthComplete;
	}

	// Token: 0x06003229 RID: 12841 RVA: 0x00133F56 File Offset: 0x00132156
	private static void OnAuthenticatedRemote(Connection connection)
	{
		EACServer.connection2status[connection] = AntiCheatCommonClientAuthStatus.RemoteAuthComplete;
	}

	// Token: 0x0600322A RID: 12842 RVA: 0x00133F64 File Offset: 0x00132164
	private static void OnClientAuthStatusChanged(ref OnClientAuthStatusChangedCallbackInfo data)
	{
		using (TimeWarning.New("AntiCheatKickPlayer", 10))
		{
			IntPtr clientHandle = data.ClientHandle;
			Connection connection = EACServer.GetConnection(clientHandle);
			if (connection == null)
			{
				Debug.LogError("[EAC] Status update for invalid client: " + clientHandle.ToString());
			}
			else if (data.ClientAuthStatus == AntiCheatCommonClientAuthStatus.LocalAuthComplete)
			{
				EACServer.OnAuthenticatedLocal(connection);
				SetClientNetworkStateOptions setClientNetworkStateOptions = new SetClientNetworkStateOptions
				{
					ClientHandle = clientHandle,
					IsNetworkActive = false
				};
				EACServer.Interface.SetClientNetworkState(ref setClientNetworkStateOptions);
			}
			else if (data.ClientAuthStatus == AntiCheatCommonClientAuthStatus.RemoteAuthComplete)
			{
				EACServer.OnAuthenticatedRemote(connection);
			}
		}
	}

	// Token: 0x0600322B RID: 12843 RVA: 0x0013400C File Offset: 0x0013220C
	private static void OnClientActionRequired(ref OnClientActionRequiredCallbackInfo data)
	{
		using (TimeWarning.New("OnClientActionRequired", 10))
		{
			IntPtr clientHandle = data.ClientHandle;
			Connection connection = EACServer.GetConnection(clientHandle);
			if (connection == null)
			{
				Debug.LogError("[EAC] Status update for invalid client: " + clientHandle.ToString());
			}
			else
			{
				AntiCheatCommonClientAction clientAction = data.ClientAction;
				if (clientAction == AntiCheatCommonClientAction.RemovePlayer)
				{
					Utf8String actionReasonDetailsString = data.ActionReasonDetailsString;
					Debug.Log(string.Format("[EAC] Kicking {0} / {1} ({2})", connection.userid, connection.username, actionReasonDetailsString));
					connection.authStatus = "eac";
					Network.Net.sv.Kick(connection, "EAC: " + actionReasonDetailsString, false);
					if (data.ActionReasonCode == AntiCheatCommonClientActionReason.PermanentBanned || data.ActionReasonCode == AntiCheatCommonClientActionReason.TemporaryBanned)
					{
						connection.authStatus = "eacbanned";
						ConsoleNetwork.BroadcastToAllClients("chat.add", new object[]
						{
							2,
							0,
							"<color=#fff>SERVER</color> Kicking " + connection.username + " (banned by anticheat)"
						});
						if (data.ActionReasonCode == AntiCheatCommonClientActionReason.PermanentBanned)
						{
							Entity.DeleteBy(connection.userid);
						}
					}
					UnregisterClientOptions unregisterClientOptions = new UnregisterClientOptions
					{
						ClientHandle = clientHandle
					};
					EACServer.Interface.UnregisterClient(ref unregisterClientOptions);
					Connection connection2;
					EACServer.client2connection.TryRemove((uint)((int)clientHandle), out connection2);
					uint num;
					EACServer.connection2client.TryRemove(connection, out num);
					AntiCheatCommonClientAuthStatus antiCheatCommonClientAuthStatus;
					EACServer.connection2status.TryRemove(connection, out antiCheatCommonClientAuthStatus);
				}
			}
		}
	}

	// Token: 0x0600322C RID: 12844 RVA: 0x001341A0 File Offset: 0x001323A0
	private static void SendToClient(ref OnMessageToClientCallbackInfo data)
	{
		IntPtr clientHandle = data.ClientHandle;
		Connection connection = EACServer.GetConnection(clientHandle);
		if (connection == null)
		{
			Debug.LogError("[EAC] Network packet for invalid client: " + clientHandle.ToString());
			return;
		}
		NetWrite netWrite = Network.Net.sv.StartWrite();
		netWrite.PacketID(Message.Type.EAC);
		netWrite.UInt32((uint)data.MessageData.Count);
		netWrite.Write(data.MessageData.Array, data.MessageData.Offset, data.MessageData.Count);
		netWrite.Send(new SendInfo(connection));
	}

	// Token: 0x0600322D RID: 12845 RVA: 0x00134238 File Offset: 0x00132438
	public static void DoStartup()
	{
		if (ConVar.Server.secure && !Application.isEditor)
		{
			EACServer.client2connection.Clear();
			EACServer.connection2client.Clear();
			EACServer.connection2status.Clear();
			EOS.Initialize(true, ConVar.Server.anticheatid, ConVar.Server.anticheatkey, ConVar.Server.rootFolder + "/Log.EAC.txt");
			EACServer.Interface = EOS.Interface.GetAntiCheatServerInterface();
			AddNotifyClientActionRequiredOptions addNotifyClientActionRequiredOptions = default(AddNotifyClientActionRequiredOptions);
			EACServer.Interface.AddNotifyClientActionRequired(ref addNotifyClientActionRequiredOptions, null, new OnClientActionRequiredCallback(EACServer.OnClientActionRequired));
			AddNotifyClientAuthStatusChangedOptions addNotifyClientAuthStatusChangedOptions = default(AddNotifyClientAuthStatusChangedOptions);
			EACServer.Interface.AddNotifyClientAuthStatusChanged(ref addNotifyClientAuthStatusChangedOptions, null, new OnClientAuthStatusChangedCallback(EACServer.OnClientAuthStatusChanged));
			AddNotifyMessageToClientOptions addNotifyMessageToClientOptions = default(AddNotifyMessageToClientOptions);
			EACServer.Interface.AddNotifyMessageToClient(ref addNotifyMessageToClientOptions, null, new OnMessageToClientCallback(EACServer.SendToClient));
			BeginSessionOptions beginSessionOptions = new BeginSessionOptions
			{
				LocalUserId = null,
				EnableGameplayData = EACServer.CanSendAnalytics,
				RegisterTimeoutSeconds = 20U,
				ServerName = ConVar.Server.hostname
			};
			EACServer.Interface.BeginSession(ref beginSessionOptions);
			LogGameRoundStartOptions logGameRoundStartOptions = new LogGameRoundStartOptions
			{
				LevelName = global::World.Name
			};
			EACServer.Interface.LogGameRoundStart(ref logGameRoundStartOptions);
			return;
		}
		EACServer.client2connection.Clear();
		EACServer.connection2client.Clear();
		EACServer.connection2status.Clear();
	}

	// Token: 0x0600322E RID: 12846 RVA: 0x001343A3 File Offset: 0x001325A3
	public static void DoUpdate()
	{
		if (ConVar.Server.secure && !Application.isEditor)
		{
			EOS.Tick();
		}
	}

	// Token: 0x0600322F RID: 12847 RVA: 0x001343B8 File Offset: 0x001325B8
	public static void DoShutdown()
	{
		if (ConVar.Server.secure && !Application.isEditor)
		{
			EACServer.client2connection.Clear();
			EACServer.connection2client.Clear();
			EACServer.connection2status.Clear();
			if (EACServer.Interface != null)
			{
				Debug.Log("EasyAntiCheat Server Shutting Down");
				EndSessionOptions endSessionOptions = default(EndSessionOptions);
				EACServer.Interface.EndSession(ref endSessionOptions);
				EACServer.Interface = null;
			}
			EOS.Shutdown();
			return;
		}
		EACServer.client2connection.Clear();
		EACServer.connection2client.Clear();
		EACServer.connection2status.Clear();
	}

	// Token: 0x06003230 RID: 12848 RVA: 0x0013444C File Offset: 0x0013264C
	public static void OnLeaveGame(Connection connection)
	{
		if (ConVar.Server.secure && !Application.isEditor)
		{
			if (EACServer.Interface != null)
			{
				IntPtr client = EACServer.GetClient(connection);
				if (client != IntPtr.Zero)
				{
					UnregisterClientOptions unregisterClientOptions = new UnregisterClientOptions
					{
						ClientHandle = client
					};
					EACServer.Interface.UnregisterClient(ref unregisterClientOptions);
					Connection connection2;
					EACServer.client2connection.TryRemove((uint)((int)client), out connection2);
				}
				uint num;
				EACServer.connection2client.TryRemove(connection, out num);
				AntiCheatCommonClientAuthStatus antiCheatCommonClientAuthStatus;
				EACServer.connection2status.TryRemove(connection, out antiCheatCommonClientAuthStatus);
				return;
			}
		}
		else
		{
			AntiCheatCommonClientAuthStatus antiCheatCommonClientAuthStatus;
			EACServer.connection2status.TryRemove(connection, out antiCheatCommonClientAuthStatus);
		}
	}

	// Token: 0x06003231 RID: 12849 RVA: 0x001344E8 File Offset: 0x001326E8
	public static void OnJoinGame(Connection connection)
	{
		if (ConVar.Server.secure && !Application.isEditor)
		{
			if (EACServer.Interface != null)
			{
				IntPtr intPtr = EACServer.GenerateCompatibilityClient();
				if (intPtr == IntPtr.Zero)
				{
					Debug.LogError("[EAC] GenerateCompatibilityClient returned invalid client: " + intPtr.ToString());
					return;
				}
				RegisterClientOptions registerClientOptions = new RegisterClientOptions
				{
					ClientHandle = intPtr,
					AccountId = connection.userid.ToString(),
					IpAddress = connection.IPAddressWithoutPort(),
					ClientType = ((connection.authLevel >= 3U && connection.os == "editor") ? AntiCheatCommonClientType.UnprotectedClient : AntiCheatCommonClientType.ProtectedClient),
					ClientPlatform = ((connection.os == "windows") ? AntiCheatCommonClientPlatform.Windows : ((connection.os == "linux") ? AntiCheatCommonClientPlatform.Linux : ((connection.os == "mac") ? AntiCheatCommonClientPlatform.Mac : AntiCheatCommonClientPlatform.Unknown)))
				};
				EACServer.Interface.RegisterClient(ref registerClientOptions);
				SetClientDetailsOptions setClientDetailsOptions = new SetClientDetailsOptions
				{
					ClientHandle = intPtr,
					ClientFlags = ((connection.authLevel > 0U) ? AntiCheatCommonClientFlags.Admin : AntiCheatCommonClientFlags.None)
				};
				EACServer.Interface.SetClientDetails(ref setClientDetailsOptions);
				EACServer.client2connection.TryAdd((uint)((int)intPtr), connection);
				EACServer.connection2client.TryAdd(connection, (uint)((int)intPtr));
				EACServer.connection2status.TryAdd(connection, AntiCheatCommonClientAuthStatus.Invalid);
				return;
			}
		}
		else
		{
			EACServer.connection2status.TryAdd(connection, AntiCheatCommonClientAuthStatus.Invalid);
			EACServer.OnAuthenticatedLocal(connection);
			EACServer.OnAuthenticatedRemote(connection);
		}
	}

	// Token: 0x06003232 RID: 12850 RVA: 0x00134678 File Offset: 0x00132878
	public static void OnStartLoading(Connection connection)
	{
		if (EACServer.Interface != null)
		{
			IntPtr client = EACServer.GetClient(connection);
			if (client != IntPtr.Zero)
			{
				SetClientNetworkStateOptions setClientNetworkStateOptions = new SetClientNetworkStateOptions
				{
					ClientHandle = client,
					IsNetworkActive = false
				};
				EACServer.Interface.SetClientNetworkState(ref setClientNetworkStateOptions);
			}
		}
	}

	// Token: 0x06003233 RID: 12851 RVA: 0x001346D0 File Offset: 0x001328D0
	public static void OnFinishLoading(Connection connection)
	{
		if (EACServer.Interface != null)
		{
			IntPtr client = EACServer.GetClient(connection);
			if (client != IntPtr.Zero)
			{
				SetClientNetworkStateOptions setClientNetworkStateOptions = new SetClientNetworkStateOptions
				{
					ClientHandle = client,
					IsNetworkActive = true
				};
				EACServer.Interface.SetClientNetworkState(ref setClientNetworkStateOptions);
			}
		}
	}

	// Token: 0x06003234 RID: 12852 RVA: 0x00134728 File Offset: 0x00132928
	public static void OnMessageReceived(Message message)
	{
		IntPtr client = EACServer.GetClient(message.connection);
		if (client == IntPtr.Zero)
		{
			Debug.LogError("EAC network packet from invalid connection: " + message.connection.userid);
			return;
		}
		byte[] array;
		int count;
		if (!message.read.TemporaryBytesWithSize(out array, out count))
		{
			return;
		}
		ReceiveMessageFromClientOptions receiveMessageFromClientOptions = new ReceiveMessageFromClientOptions
		{
			ClientHandle = client,
			Data = new ArraySegment<byte>(array, 0, count)
		};
		EACServer.Interface.ReceiveMessageFromClient(ref receiveMessageFromClientOptions);
	}

	// Token: 0x040028B7 RID: 10423
	public static AntiCheatServerInterface Interface = null;

	// Token: 0x040028B8 RID: 10424
	public static ReportsInterface Reports = null;

	// Token: 0x040028B9 RID: 10425
	private static ConcurrentDictionary<uint, Connection> client2connection = new ConcurrentDictionary<uint, Connection>();

	// Token: 0x040028BA RID: 10426
	private static ConcurrentDictionary<Connection, uint> connection2client = new ConcurrentDictionary<Connection, uint>();

	// Token: 0x040028BB RID: 10427
	private static ConcurrentDictionary<Connection, AntiCheatCommonClientAuthStatus> connection2status = new ConcurrentDictionary<Connection, AntiCheatCommonClientAuthStatus>();

	// Token: 0x040028BC RID: 10428
	private static uint clientHandleCounter = 0U;
}
