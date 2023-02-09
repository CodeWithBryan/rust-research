using System;
using ProtoBuf;

namespace CompanionServer
{
	// Token: 0x020009B0 RID: 2480
	public interface IConnection
	{
		// Token: 0x06003AC4 RID: 15044
		void Send(AppResponse response);

		// Token: 0x06003AC5 RID: 15045
		void Subscribe(PlayerTarget target);

		// Token: 0x06003AC6 RID: 15046
		void Unsubscribe(PlayerTarget target);

		// Token: 0x06003AC7 RID: 15047
		void Subscribe(EntityTarget target);

		// Token: 0x06003AC8 RID: 15048
		void Unsubscribe(EntityTarget target);
	}
}
