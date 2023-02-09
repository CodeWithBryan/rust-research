using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Facepunch.Network.Raknet
{
	// Token: 0x02000002 RID: 2
	[SuppressUnmanagedCodeSecurity]
	public class Native
	{
		// Token: 0x06000001 RID: 1
		[DllImport("RakNet")]
		public static extern IntPtr NET_Create();

		// Token: 0x06000002 RID: 2
		[DllImport("RakNet")]
		public static extern void NET_Close(IntPtr nw);

		// Token: 0x06000003 RID: 3
		[DllImport("RakNet")]
		public static extern int NET_StartClient(IntPtr nw, string hostName, int port, int retries, int retryDelay, int timeout);

		// Token: 0x06000004 RID: 4
		[DllImport("RakNet")]
		public static extern int NET_StartServer(IntPtr nw, string ip, int port, int maxConnections);

		// Token: 0x06000005 RID: 5
		[DllImport("RakNet")]
		public static extern IntPtr NET_LastStartupError(IntPtr nw);

		// Token: 0x06000006 RID: 6
		[DllImport("RakNet")]
		public static extern uint NET_GetReceiveBufferSize(IntPtr nw);

		// Token: 0x06000007 RID: 7
		[DllImport("RakNet")]
		public static extern bool NET_Receive(IntPtr nw);

		// Token: 0x06000008 RID: 8
		[DllImport("RakNet")]
		public static extern ulong NETRCV_GUID(IntPtr nw);

		// Token: 0x06000009 RID: 9
		[DllImport("RakNet")]
		public static extern uint NETRCV_Address(IntPtr nw);

		// Token: 0x0600000A RID: 10
		[DllImport("RakNet")]
		public static extern uint NETRCV_Port(IntPtr nw);

		// Token: 0x0600000B RID: 11
		[DllImport("RakNet")]
		public static extern IntPtr NETRCV_RawData(IntPtr nw);

		// Token: 0x0600000C RID: 12
		[DllImport("RakNet")]
		public static extern int NETRCV_LengthBits(IntPtr nw);

		// Token: 0x0600000D RID: 13
		[DllImport("RakNet")]
		public static extern int NETRCV_UnreadBits(IntPtr nw);

		// Token: 0x0600000E RID: 14
		[DllImport("RakNet")]
		public unsafe static extern bool NETRCV_ReadBytes(IntPtr nw, byte* data, int length);

		// Token: 0x0600000F RID: 15
		[DllImport("RakNet")]
		public static extern void NETRCV_SetReadPointer(IntPtr nw, int bitsOffset);

		// Token: 0x06000010 RID: 16
		[DllImport("RakNet")]
		public static extern void NETSND_Start(IntPtr nw);

		// Token: 0x06000011 RID: 17
		[DllImport("RakNet")]
		public unsafe static extern void NETSND_WriteBytes(IntPtr nw, byte* data, int length);

		// Token: 0x06000012 RID: 18
		[DllImport("RakNet")]
		public static extern uint NETSND_Size(IntPtr nw);

		// Token: 0x06000013 RID: 19
		[DllImport("RakNet")]
		public static extern uint NETSND_Broadcast(IntPtr nw, int priority, int reliability, int channel);

		// Token: 0x06000014 RID: 20
		[DllImport("RakNet")]
		public static extern uint NETSND_Send(IntPtr nw, ulong connectionID, int priority, int reliability, int channel);

		// Token: 0x06000015 RID: 21
		[DllImport("RakNet")]
		public static extern void NET_CloseConnection(IntPtr nw, ulong connectionID);

		// Token: 0x06000016 RID: 22
		[DllImport("RakNet")]
		public static extern IntPtr NET_GetAddress(IntPtr nw, ulong connectionID);

		// Token: 0x06000017 RID: 23
		[DllImport("RakNet")]
		public static extern IntPtr NET_GetStatisticsString(IntPtr nw, ulong connectionID);

		// Token: 0x06000018 RID: 24
		[DllImport("RakNet")]
		public static extern bool NET_GetStatistics(IntPtr nw, ulong connectionID, ref Native.RaknetStats data, int dataLength);

		// Token: 0x06000019 RID: 25
		[DllImport("RakNet")]
		public static extern int NET_GetAveragePing(IntPtr nw, ulong connectionID);

		// Token: 0x0600001A RID: 26
		[DllImport("RakNet")]
		public static extern int NET_GetLastPing(IntPtr nw, ulong connectionID);

		// Token: 0x0600001B RID: 27
		[DllImport("RakNet")]
		public static extern int NET_GetLowestPing(IntPtr nw, ulong connectionID);

		// Token: 0x0600001C RID: 28
		[DllImport("RakNet")]
		public unsafe static extern void NET_SendMessage(IntPtr nw, byte* data, int length, uint adr, ushort port);

		// Token: 0x0600001D RID: 29
		[DllImport("RakNet")]
		public static extern float NETSND_ReadCompressedFloat(IntPtr nw);

		// Token: 0x02000007 RID: 7
		public enum Metrics
		{
			// Token: 0x0400000D RID: 13
			USER_MESSAGE_BYTES_PUSHED,
			// Token: 0x0400000E RID: 14
			USER_MESSAGE_BYTES_SENT,
			// Token: 0x0400000F RID: 15
			USER_MESSAGE_BYTES_RESENT,
			// Token: 0x04000010 RID: 16
			USER_MESSAGE_BYTES_RECEIVED_PROCESSED,
			// Token: 0x04000011 RID: 17
			USER_MESSAGE_BYTES_RECEIVED_IGNORED,
			// Token: 0x04000012 RID: 18
			ACTUAL_BYTES_SENT,
			// Token: 0x04000013 RID: 19
			ACTUAL_BYTES_RECEIVED,
			// Token: 0x04000014 RID: 20
			RNS_PER_SECOND_METRICS_COUNT
		}

		// Token: 0x02000008 RID: 8
		public enum PacketPriority
		{
			// Token: 0x04000016 RID: 22
			IMMEDIATE_PRIORITY,
			// Token: 0x04000017 RID: 23
			HIGH_PRIORITY,
			// Token: 0x04000018 RID: 24
			MEDIUM_PRIORITY,
			// Token: 0x04000019 RID: 25
			LOW_PRIORITY,
			// Token: 0x0400001A RID: 26
			NUMBER_OF_PRIORITIES
		}

		// Token: 0x02000009 RID: 9
		public struct RaknetStats
		{
			// Token: 0x0400001B RID: 27
			[FixedBuffer(typeof(ulong), 7)]
			public Native.RaknetStats.<valueOverLastSecond>e__FixedBuffer valueOverLastSecond;

			// Token: 0x0400001C RID: 28
			[FixedBuffer(typeof(ulong), 7)]
			public Native.RaknetStats.<runningTotal>e__FixedBuffer runningTotal;

			// Token: 0x0400001D RID: 29
			public ulong connectionStartTime;

			// Token: 0x0400001E RID: 30
			public byte isLimitedByCongestionControl;

			// Token: 0x0400001F RID: 31
			public ulong BPSLimitByCongestionControl;

			// Token: 0x04000020 RID: 32
			public byte isLimitedByOutgoingBandwidthLimit;

			// Token: 0x04000021 RID: 33
			public ulong BPSLimitByOutgoingBandwidthLimit;

			// Token: 0x04000022 RID: 34
			[FixedBuffer(typeof(uint), 4)]
			public Native.RaknetStats.<messageInSendBuffer>e__FixedBuffer messageInSendBuffer;

			// Token: 0x04000023 RID: 35
			[FixedBuffer(typeof(double), 4)]
			public Native.RaknetStats.<bytesInSendBuffer>e__FixedBuffer bytesInSendBuffer;

			// Token: 0x04000024 RID: 36
			public uint messagesInResendBuffer;

			// Token: 0x04000025 RID: 37
			public ulong bytesInResendBuffer;

			// Token: 0x04000026 RID: 38
			public float packetlossLastSecond;

			// Token: 0x04000027 RID: 39
			public float packetlossTotal;

			// Token: 0x0200000C RID: 12
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 56)]
			public struct <valueOverLastSecond>e__FixedBuffer
			{
				// Token: 0x04000033 RID: 51
				public ulong FixedElementField;
			}

			// Token: 0x0200000D RID: 13
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 56)]
			public struct <runningTotal>e__FixedBuffer
			{
				// Token: 0x04000034 RID: 52
				public ulong FixedElementField;
			}

			// Token: 0x0200000E RID: 14
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 16)]
			public struct <messageInSendBuffer>e__FixedBuffer
			{
				// Token: 0x04000035 RID: 53
				public uint FixedElementField;
			}

			// Token: 0x0200000F RID: 15
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 32)]
			public struct <bytesInSendBuffer>e__FixedBuffer
			{
				// Token: 0x04000036 RID: 54
				public double FixedElementField;
			}
		}
	}
}
