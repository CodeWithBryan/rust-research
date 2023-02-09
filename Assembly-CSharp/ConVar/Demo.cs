using System;
using System.IO;
using Network;
using ProtoBuf;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000A72 RID: 2674
	[ConsoleSystem.Factory("demo")]
	public class Demo : ConsoleSystem
	{
		// Token: 0x17000537 RID: 1335
		// (get) Token: 0x06003F95 RID: 16277 RVA: 0x0017749A File Offset: 0x0017569A
		// (set) Token: 0x06003F96 RID: 16278 RVA: 0x001774A1 File Offset: 0x001756A1
		[ServerVar(Saved = true, Help = "Controls the behavior of recordlist, 0=whitelist, 1=blacklist")]
		public static int recordlistmode
		{
			get
			{
				return Demo._recordListModeValue;
			}
			set
			{
				Demo._recordListModeValue = Mathf.Clamp(value, 0, 1);
			}
		}

		// Token: 0x06003F97 RID: 16279 RVA: 0x001774B0 File Offset: 0x001756B0
		[ServerVar]
		public static string record(ConsoleSystem.Arg arg)
		{
			global::BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			if (!playerOrSleeper || playerOrSleeper.net == null || playerOrSleeper.net.connection == null)
			{
				return "Player not found";
			}
			if (playerOrSleeper.net.connection.IsRecording)
			{
				return "Player already recording a demo";
			}
			playerOrSleeper.StartDemoRecording();
			return null;
		}

		// Token: 0x06003F98 RID: 16280 RVA: 0x00177508 File Offset: 0x00175708
		[ServerVar]
		public static string stop(ConsoleSystem.Arg arg)
		{
			global::BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			if (!playerOrSleeper || playerOrSleeper.net == null || playerOrSleeper.net.connection == null)
			{
				return "Player not found";
			}
			if (!playerOrSleeper.net.connection.IsRecording)
			{
				return "Player not recording a demo";
			}
			playerOrSleeper.StopDemoRecording();
			return null;
		}

		// Token: 0x04003926 RID: 14630
		public static uint Version = 3U;

		// Token: 0x04003927 RID: 14631
		[ServerVar]
		public static float splitseconds = 3600f;

		// Token: 0x04003928 RID: 14632
		[ServerVar]
		public static float splitmegabytes = 200f;

		// Token: 0x04003929 RID: 14633
		[ServerVar(Saved = true)]
		public static string recordlist = "";

		// Token: 0x0400392A RID: 14634
		private static int _recordListModeValue = 0;

		// Token: 0x02000EED RID: 3821
		public class Header : DemoHeader, IDemoHeader
		{
			// Token: 0x170006BA RID: 1722
			// (get) Token: 0x0600518D RID: 20877 RVA: 0x001A4DD3 File Offset: 0x001A2FD3
			// (set) Token: 0x0600518E RID: 20878 RVA: 0x001A4DDB File Offset: 0x001A2FDB
			long IDemoHeader.Length
			{
				get
				{
					return this.length;
				}
				set
				{
					this.length = value;
				}
			}

			// Token: 0x0600518F RID: 20879 RVA: 0x001A4DE4 File Offset: 0x001A2FE4
			public void Write(BinaryWriter writer)
			{
				byte[] array = base.ToProtoBytes();
				writer.Write("RUST DEMO FORMAT");
				writer.Write(array.Length);
				writer.Write(array);
				writer.Write('\0');
			}
		}
	}
}
