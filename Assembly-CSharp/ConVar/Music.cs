using System;
using System.Text;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000A8B RID: 2699
	[ConsoleSystem.Factory("music")]
	public class Music : ConsoleSystem
	{
		// Token: 0x06004065 RID: 16485 RVA: 0x0017B848 File Offset: 0x00179A48
		[ClientVar]
		public static void info(ConsoleSystem.Arg arg)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (SingletonComponent<MusicManager>.Instance == null)
			{
				stringBuilder.Append("No music manager was found");
			}
			else
			{
				stringBuilder.Append("Current music info: ");
				stringBuilder.AppendLine();
				stringBuilder.Append("  theme: " + SingletonComponent<MusicManager>.Instance.currentTheme);
				stringBuilder.AppendLine();
				stringBuilder.Append("  intensity: " + SingletonComponent<MusicManager>.Instance.intensity);
				stringBuilder.AppendLine();
				stringBuilder.Append("  next music: " + SingletonComponent<MusicManager>.Instance.nextMusic);
				stringBuilder.AppendLine();
				stringBuilder.Append("  current time: " + Time.time);
				stringBuilder.AppendLine();
			}
			arg.ReplyWith(stringBuilder.ToString());
		}

		// Token: 0x04003972 RID: 14706
		[ClientVar]
		public static bool enabled = true;

		// Token: 0x04003973 RID: 14707
		[ClientVar]
		public static int songGapMin = 240;

		// Token: 0x04003974 RID: 14708
		[ClientVar]
		public static int songGapMax = 480;
	}
}
