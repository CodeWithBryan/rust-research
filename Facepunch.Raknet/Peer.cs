using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using Network;
using UnityEngine;

namespace Facepunch.Network.Raknet
{
	// Token: 0x02000006 RID: 6
	[SuppressUnmanagedCodeSecurity]
	internal class Peer
	{
		// Token: 0x0600003B RID: 59 RVA: 0x00002CB8 File Offset: 0x00000EB8
		public static Peer CreateServer(string ip, int port, int maxConnections)
		{
			Peer peer = new Peer();
			peer.ptr = Native.NET_Create();
			if (Native.NET_StartServer(peer.ptr, ip, port, maxConnections) == 0)
			{
				return peer;
			}
			peer.Close();
			string text = Peer.StringFromPointer(Native.NET_LastStartupError(peer.ptr));
			Debug.LogWarning(string.Concat(new object[]
			{
				"Couldn't create server on port ",
				port,
				" (",
				text,
				")"
			}));
			return null;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00002D38 File Offset: 0x00000F38
		public static Peer CreateConnection(string hostname, int port, int retries, int retryDelay, int timeout)
		{
			Peer peer = new Peer();
			peer.ptr = Native.NET_Create();
			if (Native.NET_StartClient(peer.ptr, hostname, port, retries, retryDelay, timeout) == 0)
			{
				return peer;
			}
			string text = Peer.StringFromPointer(Native.NET_LastStartupError(peer.ptr));
			Debug.LogWarning(string.Concat(new object[]
			{
				"Couldn't connect to server ",
				hostname,
				":",
				port,
				" (",
				text,
				")"
			}));
			peer.Close();
			return null;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002DC6 File Offset: 0x00000FC6
		public void Close()
		{
			if (this.ptr != IntPtr.Zero)
			{
				Native.NET_Close(this.ptr);
				this.ptr = IntPtr.Zero;
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00002DF0 File Offset: 0x00000FF0
		public uint PendingReceiveCount()
		{
			if (this.ptr == IntPtr.Zero)
			{
				return 0U;
			}
			return Native.NET_GetReceiveBufferSize(this.ptr);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00002E11 File Offset: 0x00001011
		public bool Receive()
		{
			return !(this.ptr == IntPtr.Zero) && Native.NET_Receive(this.ptr);
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000040 RID: 64 RVA: 0x00002E32 File Offset: 0x00001032
		public virtual ulong incomingGUID
		{
			get
			{
				this.Check();
				return Native.NETRCV_GUID(this.ptr);
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000041 RID: 65 RVA: 0x00002E45 File Offset: 0x00001045
		public virtual uint incomingAddressInt
		{
			get
			{
				this.Check();
				return Native.NETRCV_Address(this.ptr);
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000042 RID: 66 RVA: 0x00002E58 File Offset: 0x00001058
		public virtual uint incomingPort
		{
			get
			{
				this.Check();
				return Native.NETRCV_Port(this.ptr);
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000043 RID: 67 RVA: 0x00002E6B File Offset: 0x0000106B
		public string incomingAddress
		{
			get
			{
				return this.GetAddress(this.incomingGUID);
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000044 RID: 68 RVA: 0x00002E79 File Offset: 0x00001079
		public virtual int incomingBits
		{
			get
			{
				this.Check();
				return Native.NETRCV_LengthBits(this.ptr);
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000045 RID: 69 RVA: 0x00002E8C File Offset: 0x0000108C
		public virtual int incomingBitsUnread
		{
			get
			{
				this.Check();
				return Native.NETRCV_UnreadBits(this.ptr);
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000046 RID: 70 RVA: 0x00002E9F File Offset: 0x0000109F
		public virtual int incomingBytes
		{
			get
			{
				return this.incomingBits / 8;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000047 RID: 71 RVA: 0x00002EA9 File Offset: 0x000010A9
		public virtual int incomingBytesUnread
		{
			get
			{
				return this.incomingBitsUnread / 8;
			}
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00002EB3 File Offset: 0x000010B3
		public virtual void SetReadPos(int bitsOffset)
		{
			this.Check();
			Native.NETRCV_SetReadPointer(this.ptr, bitsOffset);
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00002EC7 File Offset: 0x000010C7
		protected unsafe virtual bool Read(byte* data, int length)
		{
			this.Check();
			return Native.NETRCV_ReadBytes(this.ptr, data, length);
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00002EDC File Offset: 0x000010DC
		public unsafe int ReadBytes(byte[] buffer, int offset, int length)
		{
			fixed (byte[] array = buffer)
			{
				byte* ptr;
				if (buffer == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				if (!this.Read(ptr + offset, length))
				{
					return 0;
				}
			}
			return length;
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00002F14 File Offset: 0x00001114
		public unsafe byte ReadByte()
		{
			byte[] array;
			byte* data;
			if ((array = Peer.ByteBuffer) == null || array.Length == 0)
			{
				data = null;
			}
			else
			{
				data = &array[0];
			}
			if (!this.Read(data, 1))
			{
				return 0;
			}
			array = null;
			return Peer.ByteBuffer[0];
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00002F52 File Offset: 0x00001152
		public virtual IntPtr RawData()
		{
			this.Check();
			return Native.NETRCV_RawData(this.ptr);
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00002F68 File Offset: 0x00001168
		public unsafe int ReadBytes(MemoryStream memoryStream, int length)
		{
			if (memoryStream.Capacity < length)
			{
				memoryStream.Capacity = length + 32;
			}
			byte[] array;
			byte* data;
			if ((array = memoryStream.GetBuffer()) == null || array.Length == 0)
			{
				data = null;
			}
			else
			{
				data = &array[0];
			}
			memoryStream.SetLength((long)memoryStream.Capacity);
			if (!this.Read(data, length))
			{
				return 0;
			}
			memoryStream.SetLength((long)length);
			array = null;
			return length;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00002FC9 File Offset: 0x000011C9
		public virtual void SendStart()
		{
			this.Check();
			Native.NETSND_Start(this.ptr);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00002FDC File Offset: 0x000011DC
		public unsafe void WriteByte(byte val)
		{
			this.Write(&val, 1);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00002FE8 File Offset: 0x000011E8
		public unsafe void WriteBytes(byte[] val, int offset, int length)
		{
			fixed (byte[] array = val)
			{
				byte* ptr;
				if (val == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				this.Write(ptr + offset, length);
			}
		}

		// Token: 0x06000051 RID: 81 RVA: 0x0000301C File Offset: 0x0000121C
		public unsafe void WriteBytes(byte[] val)
		{
			fixed (byte[] array = val)
			{
				byte* data;
				if (val == null || array.Length == 0)
				{
					data = null;
				}
				else
				{
					data = &array[0];
				}
				this.Write(data, val.Length);
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x0000304D File Offset: 0x0000124D
		public void WriteBytes(MemoryStream stream)
		{
			this.WriteBytes(stream.GetBuffer(), 0, (int)stream.Length);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00003063 File Offset: 0x00001263
		protected unsafe virtual void Write(byte* data, int size)
		{
			if (size == 0)
			{
				return;
			}
			if (data == null)
			{
				return;
			}
			this.Check();
			Native.NETSND_WriteBytes(this.ptr, data, size);
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00003082 File Offset: 0x00001282
		public virtual uint SendBroadcast(Priority priority, SendMethod reliability, sbyte channel)
		{
			this.Check();
			return Native.NETSND_Broadcast(this.ptr, this.ToRaknetPriority(priority), this.ToRaknetPacketReliability(reliability), (int)channel);
		}

		// Token: 0x06000055 RID: 85 RVA: 0x000030A4 File Offset: 0x000012A4
		public virtual uint SendTo(ulong guid, Priority priority, SendMethod reliability, sbyte channel)
		{
			this.Check();
			return Native.NETSND_Send(this.ptr, guid, this.ToRaknetPriority(priority), this.ToRaknetPacketReliability(reliability), (int)channel);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x000030C8 File Offset: 0x000012C8
		public unsafe void SendUnconnectedMessage(byte* data, int length, uint adr, ushort port)
		{
			this.Check();
			Native.NET_SendMessage(this.ptr, data, length, adr, port);
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000030E0 File Offset: 0x000012E0
		public string GetAddress(ulong guid)
		{
			this.Check();
			return Peer.StringFromPointer(Native.NET_GetAddress(this.ptr, guid));
		}

		// Token: 0x06000058 RID: 88 RVA: 0x000030F9 File Offset: 0x000012F9
		private static string StringFromPointer(IntPtr p)
		{
			if (p == IntPtr.Zero)
			{
				return string.Empty;
			}
			return Marshal.PtrToStringAnsi(p);
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003114 File Offset: 0x00001314
		public int ToRaknetPriority(Priority priority)
		{
			if (priority == Priority.Immediate)
			{
				return 0;
			}
			if (priority != Priority.Normal)
			{
				return 2;
			}
			return 2;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00003124 File Offset: 0x00001324
		public int ToRaknetPacketReliability(SendMethod reliability)
		{
			switch (reliability)
			{
			case SendMethod.Reliable:
				return 3;
			case SendMethod.ReliableUnordered:
				return 2;
			case SendMethod.Unreliable:
				return 0;
			default:
				return 3;
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003141 File Offset: 0x00001341
		public void Kick(Connection connection)
		{
			this.Check();
			Native.NET_CloseConnection(this.ptr, connection.guid);
		}

		// Token: 0x0600005C RID: 92 RVA: 0x0000315A File Offset: 0x0000135A
		protected virtual void Check()
		{
			if (this.ptr == IntPtr.Zero)
			{
				throw new NullReferenceException("Peer has already shut down!");
			}
		}

		// Token: 0x0600005D RID: 93 RVA: 0x0000317C File Offset: 0x0000137C
		public virtual string GetStatisticsString(ulong guid)
		{
			this.Check();
			return string.Format("Average Ping:\t\t{0}\nLast Ping:\t\t{1}\nLowest Ping:\t\t{2}\n{3}", new object[]
			{
				this.GetPingAverage(guid),
				this.GetPingLast(guid),
				this.GetPingLowest(guid),
				Peer.StringFromPointer(Native.NET_GetStatisticsString(this.ptr, guid))
			});
		}

		// Token: 0x0600005E RID: 94 RVA: 0x000031E0 File Offset: 0x000013E0
		public virtual int GetPingAverage(ulong guid)
		{
			this.Check();
			return Native.NET_GetAveragePing(this.ptr, guid);
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000031F4 File Offset: 0x000013F4
		public virtual int GetPingLast(ulong guid)
		{
			this.Check();
			return Native.NET_GetLastPing(this.ptr, guid);
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00003208 File Offset: 0x00001408
		public virtual int GetPingLowest(ulong guid)
		{
			this.Check();
			return Native.NET_GetLowestPing(this.ptr, guid);
		}

		// Token: 0x06000061 RID: 97 RVA: 0x0000321C File Offset: 0x0000141C
		public virtual Native.RaknetStats GetStatistics(ulong guid)
		{
			this.Check();
			Native.RaknetStats result = default(Native.RaknetStats);
			int num = sizeof(Native.RaknetStats);
			if (!Native.NET_GetStatistics(this.ptr, guid, ref result, num))
			{
				Debug.Log("NET_GetStatistics:  Wrong size " + num);
			}
			return result;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00003268 File Offset: 0x00001468
		public unsafe virtual ulong GetStat(Connection connection, BaseNetwork.StatTypeLong type)
		{
			this.Check();
			Native.RaknetStats raknetStats = (connection == null) ? this.GetStatistics(0UL) : this.GetStatistics(connection.guid);
			switch (type)
			{
			case BaseNetwork.StatTypeLong.BytesSent:
				return *(ref raknetStats.runningTotal.FixedElementField + (IntPtr)5 * 8);
			case BaseNetwork.StatTypeLong.BytesSent_LastSecond:
				return *(ref raknetStats.valueOverLastSecond.FixedElementField + (IntPtr)5 * 8);
			case BaseNetwork.StatTypeLong.BytesReceived:
				return *(ref raknetStats.runningTotal.FixedElementField + (IntPtr)6 * 8);
			case BaseNetwork.StatTypeLong.BytesReceived_LastSecond:
				return *(ref raknetStats.valueOverLastSecond.FixedElementField + (IntPtr)6 * 8);
			case BaseNetwork.StatTypeLong.BytesInSendBuffer:
				return &raknetStats.bytesInSendBuffer.FixedElementField;
			case BaseNetwork.StatTypeLong.BytesInResendBuffer:
				return raknetStats.bytesInResendBuffer;
			case BaseNetwork.StatTypeLong.PacketLossAverage:
				return (ulong)raknetStats.packetlossTotal * 10000UL;
			case BaseNetwork.StatTypeLong.PacketLossLastSecond:
				return (ulong)raknetStats.packetlossLastSecond * 10000UL;
			case BaseNetwork.StatTypeLong.ThrottleBytes:
				if (raknetStats.isLimitedByCongestionControl == 0)
				{
					return 0UL;
				}
				return raknetStats.BPSLimitByCongestionControl;
			}
			return 0UL;
		}

		// Token: 0x0400000A RID: 10
		private IntPtr ptr;

		// Token: 0x0400000B RID: 11
		private static byte[] ByteBuffer = new byte[512];

		// Token: 0x0200000A RID: 10
		public enum PacketPriority
		{
			// Token: 0x04000029 RID: 41
			IMMEDIATE_PRIORITY,
			// Token: 0x0400002A RID: 42
			HIGH_PRIORITY,
			// Token: 0x0400002B RID: 43
			MEDIUM_PRIORITY,
			// Token: 0x0400002C RID: 44
			LOW_PRIORITY
		}

		// Token: 0x0200000B RID: 11
		public enum PacketReliability
		{
			// Token: 0x0400002E RID: 46
			UNRELIABLE,
			// Token: 0x0400002F RID: 47
			UNRELIABLE_SEQUENCED,
			// Token: 0x04000030 RID: 48
			RELIABLE,
			// Token: 0x04000031 RID: 49
			RELIABLE_ORDERED,
			// Token: 0x04000032 RID: 50
			RELIABLE_SEQUENCED
		}
	}
}
