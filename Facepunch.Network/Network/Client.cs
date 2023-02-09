using System;
using Facepunch;
using Network.Visibility;

namespace Network
{
	// Token: 0x0200000A RID: 10
	public class Client : BaseNetwork
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600003E RID: 62 RVA: 0x00002B52 File Offset: 0x00000D52
		protected override float MaxReceiveTimeValue
		{
			get
			{
				return Client.MaxReceiveTime;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600003F RID: 63 RVA: 0x00002B59 File Offset: 0x00000D59
		protected override int MaxReadQueueValue
		{
			get
			{
				return Client.MaxReadQueue;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000040 RID: 64 RVA: 0x00002B60 File Offset: 0x00000D60
		protected override int MaxWriteQueueValue
		{
			get
			{
				return Client.MaxWriteQueue;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000041 RID: 65 RVA: 0x00002B67 File Offset: 0x00000D67
		protected override int MaxDecryptQueueValue
		{
			get
			{
				return Client.MaxDecryptQueue;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000042 RID: 66 RVA: 0x00002B6E File Offset: 0x00000D6E
		// (set) Token: 0x06000043 RID: 67 RVA: 0x00002B76 File Offset: 0x00000D76
		public Connection Connection { get; protected set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000044 RID: 68 RVA: 0x00002B7F File Offset: 0x00000D7F
		public virtual bool IsPlaying { get; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000045 RID: 69 RVA: 0x00002B87 File Offset: 0x00000D87
		// (set) Token: 0x06000046 RID: 70 RVA: 0x00002B8F File Offset: 0x00000D8F
		public string ConnectedAddress { get; set; } = "unset";

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000047 RID: 71 RVA: 0x00002B98 File Offset: 0x00000D98
		// (set) Token: 0x06000048 RID: 72 RVA: 0x00002BA0 File Offset: 0x00000DA0
		public int ConnectedPort { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000049 RID: 73 RVA: 0x00002BA9 File Offset: 0x00000DA9
		// (set) Token: 0x0600004A RID: 74 RVA: 0x00002BB1 File Offset: 0x00000DB1
		public string ServerName { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600004B RID: 75 RVA: 0x00002BBA File Offset: 0x00000DBA
		// (set) Token: 0x0600004C RID: 76 RVA: 0x00002BC2 File Offset: 0x00000DC2
		public bool IsOfficialServer { get; set; }

		// Token: 0x0600004D RID: 77 RVA: 0x00002BCB File Offset: 0x00000DCB
		public virtual bool Connect(string strURL, int port)
		{
			Client.disconnectReason = "Disconnected";
			return true;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00002209 File Offset: 0x00000409
		public virtual void Flush()
		{
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00002209 File Offset: 0x00000409
		public virtual void Disconnect(string reason, bool sendReasonToServer = true)
		{
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00002BD8 File Offset: 0x00000DD8
		protected void OnDisconnected(string str)
		{
			if (this.callbackHandler != null)
			{
				this.callbackHandler.OnClientDisconnected(str);
			}
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00002BEE File Offset: 0x00000DEE
		public Networkable CreateNetworkable(uint networkID, uint networkGroup)
		{
			Networkable networkable = Pool.Get<Networkable>();
			networkable.ID = networkID;
			networkable.SwitchGroup(this.visibility.Get(networkGroup));
			return networkable;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00002C0F File Offset: 0x00000E0F
		public void DestroyNetworkable(ref Networkable networkable)
		{
			networkable.Destroy();
			Pool.Free<Networkable>(ref networkable);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00002C1E File Offset: 0x00000E1E
		public void SetupNetworkable(Networkable net)
		{
			net.cl = this;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x000021A0 File Offset: 0x000003A0
		public virtual int GetLastPing()
		{
			return 0;
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000055 RID: 85 RVA: 0x00002C27 File Offset: 0x00000E27
		public bool IsRecording
		{
			get
			{
				return this.Connection != null && this.Connection.IsRecording;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000056 RID: 86 RVA: 0x00002C3E File Offset: 0x00000E3E
		public string RecordFilename
		{
			get
			{
				return this.Connection.RecordFilename;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00002C4B File Offset: 0x00000E4B
		public TimeSpan RecordTimeElapsed
		{
			get
			{
				return this.Connection.RecordTimeElapsed;
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00002C58 File Offset: 0x00000E58
		public bool StartRecording(string targetFilename, IDemoHeader header)
		{
			return this.Connection.StartRecording(targetFilename, header);
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00002C67 File Offset: 0x00000E67
		public void StopRecording()
		{
			this.Connection.StopRecording();
		}

		// Token: 0x04000031 RID: 49
		public static float MaxReceiveTime = 10f;

		// Token: 0x04000032 RID: 50
		public static float MinReceiveFraction = 0.05f;

		// Token: 0x04000033 RID: 51
		public static int MaxReadQueue = 1000;

		// Token: 0x04000034 RID: 52
		public static int MaxWriteQueue = 1000;

		// Token: 0x04000035 RID: 53
		public static int MaxDecryptQueue = 1000;

		// Token: 0x04000038 RID: 56
		public Manager visibility;

		// Token: 0x04000039 RID: 57
		public static string disconnectReason;

		// Token: 0x0400003E RID: 62
		public Stats IncomingStats = new Stats();

		// Token: 0x0400003F RID: 63
		public IClientCallback callbackHandler;
	}
}
