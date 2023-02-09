using System;
using System.Collections.Generic;

// Token: 0x02000430 RID: 1072
public class ServerStatistics
{
	// Token: 0x06002360 RID: 9056 RVA: 0x000E09AA File Offset: 0x000DEBAA
	public ServerStatistics(BasePlayer player)
	{
		this.player = player;
	}

	// Token: 0x06002361 RID: 9057 RVA: 0x000E09B9 File Offset: 0x000DEBB9
	public void Init()
	{
		this.storage = ServerStatistics.Get(this.player.userID);
	}

	// Token: 0x06002362 RID: 9058 RVA: 0x000059DD File Offset: 0x00003BDD
	public void Save()
	{
	}

	// Token: 0x06002363 RID: 9059 RVA: 0x000E09D1 File Offset: 0x000DEBD1
	public void Add(string name, int val)
	{
		if (this.storage != null)
		{
			this.storage.Add(name, val);
		}
	}

	// Token: 0x06002364 RID: 9060 RVA: 0x000E09E8 File Offset: 0x000DEBE8
	public static ServerStatistics.Storage Get(ulong id)
	{
		ServerStatistics.Storage storage;
		if (ServerStatistics.players.TryGetValue(id, out storage))
		{
			return storage;
		}
		storage = new ServerStatistics.Storage();
		ServerStatistics.players.Add(id, storage);
		return storage;
	}

	// Token: 0x04001C20 RID: 7200
	private BasePlayer player;

	// Token: 0x04001C21 RID: 7201
	private ServerStatistics.Storage storage;

	// Token: 0x04001C22 RID: 7202
	private static Dictionary<ulong, ServerStatistics.Storage> players = new Dictionary<ulong, ServerStatistics.Storage>();

	// Token: 0x02000C95 RID: 3221
	public class Storage
	{
		// Token: 0x06004D0F RID: 19727 RVA: 0x00196E8C File Offset: 0x0019508C
		public int Get(string name)
		{
			int result;
			this.dict.TryGetValue(name, out result);
			return result;
		}

		// Token: 0x06004D10 RID: 19728 RVA: 0x00196EAC File Offset: 0x001950AC
		public void Add(string name, int val)
		{
			if (this.dict.ContainsKey(name))
			{
				Dictionary<string, int> dictionary = this.dict;
				dictionary[name] += val;
				return;
			}
			this.dict.Add(name, val);
		}

		// Token: 0x0400432C RID: 17196
		private Dictionary<string, int> dict = new Dictionary<string, int>();
	}
}
