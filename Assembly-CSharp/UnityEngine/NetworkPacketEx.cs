using System;
using Network;

namespace UnityEngine
{
	// Token: 0x020009E3 RID: 2531
	public static class NetworkPacketEx
	{
		// Token: 0x06003B9C RID: 15260 RVA: 0x0015C064 File Offset: 0x0015A264
		public static BasePlayer Player(this Message v)
		{
			if (v.connection == null)
			{
				return null;
			}
			return v.connection.player as BasePlayer;
		}
	}
}
