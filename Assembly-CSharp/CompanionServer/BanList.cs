using System;
using System.Collections.Generic;
using Facepunch;
using Network;

namespace CompanionServer
{
	// Token: 0x020009AB RID: 2475
	public class BanList<TKey>
	{
		// Token: 0x06003AA4 RID: 15012 RVA: 0x00158309 File Offset: 0x00156509
		public BanList()
		{
			this._bans = new Dictionary<TKey, double>();
		}

		// Token: 0x06003AA5 RID: 15013 RVA: 0x0015831C File Offset: 0x0015651C
		public void Ban(TKey key, double timeInSeconds)
		{
			Dictionary<TKey, double> bans = this._bans;
			lock (bans)
			{
				double num = TimeEx.realtimeSinceStartup + timeInSeconds;
				double val;
				if (this._bans.TryGetValue(key, out val))
				{
					num = Math.Max(num, val);
				}
				this._bans[key] = num;
			}
		}

		// Token: 0x06003AA6 RID: 15014 RVA: 0x00158384 File Offset: 0x00156584
		public bool IsBanned(TKey key)
		{
			Dictionary<TKey, double> bans = this._bans;
			bool result;
			lock (bans)
			{
				double num;
				if (!this._bans.TryGetValue(key, out num))
				{
					result = false;
				}
				else if (TimeEx.realtimeSinceStartup < num)
				{
					result = true;
				}
				else
				{
					this._bans.Remove(key);
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06003AA7 RID: 15015 RVA: 0x001583F0 File Offset: 0x001565F0
		public void Cleanup()
		{
			double realtimeSinceStartup = TimeEx.realtimeSinceStartup;
			List<TKey> list = Pool.GetList<TKey>();
			Dictionary<TKey, double> bans = this._bans;
			lock (bans)
			{
				foreach (KeyValuePair<TKey, double> keyValuePair in this._bans)
				{
					if (realtimeSinceStartup >= keyValuePair.Value)
					{
						list.Add(keyValuePair.Key);
					}
				}
				foreach (TKey key in list)
				{
					this._bans.Remove(key);
				}
			}
			Pool.FreeList<TKey>(ref list);
		}

		// Token: 0x040034DD RID: 13533
		private readonly Dictionary<TKey, double> _bans;
	}
}
