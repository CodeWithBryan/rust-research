using System;
using System.Collections.Generic;
using Facepunch;

namespace CompanionServer
{
	// Token: 0x020009B9 RID: 2489
	public class SubscriberList<TKey, TTarget, TMessage> where TKey : IEquatable<TKey> where TTarget : class
	{
		// Token: 0x06003AFE RID: 15102 RVA: 0x00159B2C File Offset: 0x00157D2C
		public SubscriberList(IBroadcastSender<TTarget, TMessage> sender)
		{
			this._syncRoot = new object();
			this._subscriptions = new Dictionary<TKey, HashSet<TTarget>>();
			this._sender = sender;
		}

		// Token: 0x06003AFF RID: 15103 RVA: 0x00159B54 File Offset: 0x00157D54
		public void Add(TKey key, TTarget value)
		{
			object syncRoot = this._syncRoot;
			lock (syncRoot)
			{
				HashSet<TTarget> hashSet;
				if (this._subscriptions.TryGetValue(key, out hashSet))
				{
					hashSet.Add(value);
				}
				else
				{
					hashSet = new HashSet<TTarget>
					{
						value
					};
					this._subscriptions.Add(key, hashSet);
				}
			}
		}

		// Token: 0x06003B00 RID: 15104 RVA: 0x00159BC4 File Offset: 0x00157DC4
		public void Remove(TKey key, TTarget value)
		{
			object syncRoot = this._syncRoot;
			lock (syncRoot)
			{
				HashSet<TTarget> hashSet;
				if (this._subscriptions.TryGetValue(key, out hashSet))
				{
					hashSet.Remove(value);
					if (hashSet.Count == 0)
					{
						this._subscriptions.Remove(key);
					}
				}
			}
		}

		// Token: 0x06003B01 RID: 15105 RVA: 0x00159C30 File Offset: 0x00157E30
		public void Clear(TKey key)
		{
			object syncRoot = this._syncRoot;
			lock (syncRoot)
			{
				HashSet<TTarget> hashSet;
				if (this._subscriptions.TryGetValue(key, out hashSet))
				{
					hashSet.Clear();
				}
			}
		}

		// Token: 0x06003B02 RID: 15106 RVA: 0x00159C80 File Offset: 0x00157E80
		public void Send(TKey key, TMessage message)
		{
			object syncRoot = this._syncRoot;
			List<TTarget> list;
			lock (syncRoot)
			{
				HashSet<TTarget> hashSet;
				if (!this._subscriptions.TryGetValue(key, out hashSet))
				{
					return;
				}
				list = Pool.GetList<TTarget>();
				foreach (TTarget item in hashSet)
				{
					list.Add(item);
				}
			}
			this._sender.BroadcastTo(list, message);
			Pool.FreeList<TTarget>(ref list);
		}

		// Token: 0x04003515 RID: 13589
		private readonly object _syncRoot;

		// Token: 0x04003516 RID: 13590
		private readonly Dictionary<TKey, HashSet<TTarget>> _subscriptions;

		// Token: 0x04003517 RID: 13591
		private readonly IBroadcastSender<TTarget, TMessage> _sender;
	}
}
