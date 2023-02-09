using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network.Visibility
{
	// Token: 0x0200001D RID: 29
	public class Group : IDisposable
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000120 RID: 288 RVA: 0x0000498F File Offset: 0x00002B8F
		public bool isGlobal
		{
			get
			{
				return this.ID == 0U;
			}
		}

		// Token: 0x06000121 RID: 289 RVA: 0x0000499A File Offset: 0x00002B9A
		public Group(Manager m, uint id)
		{
			this.manager = m;
			this.ID = id;
		}

		// Token: 0x06000122 RID: 290 RVA: 0x000049C7 File Offset: 0x00002BC7
		public virtual void Dispose()
		{
			this.networkables = null;
			this.subscribers = null;
			this.manager = null;
			this.ID = 0U;
		}

		// Token: 0x06000123 RID: 291 RVA: 0x000049E5 File Offset: 0x00002BE5
		public void Join(Networkable nw)
		{
			if (this.networkables == null)
			{
				return;
			}
			if (this.networkables.Contains(nw))
			{
				Debug.LogWarning("Insert: Network Group already contains networkable!");
				return;
			}
			this.networkables.Add(nw);
		}

		// Token: 0x06000124 RID: 292 RVA: 0x00004A15 File Offset: 0x00002C15
		public void Leave(Networkable nw)
		{
			if (this.networkables == null)
			{
				return;
			}
			if (!this.networkables.Contains(nw))
			{
				Debug.LogWarning("Leave: Network Group doesn't contain networkable!");
				return;
			}
			this.networkables.Remove(nw);
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00004A46 File Offset: 0x00002C46
		public void AddSubscriber(Connection cn)
		{
			this.subscribers.Add(cn);
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00004A54 File Offset: 0x00002C54
		public void RemoveSubscriber(Connection cn)
		{
			if (this.subscribers == null)
			{
				return;
			}
			this.subscribers.Remove(cn);
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00004A6C File Offset: 0x00002C6C
		public bool HasSubscribers()
		{
			return this.subscribers.Count > 0;
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00004A7C File Offset: 0x00002C7C
		public override string ToString()
		{
			return "NWGroup" + this.ID;
		}

		// Token: 0x0400008C RID: 140
		protected Manager manager;

		// Token: 0x0400008D RID: 141
		public uint ID;

		// Token: 0x0400008E RID: 142
		public Bounds bounds;

		// Token: 0x0400008F RID: 143
		public ListHashSet<Networkable> networkables = new ListHashSet<Networkable>(8);

		// Token: 0x04000090 RID: 144
		public List<Connection> subscribers = new List<Connection>();
	}
}
