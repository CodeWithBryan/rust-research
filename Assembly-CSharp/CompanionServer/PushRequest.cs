using System;
using System.Collections.Generic;
using Facepunch;

namespace CompanionServer
{
	// Token: 0x020009B6 RID: 2486
	public class PushRequest : Pool.IPooled
	{
		// Token: 0x06003AE7 RID: 15079 RVA: 0x0015974A File Offset: 0x0015794A
		public void EnterPool()
		{
			Pool.FreeList<ulong>(ref this.SteamIds);
			this.Channel = (NotificationChannel)0;
			this.Title = null;
			this.Body = null;
			if (this.Data != null)
			{
				this.Data.Clear();
				Pool.Free<Dictionary<string, string>>(ref this.Data);
			}
		}

		// Token: 0x06003AE8 RID: 15080 RVA: 0x000059DD File Offset: 0x00003BDD
		public void LeavePool()
		{
		}

		// Token: 0x0400350A RID: 13578
		public string ServerToken;

		// Token: 0x0400350B RID: 13579
		public List<ulong> SteamIds;

		// Token: 0x0400350C RID: 13580
		public NotificationChannel Channel;

		// Token: 0x0400350D RID: 13581
		public string Title;

		// Token: 0x0400350E RID: 13582
		public string Body;

		// Token: 0x0400350F RID: 13583
		public Dictionary<string, string> Data;
	}
}
