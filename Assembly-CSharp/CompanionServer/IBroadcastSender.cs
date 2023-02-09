using System;
using System.Collections.Generic;

namespace CompanionServer
{
	// Token: 0x020009B8 RID: 2488
	public interface IBroadcastSender<TTarget, TMessage> where TTarget : class
	{
		// Token: 0x06003AFD RID: 15101
		void BroadcastTo(List<TTarget> targets, TMessage message);
	}
}
