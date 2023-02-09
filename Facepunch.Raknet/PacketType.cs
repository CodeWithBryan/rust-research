using System;

namespace Facepunch.Network.Raknet
{
	// Token: 0x02000004 RID: 4
	public static class PacketType
	{
		// Token: 0x04000002 RID: 2
		public const byte NEW_INCOMING_CONNECTION = 19;

		// Token: 0x04000003 RID: 3
		public const byte CONNECTION_REQUEST_ACCEPTED = 16;

		// Token: 0x04000004 RID: 4
		public const byte CONNECTION_ATTEMPT_FAILED = 17;

		// Token: 0x04000005 RID: 5
		public const byte DISCONNECTION_NOTIFICATION = 21;

		// Token: 0x04000006 RID: 6
		public const byte NO_FREE_INCOMING_CONNECTIONS = 20;

		// Token: 0x04000007 RID: 7
		public const byte CONNECTION_LOST = 22;

		// Token: 0x04000008 RID: 8
		public const byte CONNECTION_BANNED = 23;
	}
}
