using System;
using Rust;
using UnityEngine;
using UnityEngine.Scripting;

namespace ConVar
{
	// Token: 0x02000A7A RID: 2682
	[ConsoleSystem.Factory("gc")]
	public class GC : ConsoleSystem
	{
		// Token: 0x17000542 RID: 1346
		// (get) Token: 0x06003FCD RID: 16333 RVA: 0x00178694 File Offset: 0x00176894
		// (set) Token: 0x06003FCE RID: 16334 RVA: 0x0017869B File Offset: 0x0017689B
		[ClientVar]
		public static int buffer
		{
			get
			{
				return ConVar.GC.m_buffer;
			}
			set
			{
				ConVar.GC.m_buffer = Mathf.Clamp(value, 64, 4096);
			}
		}

		// Token: 0x17000543 RID: 1347
		// (get) Token: 0x06003FCF RID: 16335 RVA: 0x001786AF File Offset: 0x001768AF
		// (set) Token: 0x06003FD0 RID: 16336 RVA: 0x001786B6 File Offset: 0x001768B6
		[ServerVar]
		[ClientVar]
		public static bool incremental_enabled
		{
			get
			{
				return GarbageCollector.isIncremental;
			}
			set
			{
				Debug.LogWarning("Cannot set gc.incremental as it is read only");
			}
		}

		// Token: 0x17000544 RID: 1348
		// (get) Token: 0x06003FD1 RID: 16337 RVA: 0x001786C2 File Offset: 0x001768C2
		// (set) Token: 0x06003FD2 RID: 16338 RVA: 0x001786D1 File Offset: 0x001768D1
		[ServerVar]
		[ClientVar]
		public static int incremental_milliseconds
		{
			get
			{
				return (int)(GarbageCollector.incrementalTimeSliceNanoseconds / 1000000UL);
			}
			set
			{
				GarbageCollector.incrementalTimeSliceNanoseconds = (ulong)(1000000L * (long)Mathf.Max(value, 0));
			}
		}

		// Token: 0x17000545 RID: 1349
		// (get) Token: 0x06003FD3 RID: 16339 RVA: 0x001786E7 File Offset: 0x001768E7
		// (set) Token: 0x06003FD4 RID: 16340 RVA: 0x001786EE File Offset: 0x001768EE
		[ServerVar]
		[ClientVar]
		public static bool enabled
		{
			get
			{
				return Rust.GC.Enabled;
			}
			set
			{
				Debug.LogWarning("Cannot set gc.enabled as it is read only");
			}
		}

		// Token: 0x06003FD5 RID: 16341 RVA: 0x001786FA File Offset: 0x001768FA
		[ServerVar]
		[ClientVar]
		public static void collect()
		{
			Rust.GC.Collect();
		}

		// Token: 0x06003FD6 RID: 16342 RVA: 0x00178701 File Offset: 0x00176901
		[ServerVar]
		[ClientVar]
		public static void unload()
		{
			Resources.UnloadUnusedAssets();
		}

		// Token: 0x06003FD7 RID: 16343 RVA: 0x0017870C File Offset: 0x0017690C
		[ServerVar]
		[ClientVar]
		public static void alloc(ConsoleSystem.Arg args)
		{
			byte[] array = new byte[args.GetInt(0, 1048576)];
			args.ReplyWith("Allocated " + array.Length + " bytes");
		}

		// Token: 0x0400392D RID: 14637
		[ClientVar]
		public static bool buffer_enabled = true;

		// Token: 0x0400392E RID: 14638
		[ClientVar]
		public static int debuglevel = 1;

		// Token: 0x0400392F RID: 14639
		private static int m_buffer = 256;
	}
}
