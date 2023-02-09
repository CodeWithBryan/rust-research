using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using CompanionServer.Handlers;
using ConVar;
using Facepunch;
using Fleck;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer
{
	// Token: 0x020009B1 RID: 2481
	public class Listener : IDisposable, IBroadcastSender<Connection, AppBroadcast>
	{
		// Token: 0x06003AC9 RID: 15049 RVA: 0x00158B94 File Offset: 0x00156D94
		public Listener(IPAddress ipAddress, int port)
		{
			this.Address = ipAddress;
			this.Port = port;
			this.Limiter = new ConnectionLimiter();
			this._ipTokenBuckets = new TokenBucketList<IPAddress>(50.0, 15.0);
			this._ipBans = new BanList<IPAddress>();
			this._playerTokenBuckets = new TokenBucketList<ulong>(25.0, 3.0);
			this._pairingTokenBuckets = new TokenBucketList<ulong>(5.0, 0.1);
			this._messageQueue = new Queue<Listener.Message>();
			this._server = new WebSocketServer(string.Format("ws://{0}:{1}/", this.Address, this.Port), true);
			this._server.Start(delegate(IWebSocketConnection socket)
			{
				IPAddress address = socket.ConnectionInfo.ClientIpAddress;
				if (!this.Limiter.TryAdd(address) || this._ipBans.IsBanned(address))
				{
					socket.Close();
					return;
				}
				Connection conn = new Connection(this, socket);
				socket.OnClose = delegate()
				{
					this.Limiter.Remove(address);
					conn.OnClose();
				};
				socket.OnBinary = new BinaryDataHandler(conn.OnMessage);
				socket.OnError = new Action<Exception>(UnityEngine.Debug.LogError);
			});
			this._stopwatch = new Stopwatch();
			this.PlayerSubscribers = new SubscriberList<PlayerTarget, Connection, AppBroadcast>(this);
			this.EntitySubscribers = new SubscriberList<EntityTarget, Connection, AppBroadcast>(this);
		}

		// Token: 0x06003ACA RID: 15050 RVA: 0x00158C8E File Offset: 0x00156E8E
		public void Dispose()
		{
			WebSocketServer server = this._server;
			if (server == null)
			{
				return;
			}
			server.Dispose();
		}

		// Token: 0x06003ACB RID: 15051 RVA: 0x00158CA0 File Offset: 0x00156EA0
		internal void Enqueue(Connection connection, MemoryBuffer data)
		{
			Queue<Listener.Message> messageQueue = this._messageQueue;
			lock (messageQueue)
			{
				if (!App.update || this._messageQueue.Count >= App.queuelimit)
				{
					data.Dispose();
				}
				else
				{
					Listener.Message item = new Listener.Message(connection, data);
					this._messageQueue.Enqueue(item);
				}
			}
		}

		// Token: 0x06003ACC RID: 15052 RVA: 0x00158D14 File Offset: 0x00156F14
		public void Update()
		{
			if (!App.update)
			{
				return;
			}
			using (TimeWarning.New("CompanionServer.MessageQueue", 0))
			{
				Queue<Listener.Message> messageQueue = this._messageQueue;
				lock (messageQueue)
				{
					this._stopwatch.Restart();
					while (this._messageQueue.Count > 0 && this._stopwatch.Elapsed.TotalMilliseconds < 5.0)
					{
						Listener.Message message = this._messageQueue.Dequeue();
						this.Dispatch(message);
					}
				}
			}
			if (this._lastCleanup >= 3f)
			{
				this._lastCleanup = 0f;
				this._ipTokenBuckets.Cleanup();
				this._ipBans.Cleanup();
				this._playerTokenBuckets.Cleanup();
				this._pairingTokenBuckets.Cleanup();
			}
		}

		// Token: 0x06003ACD RID: 15053 RVA: 0x00158E14 File Offset: 0x00157014
		private void Dispatch(Listener.Message message)
		{
			Listener.<>c__DisplayClass19_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.message = message;
			using (CS$<>8__locals1.message.Buffer)
			{
				try
				{
					Listener.Stream.SetData(CS$<>8__locals1.message.Buffer.Data, 0, CS$<>8__locals1.message.Buffer.Length);
					CS$<>8__locals1.request = AppRequest.Deserialize(Listener.Stream);
				}
				catch
				{
					DebugEx.LogWarning(string.Format("Malformed companion packet from {0}", CS$<>8__locals1.message.Connection.Address), StackTraceLogType.None);
					CS$<>8__locals1.message.Connection.Close();
					throw;
				}
			}
			CompanionServer.Handlers.IHandler handler;
			if (!this.<Dispatch>g__Handle|19_12<AppEmpty, Info>((AppRequest r) => r.getInfo, out handler, ref CS$<>8__locals1))
			{
				if (!this.<Dispatch>g__Handle|19_12<AppEmpty, CompanionServer.Handlers.Time>((AppRequest r) => r.getTime, out handler, ref CS$<>8__locals1))
				{
					if (!this.<Dispatch>g__Handle|19_12<AppEmpty, Map>((AppRequest r) => r.getMap, out handler, ref CS$<>8__locals1))
					{
						if (!this.<Dispatch>g__Handle|19_12<AppEmpty, TeamInfo>((AppRequest r) => r.getTeamInfo, out handler, ref CS$<>8__locals1))
						{
							if (!this.<Dispatch>g__Handle|19_12<AppEmpty, TeamChat>((AppRequest r) => r.getTeamChat, out handler, ref CS$<>8__locals1))
							{
								if (!this.<Dispatch>g__Handle|19_12<AppSendMessage, SendTeamChat>((AppRequest r) => r.sendTeamMessage, out handler, ref CS$<>8__locals1))
								{
									if (!this.<Dispatch>g__Handle|19_12<AppEmpty, EntityInfo>((AppRequest r) => r.getEntityInfo, out handler, ref CS$<>8__locals1))
									{
										if (!this.<Dispatch>g__Handle|19_12<AppSetEntityValue, SetEntityValue>((AppRequest r) => r.setEntityValue, out handler, ref CS$<>8__locals1))
										{
											if (!this.<Dispatch>g__Handle|19_12<AppEmpty, CheckSubscription>((AppRequest r) => r.checkSubscription, out handler, ref CS$<>8__locals1))
											{
												if (!this.<Dispatch>g__Handle|19_12<AppFlag, SetSubscription>((AppRequest r) => r.setSubscription, out handler, ref CS$<>8__locals1))
												{
													if (!this.<Dispatch>g__Handle|19_12<AppEmpty, MapMarkers>((AppRequest r) => r.getMapMarkers, out handler, ref CS$<>8__locals1))
													{
														if (!this.<Dispatch>g__Handle|19_12<AppPromoteToLeader, PromoteToLeader>((AppRequest r) => r.promoteToLeader, out handler, ref CS$<>8__locals1))
														{
															AppResponse appResponse = Facepunch.Pool.Get<AppResponse>();
															appResponse.seq = CS$<>8__locals1.request.seq;
															appResponse.error = Facepunch.Pool.Get<AppError>();
															appResponse.error.error = "unhandled";
															CS$<>8__locals1.message.Connection.Send(appResponse);
															CS$<>8__locals1.request.Dispose();
															return;
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			try
			{
				ValidationResult validationResult = handler.Validate();
				if (validationResult == ValidationResult.Rejected)
				{
					CS$<>8__locals1.message.Connection.Close();
				}
				else if (validationResult != ValidationResult.Success)
				{
					handler.SendError(validationResult.ToErrorCode());
				}
				else
				{
					handler.Execute();
				}
			}
			catch (Exception arg)
			{
				UnityEngine.Debug.LogError(string.Format("AppRequest threw an exception: {0}", arg));
				handler.SendError("server_error");
			}
			Facepunch.Pool.FreeDynamic<CompanionServer.Handlers.IHandler>(ref handler);
		}

		// Token: 0x06003ACE RID: 15054 RVA: 0x001591BC File Offset: 0x001573BC
		public void BroadcastTo(List<Connection> targets, AppBroadcast broadcast)
		{
			MemoryBuffer broadcastBuffer = Listener.GetBroadcastBuffer(broadcast);
			foreach (Connection connection in targets)
			{
				connection.SendRaw(broadcastBuffer.DontDispose());
			}
			broadcastBuffer.Dispose();
		}

		// Token: 0x06003ACF RID: 15055 RVA: 0x0015921C File Offset: 0x0015741C
		private static MemoryBuffer GetBroadcastBuffer(AppBroadcast broadcast)
		{
			MemoryBuffer memoryBuffer = new MemoryBuffer(65536);
			Listener.Stream.SetData(memoryBuffer.Data, 0, memoryBuffer.Length);
			AppMessage appMessage = Facepunch.Pool.Get<AppMessage>();
			appMessage.broadcast = broadcast;
			appMessage.ToProto(Listener.Stream);
			if (appMessage.ShouldPool)
			{
				appMessage.Dispose();
			}
			return memoryBuffer.Slice((int)Listener.Stream.Position);
		}

		// Token: 0x06003AD0 RID: 15056 RVA: 0x00159286 File Offset: 0x00157486
		public bool CanSendPairingNotification(ulong playerId)
		{
			return this._pairingTokenBuckets.Get(playerId).TryTake(1.0);
		}

		// Token: 0x06003AD3 RID: 15059 RVA: 0x00159350 File Offset: 0x00157550
		[CompilerGenerated]
		private bool <Dispatch>g__Handle|19_12<TProto, THandler>(Func<AppRequest, TProto> protoSelector, out CompanionServer.Handlers.IHandler requestHandler, ref Listener.<>c__DisplayClass19_0 A_3) where TProto : class where THandler : BaseHandler<TProto>, new()
		{
			TProto tproto = protoSelector(A_3.request);
			if (tproto == null)
			{
				requestHandler = null;
				return false;
			}
			THandler thandler = Facepunch.Pool.Get<THandler>();
			thandler.Initialize(this._playerTokenBuckets, A_3.message.Connection, A_3.request, tproto);
			requestHandler = thandler;
			return true;
		}

		// Token: 0x040034E9 RID: 13545
		private static readonly ByteArrayStream Stream = new ByteArrayStream();

		// Token: 0x040034EA RID: 13546
		private readonly TokenBucketList<IPAddress> _ipTokenBuckets;

		// Token: 0x040034EB RID: 13547
		private readonly BanList<IPAddress> _ipBans;

		// Token: 0x040034EC RID: 13548
		private readonly TokenBucketList<ulong> _playerTokenBuckets;

		// Token: 0x040034ED RID: 13549
		private readonly TokenBucketList<ulong> _pairingTokenBuckets;

		// Token: 0x040034EE RID: 13550
		private readonly Queue<Listener.Message> _messageQueue;

		// Token: 0x040034EF RID: 13551
		private readonly WebSocketServer _server;

		// Token: 0x040034F0 RID: 13552
		private readonly Stopwatch _stopwatch;

		// Token: 0x040034F1 RID: 13553
		private RealTimeSince _lastCleanup;

		// Token: 0x040034F2 RID: 13554
		public readonly IPAddress Address;

		// Token: 0x040034F3 RID: 13555
		public readonly int Port;

		// Token: 0x040034F4 RID: 13556
		public readonly ConnectionLimiter Limiter;

		// Token: 0x040034F5 RID: 13557
		public readonly SubscriberList<PlayerTarget, Connection, AppBroadcast> PlayerSubscribers;

		// Token: 0x040034F6 RID: 13558
		public readonly SubscriberList<EntityTarget, Connection, AppBroadcast> EntitySubscribers;

		// Token: 0x02000E8B RID: 3723
		private struct Message
		{
			// Token: 0x060050EB RID: 20715 RVA: 0x001A30C0 File Offset: 0x001A12C0
			public Message(Connection connection, MemoryBuffer buffer)
			{
				this.Connection = connection;
				this.Buffer = buffer;
			}

			// Token: 0x04004AFF RID: 19199
			public readonly Connection Connection;

			// Token: 0x04004B00 RID: 19200
			public readonly MemoryBuffer Buffer;
		}
	}
}
