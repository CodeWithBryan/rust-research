using System;
using Network;
using UnityEngine;

namespace Facepunch.Network.Raknet
{
	// Token: 0x02000005 RID: 5
	public class Server : Server
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600002B RID: 43 RVA: 0x0000265C File Offset: 0x0000085C
		public override string ProtocolId
		{
			get
			{
				return "rak";
			}
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002663 File Offset: 0x00000863
		public override bool IsConnected()
		{
			return this.peer != null;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002670 File Offset: 0x00000870
		public override bool Start()
		{
			object readLock = this.readLock;
			bool result;
			lock (readLock)
			{
				object writeLock = this.writeLock;
				lock (writeLock)
				{
					this.peer = Peer.CreateServer(this.ip, this.port, 1024);
					if (this.peer == null)
					{
						result = false;
					}
					else
					{
						base.MultithreadingInit();
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002708 File Offset: 0x00000908
		public override void Stop(string shutdownMsg)
		{
			object readLock = this.readLock;
			lock (readLock)
			{
				object writeLock = this.writeLock;
				lock (writeLock)
				{
					if (this.peer != null)
					{
						Console.WriteLine("[Raknet] Server Shutting Down (" + shutdownMsg + ")");
						using (TimeWarning.New("ServerStop", 0))
						{
							this.peer.Close();
							this.peer = null;
							base.Stop(shutdownMsg);
						}
					}
				}
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x000027C8 File Offset: 0x000009C8
		public override void Disconnect(Connection cn)
		{
			object readLock = this.readLock;
			lock (readLock)
			{
				object writeLock = this.writeLock;
				lock (writeLock)
				{
					if (this.peer != null)
					{
						this.peer.Kick(cn);
						base.OnDisconnected("Disconnected", cn);
					}
				}
			}
		}

		// Token: 0x06000030 RID: 48 RVA: 0x0000284C File Offset: 0x00000A4C
		public override void Kick(Connection cn, string message, bool logfile)
		{
			object readLock = this.readLock;
			lock (readLock)
			{
				object writeLock = this.writeLock;
				lock (writeLock)
				{
					if (this.peer != null)
					{
						NetWrite netWrite = base.StartWrite();
						netWrite.PacketID(Message.Type.DisconnectReason);
						netWrite.String(message);
						netWrite.Send(new SendInfo(cn)
						{
							method = SendMethod.ReliableUnordered,
							priority = Priority.Immediate
						});
						string text = cn.ToString() + " kicked: " + message;
						if (logfile)
						{
							DebugEx.LogWarning(text, StackTraceLogType.None);
						}
						else
						{
							Console.WriteLine(text);
						}
						this.peer.Kick(cn);
						base.OnDisconnected("Kicked: " + message, cn);
					}
				}
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002934 File Offset: 0x00000B34
		internal bool HandleRaknetPacket(byte type, NetRead read)
		{
			if (type >= 140)
			{
				return false;
			}
			switch (type)
			{
			case 19:
				using (TimeWarning.New("OnNewConnection", 20))
				{
					this.OnNewConnection(read.guid, read.ipaddress);
				}
				return true;
			case 21:
				if (read.connection != null)
				{
					using (TimeWarning.New("OnDisconnected", 20))
					{
						base.OnDisconnected("Disconnected", read.connection);
					}
				}
				return true;
			case 22:
				if (read.connection != null)
				{
					using (TimeWarning.New("OnDisconnected (timed out)", 20))
					{
						base.OnDisconnected("Timed Out", read.connection);
					}
				}
				return true;
			}
			return true;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002A28 File Offset: 0x00000C28
		private void HandleMessage()
		{
			if (this.peer.incomingBytesUnread > Server.MaxPacketSize)
			{
				return;
			}
			Connection connection = base.FindConnection(this.peer.incomingGUID);
			if (connection != null)
			{
				if (connection.GetPacketsPerSecond(0) >= Server.MaxPacketsPerSecond)
				{
					this.Kick(connection, "Packet Flooding", connection.connected);
					return;
				}
				connection.AddPacketsPerSecond(0);
			}
			NetRead netRead = Pool.Get<NetRead>();
			if (connection != null)
			{
				netRead.Start(connection, this.peer.RawData(), this.peer.incomingBytesUnread);
			}
			else
			{
				netRead.Start(this.peer.incomingGUID, this.peer.incomingAddress, this.peer.RawData(), this.peer.incomingBytesUnread);
			}
			if (BaseNetwork.Multithreading)
			{
				base.EnqueueDecrypt(netRead);
				return;
			}
			base.ProcessDecrypt(netRead);
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002AF8 File Offset: 0x00000CF8
		public override void ProcessRead(NetRead read)
		{
			byte b = read.PacketID();
			if (this.HandleRaknetPacket(b, read))
			{
				Pool.Free<NetRead>(ref read);
				return;
			}
			if (read.connection != null)
			{
				b -= 140;
				Message message = base.StartMessage((Message.Type)b, read);
				if (this.callbackHandler != null)
				{
					this.callbackHandler.OnNetworkMessage(message);
				}
				message.Clear();
				Pool.Free<Message>(ref message);
			}
			Pool.Free<NetRead>(ref read);
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002B60 File Offset: 0x00000D60
		protected override bool Receive()
		{
			if (this.peer.Receive())
			{
				this.HandleMessage();
				return true;
			}
			return false;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002B78 File Offset: 0x00000D78
		public override string GetDebug(Connection connection)
		{
			if (this.peer == null)
			{
				return string.Empty;
			}
			if (connection == null)
			{
				return this.peer.GetStatisticsString(0UL);
			}
			return this.peer.GetStatisticsString(connection.guid);
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002BAA File Offset: 0x00000DAA
		public override int GetAveragePing(Connection connection)
		{
			if (this.peer == null)
			{
				return 0;
			}
			return this.peer.GetPingAverage(connection.guid);
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002BC7 File Offset: 0x00000DC7
		public override ulong GetStat(Connection connection, BaseNetwork.StatTypeLong type)
		{
			if (this.peer == null)
			{
				return 0UL;
			}
			return this.peer.GetStat(connection, type);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00002BE4 File Offset: 0x00000DE4
		public override void ProcessWrite(NetWrite write)
		{
			foreach (Connection connection in write.connections)
			{
				this.ProcessWrite(write, connection);
			}
			Pool.Free<NetWrite>(ref write);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002C40 File Offset: 0x00000E40
		private void ProcessWrite(NetWrite write, Connection connection)
		{
			base.Record(connection, write);
			ArraySegment<byte> arraySegment = base.Encrypt(connection, write);
			this.peer.SendStart();
			this.peer.WriteBytes(arraySegment.Array, 0, arraySegment.Offset + arraySegment.Count);
			this.peer.SendTo(connection.guid, write.priority, write.method, write.channel);
		}

		// Token: 0x04000009 RID: 9
		private Peer peer;
	}
}
