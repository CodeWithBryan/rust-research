using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConVar;
using Facepunch.Extend;
using Facepunch.Math;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

// Token: 0x0200071A RID: 1818
public static class ServerUsers
{
	// Token: 0x06003274 RID: 12916 RVA: 0x00137A7C File Offset: 0x00135C7C
	public static void Remove(ulong uid)
	{
		ServerUsers.users.Remove(uid);
	}

	// Token: 0x06003275 RID: 12917 RVA: 0x00137A8C File Offset: 0x00135C8C
	public static void Set(ulong uid, ServerUsers.UserGroup group, string username, string notes, long expiry = -1L)
	{
		ServerUsers.Remove(uid);
		ServerUsers.User value = new ServerUsers.User
		{
			steamid = uid,
			group = group,
			username = username,
			notes = notes,
			expiry = expiry
		};
		ServerUsers.users.Add(uid, value);
	}

	// Token: 0x06003276 RID: 12918 RVA: 0x00137AD8 File Offset: 0x00135CD8
	public static ServerUsers.User Get(ulong uid)
	{
		ServerUsers.User user;
		if (!ServerUsers.users.TryGetValue(uid, out user))
		{
			return null;
		}
		if (!user.IsExpired)
		{
			return user;
		}
		ServerUsers.Remove(uid);
		return null;
	}

	// Token: 0x06003277 RID: 12919 RVA: 0x00137B08 File Offset: 0x00135D08
	public static bool Is(ulong uid, ServerUsers.UserGroup group)
	{
		ServerUsers.User user = ServerUsers.Get(uid);
		return user != null && user.group == group;
	}

	// Token: 0x06003278 RID: 12920 RVA: 0x00137B2C File Offset: 0x00135D2C
	public static IEnumerable<ServerUsers.User> GetAll(ServerUsers.UserGroup group)
	{
		return from x in ServerUsers.users.Values
		where x.@group == @group
		where !x.IsExpired
		select x;
	}

	// Token: 0x06003279 RID: 12921 RVA: 0x00137B85 File Offset: 0x00135D85
	public static void Clear()
	{
		ServerUsers.users.Clear();
	}

	// Token: 0x0600327A RID: 12922 RVA: 0x00137B94 File Offset: 0x00135D94
	public static void Load()
	{
		ServerUsers.Clear();
		string serverFolder = Server.GetServerFolder("cfg");
		if (File.Exists(serverFolder + "/bans.cfg"))
		{
			string text = File.ReadAllText(serverFolder + "/bans.cfg");
			if (!string.IsNullOrEmpty(text))
			{
				Debug.Log("Running " + serverFolder + "/bans.cfg");
				ConsoleSystem.RunFile(ConsoleSystem.Option.Server.Quiet(), text);
			}
		}
		if (File.Exists(serverFolder + "/users.cfg"))
		{
			string text2 = File.ReadAllText(serverFolder + "/users.cfg");
			if (!string.IsNullOrEmpty(text2))
			{
				Debug.Log("Running " + serverFolder + "/users.cfg");
				ConsoleSystem.RunFile(ConsoleSystem.Option.Server.Quiet(), text2);
			}
		}
	}

	// Token: 0x0600327B RID: 12923 RVA: 0x00137C58 File Offset: 0x00135E58
	public static void Save()
	{
		foreach (ulong uid in (from kv in ServerUsers.users
		where kv.Value.IsExpired
		select kv.Key).ToList<ulong>())
		{
			ServerUsers.Remove(uid);
		}
		string serverFolder = Server.GetServerFolder("cfg");
		StringBuilder stringBuilder = new StringBuilder(67108864);
		stringBuilder.Clear();
		foreach (ServerUsers.User user in ServerUsers.GetAll(ServerUsers.UserGroup.Banned))
		{
			if (!(user.notes == "EAC"))
			{
				stringBuilder.Append("banid ");
				stringBuilder.Append(user.steamid);
				stringBuilder.Append(' ');
				stringBuilder.Append(user.username.QuoteSafe());
				stringBuilder.Append(' ');
				stringBuilder.Append(user.notes.QuoteSafe());
				stringBuilder.Append(' ');
				stringBuilder.Append(user.expiry);
				stringBuilder.Append("\r\n");
			}
		}
		File.WriteAllText(serverFolder + "/bans.cfg", stringBuilder.ToString());
		stringBuilder.Clear();
		foreach (ServerUsers.User user2 in ServerUsers.GetAll(ServerUsers.UserGroup.Owner))
		{
			stringBuilder.Append("ownerid ");
			stringBuilder.Append(user2.steamid);
			stringBuilder.Append(' ');
			stringBuilder.Append(user2.username.QuoteSafe());
			stringBuilder.Append(' ');
			stringBuilder.Append(user2.notes.QuoteSafe());
			stringBuilder.Append("\r\n");
		}
		foreach (ServerUsers.User user3 in ServerUsers.GetAll(ServerUsers.UserGroup.Moderator))
		{
			stringBuilder.Append("moderatorid ");
			stringBuilder.Append(user3.steamid);
			stringBuilder.Append(' ');
			stringBuilder.Append(user3.username.QuoteSafe());
			stringBuilder.Append(' ');
			stringBuilder.Append(user3.notes.QuoteSafe());
			stringBuilder.Append("\r\n");
		}
		foreach (ServerUsers.User user4 in ServerUsers.GetAll(ServerUsers.UserGroup.SkipQueue))
		{
			stringBuilder.Append("skipqueueid ");
			stringBuilder.Append(user4.steamid);
			stringBuilder.Append(' ');
			stringBuilder.Append(user4.username.QuoteSafe());
			stringBuilder.Append(' ');
			stringBuilder.Append(user4.notes.QuoteSafe());
			stringBuilder.Append("\r\n");
		}
		File.WriteAllText(serverFolder + "/users.cfg", stringBuilder.ToString());
	}

	// Token: 0x0600327C RID: 12924 RVA: 0x00137FC4 File Offset: 0x001361C4
	public static string BanListString(bool bHeader = false)
	{
		List<ServerUsers.User> list = ServerUsers.GetAll(ServerUsers.UserGroup.Banned).ToList<ServerUsers.User>();
		StringBuilder stringBuilder = new StringBuilder(67108864);
		if (bHeader)
		{
			if (list.Count == 0)
			{
				return "ID filter list: empty\n";
			}
			if (list.Count == 1)
			{
				stringBuilder.Append("ID filter list: 1 entry\n");
			}
			else
			{
				stringBuilder.Append(string.Format("ID filter list: {0} entries\n", list.Count));
			}
		}
		int num = 1;
		foreach (ServerUsers.User user in list)
		{
			stringBuilder.Append(num);
			stringBuilder.Append(' ');
			stringBuilder.Append(user.steamid);
			stringBuilder.Append(" : ");
			if (user.expiry > 0L)
			{
				stringBuilder.Append(((double)(user.expiry - (long)Epoch.Current) / 60.0).ToString("F3"));
				stringBuilder.Append(" min");
			}
			else
			{
				stringBuilder.Append("permanent");
			}
			stringBuilder.Append('\n');
			num++;
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0600327D RID: 12925 RVA: 0x00138100 File Offset: 0x00136300
	public static string BanListStringEx()
	{
		IEnumerable<ServerUsers.User> all = ServerUsers.GetAll(ServerUsers.UserGroup.Banned);
		StringBuilder stringBuilder = new StringBuilder(67108864);
		int num = 1;
		foreach (ServerUsers.User user in all)
		{
			stringBuilder.Append(num);
			stringBuilder.Append(' ');
			stringBuilder.Append(user.steamid);
			stringBuilder.Append(' ');
			stringBuilder.Append(user.username.QuoteSafe());
			stringBuilder.Append(' ');
			stringBuilder.Append(user.notes.QuoteSafe());
			stringBuilder.Append(' ');
			stringBuilder.Append(user.expiry);
			stringBuilder.Append('\n');
			num++;
		}
		return stringBuilder.ToString();
	}

	// Token: 0x040028D0 RID: 10448
	private static Dictionary<ulong, ServerUsers.User> users = new Dictionary<ulong, ServerUsers.User>();

	// Token: 0x02000DF4 RID: 3572
	public enum UserGroup
	{
		// Token: 0x0400487F RID: 18559
		None,
		// Token: 0x04004880 RID: 18560
		Owner,
		// Token: 0x04004881 RID: 18561
		Moderator,
		// Token: 0x04004882 RID: 18562
		Banned,
		// Token: 0x04004883 RID: 18563
		SkipQueue
	}

	// Token: 0x02000DF5 RID: 3573
	public class User
	{
		// Token: 0x17000675 RID: 1653
		// (get) Token: 0x06004FB3 RID: 20403 RVA: 0x001A02E2 File Offset: 0x0019E4E2
		[JsonIgnore]
		public bool IsExpired
		{
			get
			{
				return this.expiry > 0L && (long)Epoch.Current > this.expiry;
			}
		}

		// Token: 0x04004884 RID: 18564
		public ulong steamid;

		// Token: 0x04004885 RID: 18565
		[JsonConverter(typeof(StringEnumConverter))]
		public ServerUsers.UserGroup group;

		// Token: 0x04004886 RID: 18566
		public string username;

		// Token: 0x04004887 RID: 18567
		public string notes;

		// Token: 0x04004888 RID: 18568
		public long expiry;
	}
}
