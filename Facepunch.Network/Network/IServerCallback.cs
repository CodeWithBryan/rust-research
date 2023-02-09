using System;

namespace Network
{
	// Token: 0x02000018 RID: 24
	public interface IServerCallback
	{
		// Token: 0x060000F1 RID: 241
		void OnNetworkMessage(Message message);

		// Token: 0x060000F2 RID: 242
		void OnDisconnected(string reason, Connection connection);
	}
}
