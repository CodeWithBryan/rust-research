using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Facepunch;
using Facepunch.Extend;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000A93 RID: 2707
	[ConsoleSystem.Factory("pool")]
	public class Pool : ConsoleSystem
	{
		// Token: 0x06004094 RID: 16532 RVA: 0x0017C8A8 File Offset: 0x0017AAA8
		[ServerVar]
		[ClientVar]
		public static void print_memory(ConsoleSystem.Arg arg)
		{
			if (Pool.directory.Count == 0)
			{
				arg.ReplyWith("Memory pool is empty.");
				return;
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn("type");
			textTable.AddColumn("pooled");
			textTable.AddColumn("active");
			textTable.AddColumn("hits");
			textTable.AddColumn("misses");
			textTable.AddColumn("spills");
			foreach (KeyValuePair<Type, Pool.ICollection> keyValuePair in from x in Pool.directory
			orderby x.Value.ItemsCreated descending
			select x)
			{
				string text = keyValuePair.Key.ToString().Replace("System.Collections.Generic.", "");
				Pool.ICollection value = keyValuePair.Value;
				textTable.AddRow(new string[]
				{
					text,
					value.ItemsInStack.FormatNumberShort(),
					value.ItemsInUse.FormatNumberShort(),
					value.ItemsTaken.FormatNumberShort(),
					value.ItemsCreated.FormatNumberShort(),
					value.ItemsSpilled.FormatNumberShort()
				});
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x06004095 RID: 16533 RVA: 0x0017CA18 File Offset: 0x0017AC18
		[ServerVar]
		[ClientVar]
		public static void print_prefabs(ConsoleSystem.Arg arg)
		{
			PrefabPoolCollection pool = GameManager.server.pool;
			if (pool.storage.Count == 0)
			{
				arg.ReplyWith("Prefab pool is empty.");
				return;
			}
			string @string = arg.GetString(0, string.Empty);
			TextTable textTable = new TextTable();
			textTable.AddColumn("id");
			textTable.AddColumn("name");
			textTable.AddColumn("count");
			foreach (KeyValuePair<uint, PrefabPool> keyValuePair in pool.storage)
			{
				string text = keyValuePair.Key.ToString();
				string text2 = StringPool.Get(keyValuePair.Key);
				string text3 = keyValuePair.Value.Count.ToString();
				if (string.IsNullOrEmpty(@string) || text2.Contains(@string, CompareOptions.IgnoreCase))
				{
					textTable.AddRow(new string[]
					{
						text,
						Path.GetFileNameWithoutExtension(text2),
						text3
					});
				}
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x06004096 RID: 16534 RVA: 0x0017CB48 File Offset: 0x0017AD48
		[ServerVar]
		[ClientVar]
		public static void print_assets(ConsoleSystem.Arg arg)
		{
			if (AssetPool.storage.Count == 0)
			{
				arg.ReplyWith("Asset pool is empty.");
				return;
			}
			string @string = arg.GetString(0, string.Empty);
			TextTable textTable = new TextTable();
			textTable.AddColumn("type");
			textTable.AddColumn("allocated");
			textTable.AddColumn("available");
			foreach (KeyValuePair<Type, AssetPool.Pool> keyValuePair in AssetPool.storage)
			{
				string text = keyValuePair.Key.ToString();
				string text2 = keyValuePair.Value.allocated.ToString();
				string text3 = keyValuePair.Value.available.ToString();
				if (string.IsNullOrEmpty(@string) || text.Contains(@string, CompareOptions.IgnoreCase))
				{
					textTable.AddRow(new string[]
					{
						text,
						text2,
						text3
					});
				}
			}
			arg.ReplyWith(arg.HasArg("--json") ? textTable.ToJson() : textTable.ToString());
		}

		// Token: 0x06004097 RID: 16535 RVA: 0x0017CC64 File Offset: 0x0017AE64
		[ServerVar]
		[ClientVar]
		public static void clear_memory(ConsoleSystem.Arg arg)
		{
			Pool.Clear();
		}

		// Token: 0x06004098 RID: 16536 RVA: 0x0017CC6B File Offset: 0x0017AE6B
		[ServerVar]
		[ClientVar]
		public static void clear_prefabs(ConsoleSystem.Arg arg)
		{
			GameManager.server.pool.Clear();
		}

		// Token: 0x06004099 RID: 16537 RVA: 0x0017CC7C File Offset: 0x0017AE7C
		[ServerVar]
		[ClientVar]
		public static void clear_assets(ConsoleSystem.Arg arg)
		{
			AssetPool.Clear();
		}

		// Token: 0x0600409A RID: 16538 RVA: 0x0017CC84 File Offset: 0x0017AE84
		[ServerVar]
		[ClientVar]
		public static void export_prefabs(ConsoleSystem.Arg arg)
		{
			PrefabPoolCollection pool = GameManager.server.pool;
			if (pool.storage.Count == 0)
			{
				arg.ReplyWith("Prefab pool is empty.");
				return;
			}
			string @string = arg.GetString(0, string.Empty);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<uint, PrefabPool> keyValuePair in pool.storage)
			{
				string arg2 = keyValuePair.Key.ToString();
				string text = StringPool.Get(keyValuePair.Key);
				string arg3 = keyValuePair.Value.Count.ToString();
				if (string.IsNullOrEmpty(@string) || text.Contains(@string, CompareOptions.IgnoreCase))
				{
					stringBuilder.AppendLine(string.Format("{0},{1},{2}", arg2, Path.GetFileNameWithoutExtension(text), arg3));
				}
			}
			File.WriteAllText("prefabs.csv", stringBuilder.ToString());
		}

		// Token: 0x04003983 RID: 14723
		[ServerVar]
		[ClientVar]
		public static int mode = 2;

		// Token: 0x04003984 RID: 14724
		[ServerVar]
		[ClientVar]
		public static bool prewarm = true;

		// Token: 0x04003985 RID: 14725
		[ServerVar]
		[ClientVar]
		public static bool enabled = true;

		// Token: 0x04003986 RID: 14726
		[ServerVar]
		[ClientVar]
		public static bool debug = false;
	}
}
