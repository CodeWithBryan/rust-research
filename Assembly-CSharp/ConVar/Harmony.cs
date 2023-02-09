using System;

namespace ConVar
{
	// Token: 0x02000A7F RID: 2687
	[ConsoleSystem.Factory("harmony")]
	public class Harmony : ConsoleSystem
	{
		// Token: 0x0600402E RID: 16430 RVA: 0x00179CF9 File Offset: 0x00177EF9
		[ServerVar(Name = "load")]
		public static void Load(ConsoleSystem.Arg args)
		{
			HarmonyLoader.TryLoadMod(args.GetString(0, ""));
		}

		// Token: 0x0600402F RID: 16431 RVA: 0x00179D0D File Offset: 0x00177F0D
		[ServerVar(Name = "unload")]
		public static void Unload(ConsoleSystem.Arg args)
		{
			HarmonyLoader.TryUnloadMod(args.GetString(0, ""));
		}
	}
}
