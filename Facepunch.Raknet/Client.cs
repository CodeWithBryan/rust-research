using System;
using Network;
using UnityEngine;

namespace Facepunch.Network.Raknet
{
	// Token: 0x02000003 RID: 3
	public class Client : Client
	{
		// Token: 0x0600001F RID: 31 RVA: 0x00002058 File Offset: 0x00000258
		public override bool IsConnected()
		{
			return this.peer != null;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002064 File Offset: 0x00000264
		public override bool Connect(string strURL, int port)
		{
			object readLock = this.readLock;
			bool result;
			lock (readLock)
			{
				object writeLock = this.writeLock;
				lock (writeLock)
				{
					base.Connect(strURL, port);
					this.peer = Peer.CreateConnection(strURL, port, 12, 400, 0);
					if (this.peer == null)
					{
						result = false;
					}
					else
					{
						base.ConnectedAddress = strURL;
						base.ConnectedPort = port;
						base.ServerName = "";
						base.Connection = new Connection();
						base.MultithreadingInit();
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002120 File Offset: 0x00000320
		internal bool HandleRaknetPacket(byte type, NetRead read)
		{
			if (type >= 140)
			{
				return false;
			}
			switch (type)
			{
			case 16:
				if (base.Connection.guid != 0UL)
				{
					Console.WriteLine("Multiple PacketType.CONNECTION_REQUEST_ACCEPTED");
				}
				base.Connection.guid = read.guid;
				this.IncomingStats.Add("Unconnected", "RequestAccepted", read.Length);
				return true;
			case 17:
				this.Disconnect("Connection Attempt Failed", false);
				return true;
			case 20:
				this.Disconnect("Server is Full", false);
				return true;
			case 21:
				if (base.Connection != null && base.Connection.guid != read.guid)
				{
					return true;
				}
				this.Disconnect(Client.disconnectReason, false);
				return true;
			case 22:
				if (base.Connection != null && base.Connection.guid != read.guid)
				{
					return true;
				}
				this.Disconnect("Timed Out", false);
				return true;
			case 23:
				if (base.Connection != null && base.Connection.guid != read.guid)
				{
					return true;
				}
				this.Disconnect("Connection Banned", false);
				return true;
			}
			this.IncomingStats.Add("Unconnected", "Unhandled", read.Length);
			if (base.Connection != null && read.guid != base.Connection.guid)
			{
				Debug.LogWarning("[CLIENT] Unhandled Raknet packet " + type + " from unknown source");
				return true;
			}
			Debug.LogWarning("Unhandled Raknet packet " + type);
			return true;
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000022B0 File Offset: 0x000004B0
		protected void HandleMessage()
		{
			NetRead netRead = Pool.Get<NetRead>();
			netRead.Start(base.Connection, this.peer.incomingGUID, this.peer.RawData(), this.peer.incomingBytesUnread);
			if (BaseNetwork.Multithreading)
			{
				base.EnqueueDecrypt(netRead);
				return;
			}
			base.ProcessDecrypt(netRead);
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002308 File Offset: 0x00000508
		public override void ProcessRead(NetRead read)
		{
			base.Record(read.connection, read);
			byte b = read.PacketID();
			if (this.HandleRaknetPacket(b, read))
			{
				Pool.Free<NetRead>(ref read);
				return;
			}
			if (read.connection != null)
			{
				b -= 140;
				if (read.guid != read.connection.guid)
				{
					this.IncomingStats.Add("Error", "WrongGuid", read.Length);
					Pool.Free<NetRead>(ref read);
					return;
				}
				if (b > 25)
				{
					Debug.LogWarning("Invalid Packet (higher than " + Message.Type.ConsoleReplicatedVars + ")");
					this.Disconnect(string.Concat(new object[]
					{
						"Invalid Packet (",
						b,
						") ",
						read.Length,
						"b"
					}), true);
					Pool.Free<NetRead>(ref read);
					return;
				}
				Message message = base.StartMessage((Message.Type)b, read);
				if (this.callbackHandler != null)
				{
					try
					{
						using (TimeWarning.New("OnMessage", 0))
						{
							this.callbackHandler.OnNetworkMessage(message);
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						this.Disconnect(ex.Message + "\n" + ex.StackTrace, true);
					}
				}
				message.Clear();
				Pool.Free<Message>(ref message);
			}
			Pool.Free<NetRead>(ref read);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x0000247C File Offset: 0x0000067C
		protected override bool Receive()
		{
			if (this.peer.Receive())
			{
				this.HandleMessage();
				return true;
			}
			return false;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002494 File Offset: 0x00000694
		public override void Disconnect(string reason, bool sendReasonToServer)
		{
			object readLock = this.readLock;
			lock (readLock)
			{
				object writeLock = this.writeLock;
				lock (writeLock)
				{
					if (sendReasonToServer)
					{
						NetWrite netWrite = base.StartWrite();
						netWrite.PacketID(Message.Type.DisconnectReason);
						netWrite.String(reason);
						netWrite.Send(new SendInfo(base.Connection)
						{
							method = SendMethod.ReliableUnordered,
							priority = Priority.Immediate
						});
					}
					if (this.peer != null)
					{
						this.peer.Close();
						this.peer = null;
					}
					base.ConnectedAddress = "";
					base.ConnectedPort = 0;
					base.Connection = null;
					base.OnDisconnected(reason);
				}
			}
		}

		// Token: 0x06000026 RID: 38 RVA: 0x0000256C File Offset: 0x0000076C
		public override string GetDebug(Connection connection)
		{
			if (this.peer == null)
			{
				return "";
			}
			if (connection == null)
			{
				return this.peer.GetStatisticsString(0UL);
			}
			return this.peer.GetStatisticsString(connection.guid);
		}

		// Token: 0x06000027 RID: 39 RVA: 0x0000259E File Offset: 0x0000079E
		public override ulong GetStat(Connection connection, BaseNetwork.StatTypeLong type)
		{
			if (this.peer == null)
			{
				return 0UL;
			}
			return this.peer.GetStat(connection, type);
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000025B8 File Offset: 0x000007B8
		public override int GetLastPing()
		{
			if (base.Connection == null)
			{
				return 1;
			}
			return this.peer.GetPingLast(base.Connection.guid);
		}

		// Token: 0x06000029 RID: 41 RVA: 0x000025DC File Offset: 0x000007DC
		public override void ProcessWrite(NetWrite write)
		{
			ArraySegment<byte> arraySegment = base.Encrypt(base.Connection, write);
			this.peer.SendStart();
			this.peer.WriteBytes(arraySegment.Array, 0, arraySegment.Offset + arraySegment.Count);
			this.peer.SendTo(base.Connection.guid, write.priority, write.method, write.channel);
			Pool.Free<NetWrite>(ref write);
		}

		// Token: 0x04000001 RID: 1
		private Peer peer;
	}
}
