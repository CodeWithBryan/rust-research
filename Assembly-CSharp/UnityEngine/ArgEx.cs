using System;

namespace UnityEngine
{
	// Token: 0x020009DC RID: 2524
	public static class ArgEx
	{
		// Token: 0x06003B82 RID: 15234 RVA: 0x0015BC5A File Offset: 0x00159E5A
		public static BasePlayer Player(this ConsoleSystem.Arg arg)
		{
			if (arg == null || arg.Connection == null)
			{
				return null;
			}
			return arg.Connection.player as BasePlayer;
		}

		// Token: 0x06003B83 RID: 15235 RVA: 0x0015BC7C File Offset: 0x00159E7C
		public static BasePlayer GetPlayer(this ConsoleSystem.Arg arg, int iArgNum)
		{
			string @string = arg.GetString(iArgNum, null);
			if (@string == null)
			{
				return null;
			}
			return BasePlayer.Find(@string);
		}

		// Token: 0x06003B84 RID: 15236 RVA: 0x0015BCA0 File Offset: 0x00159EA0
		public static BasePlayer GetSleeper(this ConsoleSystem.Arg arg, int iArgNum)
		{
			string @string = arg.GetString(iArgNum, "");
			if (@string == null)
			{
				return null;
			}
			return BasePlayer.FindSleeping(@string);
		}

		// Token: 0x06003B85 RID: 15237 RVA: 0x0015BCC8 File Offset: 0x00159EC8
		public static BasePlayer GetPlayerOrSleeper(this ConsoleSystem.Arg arg, int iArgNum)
		{
			string @string = arg.GetString(iArgNum, "");
			if (@string == null)
			{
				return null;
			}
			return BasePlayer.FindAwakeOrSleeping(@string);
		}

		// Token: 0x06003B86 RID: 15238 RVA: 0x0015BCF0 File Offset: 0x00159EF0
		public static BasePlayer GetPlayerOrSleeperOrBot(this ConsoleSystem.Arg arg, int iArgNum)
		{
			uint num;
			if (arg.TryGetUInt(iArgNum, out num))
			{
				return BasePlayer.FindBot((ulong)num);
			}
			return arg.GetPlayerOrSleeper(iArgNum);
		}
	}
}
