using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000861 RID: 2145
public class ServerBrowserList : BaseMonoBehaviour, VirtualScroll.IDataSource
{
	// Token: 0x06003523 RID: 13603 RVA: 0x00007074 File Offset: 0x00005274
	public int GetItemCount()
	{
		return 0;
	}

	// Token: 0x06003524 RID: 13604 RVA: 0x000059DD File Offset: 0x00003BDD
	public void SetItemData(int i, GameObject obj)
	{
	}

	// Token: 0x04002F97 RID: 12183
	public ServerBrowserList.QueryType queryType;

	// Token: 0x04002F98 RID: 12184
	public static string VersionTag = "v" + 2370;

	// Token: 0x04002F99 RID: 12185
	public ServerBrowserList.ServerKeyvalues[] keyValues = new ServerBrowserList.ServerKeyvalues[0];

	// Token: 0x04002F9A RID: 12186
	public ServerBrowserCategory categoryButton;

	// Token: 0x04002F9B RID: 12187
	public bool startActive;

	// Token: 0x04002F9C RID: 12188
	public Transform listTransform;

	// Token: 0x04002F9D RID: 12189
	public int refreshOrder;

	// Token: 0x04002F9E RID: 12190
	public bool UseOfficialServers;

	// Token: 0x04002F9F RID: 12191
	public VirtualScroll VirtualScroll;

	// Token: 0x04002FA0 RID: 12192
	public ServerBrowserList.Rules[] rules;

	// Token: 0x04002FA1 RID: 12193
	public bool hideOfficialServers;

	// Token: 0x04002FA2 RID: 12194
	public bool excludeEmptyServersUsingQuery;

	// Token: 0x04002FA3 RID: 12195
	public bool alwaysIncludeEmptyServers;

	// Token: 0x04002FA4 RID: 12196
	public bool clampPlayerCountsToTrustedValues;

	// Token: 0x02000E3E RID: 3646
	public enum QueryType
	{
		// Token: 0x040049B7 RID: 18871
		RegularInternet,
		// Token: 0x040049B8 RID: 18872
		Friends,
		// Token: 0x040049B9 RID: 18873
		History,
		// Token: 0x040049BA RID: 18874
		LAN,
		// Token: 0x040049BB RID: 18875
		Favourites,
		// Token: 0x040049BC RID: 18876
		None
	}

	// Token: 0x02000E3F RID: 3647
	[Serializable]
	public struct ServerKeyvalues
	{
		// Token: 0x040049BD RID: 18877
		public string key;

		// Token: 0x040049BE RID: 18878
		public string value;
	}

	// Token: 0x02000E40 RID: 3648
	[Serializable]
	public struct Rules
	{
		// Token: 0x040049BF RID: 18879
		public string tag;

		// Token: 0x040049C0 RID: 18880
		public ServerBrowserList serverList;
	}

	// Token: 0x02000E41 RID: 3649
	private class HashSetEqualityComparer<T> : IEqualityComparer<HashSet<T>> where T : IComparable<T>
	{
		// Token: 0x17000681 RID: 1665
		// (get) Token: 0x06005028 RID: 20520 RVA: 0x001A0FCB File Offset: 0x0019F1CB
		public static ServerBrowserList.HashSetEqualityComparer<T> Instance { get; } = new ServerBrowserList.HashSetEqualityComparer<T>();

		// Token: 0x06005029 RID: 20521 RVA: 0x001A0FD4 File Offset: 0x0019F1D4
		public bool Equals(HashSet<T> x, HashSet<T> y)
		{
			if (x == y)
			{
				return true;
			}
			if (x == null)
			{
				return false;
			}
			if (y == null)
			{
				return false;
			}
			if (x.GetType() != y.GetType())
			{
				return false;
			}
			if (x.Count != y.Count)
			{
				return false;
			}
			foreach (T item in x)
			{
				if (!y.Contains(item))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600502A RID: 20522 RVA: 0x001A1060 File Offset: 0x0019F260
		public int GetHashCode(HashSet<T> set)
		{
			int num = 0;
			if (set != null)
			{
				foreach (T t in set)
				{
					num ^= (((t != null) ? t.GetHashCode() : 0) & int.MaxValue);
				}
			}
			return num;
		}
	}
}
