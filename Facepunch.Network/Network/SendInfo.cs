using System;
using System.Collections.Generic;

namespace Network
{
	// Token: 0x02000017 RID: 23
	public struct SendInfo
	{
		// Token: 0x060000EF RID: 239 RVA: 0x000043F4 File Offset: 0x000025F4
		public SendInfo(List<Connection> connections)
		{
			this = default(SendInfo);
			this.channel = 0;
			this.method = SendMethod.Reliable;
			this.priority = Priority.Normal;
			this.connections = connections;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00004419 File Offset: 0x00002619
		public SendInfo(Connection connection)
		{
			this = default(SendInfo);
			this.channel = 0;
			this.method = SendMethod.Reliable;
			this.priority = Priority.Normal;
			this.connection = connection;
		}

		// Token: 0x0400006F RID: 111
		public SendMethod method;

		// Token: 0x04000070 RID: 112
		public sbyte channel;

		// Token: 0x04000071 RID: 113
		public Priority priority;

		// Token: 0x04000072 RID: 114
		public List<Connection> connections;

		// Token: 0x04000073 RID: 115
		public Connection connection;
	}
}
