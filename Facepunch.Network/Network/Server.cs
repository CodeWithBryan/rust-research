using System;
using System.Collections.Generic;
using Facepunch;
using Network.Visibility;
using UnityEngine;

namespace Network
{
	// Token: 0x02000019 RID: 25
	public abstract class Server : BaseNetwork
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000F3 RID: 243
		protected override float MaxReceiveTimeValue
		{
			get
			{
				return Server.MaxReceiveTime;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000F4 RID: 244
		protected override int MaxReadQueueValue
		{
			get
			{
				return Server.MaxReadQueue;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000F5 RID: 245
		protected override int MaxWriteQueueValue
		{
			get
			{
				return Server.MaxWriteQueue;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000F6 RID: 246
		protected override int MaxDecryptQueueValue
		{
			get
			{
				return Server.MaxDecryptQueue;
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000F7 RID: 247
		public virtual string ProtocolId
		{
			get
			{
				return "none";
			}
		}

		// Token: 0x060000F8 RID: 248
		public void Reset()
		{
			this.ResetUIDs();
		}

		// Token: 0x060000F9 RID: 249
		public virtual bool Start()
		{
			return true;
		}

		// Token: 0x060000FA RID: 250
		public virtual void Stop(string shutdownMsg)
		{
		}

		// Token: 0x060000FB RID: 251
		public virtual void Flush(Connection cn)
		{
		}

		// Token: 0x060000FC RID: 252
		public abstract void Disconnect(Connection cn);

		// Token: 0x060000FD RID: 253
		public abstract void Kick(Connection cn, string message, bool logfile = false);

		// Token: 0x060000FE RID: 254
		public uint TakeUID()
		{
			if (this.lastValueGiven > 4294967263U)
			{
				Debug.LogError("TakeUID - hitting ceiling limit!" + this.lastValueGiven);
			}
			this.lastValueGiven += 1U;
			return this.lastValueGiven;
		}

		// Token: 0x060000FF RID: 255
		public void ReturnUID(uint uid)
		{
		}

		// Token: 0x06000100 RID: 256
		public void RegisterUID(uint uid)
		{
			if (uid > this.lastValueGiven)
			{
				this.lastValueGiven = uid;
			}
		}

		// Token: 0x06000101 RID: 257
		internal void ResetUIDs()
		{
			this.lastValueGiven = 1024U;
		}

		// Token: 0x06000102 RID: 258
		public Networkable CreateNetworkable()
		{
			Networkable networkable = Pool.Get<Networkable>();
			networkable.ID = this.TakeUID();
			networkable.sv = this;
			return networkable;
		}

		// Token: 0x06000103 RID: 259
		public Networkable CreateNetworkable(uint uid)
		{
			Networkable networkable = Pool.Get<Networkable>();
			networkable.ID = uid;
			networkable.sv = this;
			this.RegisterUID(uid);
			return networkable;
		}

		// Token: 0x06000104 RID: 260
		public void DestroyNetworkable(ref Networkable networkable)
		{
			networkable.Destroy();
			Pool.Free<Networkable>(ref networkable);
		}

		// Token: 0x06000105 RID: 261
		protected void OnDisconnected(string strReason, Connection cn)
		{
			if (cn == null)
			{
				return;
			}
			cn.connected = false;
			cn.active = false;
			if (this.callbackHandler != null)
			{
				this.callbackHandler.OnDisconnected(strReason, cn);
			}
			this.RemoveConnection(cn);
		}

		// Token: 0x06000106 RID: 262
		protected Connection FindConnection(ulong guid)
		{
			Connection result;
			if (this.connectionByGUID.TryGetValue(guid, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x06000107 RID: 263
		protected virtual void OnNewConnection(ulong guid, string ipaddress)
		{
			Debug.Log("OnNewConnection 1");
			if (string.IsNullOrEmpty(ipaddress) || ipaddress == "UNASSIGNED_SYSTEM_ADDRESS")
			{
				return;
			}
			this.OnNewConnection(new Connection
			{
				guid = guid,
				ipaddress = ipaddress,
				active = true
			});
		}

		// Token: 0x06000108 RID: 264
		protected virtual void OnNewConnection(Connection connection)
		{
			Debug.Log("OnNewConnection 2");
			connection.connectionTime = TimeEx.realtimeSinceStartup;
			this.connections.Add(connection);
			this.connectionByGUID.Add(connection.guid, connection);
			if (this.LimitConnectionsPerIP())
			{
				string key = connection.IPAddressWithoutPort();
				List<Connection> list;
				if (!this.connectionsByIP.TryGetValue(key, out list))
				{
					this.connectionsByIP.Add(key, list = Pool.GetList<Connection>());
				}
				list.Add(connection);
				if (list.Count > Server.MaxConnectionsPerIP)
				{
					this.Kick(connection, "Too many connections from this IP", false);
					return;
				}
			}
			NetWrite netWrite = base.StartWrite();
			netWrite.PacketID(Message.Type.RequestUserInformation);
			netWrite.Send(new SendInfo(connection));
		}

		// Token: 0x06000109 RID: 265
		protected void RemoveConnection(Connection connection)
		{
			if (this.LimitConnectionsPerIP())
			{
				string key = connection.IPAddressWithoutPort();
				List<Connection> list;
				if (this.connectionsByIP.TryGetValue(key, out list))
				{
					list.Remove(connection);
				}
				if (list != null && list.Count == 0)
				{
					this.connectionsByIP.Remove(key);
					Pool.FreeList<Connection>(ref list);
				}
			}
			this.connectionByGUID.Remove(connection.guid);
			this.connections.Remove(connection);
			connection.OnDisconnected();
		}

		// Token: 0x0600010A RID: 266
		public virtual bool LimitConnectionsPerIP()
		{
			return true;
		}

		// Token: 0x0600010B RID: 267
		public virtual int GetAveragePing(Connection connection)
		{
			return 0;
		}

		// Token: 0x04000074 RID: 116
		public static ulong MaxPacketsPerSecond = 1500UL;

		// Token: 0x04000075 RID: 117
		public static int MaxPacketSize = 10000000;

		// Token: 0x04000076 RID: 118
		public static int MaxConnectionsPerIP = 5;

		// Token: 0x04000077 RID: 119
		public static float MaxReceiveTime = 20f;

		// Token: 0x04000078 RID: 120
		public static int MaxReadQueue = 1000;

		// Token: 0x04000079 RID: 121
		public static int MaxWriteQueue = 1000;

		// Token: 0x0400007A RID: 122
		public static int MaxDecryptQueue = 1000;

		// Token: 0x0400007B RID: 123
		public string ip = "";

		// Token: 0x0400007C RID: 124
		public int port = 5678;

		// Token: 0x0400007D RID: 125
		public bool compressionEnabled;

		// Token: 0x0400007E RID: 126
		public bool logging;

		// Token: 0x0400007F RID: 127
		public Manager visibility;

		// Token: 0x04000080 RID: 128
		public IServerCallback callbackHandler;

		// Token: 0x04000081 RID: 129
		public bool debug;

		// Token: 0x04000082 RID: 130
		internal uint lastValueGiven = 1024U;

		// Token: 0x04000083 RID: 131
		public List<Connection> connections = new List<Connection>();

		// Token: 0x04000084 RID: 132
		private Dictionary<ulong, Connection> connectionByGUID = new Dictionary<ulong, Connection>();

		// Token: 0x04000085 RID: 133
		private Dictionary<string, List<Connection>> connectionsByIP = new Dictionary<string, List<Connection>>();
	}
}
