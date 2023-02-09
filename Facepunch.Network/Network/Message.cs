using System;

namespace Network
{
	// Token: 0x02000010 RID: 16
	public class Message
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000073 RID: 115 RVA: 0x000030A4 File Offset: 0x000012A4
		public Connection connection
		{
			get
			{
				return this.read.connection;
			}
		}

		// Token: 0x06000074 RID: 116 RVA: 0x000030B1 File Offset: 0x000012B1
		public virtual void Clear()
		{
			this.read = null;
			this.peer = null;
			this.type = Message.Type.First;
		}

		// Token: 0x04000047 RID: 71
		public static bool[] EncryptionPerType = new bool[]
		{
			false,
			false,
			false,
			false,
			false,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			false,
			false,
			true,
			true,
			true,
			false,
			true,
			false,
			false,
			false
		};

		// Token: 0x04000048 RID: 72
		public Message.Type type;

		// Token: 0x04000049 RID: 73
		public BaseNetwork peer;

		// Token: 0x0400004A RID: 74
		public NetRead read;

		// Token: 0x02000024 RID: 36
		public enum Type : byte
		{
			// Token: 0x040000AA RID: 170
			First,
			// Token: 0x040000AB RID: 171
			Welcome,
			// Token: 0x040000AC RID: 172
			Auth,
			// Token: 0x040000AD RID: 173
			Approved,
			// Token: 0x040000AE RID: 174
			Ready,
			// Token: 0x040000AF RID: 175
			Entities,
			// Token: 0x040000B0 RID: 176
			EntityDestroy,
			// Token: 0x040000B1 RID: 177
			GroupChange,
			// Token: 0x040000B2 RID: 178
			GroupDestroy,
			// Token: 0x040000B3 RID: 179
			RPCMessage,
			// Token: 0x040000B4 RID: 180
			EntityPosition,
			// Token: 0x040000B5 RID: 181
			ConsoleMessage,
			// Token: 0x040000B6 RID: 182
			ConsoleCommand,
			// Token: 0x040000B7 RID: 183
			Effect,
			// Token: 0x040000B8 RID: 184
			DisconnectReason,
			// Token: 0x040000B9 RID: 185
			Tick,
			// Token: 0x040000BA RID: 186
			Message,
			// Token: 0x040000BB RID: 187
			RequestUserInformation,
			// Token: 0x040000BC RID: 188
			GiveUserInformation,
			// Token: 0x040000BD RID: 189
			GroupEnter,
			// Token: 0x040000BE RID: 190
			GroupLeave,
			// Token: 0x040000BF RID: 191
			VoiceData,
			// Token: 0x040000C0 RID: 192
			EAC,
			// Token: 0x040000C1 RID: 193
			EntityFlags,
			// Token: 0x040000C2 RID: 194
			World,
			// Token: 0x040000C3 RID: 195
			ConsoleReplicatedVars,
			// Token: 0x040000C4 RID: 196
			Last = 25
		}
	}
}
