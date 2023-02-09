using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000A64 RID: 2660
	[ConsoleSystem.Factory("audio")]
	public class Audio : ConsoleSystem
	{
		// Token: 0x17000535 RID: 1333
		// (get) Token: 0x06003F54 RID: 16212 RVA: 0x00175C14 File Offset: 0x00173E14
		// (set) Token: 0x06003F55 RID: 16213 RVA: 0x00175C1C File Offset: 0x00173E1C
		[ClientVar(Help = "Volume", Saved = true)]
		public static int speakers
		{
			get
			{
				return (int)UnityEngine.AudioSettings.speakerMode;
			}
			set
			{
				value = Mathf.Clamp(value, 2, 7);
				if (!Application.isEditor)
				{
					AudioConfiguration configuration = UnityEngine.AudioSettings.GetConfiguration();
					configuration.speakerMode = (AudioSpeakerMode)value;
					using (TimeWarning.New("Audio Settings Reset", 250))
					{
						UnityEngine.AudioSettings.Reset(configuration);
					}
				}
			}
		}

		// Token: 0x06003F56 RID: 16214 RVA: 0x000059DD File Offset: 0x00003BDD
		[ClientVar]
		public static void printSounds(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x06003F57 RID: 16215 RVA: 0x000059DD File Offset: 0x00003BDD
		[ClientVar(ClientAdmin = true, Help = "print active engine sound info")]
		public static void printEngineSounds(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x040038E3 RID: 14563
		[ClientVar(Help = "Volume", Saved = true)]
		public static float master = 1f;

		// Token: 0x040038E4 RID: 14564
		[ClientVar(Help = "Volume", Saved = true)]
		public static float musicvolume = 1f;

		// Token: 0x040038E5 RID: 14565
		[ClientVar(Help = "Volume", Saved = true)]
		public static float musicvolumemenu = 1f;

		// Token: 0x040038E6 RID: 14566
		[ClientVar(Help = "Volume", Saved = true)]
		public static float game = 1f;

		// Token: 0x040038E7 RID: 14567
		[ClientVar(Help = "Volume", Saved = true)]
		public static float voices = 1f;

		// Token: 0x040038E8 RID: 14568
		[ClientVar(Help = "Volume", Saved = true)]
		public static float instruments = 1f;

		// Token: 0x040038E9 RID: 14569
		[ClientVar(Help = "Volume", Saved = true)]
		public static float voiceProps = 1f;

		// Token: 0x040038EA RID: 14570
		[ClientVar(Help = "Volume", Saved = true)]
		public static float eventAudio = 1f;

		// Token: 0x040038EB RID: 14571
		[ClientVar(Help = "Ambience System")]
		public static bool ambience = true;

		// Token: 0x040038EC RID: 14572
		[ClientVar(Help = "Max ms per frame to spend updating sounds")]
		public static float framebudget = 0.3f;

		// Token: 0x040038ED RID: 14573
		[ClientVar]
		public static float minupdatefraction = 0.1f;

		// Token: 0x040038EE RID: 14574
		[ClientVar(Help = "Use more advanced sound occlusion", Saved = true)]
		public static bool advancedocclusion = false;

		// Token: 0x040038EF RID: 14575
		[ClientVar(Help = "Use higher quality sound fades on some sounds")]
		public static bool hqsoundfade = false;

		// Token: 0x040038F0 RID: 14576
		[ClientVar(Saved = false)]
		public static bool debugVoiceLimiting = false;
	}
}
