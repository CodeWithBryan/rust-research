using System;
using System.IO;

namespace ConVar
{
	// Token: 0x02000A94 RID: 2708
	[ConsoleSystem.Factory("profile")]
	public class Profile : ConsoleSystem
	{
		// Token: 0x0600409D RID: 16541 RVA: 0x0017CD9A File Offset: 0x0017AF9A
		private static void NeedProfileFolder()
		{
			if (!Directory.Exists("profile"))
			{
				Directory.CreateDirectory("profile");
			}
		}

		// Token: 0x0600409E RID: 16542 RVA: 0x000059DD File Offset: 0x00003BDD
		[ClientVar]
		[ServerVar]
		public static void start(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x0600409F RID: 16543 RVA: 0x000059DD File Offset: 0x00003BDD
		[ServerVar]
		[ClientVar]
		public static void stop(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x060040A0 RID: 16544 RVA: 0x000059DD File Offset: 0x00003BDD
		[ServerVar]
		[ClientVar]
		public static void flush_analytics(ConsoleSystem.Arg arg)
		{
		}
	}
}
