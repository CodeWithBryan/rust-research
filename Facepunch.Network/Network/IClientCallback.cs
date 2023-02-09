using System;

namespace Network
{
	// Token: 0x02000009 RID: 9
	public interface IClientCallback
	{
		// Token: 0x0600003C RID: 60
		void OnNetworkMessage(Message message);

		// Token: 0x0600003D RID: 61
		void OnClientDisconnected(string reason);
	}
}
