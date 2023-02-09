using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Facepunch;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000A69 RID: 2665
	[ConsoleSystem.Factory("console")]
	public class Console : ConsoleSystem
	{
		// Token: 0x06003F6C RID: 16236 RVA: 0x001765A8 File Offset: 0x001747A8
		[ServerVar]
		[Help("Return the last x lines of the console. Default is 200")]
		public static IEnumerable<Output.Entry> tail(ConsoleSystem.Arg arg)
		{
			int @int = arg.GetInt(0, 200);
			int num = Output.HistoryOutput.Count - @int;
			if (num < 0)
			{
				num = 0;
			}
			return Output.HistoryOutput.Skip(num);
		}

		// Token: 0x06003F6D RID: 16237 RVA: 0x001765E0 File Offset: 0x001747E0
		[ServerVar]
		[Help("Search the console for a particular string")]
		public static IEnumerable<Output.Entry> search(ConsoleSystem.Arg arg)
		{
			string search = arg.GetString(0, null);
			if (search == null)
			{
				return Enumerable.Empty<Output.Entry>();
			}
			return from x in Output.HistoryOutput
			where x.Message.Length < 4096 && x.Message.Contains(search, CompareOptions.IgnoreCase)
			select x;
		}
	}
}
