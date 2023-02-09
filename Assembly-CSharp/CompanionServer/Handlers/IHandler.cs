using System;
using Facepunch;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x020009C4 RID: 2500
	public interface IHandler : Pool.IPooled
	{
		// Token: 0x1700049D RID: 1181
		// (get) Token: 0x06003B3F RID: 15167
		AppRequest Request { get; }

		// Token: 0x06003B40 RID: 15168
		ValidationResult Validate();

		// Token: 0x06003B41 RID: 15169
		void Execute();

		// Token: 0x06003B42 RID: 15170
		void SendError(string code);
	}
}
