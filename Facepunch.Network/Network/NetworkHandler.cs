using System;
using System.Collections.Generic;
using Network.Visibility;

namespace Network
{
	// Token: 0x02000015 RID: 21
	public interface NetworkHandler
	{
		// Token: 0x060000D8 RID: 216
		void OnNetworkSubscribersEnter(List<Connection> connections);

		// Token: 0x060000D9 RID: 217
		void OnNetworkSubscribersLeave(List<Connection> connections);

		// Token: 0x060000DA RID: 218
		void OnNetworkGroupChange();

		// Token: 0x060000DB RID: 219
		void OnNetworkGroupLeave(Group group);

		// Token: 0x060000DC RID: 220
		void OnNetworkGroupEnter(Group group);
	}
}
