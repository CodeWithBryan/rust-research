using System;
using Facepunch;
using UnityEngine;

namespace Network.Visibility
{
	// Token: 0x0200001C RID: 28
	public class Subscriber : Pool.IPooled
	{
		// Token: 0x06000117 RID: 279 RVA: 0x0000488B File Offset: 0x00002A8B
		public Group Subscribe(Group group)
		{
			if (this.subscribed.Contains(group))
			{
				Debug.LogWarning("Subscribe: Network Group already subscribed!");
				return null;
			}
			this.subscribed.Add(group);
			group.AddSubscriber(this.connection);
			return group;
		}

		// Token: 0x06000118 RID: 280 RVA: 0x000048C0 File Offset: 0x00002AC0
		public Group Subscribe(uint group)
		{
			return this.Subscribe(this.manager.Get(group));
		}

		// Token: 0x06000119 RID: 281 RVA: 0x000048D4 File Offset: 0x00002AD4
		public bool IsSubscribed(Group group)
		{
			return this.subscribed.Contains(group);
		}

		// Token: 0x0600011A RID: 282 RVA: 0x000048E4 File Offset: 0x00002AE4
		public void UnsubscribeAll()
		{
			foreach (Group group in this.subscribed)
			{
				group.RemoveSubscriber(this.connection);
			}
			this.subscribed.Clear();
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00004948 File Offset: 0x00002B48
		public void Unsubscribe(Group group)
		{
			this.subscribed.Remove(group);
			group.RemoveSubscriber(this.connection);
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00004963 File Offset: 0x00002B63
		public void Destroy()
		{
			this.UnsubscribeAll();
		}

		// Token: 0x0600011D RID: 285 RVA: 0x0000496B File Offset: 0x00002B6B
		public void EnterPool()
		{
			this.connection = null;
			this.manager = null;
		}

		// Token: 0x0600011E RID: 286 RVA: 0x00002209 File Offset: 0x00000409
		public void LeavePool()
		{
		}

		// Token: 0x04000089 RID: 137
		internal Manager manager;

		// Token: 0x0400008A RID: 138
		internal Connection connection;

		// Token: 0x0400008B RID: 139
		public ListHashSet<Group> subscribed = new ListHashSet<Group>(8);
	}
}
