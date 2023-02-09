using System;
using System.Collections;
using System.Collections.Generic;
using JSON;
using UnityEngine;

// Token: 0x0200084F RID: 2127
public static class SteamNewsSource
{
	// Token: 0x060034DD RID: 13533 RVA: 0x0013F3EE File Offset: 0x0013D5EE
	public static IEnumerator GetStories()
	{
		WWW www = new WWW("http://api.steampowered.com/ISteamNews/GetNewsForApp/v0002/?appid=252490&count=8&format=json&feeds=steam_community_announcements");
		yield return www;
		JSON.Object @object = JSON.Object.Parse(www.text);
		www.Dispose();
		if (@object == null)
		{
			yield break;
		}
		JSON.Array array = @object.GetObject("appnews").GetArray("newsitems");
		List<SteamNewsSource.Story> list = new List<SteamNewsSource.Story>();
		foreach (Value value in array)
		{
			string text = value.Obj.GetString("contents", "Missing Contents");
			text = text.Replace("\\n", "\n").Replace("\\r", "").Replace("\\\"", "\"");
			list.Add(new SteamNewsSource.Story
			{
				name = value.Obj.GetString("title", "Missing Title"),
				url = value.Obj.GetString("url", "Missing URL"),
				date = value.Obj.GetInt("date", 0),
				text = text,
				author = value.Obj.GetString("author", "Missing Author")
			});
		}
		SteamNewsSource.Stories = list.ToArray();
		yield break;
	}

	// Token: 0x04002F5E RID: 12126
	public static SteamNewsSource.Story[] Stories;

	// Token: 0x02000E39 RID: 3641
	public struct Story
	{
		// Token: 0x040049A7 RID: 18855
		public string name;

		// Token: 0x040049A8 RID: 18856
		public string url;

		// Token: 0x040049A9 RID: 18857
		public int date;

		// Token: 0x040049AA RID: 18858
		public string text;

		// Token: 0x040049AB RID: 18859
		public string author;
	}
}
