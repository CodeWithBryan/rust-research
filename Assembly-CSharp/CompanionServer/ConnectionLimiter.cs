using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ConVar;

namespace CompanionServer
{
	// Token: 0x020009AE RID: 2478
	public class ConnectionLimiter
	{
		// Token: 0x06003AB8 RID: 15032 RVA: 0x00158899 File Offset: 0x00156A99
		public ConnectionLimiter()
		{
			this._sync = new object();
			this._addressCounts = new Dictionary<IPAddress, int>();
			this._overallCount = 0;
		}

		// Token: 0x06003AB9 RID: 15033 RVA: 0x001588C0 File Offset: 0x00156AC0
		public bool TryAdd(IPAddress address)
		{
			if (address == null)
			{
				return false;
			}
			object sync = this._sync;
			bool result;
			lock (sync)
			{
				if (this._overallCount >= App.maxconnections)
				{
					result = false;
				}
				else
				{
					int num;
					if (this._addressCounts.TryGetValue(address, out num))
					{
						if (num >= App.maxconnectionsperip)
						{
							return false;
						}
						this._addressCounts[address] = num + 1;
					}
					else
					{
						this._addressCounts.Add(address, 1);
					}
					this._overallCount++;
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06003ABA RID: 15034 RVA: 0x0015895C File Offset: 0x00156B5C
		public void Remove(IPAddress address)
		{
			if (address == null)
			{
				return;
			}
			object sync = this._sync;
			lock (sync)
			{
				int num;
				if (this._addressCounts.TryGetValue(address, out num))
				{
					if (num <= 1)
					{
						this._addressCounts.Remove(address);
					}
					else
					{
						this._addressCounts[address] = num - 1;
					}
					this._overallCount--;
				}
			}
		}

		// Token: 0x06003ABB RID: 15035 RVA: 0x001589DC File Offset: 0x00156BDC
		public void Clear()
		{
			object sync = this._sync;
			lock (sync)
			{
				this._addressCounts.Clear();
				this._overallCount = 0;
			}
		}

		// Token: 0x06003ABC RID: 15036 RVA: 0x00158A28 File Offset: 0x00156C28
		public override string ToString()
		{
			TextTable textTable = new TextTable();
			textTable.AddColumns(new string[]
			{
				"IP",
				"connections"
			});
			object sync = this._sync;
			string result;
			lock (sync)
			{
				foreach (KeyValuePair<IPAddress, int> keyValuePair in from t in this._addressCounts
				orderby t.Value descending
				select t)
				{
					textTable.AddRow(new string[]
					{
						keyValuePair.Key.ToString(),
						keyValuePair.Value.ToString()
					});
				}
				result = string.Format("{0}\n{1} total", textTable, this._overallCount);
			}
			return result;
		}

		// Token: 0x040034E5 RID: 13541
		private readonly object _sync;

		// Token: 0x040034E6 RID: 13542
		private readonly Dictionary<IPAddress, int> _addressCounts;

		// Token: 0x040034E7 RID: 13543
		private int _overallCount;
	}
}
