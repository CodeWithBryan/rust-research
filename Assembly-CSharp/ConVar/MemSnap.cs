using System;
using System.IO;
using UnityEngine.Profiling.Memory.Experimental;

namespace ConVar
{
	// Token: 0x02000A89 RID: 2697
	[ConsoleSystem.Factory("memsnap")]
	public class MemSnap : ConsoleSystem
	{
		// Token: 0x0600405F RID: 16479 RVA: 0x0017B760 File Offset: 0x00179960
		private static string NeedProfileFolder()
		{
			string path = "profile";
			if (!Directory.Exists(path))
			{
				return Directory.CreateDirectory(path).FullName;
			}
			return new DirectoryInfo(path).FullName;
		}

		// Token: 0x06004060 RID: 16480 RVA: 0x0017B794 File Offset: 0x00179994
		[ClientVar]
		[ServerVar]
		public static void managed(ConsoleSystem.Arg arg)
		{
			MemoryProfiler.TakeSnapshot(MemSnap.NeedProfileFolder() + "/memdump-" + DateTime.Now.ToString("MM-dd-yyyy-h-mm-ss") + ".snap", null, CaptureFlags.ManagedObjects);
		}

		// Token: 0x06004061 RID: 16481 RVA: 0x0017B7D0 File Offset: 0x001799D0
		[ClientVar]
		[ServerVar]
		public static void native(ConsoleSystem.Arg arg)
		{
			MemoryProfiler.TakeSnapshot(MemSnap.NeedProfileFolder() + "/memdump-" + DateTime.Now.ToString("MM-dd-yyyy-h-mm-ss") + ".snap", null, CaptureFlags.NativeObjects);
		}

		// Token: 0x06004062 RID: 16482 RVA: 0x0017B80C File Offset: 0x00179A0C
		[ClientVar]
		[ServerVar]
		public static void full(ConsoleSystem.Arg arg)
		{
			MemoryProfiler.TakeSnapshot(MemSnap.NeedProfileFolder() + "/memdump-" + DateTime.Now.ToString("MM-dd-yyyy-h-mm-ss") + ".snap", null, CaptureFlags.ManagedObjects | CaptureFlags.NativeObjects | CaptureFlags.NativeAllocations | CaptureFlags.NativeAllocationSites | CaptureFlags.NativeStackTraces);
		}
	}
}
